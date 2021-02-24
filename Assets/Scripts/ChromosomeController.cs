﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public struct Point
{
    public Vector3 position;
    public int basePairIndex;
    public int originalIndex;
}

public class ChromosomeController : MonoBehaviour
{
    public float overallScale = 1.5f;
    public float lineWidth = 1;

    static bool cartoon = false;
    static (Vector3 position, Vector3 rotation, Vector3 scale) cameraParentCachedPosition;
    public GameObject cameraParent;
    public GameObject VisualizeSectionButton;
    public GameObject SeeFullGenomeButton;
    public TextAsset locationSequence;
    public TextAsset geneAnnotations;
    public TextAsset GATA;
    public TextAsset CTCF;
    public TextAsset IRF1;
    public TextAsset ChromatinInteractionPrediction;
    public static (List<Point> original, List<Point> coarse) points;
    public static List<(string name, int start, int end)> genes;
    public List<(int start, int end)> gata;
    public List<(int start, int end)> ctcf;
    public List<(int start, int end)> irf;
    public List<((int start, int end) start, (int start, int end) end)> chromatinInteractionPrediction;



    public GameObject cylinderPrefab_LOD0;
    public GameObject coloredCylinderPrefab_LOD0;
    public GameObject cylinderPrefab_LOD1;
    public GameObject coloredCylinderPrefab_LOD1;
    public GameObject cylinderPrefab_LOD2;
    public GameObject coloredCylinderPrefab_LOD2;
    public GameObject cylinderPrefab_LOD3;
    public GameObject coloredCylinderPrefab_LOD3;
    public GameObject Sphere_CTCF;
    public GameObject Sphere_GATA;
    public GameObject Sphere_IDR;
    public GameObject BridgePrefab;

    public Material coloredMaterial;
    public Material highlightedColoredMaterial;

    public GameObject geneTextCanvas;

    public float cartoonAmount = .1f;

    // Unity places a limit on the number of verts on a mesh that's quite a bit lower than the amount we need
    // So, we need to use multiple meshes, which means multiple mesh renderers
    public List<MeshFilter> backboneRenderers;
    public List<MeshFilter> geneRenderers;



    public KTrie.StringTrie<(int start, int end, int index)> geneDict;

    static private int numberOfRows = 0;
    static public int totalBasePairs = 0;
    public int basePairsPerRow = 5000;

    public string focusedGene = "";


    List<List<Vector3>> verticiesl;
    List<List<int>> indicesl;

    Vector3 randoVector;



    void Start()
    {
        if (cameraParentCachedPosition.position != Vector3.zero || cameraParentCachedPosition.rotation != Vector3.zero || cameraParentCachedPosition.scale != Vector3.zero)
        {
            cameraParent.transform.position = cameraParentCachedPosition.position;
            cameraParent.transform.eulerAngles = cameraParentCachedPosition.rotation;
            cameraParent.transform.localScale = cameraParentCachedPosition.scale;
        }
        if (cartoon)
        {
            VisualizeSectionButton.SetActive(false);
            SeeFullGenomeButton.SetActive(true);
        }
        else
        {

            VisualizeSectionButton.SetActive(true);
            SeeFullGenomeButton.SetActive(false);
        }



        geneDict = new KTrie.StringTrie<(int start, int end, int index)>();
        points = getPoints();
        genes = getGenes();
        gata = getGATA();
        ctcf = getCTCF();
        irf = getIRF();
        chromatinInteractionPrediction = getChromatinInteractionPrediction();


        totalBasePairs = basePairsPerRow * numberOfRows;
        randoVector = Random.insideUnitSphere;

        createBackboneMesh();
        createGenesMesh();
        createChromatidInterationPredictionLines();


    }

    void createBackboneMesh()
    {
        verticiesl = new List<List<Vector3>>();
        indicesl = new List<List<int>>();
        foreach (var (pointsRangeI, meshIndex) in points.original.Split(backboneRenderers.Count).Select((x, i) => (x, i)))
        {
            // Create mesh
            var pointsRange = pointsRangeI.ToList();

            Mesh mesh = new Mesh();
            backboneRenderers[meshIndex].mesh = mesh;

            var (verticies, indices) = createMeshConnectingPointsInRange(pointsRange.Select((p) => p.position).ToList(), lineWidth);

            mesh.Clear();
            mesh.vertices = verticies.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();

            // Add collider
            var meshCollider = backboneRenderers[meshIndex].gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }
    }

