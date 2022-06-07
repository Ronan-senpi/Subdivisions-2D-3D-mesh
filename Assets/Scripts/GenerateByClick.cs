using System.Collections.Generic;
using Objects;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class GenerateByClick : MonoBehaviour
{
    public static List<Point> points = new List<Point>();
    public GameObject pointPrefab;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            GeneratePointOnClick();
            
        }
    }

    private void GeneratePointOnClick()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Instantiate(pointPrefab, mousePos, Quaternion.identity, transform);
        points.Add(new Point(mousePos));
    }

    public Vector3[] GetPositions()
    {
        Vector3[] positions = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            positions[i] = points[i].Position;
        }

        return positions;
    }
}