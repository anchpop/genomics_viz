using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Util;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Assertions;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.EventSystems;
using CapnpGen;
using Capnp;

using Segment = OneOf.OneOf<
    CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>,
    CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>>;
using SegmentList =
    OneOf.OneOf<
        System.Collections.Generic.List<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>>,
        System.Collections.Generic.List<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>>
    >;
using SegmentInfo = System.Collections.Generic.Dictionary<
    string,
    (
        OneOf.OneOf<
            System.Collections.Generic.List<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>>,
            System.Collections.Generic.List<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>>> segments,
        Supercluster.KDTree.KDTree<float, int> worldPositions,
    KTrie.StringTrie<int> nameDict)
>;

public class CameraController : MonoBehaviour
{
    public ChromosomeController chromosome;
    public ChromosomeParentController parentController;
    public TextMeshProUGUI searchInput;


    public TextMeshProUGUI sideText;
    public TextMeshProUGUI sideLoc;

    public GameObject OneDViewCanvas;
    public GameObject GeneInfoCanvas;
    public GameObject ButtonCanvas;
    public TextMeshProUGUI scaleText;
    public List<TextMeshProUGUI> texts;

    public GraphicRaycaster OneDViewGraphicRaycaster;
    public GraphicRaycaster ButtonViewGraphicRaycaster;
    public UnityEngine.EventSystems.EventSystem m_EventSystem;

    private int horizontalTextChars = 144;

    private int baseScale = 20000;
    public Slider slider;
    private int currentCenter = 0;
    public static (int center, (int startIndex, SegmentList segmentList) displayed) OneDView;
    private bool OneDViewFocused = false;

    public GameObject selectionIndicator;

    public GameObject labelPrefab;
    public Transform geneLabelsParent;

    List<GameObject> geneLabels;

    bool canvases_setup = false;

    public string focusedSegmentSet = "genes";
    public int focusedSegmentIndex = 0;

