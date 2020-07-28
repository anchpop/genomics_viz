using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

public struct Point
{
    public Vector3 position;
    public int basePairIndex;
    public int originalIndex;
}

public class ChromosomeController : MonoBehaviour
{
    public float overallScale = 1.5f;
    public float linewidth = 1;
    public float simplificationFactorCoarse = .95f;
    public float simplificationFactorFine = .5f;
    public TextAsset locationSequence;
    public TextAsset geneAnnotations;
    private (List<Point> original, List<Point> fine, List<Point> coarse) points;
    private List<(int start, int end)> genes;

    public GameObject cylinderPrefab_LOD0;
    public GameObject coloredCylinderPrefab_LOD0;
    public GameObject cylinderPrefab_LOD1;
    public GameObject coloredCylinderPrefab_LOD1;
    public GameObject cylinderPrefab_LOD2;
    public GameObject coloredCylinderPrefab_LOD2;
    public GameObject cylinderPrefab_LOD3;
    public GameObject coloredCylinderPrefab_LOD3;

    private LineRenderer line;
    private int numberOfRows = 0;
    private int basePairsPerRow = 5000;

    void Start()
    {
        points = getPoints();
        genes = getGenes();

        var segments = new List<(Point p1, Point p2, List<(float, float)> sections)>(points.fine.Count);
        var currentGeneIndex = 0;
        var toEnd = new List<(int start, int end)>();
        for (int i = 0; i < points.coarse.Count - 1; i++)
        {
            var (endPointListIndex, newCurrentGeneIndex, newSegments, newToEnd) = linesToAdd(i, points.coarse[i + 1].originalIndex, currentGeneIndex, points.coarse, genes, toEnd);
            currentGeneIndex = newCurrentGeneIndex;
            toEnd = newToEnd;
            segments.AddRange(newSegments);
        }

        foreach (var (p1, p2, sections) in segments)
        {
            AddLineSegment(p1, p2, sections, 3);
        }
    }

    (int endPointListIndex, int currentGeneIndex, List<(Point p1, Point p2, List<(float, float)> sections)> segments, List<(int start, int end)> toEnd)
        linesToAdd(int pointListIndexFrom, int originalIndexTo, int currentGeneIndex, List<Point> points, List<(int start, int end)> genes, List<(int start, int end)> toEnd)
    {
        var segments = new List<(Point p1, Point p2, List<(float, float)> sections)>();

        var i = pointListIndexFrom;
        while (true)
        {
            if (points[i].originalIndex < originalIndexTo)
            {
                var p1 = points[i];
                var p2 = points[i + 1];

                var sections = new List<(float, float)>();

                while (true)
                {
                    if (currentGeneIndex >= genes.Count)
                    {
                        break;
                    }

                    var gene = genes[currentGeneIndex];

                    Assert.IsTrue(gene.start >= p1.basePairIndex); // If the gene we're looking at starts *before* the current section, something has gone seriously wrong.

                    if (gene.start >= p2.basePairIndex) // if the gene starts after the end of this section, we are done for now.
                    {
                        break;
                    }
                    else if (gene.end >= p2.basePairIndex)
                    {
                        sections.Add((Mathf.InverseLerp(p1.basePairIndex, p2.basePairIndex, gene.start), 1));
                        toEnd.Add(gene);
                    }
                    else
                    {
                        sections.Add((Mathf.InverseLerp(p1.basePairIndex, p2.basePairIndex, gene.start), Mathf.InverseLerp(p1.basePairIndex, p2.basePairIndex, gene.end)));
                    }


                    currentGeneIndex++;
                }

                foreach (var gene in toEnd.ToList())
                {
                    if (gene.end < p2.basePairIndex)
                    {
                        sections.Add((0, Mathf.InverseLerp(p1.basePairIndex, p2.basePairIndex, gene.end)));
                        toEnd.Remove(gene);
                    }
                    else
                    {
                        sections.Add((0, 1));
                    }
                }

                AddLineSegment(p1, p2, sections, 3);
            }
            else
            {
                return (endPointListIndex: i, currentGeneIndex, segments, toEnd);
            }

            i++;
        }
    }

    List<(int start, int end)> getGenes()
    {
        var genes = new List<(int, int)>();
        bool firstLine = true;
        foreach (var line in geneAnnotations.text.Split('\n'))
        {
            if (firstLine)
            {
                firstLine = false;
                continue;
            }

            if (line != "")
            {
                var info = line.Split('\t');
                genes.Add((int.Parse(info[2]), int.Parse(info[3])));
            }
        }
        return genes;
    }

    (List<Point> original, List<Point> fine, List<Point> coarse) getPoints()
    {
        var center = Vector3.zero;

        var pointsRaw = new List<Vector3>();
        var points = new List<Point>();

        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;


        foreach (var line in locationSequence.text.Split('\n'))
        {
            if (line != "")
            {
                numberOfRows++;

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
            var scaling = Mathf.Max(Mathf.Max(min.x - max.x, min.y - max.y), min.z - max.z);
            var p = new Point();
            p.position = (point - center) * overallScale / scaling;
            p.originalIndex = count;
            p.basePairIndex = count * basePairsPerRow;

            points.Add(p);

            count++;
        }

        var removalOrder = GetSimplificationOrder(points);

        var pointsOriginal = points.ToList();
        var pointsCoarse = points.ToList();
        var pointsFine = points.ToList();


        for (var i = 0; i < numberOfRows * simplificationFactorCoarse; i++)
        {
            pointsCoarse.RemoveAt(removalOrder[i]);
        }


        for (var i = 0; i < numberOfRows * simplificationFactorFine; i++)
        {
            pointsFine.RemoveAt(removalOrder[i]);
        }


        return (original: pointsOriginal, fine: pointsFine, coarse: pointsCoarse);
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
            removalOrder.Add(lowestIndex + 1);
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

    void AddLineSegment(Point p1, Point p2, List<(float f1, float f2)> sections, int LOD)
    {
        void AddSegment(float f1, float f2, GameObject prefab)
        {
            void AddSubsegment(Vector3 p1_, Vector3 p2_)
            {
                var obj = Instantiate(prefab, ((p1_ + p2_) / 2), Quaternion.LookRotation(p1_ - p2_, Vector3.up), transform);
                obj.transform.localScale = new Vector3(
                    obj.transform.localScale.x * linewidth / 100 * overallScale,
                    obj.transform.localScale.y * linewidth / 100 * overallScale,
                    (p1_ - p2_).magnitude
                );
            }

            AddSubsegment(Vector3.Lerp(p1.position, p2.position, f1), Vector3.Lerp(p1.position, p2.position, f2));
        }
        if (sections.Count == 0 || sections[0] != (0, 1))
        {
            AddSegment(0, 1, cylinderGetter(LOD, false));
        }

        foreach (var (f1, f2) in sections)
        {
            AddSegment(f1, f2, cylinderGetter(LOD, true));
        }
    }

    GameObject cylinderGetter(int LOD, bool colored)
    {
        switch (LOD)
        {
            case 0:
                return colored ? coloredCylinderPrefab_LOD0 : cylinderPrefab_LOD0;
            case 1:
                return colored ? coloredCylinderPrefab_LOD1 : cylinderPrefab_LOD1;
            case 2:
                return colored ? coloredCylinderPrefab_LOD2 : cylinderPrefab_LOD2;
            default:
                return colored ? coloredCylinderPrefab_LOD3 : cylinderPrefab_LOD3;
        }
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
