using System;
using System.Collections.Generic;
using System.Text;
using static PRO1.CGP1;

namespace PRO1
{
    interface Relation
    {
    }

    class FixedLengthRelation : Relation
    {
        public (Vertex v1, Vertex v2)? edge;
    }
}
