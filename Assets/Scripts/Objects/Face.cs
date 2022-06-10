using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;

namespace Objects
{
    public class Face
    {
        private Edge[] edges = new Edge[3];

        public Edge[] Edges
        {
            get { return edges; }
            private set { edges = value; }
        }

        public Point FacePoints { get; private set; }

        public Face()
        {
            Edges[0] = new Edge();
            Edges[1] = new Edge();
            Edges[2] = new Edge();
        }

        public Face(List<Edge> newEdges)
        {
            Edges = newEdges.ToArray();
        }


        /// <summary>
        /// Calculate the gravity center of the face
        /// </summary>
        /// <returns>The position of the center of the face</returns>
        public void ComputeFacePoint()
        {
            Vector3 centroid = Vector3.zero;
            foreach (Edge edge in Edges)
                centroid += edge.firstPoint.Position;

            FacePoints = new Point(centroid /= Edges.Length);
        }

        /// <summary>
        /// Check if an edge belongs to the face
        /// </summary>
        /// <param name="edgeContained">The edge to check</param>
        /// <returns>True if edge is found, false otherwise</returns>
        public bool Contains(Edge edgeContained)
        {
            foreach (Edge edge in Edges)
            {
                if (edge.CompareEdge(edgeContained))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if a point belongs to the face
        /// </summary>
        /// <param name="pointContained">The point to check</param>
        /// <returns>True if point is found, false otherwise</returns>
        public bool Contains(Point pointContained)
        {
            foreach (Edge edge in Edges)
            {
                if (edge.firstPoint.Position == pointContained.Position ||
                    edge.secondPoint.Position == pointContained.Position)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Compare each points of two faces to determine similitude
        /// </summary>
        /// <param name="otherFace"></param>
        /// <returns>true if both faces are the same, false otherwise</returns>
        public bool CompareFaces(Face otherFace)
        {
            int compareCount = 0;
            foreach (Edge e in Edges)
            {
                foreach (Edge otherE in otherFace.Edges)
                {
                    if (e.CompareEdge(otherE))
                    {
                        compareCount++;
                        break;
                    }
                }
            }

            if (compareCount == Edges.Length)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return the points forming the face as a tuple
        /// </summary>
        public (Point, Point, Point) GetPoints()
        {
            List<Point> points = new List<Point>();
            foreach (Edge e in Edges)
            {
                points.Add(e.firstPoint);
                points.Add(e.secondPoint);
            }

            points = points.GroupBy(x => x).Select(x => x.First()).ToList();
            return (points[0], points[1], points[2]);
        }

        public void SetEdges(Edge a, Edge a1, Edge a2)
        {
            Edges[0] = a;
            if (a.secondPoint.Position == a1.firstPoint.Position)
            {
                Edges[1] = a1;
            }
            else if (a.secondPoint.Position == a1.secondPoint.Position)
            {
                Edges[1] = a1.Reverse();
            }

            if (a.firstPoint.Position == a1.secondPoint.Position)
            {
                Edges[2] = a1;
            }
            else if (a.firstPoint.Position == a1.firstPoint.Position)
            {
                Edges[2] = a1.Reverse();
            }

            if (a.secondPoint.Position == a2.firstPoint.Position)
            {
                Edges[1] = a2;
            }
            else if (a.secondPoint.Position == a2.secondPoint.Position)
            {
                Edges[1] = a2.Reverse();
            }

            if (a.firstPoint.Position == a2.secondPoint.Position)
            {
                Edges[2] = a2;
            }
            else if (a.firstPoint.Position == a2.firstPoint.Position)
            {
                Edges[2] = a2.Reverse();
            }
        }
    }
}