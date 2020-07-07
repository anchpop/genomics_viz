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
        transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
    }
}
