using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SamSeifert.GLE
{
    public static class Draw
    {
        static readonly Vector3[] _PrismVector3 = new Vector3[8];

        public static void RectangularPrism(Single x, Single y, Single z, Single scale)
        {
            Draw.RectangularPrism(x * scale, y * scale, z * scale);
        }

        public static void RectangularPrism(Single x, Single y, Single z)
        {
            _PrismVector3[0] = new Vector3(x, y, z);
            _PrismVector3[1] = new Vector3(-x, y, z);
            _PrismVector3[2] = new Vector3(-x, -y, z);
            _PrismVector3[3] = new Vector3(x, -y, z);

            _PrismVector3[4] = new Vector3(x, y, -z);
            _PrismVector3[5] = new Vector3(-x, y, -z);
            _PrismVector3[6] = new Vector3(-x, -y, -z);
            _PrismVector3[7] = new Vector3(x, -y, -z);

            GL.Begin(BeginMode.Quads);
            {
                GL.Normal3(Vector3.UnitZ);
                GL.Vertex3(_PrismVector3[0]);
                GL.Vertex3(_PrismVector3[1]);
                GL.Vertex3(_PrismVector3[2]);
                GL.Vertex3(_PrismVector3[3]);

                GL.Normal3(-Vector3.UnitZ);
                GL.Vertex3(_PrismVector3[7]);
                GL.Vertex3(_PrismVector3[6]);
                GL.Vertex3(_PrismVector3[5]);
                GL.Vertex3(_PrismVector3[4]);
                
                GL.Normal3(Vector3.UnitY);
                GL.Vertex3(_PrismVector3[1]);
                GL.Vertex3(_PrismVector3[0]);
                GL.Vertex3(_PrismVector3[4]);
                GL.Vertex3(_PrismVector3[5]);

                GL.Normal3(-Vector3.UnitY);
                GL.Vertex3(_PrismVector3[3]);
                GL.Vertex3(_PrismVector3[2]);
                GL.Vertex3(_PrismVector3[6]);
                GL.Vertex3(_PrismVector3[7]);

                GL.Normal3(Vector3.UnitX);
                GL.Vertex3(_PrismVector3[0]);
                GL.Vertex3(_PrismVector3[3]);
                GL.Vertex3(_PrismVector3[7]);
                GL.Vertex3(_PrismVector3[4]);

                GL.Normal3(-Vector3.UnitX);
                GL.Vertex3(_PrismVector3[2]);
                GL.Vertex3(_PrismVector3[1]);
                GL.Vertex3(_PrismVector3[5]);
                GL.Vertex3(_PrismVector3[6]);
            }
            GL.End();
        }

        public static void RectangularPrismShell(Single x, Single y, Single z, Single scale)
        {
            Draw.RectangularPrismShell(x * scale, y * scale, z * scale);
        }

        public static void RectangularPrismShell(Single x, Single y, Single z)
        {
            _PrismVector3[0] = new Vector3(x, y, z);
            _PrismVector3[1] = new Vector3(-x, y, z);
            _PrismVector3[2] = new Vector3(-x, -y, z);
            _PrismVector3[3] = new Vector3(x, -y, z);

            _PrismVector3[4] = new Vector3(x, y, -z);
            _PrismVector3[5] = new Vector3(-x, y, -z);
            _PrismVector3[6] = new Vector3(-x, -y, -z);
            _PrismVector3[7] = new Vector3(x, -y, -z);

            GL.Begin(BeginMode.Lines);
            {
                GL.Vertex3(_PrismVector3[0]); GL.Vertex3(_PrismVector3[1]);
                GL.Vertex3(_PrismVector3[1]); GL.Vertex3(_PrismVector3[2]);
                GL.Vertex3(_PrismVector3[2]); GL.Vertex3(_PrismVector3[3]);
                GL.Vertex3(_PrismVector3[3]); GL.Vertex3(_PrismVector3[0]);

                GL.Vertex3(_PrismVector3[4]); GL.Vertex3(_PrismVector3[5]);
                GL.Vertex3(_PrismVector3[5]); GL.Vertex3(_PrismVector3[6]);
                GL.Vertex3(_PrismVector3[6]); GL.Vertex3(_PrismVector3[7]);
                GL.Vertex3(_PrismVector3[7]); GL.Vertex3(_PrismVector3[4]);

                GL.Vertex3(_PrismVector3[4]); GL.Vertex3(_PrismVector3[0]);
                GL.Vertex3(_PrismVector3[5]); GL.Vertex3(_PrismVector3[1]);
                GL.Vertex3(_PrismVector3[6]); GL.Vertex3(_PrismVector3[2]);
                GL.Vertex3(_PrismVector3[7]); GL.Vertex3(_PrismVector3[3]);
            }
            GL.End();
        }

















        const int CylCount = 24;
        const int CylCount1 = CylCount + 1;
        static readonly Vector3[] top = new Vector3[CylCount1];
        static readonly Vector3[] bot = new Vector3[CylCount1];
        static readonly Vector3[] nor = new Vector3[CylCount1];

        public static void CylinderPrism(Single r, Single x, Single scale)
        {
            Draw.CylinderPrism(r * scale, x * scale);
        }

        public static void CylinderPrism(Single r, Single x)
        {
            Single ang, cosA, sinA;
            for (int i = 0; i < CylCount; i++)
            {
                ang = (i) * (Single)(2 * Math.PI / CylCount);
                cosA = r * (Single)Math.Cos(ang);
                sinA = r * (Single)Math.Sin(ang);
                bot[i] = new Vector3(-x, cosA, sinA);
                top[i] = new Vector3(x, cosA, sinA);
                nor[i] = new Vector3(0, cosA, sinA);
            }

            top[CylCount] = top[0];
            bot[CylCount] = bot[0];
            nor[CylCount] = nor[0];

            GL.Begin(BeginMode.TriangleFan);
            {
                GL.Normal3(Vector3.UnitX);
                GL.Vertex3(x, 0, 0);
                for (int i = 0; i < CylCount1; i++) GL.Vertex3(top[i]);
            }
            GL.End();

            GL.Begin(BeginMode.TriangleFan);
            {
                GL.Normal3(-Vector3.UnitX);
                GL.Vertex3(-x, 0, 0);
                for (int i = CylCount1; i > 0; i--) GL.Vertex3(bot[i - 1]);
            }
            GL.End();


            GL.Begin(BeginMode.Quads);
            {
                Vector3 norm = Vector3.Zero;

                for (int i = 0; i < CylCount; i++)
                {
                    GL.Normal3(nor[i]);
                    GL.Vertex3(top[i]);
                    GL.Vertex3(bot[i]);
                    GL.Normal3(nor[i + 1]);
                    GL.Vertex3(bot[i + 1]);
                    GL.Vertex3(top[i + 1]);
                }
            }
            GL.End();
        }

        public static void CylinderPrismShell(Single r, Single x, Single scale)
        {
            Draw.CylinderPrismShell(r * scale, x * scale);
        }

        public static void CylinderPrismShell(Single r, Single x)
        {
            Single ang, cosA, sinA;
            for (int i = 0; i < CylCount; i++)
            {
                ang = (i) * (Single)(2 * Math.PI / CylCount);
                cosA = r * (Single)Math.Cos(ang);
                sinA = r * (Single)Math.Sin(ang);
                bot[i] = new Vector3(-x, cosA, sinA);
                top[i] = new Vector3(x, cosA, sinA);
            }

            GL.Begin(BeginMode.Lines);
            {
                for (int i = 0; i < CylCount; i++)
                {
                    GL.Vertex3(bot[i]); GL.Vertex3(bot[i + 1]);
                    GL.Vertex3(top[i]); GL.Vertex3(top[i + 1]);
                    GL.Vertex3(bot[i]); GL.Vertex3(top[i]);
                }
            }
            GL.End();
        }















        private static Triangle[] _Sphere = null;
        public static void Sphere(float radius)
        {
            if (_Sphere == null)
            {
                _Sphere = new Triangle[] {
          new Triangle(
            fm(-0.27639,0.85065,0.44721),
            fm(0.26287,0.80902,0.52573),
            fm(6.5257e-17,1,-3.2629e-17)),
          new Triangle(
            fm(0.26287,0.80902,0.52573),
            fm(0.72361,0.52573,0.44721),
            fm(0.58779,0.80902,-3.2629e-17)),
          new Triangle(
            fm(6.5257e-17,1,-3.2629e-17),
            fm(0.26287,0.80902,0.52573),
            fm(0.58779,0.80902,-3.2629e-17)),
          new Triangle(
            fm(6.5257e-17,1,-3.2629e-17),
            fm(0.58779,0.80902,-3.2629e-17),
            fm(0.27639,0.85065,-0.44721)),
          new Triangle(
            fm(-0.72361,0.52573,-0.44721),
            fm(-0.58779,0.80902,0),
            fm(-0.26287,0.80902,-0.52573)),
          new Triangle(
            fm(-0.58779,0.80902,0),
            fm(-0.27639,0.85065,0.44721),
            fm(6.5257e-17,1,-3.2629e-17)),
          new Triangle(
            fm(-0.26287,0.80902,-0.52573),
            fm(-0.58779,0.80902,0),
            fm(6.5257e-17,1,-3.2629e-17)),
          new Triangle(
            fm(-0.26287,0.80902,-0.52573),
            fm(6.5257e-17,1,-3.2629e-17),
            fm(0.27639,0.85065,-0.44721)),
          new Triangle(
            fm(-0.89443,1.0954e-16,0.44721),
            fm(-0.68819,0.5,0.52573),
            fm(-0.95106,0.30902,0)),
          new Triangle(
            fm(-0.68819,0.5,0.52573),
            fm(-0.27639,0.85065,0.44721),
            fm(-0.58779,0.80902,0)),
          new Triangle(
            fm(-0.95106,0.30902,0),
            fm(-0.68819,0.5,0.52573),
            fm(-0.58779,0.80902,0)),
          new Triangle(
            fm(-0.95106,0.30902,0),
            fm(-0.58779,0.80902,0),
            fm(-0.72361,0.52573,-0.44721)),
          new Triangle(
            fm(-0.72361,-0.52573,-0.44721),
            fm(-0.95106,-0.30902,0),
            fm(-0.85065,1.3051e-16,-0.52573)),
          new Triangle(
            fm(-0.95106,-0.30902,0),
            fm(-0.89443,1.0954e-16,0.44721),
            fm(-0.95106,0.30902,0)),
          new Triangle(
            fm(-0.85065,1.3051e-16,-0.52573),
            fm(-0.95106,-0.30902,0),
            fm(-0.95106,0.30902,0)),
          new Triangle(
            fm(-0.85065,1.3051e-16,-0.52573),
            fm(-0.95106,0.30902,0),
            fm(-0.72361,0.52573,-0.44721)),
          new Triangle(
            fm(-0.27639,-0.85065,0.44721),
            fm(-0.68819,-0.5,0.52573),
            fm(-0.58779,-0.80902,3.2629e-17)),
          new Triangle(
            fm(-0.68819,-0.5,0.52573),
            fm(-0.89443,1.0954e-16,0.44721),
            fm(-0.95106,-0.30902,0)),
          new Triangle(
            fm(-0.58779,-0.80902,3.2629e-17),
            fm(-0.68819,-0.5,0.52573),
            fm(-0.95106,-0.30902,0)),
          new Triangle(
            fm(-0.58779,-0.80902,3.2629e-17),
            fm(-0.95106,-0.30902,0),
            fm(-0.72361,-0.52573,-0.44721)),
          new Triangle(
            fm(0.27639,-0.85065,-0.44721),
            fm(-1.9577e-16,-1,3.2629e-17),
            fm(-0.26287,-0.80902,-0.52573)),
          new Triangle(
            fm(-1.9577e-16,-1,3.2629e-17),
            fm(-0.27639,-0.85065,0.44721),
            fm(-0.58779,-0.80902,3.2629e-17)),
          new Triangle(
            fm(-0.26287,-0.80902,-0.52573),
            fm(-1.9577e-16,-1,3.2629e-17),
            fm(-0.58779,-0.80902,3.2629e-17)),
          new Triangle(
            fm(-0.26287,-0.80902,-0.52573),
            fm(-0.58779,-0.80902,3.2629e-17),
            fm(-0.72361,-0.52573,-0.44721)),
          new Triangle(
            fm(0.72361,-0.52573,0.44721),
            fm(0.26287,-0.80902,0.52573),
            fm(0.58779,-0.80902,0)),
          new Triangle(
            fm(0.26287,-0.80902,0.52573),
            fm(-0.27639,-0.85065,0.44721),
            fm(-1.9577e-16,-1,3.2629e-17)),
          new Triangle(
            fm(0.58779,-0.80902,0),
            fm(0.26287,-0.80902,0.52573),
            fm(-1.9577e-16,-1,3.2629e-17)),
          new Triangle(
            fm(0.58779,-0.80902,0),
            fm(-1.9577e-16,-1,3.2629e-17),
            fm(0.27639,-0.85065,-0.44721)),
          new Triangle(
            fm(0.89443,-2.1907e-16,-0.44721),
            fm(0.95106,-0.30902,0),
            fm(0.68819,-0.5,-0.52573)),
          new Triangle(
            fm(0.95106,-0.30902,0),
            fm(0.72361,-0.52573,0.44721),
            fm(0.58779,-0.80902,0)),
          new Triangle(
            fm(0.68819,-0.5,-0.52573),
            fm(0.95106,-0.30902,0),
            fm(0.58779,-0.80902,0)),
          new Triangle(
            fm(0.68819,-0.5,-0.52573),
            fm(0.58779,-0.80902,0),
            fm(0.27639,-0.85065,-0.44721)),
          new Triangle(
            fm(0.72361,0.52573,0.44721),
            fm(0.85065,-1.9577e-16,0.52573),
            fm(0.95106,0.30902,3.2629e-17)),
          new Triangle(
            fm(0.85065,-1.9577e-16,0.52573),
            fm(0.72361,-0.52573,0.44721),
            fm(0.95106,-0.30902,0)),
          new Triangle(
            fm(0.95106,0.30902,3.2629e-17),
            fm(0.85065,-1.9577e-16,0.52573),
            fm(0.95106,-0.30902,0)),
          new Triangle(
            fm(0.95106,0.30902,3.2629e-17),
            fm(0.95106,-0.30902,0),
            fm(0.89443,-2.1907e-16,-0.44721)),
          new Triangle(
            fm(0.27639,0.85065,-0.44721),
            fm(0.58779,0.80902,0),
            fm(0.68819,0.5,-0.52573)),
          new Triangle(
            fm(0.58779,0.80902,0),
            fm(0.72361,0.52573,0.44721),
            fm(0.95106,0.30902,3.2629e-17)),
          new Triangle(
            fm(0.68819,0.5,-0.52573),
            fm(0.58779,0.80902,0),
            fm(0.95106,0.30902,3.2629e-17)),
          new Triangle(
            fm(0.68819,0.5,-0.52573),
            fm(0.95106,0.30902,3.2629e-17),
            fm(0.89443,-2.1907e-16,-0.44721)),
          new Triangle(
            fm(-0.89443,1.0954e-16,0.44721),
            fm(-0.68819,-0.5,0.52573),
            fm(-0.52573,6.4383e-17,0.85065)),
          new Triangle(
            fm(-0.68819,-0.5,0.52573),
            fm(-0.27639,-0.85065,0.44721),
            fm(-0.16246,-0.5,0.85065)),
          new Triangle(
            fm(-0.52573,6.4383e-17,0.85065),
            fm(-0.68819,-0.5,0.52573),
            fm(-0.16246,-0.5,0.85065)),
          new Triangle(
            fm(-0.52573,6.4383e-17,0.85065),
            fm(-0.16246,-0.5,0.85065),
            fm(0,0,1)),
          new Triangle(
            fm(-0.27639,-0.85065,0.44721),
            fm(0.26287,-0.80902,0.52573),
            fm(-0.16246,-0.5,0.85065)),
          new Triangle(
            fm(0.26287,-0.80902,0.52573),
            fm(0.72361,-0.52573,0.44721),
            fm(0.42533,-0.30902,0.85065)),
          new Triangle(
            fm(-0.16246,-0.5,0.85065),
            fm(0.26287,-0.80902,0.52573),
            fm(0.42533,-0.30902,0.85065)),
          new Triangle(
            fm(-0.16246,-0.5,0.85065),
            fm(0.42533,-0.30902,0.85065),
            fm(0,0,1)),
          new Triangle(
            fm(0.72361,-0.52573,0.44721),
            fm(0.85065,-1.9577e-16,0.52573),
            fm(0.42533,-0.30902,0.85065)),
          new Triangle(
            fm(0.85065,-1.9577e-16,0.52573),
            fm(0.72361,0.52573,0.44721),
            fm(0.42533,0.30902,0.85065)),
          new Triangle(
            fm(0.42533,-0.30902,0.85065),
            fm(0.85065,-1.9577e-16,0.52573),
            fm(0.42533,0.30902,0.85065)),
          new Triangle(
            fm(0.42533,-0.30902,0.85065),
            fm(0.42533,0.30902,0.85065),
            fm(0,0,1)),
          new Triangle(
            fm(0.72361,0.52573,0.44721),
            fm(0.26287,0.80902,0.52573),
            fm(0.42533,0.30902,0.85065)),
          new Triangle(
            fm(0.26287,0.80902,0.52573),
            fm(-0.27639,0.85065,0.44721),
            fm(-0.16246,0.5,0.85065)),
          new Triangle(
            fm(0.42533,0.30902,0.85065),
            fm(0.26287,0.80902,0.52573),
            fm(-0.16246,0.5,0.85065)),
          new Triangle(
            fm(0.42533,0.30902,0.85065),
            fm(-0.16246,0.5,0.85065),
            fm(0,0,1)),
          new Triangle(
            fm(-0.27639,0.85065,0.44721),
            fm(-0.68819,0.5,0.52573),
            fm(-0.16246,0.5,0.85065)),
          new Triangle(
            fm(-0.68819,0.5,0.52573),
            fm(-0.89443,3.2861e-16,0.44721),
            fm(-0.52573,1.9315e-16,0.85065)),
          new Triangle(
            fm(-0.16246,0.5,0.85065),
            fm(-0.68819,0.5,0.52573),
            fm(-0.52573,1.9315e-16,0.85065)),
          new Triangle(
            fm(-0.16246,0.5,0.85065),
            fm(-0.52573,1.9315e-16,0.85065),
            fm(0,0,1)),
          new Triangle(
            fm(-0.72361,-0.52573,-0.44721),
            fm(-0.85065,1.3051e-16,-0.52573),
            fm(-0.42533,-0.30902,-0.85065)),
          new Triangle(
            fm(-0.85065,1.3051e-16,-0.52573),
            fm(-0.72361,0.52573,-0.44721),
            fm(-0.42533,0.30902,-0.85065)),
          new Triangle(
            fm(-0.42533,-0.30902,-0.85065),
            fm(-0.85065,1.3051e-16,-0.52573),
            fm(-0.42533,0.30902,-0.85065)),
          new Triangle(
            fm(-0.42533,-0.30902,-0.85065),
            fm(-0.42533,0.30902,-0.85065),
            fm(0,0,-1)),
          new Triangle(
            fm(0.27639,-0.85065,-0.44721),
            fm(-0.26287,-0.80902,-0.52573),
            fm(0.16246,-0.5,-0.85065)),
          new Triangle(
            fm(-0.26287,-0.80902,-0.52573),
            fm(-0.72361,-0.52573,-0.44721),
            fm(-0.42533,-0.30902,-0.85065)),
          new Triangle(
            fm(0.16246,-0.5,-0.85065),
            fm(-0.26287,-0.80902,-0.52573),
            fm(-0.42533,-0.30902,-0.85065)),
          new Triangle(
            fm(0.16246,-0.5,-0.85065),
            fm(-0.42533,-0.30902,-0.85065),
            fm(0,0,-1)),
          new Triangle(
            fm(0.89443,-2.1907e-16,-0.44721),
            fm(0.68819,-0.5,-0.52573),
            fm(0.52573,-1.2877e-16,-0.85065)),
          new Triangle(
            fm(0.68819,-0.5,-0.52573),
            fm(0.27639,-0.85065,-0.44721),
            fm(0.16246,-0.5,-0.85065)),
          new Triangle(
            fm(0.52573,-1.2877e-16,-0.85065),
            fm(0.68819,-0.5,-0.52573),
            fm(0.16246,-0.5,-0.85065)),
          new Triangle(
            fm(0.52573,-1.2877e-16,-0.85065),
            fm(0.16246,-0.5,-0.85065),
            fm(0,0,-1)),
          new Triangle(
            fm(0.27639,0.85065,-0.44721),
            fm(0.68819,0.5,-0.52573),
            fm(0.16246,0.5,-0.85065)),
          new Triangle(
            fm(0.68819,0.5,-0.52573),
            fm(0.89443,-2.1907e-16,-0.44721),
            fm(0.52573,-1.2877e-16,-0.85065)),
          new Triangle(
            fm(0.16246,0.5,-0.85065),
            fm(0.68819,0.5,-0.52573),
            fm(0.52573,-1.2877e-16,-0.85065)),
          new Triangle(
            fm(0.16246,0.5,-0.85065),
            fm(0.52573,-1.2877e-16,-0.85065),
            fm(0,0,-1)),
          new Triangle(
            fm(-0.72361,0.52573,-0.44721),
            fm(-0.26287,0.80902,-0.52573),
            fm(-0.42533,0.30902,-0.85065)),
          new Triangle(
            fm(-0.26287,0.80902,-0.52573),
            fm(0.27639,0.85065,-0.44721),
            fm(0.16246,0.5,-0.85065)),
          new Triangle(
            fm(-0.42533,0.30902,-0.85065),
            fm(-0.26287,0.80902,-0.52573),
            fm(0.16246,0.5,-0.85065)),
          new Triangle(
            fm(-0.42533,0.30902,-0.85065),
            fm(0.16246,0.5,-0.85065),
            fm(0,0,-1)) };
            }

            GL.Begin(BeginMode.Triangles);
            {
                foreach (var t in _Sphere)
                {
                    t.Draw(radius);
                }
            }
            GL.End();
        }


        private static Vector3 fm(double a, double b, double c)
        {
            return new Vector3((float)a, (float)b, (float)c);
        }



        private struct Triangle
        {
            Vector3 v1, v2, v3;

            public Triangle(Vector3 vv1, Vector3 vv2, Vector3 vv3)
            {
                this.v1 = vv1;
                this.v2 = vv2;
                this.v3 = vv3;
            }

            public void Draw(float rad)
            {
                GL.Normal3(this.v1);
                GL.Vertex3(this.v1.X * rad, this.v1.Y * rad, this.v1.Z * rad);
                GL.Normal3(this.v2);
                GL.Vertex3(this.v2.X * rad, this.v2.Y * rad, this.v2.Z * rad);
                GL.Normal3(this.v3);
                GL.Vertex3(this.v3.X * rad, this.v3.Y * rad, this.v3.Z * rad);
            }
        }






















    }
}
