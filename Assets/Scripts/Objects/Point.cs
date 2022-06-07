using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    [Serializable]
    public class Point : IComparable
    {
        public Vector3 Position;
        public GameObject Go;

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
    }
}