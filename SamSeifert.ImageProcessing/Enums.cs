using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public enum SectType
    { 
        RGB_Red,
        RGB_Green, 
        RGB_Blue, 
        
        Gray, 
        
        Hue,

        HSL_H = Hue, 
        HSL_S, 
        HSL_L,

        HSV_H = Hue, 
        HSV_S, 
        HSV_V,

        Hough_Foot_Of_Normal,
        Hough_Rho_Theta,

        NaN 
    };

    public enum DataType
    {
        Read,
        Write,
        ReadWrite,
    };

    public static class Extensions
    {
        public static bool isWrite(this DataType t)
        {
            return t == DataType.ReadWrite || t == DataType.Write;
        }
    }
}
