using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Text geneNameDisplay;
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
                geneNameDisplay.text = gene.name;
            }
        }
        else
        {
            geneNameDisplay.text = "";
        }
    }
}
