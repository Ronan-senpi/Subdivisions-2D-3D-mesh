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

        public Point EdgePoint { get; private set; }

        public List<Face> faceParents = new List<Face>();

        public Edge()
        {
            points[0] = new Point();
            points[1] = new Point();
            EdgePoint = null;
            points[0].edgeParents.Add(this);
            points[0].faceParents = this.faceParents;
            points[1].edgeParents.Add(this);
            points[1].faceParents = this.faceParents;
        }

        public Edge(Point a, Point b)
        {
            points[0] = a;
            points[1] = b;
            points[0].edgeParents.Add(this);
            points[0].faceParents = this.faceParents;
            points[1].edgeParents.Add(this);
            points[1].faceParents = this.faceParents;


            EdgePoint = null;
        }

        public Edge(List<Point> newPoints)
        {
            if (newPoints == null || newPoints.Count < 2) return;
            points[0] = newPoints[0];
            points[1] = newPoints[1];
            points[0].edgeParents.Add(this);
            points[0].faceParents = this.faceParents;
            points[1].edgeParents.Add(this);
            points[1].faceParents = this.faceParents;

            EdgePoint = null;
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

        public int BelongsToFace(List<Face> faces)
        {
            int index = -1;

            for (int i = 0; i < faces.Count; i++)
            {
                if (faces[i].Contains(this))
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public void DisplayEdge(ref LineRenderer lr, int indexLr)
        {
            lr.SetPosition(indexLr, points[0].Position);
            lr.SetPosition(indexLr + 1, points[1].Position);
        }

        public Edge Reverse()
        {
            return new Edge(this.secondPoint, this.firstPoint);
        }

        public void ComputeEdgePoint()
        {
            Debug.Log(faceParents.Count);
            EdgePoint = new Point((points[0].Position + points[1].Position + faceParents[0].FacePoints.Position +
                                   faceParents[1].FacePoints.Position) / 4);
        }

        public bool Contains(Point point)
        {
            if (point == firstPoint || point == secondPoint)
                return true;

            return false;
        }

        public bool CompareEdge(Edge b)
        {
            return (Mathf.Abs((this.firstPoint.Position - b.firstPoint.Position).magnitude) < 0.001f &&
                    Mathf.Abs((this.secondPoint.Position - b.secondPoint.Position).magnitude) < 0.001f) ||
                   (Mathf.Abs((this.firstPoint.Position - b.secondPoint.Position).magnitude) < 0.001f &&
                    Mathf.Abs((this.secondPoint.Position - b.firstPoint.Position).magnitude) < 0.001f);
        }

        /// <summary>
        /// Returns the indices of the two points, in ascending order
        /// </summary>
        /// <returns>a tuple containing the two indices</returns>
        public (int, int) getIndices()
        {
            int i1 = points[0].index;
            int i2 = points[1].index;
            
            if (i1 > i2) return (i2, i1);
            return (i1, i2);
        }
    }
}