    void Start()
    {
        createLabels();

        OneDView = (0, (0, new List<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>>()));
        foreach (var t in texts)
        {
            t.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetupCanvases();
        ShowLabels();
        Tween1DView();


        var mouse = Mouse.current;
        var keyboard = Keyboard.current;

        if (keyboard.enterKey.wasPressedThisFrame)
        {
            var search = String.Concat(searchInput.text.ToUpper().Where(c => Char.IsLetterOrDigit(c) || c == '-'));

            int basePairSearch = 0;
            if (int.TryParse(search, out basePairSearch))
            {
                parentController.goToBin(basePairSearch);
            }
            else
            {
                string[] parts = search.Split('-');
                if (parts.Length == 2)
                {
                    int basePairSearch0 = 0;
                    int basePairSearch1 = 0;
                    if (int.TryParse(parts[0], out basePairSearch0) && int.TryParse(parts[1], out basePairSearch1))
                    {
                        parentController.goToBin(basePairSearch0 / 2 + basePairSearch1 / 2);
                    }
                }
                else
                {
                    IEnumerable<(string segmentSetName, Segment segment)> results = ChromosomeController.chromosomeRenderingInfo.segmentInfos.SelectMany(segmentInfo =>
                        segmentInfo.Value.nameDict.GetByPrefix(search).Select(entry =>
                            (segmentSetName: segmentInfo.Key,
                             segment: segmentInfo.Value.segments.Match<Segment>(x => x[entry.Value], x => x[entry.Value]))));

                    if (results.Any())
                    {
                        var (segmentSetName, segment) = results.First();
                        /*
                         * TODO: uncomment
                         * 
                        chromosome.focusSegment((segmentSetName, segment));
                        parentController.focusSegment((segmentSetName, segment));
                        */
                    }
                    else
                    {
                        Debug.Log("'" + search + "' (" + search.Length.ToString() + ") not found. ");
                    }
                }
            }

        }
    }

    public void SetupCanvases()
    {
        if (!canvases_setup)
        {
            if (parentController.inVr)
            {
                OneDViewCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
                OneDViewCanvas.transform.localScale = new Vector3(0.0005f, 0.0005f, 1);
                OneDViewCanvas.transform.SetParent(parentController.leftController.transform);
                OneDViewCanvas.transform.localPosition = new Vector3(0, -0.05f, 0);
                OneDViewCanvas.transform.eulerAngles = new Vector3(40, 0, 0);

                GeneInfoCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
                GeneInfoCanvas.transform.localScale = new Vector3(0.0007f, 0.0007f, 1);
                GeneInfoCanvas.transform.SetParent(parentController.rightController.transform);
                GeneInfoCanvas.transform.localPosition = new Vector3(0.1f, -0.05f, 0);
                GeneInfoCanvas.transform.eulerAngles = new Vector3(40, 0, 0);
            }
            else
            {
                OneDViewCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                GeneInfoCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                ButtonCanvas.transform.SetParent(OneDViewCanvas.transform);
            }

            canvases_setup = true;
        }
    }

    public Vector3? highlightHit(Ray ray, bool focus)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.GetComponent<BackboneMarker>())
            {
                var hitLoc = chromosome.transform.InverseTransformPoint(hit.point);
                var pointIndices = chromosome.localPositionToBackbonePointIndex(hitLoc);
                var p1 = ChromosomeController.chromosomeRenderingInfo.backbonePoints[pointIndices.closest];
                var p2 = ChromosomeController.chromosomeRenderingInfo.backbonePoints[pointIndices.nextClosest];

                var cursorPoint = hitLoc.GetClosestPointOnInfiniteLine(p1.position, p2.position);
                var cursorDistance = Vector3Utils.InverseLerp(p1.position, p2.position, cursorPoint);
                var cursorBasePair = (int)Mathf.Lerp(p1.bin, p2.bin, cursorDistance);

                selectionIndicator.transform.localPosition = chromosome.binToPosition(cursorBasePair);

                var segments = chromosome.getSegmentsAtBin(ChromosomeController.chromosomeRenderingInfo.segmentInfos, cursorBasePair);

                // Don't have a principled way to do this, so I'll just pick the first gene to display
                if (segments.Any())
                {
                    var segmentSet = segments.First();
                    var segmentIndex = segmentSet.Value.First();
                    var segment = ChromosomeController.GetSegmentFromCurrentChromosome(segmentSet.Key, segmentIndex);
                    chromosome.highlightSegment(segmentSet.Key, ChromosomeController.GetSegmentLocation(segment));
                    segment.Switch(gene =>
                    {
                        sideText.text = gene.ExtraInfo.Name;
                        sideLoc.text = cursorBasePair.ToString("D");

                        Debug.Log(gene.Location.Lower + " <= " + cursorBasePair + " <= " + gene.Location.Upper);

                        if (focus)
                        {
                            focusSegment(focusedSegmentSet, segmentIndex);
                        }

                    }, segment => { Debug.LogError("Only support genes right now!"); });
                }
                return hit.point;
            }
        }
        else
        {
            chromosome.unhighlightGene();
        }


        return null;
    }


    public void focusSegment(string segmentSet, int index)
    {
        focusedSegmentIndex = index;
        focusedSegmentSet = segmentSet;
        chromosome.highlightSegment(chromosome.focusRenderer,
            ChromosomeController.chromosomeRenderingInfo.segmentInfos[segmentSet].segments.Match(
                segments => ChromosomeController.GetSegmentLocation(segments[index]),
                segments => ChromosomeController.GetSegmentLocation(segments[index])));

        parentController.goToSegment(segmentSet, index);
    }

    public void unfocusSegment()
    {
        focusedSegmentIndex = 0;
        chromosome.focusRenderer.mesh.Clear();
    }

    private void createLabels()
    {
        geneLabels = new List<GameObject>();
        foreach (int i in Enumerable.Range(0, 30))
        {
            var label = Instantiate(labelPrefab, geneLabelsParent);
            geneLabels.Add(label);
        }
    }
    private void ShowLabels()
    {
        var localSpacePos = chromosome.transform.InverseTransformPoint(transform.position + .1f * transform.forward);
        var genesToShow = ChromosomeController.chromosomeRenderingInfo.segmentInfos.SelectMany(info => info.Value.worldPositions.NearestNeighbors(
            new float[] { localSpacePos.x, localSpacePos.y, localSpacePos.z },
            geneLabels.Count
            ).Select((node) =>
                (new Vector3(node.Item1[0], node.Item1[1], node.Item1[2]), info.Value.segments.Match(x => x[node.Item2].ExtraInfo.Name, x => x[node.Item2].ExtraInfo.Info))));


        foreach (var ((position, name), index) in genesToShow.Select((x, i) => (x, i)))
        {
            var label = geneLabels[index];
            label.transform.localPosition = position;
            label.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
            label.transform.LookAt(label.transform.position + -(transform.position - label.transform.position), transform.up);
        }
        /*
         * todo: uncomment
        */
    }

    public void Tween1DView()
    {
        var buffer = getScale() * horizontalTextChars * 2;
        currentCenter = (int)Mathf.Clamp(Mathf.Lerp(currentCenter, OneDView.center, 2 * Time.deltaTime), OneDView.center - buffer, OneDView.center + buffer);
        Refresh1DView();
    }

    public void clicked_on_1D(float pos)
    {
        if (OneDViewFocused)
        {
            pos = (pos - .5f) * horizontalTextChars;
            var basePair = Mathf.RoundToInt(pos * getScale() + OneDView.center);
            parentController.goToBin(basePair);
        }
    }

    // TODO: refactor this and Update1DViewBasePairIndex, they have a lot of duplicate, messy code
    public void Update1DViewSegment(int segmentIndex)
    {
        Update1DViewSegment(focusedSegmentSet, segmentIndex);
    }
    public void Update1DViewSegment(string segmentSet, int segmentIndex)
    {
        focusedSegmentSet = segmentSet;
        ChromosomeController.chromosomeRenderingInfo.segmentInfos[segmentSet].segments.Switch(segmentSet =>
        {
            var info = segmentSet[segmentIndex];

            var scale = slider.value * baseScale;

            var adjecentsToCheck = 30;

            var toDisplay = new List<Chromosome.SegmentSet.Segment<Chromosome.SegmentSet.Gene>>();

            var numberOfGenes = segmentSet.Count;

            var startIndex = int.MaxValue;

            for (int i = Mathf.Max((segmentIndex - adjecentsToCheck / 2), 0); i < Mathf.Min((segmentIndex + adjecentsToCheck / 2), numberOfGenes); i++)
            {
                if (i < startIndex)
                {
                    startIndex = i;
                }
                toDisplay.Add(segmentSet[i]);
            }

            var bin = (int)info.Location.Lower / 2 + (int)info.Location.Upper / 2;
            OneDView = (bin, (startIndex, toDisplay));
        }
        , segmentSet => Debug.LogError("ONly genes supported right now!"));
    }

    public void Update1DViewBasePairIndex(int bin)
    {
        ChromosomeController.chromosomeRenderingInfo.segmentInfos[focusedSegmentSet].segments.Switch(segmentSet =>
        {
            var closestGeneIndex = 0;
            var closestGeneDistance = float.MaxValue;
            var numberOfGenes = segmentSet.Count;

            for (int i = 0; i < numberOfGenes; i++)
            {
                var info = segmentSet[i];
                var distance = (info.Location.Lower < bin && info.Location.Upper > bin) ? 0 : Mathf.Min(Mathf.Abs(bin - info.Location.Lower), Mathf.Abs(bin - info.Location.Upper));
                if (distance < closestGeneDistance)
                {
                    closestGeneIndex = i;
                    closestGeneDistance = distance;
                }
            }

            var scale = slider.value * baseScale;

            var adjecentsToCheck = 30;

            var toDisplay = new List<Chromosome.SegmentSet.Segment<Chromosome.SegmentSet.Gene>>();

            var startIndex = int.MaxValue;

            for (int i = Mathf.Max((closestGeneIndex - adjecentsToCheck / 2), 0); i < Mathf.Min((closestGeneIndex + adjecentsToCheck / 2), numberOfGenes); i++)
            {
                if (i < startIndex)
                {
                    startIndex = i;
                }
                toDisplay.Add(segmentSet[i]);
            }

            OneDView = (bin, (startIndex, toDisplay));
        }
        , segmentSet => Debug.LogError("ONly genes supported right now!"));
    }


    public void Refresh1DView()
    {
        Update1DView(currentCenter, OneDView.displayed);
    }

    public float getScale()
    {
        return slider.value * baseScale;
    }
    public void Update1DView(int center, (int startIndex, SegmentList segmentList) displayed)
    {
        var scale = getScale();
        OneDViewFocused = true;

        //var legendSize = 5;

        for (int i = 0; i < 20; i++)
        {
            var dist = (int)(5000 * Mathf.Pow(2, i));
            var distS = (dist / 1000).ToString() + "K";
            var legendS = (int)(dist / scale);
            if (10 <= legendS && legendS <= 50)
            {
                scaleText.text = "|" + new String('-', (legendS - distS.Length) / 2) + distS + new String('-', (legendS - distS.Length) / 2) + "|";
            }
        }

        // scaleText.text = (-Mathf.RoundToInt(scale) * legendSize / 2).ToString() + "|" + new String('-', legendSize) + "|" + (Mathf.RoundToInt(scale) * legendSize / 2).ToString();

        // OneDView = (center: geneStart / 2 + geneEnd / 2, scale: scale, new List<(string geneName, int geneStart, int geneEnd)> { (geneName, geneStart, geneEnd) });

        foreach (var t in texts)
        {
            t.text = "." + new String(' ', horizontalTextChars - 2) + ".";
        }


        var left = center - scale * (horizontalTextChars / 2);
        var right = center + scale * (horizontalTextChars / 2);

        var reservedAreas = new List<List<(int start, int end)>> { new List<(int start, int end)>(), new List<(int start, int end)>(), new List<(int start, int end)>(), new List<(int start, int end)>(), new List<(int start, int end)>(), };

        IEnumerable<(int index, (string name, char dash, Chromosome.BinRange segmentInfo) info)> renderList =
            displayed.segmentList.Match(
                genes => genes.Select(gene => (gene.ExtraInfo.Name, gene.ExtraInfo.Ascending ? '>' : '<', gene.Location)),
                others => others.Select(other => (other.ExtraInfo.Info, '-', other.Location))).Select((info, index) => (index + displayed.startIndex, info));

        // We want to treat the focused gene specially - writing the coordinates in the bottom and putting it in the center
        var focused = (from segment in renderList
                       where segment.index == focusedSegmentIndex
                       select segment.info).ToList();
        if (focused.Any())
        {
            var (name, dirchar, info) = focused.First();

            var length = Mathf.RoundToInt((info.Upper - info.Lower) / scale);
            var startPos = Mathf.RoundToInt(InvLerp(left, right, info.Lower) * horizontalTextChars);

            if (name.Length + 2 > length)
            {
                var geneBar = "|" + new String(dirchar, length) + "|";
                var genePosMarkers = info.Lower.ToString().PadRight(length) + info.Upper;
                var genePosMarkersStartPos = startPos - (genePosMarkers.Length - geneBar.Length) / 2;

                texts[2].text = updateSubportion(texts[2].text, startPos, geneBar);
                texts[3].text = updateSubportion(texts[3].text, genePosMarkersStartPos, genePosMarkers);

                reservedAreas[2].Add((startPos, startPos + geneBar.Length));
                reservedAreas[3].Add((genePosMarkersStartPos, genePosMarkersStartPos + genePosMarkers.Length));
            }
            else
            {
                var length1 = Mathf.RoundToInt(-.001f + (length - name.Length) / 2.0f);
                var length2 = Mathf.RoundToInt(.001f + (length - name.Length) / 2.0f);


                var geneBar = "|" + new String(dirchar, length1) + name + new String(dirchar, length2) + "|";
                var genePosMarkers = info.Lower.ToString().PadRight(length) + "  " + info.Upper;


                var genePosMarkersStartPos = startPos - (genePosMarkers.Length - geneBar.Length) / 2;

                texts[2].text = updateSubportion(texts[2].text, startPos, geneBar);
                texts[3].text = updateSubportion(texts[3].text, genePosMarkersStartPos, genePosMarkers);

                reservedAreas[2].Add((startPos, startPos + geneBar.Length));
                reservedAreas[3].Add((genePosMarkersStartPos, genePosMarkersStartPos + genePosMarkers.Length));
            }
        }


        // now we handle the rest of the genes, and decide where to write them by checking the reservedareas 
        foreach (var (index, (name, dirchar, info)) in renderList)
        {
            var length = Mathf.RoundToInt((info.Upper - info.Lower) / scale);
            var startPos = Mathf.RoundToInt(InvLerp(left, right, info.Lower) * horizontalTextChars);

            if (index != focusedSegmentIndex && length > 1)
            {
                var geneBar = "";
                if (name.Length + 2 > length)
                {
                    geneBar = "|" + new String(dirchar, length) + "|";
                }
                else
                {
                    var length1 = Mathf.RoundToInt(-.001f + (length - name.Length) / 2.0f);
                    var length2 = Mathf.RoundToInt(.001f + (length - name.Length) / 2.0f);
                    geneBar = "|" + new String(dirchar, length1) + name + new String(dirchar, length2) + "|";
                }

                var allowed = Enumerable.Repeat(true, reservedAreas.Count).ToList();
                for (int i = 0; i < allowed.Count; i++)
                {
                    foreach (var (reserveStart, reserveEnd) in reservedAreas[i])
                    {
                        if (reserveStart <= startPos && (startPos + geneBar.Length) <= reserveEnd) // check if ranges [reserveStart, reserveEnd], [startPos, startPos + geneBar.Length] overlap
                        {
                            allowed[i] = false;
                            break;
                        }
                    }
                }

                for (int i = 0; i < allowed.Count; i++)
                {
                    var v = allowed.Count - i - 1; // invert order to prefer the bottom row for flanking regions
                    if (allowed[v])
                    {
                        texts[v].text = updateSubportion(texts[v].text, startPos, geneBar);
                        reservedAreas[v].Add((startPos, startPos + geneBar.Length));
                        break;
                    }
                }
            }
        }


    }


    public float InvLerp(float a, float b, float v)
    {
        return (v - a) / (b - a);
    }

    public String updateSubportion(String original, int startIndex, String sub)
    {
        if (startIndex >= original.Length)
        {
            return original;
        }
        if (startIndex < 0)
        {
            if (-startIndex >= sub.Length)
            {
                return original;
            }
            var f = sub.Substring(-startIndex, sub.Length - (-startIndex));
            if (sub.Length - (-startIndex) > original.Length)
            {
                return f;
            }
            var l = original.Substring(sub.Length - (-startIndex), original.Length - (sub.Length - (-startIndex)));
            return f + l;
        }
        var output = original.Substring(0, startIndex) + ((sub.Length > original.Length - startIndex) ? sub.Substring(0, original.Length - startIndex) : (sub + original.Substring(startIndex + sub.Length, original.Length - (startIndex + sub.Length))));
        Assert.AreEqual(original.Length, output.Length);
        return output;
    }
}
