using KdTree;
using KdTree.Math;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class ChromosomePart : MonoBehaviour
{
    public int startPointsIndex;
    public int endPointsIndex;
    private KdTree<float, int> backbonePointsTree;
    // Start is called before the first frame update
    void Awake()
    {
        backbonePointsTree = new KdTree<float, int>(3, new FloatMath());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addPoints(IEnumerable<Point> pointRange, int startPointsIndexp)
    {
        startPointsIndex = startPointsIndexp;
        endPointsIndex = startPointsIndex + pointRange.Count();
        foreach (var (point, index) in pointRange.Select((x, i) => (x, i)))
        {
            backbonePointsTree.Add(new[] { point.position.x, point.position.y, point.position.z }, index + startPointsIndex);
        }
    }
    public (int closest, int nextClosest) getPointIndexOfWorldPosition(Vector3 point)
    {
        var p = backbonePointsTree.GetNearestNeighbours(new[] { point.x, point.y, point.z }, 2);

        return (p[0].Value, p[0].Value + (int)Mathf.Sign(p[1].Value - p[0].Value));
    }
}
