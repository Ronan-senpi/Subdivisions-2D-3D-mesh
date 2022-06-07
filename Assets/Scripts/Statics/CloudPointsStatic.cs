using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;

public static class CloudPointsStatic
{
    private static int xMax = 18;
    private static int xMin = 0;
    private static int yMax = 10;
    private static int yMin = 0;
    
    public static  Point[]  Create2DCloudPoints(int nbPoints)
    {
        Point[] points = new Point[nbPoints];
        for (int i = 0; i < nbPoints; i++)
        {
            points[i] =new Point(new Vector3(
                Random.Range((float)xMin, (float)xMax), 
                Random.Range((float)yMin, (float)yMax),
                0f));
        }
        return points;
    }
}
