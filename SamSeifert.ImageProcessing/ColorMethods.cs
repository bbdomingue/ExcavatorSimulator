
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public static class ColorMethods
    {
        /// <summary>
        /// HSL all on scale of 0 to 1
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void hsl2rgb(float h, float s, float l, out float r, out float g, out float b)
        {
            h *= 6;
            float c = s * (1 - Math.Abs(2 * l - 1));
            float x = c * (1 - Math.Abs(h % 2 - 1));
            float m = l - c / 2;

            float bx = Math.Min(1, Math.Max(0, (x + m)));
            float bc = Math.Min(1, Math.Max(0, (c + m)));
            float bm = Math.Min(1, Math.Max(0, m));

            switch ((int)h)
            {
                case 0:
                    {
                        r = bc;
                        g = bx;
                        b = bm;
                        break;
                    }
                case 1:
                    {
                        r = bx;
                        g = bc;
                        b = bm;
                        break;
                    }
                case 2:
                    {
                        r = bm;
                        g = bc;
                        b = bx;
                        break;
                    }
                case 3:
                    {
                        r = bm;
                        g = bx;
                        b = bc;
                        break;
                    }
                case 4:
                    {
                        r = bx;
                        g = bm;
                        b = bc;
                        break;
                    }
                default:
                    {
                        r = bc;
                        g = bm;
                        b = bx;
                        break;
                    }
            }
        }





        /// <summary>
        /// HSL scale 0 to 1, RGB scale 0 to 1
        /// </summary>
        /// <param name="c"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        public static void rgb2hsl(Color c, out float h, out float s, out float l)
        {
            ColorMethods.rgb2hsl(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, out h, out s, out l);
        }
        /// <summary>
        /// HSL scale 0 to 1, RGB scale 0 to 1
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        public static void rgb2hsl(float fr, float fg, float fb, out float h, out float s, out float l)
        {
            const float PIF = (float)Math.PI;
            const float PIF2 = PIF * 2;

            float hh = (float)(Math.Atan2(1.73205080757f * (fg - fb), 2 * fr - fg - fb) / PIF2);
            if (hh < 0) hh = 1 + hh;

            float mx = Math.Max(fr, Math.Max(fg, fb));
            float mn = Math.Min(fr, Math.Min(fg, fb));

            float ll = (mx + mn) / 2;
            float delta = mx - mn;

            h = hh;
            s = delta == 0 ? 0 : delta / (1 - Math.Abs(2 * ll - 1));
            l = ll;
        }









        /// <summary>
        /// HSV scale 0 to 1, RGB scale 0 to 1
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Color hsv2rgb(float h, float s, float v)
        {
            Color c;
            ColorMethods.hsv2rgb(h, s, v, out c);
            return c;
        }
        /// <summary>
        /// HSV scale 0 to 1, RGB scale 0 to 1
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <param name="c"></param>
        public static void hsv2rgb(float h, float s, float v, out Color c)
        {
            float r, g, b;
            ColorMethods.hsv2rgb(h, s, v, out r, out g, out b);
            c = Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }
        /// <summary>
        /// HSV scale 0 to 1, RGB scale 0 to 1
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void hsv2rgb(float h, float s, float v, out float r, out float g, out float b)
        {
            float c = v * s;
            float x = c * (1 - Math.Abs((h * 6) % 2 - 1));
            float m = v - c;

            switch ((int)(h * 6))
            {
                case 0:
                    r = c;
                    g = x;
                    b = 0;
                    break;
                case 1:
                    r = x;
                    g = c;
                    b = 0;
                    break;
                case 2:
                    r = 0;
                    g = c;
                    b = x;
                    break;
                case 3:
                    r = 0;
                    g = x;
                    b = c;
                    break;
                case 4:
                    r = x;
                    g = 0;
                    b = c;
                    break;
                default: 
                    r = c;
                    g = 0;
                    b = x;
                    break;                    
            }

            r += m;
            g += m;
            b += m;
        }


        /// <summary>
        /// HSV scale 0 to 1, RGB scale 0 to 1
        /// </summary>
        /// <param name="c"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        public static void rgb2hsv(Color c, out float h, out float s, out float v)
        {
            ColorMethods.rgb2hsv(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, out h, out s, out v);
        }
        /// <summary>
        /// HSV scale 0 to 1, RGB scale 0 to 1
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        public static void rgb2hsv(float fr, float fg, float fb, out float h, out float s, out float v)
        {
            const float PIF = (float)Math.PI;
            const float PIF2 = PIF * 2;

            float hh = (float)(Math.Atan2(1.73205080757f * (fg - fb), 2 * fr - fg - fb) / PIF2);
            if (hh < 0) hh = 1 + hh;

            float mx = Math.Max(fr, Math.Max(fg, fb));
            float mn = Math.Min(fr, Math.Min(fg, fb));

            h = hh;
            s = mx == 0 ? 0 : 1 - (mn / mx);
            v = mx;
        }
    }
}
