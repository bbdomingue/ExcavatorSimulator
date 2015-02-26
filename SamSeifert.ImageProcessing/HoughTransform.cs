using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public static class HoughTransform
    {
        /// Creates a new ImageData class
        public static ToolboxReturn FootOfNormal_O(ref ImageData inp, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                var sz = new Size(inp.Size.Width * 2 - 1, inp.Size.Height * 2 - 1);
                outp = new ImageData(sz, inp.Size);
                return HoughTransform.FootOfNormal_R(ref inp, ref outp);
            }
        }
        /// Modifies an existing ImageData class
        public static ToolboxReturn FootOfNormal_R(ref ImageData inp, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return HoughTransform.FootOfNormal_O(ref inp, out outp);
            if (inp.Size != outp.SizeOriginal) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                var st = outp.getSect(SectType.Hough_Foot_Of_Normal, DataType.Write);

                var dt = st._Data;

                int hough_h = dt.GetLength(0);
                int hough_w = dt.GetLength(1);

                st.setValue(0);

                var valsX = new Single[,]
                {
                    {-1,  0,  1},
                    {-2,  0,  2},
                    {-1,  0,  1},
                };

                var valsY = new Single[,]
                {
                    {-1, -2, -1},
                    { 0,  0,  0},
                    { 1,  2,  1},
                };

                const int dim = 3;

                Single v;
                int x0, y0;

                int xm1 = inp.Size.Width - 1;
                int ym1 = inp.Size.Height - 1;
                int xm2 = outp.Size.Width;
                int ym2 = outp.Size.Height;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    var inpArray = inSect._Data;

                    int ty;
                    float val;
                    float gx;
                    float gy;

                    for (int y = 1; y < ym1; y++)
                    {
                        for (int x = 1; x < xm1; x++)
                        {
                            gx = 0;
                            gy = 0;

                            for (int i = 0; i < dim; i++)
                            {
                                ty = y + i - 1;
                                for (int j = 0; j < dim; j++)
                                {
                                    val = inpArray[ty, x + j - 1];
                                    gx += val * valsX[i, j];
                                    gy += val * valsY[i, j];
                                }
                            }

                            val = (gx * gx + gy * gy);

                            if (val != 0)
                            {
                                v = (x * gx + y * gy) / val;

                                x0 = (int)Math.Round(v * gx + xm1);
                                y0 = (int)Math.Round(v * gy + ym1);

                                if (x0 > 0 && x0 < xm2 && y0 > 0 && y0 < ym2)
                                {
                                    dt[y0, x0] += (Single)Math.Sqrt(val);
                                }
                            }
                        }
                    }

                    Single min = st.min;
                    Single range = st.max - min;


                    st.add(-min);                    
                    if (range != 0) st.multiply(1 / range);
                }

                return ToolboxReturn.Good;
            }
        }
        /// Modifies an input ImageData
        public static ToolboxReturn FootOfNormal_S(ref ImageData inp)
        {
            return HoughTransform.FootOfNormal_R(ref inp, ref inp);
        }





        public static Size rhoThetaSizeForSize(Size inp)
        {
            var sz = new Size(180, (int)Math.Ceiling(Math.Sqrt(inp.Width * inp.Width + inp.Height * inp.Height)));
            sz.Height = 1 + 2 * sz.Height;
            return sz;
        }

        /// Creates a new ImageData class
        public static ToolboxReturn RhoTheta_O(ref ImageData inp, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                var sz = rhoThetaSizeForSize(inp.Size);
                outp = new ImageData(sz, sz);
                return HoughTransform.RhoTheta_R(ref inp, ref outp);
            }
        }
        /// Modifies an existing ImageData class
        public static ToolboxReturn RhoTheta_R(ref ImageData inp, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return HoughTransform.RhoTheta_O(ref inp, out outp);
            if (rhoThetaSizeForSize(inp.Size) != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                var st = outp.getSect(SectType.Hough_Rho_Theta, DataType.Write);
                var dt = st._Data;

                st.setValue(0);

                var valsX = new Single[,]
                {
                    {-1,  0,  1},
                    {-2,  0,  2},
                    {-1,  0,  1},
                };

                var valsY = new Single[,]
                {
                    {-1, -2, -1},
                    { 0,  0,  0},
                    { 1,  2,  1},
                };

                const int dim = 3;

                Single v;
                Single x0, y0;
                Single gx, gy;
                Single val;
                Single val_sq;
                int rho;
                int theta;

                int xm1 = inp.Size.Width - 1;
                int ym1 = inp.Size.Height - 1;

                int ym2 = outp.Size.Height;
                int ym2z = (ym2 - 1) / 2;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    var inpArray = inSect._Data;

                    int ty;

                    for (int y = 1; y < ym1; y++)
                    {
                        for (int x = 1; x < xm1; x++)
                        {
                            gx = 0;
                            gy = 0;

                            for (int i = 0; i < dim; i++)
                            {
                                ty = y + i - 1;
                                for (int j = 0; j < dim; j++)
                                {
                                    val = inpArray[ty, x + j - 1];
                                    gx += val * valsX[i, j];
                                    gy += val * valsY[i, j];
                                }
                            }

                            val = (gx * gx + gy * gy);

                            if (val != 0)
                            {
                                val_sq = (Single)Math.Sqrt(val);

                                v = (x * gx + y * gy) / val;
                                x0 = (v * gx);
                                y0 = (v * gy);     
                        
                                rho = (int)Math.Round(Math.Sqrt(x0 * x0 + y0 * y0));
                                theta = (int)Math.Round(Math.Atan2(y0, x0) * 180 / Math.PI);
                                if (theta < 0)
                                {
                                    theta += 180;
                                    rho *= -1;
                                }
                                else if (theta > 179) theta -= 180;

                                
                                dt[rho + ym2z, theta] += val_sq;
                            }
                        }
                    }

                    Single min = st.min;
                    Single range = st.max - min;

                    st.add(-min);
                    if (range != 0) st.multiply(1 / range);
                }

                return ToolboxReturn.Good;
            }
        }
        /// Modifies an input ImageData
        public static ToolboxReturn RhoTheta_S(ref ImageData inp)
        {
            return HoughTransform.FootOfNormal_R(ref inp, ref inp);
        }
    }
}
