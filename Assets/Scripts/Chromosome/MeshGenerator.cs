using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using OneOf.Types;
using OneOf;
using static MoreLinq.Extensions.PairwiseExtension;
using static MoreLinq.Extensions.AtMostExtension;
using static MoreLinq.Extensions.AtLeastExtension;
using static MoreLinq.Extensions.ScanByExtension;
using static MoreLinq.Extensions.EndsWithExtension;
using static MoreLinq.Extensions.AssertExtension;
using CapnpGen;
using UnityEngine.Profiling;

public class MeshGenerator
{
    public static (List<Vector3> verts, List<int> indices) CombineVertsAndIndices(IEnumerable<(List<Vector3> verts, List<int> indices)> toCombine, bool attemptMerge = true)
    {
        var verts = new List<Vector3>();
        var indices = new List<int>();

        return toCombine.Aggregate((verts, indices), (acc, current) =>
        {
            if (attemptMerge)
            {
                // first, see if the first verts of the current mesh to combine overlap with the last verts of the accumulated mesh.
                var prefixes = Enumerable.Range(0, Mathf.Min(current.verts.Count, acc.verts.Count))
                                    .Select(p => current.verts.GetRange(0, p)).Reverse();
                var overlappingPrefixes = prefixes.Where(prefix => acc.verts.EndsWith(prefix));
                var firstOverlap = overlappingPrefixes.Append(new List<Vector3>()).First();

                var vertsToAdd = current.verts.GetRange(firstOverlap.Count, current.verts.Count - firstOverlap.Count).ToList();
                var indicesToAdd = current.indices.Select(index => index - firstOverlap.Count + acc.verts.Count).ToList();

                acc.verts.AddRange(vertsToAdd);
                acc.indices.AddRange(indicesToAdd);
            }
            else
            {
                acc.indices.AddRange(current.indices.Select(index => index + acc.verts.Count));
                acc.verts.AddRange(current.verts);
            }
            return acc;
        });

    }



    // ===============
    // Mesh Generators
    // ===============

    // Pipes
    // -----

    public static (List<Vector3> verts, List<int> indices) generateMeshForBinRange(List<Point> backbonePoints, (int[] jumps, int binsPerJumpPoint) jumpPoints, Chromosome.BinRange binRange, float lineWidth)
    {
        List<Point> points()
        {
            var startIndex = ChromosomeController.binToLocationIndex(backbonePoints, jumpPoints, (int)binRange.Lower);
            var endIndex = ChromosomeController.binToLocationIndex(backbonePoints, jumpPoints, (int)binRange.Upper);

            var startPoint = ChromosomeController.binToPoint(backbonePoints, jumpPoints, (int)binRange.Lower);
            var endPoint = ChromosomeController.binToPoint(backbonePoints, jumpPoints, (int)binRange.Upper);

            if (startIndex == endIndex)
            {
                return new List<Point> { startPoint, endPoint };
            }
            else
            {
                return backbonePoints.GetRange(startIndex + 1, endIndex - (startIndex)).Prepend(startPoint).Append(endPoint).ToList();
            }
        }

        Profiler.BeginSample("getting points");
        var p = points();
        Profiler.EndSample();

        Profiler.BeginSample("generating mesh");
        var mesh = generateMeshConnectingPoints(p, lineWidth);
        Profiler.EndSample();
        return mesh;
    }

    public static (List<Vector3> verts, List<int> indices) generateMeshConnectingPoints(List<Point> pointsToConnect, float lineWidth)
    {
        Profiler.BeginSample("getting uncombined");
        Assert.IsTrue(pointsToConnect.Count >= 2);
        var meshUncombined = Enumerable.Range(0, pointsToConnect.Count)
            .Pairwise((fromIndex, toIndex) =>
                MeshGenerator.generatePipeSegment(pointsToConnect[fromIndex], pointsToConnect[toIndex], lineWidth)).ToList();
        Profiler.EndSample();

        Profiler.BeginSample("combining");
        Assert.AreEqual(meshUncombined.Count, pointsToConnect.Count - 1);
        var meshCombined = MeshGenerator.CombineVertsAndIndices(meshUncombined);
        Profiler.EndSample();

        return meshCombined;
    }

