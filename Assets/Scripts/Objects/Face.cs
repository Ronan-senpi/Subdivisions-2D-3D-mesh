using System.Collections;
using System.Collections.Generic;
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

        public bool Contains(Edge edgeContained)
        {
            foreach (Edge edge in Edges)
            {
                if (edge == edgeContained)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(Point pointContained)
        {
            foreach (Edge edge in Edges)
            {
                if (edge.firstPoint == pointContained || edge.secondPoint == pointContained)
                {
                    return true;
                }
            }

            return false;
        }
    }
}