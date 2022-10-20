using System;
using System.Collections.Generic;
using System.Text;
using static PRO1.Entities;

namespace PRO1
{
    public interface IRelation
    {
        public brushesColor Color();
        public void Enforce(Vertex movedOne);
    }

    public class RelationFixedLength : IRelation
    {
        public readonly Edge edge;
        public readonly int edgeLengthSquared;

        public void Enforce(Vertex movedOne)
        {
            //if (edge.v1 != movedOne && edge.v2 != movedOne) return;
            int dist = DistanceFromVertexSquared(edge.v1.point, edge.v2.point);
            if (dist == edgeLengthSquared) return;

            Vertex otherOne = (edge.v1 == movedOne) ? edge.v2 : edge.v1;

            double dx = movedOne.point.X - otherOne.point.X;
            double dy = movedOne.point.Y - otherOne.point.Y;
            double ratio = 1 - Convert.ToDouble(edgeLengthSquared) / dist;
            dx = ratio * dx;
            dy = ratio * dy;

            otherOne.Move((int)dx, (int)dy);
        }

        public brushesColor Color()
        {
            return brushesColor.RelConstLen;
        }

        public RelationFixedLength(Edge _edge)
        {
            edge = _edge;
            edgeLengthSquared = DistanceFromVertexSquared(_edge.v1.point, _edge.v2.point);
            edge.color = brushesColor.RelConstLen;
        }
    }
}
