using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
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
    float simplificationFactorCoarse = .90f;
    float simplificationFactorFine = .1f;
    public TextAsset locationSequence;
    public TextAsset geneAnnotations;
    private (List<Point> original, List<Point> fine, List<Point> coarse) points;
    private List<(string name, int start, int end)> genes;

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

        var currentGeneIndex = 0;
        var toEnd = new List<(string name, int start, int end)>();

        var fineIndex = 0;
        var orignalIndex = 0;
        for (int i = 0; i < points.coarse.Count - 1; i++)
        {
            var stopIndex = points.coarse[i + 1].originalIndex;
            var (_, newCurrentGeneIndex, segmentsCoarse, newToEnd) = linesToAdd(i, stopIndex, currentGeneIndex, points.coarse, genes, toEnd.ToList());
            var (newFineIndex, _, segmentsFine, _) = linesToAdd(fineIndex, stopIndex, currentGeneIndex, points.fine, genes, toEnd.ToList());
            var (newOriginalIndex, _, segmentsOriginal, _) = linesToAdd(orignalIndex, stopIndex, currentGeneIndex, points.original, genes, toEnd.ToList());
            currentGeneIndex = newCurrentGeneIndex;
            toEnd = newToEnd;
            fineIndex = newFineIndex;
            orignalIndex = newOriginalIndex;

            var coarseSegments = new List<MeshRenderer>();
            foreach (var (p1, p2, sections) in segmentsCoarse)
            {
                coarseSegments.AddRange(AddLineSegment(p1, p2, sections, 3));
            }
            var fineSegments = new List<MeshRenderer>();
            foreach (var (p1, p2, sections) in segmentsFine)
            {
                fineSegments.AddRange(AddLineSegment(p1, p2, sections, 3));
            }


            var LODParent = new GameObject("LODParent");
            var coarseParent = new GameObject("coarseParent");
            var fineParent = new GameObject("fineParent");
            coarseParent.transform.parent = LODParent.transform;
            fineParent.transform.parent = LODParent.transform;
            foreach (var segment in coarseSegments)
            {
                segment.transform.parent = coarseParent.transform;
            }
            foreach (var segment in fineSegments)
            {
                segment.transform.parent = fineParent.transform;
            }

            var LODGroup = LODParent.AddComponent<LODGroup>();
            LOD[] lods = new LOD[2];
            lods[0] = new LOD(1.0F / (1 + 1), fineSegments.ToArray());
            lods[1] = new LOD(1.0F / (200 + 1), coarseSegments.ToArray());
            LODGroup.SetLODs(lods);
            LODGroup.RecalculateBounds();
        }

    }

    (int endPointListIndex, int currentGeneIndex, List<(Point p1, Point p2, List<(string name, float start, float end)> sections)> segments, List<(string name, int start, int end)> toEnd)
        linesToAdd(int pointListIndexFrom, int originalIndexTo, int currentGeneIndex, List<Point> points, List<(string name, int start, int end)> genes, List<(string name, int start, int end)> toEnd)
    {
        var newToEnd = new List<(string name, int start, int end)>();
        var segments = new List<(Point p1, Point p2, List<(string name, float, float)> sections)>();

        var i = pointListIndexFrom;
        while (true)
        {
            if (points[i].originalIndex < originalIndexTo)
            {
                var p1 = points[i];
                var p2 = points[i + 1];

                var sections = new List<(string name, float start, float end)>();

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
                        var f1 = Mathf.InverseLerp(p1.basePairIndex, p2.basePairIndex, gene.start);
                        if (f1 != 1)
                        {
                            sections.Add((gene.name, f1, 1));
                        }
                        newToEnd.Add(gene);
                    }
                    else
                    {
                        var f1 = Mathf.InverseLerp(p1.basePairIndex, p2.basePairIndex, gene.start);
                        var f2 = Mathf.InverseLerp(p1.basePairIndex, p2.basePairIndex, gene.end);
                        if (f1 < f2)
                        {
                            sections.Add((gene.name, f1, f2));
                        }
                        else if (f1 == f2)
                        {
                            Debug.Log("Zero-length gene");
                        }
                        Assert.IsTrue(f1 <= f2);
                    }


                    currentGeneIndex++;
                }

                foreach (var gene in toEnd.ToList())
                {
                    if (gene.end < p2.basePairIndex)
                    {
                        var f2 = Mathf.InverseLerp(p1.basePairIndex, p2.basePairIndex, gene.end);
                        if (f2 != 0) // sometimes a gene ends exactly on a curve, lets not bother doing anything in that case
                        {
                            sections.Add((gene.name, 0, f2));
                        }
                        toEnd.Remove(gene);
                    }
                    else
                    {
                        sections.Add((gene.name, 0, 1));
                    }
                }
                toEnd.AddRange(newToEnd);
                newToEnd = new List<(string name, int start, int end)>();
                segments.Add((p1, p2, sections));
            }
            else
            {
                return (endPointListIndex: i, currentGeneIndex, segments, toEnd);
            }

            i++;
        }
    }

    List<(string name, int start, int end)> getGenes()
    {
        var genes = new List<(string name, int start, int end)>();
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
                Assert.AreNotEqual(info[6], "");
                genes.Add((name: info[6], start: int.Parse(info[2]), end: int.Parse(info[3])));
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

    List<MeshRenderer> AddLineSegment(Point p1, Point p2, List<(string name, float f1, float f2)> sections, int LOD)
    {
        var segments = new List<MeshRenderer>();
        GameObject AddSubsegment(Vector3 p1_, Vector3 p2_, GameObject prefab)
        {
            var obj = Instantiate(prefab, ((p1_ + p2_) / 2), Quaternion.LookRotation(p1_ - p2_, Vector3.up), transform);
            obj.transform.localScale = new Vector3(
                obj.transform.localScale.x * linewidth / 100 * overallScale,
                obj.transform.localScale.y * linewidth / 100 * overallScale,
                (p1_ - p2_).magnitude
            );
            segments.Add(obj.GetComponent<MeshRenderer>());
            return obj;
        }
        void AddGeneSegment(string name, float f1, float f2, GameObject prefab, bool gene)
        {
            if (gene && LOD > 2 && (f2 - f1) < .1f)
            {
                return;
            }

            var geneObj = AddSubsegment(Vector3.Lerp(p1.position, p2.position, f1), Vector3.Lerp(p1.position, p2.position, f2), prefab);
            var geneController = geneObj.AddComponent<GeneController>();
            geneController.name = name;
        }
        if (sections.Count == 0 || (sections[0].f1 != 0 || sections[0].f2 != 1))
        {
            AddSubsegment(p1.position, p2.position, cylinderGetter(3, false)); // ignore input LOD and always get the 3rd one, it's good enough
        }

        foreach (var (name, f1, f2) in sections)
        {
            if (f1 >= f2)
            {
                Assert.IsTrue(f1 < f2);
            }
            AddGeneSegment(name, f1, f2, cylinderGetter(3, true), true); // ignore input LOD and always get the 3rd one, it's good enough
        }
        return segments;
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
