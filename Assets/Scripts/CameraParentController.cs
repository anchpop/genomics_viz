using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0));
        }


        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.localScale = transform.localScale * (1 - (.1f * Time.deltaTime));
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.localScale = transform.localScale * (1 + (.1f * Time.deltaTime));
        }
    }
}
