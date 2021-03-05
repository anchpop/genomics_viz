﻿using KdTree;
using KdTree.Math;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public struct Point
{
    public Vector3 position;
    public int bin;
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
    public TextAsset coordinateMapping;
    public TextAsset geneAnnotations;
    public TextAsset GATA;
    public TextAsset CTCF;
    public TextAsset IRF1;
    public TextAsset ChromatinInteractionPrediction;
    public static (List<Point> original, bool dummy) points; // dummy is only here because I wanted this to be a tuple and tuples need at least two elements
    public static List<(string name, int start, int end)> genes;
    public List<(int start, int end)> gata;
    public List<(int start, int end)> ctcf;
    public List<(int start, int end)> irf;
    public List<((int start, int end) start, (int start, int end) end)> chromatinInteractionPrediction;

    private List<List<Vector3>> backbonePointNormals;

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
    public GameObject bridgePrefab;

    public GameObject bridgeParent;

    public Material coloredMaterial;
    public Material highlightedColoredMaterial;

    public GameObject geneTextCanvas;

    public float cartoonAmount = .1f;

    // Unity places a limit on the number of verts on a mesh that's quite a bit lower than the amount we need
    // So, we need to use multiple meshes, which means multiple mesh renderers
    public List<MeshFilter> backboneRenderers;
    public List<MeshFilter> geneRenderers;
    public MeshFilter highlightRenderer;
    public MeshFilter focusRenderer;

    public int numsides = 3;

    public KTrie.StringTrie<(int start, int end, int index)> geneDict;

    static public int totalBasePairs = 0;
    static public int basePairsPerRow = 5000;

    public string focusedGene = "";


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

        randoVector = Random.insideUnitSphere;

        createBackboneMesh();
        createGenesMesh();
        createChromatidInterationPredictionLines();


    }

    void createBackboneMesh()
    {
        backbonePointNormals = new List<List<Vector3>>();

        var verticiesl = new List<List<Vector3>>();
        var indicesl = new List<List<int>>();
        var pointsAdded = 0;

        var lastpoints = new List<Vector3>();

        var pointss = points.original.Split(backboneRenderers.Count).Select((x, i) => (points: x.ToList(), meshIndex: i)).ToList();

        foreach (var (pointsRangeI, meshIndex) in pointss)
        {
            var lastMesh = meshIndex == backboneRenderers.Count - 1;
            // Create mesh
            var pointsRange = !lastMesh ?
                pointsRangeI.Append(pointss[meshIndex + 1].points[0]).Append(pointss[meshIndex + 1].points[1])
                : pointsRangeI.AsEnumerable();

            Mesh mesh = new Mesh();
            backboneRenderers[meshIndex].mesh = mesh;

            var (verticies, indices, normies, lastpointsp) = lastpoints.Count == 0 ?
                createMeshConnectingPointsInRange(pointsRange.Select((p) => p.position).ToList(), lineWidth, false) :
                createMeshConnectingPointsInRange(pointsRange.Select((p) => p.position).ToList(), lastpoints, false);
            lastpoints = lastpointsp;

            Assert.AreEqual(pointsRange.Count() - 1, normies.Count);
            backbonePointNormals.AddRange(normies.GetRange(0, !lastMesh ? normies.Count - 1 : normies.Count));

            mesh.Clear();
            mesh.vertices = verticies.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();

            // Add collider
            var meshCollider = backboneRenderers[meshIndex].gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;


            // Set up renderer info
            var chromosomeSubrenderer = backboneRenderers[meshIndex].gameObject.GetComponent<ChromosomePart>();
            chromosomeSubrenderer.addPoints(pointsRange, pointsAdded);
            pointsAdded += pointsRange.Count();
        }
        Debug.Log(points.original.Count);
        Assert.AreEqual(points.original.Count - 1, backbonePointNormals.Count);
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
                    current_section = (current_section.start, Mathf.Max(gene.end, current_section.end));
                }
            }
            sections.Add(current_section);
            return sections;
        }


        var geneSections = getGeneSections();
        var genePointses = new List<List<Vector3>>();
        foreach (var (start, end) in geneSections)
        {
            var startBackboneIndex = basePairIndexToLocationIndex(start);
            var endBackboneIndex = basePairIndexToLocationIndex(end);
            Assert.IsTrue(startBackboneIndex <= endBackboneIndex, "start index should be before end index - this is my fault");
            genePointses.Add(points.original.GetRange(startBackboneIndex, endBackboneIndex - startBackboneIndex).Select((v) => v.position).ToList());
        }

        foreach (var (genePointsCurrent, geneRendererIndex) in genePointses.Split(geneRenderers.Count).Select((x, i) => (x, i)))
        {
            Mesh mesh = new Mesh();
            geneRenderers[geneRendererIndex].mesh = mesh;


            var verticies = new List<Vector3>();
            var indices = new List<int>();
            foreach (var genePoints in genePointsCurrent)
            {
                // Assert.AreNotEqual(genePoints.Count, 0); // WTF? todo, investigate why this is sometimes true 
                if (genePoints.Count > 1) // todo - put in next bin
                {
                    var (verticiesToAdd, indicesToAdd, _, _) = createMeshConnectingPointsInRange(genePoints, lineWidth * 1.1f);
                    var preexistingVerticies = verticies.Count;
                    verticies.AddRange(verticiesToAdd);
                    indices.AddRange(indicesToAdd.Select((i) => i + preexistingVerticies));
                }

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

                // TODO: putting all these lines in seperate components has a substantial performance cost - can I combine them into one mesh like I do with the bridges? 
                // It would add a lot of tris, but it would move work from the CPU to the GPU. 
                var bridge = Instantiate(bridgePrefab, bridgeParent.transform);
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

    (List<Vector3> verticies, List<int> indices, List<List<Vector3>> normalsAtPoint, List<Vector3> lastPoints) createMeshConnectingPointsInRange(List<Vector3> points, float lineWidth, bool extrudeEnd = true)
    {

        List<Vector3> cylinderExtrusion(Vector3 p, Vector3 direction)
        {
            var normal = Vector3.Cross(direction, randoVector).normalized * lineWidth;
            var verts = Enumerable.Range(0, numsides).Select((i) => Quaternion.AngleAxis(i * 360.0f / numsides, direction) * normal).Select((v) => p + v).ToList();
            return verts;
        }

        return createMeshConnectingPointsInRange(points, cylinderExtrusion(points[0], points[1] - points[0]), extrudeEnd);
    }

    (List<Vector3> verticies, List<int> indices, List<List<Vector3>> normalsAtPoint, List<Vector3> lastPoints) createMeshConnectingPointsInRange(List<Vector3> points, List<Vector3> startingPoints, bool extrudeEnd)
    {
        // Thanks to http://www.songho.ca/math/line/line.html#intersect_lineplane
        Vector3 intersectLineAndPlane(Vector3 linePoint, Vector3 lineDirection, Vector3 planePoint, Vector3 planeNormal)
        {
            if (Vector3.Dot(planeNormal, linePoint) == 0)
            {
                return Vector3.zero; // not sure why but this is bad
            }

            float t = (Vector3.Dot(planeNormal, planePoint) - Vector3.Dot(planeNormal, linePoint)) / Vector3.Dot(planeNormal, lineDirection.normalized);
            return linePoint + (lineDirection.normalized * t);
        }

        if (points.Count >= 3)
        {
            var verticies = new List<Vector3>(points.Count * numsides + numsides * 2);
            var indices = new List<int>(points.Count * numsides * 3);
            var normalsAtPoint = new List<List<Vector3>>(points.Count);

            var lastPoints = startingPoints;
            normalsAtPoint.Add(lastPoints.Select((x, i) => x - points[0]).ToList());

            verticies.AddRange(lastPoints);
            var endPoint = points[points.Count - 1];
            var endPointDir = endPoint - points[points.Count - 2];
            var points_appended = extrudeEnd ? points.Append(endPoint + endPointDir) : points.AsEnumerable();
            foreach (
                var (Q_1, Q_2, Q_3)
                in points_appended
                     .Zip(points.GetRange(1, points.Count - 1), (a, b) => (a, b))
                     .Zip(points.GetRange(2, points.Count - 2), (first, c) => (first.a, first.b, c))
                    )
            {

                var preexistingVerticies = verticies.Count;

                // thanks to http://www.songho.ca/opengl/gl_cylinder.html#pipe
                var v_1 = Q_2 - Q_1;
                var v_2 = Q_3 - Q_2;

                // apparently you can calculate a normal vector this way lol? It doesn't say to normalize them first but I will just in case that's an assumption they make
                var n = v_1.normalized + v_2.normalized;
                // except if the directions were the same or opposite, the normal vector will be zero, and that's not what we want
                if (n == Vector3.zero)
                {
                    n = Vector3.Cross(v_1, randoVector).normalized;
                }


                lastPoints = lastPoints.Select((p, i) => intersectLineAndPlane(p, v_1, Q_2, n)).ToList();

                verticies.AddRange(lastPoints);

                var inds = Enumerable.Range(0, numsides).SelectMany((sideIndex) =>
                    new List<int>() {
                        (0        + sideIndex) % numsides,
                        (1        + sideIndex) % numsides,
                        (0        + sideIndex) % numsides + numsides,

                        (1            + sideIndex) % numsides,
                        (1            + sideIndex) % numsides + numsides,
                        (0            + sideIndex) % numsides + numsides,}
                    /*Enumerable.Range(0, numsides).SelectMany((j) => 
                        
                    )*/

                    .Select((j) => j - numsides)
                    .Select((j) => j + preexistingVerticies)
                );
                indices.AddRange(inds);


                normalsAtPoint.Add(lastPoints.Select((x, i) => x - Q_2).ToList());
            }
            return (verticies, indices, normalsAtPoint, lastPoints);
        }
        else
        {
            return (new List<Vector3> { }, new List<int> { }, new List<List<Vector3>> { }, new List<Vector3> { });
        }



    }



    public void loadMain()
    {
        cameraParentCachedPosition = (cameraParent.transform.position, cameraParent.transform.eulerAngles, cameraParent.transform.localScale);
        SceneManager.LoadScene("mainScene");
    }


    List<(string name, int start, int end)> getGenes()
    {
        var genes = new List<(string name, int start, int end)>();

        int lastStart = 0;

        var splitAnnotations = geneAnnotations.text.Split('\n').Skip(1).ToArray();


        for (int i = 0; i < splitAnnotations.Length; i++)
        {
            var annotationLine = splitAnnotations[i];

            if (annotationLine != "")
            {
                // look at gene positions
                var info = annotationLine.Split('\t');
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
            if (!geneDict.ContainsKey(gene.name))
            {
                geneDict.Add(gene.name, (gene.start, gene.end, index));
            }
            index++;
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


    (List<Point> original, bool dummy) getPoints()
    {
        if (ChromosomeController.points.original != null)
        {
            return ChromosomeController.points;
        }

        var center = Vector3.zero;

        var pointsRaw = new List<(Vector3 point, int bin)>();

        var points = new List<Point>();

        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;


        var splitCoordinateMapping = coordinateMapping.text.Split('\n').ToArray();
        var splitLocationSequence = locationSequence.text.Split('\n').ToArray();
        Assert.AreEqual(splitCoordinateMapping.Length, splitLocationSequence.Length);

        //var basePairMapping = new KdTree<float, int>(1, new FloatMath());

        for (int i = 0; i < splitLocationSequence.Length; i++)
        {
            var locationSequenceLine = splitLocationSequence[i];
            var mappingLine = splitCoordinateMapping[i];
            if (locationSequenceLine != "" && mappingLine != "")
            {
                // look at coordinate mapping
                var info = mappingLine.Split('\t');
                var bin = int.Parse(info[0]);
                var associatedIndex = int.Parse(info[1]);
                //basePairMapping.Add(new float[] { bin }, associatedIndex);
                totalBasePairs = bin;

                // look at coordinate sequence
                var coords = locationSequenceLine.Split('\t');
                var point = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));

                if (point.x < min.x)
                {
                    min = new Vector3(point.x, min.y, min.z);
                }
                if (point.y < min.y)
                {
                    min = new Vector3(min.x, point.y, min.z);
                }
                if (point.z < min.z)
                {
                    min = new Vector3(min.x, min.y, point.z);
                }
                if (point.x > max.x)
                {
                    max = new Vector3(point.x, max.y, max.z);
                }
                if (point.y > max.y)
                {
                    max = new Vector3(max.x, point.y, max.z);
                }
                if (point.z > max.z)
                {
                    max = new Vector3(max.x, max.y, point.z);
                }

                pointsRaw.Add((point, bin));
                center += point;
            }
        }

        center = center / pointsRaw.Count;

        var count = 0;
        foreach (var (point, bin) in pointsRaw)
        {
            var scaling = Mathf.Max(Mathf.Max(min.x - max.x, min.y - max.y), min.z - max.z);
            var p = new Point();
            p.position = (point - center) * overallScale / scaling;
            p.originalIndex = count;
            p.bin = bin;

            points.Add(p);
            count++;
        }
#if true

        //var removalOrder = GetSimplificationOrder(points);


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

        return (original: points, dummy: false/*, basePairMapping*/);
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

    public void highlightArea(MeshFilter renderer, (string name, int start, int end) info)
    {
        var startBackboneIndex = basePairIndexToLocationIndex(info.start);
        var endBackboneIndex = basePairIndexToLocationIndex(info.end);
        // Once I integrate Hao's new file that tells me what genes he skipped, this should be fixed, the assert can be uncommented, and the next two lines after it can be removed
        // Assert.IsTrue(endBackboneIndex <= points.original.Count, "Too many genes >:("); 
        var startBackboneIndexHACK = Mathf.Min(startBackboneIndex, points.original.Count - 1);
        var endBackboneIndexHACK = Mathf.Min(endBackboneIndex, points.original.Count - 1);
        Assert.IsTrue(startBackboneIndexHACK <= endBackboneIndexHACK, "start index should be before end index - this is my fault");
        var genePoints = points.original.GetRange(startBackboneIndexHACK, endBackboneIndexHACK - startBackboneIndexHACK).Select((v) => v.position).ToList();

        Mesh mesh = new Mesh();
        renderer.mesh = mesh;


        var (verticies, indices, _, _) = createMeshConnectingPointsInRange(genePoints, lineWidth * 1.2f);

        mesh.Clear();
        mesh.vertices = verticies.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        /*
        if (name == "") return;
        foreach (var geneRenderer in geneDict[name].renderer)
        {
            geneRenderer.material = highlightedColoredMaterial;
        }
        */
    }

    public void highlightGene((string name, int start, int end) info)
    {
        highlightArea(highlightRenderer, info);
    }

    public void unhighlightGene()
    {
        highlightRenderer.mesh.Clear();
    }


    public void focusGene((string name, int start, int end) info)
    {
        if (name == "") return;
        focusedGene = info.name;

        highlightArea(focusRenderer, info);
    }

    public void unfocusGene()
    {
        focusedGene = "";
        focusRenderer.mesh.Clear();
    }

    // TODO: This could be sped up with a binary search, or with the kd tree tech
    public IEnumerable<(string name, int start, int end)> getGenesAtBpIndex(int bpIndex)
    {
        return from gene in genes
               where gene.start <= bpIndex
               where bpIndex <= gene.end
               select gene;
    }

    public int basePairIndexToLocationIndex(int bpIndex)
    {
        if (bpIndex <= 525000) return 0;
        if (bpIndex >= 249230000) return points.original.Count - 1;

        // var node = points.basePairMapping.GetNearestNeighbours(new float[] { bpIndex }, 1);
        //var a = node[0].Value;

        var index = points.original.Select((p) => p.bin).ToList().BinarySearch(bpIndex);
        if (index < 0)
        {
            index = ~index; // index of the first element that is larger than the search value
        }
        index = index >= points.original.Count ? points.original.Count - 1 : index;




        //var a = bpIndex / basePairsPerRow;
        return index;
    }

    public Point basePairIndexToPoint(int bpIndex)
    {
        var a = points.original[basePairIndexToLocationIndex(bpIndex)];
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
