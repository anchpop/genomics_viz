//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Controls for the non-VR debug camera
//
//=============================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    [RequireComponent(typeof(Camera))]
    public class FallbackCameraController : MonoBehaviour
    {
        public float speed = 4.0f;
        public float shiftSpeed = 16.0f;
        public bool showInstructions = true;

        private Vector3 startEulerAngles;
        private Vector3 startMousePosition;
        private float realTime;

        //-------------------------------------------------
        void OnEnable()
        {
            realTime = Time.realtimeSinceStartup;
        }


        //-------------------------------------------------
        void Update()
        {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;
            float forward = 0.0f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                forward += 1.0f;
            }
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                forward -= 1.0f;
            }

            float up = 0.0f;
            if (keyboard.eKey.isPressed)
            {
                up += 1.0f;
            }
            if (keyboard.qKey.isPressed)
            {
                up -= 1.0f;
            }

            float right = 0.0f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                right += 1.0f;
            }
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                right -= 1.0f;
            }

            float currentSpeed = speed;
            if (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed)
            {
                currentSpeed = shiftSpeed;
            }

            float realTimeNow = Time.realtimeSinceStartup;
            float deltaRealTime = realTimeNow - realTime;
            realTime = realTimeNow;

            Vector3 delta = new Vector3(right, up, forward) * currentSpeed * deltaRealTime;

            transform.position += transform.TransformDirection(delta);

            Vector3 mousePosition = mouse.position.ReadValue();

            if (mouse.leftButton.wasPressedThisFrame/* right mouse */)
            {
                startMousePosition = mousePosition;
                startEulerAngles = transform.localEulerAngles;
            }

            if (mouse.leftButton.wasPressedThisFrame /* right mouse */)
            {
                Vector3 offset = mousePosition - startMousePosition;
                transform.localEulerAngles = startEulerAngles + new Vector3(-offset.y * 360.0f / Screen.height, offset.x * 360.0f / Screen.width, 0.0f);
            }
        }


        //-------------------------------------------------
        void OnGUI()
        {
            if (showInstructions)
            {
                GUI.Label(new Rect(10.0f, 10.0f, 600.0f, 400.0f),
                    "WASD EQ/Arrow Keys to translate the camera\n" +
                    "Right mouse click to rotate the camera\n" +
                    "Left mouse click for standard interactions.\n");
            }
        }
    }
}
