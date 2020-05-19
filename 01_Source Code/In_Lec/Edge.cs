using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace In_Lec
{
    class Edge
    {
        public _3D_Point edgePoint = new _3D_Point(0, 0, 0);
        public int faceI, faceJ;
        public int i, j;
        public Color cl;
        public bool Ifilled = false;
        public bool visible = true;
        public bool blend = false;

        public Edge(int ii, int jj)
        {
            i = ii;
            j = jj;
        }
        
        public void setFaces(int ii, int jj)
        {
            faceI = ii;
            faceJ = jj;
        }
    }
}
