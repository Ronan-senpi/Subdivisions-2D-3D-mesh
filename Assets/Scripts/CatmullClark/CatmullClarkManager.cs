using System;
using System.Collections;
using System.Collections.Generic;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using Edge = Objects.Edge;

public class CatmullClarkManager : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    private Mesh mesh;

    public void ApplyCatmullClark()
    {
        mesh = meshFilter.mesh;

        if (mesh != null)
        {
            List<Face> faces = new List<Face>();
            List<Edge> edges = new List<Edge>();
            List<Point> vertices = new List<Point>();

            List<Edge> tmpEdge = new List<Edge>();
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                Point v1 = new Point(mesh.vertices[mesh.triangles[i]]);
                Point v2 = new Point(mesh.vertices[mesh.triangles[i + 1]]);
                Point v3 = new Point(mesh.vertices[mesh.triangles[i + 2]]);

                Edge edge1 = new Edge(v1, v2);
                Edge edge2 = new Edge(v2, v3);
                Edge edge3 = new Edge(v3, v1);
                tmpEdge.Add(edge1);
                tmpEdge.Add(edge2);
                tmpEdge.Add(edge3);

                Face newFace = new Face(new List<Edge>(tmpEdge));
                newFace.ComputeFacePoint();
                faces.Add(newFace);

                UpdateVerticesInfo(vertices, v1, edge1, edge3, newFace);
                UpdateVerticesInfo(vertices, v2, edge1, edge2, newFace);
                UpdateVerticesInfo(vertices, v3, edge2, edge3, newFace);

                foreach (Edge newEdge in tmpEdge)
                    UpdateEdgesInfo(edges, newEdge, newFace);

                tmpEdge.Clear();
            }

            for (int i = 0; i < edges.Count; i++) //(Edge e in edges)
            {
                if (edges[i].EdgePoint == null)
                {
                    edges[i].ComputeEdgePoint();
                }
            }

            foreach (Point v in vertices)
            {
                if (v.VertexPoint == null)
                    v.ComputeVertexPoint();
            }

            List<Face> catmullFaces = new List<Face>();
            List<Edge> faceCreationEdges = new List<Edge>();
            foreach (Face f in faces)
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    Edge newEdge1 = new Edge(f.FacePoints, edges[i].EdgePoint);
                    Edge newEdge2 = new Edge(edges[i].EdgePoint, edges[i].secondPoint.VertexPoint);
                    Edge newEdge3 = new Edge(edges[i].secondPoint.VertexPoint, edges[i + 1].EdgePoint);
                    Edge newEdge4 = new Edge(edges[i + 1].EdgePoint, f.FacePoints);
                    faceCreationEdges.Add(newEdge1);
                    faceCreationEdges.Add(newEdge2);
                    faceCreationEdges.Add(newEdge3);
                    faceCreationEdges.Add(newEdge4);

                    catmullFaces.Add(new Face(faceCreationEdges));
                }
            }

            foreach (Face f in catmullFaces)
            {
                foreach (Edge e in f.Edges)
                {
                    Debug.DrawLine(e.firstPoint.Position, e.secondPoint.Position);
                }
            }
        }
        else
        {
            Debug.Log("Missing mesh");
        }
    }

    public void UpdateVerticesInfo(List<Point> vertices, Point v, Edge edgeParentA, Edge edgeParentB, Face faceParent)
    {
        if (!vertices.Contains(v))
        {
            v.edgeParents.Add(edgeParentA);
            v.edgeParents.Add(edgeParentB);
            v.faceParents.Add(faceParent);
            vertices.Add(v);
        }
        else
        {
            Point oldPoint = vertices.Find(x => x == v);
            if (!oldPoint.edgeParents.Contains(edgeParentA))
                oldPoint.edgeParents.Add(edgeParentA);

            if (!oldPoint.edgeParents.Contains(edgeParentB))
                oldPoint.edgeParents.Add(edgeParentB);

            if (!oldPoint.faceParents.Contains(faceParent))
                oldPoint.faceParents.Add(faceParent);
        }
    }

    public void UpdateEdgesInfo(List<Edge> edges, Edge updateEdge, Face faceParent)
    {
        Edge oldEdge = null;
        foreach (Edge e in edges)
        {
            if (e.CompareEdge(updateEdge))
            {
                oldEdge = e;
            }
        }

        if (oldEdge == null)
        {
            Debug.Log("NULL");
            updateEdge.faceParents.Add(faceParent);
            edges.Add(updateEdge);
        }
        else
        {
            Debug.Log("Found the same");
            // if (!oldEdge.faceParents.Contains(faceParent))
            // {
            //     oldEdge.faceParents.Add(faceParent);
            // }
            // foreach ( VARIABLE in COLLECTION)
            // {
            //     
            // }
        }
    }
}