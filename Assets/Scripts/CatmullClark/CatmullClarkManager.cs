using System;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;
using Edge = Objects.Edge;

public class CatmullClarkManager : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    private Mesh mesh;
    private bool drawing = false;
    private List<Face> catmullFaces = new List<Face>();
    public GameObject point;

    private void Update()
    {
        if (drawing)
        {
            for (int i = 0; i < catmullFaces.Count; i++)
            {
                //(Face f in catmullFaces)
                for (int j = 0; j < catmullFaces[i].Edges.Count; j++)
                {
                    //(Edge e in f.Edges)
                    Debug.DrawLine(catmullFaces[i].Edges[j].firstPoint.Position,
                        catmullFaces[i].Edges[j].secondPoint.Position);
                }
            }
        }
    }

    public void ApplyCatmullClark()
    {
        catmullFaces.Clear();
        mesh = meshFilter.mesh;

        if (mesh != null)
        {
            List<Face> faces = new List<Face>();
            List<Edge> edges = new List<Edge>();
            List<Point> vertices = new List<Point>();

            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                Point v1 = new Point(mesh.vertices[mesh.triangles[i]]);
                Point v2 = new Point(mesh.vertices[mesh.triangles[i + 1]]);
                Point v3 = new Point(mesh.vertices[mesh.triangles[i + 2]]);

                Edge edge1 = new Edge(v1, v2);
                Edge edge2 = new Edge(v2, v3);
                Edge edge3 = new Edge(v3, v1);

                if (i == 0)
                {
                    vertices.Add(v1);
                    vertices.Add(v2);
                    vertices.Add(v3);
                    edges.Add(edge1);
                    edges.Add(edge2);
                    edges.Add(edge3);
                }
                else
                {
                    bool contains1 = false, contains2 = false, contains3 = false;
                    foreach (Point v in vertices)
                    {
                        if (v1.Position == v.Position) contains1 = true;
                        if (v2.Position == v.Position) contains2 = true;
                        if (v3.Position == v.Position) contains3 = true;
                    }
                    if (!contains1)
                        vertices.Add(v1);
                    if (!contains2)
                        vertices.Add(v2);
                    if (!contains3)
                        vertices.Add(v3);

                    contains1 = contains2 = contains3 = false;
                    foreach (Edge e in edges)
                    {
                        if (e.CompareEdge(edge1)) contains1 = true;
                        if (e.CompareEdge(edge2)) contains2 = true;
                        if (e.CompareEdge(edge3)) contains3 = true;
                    }
                    if (!contains1)
                    {
                        edges.Add(edge1);
                    }

                    if (!contains2)
                    {
                        edges.Add(edge2);
                    }

                    if (!contains3)
                    {
                        edges.Add(edge3);
                    }
                }

                Face newFace = new Face(new List<Edge>(new[] { edge1, edge2, edge3 }));
                newFace.ComputeFacePoint();

                faces.Add(newFace);
            }

            foreach (Face f in faces)
            {
                foreach (Edge e in f.Edges)
                {
                    if (e.EdgePoint == null)
                        e.ComputeEdgePoint(e.BelongsToFaces(faces));
                }
            }

            foreach (Face f in faces)
            {
                foreach (Edge e in f.Edges)
                {
                    if (e.firstPoint.VertexPoint == null)
                        e.firstPoint.ComputeVertexPoint(e.firstPoint.BelongsToFaces(faces),
                            e.firstPoint.BelongsToEdges(edges));
                    if (e.secondPoint.VertexPoint == null)
                        e.secondPoint.ComputeVertexPoint(e.secondPoint.BelongsToFaces(faces),
                            e.secondPoint.BelongsToEdges(edges));
                }
            }
            
            foreach (Face f in faces)
            {
                for (int i = 0; i < f.Edges.Count; i++)
                {
                    List<Edge> faceCreationEdges = new List<Edge>();
                    List<Edge> faceCreationEdges2 = new List<Edge>();

                    int nextIndex = i + 1 < f.Edges.Count ? i + 1 : 0;
                    Edge newEdge1 = new Edge(f.FacePoints, f.Edges[i].EdgePoint);
                    Edge newEdge2 = new Edge(f.Edges[i].EdgePoint, f.Edges[nextIndex].EdgePoint);
                    Edge newEdge3 = new Edge(f.Edges[nextIndex].EdgePoint, f.FacePoints);

                    Edge newEdge4 = new Edge(f.Edges[i].EdgePoint, f.Edges[nextIndex].EdgePoint);
                    Edge newEdge5 = new Edge(f.Edges[nextIndex].EdgePoint, f.Edges[i].secondPoint.VertexPoint);
                    Edge newEdge6 = new Edge(f.Edges[i].secondPoint.VertexPoint, f.Edges[i].EdgePoint);
                    faceCreationEdges.Add(newEdge1);
                    faceCreationEdges.Add(newEdge2);
                    faceCreationEdges.Add(newEdge3);

                    faceCreationEdges2.Add(newEdge4);
                    faceCreationEdges2.Add(newEdge5);
                    faceCreationEdges2.Add(newEdge6);

                    catmullFaces.Add(new Face(faceCreationEdges));
                    catmullFaces.Add(new Face(faceCreationEdges2));
                }
            }


            drawing = true;
        }
        else
        {
            Debug.Log("Missing mesh");
        }
    }
}