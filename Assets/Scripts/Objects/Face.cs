using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;

namespace Objects
{
    public class Face
    {
        public List<Edge> Edges { get; private set; }

        public Point FacePoints { get; private set; }

        public Face(List<Edge> newEdges)
        {
            Edges = newEdges;
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

            FacePoints = new Point(centroid /= Edges.Count);
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

            if (compareCount == Edges.Count)
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
    }
}