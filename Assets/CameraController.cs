﻿using System.Collections;
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

    private (int center, float scale, List<(string geneName, int geneStart, int geneEnd)> displayed) OneDView;

    string lastLit = "";
    void Start()
    {
        foreach (var t in texts)
        {
            t.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {

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
                var nextGene = chromosome.genes[geneInfo.index + 1];

                chromosome.focusGene(nextGene.name);
                parentController.goToGene(nextGene.name);
            }

            if (keyboard.gKey.wasPressedThisFrame)
            {
                openGeneInfoOnline();
            }
        }
    }

    public void clicked_on_1D(float pos)
    {
        pos = (pos - .5f) * horizontalTextChars;
        var basePair = Mathf.RoundToInt(pos * OneDView.scale + OneDView.center);
        parentController.goToBasePairIndex(basePair);
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
        var scale = (info.end - info.start < (7000 * horizontalTextChars * .8f) ? 7000 : 7000 * 3);

        var adjecentsToCheck = 30;

        var toDisplay = new List<(string geneName, int geneStart, int geneEnd)>();

        var numberOfGenes = chromosome.genes.Count;
        for (int i = Mathf.Max((info.index - adjecentsToCheck / 2), 0); i < Mathf.Min((info.index + adjecentsToCheck / 2), numberOfGenes); i++)
        {
            toDisplay.Add(chromosome.genes[i]);
        }

        Update1DView(info.start / 2 + info.end / 2, scale, toDisplay);
    }




    public void Update1DView(int center, float scale, List<(string geneName, int geneStart, int geneEnd)> displayed)
    {
        OneDView = (center, scale, displayed);
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
            var startPos = Mathf.RoundToInt(Mathf.InverseLerp(left, right, geneStart) * horizontalTextChars);
            if (geneName != chromosome.focusedGene && length > 0)
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
            return sub.Substring(-startIndex, -startIndex - sub.Length) + original.Substring(-startIndex - sub.Length, (-startIndex - sub.Length) - original.Length);
        }
        var output = original.Substring(0, startIndex) + ((sub.Length > original.Length - startIndex) ? sub.Substring(0, original.Length - startIndex) : (sub + original.Substring(startIndex + sub.Length, original.Length - (startIndex + sub.Length))));
        Assert.AreEqual(original.Length, output.Length);
        return output;
    }
}
