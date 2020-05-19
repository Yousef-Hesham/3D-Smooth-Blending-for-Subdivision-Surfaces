using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace In_Lec
{
    class Sphere : _3D_Model
    {

        int iP = 0;

        int N = 30;
        int verticalSegments;
        int horizontalSegments;
        public float radius = 61;
        public float x, z = 1000;

        void BuildOuter()
        {
            verticalSegments = 20;
            horizontalSegments = N * 2;
          

            float incTheta = 180.0f / verticalSegments;
            float theta = 90;
            for (int k = 0; k < verticalSegments; k++)
            {
                float dx = (float)Math.Cos(theta * Math.PI / 180) * radius;
                float dy = (float)Math.Sin(theta * Math.PI / 180) * radius;

                _3D_Point v = new _3D_Point(dx, dy, z);
                AddPoint(v);
                iP++;

                float incTheta2 = 360 / horizontalSegments;
                float theta2 = 0;
                for (int h = 0; h < horizontalSegments; h++)
                {
                    x = (float)Math.Cos(theta2 * Math.PI / 180) * dx;
                    z = (float)Math.Sin(theta2 * Math.PI / 180) * dx;

                    v = new _3D_Point(x, (float)dy, z);
                    AddPoint(v);

                    if (h > 0)
                    {
                        AddEdge(iP, iP - 1, Color.Yellow);
                    }
                    theta2 += incTheta2;
                    iP++;
                }

                theta += incTheta;
            }
        }

        public void Design()
        {
            L_3D_Pts = new List<_3D_Point>();
            L_Edges = new List<Edge>();

            BuildOuter();
        }

    }
}