    void createGenesMesh()
    {
        List<(int start, int end)> getGeneSections()
        {
            var sections = new List<(int start, int end)>();
            var current_section = (genes[0].start, genes[0].end);
            foreach (var gene in genes.GetRange(1, genes.Count - 1))
            {
                if (current_section.end < gene.start)
                {
                    sections.Add(current_section);
                    current_section = (gene.start, gene.end);
                }
                else
                {
                    current_section = (current_section.start, gene.end);
                }
            }
            return sections;
        }

        var geneSections = getGeneSections();
        var genePointses = new List<List<Vector3>>();
        foreach (var (start, end) in geneSections)
        {
            var startBackboneIndex = start / basePairsPerRow;
            var endBackboneIndex = end / basePairsPerRow;
            // Once I integrate Hao's new file that tells me what genes he skipped, this should be fixed, the assert can be uncommented, and the next two lines after it can be removed
            // Assert.IsTrue(endBackboneIndex <= points.original.Count, "Too many genes >:("); 
            var startBackboneIndexHACK = Mathf.Min(startBackboneIndex, points.original.Count - 1);
            var endBackboneIndexHACK = Mathf.Min(endBackboneIndex, points.original.Count - 1);
            Assert.IsTrue(startBackboneIndexHACK <= endBackboneIndexHACK, "start index should be before end index - this is my fault");
            genePointses.Add(points.original.GetRange(startBackboneIndexHACK, endBackboneIndexHACK - startBackboneIndexHACK).Select((v) => v.position).ToList());
        }

        foreach (var (genePointsCurrent, geneRendererIndex) in genePointses.Split(geneRenderers.Count).Select((x, i) => (x, i)))
        {
            Mesh mesh = new Mesh();
            geneRenderers[geneRendererIndex].mesh = mesh;


            var verticies = new List<Vector3>();
            var indices = new List<int>();
            foreach (var genePoints in genePointsCurrent)
            {
                var (verticiesToAdd, indicesToAdd) = createMeshConnectingPointsInRange(genePoints, lineWidth * 2f);
                var preexistingVerticies = verticies.Count;
                verticies.AddRange(verticiesToAdd);
                indices.AddRange(indicesToAdd.Select((i) => i + preexistingVerticies));
            }


            mesh.Clear();
            mesh.vertices = verticies.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
        }
    }

    void createChromatidInterationPredictionLines()
    {
        foreach (var (start, end) in chromatinInteractionPrediction)
        {
            try
            {
                Assert.IsTrue(start.start >= 0);
                Assert.IsTrue(start.end >= 0);
                Assert.IsTrue(end.start >= 0);
                Assert.IsTrue(end.end >= 0);
                Assert.IsTrue(start.start <= totalBasePairs);
                Assert.IsTrue(start.end <= totalBasePairs);
                Assert.IsTrue(end.start <= totalBasePairs);
                Assert.IsTrue(end.end <= totalBasePairs);
                var midpointStart = (start.start + start.end) / 2;
                var midpointEnd = (end.start + end.end) / 2;

                var bridge = Instantiate(BridgePrefab);
                var line = bridge.GetComponent<LineRenderer>();
                line.startWidth *= overallScale * lineWidth * 3;
                line.endWidth *= overallScale * lineWidth * 3;
                line.SetPositions(new Vector3[] { basePairIndexToPoint(midpointStart).position, basePairIndexToPoint(midpointEnd).position });
            }
            catch
            {
                //Debug.Log((start, end) + " is outside the range of [0, " + totalBasePairs + "]");
            }
        }
    }

    (List<Vector3> verticies, List<int> indices) createMeshConnectingPointsInRange(List<Vector3> points, float lineWidth)
    {
        var numsides = 5;

        var verticies = new List<Vector3>(points.Count * numsides * 2);
        var indices = new List<int>(points.Count * numsides * 3);
        if (points.Count > 2)
        {
            foreach (var (point0, point1, point2) in points.Zip(points.GetRange(1, points.Count - 1), (a, b) => (a, b)).Zip(points.GetRange(2, points.Count - 2), (first, c) => (first.a, first.b, c)))
            {
                var preexistingVerticies = verticies.Count;

                var direction = point1 - point0;
                var normal = Vector3.Cross(direction, randoVector).normalized * lineWidth;

                var verts = Enumerable.Range(0, numsides).Select((i) => Quaternion.AngleAxis(i * 360.0f / numsides, direction) * normal).SelectMany((v) => new List<Vector3>() { point0 + v, point1 + v }).ToList();
                var inds = Enumerable.Range(0, numsides).SelectMany((i) => new List<int>() { 2, 1, 0, 1, 2, 3 }.Select((j) => (i * 2 + j) % verts.Count).Select((j) => j + preexistingVerticies)).ToList();
                verticies.AddRange(verts);
                indices.AddRange(inds);
            }
        }

        return (verticies, indices);
    }



    public void loadMain()
    {
        cameraParentCachedPosition = (cameraParent.transform.position, cameraParent.transform.eulerAngles, cameraParent.transform.localScale);
        SceneManager.LoadScene("mainScene");
    }


