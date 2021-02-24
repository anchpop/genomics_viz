using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraParentController : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public ChromosomeController chromosomeController;
    public GameObject mainCamera;


    public float tweenSpeed = 1;

    private float rott = 0;
    private Quaternion startQ = Quaternion.identity;
    private Quaternion endQ = Quaternion.identity;
    private Vector3 startS = Vector3.zero;
    private Vector3 endS = Vector3.zero;
    private bool currentlyTweening = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyTweening)
        {
            if (rott >= 1)
            {
                currentlyTweening = false;
                rott = 0;
            }
            else
            {
                rott += Time.deltaTime * tweenSpeed;
                rott = Mathf.Min(1, rott);

                var o = Util.Math.easeInOutQuart(rott);
                transform.rotation = Quaternion.Slerp(startQ, endQ, o);
                transform.localScale = Vector3.Lerp(startS, endS, o);
            }
        }


        if (!currentlyTweening)
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard.leftArrowKey.IsPressed() || keyboard.aKey.IsPressed())
            {
                transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
            }
            else if (keyboard.rightArrowKey.IsPressed() || keyboard.dKey.IsPressed())
            {
                transform.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0));
            }


            if (keyboard.upArrowKey.IsPressed() || keyboard.wKey.IsPressed())
            {
                transform.Rotate(new Vector3(rotationSpeed * Time.deltaTime, 0, 0));
            }
            else if (keyboard.downArrowKey.IsPressed() || keyboard.sKey.IsPressed())
            {
                transform.Rotate(new Vector3(-rotationSpeed * Time.deltaTime, 0, 0));
            }

            var scroll = mouse.scroll.ReadValue();
            if (scroll.y != 0)
            {
                if (scroll.y > 0)
                {
                    transform.localScale = transform.localScale * (1 - .3f * Time.deltaTime);
                }
                else if (scroll.y < 0)
                {
                    transform.localScale = transform.localScale * (1 + .3f * Time.deltaTime);
                }
            }

        }
    }

    public void goToGene(string geneName)
    {
        /*
        var info = chromosomeController.geneDict[geneName];

        var geneloc = Vector3.zero;
        foreach (var renderer in info.renderer)
        {
            geneloc += renderer.gameObject.transform.position;
        }
        geneloc /= info.renderer.Count;
        Debug.DrawLine(Vector3.zero, geneloc);


        if (mainCamera.transform.localPosition.normalized == geneloc.normalized)
        {
            startQ = transform.rotation;
            endQ = transform.rotation;
        }
        else
        {
            startQ = transform.rotation;
            endQ = Quaternion.FromToRotation(mainCamera.transform.localPosition, geneloc);
        }

        startS = transform.localScale;
        var endScale = 1.7f * geneloc.magnitude / mainCamera.transform.localPosition.magnitude;
        endS = new Vector3(endScale, endScale, endScale);

        currentlyTweening = true;
        rott = 0;

        chromosomeController.highlightGene(geneName);


        mainCamera.GetComponent<CameraController>().Update1DViewGene(geneName);
        */
    }



    public void goToBasePairIndex(int bpindex)
    {
        var info = chromosomeController.basePairIndexToPoint(bpindex);

        if (mainCamera.transform.localPosition.normalized == info.position.normalized)
        {
            startQ = transform.rotation;
            endQ = transform.rotation;
        }
        else
        {
            startQ = transform.rotation;

            endQ = Quaternion.FromToRotation(mainCamera.transform.localPosition, info.position);
        }

        startS = transform.localScale;
        var endScale = 1.7f * info.position.magnitude / mainCamera.transform.localPosition.magnitude;
        endS = new Vector3(endScale, endScale, endScale);

        currentlyTweening = true;
        rott = 0;

        mainCamera.GetComponent<CameraController>().Update1DViewBasePairIndex(bpindex);
    }

}
