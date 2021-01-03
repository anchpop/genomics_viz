using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class fakeChromosomeController : MonoBehaviour
{
    public GameObject randoBall1;
    public GameObject randoBall2;
    public GameObject randoBall3;
    
    public LineRenderer line;


    public GameObject rna;

    public Material enabledBridgeMat;
    public Material disabledBridgeMat;


    Vector3 b1pos;
    Vector3 b2pos;
    Vector3 b3pos;
    Vector3 b1off;
    Vector3 b2off;
    Vector3 b3off;


    public List<Transform> gene;

    public GameObject RNASprite;

    public List<float> times;
    public List<float> currentTimes;
    int currentIndex = 0;

    public float firstSpeed = 1;
    public float secondSpeed = 1;
    public float thirdSpeed = 1;
    public float fourthSpeed = 1;
    public float travelDist = .3f;

    float currentGeneEmitter = 0;
    public float geneEmitterRotationSpeed = 1;
    public float emitterRate = 3;
    float lastEmittedTime = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        b1pos = randoBall1.transform.position;
        b2pos = randoBall2.transform.position;
        b3pos = randoBall3.transform.position;

        b1off = Random.insideUnitCircle.normalized * travelDist;
        b2off = Random.insideUnitCircle.normalized * travelDist;
        b3off = Random.insideUnitCircle.normalized * travelDist;

        currentTimes = Enumerable.Repeat(0.0f, times.Count).ToList();

        
    }

    // Update is called once per frame
    void Update()
    {
        currentTimes[currentIndex] += Time.deltaTime * times[currentIndex];
        currentTimes[currentIndex] = Mathf.Clamp01(currentTimes[currentIndex]);
        if  (currentTimes[currentIndex] >= 1)
        {
            currentIndex += 1;
        }

        if (currentIndex == currentTimes.Count)
        {
            currentIndex = 0;
            currentTimes = Enumerable.Repeat(0.0f, times.Count).ToList();
        }



        randoBall1.transform.position = Vector3.Lerp(b1pos + b1off, b1pos, currentTimes[0]);
        randoBall2.transform.position = Vector3.Lerp(b2pos + b2off, b2pos, currentTimes[0]);
        randoBall3.transform.position = Vector3.Lerp(b3pos + b3off, b3pos, currentTimes[2]);

        if (currentTimes[3] > 0)
        {
            line.material = enabledBridgeMat;
        }
        else
        {
            line.material = disabledBridgeMat;
        }

        if (currentTimes[4] > 0)
        {
            currentGeneEmitter += geneEmitterRotationSpeed * Time.deltaTime;
            if (currentGeneEmitter > gene.Count)
            {
                currentGeneEmitter = 0;
            }

            var toEmitFrom = gene[(int)currentGeneEmitter];
            if (Time.time - lastEmittedTime > 1.0/emitterRate)
            {
                lastEmittedTime = Time.time;
                Instantiate(RNASprite, toEmitFrom.position, Quaternion.identity);
            }
        }
    }
}
