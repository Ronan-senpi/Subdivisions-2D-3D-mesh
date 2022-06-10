using System.Collections.Generic;
using Objects;
using UnityEngine;

public static class MathTools
{
    public static MeshObject ConvertMesh(Mesh mesh)
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
}