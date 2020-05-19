using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace In_Lec
{
    class _3D_Model
    {
        public List<Face> Faces = new List<Face>();
        public List<_3D_Point> L_3D_Pts = new List<_3D_Point>();
        public List<Edge> L_Edges = new List<Edge>();
        public _3D_Point Centroid = new _3D_Point(0, 0, 0);
        public Camera cam;
        public int selectedFace = -1;
        public float Radius = 0;

        

        public void AddPoint(_3D_Point pnn)
        {
            L_3D_Pts.Add(pnn);
        }

        public void AddEdge(int i , int j , Color cl)
        {
            Edge pnn = new Edge(i, j);
            pnn.cl = cl;
            L_Edges.Add(pnn);
        }

        public void RotX(float th)
        {
            Transformation.RotatX(L_3D_Pts, th);
        }

        public void TransX(float tx)
        {
            Transformation.TranslateX2(L_3D_Pts , Faces, tx);
        }
        public void TransY(float ty)
        {
            Transformation.TranslateY2(L_3D_Pts, Faces, ty);
        }

        public void TransZ(float tz)
        {
            Transformation.TranslateZ2(L_3D_Pts, Faces , tz);
        }

        public void RotY(float th)
        {
            Transformation.RotatY(L_3D_Pts, th);
        }

        public void RotZ(float th)
        {
            Transformation.RotatZ(L_3D_Pts, th);
        }

        public bool isPointThere(_3D_Point P)
        {

            for (int i = 0; i < L_3D_Pts.Count(); i ++ )
            {
                if (P == L_3D_Pts[i])
                    return true;
            }

            for (int i = 0; i < L_3D_Pts.Count(); i++)
            {
                if ((int)P.X == (int)L_3D_Pts[i].X && (int)P.Y == (int)L_3D_Pts[i].Y && (int)P.Z == (int)L_3D_Pts[i].Z)
                    return true;
            }

            return false;
        }

        public bool isEdgeThere(Edge E)
        {
            for (int i = 0; i < L_Edges.Count(); i++)
            {
                if (E.i == L_Edges[i].i && E.j == L_Edges[i].j)
                    return true;
            }

            return false;
        }

        public int getEdge(Edge E)
        {
            for (int i = 0; i < L_Edges.Count(); i++)
            {
                if (E.i == L_Edges[i].i && E.j == L_Edges[i].j)
                    return i;
            }

            return 0;
        }

        public int getPoint (_3D_Point P)
        {
            for (int i = 0; i < L_3D_Pts.Count(); i++)
            {
                if (P == L_3D_Pts[i])
                    return i;
            }

            for (int i = 0; i < L_3D_Pts.Count(); i++)
            {
                if ((int)P.X == (int)L_3D_Pts[i].X && (int)P.Y == (int)L_3D_Pts[i].Y && (int)P.Z == (int)L_3D_Pts[i].Z)
                    return i;
            }
            return 0;
        }

        public void RotateAroundEdge(int iWhichEdge, float th)
        {
            _3D_Point p1 = new _3D_Point  (L_3D_Pts[L_Edges[iWhichEdge].i] );
            _3D_Point p2 = new _3D_Point  (L_3D_Pts[L_Edges[iWhichEdge].j] );
            Transformation.RotateArbitrary(L_3D_Pts, p1, p2, th);
        }

        public void DrawYourSelf(Graphics g)
        {
            Font FF = new Font("System", 9);
            for (int k = 0; k < L_Edges.Count; k++)
            {
                int i = L_Edges[k].i;
                int j = L_Edges[k].j;

                _3D_Point pi = new _3D_Point(L_3D_Pts[i]);
                _3D_Point pj = new _3D_Point(L_3D_Pts[j]);

                _3D_Point PE1 = new _3D_Point(L_Edges[k].edgePoint);
                _3D_Point PE2 = new _3D_Point(0,0,0);

                bool isVisible = cam.TransformToOrigin_And_Rotate_And_Project(pi, pj);
                bool isVisible2 = cam.TransformToOrigin_And_Rotate_And_Project(PE1, PE2);

                if (isVisible && L_Edges[k].visible )
                {
                    Pen Pn = new Pen(L_Edges[k].cl, 2);
                    g.DrawLine(Pn, pi.X, pi.Y, pj.X, pj.Y);
                    //draw edge number
                    //g.DrawString("E" + (k), FF, Brushes.Blue, (pi.X+pj.X)/2, (pi.Y+pj.Y)/2);


                    //drawing Point number
                    //g.DrawString("P" + (i), FF, Brushes.Green, pi.X, pi.Y);
                    //g.DrawString("P" + (j), FF, Brushes.Green, pj.X, pj.Y);

                    //drawing vertx points in red
                    if (L_3D_Pts[i].vertx)
                        g.FillEllipse(Brushes.Red, pi.X, pi.Y, 10, 10);
                
                    if(L_3D_Pts[j].vertx)
                        g.FillEllipse(Brushes.Red, pj.X, pj.Y, 10, 10);

                    //draw edge points
                    //g.FillEllipse(Brushes.Red, PE1.X, PE1.Y, 10, 10);
                }
     

            }
            //Draw faces in yellow
            if(selectedFace >= 0 && selectedFace < Faces.Count())
            {
                for (int k = 0; k < Faces[selectedFace].edges.Count(); k++)
                {

                    int i = L_Edges[Faces[selectedFace].edges[k]].i;
                    int j = L_Edges[Faces[selectedFace].edges[k]].j;

                    _3D_Point pi = new _3D_Point(L_3D_Pts[i]);
                    _3D_Point pj = new _3D_Point(L_3D_Pts[j]);


                    bool isVisible = cam.TransformToOrigin_And_Rotate_And_Project(pi, pj);
                    if (isVisible)
                    {
                        Pen Pn = new Pen(Color.Yellow, 5);
                        g.DrawLine(Pn, pi.X, pi.Y, pj.X, pj.Y);
                    }
                    g.DrawString("P" + (i), FF, Brushes.Green, pi.X, pi.Y);
                    g.DrawString("P" + (j), FF, Brushes.Green, pj.X, pj.Y);
                }
            }
            
            //draw blend region
            for(int p = 0 ; p < Faces.Count();p++)
            {
                if(Faces[p].BlendRegion == true)
                {
                    for (int k = 0; k < Faces[p].edges.Count(); k++)
                    {
                        

                        int i = L_Edges[Faces[p].edges[k]].i;
                        int j = L_Edges[Faces[p].edges[k]].j;

                        _3D_Point pi = new _3D_Point(L_3D_Pts[i]);
                        _3D_Point pj = new _3D_Point(L_3D_Pts[j]);


                        bool isVisible = cam.TransformToOrigin_And_Rotate_And_Project(pi, pj);
                        if (isVisible && L_Edges[Faces[p].edges[k]].visible)
                        {
                            Pen Pn = new Pen(Color.Crimson, 5);
                            g.DrawLine(Pn, pi.X, pi.Y, pj.X, pj.Y);
                        }
                    }
                }
            }


        }
    }
}
