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

public class CameraController : MonoBehaviour
{
    public ChromosomeController chromosome;
    public CameraParentController parentController;
    public TextMeshProUGUI searchInput;


    public TextMeshProUGUI sideText;
    public TextMeshProUGUI sideLoc;

    public List<TextMeshProUGUI> texts;

    public GraphicRaycaster GraphicRaycaster;
    public UnityEngine.EventSystems.EventSystem m_EventSystem;

    private int horizontalTextChars = 144;

    private int baseScale = 20000;
    public Slider slider;
    private int currentCenter = 0;
    public static (int center, List<(string geneName, int geneStart, int geneEnd)> displayed) OneDView;
    private bool OneDViewFocused = false;

    string lastLit = "";
    void Start()
    {
        OneDView = (0, new List<(string geneName, int geneStart, int geneEnd)>());
        foreach (var t in texts)
        {
            t.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        Tween1DView();



        RaycastHit hit;
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;

        if (keyboard.enterKey.wasPressedThisFrame)
        {
            var search = String.Concat(searchInput.text.ToUpper().Where(c => Char.IsLetterOrDigit(c) || c == '-'));

            int basePairSearch = 0;
            if (int.TryParse(search, out basePairSearch))
            {
                parentController.goToBasePairIndex(basePairSearch);
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
                        parentController.goToBasePairIndex(basePairSearch0 / 2 + basePairSearch1 / 2);
                    }
                }
                else
                {
                    var results = chromosome.geneDict.GetByPrefix(search);
                    foreach (var result in results)
                    {
                        Debug.Log(result);
                    }
                    if (chromosome.geneDict.ContainsKey(search))
                    {
                        chromosome.focusGene(search);
                        parentController.goToGene(search);
                    }
                    else
                    {
                        Debug.Log("'" + search + "' (" + search.Length.ToString() + ") not found. ");
                    }
                }
            }

        }
        else
        {
            List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
            //Set up the new Pointer Event
            var m_PointerEventData = new UnityEngine.EventSystems.PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = mouse.position.ReadValue();
            //Raycast using the Graphics Raycaster and mouse click position
            GraphicRaycaster.Raycast(m_PointerEventData, results);

            if (results.Count <= 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
                if (Physics.Raycast(ray, out hit))
                {
                    GeneController gene = hit.collider.gameObject.GetComponent<GeneController>();
                    if (gene)
                    {
                        chromosome.unhighlightGene(lastLit);
                        lastLit = gene.geneName;
                        chromosome.highlightGene(gene.geneName);

                        var cursorPoint = hit.point.GetClosestPointOnInfiniteLine(gene.startPoint, gene.endPoint);
                        var cursorDistance = Vector3Utils.InverseLerp(gene.startPoint, gene.endPoint, cursorPoint);
                        var cursorBasePair = (int)Mathf.Lerp(gene.segmentStart, gene.segmentEnd, cursorDistance);


                        sideText.text = gene.geneName;
                        sideLoc.text = cursorBasePair.ToString("D");

                        if (mouse.leftButton.wasPressedThisFrame)
                        {
                            chromosome.focusGene(gene.geneName);
                            parentController.goToGene(gene.geneName);
                        }
                    }
                }
                else
                {
                    chromosome.unhighlightGene(lastLit);
                    lastLit = "";
                }

            }
        }

