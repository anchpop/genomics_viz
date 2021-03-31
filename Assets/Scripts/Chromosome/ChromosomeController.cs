using Supercluster.KDTree;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Valve.VR;
using Valve.VR.InteractionSystem;
using DG.Tweening;

using UnityEngine.Profiling;


public struct Point
{
    public Vector3 position;
    public int bin;
    public int originalIndex;
    public override string ToString() =>
        $"Point(position: {position}, bin: {bin}, originalIndex: {originalIndex})";
}

public class ChromosomeController : MonoBehaviour
{
    public float overallScale = 1.5f;
    public float lineWidth = 1;

    static (Vector3 position, Vector3 rotation, Vector3 scale) cameraParentCachedPosition;
    public GameObject cameraParent;
    public TextAsset locationSequence;
    public TextAsset coordinateMapping;
    public TextAsset geneAnnotations;
    public TextAsset GATA;
    public TextAsset CTCF;
    public TextAsset IRF1;
    public TextAsset ChromatinInteractionPrediction;
    public static (List<Point> original, bool dummy) points; // dummy is only here because I wanted this to be a tuple and tuples need at least two elements
    public static List<(string name, int start, int end, bool direction)> genes;
    public static KDTree<float, int> geneWorldPositions;
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



    // the plan for showing text: we're going to make another kdtree, except this one stores the positions of each gene. 
    // We create some text gameobjects and leave them disabled at the beginning.
    // We get the genes closest to the camera with the kdtree, then position the text gameobjects to display them
    // then profit :sunglasses:
    // once I do that, the last thing I want to do for today is fix the 1D view, idk why it's so goddamn janky


