using Supercluster.KDTree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class ChromosomePart : MonoBehaviour
{
    public GameObject chromosome;
    public int startPointsIndex;
    public int endPointsIndex;
    private KDTree<float, int> backbonePointsTree;
    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addPoints(IEnumerable<Point> pointRange, int startPointsIndexp)
    {
        startPointsIndex = startPointsIndexp;
        endPointsIndex = startPointsIndex + pointRange.Count();

        float[][] points = pointRange.Select(point => new float[] { point.position.x, point.position.y, point.position.z }).ToArray();
        int[] nodes = pointRange.Select((x, i) => i + startPointsIndex).ToArray();
        backbonePointsTree = new KDTree<float, int>(3, points, nodes, (f1, f2) => (new Vector3(f1[0], f1[1], f1[2]) - new Vector3(f2[0], f2[1], f2[2])).magnitude);
    }
    public (int closest, int nextClosest) getPointIndexOfLocalPosition(Vector3 point)
    {
        var p = backbonePointsTree.NearestNeighbors(new[] { point.x, point.y, point.z }, 2);
        //var p = backbonePointsTree.GetNearestNeighbours(new[] { point.x, point.y, point.z }, 2);

        return (p[0].Item2, p[0].Item2 + (int)Mathf.Sign(p[1].Item2 - p[0].Item2));
    }
    public (int closest, int nextClosest) getPointIndexOfWorldPosition(Vector3 point)
    {
        return getPointIndexOfLocalPosition(chromosome.transform.InverseTransformPoint(point));
    }
}