        if (chromosome.focusedGene != "")
        {
            if (keyboard.qKey.wasPressedThisFrame)
            {
                var geneInfo = chromosome.geneDict[chromosome.focusedGene];
                var nextGene = ChromosomeController.genes[geneInfo.index + 1];

                chromosome.focusGene(nextGene.name);
                parentController.goToGene(nextGene.name);
            }

            if (keyboard.gKey.wasPressedThisFrame)
            {
                openGeneInfoOnline();
            }
        }


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
            parentController.goToBasePairIndex(basePair);
        }
    }

    public void openGeneInfoOnline()
    {
        var geneInfo = chromosome.geneDict[chromosome.focusedGene];
        Application.OpenURL(
            "http://genome.ucsc.edu/cgi-bin/hgTracks?db=hg19&lastVirtModeType=default&lastVirtModeExtraState=&virtModeType=default&virtMode=0&nonVirtPosition=&position=chr1%3A"
            + geneInfo.start.ToString("D")
            + "%2D"
            + geneInfo.end.ToString("D")
            + "&hgsid=908127743_HmMER1nPkAhvlmaDlkaob9Vh99Va");
    }

    public void Update1DViewGene(string geneName)
    {
        var info = chromosome.geneDict[geneName];
        var scale = slider.value * baseScale;

        var adjecentsToCheck = 30;

        var toDisplay = new List<(string geneName, int geneStart, int geneEnd)>();

        var numberOfGenes = ChromosomeController.genes.Count;
        for (int i = Mathf.Max((info.index - adjecentsToCheck / 2), 0); i < Mathf.Min((info.index + adjecentsToCheck / 2), numberOfGenes); i++)
        {
            toDisplay.Add(ChromosomeController.genes[i]);
        }

        OneDView = (info.start / 2 + info.end / 2, toDisplay);
    }

    public void Update1DViewBasePairIndex(int bpindex)
    {
        var closestGeneIndex = 0;
        var closestGeneDistance = 100000000000;
        var numberOfGenes = ChromosomeController.genes.Count;

        for (int i = 0; i < numberOfGenes; i++)
        {
            var info = ChromosomeController.genes[i];
            var distance = (info.start < bpindex && info.end > bpindex) ? 0 : Mathf.Min(Mathf.Abs(bpindex - info.start), Mathf.Abs(bpindex - info.end));
            if (distance < closestGeneDistance)
            {
                closestGeneIndex = i;
                closestGeneDistance = distance;
            }
        }

        var scale = slider.value * baseScale;

        var adjecentsToCheck = 30;

        var toDisplay = new List<(string geneName, int geneStart, int geneEnd)>();

        for (int i = Mathf.Max((closestGeneIndex - adjecentsToCheck / 2), 0); i < Mathf.Min((closestGeneIndex + adjecentsToCheck / 2), numberOfGenes); i++)
        {
            toDisplay.Add(ChromosomeController.genes[i]);
        }

        OneDView = (bpindex, toDisplay);
    }


    public void Refresh1DView()
    {
        Update1DView(currentCenter, OneDView.displayed);
    }

    public float getScale()
    {
        return slider.value * baseScale;
    }
    public void Update1DView(int center, List<(string geneName, int geneStart, int geneEnd)> displayed)
    {
        var scale = getScale();
        OneDViewFocused = true;

        //

        // OneDView = (center: geneStart / 2 + geneEnd / 2, scale: scale, new List<(string geneName, int geneStart, int geneEnd)> { (geneName, geneStart, geneEnd) });

        foreach (var t in texts)
        {
            t.text = "." + new String(' ', horizontalTextChars - 2) + ".";
        }


        var left = center - scale * (horizontalTextChars / 2);
        var right = center + scale * (horizontalTextChars / 2);

        var reservedAreas = new List<List<(int start, int end)>> { new List<(int start, int end)>(), new List<(int start, int end)>(), new List<(int start, int end)>(), new List<(int start, int end)>(), new List<(int start, int end)>(), };

        // We want to treat the focused gene specially - writing the coordinates in the bottom and putting it in the center
        var focused = (from info in displayed
                       where info.geneName == chromosome.focusedGene
                       select info).ToList();
        if (focused.Count == 1)
        {
            var (geneName, geneStart, geneEnd) = focused[0];

            var length = Mathf.RoundToInt((geneEnd - geneStart) / scale);
            var startPos = Mathf.RoundToInt(InvLerp(left, right, geneStart) * horizontalTextChars);

            if (geneName.Length + 2 > length)
            {
                var geneBar = "|" + new String('-', length) + "|";
                var genePosMarkers = geneStart.ToString().PadRight(length) + geneEnd;
                var genePosMarkersStartPos = startPos - (genePosMarkers.Length - geneBar.Length) / 2;

                texts[2].text = updateSubportion(texts[2].text, startPos, geneBar);
                texts[3].text = updateSubportion(texts[3].text, genePosMarkersStartPos, genePosMarkers);

                reservedAreas[2].Add((startPos, startPos + geneBar.Length));
                reservedAreas[3].Add((genePosMarkersStartPos, genePosMarkersStartPos + genePosMarkers.Length));
            }
            else
            {
                var length1 = Mathf.RoundToInt(-.001f + (length - geneName.Length) / 2.0f);
                var length2 = Mathf.RoundToInt(.001f + (length - geneName.Length) / 2.0f);


                var geneBar = "|" + new String('-', length1) + geneName + new String('-', length2) + "|";
                var genePosMarkers = geneStart.ToString().PadRight(length) + "  " + geneEnd;


                var genePosMarkersStartPos = startPos - (genePosMarkers.Length - geneBar.Length) / 2;

                texts[2].text = updateSubportion(texts[2].text, startPos, geneBar);
                texts[3].text = updateSubportion(texts[3].text, genePosMarkersStartPos, genePosMarkers);

                reservedAreas[2].Add((startPos, startPos + geneBar.Length));
                reservedAreas[3].Add((genePosMarkersStartPos, genePosMarkersStartPos + genePosMarkers.Length));
            }
        }

        // now we handle the rest of the genes, and decide where to write them by checking the reservedareas 
        foreach (var (geneName, geneStart, geneEnd) in displayed)
        {
            var length = Mathf.RoundToInt((geneEnd - geneStart) / scale);
            var startPos = Mathf.RoundToInt(InvLerp(left, right, geneStart) * horizontalTextChars);
            if (geneName != chromosome.focusedGene && length > 1)
            {
                var geneBar = "";
                if (geneName.Length + 2 > length)
                {
                    geneBar = "|" + new String('-', length) + "|";
                }
                else
                {
                    var length1 = Mathf.RoundToInt(-.001f + (length - geneName.Length) / 2.0f);
                    var length2 = Mathf.RoundToInt(.001f + (length - geneName.Length) / 2.0f);
                    geneBar = "|" + new String('-', length1) + geneName + new String('-', length2) + "|";
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
