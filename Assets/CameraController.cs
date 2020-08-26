using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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


                var textToShow = gene.geneName.PadBoth(15);
                text1.text = textToShow;
            }
        }
        else
        {
            text1.text = "";
            chromosome.unhighlightGene(lastLit);
            lastLit = "";
        }
    }
}
