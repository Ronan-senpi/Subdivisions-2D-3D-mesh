using System.Collections.Generic;

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
        public List<Triangle> Triangles { get; set; }
    }
}