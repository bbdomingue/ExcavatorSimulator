using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SamSeifert.GLE
{
    public class HeightMapCircle : HeightMap
    {
        private int _IntInterleaveBufferID = 0;
        private int _IntIndicesBufferID = 0;
        private bool _BoolSetupGL4 = false;
        private int _IntNumTriangles = 0;




        private Vector3[] _VecsData = null;

        private uint[] _IntsTriangs = null;
        
        private Bitmap _BitmapHeightVals = null;

        private readonly float scaleHeight;
        private readonly float scaleDim;
        private float offsetHeight = -1.0f;

        private readonly int incDegrees;
        private readonly int countDegrees;
        private readonly int countRadius;
        private readonly float startRadius;

        public HeightMapCircle(Bitmap mapper,
            float ScaleHeight,
            float ScaleDim,
            int DegreesPerSweep = 4, 
            int RadiusSections = 200, 
            float StartRadius = 0)
        {
            this.scaleDim = ScaleDim;
            this.scaleHeight = ScaleHeight;

            this.incDegrees = DegreesPerSweep;
            this.countRadius = RadiusSections;
            this.startRadius = StartRadius;
            this._BitmapHeightVals = mapper;
            this.countDegrees = 360 / this.incDegrees;
        }

        public override void GLDelete()
        {
            if (this._IntIndicesBufferID != 0) GL.DeleteBuffers(1, ref this._IntIndicesBufferID);
            if (this._IntInterleaveBufferID != 0) GL.DeleteBuffers(1, ref this._IntInterleaveBufferID);

            this._IntIndicesBufferID = 0;
            this._IntInterleaveBufferID = 0;

            this._BoolSetupGL4 = false;
        }

        public override void GLDraw()
        {
            if (this._BoolSetupGL4)
            {
                int stride = Vector3.SizeInBytes * 2;

                GL.BindBuffer(BufferTarget.ArrayBuffer, _IntInterleaveBufferID);
                GL.VertexPointer(3, VertexPointerType.Float, stride, IntPtr.Zero);
                GL.NormalPointer(NormalPointerType.Float, stride, stride - Vector3.SizeInBytes);

                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.NormalArray);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);

                GL.DrawElements(BeginMode.Triangles, this._IntNumTriangles * 3, DrawElementsType.UnsignedInt, 0);
            }
            else this.GenerateHeightMap();
        }

        public override void GLDraw(float DirectionInDegrees, int dAngle) // Direction in Degrees;
        {
            if (this._BoolSetupGL4)
            {
                int stride = Vector3.SizeInBytes * 2;

                GL.BindBuffer(BufferTarget.ArrayBuffer, _IntInterleaveBufferID);
                GL.VertexPointer(3, VertexPointerType.Float, stride, IntPtr.Zero);
                GL.NormalPointer(NormalPointerType.Float, stride, stride - Vector3.SizeInBytes);

                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.NormalArray);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);

                int did = (int)(DirectionInDegrees - dAngle);

                int dAngle2 = dAngle * 2;

                while (did < 0) did += 360;
                while (did > 360) did -= 360;

                int stripL = (this.countRadius - 1) * 6;
                int stripLP = stripL * 4;

                if (true)
                {
                    if (did + dAngle2 < 360)
                    {
                        GL.DrawRangeElements(
                            BeginMode.Triangles,
                            0,
                            this._IntNumTriangles * 3,
                            stripL * (dAngle2 / this.incDegrees),
                            DrawElementsType.UnsignedInt,
                            new IntPtr(stripLP * (did / this.incDegrees)));
                    }
                    else
                    {
                        int lens = did + dAngle2 - 360;
                        lens /= this.incDegrees;
                        lens *= this.incDegrees;

                        GL.DrawRangeElements(
                            BeginMode.Triangles,
                            0,
                            this._IntNumTriangles * 3,
                            stripL * (lens / this.incDegrees),
                            DrawElementsType.UnsignedInt,
                            new IntPtr(0));

                        GL.DrawRangeElements(
                            BeginMode.Triangles,
                            0,
                            this._IntNumTriangles * 3,
                            stripL * ((dAngle2 - lens) / this.incDegrees),
                            DrawElementsType.UnsignedInt,
                            new IntPtr(stripLP * (did / this.incDegrees)));

                    }
                }
                else
                {
                    for (int i = 0; i < countDegrees; i += 2)
                    {
                        GL.DrawRangeElements(
                            BeginMode.Triangles,
                            0,
                            this._IntNumTriangles * 3,
                            stripL,
                            DrawElementsType.UnsignedInt,
                            new IntPtr(stripLP * i));
                    }
                }



            }
            else this.GenerateHeightMap();
        }




        private Vector3 bestFitPlane(Vector3[] vs)
        {
            Vector3 avgs = Vector3.Zero;

            foreach (Vector3 v in vs) avgs = Vector3.Add(v, avgs);

            avgs /= vs.Length;

            var zerod = new Vector3[vs.Length];

            for (int i = 0; i < vs.Length; i++) zerod[i] = Vector3.Subtract(avgs, vs[i]);

            // a b c
            // d e f
            // g h i
            Vector4 row1 = new Vector4(0, 0, 0, 0);
            Vector4 row2 = new Vector4(0, 0, 0, 0);
            Vector4 row3 = new Vector4(0, 0, 0, 0);
            Vector4 row4 = new Vector4(0, 0, 0, 1);

            Vector4 b = new Vector4(0, 0, 0, 1);

            foreach (Vector3 v in zerod)
            {
                row1.X += v.X * v.X;
                row1.Y += v.X * v.Z;
                row1.Z += v.X;

                row2.X += v.Z * v.X;
                row2.Y += v.Z * v.Z;
                row2.Z += v.Z;

                row3.X += v.X;
                row3.Y += v.Z;
                row3.Z += 1;

                b.X += v.Y * v.X;
                b.Y += v.Y * v.Z;
                b.Z += v.Y;
            }

            Matrix4 mat = new Matrix4(row1, row2, row3, row4);
            mat.Invert();

            Vector4 result;
            Vector4.Transform(ref b, ref mat, out result);
            Vector3 ret = new Vector3(result.X, -1, result.Y);

            ret.Normalize();

            if (ret.Y < 0) ret *= -1;

            return ret;
        }


































        unsafe void GenerateHeightMap()
        {
            if (this._VecsData == null)
            {
                Vector3[,] verts = new Vector3[countDegrees, countRadius];

                RectangleF sz = HeightMapCircle.fitAinB(new Size(1, 1), this._BitmapHeightVals.Size);
                sz.X += sz.Width / 2;
                sz.Y += sz.Height / 2;

                Double imageRadius = (sz.Width + sz.Height) / (4.1f); // Plus .1 keeps it inside bitmap

                float srsq = (float)Math.Sqrt(this.startRadius);
                float srsq1m = 1 - srsq;

                for (int i = 0; i < countDegrees; i++)
                {
                    int degrees = i * incDegrees;
                    Double radians = degrees * (Math.PI / 180);
                    Double cos = Math.Cos(radians);
                    Double sin = Math.Sin(radians);

                    Single cosf = (Single) cos;
                    Single sinf = (Single) sin;

                    for (int j = 0; j < countRadius; j++)
                    {
                        float sc = srsq + (srsq1m * j) / (countRadius - 1);
                        sc *= sc;

                        Double radius = sc * imageRadius;
                        float radiusWorld = this.scaleDim * sc;

                        if (j == 0 && this.startRadius != 0)
                        {
                            verts[i, j] = new Vector3(
                                (sinf * radiusWorld),
                                this.offsetHeight,
                                (cosf * radiusWorld));
                        }
                        else
                        {

                            Double xAdj = sz.X + radius * sin;
                            Double xAdj2 = xAdj % 1;

                            Double yAdj = sz.Y + radius * cos;
                            Double yAdj2 = yAdj % 1;

                            int yUp = (int)Math.Ceiling(yAdj);
                            int yDown = (int)yAdj;

                            int xUp = (int)Math.Ceiling(xAdj);
                            int xDown = (int)xAdj;

                            Color c;

                            if (xUp == xDown && yUp == yDown)
                            {
                                c = this._BitmapHeightVals.GetPixel(xUp, yUp);
                            }
                            else if (xUp == xDown)
                            {
                                c = this.getLinearEstimate(yDown, xUp, yUp, xUp, yAdj2);
                            }
                            else if (yUp == yDown)
                            {
                                c = this.getLinearEstimate(yUp, xDown, yUp, xUp, xAdj2);
                            }
                            else
                            {
                                c = this.getLinearEstimate(
                                    this.getLinearEstimate(yDown, xDown, yUp, xDown, yAdj2),
                                    this.getLinearEstimate(yDown, xUp, yUp, xUp, yAdj2),
                                    xAdj2);
                            }

                            verts[i, j] = new Vector3(
                                (sinf * radiusWorld),
                                ((c.R + c.G + c.B) / (255.0f * 3)) * this.scaleHeight + this.offsetHeight,
                                (cosf * radiusWorld));
                        }
                    }

                }

                this._BitmapHeightVals = null;


                if (true)
                {
                    Vector3[,] vertsTemp = new Vector3[countDegrees, countRadius];

                    for (int kk = 0; kk < 10; kk++)
                    {
                        Array.Copy(verts, vertsTemp, verts.Length);

                        for (int i = 0; i < countDegrees; i++)
                        {
                            int im1 = (i + countDegrees - 1) % countDegrees;
                            int ip1 = (i + countDegrees + 1) % countDegrees;

                            for (int j = 1; j + 1 < countRadius; j++)
                            {
                                verts[i, j].Y = (
                                    vertsTemp[ip1, j + 1].Y +
                                    vertsTemp[ip1, j + 0].Y +
                                    vertsTemp[ip1, j - 1].Y +
                                    vertsTemp[i, j + 1].Y +
                                    vertsTemp[i, j + 0].Y +
                                    vertsTemp[i, j - 1].Y +
                                    vertsTemp[im1, j + 1].Y +
                                    vertsTemp[im1, j + 0].Y +
                                    vertsTemp[im1, j - 1].Y
                                    ) / 9.0f;
                            }

                            verts[i, countRadius - 1].Y = vertsTemp[i, countRadius - 2].Y;
                        }
                    }
                }

                Vector3[,] norms = new Vector3[countDegrees, countRadius];


                for (int i = 0; i < countDegrees; i++)
                {
                    for (int j = 0; j < countRadius; j++)
                    {
                        var ls = new List<Vector3>();

                        int iL = (i + countDegrees - 1) % countDegrees;
                        int iR = (i + countDegrees + 1) % countDegrees;

                        if (j > 0)
                        {
                            ls.Add(verts[iL, j - 1]);
                            ls.Add(verts[i, j - 1]);
                            ls.Add(verts[iR, j - 1]);
                        }

                        ls.Add(verts[iL, j]);
                        ls.Add(verts[i, j]);
                        ls.Add(verts[iR, j]);

                        if (j + 1 < countRadius)
                        {
                            ls.Add(verts[iL, j + 1]);
                            ls.Add(verts[i, j + 1]);
                            ls.Add(verts[iR, j + 1]);
                        }

                        norms[i, j] = this.bestFitPlane(ls.ToArray());
                    }
                }

                var _VecsDataHolder = new List<Vector3>();

                for (int i = 0; i < countDegrees; i++)
                {
                    for (int j = 0; j < countRadius; j++)
                    {
                        _VecsDataHolder.Add(verts[i, j]);
                        _VecsDataHolder.Add(norms[i, j]);
                    }
                }

                var _IntsTriangsHolder = new List<uint>();

                this._IntNumTriangles = 0;

                for (int i = 0; i < countDegrees; i++)
                {
                    for (int j = 0; j < countRadius - 1; j++)
                    {
                        int ip1 = (i + countDegrees + 1) % countDegrees;

                        uint i0 = (uint)(ip1 * countRadius + j);
                        uint i1 = (uint)(i * countRadius + j);
                        uint i2 = (uint)(ip1 * countRadius + j + 1);
                        uint i3 = (uint)(i * countRadius + j + 1);

                        _IntsTriangsHolder.Add(i0);
                        _IntsTriangsHolder.Add(i1);
                        _IntsTriangsHolder.Add(i2);
                        _IntsTriangsHolder.Add(i2);
                        _IntsTriangsHolder.Add(i1);
                        _IntsTriangsHolder.Add(i3);

                        this._IntNumTriangles += 2;
                    }
                }

                this._VecsData = _VecsDataHolder.ToArray();
                this._IntsTriangs = _IntsTriangsHolder.ToArray();
            }

            int bufferSizeE;

            bufferSizeE = _VecsData.Length * Vector3.SizeInBytes;
            GL.GenBuffers(1, out this._IntInterleaveBufferID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._IntInterleaveBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferSizeE), _VecsData, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            bufferSizeE = this._IntsTriangs.Length * sizeof(uint);
            GL.GenBuffers(1, out _IntIndicesBufferID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSizeE), this._IntsTriangs, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            this._BoolSetupGL4 = true;
        }



        private Color getLinearEstimate(int x1, int y1, int x2, int y2, Double p)
        {
            return this.getLinearEstimate(
                this._BitmapHeightVals.GetPixel(x1, y1),
                this._BitmapHeightVals.GetPixel(x2, y2),
                p);
        }

        private Color getLinearEstimate(Color y0, Color y1, Double p)
        {
            return Color.FromArgb(
                (int)(y0.R + p + (y1.R - y0.R)),
                (int)(y0.G + p + (y1.G - y0.G)),
                (int)(y0.B + p + (y1.B - y0.B)));
        }





        public static RectangleF fitAinB(Size A, Size B)
        {
            RectangleF _Rectangle = new RectangleF();

            if (A.Width * A.Height == 0) return _Rectangle;

            if (A.Width * B.Height > B.Width * A.Height)
            {
                // image wider than screen
                _Rectangle.Width = B.Width;
                _Rectangle.Height = (B.Width *  A.Height) / ((float)(A.Width));
                _Rectangle.Y = (B.Height - _Rectangle.Height) / 2.0f;
            }
            else
            {
                // image taller than screen
                _Rectangle.Width = (B.Height * A.Width) / ((float)(A.Height));
                _Rectangle.Height = B.Height;
                _Rectangle.X = (B.Width - _Rectangle.Width) / 2.0f;
            }

            return _Rectangle;
        }























    }
}
