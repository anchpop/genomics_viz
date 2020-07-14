using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ChromosomeController : MonoBehaviour
{
    public float overallScale = 1.5f;
    public float linewidth = 15;
    public int simplificationFactor = 1;
    private int numberOfRows = 0;
    public TextAsset locationSequence;
    public List<Vector3> locations;

    public GameObject spherePrefab;
    public GameObject cylinderPrefab;

    LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        var center = Vector3.zero;
        line = GetComponent<LineRenderer>();

        var pointsRaw = new List<Vector3>();
        var points = new List<Vector3>();

        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;


        foreach (var line in locationSequence.text.Split('\n'))
        {
            if (line != "")
            {
                numberOfRows++;
                if (numberOfRows % simplificationFactor != 0) continue;


                var coords = line.Split('\t');
                var newVector = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));

                if (newVector.x < min.x)
                {
                    min = new Vector3(newVector.x, min.y, min.z);
                }
                if (newVector.y < min.y)
                {
                    min = new Vector3(min.x, newVector.y, min.z);
                }
                if (newVector.z < min.z)
                {
                    min = new Vector3(min.x, min.y, newVector.z);
                }
                if (newVector.x > max.x)
                {
                    max = new Vector3(newVector.x, max.y, max.z);
                }
                if (newVector.y > max.y)
                {
                    max = new Vector3(max.x, newVector.y, max.z);
                }
                if (newVector.z > max.z)
                {
                    max = new Vector3(max.x, max.y, newVector.z);
                }

                pointsRaw.Add(newVector);
                center += newVector;
            }
        }



        center = center / pointsRaw.Count;

        foreach (var point in pointsRaw)
        {
            var scaling = Mathf.Max(Mathf.Max(min.x - max.x, min.y - max.y), min.z - max.z);

            points.Add((point - center) * overallScale / scaling);
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            AddLineSegment(points[i], points[i + 1]);
        }

        /*
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
        line.startWidth = linewidth / 100;
        line.endWidth = linewidth / 100;
        line.alignment = LineAlignment.View;*/
    }

    void AddLineSegment(Vector3 p1, Vector3 p2)
    {
        var obj = Instantiate(cylinderPrefab, ((p1 + p2) / 2), Quaternion.LookRotation(p1 - p2, Vector3.right), transform);
        obj.transform.localScale = new Vector3(linewidth / 100, linewidth / 100, (p1 - p2).magnitude);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
