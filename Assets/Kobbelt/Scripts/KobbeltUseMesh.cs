using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(
    typeof(MeshFilter),
    typeof(MeshRenderer)
)]
public class KobbeltUseMesh : MonoBehaviour
{
    [SerializeField] private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        List<int> tris = new List<int>();
        List<Vector3> verts = new List<Vector3>();
        
        for (int i = 0; i < mesh.triangles.Length; i = i + 3)
        {
            // int idx1 = i + 1 < mesh.triangles.Length ? i + 1 : 0;
            // int idx2 = i + 2 < mesh.triangles.Length ? i + 2 : 1;
            int idx1 = i + 1;
            int idx2 = i + 2;

            Vector3 v1 = mesh.vertices[mesh.triangles[i]];
            Vector3 v2 = mesh.vertices[mesh.triangles[idx1]];
            Vector3 v3 = mesh.vertices[mesh.triangles[idx2]];
            Vector3 center = (v1 + v2 + v3) / 3;
            
            //Index des nouveau vertices
            int nNdx0 = verts.Count;
            int nNdx1 = verts.Count + 1;
            int nNdx2 = verts.Count + 2;
            int nNdx3 = verts.Count + 3;
            
            verts.Add(v1);
            verts.Add(v2);
            verts.Add(v3);
            verts.Add(center);
            
            
            // ajouts des nouveau trinagles
            tris.AddRange(new List<int> { nNdx0, nNdx1, nNdx3 });
            tris.AddRange(new List<int> { nNdx1, nNdx2, nNdx3 });
            tris.AddRange(new List<int> { nNdx0, nNdx3, nNdx2  });
        }

        GameObject Kobbelted = new GameObject("Kobbelted");
        Kobbelted.transform.position = new Vector3(0, 0, -5);
        MeshFilter kmf = Kobbelted.AddComponent<MeshFilter>();
        MeshRenderer kmr = Kobbelted.AddComponent<MeshRenderer>();
        kmr.material = mat;
        kmf.mesh.Clear();
        kmf.mesh.vertices = verts.ToArray();
        kmf.mesh.triangles = tris.ToArray();
     
    }
}