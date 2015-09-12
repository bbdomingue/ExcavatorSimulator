using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;

namespace SamSeifert.GLE
{
    public class Color_GL
    {
        public float[] _Ambient = new float[] { 0f, 0f, 0f, 1.0f };
        public float[] _Diffuse = new float[] { 0f, 0f, 0f, 1.0f };
        public float[] _Emission = new float[] { 0f, 0f, 0f, 1.0f };
        public float[] _Specular = new float[] { 0f, 0f, 0f, 1.0f };        
        public float[] _Shininess = new float[] { 0 }; // max 128

        public Color_GL()
        {
        }

        public Color_GL(System.Drawing.Color c)
        {
            this.setColor(c);
        }

        public void SendToGL()
        {
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, this._Ambient);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, this._Diffuse);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, this._Emission);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, this._Specular);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, this._Shininess);
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

    }
}
