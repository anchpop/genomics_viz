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


using Site = OneOf.OneOf<
    CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>,
    CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ChromatinAccessibility>>;
using SiteList =
    OneOf.OneOf<
        System.Collections.Generic.List<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>>,
        System.Collections.Generic.List<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ChromatinAccessibility>>
    >;
using SiteInfo = System.Collections.Generic.Dictionary<
    string,
    (
        OneOf.OneOf<
            System.Collections.Generic.List<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>>,
            System.Collections.Generic.List<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ChromatinAccessibility>>> Sites,
        Supercluster.KDTree.KDTree<float, int> worldPositions)
>;

using Connection = OneOf.OneOf<
    CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChromatinInteractionPredictions>,
    CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.SignificantHiCInteractions>>;
using ConnectionList =
    OneOf.OneOf<
        System.Collections.Generic.List<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChromatinInteractionPredictions>>,
        System.Collections.Generic.List<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.SignificantHiCInteractions>>
    >;
using ConnectionInfo = System.Collections.Generic.List<(
    CapnpGen.Description description,
    OneOf.OneOf<
        System.Collections.Generic.List<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChromatinInteractionPredictions>>,
        System.Collections.Generic.List<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.SignificantHiCInteractions>>
    > info
)>;



public struct Point
{
    public Vector3 position;
    public int bin;
    public List<Vector3> pipeNormals;
    public override string ToString() =>
        $"Point(position: {position}, bin: {bin}";
    public static Point Lerp(Point a, Point b, float t)
    {
        var point = new Point();
        point.position = Vector3.Lerp(a.position, b.position, t);
        point.bin = (int)Mathf.Lerp(a.bin, b.bin, t);
        point.pipeNormals = a.pipeNormals.Zip(b.pipeNormals, (pn1, pn2) => Vector3.Lerp(pn1, pn2, t)).ToList();
        return point;
    }
}

public struct ChromosomeSetRenderingInfo
{
    public ChromosomeSet chromosomeSet;
    public Dictionary<string, Chromosome> chromosomes;
}

public struct ChromosomeRenderingInfo
{
    public Chromosome chromosome;
    public List<Point> backbonePoints;
    public (int[] jumps, int binsPerJumpPoint) binIndexJumpPoints;
    public KDTree<float, int> backbonePointsTree;
    public SegmentInfo segmentInfos;
    public ConnectionInfo connectionInfos;
    public SiteInfo siteInfos;
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

    public static ChromosomeSetRenderingInfo chromosomeSetRenderingInfo;
    public static ChromosomeRenderingInfo chromosomeRenderingInfo;
    string currentlyRenderingSetIndex = "1";

    public GameObject cylinderPrefab_LOD0;
    public GameObject coloredCylinderPrefab_LOD0;
    public GameObject cylinderPrefab_LOD1;
    public GameObject coloredCylinderPrefab_LOD1;
    public GameObject cylinderPrefab_LOD2;
    public GameObject coloredCylinderPrefab_LOD2;
    public GameObject cylinderPrefab_LOD3;
    public GameObject coloredCylinderPrefab_LOD3;
    public GameObject SiteSphere;
    public GameObject bridgePrefab;


    public Material coloredMaterial;
    public Material highlightedColoredMaterial;

    public GameObject geneTextCanvas;

    public float cartoonAmount = .1f;

    // Unity places a limit on the number of verts on a mesh that's quite a bit lower than the amount we need
    // So, we need to use multiple meshes, which means multiple mesh renderers
    public GameObject rendererTemplate;
    public GameObject backboneRenderer;
    public GameObject segmentParent;
    public GameObject siteParent;
    public GameObject connectionParent;
    public GameObject bridgeParent;

    public Material BaseMaterial;


    public MeshFilter highlightRenderer;
    public MeshFilter focusRenderer;

    public int numsides = 3;

    Vector3 randoVector;

