using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace In_Lec
{
    /*
     * A,D --> SELECT OBJECT
     * S --> SUBDIVIDE Selected Object
     * 
     * CAM:
     * Y --> UP
     * H --> DOWN
     * LEFT --> LEFT
     * RIGHT --> RIGHT
     * UP --> ZOOM IN
     * DOWN --> ZOOM OUT
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * */
    public partial class Form1 : Form
    {
        Bitmap off;

        List<SubDivison> Objects = new List<SubDivison>();
        Sphere S = new Sphere();
        int selectedObject = 0;
        SubDivison Tri = new SubDivison();
        SubDivison Normals = new SubDivison();

        List<int> BoundryPointsI1 = new List<int>();
        List<int> BoundryPointsI2 = new List<int>();

        List<int> Paired1 = new List<int>();
        List<int> Paired2 = new List<int>();

        Camera cam = new Camera();
        double cs, sn, zn, xn, yn;
        public bool addValue;


        List<float> Weights = new List<float>();

        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.Load += new EventHandler(Form1_Load);
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
   
        }

        void CamRight()
        {
            for(int i=0 ; i < 3 ; i++)
            {
                zn = cam.cop.Z * cs - cam.cop.X * sn;
                xn = cam.cop.Z * sn + cam.cop.X * cs;
                cam.cop.X = (float)xn;
                cam.cop.Z = (float)zn;
                cam.BuildNewSystem();
            }
        }
        void CamLeft()
        {
            for (int i = 0; i < 3; i++) 
            {
                zn = cam.cop.Z * cs - cam.cop.X * -sn;
                xn = cam.cop.Z * -sn + cam.cop.X * cs;
                cam.cop.X = (float)xn;
                cam.cop.Z = (float)zn;
                cam.BuildNewSystem();
            }
        }
        void CamUp()
        {
            for (int i = 0; i < 3; i++)
            {
                yn = cam.cop.Y * cs - cam.cop.Z * sn;
                zn = cam.cop.Y * sn + cam.cop.Z * cs;
                cam.cop.Y = (float)yn;
                cam.cop.Z = (float)zn;
                cam.BuildNewSystem();
            }
        }
        void CamDown()
        {
            for (int i = 0; i < 3; i++)
            {
                yn = cam.cop.Y * cs - cam.cop.Z * -sn;
                zn = cam.cop.Y * -sn + cam.cop.Z * cs;
                cam.cop.Y = (float)yn;
                cam.cop.Z = (float)zn;
                cam.BuildNewSystem();
            }
        }
        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            double dx, dy, dz;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    dx = cam.lookAt.X - cam.cop.X;
                    dy = cam.lookAt.Y - cam.cop.Y;
                    dz = cam.lookAt.Z - cam.cop.Z;
                    cam.cop.X += (float)dx * 0.03f;
                    cam.cop.Y += (float)dy * 0.03f;
                    cam.cop.Z += (float)dz * 0.03f;             
                    break;

                case Keys.Down:
                    dx = cam.lookAt.X - cam.cop.X;
                    dy = cam.lookAt.Y - cam.cop.Y;
                    dz = cam.lookAt.Z - cam.cop.Z;
                    cam.cop.X -= (float)dx * 0.03f;
                    cam.cop.Y -= (float)dy * 0.03f;
                    cam.cop.Z -= (float)dz * 0.03f;
                    break;

                case Keys.Right:
                    CamRight();
                    break;

                case Keys.Left:
                    CamLeft();
                    break;

                case Keys.Y:
                    CamUp();
                    break;

                case Keys.H:
                    CamDown();
                    break;


                case Keys.L: //move selected object right
                    Objects[selectedObject].TransX(10);
                    calcObjectCentroid(Objects[selectedObject]);
                    break;

                case Keys.K: //move selected object left
                    Objects[selectedObject].TransX(-10);
                    calcObjectCentroid(Objects[selectedObject]);
                    break;

                case Keys.I: //move selected object TOP
                    Objects[selectedObject].TransY(10);
                    calcObjectCentroid(Objects[selectedObject]);
                    break;

                case Keys.O: //move selected object TOP
                    Objects[selectedObject].TransY(-10);
                    calcObjectCentroid(Objects[selectedObject]);
                    break;

                case Keys.A:
                    if (selectedObject > 0)
                        selectedObject--;
                    this.Text = "" + (selectedObject+1);
                    break;

                case Keys.D:
                    if (selectedObject < Objects.Count() - 1)
                    {
                        selectedObject++;
                    }
                    this.Text = "" + (selectedObject+1);
                    break;

                case Keys.S: //sub-divide 
                    SubDivideObject(Objects[selectedObject]);
                    calculateRadius();
                    this.Text = "" + Objects[selectedObject].Faces.Count();
                    break;

                case Keys.Z: //Locate blend region
                    LocateBlendRegion(Objects[0], Objects[1]);
                    break;

                case Keys.X: //remove blend region
                    RemoveBlendRegion(Objects[0], Objects[1]);
                    break;

                case Keys.C: //locate boundry points for both objects, and smooth the boundries 
                    BoundrySmoothing(Objects[0], Objects[1]);
                    break;

                case Keys.V: //Pairing & Blending
                    Pairing(Objects[0], Objects[1], BoundryPointsI1, BoundryPointsI2, 3);
         
                    Blend2Objects(Objects[0], Objects[1]);
                    break;

            }
            cam.BuildNewSystem();

            DrawDubble(this.CreateGraphics());
        }

        void Blend2Objects(SubDivison I1, SubDivison I2)
        {
            List<_3D_Point> LL = new List<_3D_Point>();

            //W(A)
            //float[] F = new float[6] { 0.5f, 0.5f, 0, 0, 0, 0};
            //W(B)
            //float[] F = new float[6] { 0.3f, 0.3f, 0.1f, 0.1f, 0.1f, 0.1f };
            //W(C)
            //float[] F = new float[6] { 0.4f, 0.4f, 0.05f, 0.05f, 0.05f, 0.05f };
            //W(D)
            //float[] F = new float[6] { 0.1666f, 0.1666f, 0.1666f, 0.1666f, 0.1666f, 0.1666f }; //d is goooood
            //W(E)
            //float[] F = new float[6] { 0.2f, 0.2f, 0.15f, 0.15f, 0.15f, 0.15f };
            //W(F)
            float[] F = new float[6] { 0.1f, 0.1f, 0.2f, 0.2f, 0.2f, 0.2f };



            for (int i = 0; i < Paired1.Count() && i < Paired2.Count(); i++)
            {
                _3D_Point P = new _3D_Point(0, 0, 0);
                I1.L_3D_Pts[Paired1[i]].Weight = F[0];
                I2.L_3D_Pts[Paired2[i]].Weight = F[1];
                LL.Add(I1.L_3D_Pts[Paired1[i]]);
                LL.Add(I2.L_3D_Pts[Paired2[i]]);


                //paired1
                for (int j = 0; j < I1.L_3D_Pts[Paired1[i]].edges.Count(); j++)
                {

                    if (I1.L_Edges[I1.L_3D_Pts[Paired1[i]].edges[j]].blend == true && I1.L_Edges[I1.L_3D_Pts[Paired1[i]].edges[j]].visible == true)
                    {

                        if (I1.L_Edges[I1.L_3D_Pts[Paired1[i]].edges[j]].i != Paired1[i])
                        {
     
                            I1.L_3D_Pts[I1.L_Edges[I1.L_3D_Pts[Paired1[i]].edges[j]].i].Weight = F[2];
                            LL.Add(I1.L_3D_Pts[I1.L_Edges[I1.L_3D_Pts[Paired1[i]].edges[j]].i]);
                        }
                        if (I1.L_Edges[I1.L_3D_Pts[Paired1[i]].edges[j]].j != Paired1[i])
                        {

                            I1.L_3D_Pts[I1.L_Edges[I1.L_3D_Pts[Paired1[i]].edges[j]].j].Weight = F[3];
                            LL.Add(I1.L_3D_Pts[I1.L_Edges[I1.L_3D_Pts[Paired1[i]].edges[j]].j]);
                        }
                    }
                }
                //paired2
                for (int j = 0; j < I2.L_3D_Pts[Paired2[i]].edges.Count(); j++)
                {

                    if (I2.L_Edges[I2.L_3D_Pts[Paired2[i]].edges[j]].blend == true && I2.L_Edges[I2.L_3D_Pts[Paired2[i]].edges[j]].visible == true)
                    {

                        if (I2.L_Edges[I2.L_3D_Pts[Paired2[i]].edges[j]].i != Paired2[i])
                        {

                            I2.L_3D_Pts[I2.L_Edges[I2.L_3D_Pts[Paired2[i]].edges[j]].i].Weight = F[4];
                            LL.Add(I2.L_3D_Pts[I2.L_Edges[I2.L_3D_Pts[Paired2[i]].edges[j]].i]);
                        }
                        if (I2.L_Edges[I2.L_3D_Pts[Paired2[i]].edges[j]].j != Paired2[i])
                        {

                            I2.L_3D_Pts[I2.L_Edges[I2.L_3D_Pts[Paired2[i]].edges[j]].j].Weight = F[5];
                            LL.Add(I2.L_3D_Pts[I2.L_Edges[I2.L_3D_Pts[Paired2[i]].edges[j]].j]);
                        }
                    }
                }


                P = CreatePointFromListOfPoints(LL);
                Normals.AddPoint(P);
                Normals.AddPoint(I1.L_3D_Pts[Paired1[i]]);

                Normals.AddEdge(Normals.L_3D_Pts.Count() - 2, Normals.L_3D_Pts.Count() - 1, Color.Black);
                Normals.L_Edges[Normals.L_Edges.Count() - 1].i = Normals.L_3D_Pts.Count() - 2;
                Normals.L_Edges[Normals.L_Edges.Count() - 1].j = Normals.L_3D_Pts.Count() - 1;

                int pos = Normals.getPoint(P);
                Normals.AddPoint(I2.L_3D_Pts[Paired2[i]]);

                Normals.AddEdge(pos, Normals.L_3D_Pts.Count() - 1, Color.Black);
                Normals.L_Edges[Normals.L_Edges.Count() - 1].i = pos;
                Normals.L_Edges[Normals.L_Edges.Count() - 1].j = Normals.L_3D_Pts.Count() - 1;

                LL.Clear();
            }


            //draw connections between curve points 

            List<int> Ps = new List<int>();

            for(int i = 0 ; i < Normals.L_3D_Pts.Count(); i += 3)
            {
                Ps.Clear();
                float min = 9999999;
                int pos = 0;
                //loacte points to draw to, from current point
                for (int j = 0; j < Normals.L_3D_Pts.Count(); j += 3)
                {
                    if (EuclideanDistance(Normals.L_3D_Pts[i], Normals.L_3D_Pts[j]) == min && i != j)
                    {
                        Ps.Add(j);
                    }

                    if (EuclideanDistance(Normals.L_3D_Pts[i], Normals.L_3D_Pts[j]) < min && i != j)
                    {
                        Ps.Clear();
                        Ps.Add(j);
                        pos = j;
                        min = EuclideanDistance(Normals.L_3D_Pts[i], Normals.L_3D_Pts[j]);
                    }
                }

                //
                min = 9999;
                int pos2=0;
                for (int j = 0; j < Normals.L_3D_Pts.Count(); j += 3)
                {
                    if (EuclideanDistance(Normals.L_3D_Pts[i], Normals.L_3D_Pts[j]) < min && i != j && j != pos)
                    {
                        pos2 = j;   
                        min = EuclideanDistance(Normals.L_3D_Pts[i], Normals.L_3D_Pts[j]);
                    }
                }
                Ps.Add(pos2);
                //

                //draw
                for(int j = 0 ; j < Ps.Count() ; j++)
                {
                    if(!Normals.isEdgeThere(new Edge(i, Ps[j])))
                    {
                        Normals.AddEdge(i, Ps[j], Color.Black);
                        Normals.L_Edges[Normals.L_Edges.Count() - 1].i = i;
                        Normals.L_Edges[Normals.L_Edges.Count() - 1].j = Ps[j];
                    }
             
                }

            }
        
        }

        _3D_Point CreatePointFromListOfPoints(List<_3D_Point> L)
        { 
            _3D_Point P = new _3D_Point(0, 0, 0);
            for (int i = 0; i < L.Count; i++)
            {
                P.X += L[i].X * L[i].Weight;
                P.Y += L[i].Y * L[i].Weight;
                P.Z += L[i].Z * L[i].Weight;
            }
            return P;
        }

        void Pairing(SubDivison I1, SubDivison I2, List<int> BoundryPointsI1, List<int> BoundryPointsI2, int Iterations)
        {
            for(int IT = 0 ; IT < Iterations ; IT++)
            {
                List<int> Match1 = new List<int>();
                List<int> Match2 = new List<int>();

                for (int i = 0; i < BoundryPointsI1.Count(); i++)
                {
                    float min = 9999999;
                    int pos = 0;
                    for (int k = 0; k < BoundryPointsI2.Count(); k++)
                    {
                        if (min > EuclideanDistance(I1.L_3D_Pts[BoundryPointsI1[i]], I2.L_3D_Pts[BoundryPointsI2[k]]))
                        {
                            min = EuclideanDistance(I1.L_3D_Pts[BoundryPointsI1[i]], I2.L_3D_Pts[BoundryPointsI2[k]]);
                            pos = BoundryPointsI2[k];
                        }
                    }

                    Match1.Add(pos);
                }

                for (int i = 0; i < BoundryPointsI2.Count(); i++)
                {
                    float min = 9999999;
                    int pos = 0;
                    for (int k = 0; k < BoundryPointsI1.Count(); k++)
                    {
                        if (min > EuclideanDistance(I2.L_3D_Pts[BoundryPointsI2[i]], I1.L_3D_Pts[BoundryPointsI1[k]]))
                        {
                            min = EuclideanDistance(I2.L_3D_Pts[BoundryPointsI2[i]], I1.L_3D_Pts[BoundryPointsI1[k]]);
                            pos = BoundryPointsI1[k];
                        }
                    }

                    Match2.Add(pos);
                }

                for (int i = 0; i < BoundryPointsI1.Count(); i++)
                {

                    for (int k = 0; k < Match2.Count(); k++)
                    {
                        if (BoundryPointsI1[i] == Match2[k] && Match1[i] == BoundryPointsI2[k])
                        {
                            Paired1.Add(BoundryPointsI1[i]);
                            Paired2.Add(BoundryPointsI2[k]);
                        }
                    }
                }


                for (int i = 0; i < Paired1.Count(); i++)
                {
                    BoundryPointsI1.Remove(Paired1[i]);
                    BoundryPointsI2.Remove(Paired2[i]);
                }
            }
        }
        void drawNormalsOfFaces()
        {
            _3D_Point N = new _3D_Point(0, 0, 0);
   

            //for(int i = 0; i < Objects.Count() -1; i ++)
            //{
                for(int k = 0 ; k < Objects[0].Faces.Count() ; k++)
                {

                    N = CalcFaceNormal(Objects[0], Objects[0].Faces[k] , k);

                    Normals.AddPoint( new _3D_Point(Objects[0].Faces[k].facePoint.X, Objects[0].Faces[k].facePoint.Y, Objects[0].Faces[k].facePoint.Z));
                    Normals.AddPoint(new _3D_Point(Objects[0].Faces[k].facePoint.X + (N.X * 100), Objects[0].Faces[k].facePoint.Y + (N.Y * 100), Objects[0].Faces[k].facePoint.Z + (N.Z * 100)));
                    Normals.AddEdge(Normals.L_3D_Pts.Count() - 2, Normals.L_3D_Pts.Count() - 1, Color.Yellow);
                    Normals.L_Edges[Normals.L_Edges.Count() - 1].i = Normals.L_3D_Pts.Count() - 2;
                    Normals.L_Edges[Normals.L_Edges.Count() - 1].j = Normals.L_3D_Pts.Count() - 1;
                }
            //}

        }

        void RemoveBlendRegion(SubDivison I1, SubDivison I2)
        {
          
            for (int i = 0; i < I1.Faces.Count; i++)
            {
                if (I1.Faces[i].BlendRegion)
                {
                    //loop edges of removed face & put the i face = -1 
                    for (int k = 0; k < I1.Faces[i].edges.Count(); k++)
                    {
                        if (I1.L_Edges[I1.Faces[i].edges[k]].faceI == i)
                        {
                            I1.L_Edges[I1.Faces[i].edges[k]].faceI = -1;
                        }
                        if (I1.L_Edges[I1.Faces[i].edges[k]].faceJ == i)
                        {
                            I1.L_Edges[I1.Faces[i].edges[k]].faceJ = -1;
                        }

                        if (I1.L_Edges[I1.Faces[i].edges[k]].faceI == -1 && I1.L_Edges[I1.Faces[i].edges[k]].faceJ == -1)
                        {
                            I1.L_Edges[I1.Faces[i].edges[k]].visible = false;
                        }

                    }
                }
            }

            for (int i = 0; i < I2.Faces.Count; i++)
            {
                if (I2.Faces[i].BlendRegion)
                {
                    //loop edges of removed face & put the i face = -1 
                    for (int k = 0; k < I2.Faces[i].edges.Count(); k++)
                    {
                        if (I2.L_Edges[I2.Faces[i].edges[k]].faceI == i)
                        {
                            I2.L_Edges[I2.Faces[i].edges[k]].faceI = -1;
                        }
                        if (I2.L_Edges[I2.Faces[i].edges[k]].faceJ == i)
                        {
                            I2.L_Edges[I2.Faces[i].edges[k]].faceJ = -1;
                        }

                        if (I2.L_Edges[I2.Faces[i].edges[k]].faceI == -1 && I2.L_Edges[I2.Faces[i].edges[k]].faceJ == -1)
                        {
                            I2.L_Edges[I2.Faces[i].edges[k]].visible = false;
                        }

                    }
                }
            }


        }

        List<int> LocateBoundryPoints(SubDivison I1)
        {
            List<int> Points = new List<int>();

            for(int i = 0 ; i < I1.L_Edges.Count(); i++)
            {
                bool Pi = false, Pj = false;

                //if (I1.Faces[I1.L_Edges[i].faceI].BlendRegion == true && I1.Faces[I1.L_Edges[i].faceJ].BlendRegion == false || I1.Faces[I1.L_Edges[i].faceJ].BlendRegion == true && I1.Faces[I1.L_Edges[i].faceI].BlendRegion == false)
                if (I1.L_Edges[i].faceI == -1 && I1.L_Edges[i].faceJ != -1 || I1.L_Edges[i].faceJ == -1 && I1.L_Edges[i].faceI != -1)
                {
                    for(int j = 0 ; j < Points.Count(); j++)
                    {
                        if (I1.L_Edges[i].i == Points[j])
                            Pi = true;
                        if (I1.L_Edges[i].j == Points[j])
                            Pj = true;
                    }

                    if (!Pi)
                    {
                        Points.Add(I1.L_Edges[i].i);
                        //I1.L_3D_Pts[I1.L_Edges[i].i].vertx = true;
                    }
                        
                    if (!Pj)
                    {
                        Points.Add(I1.L_Edges[i].j);
                        //I1.L_3D_Pts[I1.L_Edges[i].j].vertx = true;
                    }
                        
                }
            }

            return Points;
        }

        List<int> LocateBoundryEdges(SubDivison I1)
        {
            List<int> Edges = new List<int>();

            for (int i = 0; i < I1.L_Edges.Count(); i++)
            {
                if (I1.L_Edges[i].faceI == -1 && I1.L_Edges[i].faceJ != -1 || I1.L_Edges[i].faceJ == -1 && I1.L_Edges[i].faceI != -1)
                {
                    Edges.Add(i);
                }
            }

            return Edges;
        }

        _3D_Point calcCrossProduct(_3D_Point V1, _3D_Point V2)
        {
            _3D_Point CrossProduct = new _3D_Point(0, 0, 0);

            CrossProduct.X = (V1.Y * V2.Z) - (V1.Z * V2.Y);
            CrossProduct.Y = (V1.Z * V2.X) - (V1.X * V2.Z);
            CrossProduct.Z = (V1.X * V2.Y) - (V1.Y * V2.X);

            return CrossProduct;
        }


        _3D_Point CalcFaceNormal(SubDivison I, Face F, int kk)
        {
            int P0 = F.points[0], P1 = 0, P2 = 0;
            int Flag = 0;

            for (int i = 0 ; i < F.edges.Count(); i++)
            {
                if (I.L_Edges[F.edges[i]].i == P0)
                {
                    if (Flag == 0)
                        P1 = I.L_Edges[F.edges[i]].j;
                    else
                        P2 = I.L_Edges[F.edges[i]].j; 

                    Flag++;
                }

                if (I.L_Edges[F.edges[i]].j == P0)
                {
                    if (Flag == 0)
                        P1 = I.L_Edges[F.edges[i]].i;
                    else
                        P2 = I.L_Edges[F.edges[i]].i;

                    Flag++;
                }
            }

            //now we have p0, p1 ,p2

            _3D_Point V1 = new _3D_Point(0, 0, 0);
            _3D_Point V2 = new _3D_Point(0, 0, 0);
            _3D_Point CrossProduct = new _3D_Point(0, 0, 0);
            _3D_Point NORMAL = new _3D_Point(0, 0, 0);

            
            V1.X = I.L_3D_Pts[P1].X - I.L_3D_Pts[P0].X;
            V1.Y = I.L_3D_Pts[P1].Y - I.L_3D_Pts[P0].Y;
            V1.Z = I.L_3D_Pts[P1].Z - I.L_3D_Pts[P0].Z;

            V2.X = I.L_3D_Pts[P2].X - I.L_3D_Pts[P0].X;
            V2.Y = I.L_3D_Pts[P2].Y - I.L_3D_Pts[P0].Y;
            V2.Z = I.L_3D_Pts[P2].Z - I.L_3D_Pts[P0].Z;
            

            CrossProduct = calcCrossProduct(V1, V2);

            NORMAL = NormalizeNormal(CrossProduct);

            float Len = Magn(NORMAL);
            //this.Text += ">> " + kk + " " + Len;

            _3D_Point Other = new _3D_Point(F.facePoint.X + (NORMAL.X * 100), F.facePoint.Y + (NORMAL.Y * 100), F.facePoint.Z + (NORMAL.Z * 100));
            if(EuclideanDistance(I.Centroid, Other) > EuclideanDistance(I.Centroid, F.facePoint) )
            {
                V1.X = I.L_3D_Pts[P2].X - I.L_3D_Pts[P0].X;
                V1.Y = I.L_3D_Pts[P2].Y - I.L_3D_Pts[P0].Y;
                V1.Z = I.L_3D_Pts[P2].Z - I.L_3D_Pts[P0].Z;

                V2.X = I.L_3D_Pts[P1].X - I.L_3D_Pts[P0].X;
                V2.Y = I.L_3D_Pts[P1].Y - I.L_3D_Pts[P0].Y;
                V2.Z = I.L_3D_Pts[P1].Z - I.L_3D_Pts[P0].Z;
                CrossProduct = calcCrossProduct(V1, V2);
                NORMAL = NormalizeNormal(CrossProduct);
            }

            return NORMAL;
        }

        float Magn(_3D_Point N)
        {
            float Mag = (float)Math.Sqrt(Math.Pow(N.X, 2) + Math.Pow(N.Y, 2) + Math.Pow(N.Z, 2));
            return Mag;
        }
        _3D_Point NormalizeNormal(_3D_Point N)
        {

            float Mag = (float)Math.Sqrt(Math.Pow(N.X, 2) + Math.Pow(N.Y, 2) + Math.Pow(N.Z, 2));

            _3D_Point Nor = new _3D_Point(N.X / Mag, N.Y / Mag, N.Z / Mag);

            return Nor;
        }

        float DotProduct( _3D_Point P1,  _3D_Point P2)
        {
            float Dot = (P1.X * P2.X) + (P1.Y * P2.Y) + (P1.Z * P2.Z);
            return Dot;
        }

        void IterationOfSmoothing(SubDivison I1, List<int> BoundryPointsI1)
        {
            List<_3D_Point> NewPoints1 = new List<_3D_Point>();


            for (int G = 0; G < BoundryPointsI1.Count(); G++)
            {
                _3D_Point Ptmp = new _3D_Point(0, 0, 0);
                _3D_Point Diff = new _3D_Point(0, 0, 0);
                _3D_Point Pnew = new _3D_Point(0, 0, 0);

                //this.Text += " Original " + BoundryPointsI1[G] + " :";

                for (int i = 0; i < I1.L_3D_Pts[BoundryPointsI1[G]].edges.Count(); i++)
                {

                    if (I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].blend == true && I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].visible == true)
                    {
                    
                        if (I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].i != BoundryPointsI1[G])
                        {
                            //this.Text += " `" + I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].i;
                            Ptmp.X += I1.L_3D_Pts[I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].i].X / 4;
                            Ptmp.Y += I1.L_3D_Pts[I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].i].Y / 4;
                            Ptmp.Z += I1.L_3D_Pts[I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].i].Z / 4;

                        }
                        if (I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].j != BoundryPointsI1[G])
                        {
                            //this.Text +=  " `" + I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].j; 
                            Ptmp.X += I1.L_3D_Pts[I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].j].X / 4;
                            Ptmp.Y += I1.L_3D_Pts[I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].j].Y / 4;
                            Ptmp.Z += I1.L_3D_Pts[I1.L_Edges[I1.L_3D_Pts[BoundryPointsI1[G]].edges[i]].j].Z / 4;
                        }
                    }
                }
                

                Ptmp.X += I1.L_3D_Pts[BoundryPointsI1[G]].X / 2;
                Ptmp.Y += I1.L_3D_Pts[BoundryPointsI1[G]].Y / 2;
                Ptmp.Z += I1.L_3D_Pts[BoundryPointsI1[G]].Z / 2;

                Diff.X = Ptmp.X - I1.L_3D_Pts[BoundryPointsI1[G]].X;
                Diff.Y = Ptmp.Y - I1.L_3D_Pts[BoundryPointsI1[G]].Y;
                Diff.Z = Ptmp.Z - I1.L_3D_Pts[BoundryPointsI1[G]].Z;

                _3D_Point nTmp = new _3D_Point(0, 0, 0);

                _3D_Point N = new _3D_Point(0, 0, 0);
                _3D_Point Normal = new _3D_Point(0, 0, 0);

                for (int i = 0; i < I1.L_3D_Pts[BoundryPointsI1[G]].faces.Count(); i++)
                {
                    if (I1.Faces[I1.L_3D_Pts[BoundryPointsI1[G]].faces[i]].BlendRegion == true)
                    {
                        nTmp = CalcFaceNormal(I1, I1.Faces[I1.L_3D_Pts[BoundryPointsI1[G]].faces[i]], I1.L_3D_Pts[BoundryPointsI1[G]].faces[i]);
                        N.X += nTmp.X;
                        N.Y += nTmp.Y;
                        N.Z += nTmp.Z;
                        //break;
                    }
                }

                N.X = N.X / I1.L_3D_Pts[BoundryPointsI1[G]].faces.Count();
                N.Y = N.Y / I1.L_3D_Pts[BoundryPointsI1[G]].faces.Count();
                N.Z = N.Z / I1.L_3D_Pts[BoundryPointsI1[G]].faces.Count();


                // N is normal , now normalize it

                Normal = NormalizeNormal(N);
                float ll = Magn(Normal);
                //MessageBox.Show("Vec = " + Normal.X + "," + Normal.Y + "," + Normal.Z + ">>>> with Len = " + ll);


                //draw normal aho


                /*SubDivison CurrentNormal = new SubDivison();
                CurrentNormal.AddPoint(new _3D_Point(I1.L_3D_Pts[BoundryPointsI1[G]].X, I1.L_3D_Pts[BoundryPointsI1[G]].Y, I1.L_3D_Pts[BoundryPointsI1[G]].Z));
                CurrentNormal.AddPoint(new _3D_Point(I1.L_3D_Pts[BoundryPointsI1[G]].X + (Normal.X * 100), I1.L_3D_Pts[BoundryPointsI1[G]].Y + (Normal.Y * 100), I1.L_3D_Pts[BoundryPointsI1[G]].Z + (Normal.Z * 100)));
                CurrentNormal.AddEdge(0, 1, Color.Blue);
                CurrentNormal.L_Edges[0].i = 0;
                CurrentNormal.L_Edges[0].j = 1;
                CurrentNormal.cam = cam;

                Objects.Add(CurrentNormal);*/



                //


                //dot product 

                float dot = DotProduct(Diff, Normal);
                _3D_Point dir = new _3D_Point(dot * Normal.X, dot * Normal.Y, dot * Normal.Z);


                Pnew.X = Ptmp.X - dir.X;
                Pnew.Y = Ptmp.Y - dir.Y;
                Pnew.Z = Ptmp.Z - dir.Z;

                NewPoints1.Add(Pnew);
            }

            for (int G = 0; G < BoundryPointsI1.Count(); G++)
            {
                I1.L_3D_Pts[BoundryPointsI1[G]].X = NewPoints1[G].X;
                I1.L_3D_Pts[BoundryPointsI1[G]].Y = NewPoints1[G].Y;
                I1.L_3D_Pts[BoundryPointsI1[G]].Z = NewPoints1[G].Z;
            }
            
        }
        void BoundrySmoothing(SubDivison I1, SubDivison I2)
        {
            
            BoundryPointsI1 = LocateBoundryPoints(I1);
            BoundryPointsI2 = LocateBoundryPoints(I2);

            IterationOfSmoothing(I1, BoundryPointsI1);
            IterationOfSmoothing(I2, BoundryPointsI2);

        }

        void CreateCube(SubDivison M , float XS , float YS , float ZS , int W , Color cl)
        {
            float[] vert = 
                            { 
                                XS,     YS+W,          ZS,
                                XS+W,   YS+W,          ZS,
                                XS+W,   YS,            ZS,
                                XS,     YS,            ZS,
                                XS,     YS+W,          ZS+W,
                                XS+W,   YS+W,          ZS+W,
                                XS+W,   YS,            ZS+W,
                                XS,     YS,            ZS+W
                            };


            _3D_Point pnn;
            int j = 0;
            for (int i = 0; i < 8; i++)
            {
                pnn = new _3D_Point(vert[j], vert[j + 1], vert[j + 2]);
                j += 3;
                M.AddPoint(pnn);
            }


            int[] Edges = {
                                0,1, //0
                                1,2, //1
                                2,3, //2 
                                3,0, //3
                                4,5, //4
                                5,6, //5
                                6,7, //6
                                7,4, //7 
                                0,4, //8
                                3,7, //9
                                2,6, //10
                                1,5  //11
                          };
            j = 0;

            for (int i = 0; i < 12; i++)
            {
                M.AddEdge(Edges[j], Edges[j + 1], cl);

                j += 2;
            }

            //faces
            Face F = new Face(); // f1 awl wa7ed wesh
            F.points.Add(0);
            F.points.Add(1);
            F.points.Add(2);
            F.points.Add(3);
            F.edges.Add(0);
            F.edges.Add(1);
            F.edges.Add(2);
            F.edges.Add(3);
            M.Faces.Add(F);

            F = new Face();//f2 ymeen
            F.points.Add(1);
            F.points.Add(5);
            F.points.Add(2);
            F.points.Add(6);
            F.edges.Add(11);
            F.edges.Add(5);
            F.edges.Add(10);
            F.edges.Add(1);
            M.Faces.Add(F);

            F = new Face();//f3 wara
            F.points.Add(4);
            F.points.Add(5);
            F.points.Add(6);
            F.points.Add(7);
            F.edges.Add(6);
            F.edges.Add(5);
            F.edges.Add(4);
            F.edges.Add(7);
            M.Faces.Add(F);

            F = new Face();//f4 shmal
            F.points.Add(0);
            F.points.Add(4);
            F.points.Add(3);
            F.points.Add(7);
            F.edges.Add(3);
            F.edges.Add(9);
            F.edges.Add(7);
            F.edges.Add(8);
            M.Faces.Add(F);

            F = new Face();//f5 foo2
            F.points.Add(0);
            F.points.Add(1);
            F.points.Add(5);
            F.points.Add(4);
            F.edges.Add(0);
            F.edges.Add(11);
            F.edges.Add(4);
            F.edges.Add(8);
            M.Faces.Add(F);

            F = new Face();//f6 t7t
            F.points.Add(2);
            F.points.Add(3);
            F.points.Add(6);
            F.points.Add(7);
            F.edges.Add(2);
            F.edges.Add(10);
            F.edges.Add(6);
            F.edges.Add(9);
            M.Faces.Add(F);


            //Edges 
            M.L_Edges[0].setFaces(0,4);
            M.L_Edges[1].setFaces(0,1);
            M.L_Edges[2].setFaces(0,5);
            M.L_Edges[3].setFaces(0,3);
            M.L_Edges[4].setFaces(4,2);
            M.L_Edges[5].setFaces(1,2);
            M.L_Edges[6].setFaces(5,2);
            M.L_Edges[7].setFaces(2,3);
            M.L_Edges[8].setFaces(4,3);
            M.L_Edges[9].setFaces(3,5);
            M.L_Edges[10].setFaces(1,5);
            M.L_Edges[11].setFaces(1,4);

            //points


            M.L_3D_Pts[0].edges.Add(0);
            M.L_3D_Pts[0].edges.Add(3);
            M.L_3D_Pts[0].edges.Add(8);
            M.L_3D_Pts[0].faces.Add(0);
            M.L_3D_Pts[0].faces.Add(4);
            M.L_3D_Pts[0].faces.Add(3);

            
            M.L_3D_Pts[1].edges.Add(0);
            M.L_3D_Pts[1].edges.Add(11);
            M.L_3D_Pts[1].edges.Add(1);
            M.L_3D_Pts[1].faces.Add(0);
            M.L_3D_Pts[1].faces.Add(1);
            M.L_3D_Pts[1].faces.Add(4);

            
            M.L_3D_Pts[2].edges.Add(1);
            M.L_3D_Pts[2].edges.Add(2);
            M.L_3D_Pts[2].edges.Add(10);
            M.L_3D_Pts[2].faces.Add(0);
            M.L_3D_Pts[2].faces.Add(1);
            M.L_3D_Pts[2].faces.Add(5);

            
            M.L_3D_Pts[3].edges.Add(2);
            M.L_3D_Pts[3].edges.Add(3);
            M.L_3D_Pts[3].edges.Add(9);
            M.L_3D_Pts[3].faces.Add(0);
            M.L_3D_Pts[3].faces.Add(3);
            M.L_3D_Pts[3].faces.Add(5);

            
            M.L_3D_Pts[4].edges.Add(4);
            M.L_3D_Pts[4].edges.Add(8);
            M.L_3D_Pts[4].edges.Add(7);
            M.L_3D_Pts[4].faces.Add(2);
            M.L_3D_Pts[4].faces.Add(3);
            M.L_3D_Pts[4].faces.Add(4);

            
            M.L_3D_Pts[5].edges.Add(4);
            M.L_3D_Pts[5].edges.Add(5);
            M.L_3D_Pts[5].edges.Add(11);
            M.L_3D_Pts[5].faces.Add(1);
            M.L_3D_Pts[5].faces.Add(4);
            M.L_3D_Pts[5].faces.Add(2);

            
            M.L_3D_Pts[6].edges.Add(6);
            M.L_3D_Pts[6].edges.Add(10);
            M.L_3D_Pts[6].edges.Add(5);
            M.L_3D_Pts[6].faces.Add(2);
            M.L_3D_Pts[6].faces.Add(5);
            M.L_3D_Pts[6].faces.Add(1);

            
            M.L_3D_Pts[7].edges.Add(6);
            M.L_3D_Pts[7].edges.Add(9);
            M.L_3D_Pts[7].edges.Add(7);
            M.L_3D_Pts[7].faces.Add(2);
            M.L_3D_Pts[7].faces.Add(3);
            M.L_3D_Pts[7].faces.Add(5);

            calcFacePoints(M);
        }

        void CreateT(SubDivison M, float XS, float YS, float ZS, int W, Color cl)
        {
            float[] vert = 
                            { 
                                XS,     YS,            ZS,       //0
                                XS+W,   YS,            ZS,       //1
                                XS+W,   YS+W,          ZS,       //2
                                XS,     YS,            ZS+(W/2), //3
                                XS+W,   YS,            ZS+(W/2), //4
                                XS+W,   YS+W,          ZS+(W/2)  //5
                            };


            _3D_Point pnn;
            int j = 0;
            for (int i = 0; i < 6; i++)
            {
                pnn = new _3D_Point(vert[j], vert[j + 1], vert[j + 2]);
                j += 3;
                M.AddPoint(pnn);
            }


            int[] Edges = {
                                0,1, //0
                                1,2, //1
                                2,0, //2
                                3,4, //3
                                4,5, //4
                                5,3, //5
                                0,3, //6
                                1,4, //7 
                                5,2, //8
                          };
            j = 0;

            for (int i = 0; i < 9; i++)
            {
                M.AddEdge(Edges[j], Edges[j + 1], cl);

                j += 2;
            }

            //faces
            Face F = new Face(); // f0 awl wa7ed wesh
            F.points.Add(0);
            F.points.Add(1);
            F.points.Add(2);
            F.edges.Add(0);
            F.edges.Add(1);
            F.edges.Add(2);
            M.Faces.Add(F);

            F = new Face();//f1 ymeen
            F.points.Add(1);
            F.points.Add(2);
            F.points.Add(4);
            F.points.Add(5);
            F.edges.Add(1);
            F.edges.Add(4);
            F.edges.Add(7);
            F.edges.Add(8);
            M.Faces.Add(F);

            F = new Face();//f2 wara
            F.points.Add(3);
            F.points.Add(4);
            F.points.Add(5);
            F.edges.Add(3);
            F.edges.Add(4);
            F.edges.Add(5);
            M.Faces.Add(F);

            F = new Face();//f3 foo2
            F.points.Add(0);
            F.points.Add(2);
            F.points.Add(3);
            F.points.Add(5);
            F.edges.Add(2);
            F.edges.Add(5);
            F.edges.Add(6);
            F.edges.Add(8);
            M.Faces.Add(F);

            F = new Face();//f4 t7t
            F.points.Add(0);
            F.points.Add(1);
            F.points.Add(3);
            F.points.Add(4);
            F.edges.Add(0);
            F.edges.Add(3);
            F.edges.Add(6);
            F.edges.Add(7);
            M.Faces.Add(F);


            //Edges 
            M.L_Edges[0].setFaces(0, 4);
            M.L_Edges[1].setFaces(0, 1);
            M.L_Edges[2].setFaces(0, 3);
            M.L_Edges[3].setFaces(4, 2);
            M.L_Edges[4].setFaces(1, 2);
            M.L_Edges[5].setFaces(2, 3);
            M.L_Edges[6].setFaces(3, 4);
            M.L_Edges[7].setFaces(1, 4);
            M.L_Edges[8].setFaces(1, 3);
   

            //points/


            M.L_3D_Pts[0].edges.Add(0);
            M.L_3D_Pts[0].edges.Add(2);
            M.L_3D_Pts[0].edges.Add(6);
            M.L_3D_Pts[0].faces.Add(0);
            M.L_3D_Pts[0].faces.Add(4);
            M.L_3D_Pts[0].faces.Add(3);


            M.L_3D_Pts[1].edges.Add(0);
            M.L_3D_Pts[1].edges.Add(1);
            M.L_3D_Pts[1].edges.Add(7);
            M.L_3D_Pts[1].faces.Add(0);
            M.L_3D_Pts[1].faces.Add(1);
            M.L_3D_Pts[1].faces.Add(4);


            M.L_3D_Pts[2].edges.Add(1);
            M.L_3D_Pts[2].edges.Add(2);
            M.L_3D_Pts[2].edges.Add(8);
            M.L_3D_Pts[2].faces.Add(0);
            M.L_3D_Pts[2].faces.Add(1);
            M.L_3D_Pts[2].faces.Add(3);


            M.L_3D_Pts[3].edges.Add(3);
            M.L_3D_Pts[3].edges.Add(5);
            M.L_3D_Pts[3].edges.Add(6);
            M.L_3D_Pts[3].faces.Add(2);
            M.L_3D_Pts[3].faces.Add(3);
            M.L_3D_Pts[3].faces.Add(4);


            M.L_3D_Pts[4].edges.Add(3);
            M.L_3D_Pts[4].edges.Add(4);
            M.L_3D_Pts[4].edges.Add(7);
            M.L_3D_Pts[4].faces.Add(1);
            M.L_3D_Pts[4].faces.Add(2);
            M.L_3D_Pts[4].faces.Add(4);


            M.L_3D_Pts[5].edges.Add(4);
            M.L_3D_Pts[5].edges.Add(5);
            M.L_3D_Pts[5].edges.Add(8);
            M.L_3D_Pts[5].faces.Add(1);
            M.L_3D_Pts[5].faces.Add(2);
            M.L_3D_Pts[5].faces.Add(3);


            calcFacePoints(M);
        }

        void calcObjectCentroid(SubDivison I)
        {
            I.Centroid.X = 0;
            I.Centroid.Y = 0;
            I.Centroid.Z = 0;

            for(int i = 0; i < I.L_3D_Pts.Count(); i++)
            {
                I.Centroid.X += I.L_3D_Pts[i].X / I.L_3D_Pts.Count();
                I.Centroid.Y += I.L_3D_Pts[i].Y / I.L_3D_Pts.Count();
                I.Centroid.Z += I.L_3D_Pts[i].Z / I.L_3D_Pts.Count();
            }
        }
        void SubDivideObject(SubDivison Cube)
        {

            SubDivison I1 = new SubDivison();

            for (int i = 0; i < Cube.Faces.Count(); i++)
            {
                I1.AddPoint(Cube.Faces[i].facePoint);
                //face points clear
                //I1.L_3D_Pts[I1.L_3D_Pts.Count() - 1].vertx = true;
            }
            //calc all edge points
            for (int i = 0; i < Cube.L_Edges.Count; i++)
            {
                List<_3D_Point> L = new List<_3D_Point>();
                L.Add(Cube.L_3D_Pts[Cube.L_Edges[i].i]);//add the 2 vertix points of the current edge 
                L.Add(Cube.L_3D_Pts[Cube.L_Edges[i].j]);//add the 2 vertix points of the current edge 

                L.Add(Cube.Faces[Cube.L_Edges[i].faceI].facePoint); //add the 2 neighbour faces
                L.Add(Cube.Faces[Cube.L_Edges[i].faceJ].facePoint); //add the 2 neighbour faces

                Cube.L_Edges[i].edgePoint = (catmullClarck_calcAvgofPoints2(L));
            }

            for (int i = 0; i < Cube.Faces.Count; i++)//loop all 6 faces
            {
                //getting verts of this face 
                for (int j = 0; j < Cube.Faces[i].points.Count; j++)
                {
                    List<_3D_Point> LF = new List<_3D_Point>();
                    List<_3D_Point> LE = new List<_3D_Point>();

                    //gahez  lf & le
                    for (int k = 0; k < Cube.L_3D_Pts[Cube.Faces[i].points[j]].faces.Count; k++)
                        LF.Add(Cube.Faces[Cube.L_3D_Pts[Cube.Faces[i].points[j]].faces[k]].facePoint);

                    for (int k = 0; k < Cube.L_3D_Pts[Cube.Faces[i].points[j]].edges.Count; k++)
                        LE.Add(Cube.L_Edges[Cube.L_3D_Pts[Cube.Faces[i].points[j]].edges[k]].edgePoint);

                  
                    _3D_Point Vert = catmullClarck_calcVertexPoint(LF, LE, Cube.L_3D_Pts[Cube.Faces[i].points[j]], Cube.L_3D_Pts[Cube.Faces[i].points[j]].edges.Count);//try 4
                    int vertPos;

                    if (I1.isPointThere(Vert))
                    {
                        vertPos = I1.getPoint(Vert);
                    }
                    else
                    {
                        //Vert.vertx = true;
                        I1.AddPoint(Vert);
                        vertPos = I1.L_3D_Pts.Count() - 1;
                    }

                    int pos1 = 0, pos2 = 0;

 
                    int y = 0;
                    for (int x = 0; x < Cube.L_3D_Pts[Cube.Faces[i].points[j]].edges.Count(); x++)
                    {
                        if (Cube.Faces[i].isEdgeThere( Cube.L_Edges[Cube.L_3D_Pts[Cube.Faces[i].points[j]].edges[x]], Cube ))
                        {
                            if (I1.isPointThere(Cube.L_Edges[Cube.L_3D_Pts[Cube.Faces[i].points[j]].edges[x]].edgePoint))
                            {
                                if (y == 0)
                                    pos1 = I1.getPoint(Cube.L_Edges[Cube.L_3D_Pts[Cube.Faces[i].points[j]].edges[x]].edgePoint);
                                else
                                    pos2 = I1.getPoint(Cube.L_Edges[Cube.L_3D_Pts[Cube.Faces[i].points[j]].edges[x]].edgePoint);
                            }
                            else
                            {
                                I1.AddPoint(Cube.L_Edges[Cube.L_3D_Pts[Cube.Faces[i].points[j]].edges[x]].edgePoint);
                                //edge points clear
                                //I1.L_3D_Pts[I1.L_3D_Pts.Count() - 1].vertx = true;
                                if (y == 0)
                                    pos1 = I1.L_3D_Pts.Count() - 1;
                                else
                                    pos2 = I1.L_3D_Pts.Count() - 1;
                            }
                            y++;
                        }
                    }



                    int e1;
                    Face F = new Face();

                    F.points.Add(i);
                    I1.L_3D_Pts[i].faces.Add(I1.Faces.Count());
                    F.points.Add(vertPos);
                    I1.L_3D_Pts[vertPos].faces.Add(I1.Faces.Count());
                    F.points.Add(pos1);
                    I1.L_3D_Pts[pos1].faces.Add(I1.Faces.Count());
                    F.points.Add(pos2);
                    I1.L_3D_Pts[pos2].faces.Add(I1.Faces.Count());

                    //connections (4 EDGES OF EACH SUBFACE)
                    if (I1.isEdgeThere(new Edge(i, pos1)))
                    {
                        e1 = I1.getEdge(new Edge(i, pos1));
                        F.edges.Add(e1);
                    }
                    else
                    {
                        I1.AddEdge(i, pos1, Color.Black);//f and e1
                        F.edges.Add(I1.L_Edges.Count() - 1);
                        //adding edges to points
                        I1.L_3D_Pts[i].edges.Add(I1.L_Edges.Count() - 1);
                        I1.L_3D_Pts[pos1].edges.Add(I1.L_Edges.Count() - 1);
                    }

                    /////
                    if (I1.isEdgeThere(new Edge(i, pos2)))
                    {
                        e1 = I1.getEdge(new Edge(i, pos2));
                        F.edges.Add(e1);
                    }
                    else
                    {
                        I1.AddEdge(i, pos2, Color.Black);//f and e1
                        F.edges.Add(I1.L_Edges.Count() - 1);
                        //adding edges to points
                        I1.L_3D_Pts[i].edges.Add(I1.L_Edges.Count() - 1);
                        I1.L_3D_Pts[pos2].edges.Add(I1.L_Edges.Count() - 1);
                    }

                    //////
                    if (I1.isEdgeThere(new Edge(vertPos, pos1)))
                    {
                        e1 = I1.getEdge(new Edge(vertPos, pos1));
                        F.edges.Add(e1);
                    }
                    else
                    {
                        I1.AddEdge(vertPos, pos1, Color.Black);//v and e1
                        F.edges.Add(I1.L_Edges.Count() - 1);
                        //adding edges to points
                        I1.L_3D_Pts[vertPos].edges.Add(I1.L_Edges.Count() - 1);
                        I1.L_3D_Pts[pos1].edges.Add(I1.L_Edges.Count() - 1);
                    }

                    ////////
                    if (I1.isEdgeThere(new Edge(vertPos, pos2)))
                    {
                        e1 = I1.getEdge(new Edge(vertPos, pos2));
                        F.edges.Add(e1);
                    }
                    else
                    {
                        I1.AddEdge(vertPos, pos2, Color.Black);//v and e1
                        F.edges.Add(I1.L_Edges.Count() - 1);
                        //adding edges to points
                        I1.L_3D_Pts[vertPos].edges.Add(I1.L_Edges.Count() - 1);
                        I1.L_3D_Pts[pos2].edges.Add(I1.L_Edges.Count() - 1);
                    }


                    //(i) is the face point, (vertPos+1) is E1,  (vertPos+2) is E2, (vertPos) is Vertex point, these are the 4 points of each face.      
                  
                    // adding FaceI and FaceJ to edges

                    I1.Faces.Add(F);
                    int facePos = I1.Faces.Count - 1;
                    for (int f = 0; f < I1.Faces[facePos].edges.Count(); f++)
                    {
                        if (I1.L_Edges[I1.Faces[facePos].edges[f]].Ifilled == false)
                        {
                            I1.L_Edges[I1.Faces[facePos].edges[f]].faceI = facePos;
                            I1.L_Edges[I1.Faces[facePos].edges[f]].Ifilled = true;
                        }
                        else
                        {
                            I1.L_Edges[I1.Faces[facePos].edges[f]].faceJ = facePos;
                        }
                    }


                    //adding face to points
                    /*for (int f = 0; f < I1.Faces[facePos].points.Count; f++)
                    {
                        I1.Faces[facePos].points[f].faces.Add(I1.Faces[facePos]);
                    }*/

                }
            }

            //
            Cube.L_3D_Pts.Clear();
            Cube.L_Edges.Clear();
            Cube.Faces.Clear();

            Cube.L_3D_Pts = I1.L_3D_Pts;
            Cube.L_Edges = I1.L_Edges;
            Cube.Faces = I1.Faces;

            calcFacePoints(Cube);
            calcObjectCentroid(Cube);
            calcObjectRadius(Cube);
        }

        void calcObjectRadius(SubDivison I)
        {
            float min = 99999;
            for(int i = 0 ; i < I.Faces.Count() ; i++)
            {
                if(EuclideanDistance(I.Centroid, I.Faces[i].facePoint) < min)
                {
                    min = EuclideanDistance(I.Centroid, I.Faces[i].facePoint);
                }
            }

            I.Radius = min;
        }
        void calcFacePoints(SubDivison I)
        {
            for (int i = 0; i < I.Faces.Count; i++)
            {
                I.Faces[i].facePoint = catmullClarck_calcAvgofPoints(I.Faces[i].points, I);
            }
        }
        float EuclideanDistance(_3D_Point P1, _3D_Point P2)
        {
            double Distance;

            Distance = Math.Sqrt(Math.Pow(System.Convert.ToDouble(P1.X - P2.X), 2) + Math.Pow(System.Convert.ToDouble(P1.Y - P2.Y), 2) + Math.Pow(System.Convert.ToDouble(P1.Z - P2.Z), 2));

            return (float)Distance;
        }
        void LocateBlendRegion(SubDivison O1, SubDivison O2)
        {
            //checking for blend region

            for (int i = 0; i < O1.Faces.Count(); i++)
            {
                for (int j = 0; j < O2.Faces.Count(); j++)
                {
                    if ((O1.Faces[i].Radius + O2.Faces[j].Radius) > EuclideanDistance(O1.Faces[i].facePoint, O2.Faces[j].facePoint))
                    {
                        //this.Text = "O1 rad= " + O1.Faces[i].Radius + "  O2 rad= " + O2.Faces[j].Radius + " Distance=" + EuclideanDistance(O1.Faces[i].facePoint, O2.Faces[j].facePoint);
                        O1.Faces[i].BlendRegion = true;
                        O2.Faces[j].BlendRegion = true;

                        for(int u = 0 ; u < O1.Faces[i].edges.Count(); u++ )
                        {
                            O1.L_Edges[O1.Faces[i].edges[u]].blend = true;
                        }
                        for (int u = 0; u < O2.Faces[j].edges.Count(); u++)
                        {
                            O2.L_Edges[O2.Faces[j].edges[u]].blend = true;
                        }
                    }
 
                }
            }

            //checking for inners

            for(int i = 0 ; i < O2.Faces.Count () ; i++)
            {
                if(EuclideanDistance(O1.Centroid, O2.Faces[i].facePoint) < O1.Radius - 20)
                {
                    O2.Faces[i].BlendRegion = true;
                    for (int u = 0; u < O2.Faces[i].edges.Count(); u++)
                    {
                        O2.L_Edges[O2.Faces[i].edges[u]].blend = true;
                    }
                }
            }

            for (int i = 0; i < O1.Faces.Count(); i++)
            {
                if (EuclideanDistance(O2.Centroid, O1.Faces[i].facePoint) < O2.Radius)
                {
                    O1.Faces[i].BlendRegion = true;
                    for (int u = 0; u < O1.Faces[i].edges.Count(); u++)
                    {
                        O1.L_Edges[O1.Faces[i].edges[u]].blend = true;
                    }
                }
            }


        }

        void calculateRadius()
        {
            for (int k = 0; k < Objects.Count(); k++)
            {
                for (int i = 0; i < Objects[k].Faces.Count(); i++)
                {
                    float max = -99999;
                    for (int j = 0; j < Objects[k].Faces[i].points.Count(); j++)
                    {
                        if (max < EuclideanDistance(Objects[k].Faces[i].facePoint, Objects[k].L_3D_Pts[Objects[k].Faces[i].points[j]]))
                        {
                            max = EuclideanDistance(Objects[k].Faces[i].facePoint, Objects[k].L_3D_Pts[Objects[k].Faces[i].points[j]]);
                        }
                    }

                    Objects[k].Faces[i].Radius = max;
                }
            }
        }

        void subdivide_face(SubDivison I, int FN)
        {

            I.AddPoint(I.Faces[FN].facePoint);
            int facePointPos = I.L_3D_Pts.Count() - 1;

            //HIGH COMPUTATIONSS
            //calc all edge points
            for (int i = 0; i < I.Faces[FN].edges.Count; i++)
            {
                List<_3D_Point> L = new List<_3D_Point>();
                L.Add(I.L_3D_Pts[I.L_Edges[I.Faces[FN].edges[i]].i]);//add the 2 vertix points of the current edge 
                L.Add(I.L_3D_Pts[I.L_Edges[I.Faces[FN].edges[i]].j]);//add the 2 vertix points of the current edge 

                L.Add(I.Faces[I.L_Edges[I.Faces[FN].edges[i]].faceI].facePoint); //add the 2 neighbour faces
                L.Add(I.Faces[I.L_Edges[I.Faces[FN].edges[i]].faceJ].facePoint); //add the 2 neighbour faces

                I.L_Edges[i].edgePoint = (catmullClarck_calcAvgofPoints2(L));
            }


            //getting verts of this face 
            for (int j = 0; j < I.Faces[FN].points.Count; j++)
            {
                List<_3D_Point> LF = new List<_3D_Point>();
                List<_3D_Point> LE = new List<_3D_Point>();

                //gahez  lf & le
                for (int k = 0; k < I.L_3D_Pts[I.Faces[FN].points[j]].faces.Count; k++)
                    LF.Add(I.Faces[I.L_3D_Pts[I.Faces[FN].points[j]].faces[k]].facePoint);

                for (int k = 0; k < I.L_3D_Pts[I.Faces[FN].points[j]].edges.Count; k++)
                    LE.Add(I.L_Edges[I.L_3D_Pts[I.Faces[FN].points[j]].edges[k]].edgePoint);


                _3D_Point Vert = catmullClarck_calcVertexPoint(LF, LE, I.L_3D_Pts[I.Faces[FN].points[j]], I.L_3D_Pts[I.Faces[FN].points[j]].edges.Count);//try 4
                int vertPos;

                if (I.isPointThere(Vert))
                {
                    vertPos = I.getPoint(Vert);
                }
                else
                {
                    Vert.vertx = true;
                    I.AddPoint(Vert);
                    vertPos = I.L_3D_Pts.Count() - 1;
                }

                int pos1 = 0, pos2 = 0;


                int y = 0;
                for (int x = 0; x < I.L_3D_Pts[I.Faces[FN].points[j]].edges.Count(); x++)
                {
                    if (I.Faces[FN].isEdgeThere(I.L_Edges[I.L_3D_Pts[I.Faces[FN].points[j]].edges[x]], I))
                    {
                        if (I.isPointThere(I.L_Edges[I.L_3D_Pts[I.Faces[FN].points[j]].edges[x]].edgePoint))
                        {
                            if (y == 0)
                                pos1 = I.getPoint(I.L_Edges[I.L_3D_Pts[I.Faces[FN].points[j]].edges[x]].edgePoint);
                            else
                                pos2 = I.getPoint(I.L_Edges[I.L_3D_Pts[I.Faces[FN].points[j]].edges[x]].edgePoint);
                        }
                        else
                        {
                            I.AddPoint(I.L_Edges[I.L_3D_Pts[I.Faces[FN].points[j]].edges[x]].edgePoint);
                            //edge points clear
                            //I1.L_3D_Pts[I1.L_3D_Pts.Count() - 1].vertx = true;
                            if (y == 0)
                                pos1 = I.L_3D_Pts.Count() - 1;
                            else
                                pos2 = I.L_3D_Pts.Count() - 1;
                        }
                        y++;
                    }
                }



                int e1;
                Face F = new Face();

                F.points.Add(facePointPos);
                I.L_3D_Pts[facePointPos].faces.Add(I.Faces.Count());
                F.points.Add(vertPos);
                I.L_3D_Pts[vertPos].faces.Add(I.Faces.Count());
                F.points.Add(pos1);
                I.L_3D_Pts[pos1].faces.Add(I.Faces.Count());
                F.points.Add(pos2);
                I.L_3D_Pts[pos2].faces.Add(I.Faces.Count());

                //connections (4 EDGES OF EACH SUBFACE)
                if (I.isEdgeThere(new Edge(facePointPos, pos1)))
                {
                    e1 = I.getEdge(new Edge(facePointPos, pos1));
                    F.edges.Add(e1);
                }
                else
                {
                    I.AddEdge(facePointPos, pos1, Color.Black);//f and e1
                    F.edges.Add(I.L_Edges.Count() - 1);
                    //adding edges to points
                    I.L_3D_Pts[facePointPos].edges.Add(I.L_Edges.Count() - 1);
                    I.L_3D_Pts[pos1].edges.Add(I.L_Edges.Count() - 1);
                }

                /////
                if (I.isEdgeThere(new Edge(facePointPos, pos2)))
                {
                    e1 = I.getEdge(new Edge(facePointPos, pos2));
                    F.edges.Add(e1);
                }
                else
                {
                    I.AddEdge(facePointPos, pos2, Color.Black);//f and e1
                    F.edges.Add(I.L_Edges.Count() - 1);
                    //adding edges to points
                    I.L_3D_Pts[facePointPos].edges.Add(I.L_Edges.Count() - 1);
                    I.L_3D_Pts[pos2].edges.Add(I.L_Edges.Count() - 1);
                }

                //////
                if (I.isEdgeThere(new Edge(vertPos, pos1)))
                {
                    e1 = I.getEdge(new Edge(vertPos, pos1));
                    F.edges.Add(e1);
                }
                else
                {
                    I.AddEdge(vertPos, pos1, Color.Black);//v and e1
                    F.edges.Add(I.L_Edges.Count() - 1);
                    //adding edges to points
                    I.L_3D_Pts[vertPos].edges.Add(I.L_Edges.Count() - 1);
                    I.L_3D_Pts[pos1].edges.Add(I.L_Edges.Count() - 1);
                }

                ////////
                if (I.isEdgeThere(new Edge(vertPos, pos2)))
                {
                    e1 = I.getEdge(new Edge(vertPos, pos2));
                    F.edges.Add(e1);
                }
                else
                {
                    I.AddEdge(vertPos, pos2, Color.Black);//v and e1
                    F.edges.Add(I.L_Edges.Count() - 1);
                    //adding edges to points
                    I.L_3D_Pts[vertPos].edges.Add(I.L_Edges.Count() - 1);
                    I.L_3D_Pts[pos2].edges.Add(I.L_Edges.Count() - 1);
                }


                //(i) is the face point, (vertPos+1) is E1,  (vertPos+2) is E2, (vertPos) is Vertex point, these are the 4 points of each face.      

                // adding FaceI and FaceJ to edges

                I.Faces.Add(F);
                int facePos = I.Faces.Count - 1;
                for (int f = 0; f < I.Faces[facePos].edges.Count(); f++)
                {
                    if (I.L_Edges[I.Faces[facePos].edges[f]].Ifilled == false)
                    {
                        I.L_Edges[I.Faces[facePos].edges[f]].faceI = facePos;
                        I.L_Edges[I.Faces[facePos].edges[f]].Ifilled = true;
                    }
                    else
                    {
                        I.L_Edges[I.Faces[facePos].edges[f]].faceJ = facePos;
                    }
                }

            }

  
        }

        _3D_Point catmullClarck_calcAvgofPoints(List<int> L, SubDivison I)
        {
            _3D_Point Face = new _3D_Point(0, 0, 0);
            for (int i = 0; i < L.Count; i++) 
            {
                Face.X += I.L_3D_Pts[L[i]].X / L.Count;
                Face.Y += I.L_3D_Pts[L[i]].Y / L.Count;
                Face.Z += I.L_3D_Pts[L[i]].Z / L.Count;
            }
            return Face;
        }

        //Usef with LIST OF POINTS
        _3D_Point catmullClarck_calcAvgofPoints2(List<_3D_Point> L)
        {
            _3D_Point Face = new _3D_Point(0, 0, 0);
            for (int i = 0; i < L.Count; i++)
            {
                Face.X += L[i].X / L.Count;
                Face.Y += L[i].Y / L.Count;
                Face.Z += L[i].Z / L.Count;
            }
            return Face;
        }
        _3D_Point catmullClarck_calcVertexPoint(List<_3D_Point> LFaces, List<_3D_Point> LEdges, _3D_Point V, float n)
        {
            _3D_Point AvgFaces = new _3D_Point(0, 0, 0);
            _3D_Point AvgEdges = new _3D_Point(0, 0, 0);

            //calculating Avg of faces (F/n);
            for (int i = 0 ; i < LFaces.Count ; i++)
            {
                AvgFaces.X += LFaces[i].X / LFaces.Count;
                AvgFaces.Y += LFaces[i].Y / LFaces.Count;
                AvgFaces.Z += LFaces[i].Z / LFaces.Count;
            }
            AvgFaces.X = AvgFaces.X / n;
            AvgFaces.Y = AvgFaces.Y / n;
            AvgFaces.Z = AvgFaces.Z / n;

            //calculating Avg of edges (2E/n);
            for (int i = 0; i < LEdges.Count; i++)
            {
                AvgEdges.X += LEdges[i].X / LEdges.Count;
                AvgEdges.Y += LEdges[i].Y / LEdges.Count;
                AvgEdges.Z += LEdges[i].Z / LEdges.Count;
            }
            AvgEdges.X = (2 * AvgEdges.X) / n;
            AvgEdges.Y = (2 * AvgEdges.Y) / n;
            AvgEdges.Z = (2 * AvgEdges.Z) / n;

            //constant C
            float Constant = n - 3;
            
            // Multipling original vertex V times the constant C
            _3D_Point VCostant = new _3D_Point((Constant * V.X) / n, (Constant * V.Y) / n, (Constant * V.Z) / n);
   
            //New Vertex point = all of the above / 3 ...... >>   (F/n) + (2E/n) + ((n-3)V/n) 
            _3D_Point Vertex = new _3D_Point(AvgFaces.X + AvgEdges.X + VCostant.X, AvgFaces.Y + AvgEdges.Y + VCostant.Y, AvgFaces.Z + AvgEdges.Z + VCostant.Z);
            return Vertex;
        }
       

        void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(this.ClientSize.Width , this.ClientSize.Height);

            cs = Math.Cos(1 * Math.PI / 180);
            sn = Math.Sin(1 * Math.PI / 180);

            this.Text = "" + (selectedObject + 1);

            int cx = 1500;
            int cy = 1500;
            cam.ceneterX =  (cx / 2);
            cam.ceneterY =  (cy / 2) - 330;
            cam.cxScreen = cx;
            cam.cyScreen = cy;            
            cam.BuildNewSystem();

            S.cam = cam;
            S.Design();


            //ALL OBJECTS 


            //cube 1
            SubDivison Cube = new SubDivison();
            Cube.cam = cam;
            CreateCube(Cube , -700, -300, -300, 600 , Color.Black);
            Objects.Add(Cube);


            /*Tri.cam = cam;
            CreateT(Tri, -700, -300, -150, 600, Color.Black);
            Objects.Add(Tri);

            //Triangle 3
            Tri = new SubDivison();
            Tri.cam = cam;
            CreateT(Tri, 100, -300, -150, 600, Color.Black);
            Objects.Add(Tri);*/
            
            //cube 2
            Cube = new SubDivison();
            Cube.cam = cam;
            CreateCube(Cube, 100, -300, -300, 600, Color.Black);
            Objects.Add(Cube);
            
            /*Cube = new SubDivison();
            Cube.cam = cam;
            CreateCube(Cube, -700, -300, -300, 600, Color.Black);
            Objects.Add(Cube);

            Cube = new SubDivison();
            Cube.cam = cam;
            CreateCube(Cube, -700, -300, -300, 600, Color.Black);
            Objects.Add(Cube);*/
            
            //Triangle 3 
            /*Tri.cam = cam;
            CreateT(Tri, -1800, -300, -300, 600, Color.Blue);
            Objects.Add(Tri);


            Tri.cam = cam;
            CreateT(Tri, -800, -300, -300, 600, Color.Blue);
            Objects.Add(Tri);*/


            //Cylinder
            /*Cylinder C = new Cylinder();
            C.cam = cam;
            C.Design();
            Objects.Add(C);*/
            

            calculateRadius();
            Normals.cam = cam;
        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubble(e.Graphics);
        }

        void DrawScene(Graphics g)
        {
            g.Clear(Color.Bisque);
            
            for(int i = 0 ; i < Objects.Count() ; i++)
            {
                Objects[i].DrawYourSelf(g);
            }

            Normals.DrawYourSelf(g);
        }

        void DrawDubble(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
