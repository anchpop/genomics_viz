using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public ChromosomeController chromosome;
    public Text geneNameDisplay;
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
                if (gene.geneName != lastLit)
                {
                    chromosome.unhighlightGene(lastLit);

                    lastLit = gene.geneName;
                    geneNameDisplay.text = gene.geneName;
                    chromosome.highlightGene(gene.geneName);
                }
            }
        }
        else
        {
            geneNameDisplay.text = "";
            chromosome.unhighlightGene(lastLit);
            lastLit = "";
        }
    }
}
