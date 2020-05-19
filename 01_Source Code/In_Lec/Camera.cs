using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace In_Lec
{
    class Camera
    {
        public _3D_Point cop;
        public _3D_Point lookAt;
        public _3D_Point up;
        public _3D_Point basisa, lookDir, basisc;
        public float focal = 0.5f;



        public int ceneterX, ceneterY;
        public int cxScreen, cyScreen;



        // -->
        public float tanH, tanV;
        public float front, back;

        public Camera()
        {
            cop = new _3D_Point(0, 0, -1250);        // new Point3D(0, -50, 0);
            lookAt = new _3D_Point(0, 0, 1);       //new Point3D(0, 50, 0);
            up = new _3D_Point(0, 1, 0);
            

            front = 10; // 70.0;
            back = 5200.0f;            
            tanH = (float)(Math.Tan(45 / 2 * Math.PI / 180));
            tanV = (float)(Math.Tan(45 / 2 * Math.PI / 180));
        }

        public void BuildNewSystem()
        {
            lookDir = new _3D_Point(0, 0, 0);
            basisa = new _3D_Point(0, 0, 0);
            basisc = new _3D_Point(0, 0, 0);

            lookDir.X = lookAt.X - cop.X;
            lookDir.Y = lookAt.Y - cop.Y;
            lookDir.Z = lookAt.Z - cop.Z;
            Matrix.Normalise(lookDir);

            basisa = Matrix.CrossProduct(up, lookDir);
            Matrix.Normalise(basisa);

            basisc = Matrix.CrossProduct(lookDir, basisa);
            Matrix.Normalise(basisc);
        }

        public void TransformToOrigin_And_Rotate(_3D_Point a, _3D_Point e)
        {
            _3D_Point w = new _3D_Point(a.X, a.Y, a.Z);
            w.X -= cop.X;
            w.Y -= cop.Y;
            w.Z -= cop.Z;

            e.X = w.X * basisa.X + w.Y * basisa.Y + w.Z * basisa.Z;
            e.Y = w.X * basisc.X + w.Y * basisc.Y + w.Z * basisc.Z;
            e.Z = w.X * lookDir.X + w.Y * lookDir.Y + w.Z * lookDir.Z;            
        }

        public bool ClipAgainstZ(_3D_Point a1, _3D_Point a2)
        {
            float t;

            if ((a1.Z <= front && a2.Z <= front)
                || (a1.Z >= back && a2.Z >= back)
               )
            {
                return false;
            }



            if ((a1.Z < front && a2.Z > front) ||
                (a1.Z > front && a2.Z < front))
            {

                t = (front - a1.Z) / (a2.Z - a1.Z);
                if (a1.Z < front)
                {
                    a1.X = a1.X + t * (a2.X - a1.X);
                    a1.Y = a1.Y + t * (a2.Y - a1.Y);
                    a1.Z = front;
                }
                else
                {
                    a2.X = a1.X + t * (a2.X - a1.X);
                    a2.Y = a1.Y + t * (a2.Y - a1.Y);
                    a2.Z = front;
                }
            }

            if ((a1.Z < back && a2.Z > back) ||
                (a1.Z > back && a2.Z < back))
            {
                t = (back - a1.Z) / (a2.Z - a1.Z);
                if (a1.Z < back)
                {
                    a2.X = a1.X + t * (a2.X - a1.X);
                    a2.Y = a1.Y + t * (a2.Y - a1.Y);
                    a2.Z = back;
                }
                else
                {
                    a1.X = a1.X + t * (a2.X - a1.X);
                    a1.Y = a1.Y + t * (a2.Y - a1.Y);
                    a1.Z = back;
                }
            }

            return true;
        }

        public bool ClipAgainst_X_and_Y(_3D_Point p1, _3D_Point p2)
        {
            float t;

            if ((p1.X >= 1 && p2.X >= 1)
                || (p1.X <= -1 && p2.X <= -1)
                )
            {
                return false;
            }


            if ((p1.X > 1 && p2.X < 1) || (p1.X < 1 && p2.X > 1))
            {
                t = (1 - p1.X) / (p2.X - p1.X);
                if (p1.X < 1)
                {
                    p2.Y = p1.Y + t * (p2.Y - p1.Y);
                    p2.X = 1;
                }
                else
                {
                    p1.Y = p1.Y + t * (p2.Y - p1.Y);
                    p1.X = 1;
                }
            }


            if ((p1.X < -1 && p2.X > -1) || (p1.X > -1 && p2.X < -1))
            {

                t = (-1 - p1.X) / (p2.X - p1.X);
                if (p1.X > -1)
                {
                    p2.Y = p1.Y + t * (p2.Y - p1.Y);
                    p2.X = -1;
                }
                else
                {
                    p1.Y = p1.Y + t * (p2.Y - p1.Y);
                    p1.X = -1;
                }
            }

            if ((p1.Y >= 1 && p2.Y >= 1)
                || (p1.Y <= -1 && p2.Y <= -1))
                return false;

            if ((p1.Y > 1 && p2.Y < 1) || (p1.Y < 1 && p2.Y > 1))
            {

                t = (1 - p1.Y) / (p2.Y - p1.Y);
                if (p1.Y < 1)
                {
                    p2.X = p1.X + t * (p2.X - p1.X);
                    p2.Y = 1;
                }
                else
                {
                    p1.X = p1.X + t * (p2.X - p1.X);
                    p1.Y = 1;
                }
            }

            if ((p1.Y < -1 && p2.Y > -1) || (p1.Y > -1 && p2.Y < -1))
            {
                t = (-1 - p1.Y) / (p2.Y - p1.Y);
                if (p1.Y > -1)
                {
                    p2.X = p1.X + t * (p2.X - p1.X);
                    p2.Y = -1;
                }
                else
                {
                    p1.X = p1.X + t * (p2.X - p1.X);
                    p1.Y = -1;
                }
            }

            return (true);
        }

        public void NormalizeFov(_3D_Point p)
        {
            p.X /= tanH;
            p.Y /= tanV;
        }

        public bool TransformToOrigin_And_Rotate_And_Project(_3D_Point w1, _3D_Point w2)
        {
            _3D_Point  e1 = new _3D_Point(0, 0, 0);            
            TransformToOrigin_And_Rotate(w1, e1);

            _3D_Point e2 = new _3D_Point(0, 0, 0);
            TransformToOrigin_And_Rotate(w2, e2);

            if (!ClipAgainstZ(e1, e2))
                return false;

            _3D_Point n1 = new _3D_Point(0, 0, 0);
            n1.X = focal * e1.X / e1.Z;
            n1.Y = focal * e1.Y / e1.Z;
            n1.Z = focal;

            _3D_Point n2 = new _3D_Point(0, 0, 0);
            n2.X = focal * e2.X / e2.Z;
            n2.Y = focal * e2.Y / e2.Z;
            n2.Z = focal;

            NormalizeFov(n1);
            NormalizeFov(n2);

            if (!ClipAgainst_X_and_Y(n1, n2))
                return false;

            // view mapping
            n1.X = (int)(ceneterX + cxScreen * n1.X / 2);
            n1.Y = (int)(ceneterY - cyScreen * n1.Y / 2);

            n2.X = (int)(ceneterX + cxScreen * n2.X / 2);
            n2.Y = (int)(ceneterY - cyScreen * n2.Y / 2);



            w1.X = n1.X;
            w1.Y = n1.Y;

            w2.X = n2.X;
            w2.Y = n2.Y;
            return true;
        }


    }
}