    void Start()
    {
        randoVector = Random.insideUnitSphere;

        chromosomeSetRenderingInfo = "getChromosomeSetRenderingInfo".profileF(() =>
            getChromosomeSetRenderingInfo());

        chromosomeRenderingInfo = "createRenderingInfo".profileF(() =>
            createRenderingInfo(chromosomeSetRenderingInfo, currentlyRenderingSetIndex));

        "createBackbone".profile(() => createBackbone(chromosomeRenderingInfo));


        "createSegments".profile(() => createSegments(chromosomeRenderingInfo));


        Profiler.BeginSample("createChromatidInterationPredictionLines");
        createChromatidInterationPredictionLines(chromosomeRenderingInfo);
        Profiler.EndSample();

        Profiler.BeginSample("createChromatidInterationPredictionLines");
        createSitePoints(chromosomeRenderingInfo);
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
            return set;
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
            return "getPoints".profileF(() =>
            {
                var center = Vector3.zero;

                var pointsRaw = new List<(Vector3 point, int bin)>();



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

                var positions = new List<Vector3>();
                var bins = new List<int>();
                foreach (var (point, bin) in pointsRaw)
                {
                    var scaling = Mathf.Max(Mathf.Max(min.x - max.x, min.y - max.y), min.z - max.z);
                    //var p = new Point();
                    positions.Add((point - center) * overallScale / scaling);
                    bins.Add(bin);
                }

                var pipeNormals = "getting pipe points".profileF(() => MeshGenerator.pipePointsList(positions, randoVector, numsides));

                var points = positions.Zip(bins, (position, bin) => (position, bin)).Zip(pipeNormals, (v, pipeNormals) =>
                {
                    var (position, bin) = v;

                    var p = new Point();
                    p.position = position;
                    p.bin = bin;
                    p.pipeNormals = pipeNormals.normals;
                    return p;
                });

                return points.ToList();
            });
        }

        KDTree<float, int> backbonePointsTree(List<Point> points)
        {
            return "getPointsTree".profileF(() =>
            {
                float[][] positions = points.Select(point => new float[] { point.position.x, point.position.y, point.position.z }).ToArray();
                int[] nodes = points.Select((x, i) => i).ToArray();
                var backbonePointsTree = new KDTree<float, int>(3, positions, nodes, (f1, f2) => (new Vector3(f1[0], f1[1], f1[2]) - new Vector3(f2[0], f2[1], f2[2])).magnitude);
                return backbonePointsTree;
            });
        }