    public static (List<Vector3> verts, List<int> indices) generatePipeSegment(Point from, Point to, float lineWidth)
    {
        var numsides = from.pipeNormals.Count();

        var verts = new List<Vector3>();
        verts.AddRange(from.pipeNormals.Select(normal => normal * lineWidth + from.position));
        verts.AddRange(to.pipeNormals.Select(normal => normal * lineWidth + to.position));

        var indices = Enumerable.Range(0, numsides).SelectMany((sideIndex) =>
                    new List<int>() {
                        (0        + sideIndex) % numsides,
                        (1        + sideIndex) % numsides,
                        (0        + sideIndex) % numsides + numsides,

                        (1        + sideIndex) % numsides,
                        (1        + sideIndex) % numsides + numsides,
                        (0        + sideIndex) % numsides + numsides,}
                ).ToList();

        return (verts, indices);
    }

    // Connections 
    // -----------

    public static (List<Vector3> verts, List<int> indices) generateConnection(Vector3 from, Vector3 to, int numsides, float lineWidth)
    {
        var direction = to - from;
        var circlePoints = createNormalsInCircleAroundVector(direction, Random.insideUnitSphere, numsides).Select(p => (p * lineWidth) + from);
        var verts = new List<Vector3>();
        verts.AddRange(circlePoints);
        verts.AddRange(circlePoints.Select(p => p + direction));

        var indices = Enumerable.Range(0, numsides).SelectMany((sideIndex) =>
            new List<int>() {
                        (0        + sideIndex) % numsides,
                        (1        + sideIndex) % numsides,
                        (0        + sideIndex) % numsides + numsides,

                        (1        + sideIndex) % numsides,
                        (1        + sideIndex) % numsides + numsides,
                        (0        + sideIndex) % numsides + numsides,}
        ).ToList();
        return (verts, indices);
    }



    // =================
    // Utility functions
    // =================

