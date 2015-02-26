using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;


using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Excavator
{
    internal static class StaticMethods
    {

        internal const float _PIF = (float)Math.PI;
        internal const double _PID = Math.PI;

        internal static float toRadiansF(float f)
        {
            return f * StaticMethods._PIF / 180.0f;
        }

        internal static float toDegreesF(float f)
        {
            return f * 180.0f / StaticMethods._PIF;
        }

        internal static float toDegreesF(double f)
        {
            return (float)(f * 180.0 / StaticMethods._PID);
        }

        internal static double toRadiansD(double f)
        {
            return f * StaticMethods._PID / 180.0f;
        }

        internal static double toDegreesD(double f)
        {
            return f * 180.0f / StaticMethods._PID;
        }

        internal static Color getRandomColorForIndex(int index)
        {
            switch (index)
            {
                case 0: return Color.Yellow; 
                case 1: return Color.Red; 
                case 2: return Color.Green; 
                case 3: return Color.Blue; 
                case 4: return Color.Purple; 
                case 5: return Color.SkyBlue; 
                case 6: return Color.Aqua; 
                default: return Color.Black; 
            }
        }





















        internal static void Rotate(float val, Vector3 v)
        {
            GL.Rotate(val, v);
            if (StaticMethods._BoolTextureAlso)
            {
                GL.MatrixMode(StaticMethods._BoolMatrixModeModelTexture ? MatrixMode.Texture : MatrixMode.Modelview);
                GL.Rotate(val, v);
                StaticMethods._BoolMatrixModeModelTexture = !StaticMethods._BoolMatrixModeModelTexture;
            }
        }

        internal static void Rotate(float a, float x, float y, float z)
        {
            GL.Rotate(a, x, y, z);
            if (StaticMethods._BoolTextureAlso)
            {
                GL.MatrixMode(StaticMethods._BoolMatrixModeModelTexture ? MatrixMode.Texture : MatrixMode.Modelview);
                GL.Rotate(a, x, y, z);
                StaticMethods._BoolMatrixModeModelTexture = !StaticMethods._BoolMatrixModeModelTexture;
            }
        }

        internal static void Translate(float x, float y, float z)
        {
            GL.Translate(x, y, z);
            if (StaticMethods._BoolTextureAlso)
            {
                GL.MatrixMode(StaticMethods._BoolMatrixModeModelTexture ? MatrixMode.Texture : MatrixMode.Modelview);
                GL.Translate(x, y, z);
                StaticMethods._BoolMatrixModeModelTexture = !StaticMethods._BoolMatrixModeModelTexture;
            }
        }

        internal static void Translate(Vector3 v)
        {
            GL.Translate(v);
            if (StaticMethods._BoolTextureAlso)
            {
                GL.MatrixMode(StaticMethods._BoolMatrixModeModelTexture ? MatrixMode.Texture : MatrixMode.Modelview);
                GL.Translate(v);
                StaticMethods._BoolMatrixModeModelTexture = !StaticMethods._BoolMatrixModeModelTexture;
            }
        }

        internal static void PushMatrix()
        {
            GL.PushMatrix();
            if (StaticMethods._BoolTextureAlso)
            {
                GL.MatrixMode(StaticMethods._BoolMatrixModeModelTexture ? MatrixMode.Texture : MatrixMode.Modelview);
                GL.PushMatrix();
                StaticMethods._BoolMatrixModeModelTexture = !StaticMethods._BoolMatrixModeModelTexture;
            }
        }

        internal static void PopMatrix()
        {
            GL.PopMatrix();
            if (StaticMethods._BoolTextureAlso)
            {
                GL.MatrixMode(StaticMethods._BoolMatrixModeModelTexture ? MatrixMode.Texture : MatrixMode.Modelview);
                GL.PopMatrix();
                StaticMethods._BoolMatrixModeModelTexture = !StaticMethods._BoolMatrixModeModelTexture;
            }
        }

        internal static bool _BoolTextureAlso;
        internal static bool _BoolMatrixModeModelTexture;

    }
}
