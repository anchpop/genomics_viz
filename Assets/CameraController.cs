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
        else
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
                        text2.text = gene.geneName;
                        text3.text = "|---------------------------------------------------------------|";
                        text4.text = gene.geneStart.ToString().PadRight(64 - (gene.geneStart.ToString().Length + gene.geneEnd.ToString().Length) / 2) + gene.geneEnd;
                        text5.text = cursorBasePair.ToString();

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

        if (chromosome.focusedGene != "")
        {
            if (keyboard.qKey.wasPressedThisFrame)
            {
                var geneInfo = chromosome.geneDict[chromosome.focusedGene];
                var nextGene = chromosome.genes[geneInfo.index + 1];

                text2.text = nextGene.name;
                text3.text = "|---------------------------------------------------------------|";
                text4.text = nextGene.start.ToString().PadRight(64 - (nextGene.start.ToString().Length + nextGene.end.ToString().Length) / 2) + nextGene.end;
                text5.text = "";

                chromosome.focusGene(nextGene.name);
                parentController.goToGene(nextGene.name);
            }

            if (keyboard.gKey.wasPressedThisFrame)
            {
                var geneInfo = chromosome.geneDict[chromosome.focusedGene];
                Application.OpenURL(
                    "http://genome.ucsc.edu/cgi-bin/hgTracks?db=hg19&lastVirtModeType=default&lastVirtModeExtraState=&virtModeType=default&virtMode=0&nonVirtPosition=&position=chr1%3A"
                    + geneInfo.start.ToString("D")
                    + "%2D"
                    + geneInfo.end.ToString("D")
                    + "&hgsid=908127743_HmMER1nPkAhvlmaDlkaob9Vh99Va");
            }
        }
    }
}
