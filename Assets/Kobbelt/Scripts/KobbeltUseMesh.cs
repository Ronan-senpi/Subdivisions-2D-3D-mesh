using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using TTri = System.Tuple<int, int, int>;
using TEdges = System.Tuple<int, int>;

[RequireComponent(
    typeof(MeshFilter),
    typeof(MeshRenderer)
)]
public class KobbeltUseMesh : MonoBehaviour
{
    [SerializeField] private Material mat;

    const float tolerence = 0.01f;
    List<Vector3> verts = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        Vector3[] meshVertices = mesh.vertices;
        int[] meshTriangles = mesh.triangles;
        MergeByDistance(ref meshVertices, ref meshTriangles);


        Mesh nMesh = CreateGeometry("Kobbelted", meshVertices, meshTriangles, new(1000, 1000, -1000));


        List<int> tris = new List<int>();
        List<TTri> trisTuple = TrisToTuple(nMesh.triangles);
        List<TEdges> edgesTuple = TTrisToEdges(trisTuple);
        List<(int, Vector3)> centerIndexes = new List<(int, Vector3)>();

        foreach (TTri triT in trisTuple)
        {
            Vector3 v1 = mesh.vertices[triT.Item1];
            Vector3 v2 = mesh.vertices[triT.Item2];
            Vector3 v3 = mesh.vertices[triT.Item3];

            Vector3 center = (v1 + v2 + v3) / 3;

            int[] neededVert1 = CalculateNeededVertices2(edgesTuple, triT.Item1);
            int[] neededVert2 = CalculateNeededVertices2(edgesTuple, triT.Item2);
            int[] neededVert3 = CalculateNeededVertices2(edgesTuple, triT.Item3);

            //Index des nouveau vertices
            var vp1 = PerturbVertex(triT.Item1, neededVert1, mesh.vertices);
            if (!verts.Any(x => (x -vp1).sqrMagnitude <= tolerence))
            {
                verts.Add(vp1);
            }
            int nNdx0  = verts.FindIndex(x =>  (x -vp1).sqrMagnitude <= tolerence);
            
            var vp2 = PerturbVertex(triT.Item2, neededVert2, mesh.vertices);
            if (!verts.Any(x => (x -vp2).sqrMagnitude <= tolerence))
            {
                verts.Add(vp2);
            }
            int nNdx1  = verts.FindIndex(x =>  (x -vp2).sqrMagnitude <= tolerence);

            var vp3 = PerturbVertex(triT.Item3, neededVert3, mesh.vertices);
            if (!verts.Any(x => (x -vp3).sqrMagnitude <= tolerence))
            {
                verts.Add(vp3);
            }
            int nNdx2  = verts.FindIndex(x =>  (x -vp3).sqrMagnitude <= tolerence);

            // verts.Add(PerturbVertex(triT.Item2, neededVert2, mesh.vertices));
            // verts.Add(PerturbVertex(triT.Item3, neededVert3, mesh.vertices));

            var vpc = center;
            if (!verts.Any(x => (x -vpc).sqrMagnitude <= tolerence))
            {
                verts.Add(vpc);
            }
            int centerId  = verts.FindIndex(x =>  (x -vpc).sqrMagnitude <= tolerence);
            

            centerIndexes.Add((centerId, center));

            // ajouts des nouveau trinagles
            tris.AddRange(new List<int> { nNdx0, nNdx1, centerId });
            tris.AddRange(new List<int> { nNdx1, nNdx2, centerId });
            tris.AddRange(new List<int> { nNdx0, centerId, nNdx2 });
        }

        Vector3[] AVerts = verts.ToArray();
        int[] ATris = tris.ToArray();

        CreateGeometry("kobb", AVerts, ATris, new Vector3(0, 0, 2));
        
        trisTuple = TrisToTuple(ATris);
        edgesTuple = TTrisToEdges(trisTuple);
        for (int i = 0; i < edgesTuple.Count; i++)
        {
            var myTris = GetTrisWithEdge(edgesTuple[i], trisTuple);
            if(myTris.Count == 2)
                continue;
            var t1 = myTris[0];
            var t2 = myTris[1];
            
            int it1 = trisTuple.FindIndex(x => x == t1);
            int it2 = trisTuple.FindIndex(x => x == t2);
            
            int vt1 = GetLastPoint(t1, edgesTuple[i]);
            int vt2 = GetLastPoint(t2, edgesTuple[i]);
            
            edgesTuple[i] = new TEdges(vt1, vt2);
            
            trisTuple[it1] = new TTri(edgesTuple[i].Item1, vt1, vt2);
            trisTuple[it2]= new TTri(edgesTuple[i].Item2, vt2, vt1);
        }

        tris = new List<int>();
        foreach (var t in trisTuple)
        {
            tris.Add(t.Item1);
            tris.Add(t.Item1);
            tris.Add(t.Item1);
        }

