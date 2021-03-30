﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class CameraParentController : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public ChromosomeController chromosomeController;
    public GameObject vrCamera;
    public GameObject fallbackCamera;
    public GameObject mainCamera;


    public float tweenSpeed = 1;

    private float rott = 0;
    private Quaternion startQ = Quaternion.identity;
    private Quaternion endQ = Quaternion.identity;
    private Vector3 startS = Vector3.zero;
    private Vector3 endS = Vector3.zero;
    private bool currentlyTweening = false;

    public GameObject leftController;
    public GameObject rightController;
    public GameObject fallbackHand;
    public GameObject headset;

    public LineRenderer VRPicker;

    public Vector3? rotatePosLastFrame;
    public Vector3? repositionPosLastFrame;
    public Quaternion? repositionRotLastFrame;

    bool triggerPressedLastFrame = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (vrCamera.activeInHierarchy)
        {
            mainCamera = vrCamera;
        }
        else
        {
            mainCamera = fallbackCamera;
        }
        Interaction();
        VRInteraction();


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

    public void Interaction()
    {
        if (fallbackHand.activeInHierarchy)
        {
            var cameraController = mainCamera.GetComponent<CameraController>();
            var mouse = Mouse.current;

            List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
            //Set up the new Pointer Event
            var m_PointerEventData = new UnityEngine.EventSystems.PointerEventData(cameraController.m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = mouse.position.ReadValue();
            //Raycast using the Graphics Raycaster and mouse click position
            cameraController.GraphicRaycaster.Raycast(m_PointerEventData, results);

            if (results.Count == 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
                cameraController.highlightHit(ray, mouse.leftButton.wasPressedThisFrame);
            }
        }
    }

    public void VRInteraction()
    {
        if (!fallbackHand.activeInHierarchy)
        {
            // Repositioning and reorienting
            if (SteamVR_Actions._default.GrabPinch[SteamVR_Input_Sources.LeftHand].state)
            {
                rotatePosLastFrame = null;

                if (repositionPosLastFrame is Vector3 repositionPosLastFrameValue && repositionRotLastFrame is Quaternion repositionRotLastFrameValue)
                {
                    var repositionRotCurrentFrame = leftController.transform.rotation;
                    var repositionPosCurrentFrame = leftController.transform.position;

                    transform.position += repositionPosCurrentFrame - repositionPosLastFrameValue;

                    float angle = Quaternion.Angle(repositionRotCurrentFrame, repositionRotLastFrameValue);
                }


                repositionPosLastFrame = leftController.transform.position;
                repositionRotLastFrame = leftController.transform.rotation;
            }
            // rotating
            else if (SteamVR_Actions._default.GrabGrip[SteamVR_Input_Sources.LeftHand].state)
            {
                repositionPosLastFrame = null;
                repositionRotLastFrame = null;
                if (rotatePosLastFrame is Vector3 dragPosLastFrameValue)
                {
                    var dragPosCurrentFrame = leftController.transform.position;
                    var rotationChange = Quaternion.FromToRotation(dragPosLastFrameValue - transform.position, dragPosCurrentFrame - transform.position);

                    transform.rotation *= rotationChange;
                }

                rotatePosLastFrame = leftController.transform.position;
            }
            else
            {
                rotatePosLastFrame = null;
                repositionPosLastFrame = null;
                repositionRotLastFrame = null;
            }

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


            var ray = new Ray(rightController.transform.position, rightController.transform.forward);
            var hitPos = mainCamera.GetComponent<CameraController>().highlightHit(ray, SteamVR_Actions._default.GrabPinch[SteamVR_Input_Sources.RightHand].state && !triggerPressedLastFrame);
            if (hitPos is Vector3 hitPosValue)
            {
                VRPicker.SetPositions(new Vector3[] { rightController.transform.position, hitPosValue });
            }
            else
            {
                VRPicker.SetPositions(new Vector3[] { rightController.transform.position, rightController.transform.position + rightController.transform.forward * 10 });
            }
            triggerPressedLastFrame = SteamVR_Actions._default.GrabPinch[SteamVR_Input_Sources.RightHand].state;
        }
    }

    public void goToGene((string name, int start, int end, bool direction) info)
    {
        Debug.Log(info.name);
        var startIndex = chromosomeController.basePairIndexToLocationIndex(info.start);
        var endIndex = chromosomeController.basePairIndexToLocationIndex(info.end);
        if (startIndex == endIndex)
        {
            endIndex += 1;
        }
        var genePositions = ChromosomeController.points.original.GetRange(startIndex, endIndex - startIndex).Select((v) => v.position);

        var geneloc = Vector3.zero;
        foreach (var pos in genePositions)
        {
            geneloc += pos / genePositions.Count();
        }
        geneloc = transform.TransformPoint(geneloc);

        Debug.DrawLine(Vector3.zero, geneloc);
        /*

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

        */

        // TODO this is actually wrong because it doesn't work correctly when the chromosome is rotated (not sure why). Needs to be fixed before release :/
        Debug.DrawRay(geneloc, Vector3.up, Color.blue, 3);
        Debug.Log("Setting position to " + (mainCamera.transform.position - geneloc));
        transform.position = mainCamera.transform.position - geneloc;

        chromosomeController.highlightGene(info);


        mainCamera.GetComponent<CameraController>().Update1DViewGene(info.name);
    }



    public void goToBasePairIndex(int bpindex)
    {
        var info = chromosomeController.basePairIndexToPoint(bpindex);

        mainCamera.GetComponent<CameraController>().selectionIndicator.transform.position = info.position;

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
