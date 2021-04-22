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
using CapnpGen;
using Capnp;
using OneOf;

using System.IO;
using UnityEngine.Profiling;

using Segment = OneOf.OneOf<
    CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>,
    CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>>;
using SegmentList =
    OneOf.OneOf<
        System.Collections.Generic.List<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>>,
        System.Collections.Generic.List<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>>
    >;
using SegmentInfo = System.Collections.Generic.Dictionary<
    string,
    (
        OneOf.OneOf<
            System.Collections.Generic.List<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>>,
            System.Collections.Generic.List<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>>> segments,
        Supercluster.KDTree.KDTree<float, int> worldPositions,
    KTrie.StringTrie<int> nameDict)
>;


public struct Point
{
    public Vector3 position;
    public int bin;
    public int originalIndex;
    public override string ToString() =>
        $"Point(position: {position}, bin: {bin}, originalIndex: {originalIndex})";
}

public struct ChromosomeSetRenderingInfo
{
    public ChromosomeSet chromosomeSet;
    public Dictionary<string, Chromosome> chromosomes;
}

public struct ChromosomeRenderingInfo
{
    public Chromosome chromosome;
    public List<Point> points;
    public SegmentInfo segmentInfos;
    public int highestBin;
}

public class ChromosomeController : MonoBehaviour
{
    public float overallScale = 1.5f;
    public float lineWidth = 1;

    public GameObject cameraParent;
    public TextAsset locationSequence;
    public TextAsset coordinateMapping;
    public TextAsset geneAnnotations;

    public TextAsset GATA;
    public TextAsset CTCF;
    public TextAsset IRF1;
    public TextAsset ChromatinInteractionPrediction;

    public static ChromosomeSetRenderingInfo chromosomeSetRenderingInfo;
    public static ChromosomeRenderingInfo chromosomeRenderingInfo;
    string currentlyRenderingSetIndex = "1";

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
    public GameObject backboneRenderers;
    public GameObject segmentRenderers;
    public List<MeshFilter> backboneMeshFilters;
    public Dictionary<int, List<MeshFilter>> segmentMeshFilters;

    public MeshFilter highlightRenderer;
    public MeshFilter focusRenderer;

    public int numsides = 3;

    Vector3 randoVector;

    void Start()
    {
        chromosomeSetRenderingInfo = getChromosomeSetRenderingInfo();

        chromosomeRenderingInfo = createRenderingInfo(chromosomeSetRenderingInfo, currentlyRenderingSetIndex);

        backboneMeshFilters = backboneRenderers.GetComponentsInChildren<MeshFilter>().ToList();
        segmentMeshFilters = chromosomeRenderingInfo.segmentInfos.Keys.Select((name, index) => (name, index)).ToDictionary(k => k.index, k =>
            {
                var renderers = Instantiate(segmentRenderers, segmentRenderers.transform.parent.transform);
                renderers.name = k.name;
                return renderers.GetComponentsInChildren<MeshFilter>().ToList();
            }
        );


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
        createSegmentsMeshes(chromosomeRenderingInfo);
        Profiler.EndSample();
        Profiler.BeginSample("createChromatidInterationPredictionLines");
        createChromatidInterationPredictionLines();
        Profiler.EndSample();


    }

    ChromosomeSetRenderingInfo getChromosomeSetRenderingInfo()
    {
        ChromosomeSet getSet()
        {
            var output_dir_path = Path.Combine(Settings.dataUrl, "Output");
            var info_file_path = Path.Combine(output_dir_path, "info.chromsdata");

            using var fs = File.OpenRead(info_file_path);
            var frame = Framing.ReadSegments(fs);
            var deserializer = DeserializerState.CreateRoot(frame);
            var set = CapnpSerializable.Create<ChromosomeSet>(deserializer);
            Debug.Log("Read " + set.Chromosomes.Count + " chromosomes");
            return set;// new ChromosomeSet.READER(deserializer);
        }

        Dictionary<string, Chromosome> getChromosomes(ChromosomeSet set)
        {
            var result = new Dictionary<string, Chromosome>();
            foreach (var chromosome in set.Chromosomes)
            {
                Assert.AreNotEqual(chromosome.Index.which, Chromosome.index.WHICH.undefined);

                if (chromosome.Index.which == Chromosome.index.WHICH.Numbered)
                {
                    result[chromosome.Index.Numbered.ToString()] = chromosome;
                }
                else if (chromosome.Index.which == Chromosome.index.WHICH.X)
                {
                    result["X"] = chromosome;
                }
                else if (chromosome.Index.which == Chromosome.index.WHICH.Y)
                {
                    result["Y"] = chromosome;
                }
                else
                {
                    Debug.LogError("Unknown type!");
                }

            }
            return result;
        }

        var set = getSet();
        var chromosomes = getChromosomes(set);


        var info = new ChromosomeSetRenderingInfo { };
        info.chromosomeSet = set;
        info.chromosomes = chromosomes;

        return info;
    }

