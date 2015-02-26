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

        public const float Conversion_Meters_To_Inches = 39.3701f;
        public const float Conversion_Inches_To_Meters = 1 / Conversion_Meters_To_Inches;




        internal static float toRadiansF(float f)
        {
            return f * StaticMethods._PIF / 180;
        }

        internal static float toRadiansF(double d)
        {
            return StaticMethods.toRadiansF((float)d);
        }



        internal static double toRadiansD(int f)
        {
            return StaticMethods.toRadiansD((double)f);
        }

        internal static double toRadiansD(float f)
        {
            return StaticMethods.toRadiansD((double)f);
        }

        internal static double toRadiansD(double d)
        {
            return d * StaticMethods._PID / 180;
        }

        internal static double toRadiansD(decimal m)
        {
            return StaticMethods.toRadiansD((double)m);
        }









        internal static float toDegreesF(float f)
        {
            return f * 180 / StaticMethods._PIF;
        }

        internal static float toDegreesF(double d)
        {
            return StaticMethods.toDegreesF((float)d); ;
        }



        internal static double toDegreesD(float f)
        {
            return StaticMethods.toDegreesD((double)f);
        }

        internal static double toDegreesD(double d)
        {
            return d * 180 / StaticMethods._PID;
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

        public static void setNudValue(System.Windows.Forms.NumericUpDown nud, float val)
        {
            StaticMethods.setNudValue(nud, (decimal)val);
        }

        public static void setNudValue(System.Windows.Forms.NumericUpDown nud, decimal val)
        {
            nud.Value = Math.Max(nud.Minimum, Math.Min(nud.Maximum, val));
        }




    }
}
