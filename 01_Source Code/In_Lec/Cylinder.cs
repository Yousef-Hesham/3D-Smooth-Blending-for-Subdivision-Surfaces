using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace In_Lec
{
    class Cylinder : SubDivison
    {
        void Build()
        {
            float xCenter = 500;
            float zCenter = 0;
            float Rad = 200;

            float theta = 0, x = 100, y = 300, z = 0;

            int j = 0;
            int k = 0;

            for (int i = 0; i <= 6;  i++) // Levels 
            {

                theta = 0;
             

                for (k = j ; theta < Math.PI * 2; )
                {
                    x = (Rad * (float)Math.Cos(theta) + xCenter);
                    z = (Rad * (float)Math.Sin(theta) + zCenter);

                    if(!this.isPointThere(new _3D_Point(x, y, z)))
                    {
                        this.AddPoint(new _3D_Point(x, y, z));
                    
                        if (k != j)
                        {
                            this.AddEdge(j - 1, j, Color.Black);
                        }
                        if (i != 0)
                        {
                            this.AddEdge(j - 8, j, Color.Black);
                        }
                        j++;
                    }
                    theta += ((float)(Math.PI * 2) / 8);
                   
                }

                this.AddEdge(j-1, k , Color.Black);
                y -= 100;
            }

        }

        void setSub()
        {
            //faces done
       
            int ETop=0, EBot=9, ERight=10, ELeft=8;
            int PTopLeft=0, PTopRight=1, PBotLeft=8, PBotRight=9;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Face F = new Face();
                    F.points.Add(PTopLeft);
                    F.points.Add(PTopRight);
                    F.points.Add(PBotLeft);
                    F.points.Add(PBotRight);

                    F.edges.Add(ETop);
                    F.edges.Add(ELeft);
                    F.edges.Add(EBot);
                    F.edges.Add(ERight);

                    PTopLeft++;
                    PBotLeft++;
                    PBotRight++;
                    PTopRight++;
                 

                    if (i == 0)
                        ETop++;
                    else
                        ETop += 2;

                    EBot += 2;
                    ERight += 2;
                    ELeft += 2;

                    this.Faces.Add(F);
                }
            }

        }
        public void Design()
        {
            L_3D_Pts = new List<_3D_Point>();
            L_Edges = new List<Edge>();
            Faces = new List<Face>();

            Build();
            setSub();
        }
    }
}