    ChromosomeRenderingInfo createRenderingInfo(ChromosomeSetRenderingInfo chromosomeSetRenderingInfo, string currentlyRenderingSetIndex)
    {
        var chromosome = chromosomeSetRenderingInfo.chromosomes[currentlyRenderingSetIndex];

        List<Point> getPoints()
        {
            var center = Vector3.zero;

            var pointsRaw = new List<(Vector3 point, int bin)>();

            var points = new List<Point>();

            Vector3 min = Vector3.zero;
            Vector3 max = Vector3.zero;


            var splitCoordinateMapping = coordinateMapping.text.Split('\n').ToArray();
            var splitLocationSequence = locationSequence.text.Split('\n').ToArray();
            Assert.AreEqual(splitCoordinateMapping.Length, splitLocationSequence.Length);

            //var basePairMapping = new KdTree<float, int>(1, new FloatMath());

            foreach (var pointI in chromosome.Backbone)
            {
                var point = new Vector3(pointI.Coordinate.X, pointI.Coordinate.Y, pointI.Coordinate.Z);

                var bin = pointI.Bin;
                pointsRaw.Add((point, bin: checked((int)bin)));
                center += point;


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

            return points;
        }

        SegmentInfo getSegmentInfo(List<Point> points)
        {
            var segmentInfo = new SegmentInfo();

            foreach (var segmentSet in chromosome.SegmentSets)
            {

                if (segmentSet.Segments.which == Chromosome.SegmentSet.segments.WHICH.Genes)
                {
                    var segments = segmentSet.Segments.Genes.ToList();
                    segments.Sort(delegate (Chromosome.SegmentSet.Segment<Chromosome.SegmentSet.Gene> x, Chromosome.SegmentSet.Segment<Chromosome.SegmentSet.Gene> y)
                    {
                        return (x.Location.StartBin).CompareTo(y.Location.StartBin);
                    });


                    var nameDict = new KTrie.StringTrie<int>();

                    foreach (var (segment, index) in segments.Select((segment, index) => (segment, index)))
                    {
                        // TODO: ask hao why this is sometimes true
                        // Assert.IsFalse(nameDict.ContainsKey(segment.Name), segment.Name + " appears multiple times in SegmentSet!");
                        if (!nameDict.ContainsKey(segment.ExtraInfo.Name))
                        {
                            nameDict.Add(segment.ExtraInfo.Name, index);
                        }
                    }

                    float[][] segment_points = segments.Select(segment =>
                    {
                        var originBin = checked((int)(segment.ExtraInfo.Ascending ? segment.Location.StartBin : segment.Location.EndBin));
                        var originPosition = basePairIndexToPoint(points, originBin);
                        return new float[] { originPosition.x, originPosition.y, originPosition.z };
                    }).ToArray();
                    int[] nodes = segments.Select((x, i) => i).ToArray();
                    var worldPositions = new KDTree<float, int>(3, segment_points, nodes, (f1, f2) => (new Vector3(f1[0], f1[1], f1[2]) - new Vector3(f2[0], f2[1], f2[2])).magnitude);

                    segmentInfo[segmentSet.Description.Name] = (segments, worldPositions, nameDict);
                }
                else
                {
                    Debug.LogError("Currently, we only support gene segments!");
                }
            }

            return segmentInfo;
        }

        var info = new ChromosomeRenderingInfo { };

        info.chromosome = chromosome;

        Profiler.BeginSample("getPoints");
        info.points = getPoints();
        info.highestBin = info.points.Last().bin;
        Profiler.EndSample();

        Profiler.BeginSample("getGenes");
        info.segmentInfos = getSegmentInfo(info.points);
        Profiler.EndSample();

        return info;
    }

    void createBackboneMesh()
    {
        backbonePointNormals = new List<List<Vector3>>();

        var verticiesl = new List<List<Vector3>>();
        var indicesl = new List<List<int>>();
        var pointsAdded = 0;

        var lastpoints = new List<Vector3>();

        var pointss = chromosomeRenderingInfo.points.Split(backboneMeshFilters.Count).Select((x, i) => (points: x.ToList(), meshIndex: i)).ToList();

        foreach (var (pointsRangeI, meshIndex) in pointss)
        {
            var lastMesh = meshIndex == backboneMeshFilters.Count - 1;
            // Create mesh
            var pointsRange = !lastMesh ?
                pointsRangeI.Append(pointss[meshIndex + 1].points[0]).Append(pointss[meshIndex + 1].points[1])
                : pointsRangeI.AsEnumerable();

            Mesh mesh = new Mesh();
            backboneMeshFilters[meshIndex].mesh = mesh;

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
            var meshCollider = backboneMeshFilters[meshIndex].gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;


            // Set up renderer info
            var chromosomeSubrenderer = backboneMeshFilters[meshIndex].gameObject.GetComponent<ChromosomePart>();
            chromosomeSubrenderer.addPoints(pointsRange, pointsAdded);
            pointsAdded += lastMesh ? pointsRange.Count() : pointsRange.Count() - 2;
        }
        Assert.AreEqual(chromosomeRenderingInfo.points.Count - 1, backbonePointNormals.Count);
    }

    (List<Vector3> points, int startBackboneIndex) getPointsConnectingBpIndices(int startBasePairIndex, int endBasePairIndex)
    {
        var startBackboneIndex = basePairIndexToLocationIndex(startBasePairIndex);
        var endBackboneIndex = basePairIndexToLocationIndex(endBasePairIndex);
        Assert.IsTrue(startBackboneIndex <= endBackboneIndex, "start index should be before end index - this is my fault");

        var startPoint = binToPoint(startBasePairIndex);
        var endPoint = binToPoint(endBasePairIndex);
        if (startBackboneIndex == endBackboneIndex)
        {
            return (new List<Vector3> { startPoint, endPoint }, startBackboneIndex);
        }
        else
        {
            var l = new List<Vector3> { startPoint };
            l.AddRange(chromosomeRenderingInfo.points.GetRange(startBackboneIndex + 1, endBackboneIndex - (startBackboneIndex)).Select((v) => v.position));
            l.Add(endPoint);
            return (l, startBackboneIndex);
        }
    }

    void createSegmentsMeshes(ChromosomeRenderingInfo chromosomeRenderingInfo)
    {
        foreach (var (segmentSetName, segmentSetInfo, segmentSetIndex) in chromosomeRenderingInfo.segmentInfos.Select((segmentInfo, index) => (segmentInfo.Key, segmentInfo.Value, index)))
        {
            List<(int startBin, int endBin)> combineSegments(List<Chromosome.SegmentSet.Location> segments)
            {
                var combined = new List<(int startBin, int endBin)>();
                var current_section = (startBin: (int)segments[0].StartBin, endBin: (int)segments[0].EndBin);
                foreach (var segment in segments.GetRange(1, segments.Count - 1))
                {
                    if (current_section.endBin < segment.StartBin)
                    {
                        combined.Add(current_section);
                        current_section = (startBin: (int)segment.StartBin, endBin: (int)segment.EndBin);
                    }
                    else
                    {
                        current_section = (current_section.startBin, Mathf.Max((int)segment.EndBin, current_section.endBin));
                    }
                }
                combined.Add(current_section);
                return combined;
            }

            var meshFilters = segmentMeshFilters[segmentSetIndex];

            var segmentInfos = segmentSetInfo.segments.Match(segments => segments.Select(segment => segment.Location), segments => segments.Select(segment => segment.Location)).ToList();
            var combinedSegments = combineSegments(segmentInfos);
            var segmentsPointsGroups = new List<(List<Vector3> genePoints, int startingBackboneindex)>();
            foreach (var (start, end) in combinedSegments)
            {
                segmentsPointsGroups.Add(getPointsConnectingBpIndices(start, end));
            }


            foreach (var (segmentsPointGroupsForCurrentGeneRenderer, meshFiltersIndex) in segmentsPointsGroups.Split(meshFilters.Count).Select((x, i) => (x, i)))
            {
                Mesh mesh = new Mesh();
                meshFilters[meshFiltersIndex].mesh = mesh;


                var verticies = new List<Vector3>();
                var indices = new List<int>();
                foreach (var (genePoints, startingBackboneIndex) in segmentsPointGroupsForCurrentGeneRenderer)
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
    }

    void createChromatidInterationPredictionLines()
    {
        foreach (var (start, end) in chromatinInteractionPrediction)
        {
            try
            {
                /*
                 * TODO: uncomment
                 * 
                Assert.IsTrue(start.start >= 0);
                Assert.IsTrue(start.end >= 0);
                Assert.IsTrue(end.start >= 0);
                Assert.IsTrue(end.end >= 0);
                Assert.IsTrue(start.start <= totalBasePairs);
                Assert.IsTrue(start.end <= totalBasePairs);
                Assert.IsTrue(end.start <= totalBasePairs);
                Assert.IsTrue(end.end <= totalBasePairs);
                */
                var midpointStart = (start.start + start.end) / 2;
                var midpointEnd = (end.start + end.end) / 2;

                // TODO: putting all these lines in seperate components has a substantial performance cost - can I combine them into one mesh like I do with the bridges? 
                // It would add a lot of tris, but it would move work from the CPU to the GPU. 
                var bridge = Instantiate(bridgePrefab, bridgeParent.transform);
                var line = bridge.GetComponent<LineRenderer>();
                line.startWidth *= overallScale * lineWidth * 3;
                line.endWidth *= overallScale * lineWidth * 3;
                line.SetPositions(new Vector3[] { binToPoint(midpointStart), binToPoint(midpointEnd) });
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



    public void highlightSegment(MeshFilter renderer, Chromosome.SegmentSet.Location info)
    {
        var genePoints = getPointsConnectingBpIndices((int)info.StartBin, (int)info.EndBin);

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

    public void highlightSegment(string segmentSet, Chromosome.SegmentSet.Location info)
    {
        highlightSegment(highlightRenderer, info);
    }

    public void unhighlightGene()
    {
        highlightRenderer.mesh.Clear();
    }




    // TODO: This could be sped up with a binary search, or with the kd tree tech
    public Dictionary<string, IEnumerable<int>> getSegmentsAtBpIndex(SegmentInfo segmentInfos, int bin)
    {
        var matched_segments = segmentInfos.Select(segmentInfo => (setName: segmentInfo.Key, matchedSegments: segmentInfo.Value.segments.Match<IEnumerable<int>>(
            segments => (from info in segments.Select((segment, index) => (segment, index))
                         where info.segment.Location.StartBin <= bin
                         where bin <= info.segment.Location.EndBin
                         select info.index),
            segments => (from info in segments.Select((segment, index) => (segment, index))
                         where info.segment.Location.StartBin <= bin
                         where bin <= info.segment.Location.EndBin
                         select info.index)
            )));
        var segmentsDict = matched_segments.Where(x => x.matchedSegments.Any()).ToDictionary(x => x.setName, x => x.matchedSegments);

        return segmentsDict;
    }

    public int basePairIndexToLocationIndex(List<Point> points, int bpIndex)
    {
        if (bpIndex <= points.First().bin) return 0;
        if (bpIndex >= points.Last().bin) return points.Count - 1;

        var index = points.Select((p) => p.bin).ToList().BinarySearch(bpIndex);
        if (index < 0)
        {
            index = ~index; // index of the first element that is larger than the search value
            index -= 1;
        }
        index = index >= points.Count ? points.Count - 1 : index;
        index = index < 0 ? 0 : index;

        return index;
    }

    public int basePairIndexToLocationIndex(int bpIndex)
    {
        return basePairIndexToLocationIndex(chromosomeRenderingInfo.points, bpIndex);
    }

    public Vector3 basePairIndexToPoint(List<Point> points, int bpIndex)
    {
        if (bpIndex <= points[0].bin)
        {
            return points[0].position;
        }
        else if (bpIndex >= points.Last().bin)
        {
            return points.Last().position;
        }
        else
        {
            var locationIndex = basePairIndexToLocationIndex(points, bpIndex);

            var a = points[locationIndex];
            var b = points[locationIndex + 1];
            Assert.IsTrue(a.bin <= bpIndex);
            Assert.IsTrue(bpIndex <= b.bin);
            return Vector3.Lerp(a.position, b.position, Mathf.InverseLerp(a.bin, b.bin, bpIndex));
        }
    }
    public Vector3 binToPoint(int bpIndex)
    {
        return basePairIndexToPoint(chromosomeRenderingInfo.points, bpIndex);
    }


    float triangleArea(Vector3 a, Vector3 b, Vector3 c)
    {
        return Mathf.Abs(a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static Chromosome.SegmentSet.Location GetSegmentInfo(Segment segment)
    {
        return segment.Match(s => s.Location, s => s.Location);
    }

    public static Segment GetSegmentFromCurrentChromosome(string segmentSet, int index)
    {
        return chromosomeRenderingInfo.segmentInfos[segmentSet].segments.Match<Segment>(genes => genes[index], other => other[index]);
    }
}
