﻿using System;
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
        public int index;

        public Point VertexPoint { get; private set; }

        public List<Edge> edgeParents = new List<Edge>();
        public List<Face> faceParents = new List<Face>();

        public Point()
        {
        }

        public Point(Vector3 pos)
        {
            this.Position = pos;
        }

        public Point(Vector3 pos, int index)
        {
            this.Position = pos;
            this.index = index;
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

        public void ComputeVertexPoint()
        {
            int n = edgeParents.Count;

            Vector3 v = Vector3.zero;
            Vector3 q = Vector3.zero;
            Vector3 r = Vector3.zero;
            foreach (Face f in faceParents)
                q += f.FacePoints.Position;


            foreach (Edge e in edgeParents)
                r += e.EdgePoint.Position;

            q /= faceParents.Count;
            r /= edgeParents.Count;

            v = q / n + (2 * r) / n;
            VertexPoint = new Point(v);
        }

        public List<int> BelongsToFaces(List<Face> faces)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < faces.Count; i++)
            {
                if (faces[i].Contains(this))
                {
                    indexes.Add(i);
                }
            }

            return indexes;
        }

        public List<int> BelongsToEdges(List<Edge> edges)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i].Contains(this))
                    indexes.Add(i);
            }

            return indexes;
        }
    }
}