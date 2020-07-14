using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public struct Point
{
    public Vector3 position;
    public int basePairIndex;
}

public class ChromosomeController : MonoBehaviour
{
    public float overallScale = 1.5f;
    public float linewidth = 1;
    public float simplificationFactor = 0;
    public bool smartSimplify = true;
    public TextAsset locationSequence;
    private List<Point> points;

    public GameObject spherePrefab;
    public GameObject cylinderPrefab;

    private LineRenderer line;
    private int numberOfRows = 0;
    private List<int> removalOrder;

    void Start()
    {
        var center = Vector3.zero;
        line = GetComponent<LineRenderer>();

        var pointsRaw = new List<Vector3>();
        points = new List<Point>();

        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;


        foreach (var line in locationSequence.text.Split('\n'))
        {
            if (line != "")
            {
                numberOfRows++;
                if (!smartSimplify && UnityEngine.Random.value < simplificationFactor) continue;

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

        var count = 0;
        foreach (var point in pointsRaw)
        {
            count++;
            var scaling = Mathf.Max(Mathf.Max(min.x - max.x, min.y - max.y), min.z - max.z);
            var p = new Point();
            p.position = (point - center) * overallScale / scaling;
            p.basePairIndex = count * 5000;

            points.Add(p);
        }

        if (smartSimplify)
        {
            removalOrder = GetSimplificationOrder(points);
            for (var i = 0; i < numberOfRows * simplificationFactor; i++)
            {
                points.RemoveAt(removalOrder[i]);
            }
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

    List<int> GetSimplificationOrder(List<Point> points)
    {
        var importanceList = new List<(Point point, float area)>();
        var removalOrder = new List<int>();

        // generate initial importance list
        for (int i = 1; i < points.Count - 1; i++)
        {
            var a = points[i].position;
            var b = points[i + 1].position;
            var c = points[i - 1].position;
            var area = triangleArea(a, b, c);
            importanceList.Add((points[i], area));
        }


        while (importanceList.Count > 1)
        {
            // find index of least important point
            int lowestIndex = 0;
            for (int i = 1; i < importanceList.Count; i++)
            {
                if (importanceList[lowestIndex].area > importanceList[i].area)
                {
                    lowestIndex = i;
                }
            }

            // This is the point to remove!
            removalOrder.Add(lowestIndex);
            importanceList.RemoveAt(lowestIndex);

            // Recalcuate the importance of the neighbour points
            foreach (var index in new List<int> { lowestIndex - 1, lowestIndex })
            {
                // if we're at the very start or the very end of the *point* list, we don't simplify anything
                if (index == -1) continue;
                if (index == importanceList.Count) continue;

                // if we're at the very start of the *importance* list, get the first point from the *point* list as the "before" point
                var before = index == 0 ? points[0].position : importanceList[index - 1].point.position;
                var current = importanceList[index].point.position;
                // if we're at the very end of the *importance* list, get the last point from the *point* list as the "after" point
                var next = index == importanceList.Count - 1 ? points[points.Count - 1].position : importanceList[index + 1].point.position;

                var area = triangleArea(before, current, next);
                importanceList[index] = (importanceList[index].point, area);
            }

            // Rinse and repeat until there's no more points left to remove! This will give us an order of points to remove when simplifying
        }

        return removalOrder;
    }

    void AddLineSegment(Point p1, Point p2)
    {
        var obj = Instantiate(cylinderPrefab, ((p1.position + p2.position) / 2), Quaternion.LookRotation(p1.position - p2.position, Vector3.right), transform);
        obj.transform.localScale = new Vector3(linewidth / 100, linewidth / 100, (p1.position - p2.position).magnitude);
    }

    float triangleArea(Vector3 a, Vector3 b, Vector3 c)
    {
        return Mathf.Abs(a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
