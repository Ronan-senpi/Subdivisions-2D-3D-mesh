using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GenerateByClick))]
public class ChaikinSolver : MonoBehaviour
{
    private GenerateByClick generator;
    
    
    [SerializeField] private LineRenderer lineRenderer;
    private float lerpFactor = 0.3333f;
    
    private Vector3[] positions;
    private int subdivisionCount = 1;
    private int subdivisionsDone;
    

    private void Awake()
    {
        generator = gameObject.GetComponent<GenerateByClick>();
        positions = generator.GetPositions();
    }

    private void Update()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = positions.Length;
            for (int i = 0; i < positions.Length; i++)
            {
                lineRenderer.SetPosition(i,positions[i]);
            }
        }
    }

    public void Subdivide()
    {
        subdivisionsDone = 0;
        List<Vector3> newPositions = new List<Vector3>(positions);
        while (subdivisionsDone < subdivisionCount)
        {
            Debug.Log("SUBDIVIDING : " + (subdivisionCount));
            var previousPositions = newPositions;
            newPositions = new List<Vector3>();
            for (int i = 0; i < previousPositions.Count - 1; i++)
            {
                Debug.Log("for point : " + (i));
                Vector3 p1 = previousPositions[i];
                Vector3 p2 = previousPositions[i+1];
                Vector3 p3 = Lerp(p1, p2, lerpFactor);
                Vector3 p4 = Lerp(p1, p2, 1f-lerpFactor);
                newPositions.Add(p3);
                newPositions.Add(p4);
            }
            subdivisionsDone++;
        }

        positions = newPositions.ToArray();
    }

    private Vector3 Lerp(Vector3 pos1, Vector3 pos2, float factor)
    {
        float x = pos1.x * (1 - factor) + pos2.x * factor;
        float y = pos1.y * (1 - factor) + pos2.y * factor;
        float z = pos1.z * (1 - factor) + pos2.z * factor;
        return new Vector3(x,y,z);
    }

    public void ReloadSubdivisionFromSlider(Slider slider)
    {
        subdivisionCount = (int)slider.value;
    }
    public void ReloadLerpFromSlider(Slider slider)
    {
        lerpFactor = slider.value;
    }
    public void RefreshFromGenerator()
    {
        positions = generator.GetPositions();
    }
}