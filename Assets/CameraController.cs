﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Util;

public class CameraController : MonoBehaviour
{
    public ChromosomeController chromosome;

    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public TextMeshProUGUI text4;
    public TextMeshProUGUI text5;

    string lastLit = "";
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
                var cursorBasePair = Mathf.Lerp(gene.segmentStart, gene.segmentEnd, cursorDistance);

                text2.text = gene.geneName;
                text3.text = "|---------------------------------------------------------------|";
                text4.text = gene.geneStart.ToString().PadRight(64 - (gene.geneStart.ToString().Length + gene.geneEnd.ToString().Length) / 2) + gene.geneEnd;
                text5.text = cursorBasePair.ToString();
            }
        }
        else
        {
            text1.text = "";
            text2.text = "";
            text3.text = "";
            text4.text = "";
            text5.text = "";
            chromosome.unhighlightGene(lastLit);
            lastLit = "";
        }
    }
}
