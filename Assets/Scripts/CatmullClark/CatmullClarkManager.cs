using System;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;
using Edge = Objects.Edge;

public class CatmullClarkManager : MonoBehaviour
{
    [SerializeField] private GameObject go;
    [SerializeField] private Material mat;

    private Mesh baseMesh;
    private bool drawing = false;
    private List<Face> catmullFaces = new List<Face>();
    private Vector3 meshPosition = Vector3.zero;

    private void Awake()
    {
        baseMesh = go.GetComponent<MeshFilter>().mesh;
    }

    /// <summary>
    /// Subdivide base mesh with Catmull-Clark algorithm
    /// Save new mesh in base mesh
    /// </summary>
    public void ApplyCatmullClark()
    {
        catmullFaces.Clear();
        if (baseMesh != null)
        {
            List<Face> faces = new List<Face>();
            List<Edge> edges = new List<Edge>();
            List<Point> vertices = new List<Point>();

            for (int i = 0; i < baseMesh.triangles.Length; i += 3)
            {
                Point v1 = new Point(baseMesh.vertices[baseMesh.triangles[i]]);
                Point v2 = new Point(baseMesh.vertices[baseMesh.triangles[i + 1]]);
                Point v3 = new Point(baseMesh.vertices[baseMesh.triangles[i + 2]]);

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

                Face newFace = new Face();
                newFace.SetEdges(edge1, edge2, edge3);
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
                for (int i = 0; i < f.Edges.Length; i++)
                {
                    int nextIndex = i + 1 < f.Edges.Length ? i + 1 : 0;
                    Edge newEdge1 = new Edge(f.FacePoints, f.Edges[i].EdgePoint);
                    Edge newEdge2 = new Edge(f.Edges[i].EdgePoint, f.Edges[nextIndex].EdgePoint);
                    Edge newEdge3 = new Edge(f.Edges[nextIndex].EdgePoint, f.FacePoints);

                    Edge newEdge4 = new Edge(f.Edges[nextIndex].EdgePoint, f.Edges[i].EdgePoint);
                    Edge newEdge5 = new Edge(f.Edges[i].EdgePoint, f.Edges[i].secondPoint.VertexPoint);
                    Edge newEdge6 = new Edge(f.Edges[i].secondPoint.VertexPoint, f.Edges[nextIndex].EdgePoint);

                    Face catmullFace1 = new Face();
                    catmullFace1.SetEdges(newEdge1, newEdge2, newEdge3);
                    Face catmullFace2 = new Face();
                    catmullFace2.SetEdges(newEdge4, newEdge5, newEdge6);

                    catmullFaces.Add(catmullFace1);
                    catmullFaces.Add(catmullFace2);
                }
            }

            List<int> finalIndexes = new List<int>();
            List<Vector3> finalVertices = new List<Vector3>();
            int countFace = 0;
            foreach (Face f in catmullFaces)
            {
                (Point, Point, Point) facePoints = f.GetPoints();
                if (!finalVertices.Contains(facePoints.Item1.Position))
                    finalVertices.Add(facePoints.Item1.Position);

                if (!finalVertices.Contains(facePoints.Item2.Position))
                    finalVertices.Add(facePoints.Item2.Position);

                if (!finalVertices.Contains(facePoints.Item3.Position))
                    finalVertices.Add(facePoints.Item3.Position);


                finalIndexes.Add(finalVertices.IndexOf(facePoints.Item1.Position));
                finalIndexes.Add(finalVertices.IndexOf(facePoints.Item2.Position));
                finalIndexes.Add(finalVertices.IndexOf(facePoints.Item3.Position));


                countFace++;
            }

            Destroy(GetComponent<MeshFilter>());
            meshPosition.x += 2;
            CreateGeometry("Catmull", finalVertices.ToArray(), finalIndexes.ToArray(), meshPosition);
        }
        else

        {
            Debug.Log("Missing mesh");
        }
    }


    /// <summary>
    /// Create new game object with mesh
    /// </summary>
    /// <param name="name">Name of game object</param>
    /// <param name="verts">List of vertices for mesh</param>
    /// <param name="tris">List of indexes for mesh</param>
    /// <param name="pos">transform position for game object</param>
    /// <returns></returns>
    private Mesh CreateGeometry(string name, Vector3[] verts, int[] tris, Vector3 pos)
    {
        GameObject Kobbelted = new GameObject(name);
        Kobbelted.transform.position = pos;
        MeshFilter kmf = Kobbelted.AddComponent<MeshFilter>();
        MeshRenderer kmr = Kobbelted.AddComponent<MeshRenderer>();
        kmr.material = mat;

        kmf.mesh.Clear();
        kmf.mesh.vertices = verts;
        kmf.mesh.triangles = tris;

        kmf.mesh.RecalculateNormals();
        baseMesh = kmf.mesh;
        return kmf.mesh;
    }
}