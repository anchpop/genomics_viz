using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fakeChromosomeController : MonoBehaviour
{
    public GameObject randoBall1;
    public GameObject randoBall2;
    public GameObject randoBall3;
    
    public LineRenderer line;

    public GameObject geneEmitter;

    public GameObject rna;

    public Material enabledBridgeMat;
    public Material disabledBridgeMat;


    Vector3 b1pos;
    Vector3 b2pos;
    Vector3 b3pos;
    Vector3 b1off;
    Vector3 b2off;
    Vector3 b3off;

    float first = 0;
    float second = 0;
    float third = 0;
    float fourth = 0;

    public float firstSpeed = 1;
    public float secondSpeed = 1;
    public float thirdSpeed = 1;
    public float fourthSpeed = 1;
    public float travelDist = .03f;
    

    // Start is called before the first frame update
    void Start()
    {
        b1pos = randoBall1.transform.position;
        b2pos = randoBall2.transform.position;
        b3pos = randoBall3.transform.position;

        b1off = Random.insideUnitCircle.normalized * travelDist;
        b2off = Random.insideUnitCircle.normalized * travelDist;
        b3off = Random.insideUnitCircle.normalized * travelDist;
    }

    // Update is called once per frame
    void Update()
    {
        first += Time.deltaTime * firstSpeed;
        first = Mathf.Clamp01(first);
        if (first > .95f)
        {
            second += Time.deltaTime * secondSpeed;
            second = Mathf.Clamp01(second);
        }
        if (second > .95f)
        {
            third += Time.deltaTime * secondSpeed;
            third = Mathf.Clamp01(third);
        }
        if (third > .95f)
        {
            fourth += Time.deltaTime * secondSpeed;
            fourth = Mathf.Clamp01(fourth);
        }
        if (fourth > .95f)
        {
            first = second = third = fourth = 0;
        }


        randoBall1.transform.position = Vector3.Lerp(b1pos + b1off, b1pos, first);
        randoBall2.transform.position = Vector3.Lerp(b2pos + b2off, b2pos, first);
        randoBall3.transform.position = Vector3.Lerp(b3pos + b3off, b3pos, second);

        if (third > 0)
        {
            line.material = enabledBridgeMat;
        }
        else
        {
            line.material = disabledBridgeMat;
        }

        
    }
}
