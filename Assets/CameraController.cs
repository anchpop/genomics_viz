﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Util;
using UnityEngine.InputSystem;
using System.Linq;

public class CameraController : MonoBehaviour
{
    public ChromosomeController chromosome;
    public CameraParentController parentController;
    public TextMeshProUGUI searchInput;


    public TextMeshProUGUI sideText;
    public TextMeshProUGUI sideLoc;

    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public TextMeshProUGUI text4;
    public TextMeshProUGUI text5;

    public GraphicRaycaster GraphicRaycaster;
    public UnityEngine.EventSystems.EventSystem m_EventSystem;

    private int horizontalTextChars = 144;

    private (int center, float scale, List<(string geneName, int geneStart, int geneEnd)> displayed) OneDView;

    string lastLit = "";
    void Start()
    {
        text1.text = "";
        text2.text = "";
        text3.text = "";
        text4.text = "";
        text5.text = "";
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

            print(results.Count);
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

    public void Update1DView(string geneName, int geneStart, int geneEnd)
    {
        var scale = (geneEnd - geneStart < (7000 * horizontalTextChars * .8f) ? 7000 : 7000 * 3);
        var length = (geneEnd - geneStart) / scale;

        OneDView = (center: geneStart / 2 + geneEnd / 2, scale: scale, new List<(string geneName, int geneStart, int geneEnd)> { (geneName, geneStart, geneEnd) });

        if (geneName.Length + 2 > length)
        {
            text3.text = "|" + new String('-', length) + "|";
            text4.text = geneStart.ToString().PadRight(length + (geneStart.ToString().Length + geneEnd.ToString().Length) / 2) + geneEnd;
        }
        else
        {
            var length1 = Mathf.RoundToInt(-.001f + (length - geneName.Length) / 2.0f);
            var length2 = Mathf.RoundToInt(.001f + (length - geneName.Length) / 2.0f);

            text3.text = "|" + new String('-', length1) + geneName + new String('-', length2) + "|";
            text4.text = geneStart.ToString().PadRight(-2 + length + (geneStart.ToString().Length + geneEnd.ToString().Length) / 4) + "  " + geneEnd;
        }


    }
}
