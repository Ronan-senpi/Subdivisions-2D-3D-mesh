using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Objects;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class LoopSolver : MonoBehaviour
{
    [SerializeField] private MeshFilter mf;

    private void Start()
    {
        List<Triangle> triangles = MathTools.ConvertMesh(mf.mesh);
    }

}
