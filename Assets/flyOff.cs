using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyOff : MonoBehaviour
{
    public Vector3 direction;
    public float flySpeed;
    public float lifetime;
    float spawnTime;
    // Start is called before the first frame update
    void Start()
    {
        direction = Random.insideUnitCircle.normalized;
        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * flySpeed * Time.deltaTime;
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }
}
