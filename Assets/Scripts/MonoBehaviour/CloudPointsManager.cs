using System;
using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;

public class CloudPointsManager : MonoBehaviour
{
    public static CloudPointsManager Instance { get; set; }
    
    [SerializeField] GameObject pointGo;
    [SerializeField] GameObject barrycenterPrefab;
    [SerializeField] Transform container;
    
    private LineRenderer convLr;
    
    private Point[] points;
    private Point barycenter = new Point(Vector3.zero);

    private bool doSomething = false;

    private int pointIndex = 0;

    private bool showSort = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        if (!TryGetComponent(out convLr))
        {
            throw new System.Exception("Wsh met un LineRenderer sur l'enfant de l'enfant stp");
        }
    }

    private int frameCounter = 0;
    private Point currentPoint = new Point(Vector3.zero);
    void Update()
    {

        if (showSort && points != null && pointIndex < points.Length)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                doSomething = true;
            }

            if (frameCounter == 100)
            {
                frameCounter = 0;
                currentPoint = points[pointIndex];
                
                pointIndex++;
            }
            else
                frameCounter++;
        }

       // Debug.DrawLine(barycenter.Position, currentPoint.Position,Color.green);
    }

    public void GenerateCloudsPoints()
    {
        for (int i = 0; i < container.childCount; i++)
        {
            PointController pc;
            if (container.GetChild(i).TryGetComponent(out pc))
            {
                pc.DestroyGO();
            }
        }
        points = CloudPointsStatic.Create2DCloudPoints(100);
        for (int i = 0; i < points.Length; i++)
        {
            points[i].Go = Instantiate(pointGo, points[i].Position, Quaternion.identity, container);
            barycenter.Position += points[i].Position;
        }

        barycenter.Position /= points.Length;
     //   barycenter.Go = Instantiate(barrycenterPrefab, barycenter.Position, Quaternion.identity, container);
        
    }

    public void SetPoint(Point[] points)
    {
        this.points = points;
    }
    public void ResetLineRenderer()
    {
        convLr.positionCount = 0;
    }

    public void ShowSort()
    {
        frameCounter = 0;
        pointIndex = 0;
        showSort = true;
    }
    public Point[] GetPoints()
    {
        return points;
    }

    public Point GetBarrycenter()
    {
        return barycenter;
    }

    public LineRenderer GetLineRenderer()
    {
        return convLr;
    }
    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n){
        // angle in [0,180]
        float angle = Vector3.Angle(a,b);
        float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        // angle in [0,360] (not used but included here for completeness)
        float angle360 =  (signed_angle + 180) % 360;

        return angle360;
    }
}