    /// <summary>
    /// We render the 3D backbone (represented as a polyline) as "piping". That means if our backbone is made of these three points:
    /// 
    /// O-----------------O
    ///                   |
    ///                   |
    ///                   |
    ///                   |
    ///                   O
    ///                   
    /// We first imagine a plane bisecting the two lines:
    /// 
    ///                    /
    /// O-----------------O
    ///                  /|
    ///                   |
    ///                   |
    ///                   |
    ///                   O
    /// 
    /// We then use this plane to "extrude" the line into a cylinder or pipe-shape.
    /// 
    /// 
    /// |``````````````````/|
    /// O                 O |
    /// |.............../   |
    ///                 |   |
    ///                 |   |
    ///                 |   |
    ///                 |_O_|
    ///
    /// This function is responsible for computing the corresponding points on the pipe for each point on the original polyline.
    /// Points on the pipe are marked with Xs:
    /// 
    /// X-------------------X
    /// O                 O |
    /// X---------------X   |
    ///                 |   |
    ///                 |   |
    ///                 |   |
    ///                 X_O_X
    ///                 
    /// This returns a list of 2-tuples. The first element represents the "center point" (the O) and the second
    /// represents the vector from the center to the points of the pipe (the Xs). 
    /// 
    /// The first and last points are "flat" - they're just the arranged in a circle around their corresponding center point. 
    /// The rest are generated via an interative process - we take the starting point (random for the first point), cast a ray in 
    /// the direction of the corresponding line segment and compute its intersection with the bisection plane, 
    /// and that gives us our new point. There is no way I know of to compute the pipe-points for the nth point on the polyline 
    /// without first computing the [0..n)th points.
    /// </summary>
    /// <param name="points"></param>
    /// <param name="initialVector"></param>
    /// <returns></returns>
    public static List<(Vector3 point, List<Vector3> normals)> pipePointsList(List<Vector3> points, Vector3 initialVector, int numsides)
    {
        Assert.IsTrue(points.Count > 1); // Doesn't make sense to generate pipe points for only a single point...

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

        Vector3 findNormalOfBisectionPlane(Vector3 Q_1, Vector3 Q_2, Vector3 Q_3)
        {
            // thanks to http://www.songho.ca/opengl/gl_cylinder.html#pipe
            // Pretty cool that this works to generate a normal vector actually
            var v_1 = Q_2 - Q_1;
            var v_2 = Q_3 - Q_2;
            var n = v_1.normalized + v_2.normalized;

            // Except if the directions were the same or opposite, the normal vector will be zero, and that's not what we want
            if (n == Vector3.zero)
            {
                n = Vector3.Cross(v_1, initialVector).normalized;
            }

            return n;
        }



        // Add an additional point to simplify the handling of the special "last case", 
        // by appending point that's just the last point + the vector from the second-to-last point to the last point.
        // aka:    second-to-last ------> last 
        // becomes
        //         second-to-last ------> last -------> appended
        // This increses the length of the list by 1, but it goes back down by 1 when we do the pairwise transformation, which
        // creates the following list
        // { (second-to-last, last), (last, appended) }
        var pointsExtended = points.Append(points.Last() + (points.Last() - points.ElementAt(points.Count - 2))).Pairwise((current, next) => (current, next));

        var output = pointsExtended.Select((x, i) => (x, i)).Aggregate(new List<(Vector3 point, List<Vector3> normals)>(), (acc, pointPair) =>
        {
            var ((current, next), index) = pointPair;
            var directionToNextPoint = next - current;
            if (acc.AtMost(0))
            {
                var normals = createNormalsInCircleAroundVector(directionToNextPoint, initialVector, numsides);
                Assert.IsTrue(normals.GetRange(1, normals.Count - 1).All(normal => normals.First() != normal));

                acc.Add((current, normals));

                return acc;
            }
            else
            {
                var (previous, previousNormals) = acc.Last();
                var directionFromPrevPoint = current - previous;

                var planeNormal = findNormalOfBisectionPlane(previous, current, next);
                var normals = previousNormals.Select(
                    (p, i) => intersectLineAndPlane(previous + p, directionFromPrevPoint, current, planeNormal) - current
                    ).ToList();


                Assert.IsTrue(normals.GetRange(1, normals.Count - 1).All(l2 => normals.First() != l2));


                acc.Add((point: current, normals));

                return acc;
            }
        });

        Assert.IsTrue(output.Count == points.Count);

        return output;
    }

    static List<Vector3> createNormalsInCircleAroundVector(Vector3 direction, Vector3 initialVector, int numsides)
    {
        var normal = Vector3.Cross(direction, initialVector).normalized;
        var rotated = Enumerable.Range(0, numsides).Select((i) => Quaternion.AngleAxis(i * 360.0f / numsides, direction) * normal).ToList();
        return rotated;
    }


    public static Mesh applyToMesh((List<Vector3> verts, List<int> indices) meshData, MeshFilter meshFilter)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        meshFilter.mesh = mesh;
        mesh.Clear();
        mesh.vertices = meshData.verts.ToArray();
        mesh.triangles = meshData.indices.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }


    public static List<Chromosome.BinRange> combineBinRanges(List<Chromosome.BinRange> binRanges)
    {
        var combined = new List<Chromosome.BinRange>();
        var current_section = binRanges[0];
        foreach (var segment in binRanges.GetRange(1, binRanges.Count - 1))
        {
            if (current_section.Upper < segment.Lower)
            {
                combined.Add(current_section);
                current_section = segment;
            }
            else
            {
                var new_section = new Chromosome.BinRange();
                new_section.Lower = current_section.Lower;
                new_section.Upper = System.Math.Max(segment.Upper, current_section.Upper);
                current_section = new_section;
            }
        }
        combined.Add(current_section);
        return combined;
    }




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
