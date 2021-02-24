using System.Collections.Generic;
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
    float simplificationFactorCoarse = .97f;
    float simplificationFactorFine = .1f;
    static bool cartoon = false;
    static int cartoonCenter = 0;
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



    public KTrie.StringTrie<(List<MeshRenderer> renderer, int start, int end, int index)> geneDict;

    static private int numberOfRows = 0;
    static public int totalBasePairs = 0;
    public int basePairsPerRow = 5000;

    public string focusedGene = "";


    List<List<Vector3>> verticiesl;
    List<List<int>> indicesl;

    Vector3 randoVector;


    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = UnityEngine.Color.yellow;
        if (verticiesl != null)
        {

            foreach (var vertex in verticiesl)
            {
                //Gizmos.DrawSphere(vertex, 0.05f);
            }
        }
    }

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
        geneDict = new KTrie.StringTrie<(List<MeshRenderer> renderer, int start, int end, int index)>();
        points = getPoints();
        genes = getGenes();
        gata = getGATA();
        ctcf = getCTCF();
        irf = getIRF();

        chromatinInteractionPrediction = getChromatinInteractionPrediction();


        randoVector = Random.insideUnitSphere;

        createBackboneMesh();
        createGenesMesh();

        /*
        var currentGeneIndex = 0;
        var toEnd = new List<(string name, int start, int end)>();

        var orignalIndex = 0;

        totalBasePairs = basePairsPerRow * numberOfRows;
        var cartoonCenterBP = Mathf.Clamp(cartoonCenter, totalBasePairs * cartoonAmount, totalBasePairs - totalBasePairs * cartoonAmount);
        var cartoonStartBP = cartoonCenterBP - cartoonAmount * totalBasePairs;
        var cartoonEndBP = cartoonCenterBP + cartoonAmount * totalBasePairs;

        Debug.Log("cartoonCenterBP " + cartoonCenterBP);
        Debug.Log("cartoonAmount " + cartoonAmount);
        Debug.Log("totalBasePairs " + totalBasePairs);
        Debug.Log("cartoonStartBP " + cartoonStartBP);
        Debug.Log("cartoonEndBP " + cartoonEndBP);


        //int cartoonStartBP = 0;
        // int cartoonEndBP = int.MaxValue;

        for (int i = 0; i < points.coarse.Count - 1; i++)
        {
            int currentBasePair = points.coarse[i].basePairIndex;
            bool cartoonPeriod = cartoonStartBP < currentBasePair && currentBasePair < cartoonEndBP;
            if (cartoonPeriod)
            {
                print("cartoonPeriod is at least sometimes true");
            }

            var stopIndex = points.coarse[i + 1].originalIndex;
            var (_, newCurrentGeneIndex, segmentsCoarse, newToEnd) = linesToAdd(i, stopIndex, currentGeneIndex, points.coarse, genes, toEnd.ToList());
            var (newOriginalIndex, _, segmentsOriginal, _) = linesToAdd(orignalIndex, stopIndex, currentGeneIndex, points.original, genes, toEnd.ToList());
            currentGeneIndex = newCurrentGeneIndex;
            toEnd = newToEnd;
            orignalIndex = newOriginalIndex;


            var coarseSegments = new List<MeshRenderer>();
            if (!cartoon)
            {
                foreach (var (p1, p2, sections) in segmentsCoarse)
                {
                    coarseSegments.AddRange(AddLineSegment(p1, p2, sections, 3));
                }
            }

            var originalSegments = new List<MeshRenderer>();
            if (!cartoon || cartoonPeriod)
            {
                foreach (var (p1, p2, sections) in segmentsOriginal)
                {
                    originalSegments.AddRange(AddLineSegment(p1, p2, sections, 2));
                }
            }


            var LODParent = new GameObject("LODParent");
            var coarseParent = new GameObject("coarseParent");
            var fineParent = new GameObject("fineParent");
            coarseParent.transform.parent = LODParent.transform;
            fineParent.transform.parent = LODParent.transform;
            if (!cartoon)
            {
                foreach (var segment in coarseSegments)
                {
                    segment.transform.parent = coarseParent.transform;
                }
            }
            if (!cartoon || cartoonPeriod)
            {
                foreach (var segment in originalSegments)
                {
                    segment.transform.parent = fineParent.transform;
                }

                if (!cartoon)
                {
                    var LODGroup = LODParent.AddComponent<LODGroup>();
                    LOD[] lods = new LOD[2];
                    lods[0] = new LOD(3.0F / (4), originalSegments.ToArray());

                    lods[1] = new LOD(1.0F / (200 + 1), coarseSegments.ToArray());
                    LODGroup.SetLODs(lods);
                    LODGroup.RecalculateBounds();
                }
            }
        }

        if (cartoon)
        {
            foreach (var (start, end) in gata)
            {
                var midpoint = (start + end) / 2;
                if (cartoonStartBP < midpoint && midpoint < cartoonEndBP)
                {
                    var pos = points.original[midpoint / basePairsPerRow];
                    var sphere = Instantiate(Sphere_GATA, pos.position, Quaternion.identity);
                    sphere.transform.localScale = new Vector3(.04f, .04f, .04f);
                }
            }
            foreach (var (start, end) in ctcf)
            {
                var midpoint = (start + end) / 2;
                if (cartoonStartBP < midpoint && midpoint < cartoonEndBP)
                {
                    var pos = points.original[midpoint / basePairsPerRow];
                    var sphere = Instantiate(Sphere_CTCF, pos.position, Quaternion.identity);
                    sphere.transform.localScale = new Vector3(.04f, .04f, .04f);
                }
            }
            foreach (var (start, end) in irf)
            {
                var midpoint = (start + end) / 2;
                if (cartoonStartBP < midpoint && midpoint < cartoonEndBP)
                {
                    var pos = points.original[midpoint / basePairsPerRow];
                    var sphere = Instantiate(Sphere_IDR, pos.position, Quaternion.identity);
                    sphere.transform.localScale = new Vector3(.04f, .04f, .04f);
                }
            }
        }

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
                if ((cartoon && cartoonStartBP < midpointStart && midpointStart < cartoonEndBP && cartoonStartBP < midpointEnd && midpointEnd < cartoonEndBP) || !cartoon)
                {
                    var bridge = Instantiate(BridgePrefab);
                    var line = bridge.GetComponent<LineRenderer>();
                    line.startWidth *= overallScale * linewidth * .1f;
                    line.endWidth *= overallScale * linewidth * .1f;
                    line.SetPositions(new Vector3[] { basePairIndexToPoint(midpointStart).position, basePairIndexToPoint(midpointEnd).position });
                }
            }
            catch
            {
                //Debug.Log((start, end) + " is outside the range of [0, " + totalBasePairs + "]");
            }

        }
        */

    }

    void createBackboneMesh()
    {
        verticiesl = new List<List<Vector3>>();
        indicesl = new List<List<int>>();
        foreach (var (pointsRangeI, meshIndex) in points.original.Split(backboneRenderers.Count).Select((x, i) => (x, i)))
        {
            var pointsRange = pointsRangeI.ToList();

            Mesh mesh = new Mesh();
            backboneRenderers[meshIndex].mesh = mesh;

            var (verticies, indices) = createMeshConnectingPointsInRange(pointsRange.Select((p) => p.position).ToList(), lineWidth);

            mesh.Clear();
            mesh.vertices = verticies.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
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

    public void loadCartoon()
    {
        cartoon = true;
        cartoonCenter = CameraController.OneDView.center;
        cameraParentCachedPosition = (cameraParent.transform.position, cameraParent.transform.eulerAngles, cameraParent.transform.localScale);
        SceneManager.LoadScene("mainScene");
    }

    public void loadMain()
    {
        cartoon = false;
        cartoonCenter = CameraController.OneDView.center;
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
                geneDict.Add(gene.name, (renderer: new List<MeshRenderer>(), gene.start, gene.end, index));
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
                obj.transform.localScale.x * lineWidth / 100 * overallScale,
                obj.transform.localScale.y * lineWidth / 100 * overallScale,
                (p1_ - p2_).magnitude
            );
            segments.Add(obj.GetComponent<MeshRenderer>());
            return obj;
        }
        void AddGeneSegment(string name, float f1, float f2, GameObject prefab, bool gene)
        {
            if (gene && LOD > 2 && (f2 - f1) < .5f)
            {
                return;
            }
            var startPoint = Vector3.Lerp(p1.position, p2.position, f1);
            var endPoint = Vector3.Lerp(p1.position, p2.position, f2);
            var geneObj = AddSubsegment(startPoint, endPoint, prefab);
            var geneController = geneObj.AddComponent<GeneController>();
            geneController.geneName = name;
            geneController.geneStart = geneDict[name].start;
            geneController.geneEnd = geneDict[name].end;
            geneController.segmentStart = Mathf.Lerp(geneDict[name].start, geneDict[name].end, f1);
            geneController.segmentEnd = Mathf.Lerp(geneDict[name].start, geneDict[name].end, f2);
            geneController.startPoint = startPoint;
            geneController.endPoint = endPoint;
            if (geneDict[name].renderer.Count == 0 && cartoon)
            {
                var text = Instantiate(geneTextCanvas, geneController.startPoint, Quaternion.identity);
                text.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
            }
            geneDict[name].renderer.Add(geneObj.GetComponent<MeshRenderer>());
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

    List<(string name, int start, int end)> getGenesAroundBasePairIndex(int basePairIndex)
    {
        return new List<(string name, int start, int end)>();
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

    public void highlightGene(string name)
    {
        if (name == "") return;
        foreach (var geneRenderer in geneDict[name].renderer)
        {
            geneRenderer.material = highlightedColoredMaterial;
        }
    }

    public void unhighlightGene(string name)
    {
        if (name == "" || name == focusedGene) return;
        foreach (var geneRenderer in geneDict[name].renderer)
        {
            geneRenderer.material = coloredMaterial;
        }
    }


    public void focusGene(string name)
    {
        if (name == "") return;
        unfocusGene();
        focusedGene = name;
        foreach (var geneRenderer in geneDict[name].renderer)
        {
            geneRenderer.material = highlightedColoredMaterial;
        }
    }

    public void unfocusGene()
    {
        if (focusedGene == "") return;
        foreach (var geneRenderer in geneDict[focusedGene].renderer)
        {
            geneRenderer.material = coloredMaterial;
            focusedGene = "";
        }
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