        (int[] jumps, int binsPerJumpPoint) genBinIndexJumpArray(List<Point> points)
        {
            return "genBinIndexJumpArray".profileF(() =>
            {
                var binsPerJumpPoint = 50000;
                var highestBin = points.Last().bin;
                var numJumpPoints = highestBin / binsPerJumpPoint + 1;
                var listIndicesPerJumpPoint = points.Count / numJumpPoints;

                int[] jumpPoints = new int[numJumpPoints];

                int jumpPointsIndex = 0;
                foreach (var (point, index) in points.Select((point, index) => (point, index)))
                {
                    while (true)
                    {
                        var targetBin = jumpPointsIndex * binsPerJumpPoint;
                        if (point.bin == targetBin)
                        {
                            jumpPoints[jumpPointsIndex] = index;
                            jumpPointsIndex++;
                        }
                        if (point.bin > targetBin)
                        {
                            jumpPoints[jumpPointsIndex] = Mathf.Max(index - 1, 0);
                            jumpPointsIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                System.Array.Resize(ref jumpPoints, jumpPointsIndex);

                return (jumps: jumpPoints, binsPerJumpPoint);
            });
        }

        SegmentInfo getSegmentInfo(List<Point> points, (int[] jumps, int binsPerJumpPoint) jumpPoints)
        {
            return "getSegmentInfo".profileF(() =>
            {
                var segmentInfo = new SegmentInfo();

                foreach (var segmentSet in chromosome.SegmentSets)
                {
                    if (segmentSet.Segments.which == Chromosome.SegmentSet.segments.WHICH.Genes)
                    {
                        var segments = segmentSet.Segments.Genes.ToList();

                        ("Sorting for segment set " + segmentSet.Description.Name).profile(() =>
                            segments.Sort(delegate (Chromosome.SegmentSet.Segment<Chromosome.SegmentSet.Gene> x, Chromosome.SegmentSet.Segment<Chromosome.SegmentSet.Gene> y)
                            {
                                return (x.Location.Lower).CompareTo(y.Location.Lower);
                            })
                        );


                        var nameDict = new KTrie.StringTrie<int>();


                        ("Adding segment set to namedict for " + segmentSet.Description.Name).profile(() =>
                        {
                            foreach (var (segment, index) in segments.Select((segment, index) => (segment, index)))
                            {
                                // TODO: ask hao why this is sometimes true
                                // Assert.IsFalse(nameDict.ContainsKey(segment.Name), segment.Name + " appears multiple times in SegmentSet!");
                                if (!nameDict.ContainsKey(segment.ExtraInfo.Name))
                                {
                                    nameDict.Add(segment.ExtraInfo.Name, index);
                                }
                            }
                        }
                        );


                        var segment_points = ("converting segment points to float[][][] for " + segmentSet.Description.Name).profileF(() =>
                        {

                            var part1 = "part1".profileF(() =>
                                segments.Select(segment => checked((int)(segment.ExtraInfo.Ascending ? segment.Location.Lower : segment.Location.Upper))).ToList());

                            var part2 = "part2".profileF(() =>
                                part1.Select(originBin => binToPosition(points, jumpPoints, originBin)).ToList())
                            ;
                            return "segment_points".profileF(() =>
                                part2.Select(originPosition => new float[] { originPosition.x, originPosition.y, originPosition.z }).ToArray());
                        });
                        int[] nodes = ("getting segment indices for " + segmentSet.Description.Name).profileF(() =>
                            segments.Select((x, i) => i).ToArray());


                        var worldPositions = ("creating segment kdtree for " + segmentSet.Description.Name).profileF(() =>
                            new KDTree<float, int>(3, segment_points, nodes, (f1, f2) => (new Vector3(f1[0], f1[1], f1[2]) - new Vector3(f2[0], f2[1], f2[2])).magnitude)
                        );


                        segmentInfo[segmentSet.Description.Name] = (segments, worldPositions, nameDict);
                    }
                    else
                    {
                        Debug.LogError("Currently, we only support gene segments!");
                    }
                }

                return segmentInfo;
            });

        }

        SiteInfo getSiteInfo(List<Point> points, (int[] jumps, int binsPerJumpPoint) jumpPoints)
        {
            return "getSiteInfo".profileF(() =>
            {
                var siteInfo = new SiteInfo();

                foreach (var siteSet in chromosome.SiteSets)
                {

                    if (siteSet.Sites.which == Chromosome.SiteSet.sites.WHICH.ProteinBinding)
                    {
                        var sites = siteSet.Sites.ProteinBinding.ToList();
                        sites.Sort(delegate (Chromosome.SiteSet.Site<Chromosome.SiteSet.ProteinBinding> x, Chromosome.SiteSet.Site<Chromosome.SiteSet.ProteinBinding> y)
                        {
                            return (x.Location.Lower).CompareTo(y.Location.Lower);
                        });

                        float[][] sitePoints = sites.Select(segment =>
                        {
                            var centerBin = binRangeMidpoint(segment.Location);
                            var worldPosition = binToPoint(points, jumpPoints, centerBin).position;
                            return new float[] { worldPosition.x, worldPosition.y, worldPosition.z };
                        }).ToArray();
                        int[] nodes = sites.Select((x, i) => i).ToArray();
                        var worldPositions = new KDTree<float, int>(3, sitePoints, nodes, (f1, f2) => (new Vector3(f1[0], f1[1], f1[2]) - new Vector3(f2[0], f2[1], f2[2])).magnitude);

                        siteInfo[siteSet.Description.Name] = (sites, worldPositions);
                    }
                    else
                    {
                        Debug.LogError("Currently, we only support protein binding sites!");
                    }
                }

                return siteInfo;
            });
        }

        ConnectionInfo getConnectionInfo(List<Point> points)
        {
            return "getConnectionInfo".profileF(() =>
                chromosome.ConnectionSets.Select(connectionSet =>
                {
                    if (connectionSet.Connections.which == Chromosome.ConnectionSet.connections.WHICH.ChromatinInteractionPredictions)
                    {
                        var connections = connectionSet.Connections.ChromatinInteractionPredictions;
                        ConnectionList connection = connectionSet.Connections.ChromatinInteractionPredictions.ToList();
                        return (description: connectionSet.Description, info: connection);
                    }
                    else
                    {
                        Debug.LogError("Currently, we only support chromosomeInteractionPredictions segments!");
                        return (description: connectionSet.Description, info: new ConnectionList());
                    }

                }).ToList()
            );
        }



        var info = new ChromosomeRenderingInfo { };

        info.chromosome = chromosome;

        info.backbonePoints = getPoints();
        info.highestBin = info.backbonePoints.Last().bin;

        info.binIndexJumpPoints = genBinIndexJumpArray(info.backbonePoints);

        info.backbonePointsTree = backbonePointsTree(info.backbonePoints);

        info.segmentInfos = getSegmentInfo(info.backbonePoints, info.binIndexJumpPoints);

        //info.siteInfos = getSiteInfo(info.backbonePoints, info.binIndexJumpArray);

        info.connectionInfos = getConnectionInfo(info.backbonePoints);

        return info;
    }

    (List<Vector3> points, int startBackboneIndex) getPointsConnectingBpIndices(int startBasePairIndex, int endBasePairIndex)
    {
        var startBackboneIndex = binToLocationIndex(startBasePairIndex);
        var endBackboneIndex = binToLocationIndex(endBasePairIndex);
        Assert.IsTrue(startBackboneIndex <= endBackboneIndex, "start index should be before end index - this is my fault");

        var startPoint = binToPoint(startBasePairIndex).position;
        var endPoint = binToPoint(endBasePairIndex).position;
        if (startBackboneIndex == endBackboneIndex)
        {
            return (new List<Vector3> { startPoint, endPoint }, startBackboneIndex);
        }
        else
        {
            var l = new List<Vector3> { startPoint };
            l.AddRange(chromosomeRenderingInfo.backbonePoints.GetRange(startBackboneIndex + 1, endBackboneIndex - (startBackboneIndex)).Select((v) => v.position));
            l.Add(endPoint);
            return (l, startBackboneIndex);
        }
    }

    void createBackbone(ChromosomeRenderingInfo chromosomeRenderingInfo)
    {
        var mesh = MeshGenerator.applyToMesh(
            MeshGenerator.generateMeshConnectingPoints(chromosomeRenderingInfo.backbonePoints, lineWidth),
            backboneRenderer.GetComponent<MeshFilter>());

        var meshCollider = backboneRenderer.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        backboneRenderer.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(BaseMaterial);
        backboneRenderer.GetComponent<Renderer>().material.color = Settings.BackboneColor;
    }


    void createSegments(ChromosomeRenderingInfo chromosomeRenderingInfo)
    {
        foreach (var (segmentSetName, segmentSetInfo) in chromosomeRenderingInfo.segmentInfos.Select((segmentInfo, index) => (segmentInfo.Key, segmentInfo.Value.segments)))
        {
            var segments = "GetSegmentLocationList".profileF(() => GetSegmentLocationList(segmentSetInfo));
           
            var ranges = "combineBinRanges".profileF(() => MeshGenerator.combineBinRanges(segments));

            var meshes = "submeshes".profileF(() => ranges.Select(binRange =>
                MeshGenerator.generateMeshForBinRange(chromosomeRenderingInfo.backbonePoints, chromosomeRenderingInfo.binIndexJumpPoints, binRange, lineWidth * 1.1f)).ToList());

            var combined = "combined".profileF(() => MeshGenerator.CombineVertsAndIndices(meshes, false));
            
            var renderer = Instantiate(rendererTemplate, segmentParent.transform);
            renderer.name = segmentSetName;
            MeshGenerator.applyToMesh(combined, renderer.GetComponent<MeshFilter>());
            renderer.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(BaseMaterial);
            renderer.GetComponent<Renderer>().material.color = Settings.SegmentColor;
        }
    }

    void createSitePoints(ChromosomeRenderingInfo chromosomeRenderingInfo)
    {
        foreach (var siteSet in chromosomeRenderingInfo.chromosome.SiteSets)
        {
            
            var sitesParent = Instantiate(siteParent, gameObject.transform);
            sitesParent.name = siteSet.Description.Name;
            if (siteSet.Sites.which == Chromosome.SiteSet.sites.WHICH.ProteinBinding)
            {
                foreach (var site in siteSet.Sites.ProteinBinding)
                {
                    var siteMarker = Instantiate(SiteSphere, sitesParent.transform);
                    siteMarker.transform.localPosition = binToPosition((int)((site.Location.Lower + site.Location.Upper) / 2));
                }
            }
            else
            {
                Debug.LogError("Only Protein binding sites supported at this time!");
            }
            
        }
    }

    void createChromatidInterationPredictionLines(ChromosomeRenderingInfo chromosomeRenderingInfo)
    {
        foreach (var (description, connectionSet) in chromosomeRenderingInfo.connectionInfos)
        {
            var renderer = Instantiate(rendererTemplate, connectionParent.transform.parent.transform);
            renderer.name = description.Name;
            var combined = MeshGenerator.CombineVertsAndIndices(GetConnectionLocationList(connectionSet).Select((connection) =>
            {
                Assert.IsTrue(connection.Start.Lower >= 0);
                Assert.IsTrue(connection.Start.Upper >= 0);
                Assert.IsTrue(connection.End.Lower >= 0);
                Assert.IsTrue(connection.End.Upper >= 0);
                Assert.IsTrue(connection.Start.Lower <= chromosomeRenderingInfo.highestBin);
                Assert.IsTrue(connection.Start.Upper <= chromosomeRenderingInfo.highestBin);
                Assert.IsTrue(connection.End.Lower <= chromosomeRenderingInfo.highestBin);
                Assert.IsTrue(connection.End.Upper <= chromosomeRenderingInfo.highestBin);

                var midpointStart = binRangeMidpoint(connection.Start);
                var midpointEnd = binRangeMidpoint(connection.End);
                return (midpointStart, midpointEnd);
            }).Select(connection => {
                var (midpointStart, midpointEnd) = connection;
                return MeshGenerator.generateConnection(binToPosition(midpointStart), binToPosition(midpointEnd), 5, 0.005f);
            }), false);
            renderer.name = description.Name;
            MeshGenerator.applyToMesh(combined, renderer.GetComponent<MeshFilter>());
            renderer.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(BaseMaterial);
            renderer.GetComponent<Renderer>().material.color = Settings.ConnectionColor;
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

        var normals = cylinderExtrusion(points[0], points[1] - points[0]);

        return createMeshConnectingPointsInRange(points, normals, extrudeEnd);
    }

    (List<Vector3> verticies, List<int> indices, List<List<Vector3>> normalsAtPoint, List<Vector3> lastPoints) createMeshConnectingPointsInRange(List<Vector3> points, List<Vector3> startingPoints, bool extrudeEnd)
    {
        // Thanks to http://www.songho.ca/math/line/line.html#intersect_lineplane
        Vector3 intersectLineAndPlane(Vector3 linePoint, Vector3 lineDirection, Vector3 planePoint, Vector3 planeNormal)
        {
            if (Vector3.Dot(planeNormal, linePoint) == 0)
            {
                return Vector3.zero;
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

                // apparently you can calculate a normal vector this way? It doesn't say to normalize them first but I will just in case that's an assumption they make
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

                        (1        + sideIndex) % numsides,
                        (1        + sideIndex) % numsides + numsides,
                        (0        + sideIndex) % numsides + numsides,}

                    .Select((j) => j - numsides)
                );
                inds = inds
                    .Select((j) => j + preexistingVerticies);
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

    public void highlightSegment(MeshFilter renderer, Chromosome.BinRange info)
    {
        var genePoints = getPointsConnectingBpIndices((int)info.Lower, (int)info.Upper);

        Assert.AreNotEqual(genePoints.points.Count, 0);
        Assert.AreNotEqual(genePoints.points.Count, 1);

        Mesh mesh = new Mesh();
        renderer.mesh = mesh;
        renderer.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(BaseMaterial);
        renderer.GetComponent<Renderer>().material.color = Settings.SegmentFocusedColor;

        var startNormals = chromosomeRenderingInfo.backbonePoints[genePoints.startBackboneIndex].pipeNormals;
        var startingPoints = startNormals.Select((v) => v * lineWidth * 1.2f + genePoints.points[0]).ToList();
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

    public void highlightSegment(string segmentSet, Chromosome.BinRange info)
    {
        highlightSegment(highlightRenderer, info);
    }

    public void unhighlightGene()
    {
        highlightRenderer.mesh.Clear();
    }

    public void getFreshSegmentMesh(string segmentSetName)
    {
        Mesh a = null;
        a.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }

    // TODO: This could be sped up with a binary search, or with the kd tree tech
    public Dictionary<string, IEnumerable<int>> getSegmentsAtBin(SegmentInfo segmentInfos, int bin)
    {
        var matched_segments = segmentInfos.Select(segmentInfo => (setName: segmentInfo.Key, matchedSegments: segmentInfo.Value.segments.Match<IEnumerable<int>>(
            segments => (from info in segments.Select((segment, index) => (segment, index))
                         where info.segment.Location.Lower <= bin
                         where bin <= info.segment.Location.Upper
                         select info.index),
            segments => (from info in segments.Select((segment, index) => (segment, index))
                         where info.segment.Location.Lower <= bin
                         where bin <= info.segment.Location.Upper
                         select info.index)
            )));
        var segmentsDict = matched_segments.Where(x => x.matchedSegments.Any()).ToDictionary(x => x.setName, x => x.matchedSegments);

        return segmentsDict;
    }





    /// <summary>
    /// Given a sorted list of points, finds the highest point with a bin less than or equal to the specified one.
    /// </summary>
    /// <param name="backbonePoints"></param>
    /// <param name="bin"></param>
    /// <returns></returns>
    public static int binToLocationIndex(List<Point> backbonePoints, (int[] jumps, int binsPerJumpPoint) jumpPoints, int bin)
    {
        if (bin <= backbonePoints.First().bin) return 0;
        if (bin >= backbonePoints.Last().bin) return backbonePoints.Count - 1;


        /// This function does something kind of interesting. Specifically, it does this transformation:
        /// 0,   1,   2,   3,   4,   5,   6,    ...
        /// ↓    ↓    ↓    ↓    ↓    ↓    ↓      ↓
        /// 0,   1,  -1,   2,  -2,   3,  -3,    ...
        int spiral(int index)
        {
            if (index % 2 == 1)
            {
                return (index - 1) / 2 + 1;
            }
            else
            {
                return -index / 2;
            }
        }

        var approxedStart = jumpPoints.jumps[bin / jumpPoints.binsPerJumpPoint];
        var index = Enumerable.Range(0, 30)
            .Select(spiral)
            .Select((offset) => offset + approxedStart)
            .Where((index) => index >= 0 && index < backbonePoints.Count - 1)
            .First(index => bin >= backbonePoints[index].bin && bin < backbonePoints[index + 1].bin);
        /*
        var index = backbonePoints.Select((p) => p.bin).ToList().BinarySearch(bin);
        if (index < 0)
        {
            index = ~index; // index of the first element that is larger than the search value
            index -= 1;
        }
        index = index >= backbonePoints.Count ? backbonePoints.Count - 1 : index;
        index = index < 0 ? 0 : index;

        Debug.Log(Mathf.Abs(index - approxed) + " = " + index + " - " + approxed);

        Assert.AreEqual(index - approxed, 0);
        */

        return index;
    }

    public int binToLocationIndex(int bpIndex)
    {
        return binToLocationIndex(chromosomeRenderingInfo.backbonePoints, chromosomeRenderingInfo.binIndexJumpPoints, bpIndex);
    }

    public static Vector3 binToPosition(List<Point> points, (int[] jumps, int binsPerJumpPoint) jumpPoints, int bpIndex)
    {
        var locationIndex = binToLocationIndex(points, jumpPoints, bpIndex);
        var a = points[locationIndex];
        var b = points[Mathf.Min(locationIndex + 1, points.Count - 1)];
        return Vector3.Lerp(a.position, b.position, Mathf.InverseLerp(a.bin, b.bin, bpIndex));
    }
    public Vector3 binToPosition(int bpIndex)
    {
        return binToPosition(chromosomeRenderingInfo.backbonePoints, chromosomeRenderingInfo.binIndexJumpPoints, bpIndex);
    }
    // function closely related to ...
    public static Point binToPoint(List<Point> points, (int[] jumps, int binsPerJumpPoint) jumpPoints, int bpIndex)
    {
        var locationIndex = binToLocationIndex(points, jumpPoints, bpIndex);
        var a = points[locationIndex];
        var b = points[Mathf.Min(locationIndex + 1, points.Count - 1)];
        return Point.Lerp(a, b, Mathf.InverseLerp(a.bin, b.bin, bpIndex));
    }
    public Point binToPoint(int bpIndex)
    {
        return binToPoint(chromosomeRenderingInfo.backbonePoints, chromosomeRenderingInfo.binIndexJumpPoints, bpIndex);
    }

    public (int closest, int nextClosest) localPositionToBackbonePointIndex(Vector3 point)
    {
        var p = chromosomeRenderingInfo.backbonePointsTree.NearestNeighbors(new[] { point.x, point.y, point.z }, 2);
        //var p = backbonePointsTree.GetNearestNeighbours(new[] { point.x, point.y, point.z }, 2);

        return (p[0].Item2, p[0].Item2 + (int)Mathf.Sign(p[1].Item2 - p[0].Item2));
    }

    float triangleArea(Vector3 a, Vector3 b, Vector3 c)
    {
        return Mathf.Abs(a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));
    }


    public static Chromosome.BinRange GetSegmentLocation(Segment segment)
    {
        return segment.Match(s => s.Location, s => s.Location);
    }
    public static List<Chromosome.BinRange> GetSegmentLocationList(SegmentList segmentList)
    {
        return segmentList.Match(s => s.Select(segment => segment.Location), s => s.Select(segment => segment.Location)).ToList();
    }
    public static Chromosome.ConnectionSet.Location GetConnectionInfo(Connection connection)
    {
        return connection.Match(s => s.Location, s => s.Location);
    }

    public static List<Chromosome.ConnectionSet.Location> GetConnectionLocationList(ConnectionList connectionList)
    {
        return connectionList.Match(s => s.Select(connection => connection.Location), s => s.Select(connection => connection.Location)).ToList();
    }

    public static int binRangeMidpoint(Chromosome.BinRange binRange)
    {
        return (int)(binRange.Lower + binRange.Upper) / 2;
    }


    public static Segment GetSegmentFromCurrentChromosome(string segmentSet, int index)
    {
        return chromosomeRenderingInfo.segmentInfos[segmentSet].segments.Match<Segment>(genes => genes[index], other => other[index]);
    }
}
