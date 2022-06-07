using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Objects;
using UnityEngine;

namespace Objects
{
    [Serializable]
    public class Edge
    {
        private Point[] points = new Point[2];
        public Point firstPoint => points[0];
        public Point secondPoint => points[1];

        public Edge()
        {
            points[0] = new Point();
            points[1] = new Point();
        }

        public Edge(Point a, Point b)
        {
            points[0] = a;
            points[1] = b;
        }

        public Edge(List<Point> newPoints)
        {
            if (newPoints == null || newPoints.Count < 2) return;
            points[0] = newPoints[0];
            points[1] = newPoints[1];
        }

        [CanBeNull]
        public Edge FindPointsDifference(Edge b)
        {
            Point p1 = new Point();
            Point p2 = new Point();
            bool hasCommonPoint = false;
            foreach (Point pA in points)
            {
                foreach (Point pB in b.points)
                {
                    if (pA.Position == pB.Position)
                    {
                        hasCommonPoint = true;
                    }
                    else
                    {
                        p1 = pA;
                        p2 = pB;
                    }
                }
            }

            return hasCommonPoint ? new Edge(p1, p2) : null;
        }

        public Tuple<int, int> BelongsToTriangles(List<Triangle> triangles)
        {
            int foundTriangle = 0;
            int[] index = new int[2] { -1, -1 };

            for (int i = 0; i < triangles.Count; i++)
            {
                if (triangles[i].Contains(this))
                {
                    index[foundTriangle] = i;
                    foundTriangle++;
                }

                if (foundTriangle >= 2)
                    break;
            }

            return new Tuple<int, int>(index[0], index[1]);
        }

        public void DisplayEdge(ref LineRenderer lr, int indexLr)
        {
            lr.SetPosition(indexLr, points[0].Position);
            lr.SetPosition(indexLr + 1, points[1].Position);
        }

        public static bool operator ==(Edge a, Edge b)
        {
            return (a.firstPoint == b.firstPoint && a.secondPoint == b.secondPoint) ||
                   (a.firstPoint == b.secondPoint && a.secondPoint == b.firstPoint);
        }


        public static bool operator !=(Edge a, Edge b)
        {
            return !(a == b);
        }

        public Edge Reverse()
        {
            return new Edge(this.secondPoint, this.firstPoint);
        }
    }
}