    void Start()
    {
        if (cameraParentCachedPosition.position != Vector3.zero || cameraParentCachedPosition.rotation != Vector3.zero || cameraParentCachedPosition.scale != Vector3.zero)
        {
            cameraParent.transform.position = cameraParentCachedPosition.position;
            cameraParent.transform.eulerAngles = cameraParentCachedPosition.rotation;
            cameraParent.transform.localScale = cameraParentCachedPosition.scale;
        }


        geneDict = new KTrie.StringTrie<(int start, int end, int index)>();
        Profiler.BeginSample("getPoints");
        points = getPoints();
        Profiler.EndSample();
        Profiler.BeginSample("getGenes");
        (genes, geneWorldPositions) = getGenes();
        Profiler.EndSample();
        Profiler.BeginSample("getGata");
        gata = getGATA();
        Profiler.EndSample();
        Profiler.BeginSample("getCTCF");
        ctcf = getCTCF();
        Profiler.EndSample();
        Profiler.BeginSample("getIRF");
        irf = getIRF();
        Profiler.EndSample();
        Profiler.BeginSample("getChromatinInteractionPrediction");
        chromatinInteractionPrediction = getChromatinInteractionPrediction();
        Profiler.EndSample();

        randoVector = Random.insideUnitSphere;

        Profiler.BeginSample("createBackboneMesh");
        createBackboneMesh();
        Profiler.EndSample();
        Profiler.BeginSample("createGenesMesh");
        createGenesMesh();
        Profiler.EndSample();
        Profiler.BeginSample("createChromatidInterationPredictionLines");
        //createChromatidInterationPredictionLines();
        Profiler.EndSample();


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
            pointsAdded += lastMesh ? pointsRange.Count() : pointsRange.Count() - 2;
        }
        Assert.AreEqual(points.original.Count - 1, backbonePointNormals.Count);
    }

    (List<Vector3> points, int startBackboneIndex) getPointsConnectingBpIndices(int startBasePairIndex, int endBasePairIndex)
    {
        var startBackboneIndex = basePairIndexToLocationIndex(startBasePairIndex);
        var endBackboneIndex = basePairIndexToLocationIndex(endBasePairIndex);
        Assert.IsTrue(startBackboneIndex <= endBackboneIndex, "start index should be before end index - this is my fault");

        var startPoint = basePairIndexToPoint(startBasePairIndex);
        var endPoint = basePairIndexToPoint(endBasePairIndex);
        if (startBackboneIndex == endBackboneIndex)
        {
            return (new List<Vector3> { startPoint, endPoint }, startBackboneIndex);
        }
        else
        {
            var l = new List<Vector3> { startPoint };
            l.AddRange(points.original.GetRange(startBackboneIndex + 1, endBackboneIndex - (startBackboneIndex)).Select((v) => v.position));
            l.Add(endPoint);
            return (l, startBackboneIndex);
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
                    current_section = (current_section.start, Mathf.Max(gene.end, current_section.end));
                }
            }
            sections.Add(current_section);
            return sections;
        }


        var geneSections = getGeneSections();
        var genePointGroups = new List<(List<Vector3> genePoints, int startingBackboneindex)>();
        foreach (var (start, end) in geneSections)
        {
            genePointGroups.Add(getPointsConnectingBpIndices(start, end));
        }

        foreach (var (genePointGroupsForCurrentGeneRenderer, geneRendererIndex) in genePointGroups.Split(geneRenderers.Count).Select((x, i) => (x, i)))
        {
            Mesh mesh = new Mesh();
            geneRenderers[geneRendererIndex].mesh = mesh;


            var verticies = new List<Vector3>();
            var indices = new List<int>();
            foreach (var (genePoints, startingBackboneIndex) in genePointGroupsForCurrentGeneRenderer)
            {
                Assert.AreNotEqual(genePoints.Count, 0);
                Assert.AreNotEqual(genePoints.Count, 1);
                if (startingBackboneIndex >= backbonePointNormals.Count) // should only be the case for genes that start on the very last point
                {
                }
                else
                {
                    var startNormals = backbonePointNormals[startingBackboneIndex];

                    var (verticiesToAdd, indicesToAdd, _, _) = createMeshConnectingPointsInRange(genePoints, startNormals.Select((v) => v * 1.1f + genePoints[0]).ToList(), true);
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
                line.SetPositions(new Vector3[] { basePairIndexToPoint(midpointStart), basePairIndexToPoint(midpointEnd) });
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


    (List<(string name, int start, int end, bool direction)> genes, KDTree<float, int> geneWorldPositions) getGenes()
    {
        var genes = new List<(string name, int start, int end, bool direction)>();

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

                    var direction = info[4] == "+";

                    Assert.AreNotEqual(name, "");
                    genes.Add((name, start, end, direction));

                    Assert.IsTrue(start >= lastStart, "gene " + name + " starts before its predecessor!");
                    lastStart = start;
                }

            }
        }

        genes.Sort(delegate ((string name, int start, int end, bool direction) x, (string name, int start, int end, bool direction) y)
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

        float[][] points = genes.Select(gene =>
        {
            var originBasePair = gene.direction ? gene.start : gene.end;
            var originPosition = basePairIndexToPoint(originBasePair);
            return new float[] { originPosition.x, originPosition.y, originPosition.z };
        }).ToArray();
        int[] nodes = genes.Select((x, i) => i).ToArray();
        var geneWorldPositions = new KDTree<float, int>(3, points, nodes, (f1, f2) => (new Vector3(f1[0], f1[1], f1[2]) - new Vector3(f2[0], f2[1], f2[2])).magnitude);



        return (genes, geneWorldPositions);
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

    public void highlightArea(MeshFilter renderer, (string name, int startBpIndex, int endBpIndex, bool direction) info)
    {
        var genePoints = getPointsConnectingBpIndices(info.startBpIndex, info.endBpIndex);

        Assert.AreNotEqual(genePoints.points.Count, 0);
        Assert.AreNotEqual(genePoints.points.Count, 1);

        Mesh mesh = new Mesh();
        renderer.mesh = mesh;

        var startNormals = backbonePointNormals[genePoints.startBackboneIndex];
        var startingPoints = startNormals.Select((v) => v * 1.2f + genePoints.points[0]).ToList();
        var (verticies, indices, _, _) = createMeshConnectingPointsInRange(genePoints.points, startingPoints, true);

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

    public void highlightGene((string name, int start, int end, bool direction) info)
    {
        highlightArea(highlightRenderer, info);
    }

    public void unhighlightGene()
    {
        highlightRenderer.mesh.Clear();
    }


    public void focusGene((string name, int start, int end, bool direction) info)
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
    public IEnumerable<(string name, int start, int end, bool direction)> getGenesAtBpIndex(int bpIndex)
    {
        return from gene in genes
               where gene.start <= bpIndex
               where bpIndex <= gene.end
               select gene;
    }

    public int basePairIndexToLocationIndex(int bpIndex)
    {
        if (bpIndex <= points.original[0].bin) return 0;
        if (bpIndex >= points.original[points.original.Count - 1].bin) return points.original.Count - 1;

        // var node = points.basePairMapping.GetNearestNeighbours(new float[] { bpIndex }, 1);
        //var a = node[0].Value;

        var index = points.original.Select((p) => p.bin).ToList().BinarySearch(bpIndex);
        if (index < 0)
        {
            index = ~index; // index of the first element that is larger than the search value
            index -= 1;
        }
        index = index >= points.original.Count ? points.original.Count - 1 : index;
        index = index < 0 ? 0 : index;




        //var a = bpIndex / basePairsPerRow;
        return index;
    }

    public Vector3 basePairIndexToPoint(int bpIndex)
    {
        if (bpIndex <= points.original[0].bin)
        {
            return points.original[0].position;
        }
        else if (bpIndex >= points.original[points.original.Count - 1].bin)
        {
            return points.original[points.original.Count - 1].position;
        }
        else
        {
            var locationIndex = basePairIndexToLocationIndex(bpIndex);

            var a = points.original[locationIndex];
            var b = points.original[locationIndex + 1];
            Assert.IsTrue(a.bin <= bpIndex);
            Assert.IsTrue(bpIndex <= b.bin);
            return Vector3.Lerp(a.position, b.position, Mathf.InverseLerp(a.bin, b.bin, bpIndex));
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
