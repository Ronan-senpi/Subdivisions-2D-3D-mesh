using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Objects;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class LoopSolver : MonoBehaviour
{
    [SerializeField] private MeshFilter mf;
    private MeshObject originalObject;
    private Dictionary<(int, int), Vector3> edgePoints;
    private void Start()
    {
        originalObject = MathTools.ConvertMesh(mf.mesh);
        edgePoints = new Dictionary<(int, int), Vector3>();
    }

    public void Subdivide()
    {
        //J'identifie mes edgePoints par pair d'entier correspondant aux indices des points des deux cotés.
        edgePoints = new Dictionary<(int, int), Vector3>();
        List<Edge> edges = MathTools.GetEdges(originalObject.Triangles);
        foreach (var edge in edges)
        {
            (int, int) indices = edge.getIndices();
            if (!edgePoints.ContainsKey(indices))
            {
                edgePoints[indices] = ComputeEdgePoint(edge);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var pair in edgePoints)
        {
            Gizmos.DrawSphere(mf.transform.worldToLocalMatrix * pair.Value, 0.05f);
        }
    }

    private Vector3 ComputeEdgePoint(Edge edge)
    {
        Debug.Log("edge " + edge.firstPoint.Position + " " + edge.secondPoint.Position);
        Tuple<int, int> triangleIndices = edge.BelongsToTriangles(originalObject.Triangles);
        Debug.Log("triangleIndices " + triangleIndices);
        if(triangleIndices.Item1 == -1 || triangleIndices.Item2 == -1) return Vector3.zero;
        Triangle t1 = originalObject.Triangles[triangleIndices.Item1];
        Point vleft = t1.GetLastVertex(edge);
        Triangle t2 = originalObject.Triangles[triangleIndices.Item2];
        Point vright = t2.GetLastVertex(edge);
        return (3/8f) * (edge.firstPoint.Position + edge.secondPoint.Position) + (1/8f) * (vleft.Position + vright.Position);
    }
}
