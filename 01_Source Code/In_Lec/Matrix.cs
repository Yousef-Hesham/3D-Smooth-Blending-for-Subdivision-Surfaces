using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace In_Lec
{
    class Matrix
    {
        static public void Normalise(_3D_Point v)
        {
            float length;

            length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            v.X /= length;
            v.Y /= length;
            v.Z /= length;
        }

        static public _3D_Point CrossProduct(_3D_Point p1, _3D_Point p2)
        {
            _3D_Point p3;
            p3 = new _3D_Point(0, 0, 0);
            p3.X = p1.Y * p2.Z - p1.Z * p2.Y;
            p3.Y = p1.Z * p2.X - p1.X * p2.Z;
            p3.Z = p1.X * p2.Y - p1.Y * p2.X;
            return p3;
        }


    }
}
