using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace In_Lec
{
    class Face
    {
        public _3D_Point facePoint = new _3D_Point(0,0,0);

        public List<int> points = new List<int>();

        public List<int> edges = new List<int>();

        public float Radius = 0;

        public bool BlendRegion = false;

        public bool isEdgeThere(Edge E, SubDivison I)
        {
            for (int i = 0; i < edges.Count(); i++)
            {
                if (E == I.L_Edges[edges[i]])
                    return true;
            }

            return false;
        }
    }
}
