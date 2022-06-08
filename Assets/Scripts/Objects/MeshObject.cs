using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class MeshObject
    {
        public MeshObject()
        {
            Triangles = new List<Triangle>();
        }

        public MeshObject(List<Triangle> tris)
        {
            Triangles = tris;
        }
        public MeshObject(List<Triangle> tris, Vector3[] verts)
        {
            Triangles = tris;
            Vertices = verts;
        }
        public List<Triangle> Triangles { get; set; }
        public Vector3[] Vertices { get; set; }
    }
}