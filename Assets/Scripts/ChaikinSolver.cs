using System;
using UnityEngine;

[RequireComponent(typeof(GenerateByClick))]
public class ChaikinSolver : MonoBehaviour
{
    private GenerateByClick generator;
    [SerializeField] private LineRenderer lineRenderer;

    private void Awake()
    {
        generator = gameObject.GetComponent<GenerateByClick>();
    }

    private void Update()
    {
        if (lineRenderer != null)
        {
            Vector3[] positions = generator.GetPositions();
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }
    }
}