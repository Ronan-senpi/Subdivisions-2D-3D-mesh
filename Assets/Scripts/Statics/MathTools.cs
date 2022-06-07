using System.Collections.Generic;
using Objects;
using UnityEngine;

public static class MathTools
{
    /// <summary>
    /// Converts a mesh given by a MeshFilter for example into Triangles, Edges and Points compatible avec our library
    /// </summary>
    /// <param name="mesh">The mesh to convert</param>
    /// <returns>A list containing all the triangles of the mesh</returns>
    public static List<Triangle> ConvertMesh(Mesh mesh)
    {
        int triangleCount = mesh.triangles.Length / 3;
        List<Triangle> triangles = new List<Triangle>();
        for (int i = 0; i < triangleCount; i++)
        {
            Point p1 = new Point(mesh.vertices[i + 0]);
            Point p2 = new Point(mesh.vertices[i + 1]);
            Point p3 = new Point(mesh.vertices[i + 2]);
            triangles.Add(new Triangle(
                new Edge(p1, p2),
                new Edge(p2, p3),
                new Edge(p3, p1)));
        }

        return new MeshObject(triangles);
    }
    public static List<Edge> GetEdges(List<Triangle> triangles)
    {
        List<Edge> edges = new List<Edge>();
        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle t = triangles[i];
            List<Edge> triangleEdges = t.GetEdges();
            if(!edges.Contains(triangleEdges[0])) edges.Add(triangleEdges[0]);
            if(!edges.Contains(triangleEdges[1])) edges.Add(triangleEdges[1]);
            if(!edges.Contains(triangleEdges[2])) edges.Add(triangleEdges[2]);
        }
        return edges;
    }
    public static List<Point> GetPoints(List<Triangle> triangles)
    {
        List<Point> points = new List<Point>();
        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle t = triangles[i];
            List<Point> trianglePoints = t.GetPoints();
            if(!points.Contains(trianglePoints[0])) points.Add(trianglePoints[0]);
            if(!points.Contains(trianglePoints[1])) points.Add(trianglePoints[1]);
            if(!points.Contains(trianglePoints[2])) points.Add(trianglePoints[2]);
        }
        return points;
    }
}