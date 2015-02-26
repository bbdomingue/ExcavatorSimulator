using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SamSeifert.GLE.CadViewer
{
    public class CadObject
    {
        public bool _Display = true;
        internal String _Name = "Untitled";

        internal float[] _Ambient = new float[] { 0f, 0f, 0f, 1.0f };
        internal float[] _Diffuse = new float[] { 0f, 0f, 0f, 1.0f };
        internal float[] _Emission = new float[] { 0f, 0f, 0f, 1.0f };
        internal float[] _Specular = new float[] { 0f, 0f, 0f, 1.0f };
        internal float[] _Shininess = new float[] { 0 }; // max 128

        internal CadObject[] _CadObjects = new CadObject[0];

        internal Vector3 _Translation = new Vector3();
        internal Vector3 _Rotation = new Vector3();

        private Vector3[] verts;
        private Vector3[] norms;

        private uint[] dices;
        private uint countStored;

        internal enum GLType { GL3, GL4, UNK };
        internal GLType _GLType = GLType.UNK;

        internal bool UseTranslationAndRotation = false;

        public CadObject(CadObject[] cos, String name = "Group")
        {
            this._CadObjects = cos;
            this._Name = name;
        }

        internal CadObject()
        {
        }

        internal CadObject(Vector3[] verts, Vector3[] norms, String name)
        {
            this._Name = name;
            this.setup(verts, norms);
        }

        public void setColor(System.Drawing.Color c, float a = 0.3f, float d = 0.6f, float e = 0, float s = 0)
        {
            this._Ambient[0] = a * c.R / 255.0f;
            this._Ambient[1] = a * c.G / 255.0f;
            this._Ambient[2] = a * c.B / 255.0f;

            this._Diffuse[0] = d * c.R / 255.0f;
            this._Diffuse[1] = d * c.G / 255.0f;
            this._Diffuse[2] = d * c.B / 255.0f;

            this._Emission[0] = e * c.R / 255.0f;
            this._Emission[1] = e * c.G / 255.0f;
            this._Emission[2] = e * c.B / 255.0f;

            this._Specular[0] = s * c.R / 255.0f;
            this._Specular[1] = s * c.G / 255.0f;
            this._Specular[2] = s * c.B / 255.0f;
        }

        public void setColor(float[] p1, float[] p2, float[] p3, float[] p4, float[] p5)
        {
            this._Ambient = p1;
            this._Diffuse = p2;
            this._Emission = p3;
            this._Specular = p4;
            this._Shininess = p5;
        }

        public override string ToString()
        {
            return this._Name + " " + this._GLType;
        }

        internal Vector3[] vertex { get { return this.verts; } }
        internal Vector3[] normal { get { return this.norms; } }
        internal uint[] indices { get { return this.dices; } }
        internal GLType type { get { return this._GLType; } }

        public void GLDelete()
        {
            if (this._CadObjects.Length > 0)
            {
                foreach (CadObject co in this._CadObjects) co.GLDelete();
            }
            else
            {
                if (this._BoolSetupGL3)
                {
                    this._BoolSetupGL3 = false;
                    GL.DeleteLists(this._IntList, 1);
                }

                if (this._BoolSetupGL4)
                {
                    this._BoolSetupGL4 = false;
                    GL.DeleteBuffers(1, ref this._IntIndicesBufferID);
                    GL.DeleteBuffers(1, ref this._IntInterleaveBufferID);
                    this._IntIndicesBufferID = 0;
                    this._IntInterleaveBufferID = 0;
                }
            }
        }

        internal void setup(Vector3[] v, Vector3[] n)
        {
            this.GLDelete();
            Vector3[] vs;
            Vector3[] ns;
            uint count;
            uint[] iss;

            if (StaticMethods.ConsolidateData(v, n, out count, out vs, out ns, out iss))
            {
                this._BoolSetupGL4 = this.setupGL4(count, vs, ns, iss);
            }

            if (this._BoolSetupGL4) this._GLType = GLType.GL4;
            else
            {
                this._BoolSetupGL3 = this.setupGL3(v, n);

                if (this._BoolSetupGL3) this._GLType = GLType.GL3;
                else this._GLType = GLType.UNK;
            }
        }

        internal void setup(uint count, Vector3[] v, Vector3[] n, uint[] indices)
        {
            this.GLDelete();

            this._BoolSetupGL4 = this.setupGL4(count, v, n, indices);

            if (this._BoolSetupGL4) this._GLType = GLType.GL4;
            else
            {
                Vector3[] vs;
                Vector3[] ns;
                uint counto;

                if (StaticMethods.ExpandData(v, n, indices, out counto, out vs, out ns))
                {
                    this._BoolSetupGL3 = this.setupGL3(v, n);
                }

                if (this._BoolSetupGL3) this._GLType = GLType.GL3;
                else this._GLType = GLType.UNK;

            }
        }


        public void draw(bool useColor = true)
        {
            if (this._Display)
            {
                if (this._CadObjects.Length > 0)
                {
                    foreach (CadObject co in this._CadObjects) co.draw(useColor);
                }
                else
                {
                    if (useColor)
                    {
                        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, this._Ambient);
                        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, this._Diffuse);
                        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, this._Emission);
                        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, this._Specular);
                        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, this._Shininess);
                    }

                    if (this.UseTranslationAndRotation)
                    {
                        GL.PushMatrix();
                        {
                            GL.Translate(this._Translation);
                            GL.Rotate(this._Rotation.X, Vector3.UnitX);
                            GL.Rotate(this._Rotation.Y, Vector3.UnitY);
                            GL.Rotate(this._Rotation.Z, Vector3.UnitZ);
                            this.drawPost();
                        }
                        GL.PopMatrix();
                    }
                    else this.drawPost();
                }
            }
        }

        private void drawPost()
        {
            switch (this._GLType)
            {
                case GLType.UNK:
                    {
                        break;
                    }
                case GLType.GL3:
                    {
                        if (this._BoolSetupGL3) this.drawGL3();
                        else if (this.setupGL3(this.verts, this.norms)) this._BoolSetupGL3 = true;
                        else this._GLType = GLType.UNK;
                        break;
                    }
                case GLType.GL4:
                    {
                        if (this._BoolSetupGL4) this.drawGL4();
                        else if (this.setupGL4(this.countStored, this.verts, this.norms, this.indices)) this._BoolSetupGL4 = true;
                        else this._GLType = GLType.UNK;
                        break;
                    }
            }
        }











        private int _IntList;
        private bool _BoolSetupGL3 = false;

        // GL Lists are Depracted in GL4
        private bool setupGL3(Vector3[] v, Vector3[] n)
        {
            bool _bool = v.Length == n.Length;

            this._IntList = GL.GenLists(1);
            GL.NewList(this._IntList, ListMode.Compile);
            {
                GL.Begin(BeginMode.Triangles);
                {
                    for (int i = 0; i < v.Length; i++)
                    {
                        if (_bool) GL.Normal3(n[i]);
                        GL.Vertex3(v[i]);
                    }
                }
                GL.End();
            }
            GL.EndList();

            this.verts = v;
            this.norms = n;

            return true;
        }

        private void drawGL3()
        {
            GL.CallList(this._IntList);
        }
























        private bool _BoolSetupGL4 = false;       
        private int _IntInterleaveBufferID;
        private int _IntIndicesBufferID;
        private int elementCount = 0;

        private bool setupGL4(uint count, Vector3[] verts, Vector3[] norms, uint[] triangs)
        {
            var idata = new Vector3[count * 2];
            for (int i = 0; i < count; i ++)
            {
                idata[i * 2] = verts[i];
                idata[i * 2 + 1] = norms[i];
            }

            int bufferSize;
            int bufferSizeE;

            bufferSizeE = idata.Length * Vector3.SizeInBytes;
            GL.GenBuffers(1, out this._IntInterleaveBufferID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._IntInterleaveBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferSizeE), idata, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (bufferSizeE != bufferSize) return false;
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            bufferSizeE = triangs.Length * sizeof(uint);
            GL.GenBuffers(1, out _IntIndicesBufferID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSizeE), triangs, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (bufferSizeE != bufferSize) return false;
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        
            elementCount = triangs.Length;

            this.verts = verts;
            this.norms = norms;
            this.dices = triangs;
            this.countStored = count;

            return true;
        }

        private void drawGL4()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _IntInterleaveBufferID);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes * 2, IntPtr.Zero);
            GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes * 2, Vector3.SizeInBytes);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);
            GL.DrawElements(BeginMode.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }










        public void swapXZ()
        {
            this.GLDelete();

            float f = 0;
            int count = this.verts.Length;

            for (int i = 0; i < count; i++)
            {
                f = this.verts[i].X;
                this.verts[i].X = this.verts[i].Z;
                this.verts[i].Z = f;
            }
        }
    }
}
