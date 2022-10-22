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
        public void DiscardRelation();
    }
    public class RelationFixedLength : IRelation
    {
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
        public readonly Edge edge1, edge2;
        public readonly int id;
        public static int count = 0;

        public RelationPerpendicular(Edge _edge1, Edge _edge2)
        {
            edge1 = _edge1;
            edge2 = _edge2;
            edge1.color = edge2.color = Color();
            id = count;
            count++;
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

            double dx = main.v1.point.X - main.v2.point.X;
            double dy = main.v1.point.Y - main.v2.point.Y;
            double mainLen = Math.Sqrt((double)DistanceFromVertexSquared(main.v1.point, main.v2.point));
            //double tangent = dy / dx;
            double otherLen = Math.Sqrt((double)DistanceFromVertexSquared(toMove.v1.point, toMove.v2.point));
            dx = 100 * dx / mainLen;
            dy = 100 * dy / mainLen;

            toMove.v1.ChangePlacement(toMove.v2.point.X + (int)dx, toMove.v2.point.Y + (int)dy);

            //double newAngle;
            //double newX, newY;
            //Vertex v1, v2;

            //if (Math.Abs(dy) < 0.001d || Math.Abs(dx) < 0.001d)
            //{
            //    //if (dy <= 0)
            //    //{
            //    //    v1 = toMove.v1; v2 = toMove.v2;
            //    //    newY = v1.point.Y - Math.Sqrt((double)edgeLengthSquared);
            //    //}
            //    //else
            //    //{
            //    //    v1 = toMove.v1; v2 = toMove.v2;
            //    //    newY = v1.point.Y + Math.Sqrt((double)edgeLengthSquared);                    
            //    //}

            //    v1 = toMove.v1; v2 = toMove.v2;
            //    newX = (double)v2.point.X;
            //    newY = (double)v2.point.Y;

            //    //if (Math.Abs(v2.point.Y - (v1.point.Y + Math.Sqrt((double)edgeLengthSquared))) < Math.Abs(v2.point.Y - (v1.point.Y - Math.Sqrt((double)edgeLengthSquared))))
            //    //{
            //    //    v1 = toMove.v1; v2 = toMove.v2;
            //    //    newX = v2.point.X;
            //    //    newY = v1.point.Y;
            //    //}
            //    //else
            //    //{
            //    //    v1 = toMove.v2; v2 = toMove.v1;
            //    //    newX = v2.point.X;
            //    //    newY = v2.point.Y;
            //    //}

            //}
            //else
            //{
            //    newAngle = -dx / dy;

            //    //

            //    v1 = toMove.v1; v2 = toMove.v2;

            //    //if (newAngle <= 0)
            //    //{
            //    //    v1 = toMove.v2; v2 = toMove.v1;
            //    //}
            //    //else
            //    //{
            //    //    v1 = toMove.v1; v2 = toMove.v2;
            //    //}

            //    newX = v1.point.X + Math.Sqrt(edgeLengthSquared) * (1 / newAngle);
            //    newY = v1.point.Y + Math.Sqrt(edgeLengthSquared) * newAngle;
            //}
            //toMove.v2.EnforceRelation();


            //v2.ChangePlacement((int)newX, (int)newY);
            //int dist = DistanceFromVertexSquared(toMove.v1.point, toMove.v2.point);

            //dx = v1.point.X - v2.point.X;
            //dy = v1.point.Y - v2.point.Y;

            //double ratio = 1 - Math.Sqrt(Convert.ToDouble(edgeLengthSquared) / Convert.ToDouble(dist));
            //dx = ratio * dx;
            //dy = ratio * dy;

            //v2.Move((int)dx, (int)dy);
        }
    }
}
