using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Objects
{
    public class Triangle
    {
        private Edge[] edges = new Edge[3];

        public Edge[] Edges
        {
            get { return edges; }
            set { edges = value; }
        }

        private Vector3 centerCircle;
        public Vector3 Center => centerCircle;
        private float rCircle;
        public float Ray => rCircle;

        public Triangle()
        {
            edges[0] = new Edge();
            edges[1] = new Edge();
            edges[2] = new Edge();
        }

        public Triangle(Edge a, Edge b, Edge c)
        {
            SetEdges(a, b, c);
        }

        public Triangle(List<Edge> newPoints)
        {
            if (newPoints == null || newPoints.Count <= 2) return;
            edges[0] = newPoints[0];
            edges[1] = newPoints[1];
            edges[2] = newPoints[2];
        }


        public void DisplayTriangle(ref LineRenderer lr)
        {
            lr.SetPosition(0, edges[0].firstPoint.Position);
            lr.SetPosition(1, edges[1].firstPoint.Position);
            lr.SetPosition(2, edges[2].firstPoint.Position);
        }

        public bool Contains(Edge edgeContained)
        {
            foreach (Edge edge in edges)
            {
                if (edge == edgeContained)
                {
                    return true;
                }
            }

            return false;
        }

        public List<Vector3> GetVertex()
        {
            return new List<Vector3>
            {
                new Vector3(edges[0].firstPoint.Position.x, edges[0].firstPoint.Position.y,
                    edges[0].firstPoint.Position.z),
                new Vector3(edges[1].firstPoint.Position.x, edges[1].firstPoint.Position.y,
                    edges[1].firstPoint.Position.z),
                new Vector3(edges[2].firstPoint.Position.x, edges[2].firstPoint.Position.y,
                    edges[2].firstPoint.Position.z)
            };
        }

        public Point GetLastVertex(Edge edge)
        {
            Vector3 a = new Vector3(edge.firstPoint.Position.x, edge.firstPoint.Position.y, edge.firstPoint.Position.z);
            Vector3 b = new Vector3(edge.secondPoint.Position.x, edge.secondPoint.Position.y,
                edge.secondPoint.Position.z);
            List<Vector3> tmp = GetVertex();
            tmp.Remove(a);
            tmp.Remove(b);
            return new Point(tmp[0]);
        }

        public Tuple<Edge, Edge> GetOtherEdges(Edge a)
        {
            Edge firstEdge = new Edge();
            Edge secondEdge = new Edge();
            foreach (Edge e in edges)
            {
                if ((e.firstPoint.Position == a.secondPoint.Position ||
                     e.secondPoint.Position == a.secondPoint.Position) && e != a)
                    firstEdge = e;


                if ((e.firstPoint.Position == a.firstPoint.Position ||
                     e.secondPoint.Position == a.firstPoint.Position) && e != a)
                    secondEdge = e;
            }

            return new Tuple<Edge, Edge>(firstEdge, secondEdge);
        }

        public void CreateCircumcircle()
        {
            Vector3 A = edges[0].firstPoint.Position;
            Vector3 B = edges[0].secondPoint.Position;
            Vector3 C = edges[1].secondPoint.Position;

            rCircle = ((A - B).magnitude * (B - C).magnitude * (C - A).magnitude) /
                      (2 * Vector3.Cross((A - B), (B - C)).magnitude);

            float alpha = Mathf.Pow((B - C).magnitude, 2) * Vector3.Dot((A - B), A - C)
                          / (2 * Mathf.Pow(Vector3.Cross((A - B), (B - C)).magnitude, 2));
            float beta = Mathf.Pow((A - C).magnitude, 2) * Vector3.Dot((B - A), B - C)
                         / (2 * Mathf.Pow(Vector3.Cross((A - B), (B - C)).magnitude, 2));
            float gamma = Mathf.Pow((A - B).magnitude, 2) * Vector3.Dot((C - A), C - B)
                          / (2 * Mathf.Pow(Vector3.Cross((A - B), (B - C)).magnitude, 2));

            centerCircle = alpha * A + beta * B + gamma * C;
        }

        public bool VerifyDelaunayCriteria(Vector3 pointTriangulation)
        {
            CreateCircumcircle();
            List<Vector3> trianglePoint = GetVertex();
            if ((pointTriangulation - centerCircle).magnitude < rCircle)
            {
                return false;
            }


            return true;
        }

        public void SetEdges(Edge a, Edge a1, Edge a2)
        {
            edges[0] = a;
            if (a.secondPoint.Position == a1.firstPoint.Position)
            {
                edges[1] = a1;
            }
            else if (a.secondPoint.Position == a1.secondPoint.Position)
            {
                edges[1] = a1.Reverse();
            }

            if (a.firstPoint.Position == a1.secondPoint.Position)
            {
                edges[2] = a1;
            }
            else if (a.firstPoint.Position == a1.firstPoint.Position)
            {
                edges[2] = a1.Reverse();
            }

            if (a.secondPoint.Position == a2.firstPoint.Position)
            {
                edges[1] = a2;
            }
            else if (a.secondPoint.Position == a2.secondPoint.Position)
            {
                edges[1] = a2.Reverse();
            }

            if (a.firstPoint.Position == a2.secondPoint.Position)
            {
                edges[2] = a2;
            }
            else if (a.firstPoint.Position == a2.firstPoint.Position)
            {
                edges[2] = a2.Reverse();
            }
        }

        float sign(Point p1, Point p2, Point p3)
        {
            return (p1.Position.x - p3.Position.x) * (p2.Position.y - p3.Position.y) -
                   (p2.Position.x - p3.Position.x) * (p1.Position.y - p3.Position.y);
        }

        public bool IsInside(Point p)
        {
            Vector3 p0 = edges[0].firstPoint.Position;
            Vector3 p1 = edges[1].firstPoint.Position;
            Vector3 p2 = edges[2].firstPoint.Position;
            double area = 0.5 * (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);

            double s = 1 / (2 * area) *
                       (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.Position.x + (p0.x - p2.x) * p.Position.y);

            double t = 1 / (2 * area) *
                       (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.Position.x + (p1.x - p0.x) * p.Position.y);

            return (s >= 0 && s <= 1 && t >= 0 && t <= 1 && (s + t) <= 1);

            // float d1, d2, d3;
            // bool has_neg, has_pos;
            //
            // d1 = sign(pt, v1, v2);
            // d2 = sign(pt, v2, v3);
            // d3 = sign(pt, v3, v1);
            //
            // has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            // has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);
            //
            // return !(has_neg && has_pos);

            // foreach (var t in edges)
            // {
            //     Vector3 normal = Vector3.Cross((t.secondPoint.Position - t.firstPoint.Position).normalized,
            //         Vector3.forward);
            //     Vector3 vectorToPoint = p.Position - t.firstPoint.Position;
            //     float dotProduct = Vector3.Dot(normal.normalized, vectorToPoint);
            //     if (dotProduct > 0)
            //     {
            //         return false;
            //     }
            // }
            //
            // return true;
        }
    }
}