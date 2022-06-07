using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KobbeltCreateMesh : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        // make changes to the Mesh by creating arrays which contain the new values
        mesh.vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0)};
        mesh.uv = new Vector2[] {new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0)};
        mesh.triangles =  new int[] {0, 1, 2};
    }
}
