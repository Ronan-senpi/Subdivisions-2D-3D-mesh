using System;
using System.Collections;
using System.Collections.Generic;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using Edge = Objects.Edge;

public class CatmullClarkManager : MonoBehaviour
{
    [SerializeField] private Mesh mesh;

    public void ApplyCatmullClark()
    {
        if (mesh != null)
        {
            List<Face> faces = new List<Face>();
            List<Edge> edges = new List<Edge>();
            List<Point> vertices = new List<Point>();

            List<Edge> tmpEdge = new List<Edge>();
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                Point v1 = new Point(mesh.vertices[i]);
                Point v2 = new Point(mesh.vertices[i + 1]);
                Point v3 = new Point(mesh.vertices[i + 2]);

                if (!vertices.Contains(v1))
                    vertices.Add(v1);
                if (!vertices.Contains(v2))
                    vertices.Add(v2);
                if (!vertices.Contains(v2))
                    vertices.Add(v3);

                tmpEdge.Add(new Edge(v1, v2));
                tmpEdge.Add(new Edge(v2, v3));
                tmpEdge.Add(new Edge(v3, v1));

                Face newFace = new Face(tmpEdge);
                newFace.ComputeFacePoint();
                faces.Add(newFace);

                foreach (Edge newEdge in tmpEdge)
                {
                    if (!edges.Contains(newEdge))
                    {
                        edges.Add(newEdge);
                    }
                }

                tmpEdge.Clear();
            }

            int F1, F2;
            foreach (Edge e in edges)
            {
                (F1, F2) = e.BelongsToFaces(faces);
                if (F1 >= 0 && F2 >= 0)
                {
                    e.ComputeEdgePoint(faces[F1].FacePoints, faces[F2].FacePoints);
                }
            }

            foreach (Point v in vertices)
            {
                List<int> indexFace = v.BelongsToFaces(faces);
                List<int> indexEdge = v.BelongsToEdges(edges);
                List<Point> facePointsAdjacent = new List<Point>();
                List<Point> edgePointsAdjacent = new List<Point>();
                foreach (int index in indexFace)
                    facePointsAdjacent.Add(faces[index].FacePoints);

                foreach (int index in indexEdge)
                    edgePointsAdjacent.Add(edges[index].EdgePoint);

                v.ComputeVertexPoint(facePointsAdjacent, edgePointsAdjacent);
            }

            foreach (Face f in faces)
            {
                List<Edge> faceEdgeConnection = new List<Edge>();
                foreach (Edge e in f.Edges)
                {
                    faceEdgeConnection.Add(new Edge(f.FacePoints, e.EdgePoint));
                }
            }
        }
        else
        {
            Debug.Log("Need model");
        }
    }
}