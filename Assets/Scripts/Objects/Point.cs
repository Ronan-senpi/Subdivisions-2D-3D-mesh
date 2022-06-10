using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Objects
{
    [Serializable]
    public class Point : IComparable
    {
        public Vector3 Position;
        public GameObject Go;

        public Point VertexPoint { get; private set; }

        public Point()
        {
        }

        public Point(Vector3 pos)
        {
            this.Position = pos;
        }

        public Point(GameObject go, Vector3 pos)
        {
            this.Go = go;
            this.Position = pos;
        }

        public float Angle { get; set; }

        public void ApplyMat(Material mat)
        {
            Renderer rend;
            if (Go.TryGetComponent(out rend))
            {
                rend.material = mat;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is Point otherPoint)
            {
                int xCompare = this.Position.x.CompareTo(otherPoint.Position.x);
                return xCompare == 0 ? this.Position.y.CompareTo(otherPoint.Position.y) : xCompare;
            }
            else
                throw new ArgumentException("Object is not a Point");
        }

        /// <summary>
        /// Calculate perturbation of vertex point
        /// </summary>
        /// <param name="faceParents">list of faces connected to point</param>
        /// <param name="edgeParents">:list of edges connected to point</param>
        public void ComputeVertexPoint(List<Face> faceParents, List<Edge> edgeParents)
        {
            Vector3 v = Vector3.zero;
            Vector3 q = Vector3.zero;
            Vector3 r = Vector3.zero;
            foreach (Face f in faceParents)
                q += f.FacePoints.Position;


            foreach (Edge e in edgeParents)
                r += (e.firstPoint.Position + e.secondPoint.Position) / 2;

            q /= faceParents.Count;
            r /= edgeParents.Count;

            v = (q / edgeParents.Count) + ((2 * r) / edgeParents.Count) +
                ((edgeParents.Count - 3) * this.Position) / edgeParents.Count;
            VertexPoint = new Point(v);
        }

        /// <summary>
        /// Find faces connected to points
        /// </summary>
        /// <param name="faces">List of faces to check</param>
        /// <returns>List of faces connected</returns>
        public List<Face> BelongsToFaces(List<Face> faces)
        {
            List<Face> belongedFaces = new List<Face>();
            for (int i = 0; i < faces.Count; i++)
            {
                if (faces[i].Contains(this))
                {
                    belongedFaces.Add(faces[i]);
                }
            }

            return belongedFaces;
        }
        
        /// <summary>
        /// Find edges connected to points
        /// </summary>
        /// <param name="edges">List of edges to check</param>
        /// <returns>List of edges connected</returns>
        public List<Edge> BelongsToEdges(List<Edge> edges)
        {
            List<Edge> belongedEdges = new List<Edge>();
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i].Contains(this))
                {
                    belongedEdges.Add(edges[i]);
                }
            }

            return belongedEdges;
        }
    }
}