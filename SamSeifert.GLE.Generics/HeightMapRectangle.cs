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
    public class HeightMapRectangle : HeightMap
    {
        private float[,] _ByteHeights = null;
        private Vector3[] _VecsData = null;
        private uint[] _IntsTriangs = null;
        private bool _BoolUseText = true;

        private float scaleXZ = 0;
        private float scaleY = 0;

        private int w = 0, h = 0;

        private int _IntTexture = 0;

        private Image _BitmapHeightVals = null;
        private Image _BitmapTexture = null;

        public HeightMapRectangle(Image mapper, Image texture, float dim_sc, float height_sc)
        {
            this.scaleXZ = dim_sc;
            this.scaleY = height_sc;

            this._BitmapHeightVals = mapper;

            if (texture == null) this._BoolUseText = false;
            else this._BitmapTexture = texture;
        }

        public override void GLDelete()
        {
            if (this._IntTexture != 0) GL.DeleteTexture(this._IntTexture);
            if (this._IntIndicesBufferID != 0) GL.DeleteBuffers(1, ref this._IntIndicesBufferID);
            if (this._IntInterleaveBufferID != 0) GL.DeleteBuffers(1, ref this._IntInterleaveBufferID);

            this._IntTexture = 0;
            this._IntIndicesBufferID = 0;
            this._IntInterleaveBufferID = 0;
            
            this._BoolSetupGL4 = false;
        }

        public override void GLDraw()
        {
            if (this._IntTexture == 0 && this._BoolUseText) this._IntTexture = Textures.getGLTexture(this._BitmapTexture);
            else if (this._BoolSetupGL4)this.drawGL4();
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
            if (this._ByteHeights == null)
            {
                this.w = this._BitmapHeightVals.Width;
                this.h = this._BitmapHeightVals.Height;

                this._ByteHeights = new Single[this.h, this.w];

                using (Bitmap input = this._BitmapHeightVals as Bitmap)
                {
                    BitmapData bmd = input.LockBits(
                    new Rectangle(0, 0, this.w, this.h),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    Byte * row;
                    int xx = 0, x;

                    for (int y = 0; y < this.h; y++)
                    {
                        row = (Byte*)bmd.Scan0 + (y * bmd.Stride);

                        for (x = 0, xx = 0; x < this.w; x++, xx += 3)
                        {
                            this._ByteHeights[y, x] = (row[xx + 2] + row[xx + 1] + row[xx + 0]) / (255.0f * 3);
                        }
                    }

                    input.UnlockBits(bmd);
                }
            }

            int wm1 = this.w - 1;
            int hm1 = this.h - 1;
            int sqrs = wm1 * hm1;

            if (this._VecsData == null)
            {

                float wc = wm1 * this.scaleXZ;
                float hc = hm1 * this.scaleXZ;
                float wc2 = wc / 2;
                float hc2 = hc / 2;

                Vector3[,] verts = new Vector3[this.h, this.w];
                Vector3[,] norms = new Vector3[this.h, this.w];

                for (int z = 0; z < this.h; z++)
                {
                    for (int x = 0; x < this.w; x++)
                    {
                        verts[z,x] = new Vector3(
                            (z * this.scaleXZ) - wc2,
                            this._ByteHeights[z,x] * this.scaleY,
                            (x * this.scaleXZ) - hc2);
                    }
                }

                for (int z = 0; z < this.h; z++)
                {
                    for (int x = 0; x < this.w; x++)
                    {
                        var ls = new List<Vector3>();

                        bool xm = x > 0;
                        bool xp = x < wm1;

                        if (z > 0)
                        {
                            if (xm) ls.Add(verts[z - 1, x - 1]);
                            ls.Add(verts[z - 1, x]);
                            if (xp) ls.Add(verts[z - 1, x + 1]);
                        }

                        if (xm) ls.Add(verts[z, x - 1]);
                        ls.Add(verts[z, x]);
                        if (xp) ls.Add(verts[z, x + 1]);

                        if (z < hm1)
                        {
                            if (xm) ls.Add(verts[z + 1, x - 1]);
                            ls.Add(verts[z + 1, x]);
                            if (xp) ls.Add(verts[z + 1, x + 1]);
                        }

                        norms[z, x] = this.bestFitPlane(ls.ToArray());
                    }
                }


                var _VecsDataHolder = new List<Vector3>();
                var _IntsTriangsHolder = new List<uint>();

                Vector3 v1, v2, v3, v4;

                uint dexI = 0;
                for (int z = 0; z < hm1; z++)
                {
                    for (int x = 0; x < wm1; x++)
                    {
                        v1 = verts[z + 1, x];
                        v2 = verts[z, x];
                        v3 = verts[z + 1, x + 1];
                        v4 = verts[z, x+1];

                        if (v1.Y + v2.Y + v3.Y + v4.Y > 0.1f)
                        {
                            _VecsDataHolder.Add(v1);
                            if (this._BoolUseText) _VecsDataHolder.Add(new Vector3(0, 1, 0));
                            _VecsDataHolder.Add(norms[z + 1, x]);
                            _VecsDataHolder.Add(v2);
                            if (this._BoolUseText) _VecsDataHolder.Add(new Vector3(0, 0, 0));
                            _VecsDataHolder.Add(norms[z, x]);
                            _VecsDataHolder.Add(v3);
                            if (this._BoolUseText) _VecsDataHolder.Add(new Vector3(1, 1, 0));
                            _VecsDataHolder.Add(norms[z + 1, x + 1]);
                            _VecsDataHolder.Add(v4);
                            if (this._BoolUseText) _VecsDataHolder.Add(new Vector3(1, 0, 0));
                            _VecsDataHolder.Add(norms[z, x + 1]);

                            _IntsTriangsHolder.Add(dexI + 0);
                            _IntsTriangsHolder.Add(dexI + 1);
                            _IntsTriangsHolder.Add(dexI + 2);
                            _IntsTriangsHolder.Add(dexI + 2);
                            _IntsTriangsHolder.Add(dexI + 1);
                            _IntsTriangsHolder.Add(dexI + 3);
                            dexI += 4;
                        }
                    }
                }

                this._VecsData = _VecsDataHolder.ToArray();
                this._IntsTriangs = _IntsTriangsHolder.ToArray();
                this._IntElementCount = this._IntsTriangs.Length;
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
        






























        private int _IntInterleaveBufferID = 0;
        private int _IntIndicesBufferID = 0;
        private bool _BoolSetupGL4 = false;
        private int _IntElementCount = 0;

        private void drawGL4()
        {

            int stride = Vector3.SizeInBytes * (this._BoolUseText ? 3 : 2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _IntInterleaveBufferID);
            GL.VertexPointer(3, VertexPointerType.Float, stride, IntPtr.Zero);
            GL.NormalPointer(NormalPointerType.Float, stride, stride - Vector3.SizeInBytes);

            if (this._BoolUseText)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, this._IntTexture);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, stride, Vector3.SizeInBytes);
                GL.EnableClientState(ArrayCap.TextureCoordArray);
            }

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);
            GL.DrawElements(BeginMode.Triangles, _IntElementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

            if (this._BoolUseText)
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.Disable(EnableCap.Texture2D);
            }
        }
    }    
}
