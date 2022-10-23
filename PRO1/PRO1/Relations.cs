using System;
using System.Collections.Generic;
using System.Text;
using static PRO1.Entities;

namespace PRO1
{
    public interface IRelation
    {
        // interface is used so that an Edge can only have one Relation field,
        // but can take either a Fixed Length Relation or a Perpendicular Relation in that field
        public brushesColor Color();
        public void Enforce(Vertex movedOne);
        public void DiscardRelation();
    }
    public class RelationFixedLength : IRelation
    {
        // Try to keep an Edge the same length throughout the whole lifetime of the Relation

        public readonly Edge edge;
        public readonly int edgeLengthSquared;

        public RelationFixedLength(Edge _edge)
        {
            edge = _edge;
            edgeLengthSquared = DistanceFromVertexSquared(_edge.v1.point, _edge.v2.point);
            edge.color = Color();
        }
        public brushesColor Color()
        {
            return brushesColor.RelConstLen;
        }
        public void DiscardRelation()
        {
            edge.relation = null;
            edge.SetColor(brushesColor.normal);
        }
        public void Enforce(Vertex movedOne)
        {
            int dist = DistanceFromVertexSquared(edge.v1.point, edge.v2.point);
            if (dist == edgeLengthSquared) return;

            Vertex otherOne = (edge.v1 == movedOne) ? edge.v2 : edge.v1;

            double dx = movedOne.point.X - otherOne.point.X;
            double dy = movedOne.point.Y - otherOne.point.Y;
            double ratio = 1 - Math.Sqrt(Convert.ToDouble(edgeLengthSquared) / Convert.ToDouble(dist));
            dx = ratio * dx;
            dy = ratio * dy;

            otherOne.Move((int)dx, (int)dy);
        }
    }

    public class RelationPerpendicular : IRelation
    {
        // try to keep two Edges as perpendicular to each other as possible

        public readonly Edge edge1, edge2;
        public readonly int id;
        public static int count = 0;

        public RelationPerpendicular(Edge _edge1, Edge _edge2)
        {
            edge1 = _edge1;
            edge2 = _edge2;

            if (_edge2.v1 == _edge1.v2) (edge1, edge2) = (edge2, edge1);

            edge1.color = edge2.color = Color();
            id = ++count;
        }

        public brushesColor Color()
        {
            return brushesColor.RelPerpendicular;
        }

        public void DiscardRelation()
        {
            edge1.relation = null;
            edge2.relation = null;
            edge1.SetColor(brushesColor.normal);
            edge2.SetColor(brushesColor.normal);
        }

        public void Enforce(Vertex movedOne)
        {
            Edge main, toMove;

            if (movedOne == edge1.v1 || movedOne == edge1.v2)
            {
                main = edge1;
                toMove = edge2;
            }
            else
            {
                main = edge2;
                toMove = edge1;
            }

            Vertex v1 = toMove.v1;
            Vertex v2 = toMove.v2;
            if ((movedOne == edge2.v2 || movedOne == edge1.v2) && (edge1.v2 == edge2.v1 || edge2.v2 == edge1.v1))
            {
                (v1, v2) = (v2, v1);
            }

            double dx = main.v1.point.X - main.v2.point.X;
            double dy = main.v1.point.Y - main.v2.point.Y;

            v2.ChangePlacement(v1.point.X - (int)dy, v1.point.Y + (int)dx);

            // If Edges belong to different Polygons, Enforce Relations on the other Polygon as well
            if (toMove.parent != main.parent) toMove.EnforceRelation(toMove);
        }
    }
}