    List<(string name, int start, int end)> getGenes()
    {
        var genes = new List<(string name, int start, int end)>();
        bool firstLine = true;
        int lastStart = 0;
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
                var chromosome = info[1];
                if (chromosome == "chr1")
                {
                    var name = info[6];
                    var start = int.Parse(info[2]);
                    var end = int.Parse(info[3]);
                    Assert.AreNotEqual(name, "");
                    genes.Add((name, start, end));

                    Assert.IsTrue(start >= lastStart, "gene " + name + " starts before its predecessor!");
                    lastStart = start;
                }
            }
        }

        genes.Sort(delegate ((string name, int start, int end) x, (string name, int start, int end) y)
        {
            return (x.start).CompareTo(y.start);
        });


        int index = 0;
        foreach (var gene in genes)
        {
            index++;
            if (!geneDict.ContainsKey(gene.name))
            {
                geneDict.Add(gene.name, (gene.start, gene.end, index));
            }
        }
        return genes;
    }

    List<(int start, int end)> readFileBed(TextAsset file)
    {
        var data = new List<(int start, int end)>();
        foreach (var line in file.text.Split('\n'))
        {
            var info = line.Split('\t');
            if (info[0] == "chr1")
            {
                var start = int.Parse(info[1]);
                var end = int.Parse(info[2]);
                data.Add((start, end));
            }

        }
        return data;
    }

    List<(int start, int end)> getGATA()
    {
        return readFileBed(GATA);
    }

    List<(int start, int end)> getCTCF()
    {
        return readFileBed(CTCF);
    }


    List<(int start, int end)> getIRF()
    {
        return readFileBed(IRF1);
    }

    List<((int start, int end) start, (int start, int end) end)> getChromatinInteractionPrediction()
    {
        List<((int start, int end) start, (int start, int end) end)> data = new List<((int start, int end) start, (int start, int end) end)>();

        foreach (var line in ChromatinInteractionPrediction.text.Split('\n'))
        {
            var info = line.Split('\t');
            if (info[0] == "chr1" && info[4] == "chr1")
            {
                var startStart = int.Parse(info[1]);
                var endStart = int.Parse(info[2]);
                var startEnd = int.Parse(info[5]);
                var endEnd = int.Parse(info[6]);
                data.Add(((start: startStart, end: endStart), (start: startEnd, end: endEnd)));
            }

        }
        return data;
    }


    (List<Point> original, List<Point> coarse) getPoints()
    {
        if (ChromosomeController.points.original != null && ChromosomeController.points.coarse != null)
        {
            return ChromosomeController.points;
        }
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

#if true

        //var removalOrder = GetSimplificationOrder(points);

        var pointsOriginal = points.ToList();
        var pointsCoarse = points.ToList();
        var pointsFine = points.ToList();

        /*
        for (var i = 0; i < numberOfRows * simplificationFactorCoarse; i++)
        {
            pointsCoarse.RemoveAt(removalOrder[i]);
        }


        for (var i = 0; i < numberOfRows * simplificationFactorFine; i++)
        {
            pointsFine.RemoveAt(removalOrder[i]);
        }
        */

        return (original: pointsOriginal, coarse: pointsCoarse);
#else
        var pointsOriginal = points.ToList();

        bool highQualityEnabled = true;
        var pointsCoarse = SimplificationHelpers.Simplify<Point>(
                        points,
                        (p1, p2) => p1.position == p2.position,
                        (p) => p.position.x * 1000,
                        (p) => p.position.y * 1000,
                        (p) => p.position.z * 1000,
                        .95,
                        highQualityEnabled
                        );


        return (original: pointsOriginal, coarse: pointsCoarse.ToList());
#endif
    }



    public void highlightGene(string name)
    {
        /*
        if (name == "") return;
        foreach (var geneRenderer in geneDict[name].renderer)
        {
            geneRenderer.material = highlightedColoredMaterial;
        }
        */
    }

    public void unhighlightGene(string name)
    {
        /*
        if (name == "" || name == focusedGene) return;
        foreach (var geneRenderer in geneDict[name].renderer)
        {
            geneRenderer.material = coloredMaterial;
        }
        */
    }


    public void focusGene(string name)
    {
        /*
        if (name == "") return;
        unfocusGene();
        focusedGene = name;
        foreach (var geneRenderer in geneDict[name].renderer)
        {
            geneRenderer.material = highlightedColoredMaterial;
        }
        */
    }

    public void unfocusGene()
    {
        /*
        if (focusedGene == "") return;
        foreach (var geneRenderer in geneDict[focusedGene].renderer)
        {
            geneRenderer.material = coloredMaterial;
            focusedGene = "";
        }
        */
    }

    public Point basePairIndexToPoint(int bpIndex)
    {
        Debug.Log(bpIndex);
        var a = points.original[bpIndex / basePairsPerRow];
        return a;
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