        CreateGeometry("kobbolted2", AVerts, tris.ToArray(), new(0, 0, 4));
    }

    private void MergeVert(ref Vector3[] verts, ref int[] tris)
    {
        List<Vector3> newVerts = new List<Vector3>();
        for (int i = 0; i < verts.Length; i++)
        {
            var v = verts[i];
            if (!newVerts.Any(x => (x -v).sqrMagnitude <= tolerence))
            {
                newVerts.Add(v);
            }
        }

        var newVertIndex = verts.Select((vert, index) => new
        {
            vert,
            index
        });
        for (int i = 0; i < tris.Length; i++)
        {
            var tmpVert = verts[tris[i]];
            tris[i] = newVertIndex.FirstOrDefault(x => x.vert == tmpVert).index;
        }
        
        // for (int i = 0; i < newVerts.Count; i++)
        // {
        //     var semeVert = verts.Select((item, index) => new{
        //                             vec = item,
        //                             index}).Where(x => x.vec == newVerts[i]).Select(item =>item.index).ToList();
        //     foreach (var sv in semeVert)
        //     {
        //         tris[sv] = i;
        //     }
        // }

        verts = newVerts.ToArray();
    }

    private int GetLastPoint(TTri t, TEdges edge)
    {
        List<int> tList = new List<int> { t.Item1, t.Item2, t.Item3 };
        tList.Remove(edge.Item1);
        tList.Remove(edge.Item2);
        if (tList.Count == 1)
        {
            return tList[0];
        }

        return -1;
    }

    /// <summary>
    /// Find all tris with specific vertex in there triangle
    /// </summary>
    /// <param name="vert">specific vertex</param>
    /// <param name="tris">List of triangles</param>
    /// <param name="triToExclude">if we want exclude a triangle of filtered tris</param>
    /// <returns>List of filtered tris</returns>
    private static List<TTri> GetTrisWithEdge(TEdges edge, List<TTri> tris)
    {
        List<TTri> filteredTris =
            tris.FindAll(t => (t.Item1 == edge.Item1 || t.Item2 == edge.Item1 || t.Item3 == edge.Item1) &&
                              (t.Item1 == edge.Item2 || t.Item2 == edge.Item2 || t.Item3 == edge.Item2));
        return filteredTris;
    }

    private List<TEdges> TTrisToEdges(List<TTri> trisTuple)
    {
        List<TEdges> edges = new List<TEdges>();
        foreach (var tri in trisTuple)
        {
            edges.Add(new TEdges(tri.Item1, tri.Item2));
            edges.Add(new TEdges(tri.Item2, tri.Item3));
            edges.Add(new TEdges(tri.Item3, tri.Item1));
        }

        return edges;
    }

    private Vector3 PerturbVertex(int vert, int[] neededVert, Vector3[] tris)
    {
        int n = neededVert.Length;
        float a = alpha(neededVert.Length);
        return (1f - a) * tris[vert] + (a / n) * Sum(neededVert, tris);
    }

    private static float alpha(int n)
    {
        return (1f / 9f) * (4f - 2f * Mathf.Cos(2f * Mathf.PI / n));
    }

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
        return kmf.mesh;
    }

    private int[] CalculateNeededVertices2(List<TEdges> edgesTuple, int currentPoint)
    {
        List<int> vertices = new List<int>();
        List<TEdges> edges = edgesTuple.Where(e => e.Item1 == currentPoint || e.Item2 == currentPoint).ToList();
        foreach (var e in edges)
        {
            if (currentPoint != e.Item1)
            {
                vertices.Add(e.Item1);
            }

            if (currentPoint != e.Item2)
            {
                vertices.Add(e.Item2);
            }
        }

        return vertices.ToArray();
    }

    /// <summary>
    /// Convert array of triangles to tuple of triangles "(0,1,2)"
    /// </summary>
    /// <param name="triangles">Original list</param>
    /// <returns>Converted list</returns>
    private static List<TTri> TrisToTuple(int[] triangles)
    {
        List<TTri> tris = new List<TTri>();
        for (int i = 0; i < triangles.Length; i = i + 3)
        {
            int idx1 = i + 1;
            int idx2 = i + 2;

            tris.Add(new TTri(
                triangles[i],
                triangles[idx1],
                triangles[idx2]
            ));
        }

        return tris;
    }

    /// <summary>
    /// Symplify geometry
    /// </summary>
    /// <param name="om"></param>
    /// <returns></returns>
    private static void MergeByDistance(ref Vector3[] vertices, ref int[] indices)
    {
        List<int> knowIndices = new List<int>();
        for (int i = 0; i < indices.Length; i++)
        {
            int index = indices[i];
            if (!knowIndices.Contains(index))
            {
                Vector3 pos = vertices[index];
                for (int j = 0; j < knowIndices.Count; j++)
                {
                    int kIndex = knowIndices[j];
                    if ((pos - vertices[kIndex]).sqrMagnitude <= tolerence)
                    {
                        index = indices[i] = kIndex;
                    }
                }

                knowIndices.Add(index);
            }
        }

        int maxIndex = indices.Max();
        Array.Resize(ref vertices, maxIndex + 1);
    }
    
    

    /// <summary>
    /// Get vertices by indexes 
    /// </summary>
    /// <param name="indexes">Index vertices needed</param>
    /// <param name="vertices">List vertices original</param>
    /// <returns>Filtered vertices</returns>
    private static Vector3[] GetVertices(int[] indexes, Vector3[] vertices)
    {
        List<Vector3> v = new List<Vector3>();
        foreach (int i in indexes)
        {
            v.Add(vertices[i]);
        }

        return v.ToArray();
    }

    /// <summary>
    /// Calculate sum of all vector
    /// </summary>
    /// <param name="vs"></param>
    /// <returns></returns>
    private static Vector3 Sum(int[] vs, Vector3[] vert)
    {
        Vector3 rv = Vector3.zero;
        foreach (var v in vs)
        {
            rv += vert[v];
        }

        return rv;
    }
}