﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Objects;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ButterflySolver : MonoBehaviour
{
    [SerializeField] private MeshFilter mf;
    [SerializeField] private Material mat;
    private MeshObject originalObject;
    private Dictionary<(int, int), Vector3> edgePoints;
    private Vector3[] newPositions;
    private Edge currentEdge;
    private Triangle currentT1;
    private Triangle currentT2;

    private List<int> originalTri;
    private List<Vector3> originalVert;

    public void Subdivide()
    {
        Mesh mesh = mf.mesh;
        originalTri = new List<int>(mesh.triangles);
        originalVert = new List<Vector3>(mesh.vertices);
        Vector3[] meshVertices = mesh.vertices;
        int[] meshTriangles = mesh.triangles;
        MergeByDistance(ref meshVertices, ref meshTriangles);
        Debug.Log("Original vertices : " + originalVert.Count + " meshVertices : " + meshVertices.Length);
        
        List<(int, int)> edges = GetEdges(meshTriangles);
        edgePoints = GetEdgePoints(meshVertices, meshTriangles);
        List<(int, int, int)> tupledTriangle = ConvertTrianglesToTuple(meshTriangles);
        List<Vector3> finalPositions;
        List<int> finalIndices;
        List<Vector3> finalNormals;
        RecreateCreateMesh(out finalPositions,out finalIndices, out finalNormals, edgePoints,meshVertices,tupledTriangle);
        mf.mesh.vertices = finalPositions.ToArray();
        mf.mesh.triangles = finalIndices.ToArray();
        //mf.mesh.normals = finalNormals.ToArray();
    }
    
    private void RecreateCreateMesh(out List<Vector3> finalPositions,out List<int> finalIndices, out List<Vector3> finalNormals, Dictionary<(int, int), Vector3> edgePoints, Vector3[] newVertices, List<(int, int, int)> tupledTriangle)
    {
        finalPositions = new List<Vector3>(newVertices);
        finalIndices = new List<int>();
        finalNormals = new List<Vector3>();
        foreach (var triangle in tupledTriangle)
        {
            int count = finalPositions.Count;//the previous amount of positions;
            Vector3 A = newVertices[triangle.Item1];
            Vector3 B = newVertices[triangle.Item2];
            Vector3 C = newVertices[triangle.Item3];
            Vector3 AB = edgePoints[(triangle.Item1, triangle.Item2)];
            Vector3 BC = edgePoints[(triangle.Item2, triangle.Item3)];
            Vector3 CA = edgePoints[(triangle.Item3, triangle.Item1)];

            finalPositions.Add(A); // + 0
            finalPositions.Add(B); // + 1
            finalPositions.Add(C); // + 2
            finalPositions.Add(AB);// + 3
            finalPositions.Add(BC);// + 4
            finalPositions.Add(CA);// + 5
            
            Vector3 n = Vector3.zero;
            //First tri A AB CA
            finalIndices.Add(count+0);
            finalIndices.Add(count+3);
            finalIndices.Add(count+5);
            n = Vector3.Cross((AB-A),CA-A).normalized;
            finalNormals.Add(n);
            //Second tri CA BC C
            finalIndices.Add(count+5);
            finalIndices.Add(count+4);
            finalIndices.Add(count+2);
            n = Vector3.Cross((BC-CA),C-CA).normalized;
            finalNormals.Add(n);
            //Third tri AB B BC
            finalIndices.Add(count+3);
            finalIndices.Add(count+1);
            finalIndices.Add(count+4);
            n = Vector3.Cross((B-AB),BC-AB).normalized;
            finalNormals.Add(n);
            //Fourth tri AB BC CA
            finalIndices.Add(count+3);
            finalIndices.Add(count+4);
            finalIndices.Add(count+5);
            n = Vector3.Cross((BC-AB),CA-AB).normalized;
            finalNormals.Add(n);
        }
    }


    private List<(int, int, int)> ConvertTrianglesToTuple(int[] meshTriangles)
    {
        List<(int, int, int)> tris = new List<(int, int, int)>();
        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            int a = meshTriangles[i];
            int b = meshTriangles[i+1];
            int c = meshTriangles[i+2];
            tris.Add((a,b,c));
        }

        return tris;
    }

    private Dictionary<(int, int), Vector3> GetEdgePoints(Vector3[] meshVertices, int[] meshTriangles)
    {
        Dictionary<(int, int), Vector3> edgePts = new Dictionary<(int, int), Vector3>();
        List<(int, int, int)> tupledTriangle = ConvertTrianglesToTuple(meshTriangles);
        List<(int, int)> edges = GetEdges(meshTriangles);
        foreach (var edge in edges)
        {
            List<(int,int,int)> trisValids = GetTrisWithEdge(edge, tupledTriangle);
            Debug.Log("trisValids count " + trisValids.Count);
            (int, int, int) t1 = trisValids[0];
            (int, int, int) t2 = trisValids[1];
            Debug.Log("edge : " + edge);
            Debug.Log("T1 : " + t1);
            Debug.Log("T2 : " + t2);
            int vLeftId = GetLastPoint(t1,edge);
            int vRightId = GetLastPoint(t2,edge);
            int v1 = edge.Item1;
            int v2 = edge.Item2;
            int va = vLeftId;
            int vb = vRightId;
            Debug.Log("va : " + va);
            Debug.Log("vb : " + vb);
            (int, int, int) C = GetTriWithEdge((v1, va), tupledTriangle, t1);
            Debug.Log("C " + C);
            int vc = GetLastPoint(C,(v1, va));
            (int, int, int) D = GetTriWithEdge((va, v2), tupledTriangle, t1);
            Debug.Log("D " + D);
            int vd = GetLastPoint(D,(va, v2));
            (int, int, int) E = GetTriWithEdge((v2, vb), tupledTriangle, t2);
            Debug.Log("E " + E);
            int ve = GetLastPoint(E,(v2, vb));
            (int, int, int) F = GetTriWithEdge((vb, v1), tupledTriangle, t2);
            Debug.Log("F " + F);
            int vf = GetLastPoint(F,(vb, v1));
            edgePts[edge] = GetEdgePoint(v1, v2, va, vb,vc,vd,ve,vf, meshVertices);
        }
        return edgePts;
    }
    private Vector3 GetEdgePoint(int v1, int v2, int va, int vb, int vc, int vd, int ve, int vf, Vector3[] positions)
    {
        Debug.Log("v1 " + v1 + " v2 " + v2 + " va " + va + " vb " + vb + " vc " + vc + " vd " + vd + " ve " + ve + " vf " + vf);
        Vector3 v1Pos = positions[v1];
        Vector3 v2Pos = positions[v2];
        Vector3 vaPos = positions[va];
        Vector3 vbPos = positions[vb];
        Vector3 vcPos = positions[vc];
        Vector3 vdPos = positions[vd];
        Vector3 vePos = positions[ve];
        Vector3 vfPos = positions[vf];
        Debug.Log("v1Pos " + v1Pos + " v2Pos " + v2Pos + " vaPos " + vaPos + " vbPos " + vbPos + " vcPos " + vcPos + " vdPos " + vdPos + " vePos " + vePos + " vfPos " + vfPos);
        return (v1Pos + v2Pos) / 2f + (vaPos + vbPos) / 8f - (vcPos + vdPos + vePos + vfPos) / 16f;
    }

    private int GetLastPoint((int, int, int) t, (int, int) edge)
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
    private static List<(int,int,int)> GetTrisWithVertex(int vert, List<(int,int,int)> tris)
    {
        List<(int,int,int)> filteredTris =
            tris.FindAll(t => t.Item1 == vert || t.Item2 == vert || t.Item3 == vert);
        return filteredTris;
    }
    /// <summary>
    /// Find all tris with specific vertex in there triangle
    /// </summary>
    /// <param name="vert">specific vertex</param>
    /// <param name="tris">List of triangles</param>
    /// <param name="triToExclude">if we want exclude a triangle of filtered tris</param>
    /// <returns>List of filtered tris</returns>
    private static List<(int,int,int)> GetTrisWithEdge((int,int) edge, List<(int,int,int)> tris)
    {
        List<(int,int,int)> filteredTris =
            tris.FindAll(t => (t.Item1 == edge.Item1 || t.Item2 == edge.Item1 || t.Item3 == edge.Item1) &&
                              (t.Item1 == edge.Item2 || t.Item2 == edge.Item2 || t.Item3 == edge.Item2));
        return filteredTris;
    }

    private static (int,int,int) GetTriWithEdge((int,int) edge, List<(int,int,int)> tris, (int,int,int) excludedTriangle)
    {
        List<(int,int,int)> filteredTris =
            tris.FindAll(t => (edge.Item1 == t.Item1 ||
                                      edge.Item1 == t.Item2 ||
                                      edge.Item1 == t.Item3)
                                            &&
                                     (edge.Item2 == t.Item1 ||
                                      edge.Item2 == t.Item2 ||
                                      edge.Item2 == t.Item3));

        filteredTris.Remove(excludedTriangle);
        //filteredTris.FindAll(t => !(t.Item1 == vertexExcluded || t.Item2 == vertexExcluded || t.Item3 == vertexExcluded));
        return filteredTris[0];
    }

    private Mesh CreateGeometry(string name, Vector3[] verts, int[] tris, Vector3 pos)
    {
        GameObject Looped = new GameObject(name);
        Looped.transform.position = pos;
        MeshFilter kmf = Looped.AddComponent<MeshFilter>();
        MeshRenderer kmr = Looped.AddComponent<MeshRenderer>();
        kmr.material = mat;

        kmf.mesh.Clear();
        kmf.mesh.vertices = verts;
        kmf.mesh.triangles = tris;
        return kmf.mesh;
    }
    
    public static List<(int,int)> GetEdges(int[] indices) {
        var edges = new List<(int,int)>();
        int triCount = indices.Length / 3;

        for (int i = 0; i < triCount; ++i) {
            int tri = i * 3;
            var e1 = (indices[tri], indices[tri + 1]);
            var e2 = (indices[tri + 1], indices[tri + 2]);
            var e3 = (indices[tri + 2], indices[tri]);
                
            if(!edges.Contains(e1)) edges.Add(e1);
            if(!edges.Contains(e2)) edges.Add(e2);
            if(!edges.Contains(e3)) edges.Add(e3);
        }
        return edges;
    }
    /// <summary>
    /// Symplify geometry
    /// </summary>
    /// <param name="om"></param>
    /// <returns></returns>
    private static void MergeByDistance(ref Vector3[] vertices, ref int[] indices)
    {
        const float tolerence = 0.001f;
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

    private void OnDrawGizmos()
    {
        if (edgePoints != null)
        {
            Gizmos.color = Color.red;
            foreach (var pair in edgePoints)
            {
                Gizmos.DrawSphere(mf.transform.localToWorldMatrix * pair.Value, 0.05f);
            }
        }
        if (newPositions != null)
        {
            Gizmos.color = Color.green;
            foreach (var pos in newPositions)
            {
                Gizmos.DrawSphere(mf.transform.localToWorldMatrix * pos, 0.05f);
            }
        }
        if (currentT1 != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(currentT1.Edges[0].firstPoint.Position,currentT1.Edges[0].secondPoint.Position);
            Gizmos.DrawLine(currentT1.Edges[1].firstPoint.Position,currentT1.Edges[1].secondPoint.Position);
            Gizmos.DrawLine(currentT1.Edges[2].firstPoint.Position,currentT1.Edges[2].secondPoint.Position);
        }

        if (currentT2 != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(currentT2.Edges[0].firstPoint.Position, currentT2.Edges[0].secondPoint.Position);
            Gizmos.DrawLine(currentT2.Edges[1].firstPoint.Position, currentT2.Edges[1].secondPoint.Position);
            Gizmos.DrawLine(currentT2.Edges[2].firstPoint.Position, currentT2.Edges[2].secondPoint.Position);
        }

        if (currentEdge != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(currentEdge.firstPoint.Position, currentEdge.secondPoint.Position);
        }
        
    }

}