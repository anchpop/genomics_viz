using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    public float distance = 1f;
    public float speed = .1f;

    Vector3 outPos;
    Vector3 originalPos;
    float n = 0;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 direction = Random.insideUnitCircle.normalized;
        Vector3 offset = distance * direction;
        originalPos = transform.position;
        outPos = originalPos + offset;
    }

    // Update is called once per frame
    void Update()
    {
        n += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(outPos, originalPos, n % 1);
    }
}
