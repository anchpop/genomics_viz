using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraParentController : MonoBehaviour
{
    public float rotationSpeed = 30f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard.leftArrowKey.IsPressed())
        {
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
        }
        else if (keyboard.rightArrowKey.IsPressed())
        {
            transform.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0));
        }


        if (keyboard.upArrowKey.IsPressed())
        {
            transform.localScale = transform.localScale * (1 - (.4f * Time.deltaTime));
        }
        else if (keyboard.downArrowKey.IsPressed())
        {
            transform.localScale = transform.localScale * (1 + (.4f * Time.deltaTime));
        }
    }
}
