using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PRO1
{
    public class Entities
    {
        public enum brushesColor { normal, highlight, RelConstLen, RelPerpendicular}
        public static Color[] colorEdge = { Color.Black, Color.DarkOrange, Color.LightGreen, Color.Red };
        public static Brush[] brushesVertex = { Brushes.Black, Brushes.DarkOrange, Brushes.LightGreen, Brushes.Red };

        public static Color GetColorFromBrush(brushesColor color)
        {
            return colorEdge[(int)color];
        }

        public class Vertex
        {
            public Point point;
            public brushesColor brush { get; set; }
            public Polygon parent;

            public void ChangePlacement(Point _p)
            {
                point = _p;
            }
            public void ChangePlacement(int _X, int _Y)
            {
                point.X = _X;
                point.Y = _Y;
            }
            public void Move(int _dX, int _dY)
            {
                point.X += _dX;
                point.Y += _dY;
            }
            public void SetColor(brushesColor _color)
            {
                brush = _color;
            }
            public List<Edge> GetEdges()
            {
                List<Edge> list = new List<Edge>();
                foreach (Edge edge in parent.edges)
                {
                    if (edge.v1 == this || edge.v2 == this) list.Add(edge);
                }
                return list;
            }
            public List<Vertex> GetNeighbors()
            {
                List<Vertex> list = new List<Vertex>();
                foreach (Edge edge in GetEdges())
                {
                    if(edge.v1 == this || edge.v2 == this) list.Add(edge.GetNeighbor(this));
                }
                return list;
            }
            public void EnforceRelation()
            {   
                foreach (Edge edge in parent.edges) edge.visited = false;
                Queue<Vertex> q = new Queue<Vertex>();
                q.Enqueue(this);
                Vertex tmp;

                while (q.Count > 0)
                {
                    tmp = q.Dequeue();
                    foreach (Edge e in tmp.GetEdges())
                        if (e.relation != null && e.visited == false)
                        {
                            e.visited = true;
                            e.relation.Enforce(tmp);
                            q.Enqueue(e.GetNeighbor(tmp));
                        }
                }
            }
            public Vertex(Point _p, Polygon _parent, brushesColor _brush)
            {
                point = _p;
                parent = _parent;
                brush = _brush;
            }
        }
        public class Edge
        {
            public Vertex v1, v2;
            public IRelation relation = null;
            public brushesColor color;
            public Polygon parent;
            public bool visited;

            public Edge(Vertex _v1, Vertex _v2, Polygon _parent, brushesColor _color)
            {
                v1 = _v1;
                v2 = _v2;
                parent = _parent;
                color = _color;
            }
            public Edge(Vertex _v1, Vertex _v2, Polygon _parent)
            {
                v1 = _v1;
                v2 = _v2;
                parent = _parent;
                color = brushesColor.normal;
            }
            public void Move(int dx, int dy)
            {
                v1.Move(dx, dy);
                v2.Move(dx, dy);
            }
            public void SetColor(brushesColor _color)
            {
                if (relation != null && _color == brushesColor.normal)
                    color = relation.Color();
                else
                    color = _color;
            }
            public Vertex GetNeighbor(Vertex _base)
            {
                return (v1 == _base) ? v2 : v1;
            }
            public void EnforceRelation()
            {
                v1.EnforceRelation();
            }
            public void DiscardRelation()
            {
                if(relation != null)    relation.DiscardRelation();
            }
        }
        public class Polygon
        {
            public List<Vertex> vertices;
            public List<Edge> edges;
            public bool inBFS = false;

            public void SetVertexBrush(brushesColor _brush)
            {
                foreach (Vertex v in this.vertices)
                {
                    v.SetColor(_brush);
                }
            }
            public void SetEdgeBrush(brushesColor _brush)
            {
                foreach (Edge e in this.edges)
                {
                    e.SetColor(_brush);
                }
            }
            public void Move(int dx, int dy)
            {
                foreach (Vertex v in vertices)
                {
                    v.Move(dx, dy);
                }
            }
        }

        public static int DistanceFromEdgeSquared(Point p1, Point p2, Point p)
        {
            int A = p.X - p1.X;
            int B = p.Y - p1.Y;
            int C = p2.X - p1.X;
            int D = p2.Y - p1.Y;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = dot / len_sq;

            if (param <= 0 || param >= 1) return int.MaxValue;

            float xx = p1.X + param * C;
            float yy = p1.Y + param * D;
            int dx = p.X - (int)xx, dy = p.Y - (int)yy;
            return dx * dx + dy * dy;
        }

        public static int DistanceFromVertexSquared(Point p, Point v)
        {
            int dx = p.X - v.X;
            int dy = p.Y - v.Y;
            return dx * dx + dy * dy;
        }
    }
}
