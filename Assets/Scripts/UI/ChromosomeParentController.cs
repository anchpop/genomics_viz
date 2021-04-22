using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Valve.VR;
using Valve.VR.InteractionSystem;
using DG.Tweening;
using CapnpGen;
using Capnp;



public class ChromosomeParentController : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public ChromosomeController chromosomeController;
    public GameObject vrCamera;
    public GameObject fallbackCamera;
    public GameObject mainCamera;
    public bool inVr;


    public float tweenDuration = 1;
    private Tween tween;

    public float zoomAwayScale = 1.2f;


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
            inVr = true;
        }
        else
        {
            mainCamera = fallbackCamera;
            mainCamera.transform.LookAt(transform.position);
            inVr = false;
        }
        Interaction();
        VRInteraction();


        if (!isTweening())
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard.rightArrowKey.IsPressed() || keyboard.dKey.IsPressed())
            {
                var rot = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, transform.InverseTransformDirection(mainCamera.transform.up));
                transform.rotation *= rot;
            }
            else if (keyboard.leftArrowKey.IsPressed() || keyboard.aKey.IsPressed())
            {
                var rot = Quaternion.AngleAxis(-rotationSpeed * Time.deltaTime, transform.InverseTransformDirection(mainCamera.transform.up));
                transform.rotation *= rot;
            }


            if (keyboard.upArrowKey.IsPressed() || keyboard.wKey.IsPressed())
            {
                var rot = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, transform.InverseTransformDirection(-mainCamera.transform.right));
                transform.rotation *= rot;
            }
            else if (keyboard.downArrowKey.IsPressed() || keyboard.sKey.IsPressed())
            {
                var rot = Quaternion.AngleAxis(-rotationSpeed * Time.deltaTime, transform.InverseTransformDirection(-mainCamera.transform.right));
                transform.rotation *= rot;
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
            cameraController.OneDViewGraphicRaycaster.Raycast(m_PointerEventData, results);
            cameraController.ButtonViewGraphicRaycaster.Raycast(m_PointerEventData, results);

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

    public bool isTweening()
    {
        return tween != null && !tween.IsComplete();
    }

    public void goToSegment(string segmentSet, int segmentSetIndex)
    {
        var info = ChromosomeController.GetSegmentInfo(ChromosomeController.GetSegmentFromCurrentChromosome(segmentSet, segmentSetIndex));
        var startIndex = chromosomeController.binToLocationIndex((int)info.StartBin);
        var endIndex = chromosomeController.binToLocationIndex((int)info.EndBin);
        if (startIndex == endIndex)
        {
            endIndex += 1;
        }
        var genePositions = ChromosomeController.chromosomeRenderingInfo.points.GetRange(startIndex, endIndex - startIndex).Select((v) => v.position);

        var gene_local_center = Vector3.zero;
        foreach (var pos in genePositions)
        {
            gene_local_center += pos / genePositions.Count();
        }

        var gene_max_dist = genePositions.Max(p => p.magnitude);
        var local_pos = gene_local_center.normalized * gene_max_dist * zoomAwayScale;

        var worldloc = transform.TransformPoint(local_pos);

        tweenToShowPos(worldloc);
        var camera = mainCamera.GetComponent<CameraController>();

        camera.Update1DViewSegment(segmentSet, segmentSetIndex);

    }

    public void tweenToShowPos(Vector3 worldPos)
    {
        var camToLoc = worldPos - mainCamera.transform.position;
        var locToCam = -camToLoc;

        var current_loc = transform.position;
        var dest_loc = current_loc + locToCam;
        var cam_pos = mainCamera.transform.position;

        var current_rel_cam = current_loc - cam_pos;
        var dest_rel_cam = dest_loc - cam_pos;
        var current_dist = current_rel_cam.magnitude;
        var dest_dist = dest_rel_cam.magnitude;

        tween = DOTween.To(() => 0.0f, x =>
        {
            var rot = Quaternion.FromToRotation(current_rel_cam.normalized, dest_rel_cam.normalized);

            var new_rel_cam = (Quaternion.Slerp(Quaternion.identity, rot, x)) * current_rel_cam.normalized * Mathf.Lerp(current_dist, dest_dist, x);

            transform.position = cam_pos + new_rel_cam;
            if (!inVr)
            {
                mainCamera.transform.LookAt(transform.position);
            }
        }, 1.0f, tweenDuration * Mathf.Clamp((current_rel_cam - dest_rel_cam).magnitude, .5f, 1.5f)).SetEase(Ease.InOutCubic);

        tween.SetAutoKill(false);
    }


    public void openGeneInfoOnline()
    {
        /*
        var geneInfo = chromosomeController.geneDict[chromosomeController.focusedGene];
        Application.OpenURL(
            "https://genome.ucsc.edu/cgi-bin/hgTracks?db=hg19&lastVirtModeType=default&lastVirtModeExtraState=&virtModeType=default&virtMode=0&nonVirtPosition=&position=chr1%3A"
            + geneInfo.start.ToString("D")
            + "%2D"
            + geneInfo.end.ToString("D")
            + "&hgsid=908127743_HmMER1nPkAhvlmaDlkaob9Vh99Va");
        */
    }


    public void clicked_left()
    {
        goToBin(Mathf.Clamp(CameraController.OneDView.center - 5000, 0, ChromosomeController.chromosomeRenderingInfo.highestBin));
    }

    public void clicked_right()
    {
        goToBin(Mathf.Clamp(CameraController.OneDView.center - 5000, 0, ChromosomeController.chromosomeRenderingInfo.highestBin));
    }


    public void goToBin(int bpindex)
    {
        var info = chromosomeController.binToPoint(bpindex);

        mainCamera.GetComponent<CameraController>().selectionIndicator.transform.localPosition = info;

        tweenToShowPos(transform.TransformPoint(info * zoomAwayScale));

        mainCamera.GetComponent<CameraController>().Update1DViewBasePairIndex(bpindex);
    }
}
