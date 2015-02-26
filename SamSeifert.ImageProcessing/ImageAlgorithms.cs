using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public enum ToolboxReturn { Good, NullInput, ImageSizeMismatch, SpecialError };

    public static class ImageAlgorithms
    {
        public static int cast(float f)
        {
            return Math.Max(-255, Math.Min(255, (int)Math.Round(f, 0)));
        }

        /*
        /// Creates a new ImageData class
        public static ToolboxReturn XXX_O(ref ImageData inp, params, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageToolbox.XXX_R(ref inp, ref outp);
            }
        }
        /// Modifies an existing ImageData class
        public static ToolboxReturn XXX_R(ref ImageData inp, params, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageToolbox.XXX_O(ref inp, vals, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
            }
        }
        /// Modifies an input ImageData
        public static ToolboxReturn XXX_S(ref ImageData inp, params)
        {
        }
        */
































        private static T[,] Covert2Rectangle<T>(T[][] arrays)
        {
            // TODO: Validation and special-casing for arrays.Count == 0
            int minorLength = arrays[0].Length;
            T[,] ret = new T[arrays.Length, minorLength];
            for (int i = 0; i < arrays.Length; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                {
                    return null;
                }
                for (int j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }


        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="outp"></param>
        /// <param name="vals"></param>
        /// <param name="mult"></param>
        public static ToolboxReturn Convolute_O(ref ImageData inp, Single[][] vals, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (vals == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageAlgorithms.Convolute_R(ref inp, ImageAlgorithms.Covert2Rectangle<Single>(vals), ref outp);
            }
        }





        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="outp"></param>
        /// <param name="vals"></param>
        /// <param name="mult"></param>
        public static ToolboxReturn Convolute_O(ref ImageData inp, Single[,] vals, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (vals == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageAlgorithms.Convolute_R(ref inp, vals, ref outp);
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="outp"></param>
        /// <param name="vals"></param>
        /// <param name="mult"></param>
        public static ToolboxReturn Convolute_R(ref ImageData inp, Single[,] vals, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (vals == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.Convolute_O(ref inp, vals, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                int rows = vals.GetLength(0);
                int cols = vals.GetLength(1);

                int c2 = cols / 2;
                int r2 = rows / 2;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    Sect outSect = outp.getSect(inSect._Type, DataType.Write);

                    var inpArray = inSect._Data;
                    var outArray = outSect._Data;

                    int tx, ty;
                    float sum;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            sum = 0;
                            for (int i = 0; i < rows; i++)
                            {
                                ty = y + i - r2;
                                if (ty >= 0 && ty < h)
                                {
                                    for (int j = 0; j < cols; j++)
                                    {
                                        tx = x + j - c2;
                                        if (tx >= 0 && tx < w)
                                        {
                                            sum += inpArray[ty, tx] * vals[i, j];
                                        }
                                    }
                                }
                            }
                            outArray[y, x] = sum;
                        }
                    }
                }

                return ToolboxReturn.Good;
            }
        }

        /// <summary>
        /// Modifies an input ImageData
        /// </summary>
        /// <param name="data"></param>
        /// <param name="vals"></param>
        /// <param name="mult"></param>
        public static ToolboxReturn Convolute_S(ref ImageData data, Single[, ] vals)
        {
            throw new NotImplementedException();
        }


















        /// <summary>
        /// Modifies an input ImageData
        /// </summary>
        /// <param name="data"></param>
        /// <param name="p"></param>
        public static ToolboxReturn AddConstant_S(ref ImageData data, Single p)
        {
            if (data == null) return ToolboxReturn.NullInput;
            else return ImageAlgorithms.AddConstant_R(ref data, p, ref data);
        }

        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="outp"></param>
        /// <param name="p"></param>
        public static ToolboxReturn AddConstant_O(ref ImageData inp, Single p, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageAlgorithms.AddConstant_R(ref inp, p, ref outp);
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="outp"></param>
        /// <param name="p"></param>
        public static ToolboxReturn AddConstant_R(ref ImageData inp, Single p, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.AddConstant_O(ref inp, p, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    Sect outSect = outp.getSect(inSect._Type, DataType.Write);

                    var inpArray = inSect._Data;
                    var outArray = outSect._Data;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            outArray[y, x] = inpArray[y, x] + p;
                        }
                    }
                }

                return ToolboxReturn.Good;
            }
        }








        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp1"></param>
        /// <param name="inp2"></param>
        /// <param name="outp"></param>
        public static ToolboxReturn Add_O(ref ImageData inp1, ref ImageData inp2, out ImageData outp)
        {
            if (inp1 == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inp2 == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inp1.Size != inp2.Size)
            {
                outp = null;
                return ToolboxReturn.ImageSizeMismatch;
            }
            else
            {
                Size s = inp1.SizeOriginal == inp2.SizeOriginal ? inp1.SizeOriginal : inp1.Size;
                outp = new ImageData(inp1.Size, s);
                return ImageAlgorithms.Add_R(ref inp1, ref inp2, ref outp);
            }        
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp1"></param>
        /// <param name="inp2"></param>
        /// <param name="outp"></param>
        public static ToolboxReturn Add_R(ref ImageData inp1, ref ImageData inp2, ref ImageData outp)
        {
            if (inp1 == null) return ToolboxReturn.NullInput;
            else if (inp2 == null) return ToolboxReturn.NullInput;
            else return ImageAlgorithms.Add_R(new ImageData[] { inp1, inp2 }, new bool[] { true, true }, ref outp);
        }




        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp1"></param>
        /// <param name="inp2"></param>
        /// <param name="outp"></param>
        public static ToolboxReturn Add_O(ImageData[] inps, Boolean[] signs, Single p, out ImageData outp)
        {
            if (inps == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (signs == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inps.Length != signs.Length)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else if (inps.Length * signs.Length == 0)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                bool first = true;
                bool bso = true;

                Size s = Size.Empty;
                Size so = Size.Empty;
                var dict = new Dictionary<SectType, Boolean>();

                foreach (ImageData dat in inps)
                {
                    if (first)
                    {
                        s = dat.Size;
                        so = dat.SizeOriginal;
                        first = false;
                        foreach (Sect ect in dat.getSects(DataType.Read)) dict[ect._Type] = true;
                    }
                    else
                    {
                        if (s != dat.Size)
                        {
                            outp = null;
                            return ToolboxReturn.ImageSizeMismatch;
                        }
                        else
                        {
                            bso &= (so == dat.SizeOriginal);
                            foreach (Sect ect in dat.getSects(DataType.Read)) dict[ect._Type] = true;
                        }
                    }
                }

                // If size originals don't match, but sizes do, create a new size original
                if (!bso) so = s;

                var ls = dict.Keys.ToArray();

                if (ls.Length == 0)
                {
                    outp = null;
                    return ToolboxReturn.SpecialError;
                }
                else
                {
                    outp = new ImageData(s, so, ls);
                    return ImageAlgorithms.Add_R(inps, signs, p, ref outp);
                }
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inps"></param>
        /// <param name="signs"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Add_R(ImageData[] inps, Boolean[] signs, Single p, ref ImageData outp)
        {
            if (inps == null) return ToolboxReturn.NullInput;
            else if (signs == null) return ToolboxReturn.NullInput;
            else if (inps.Length != signs.Length) return ToolboxReturn.SpecialError;
            else if (inps.Length * signs.Length == 0) return ToolboxReturn.SpecialError;
            else if (outp == null) return ImageAlgorithms.Add_O(inps, signs, p, out outp);
            else
            {
                Console.WriteLine("SINKER - " + inps.Length);
                Size s = outp.Size;

                foreach (ImageData dat in inps)
                {
                    if (s != dat.Size)
                    {
                        outp = null;
                        return ToolboxReturn.ImageSizeMismatch;
                    }
                }


                foreach (Sect outSect in outp.getSects(DataType.ReadWrite))
                {
                    outSect.setValue(p);

                    for (int i = 0; i < inps.Length; i++)
                    {
                        ImageData dat = inps[i];

                        if (dat.checkSect(outSect._Type))
                        {
                            if (signs[i]) outSect.add(dat.getSect(outSect._Type, DataType.Read));
                            else outSect.sub(dat.getSect(outSect._Type, DataType.Read));
                        }
                    }
                }

                return ToolboxReturn.Good;
            }
        }
        
        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inps"></param>
        /// <param name="signs"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Add_R(ImageData[] inps, Boolean[] signs, ref ImageData outp)
        {
            return ImageAlgorithms.Add_R(inps, signs, 0.0f, ref outp);
        }

        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp1"></param>
        /// <param name="inp2"></param>
        /// <param name="outp"></param>
        public static ToolboxReturn Add_O(ImageData[] inps, Boolean[] signs, out ImageData outp)
        {
            return ImageAlgorithms.Add_O(inps, signs, 0.0f, out outp);
        }


































        public enum GrayScaleType { Mean, Maximum, Minimum, SingleInput};
        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="t"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn GrayScale_O(ref ImageData inp,  GrayScaleType t, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp.Size, inp.SizeOriginal, new SectType[] { SectType.Gray });
                return ImageAlgorithms.GrayScale_R(ref inp, t, ref outp);
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="t"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn GrayScale_R(ref ImageData inp, GrayScaleType t, ref ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (outp == null) return ImageAlgorithms.GrayScale_O(ref inp, t, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                Sect OutGray = outp.getSect(SectType.Gray, DataType.Write);
                int numSects = inp.getSectCount();

                switch (numSects)
                {
                    case 0: return ToolboxReturn.SpecialError;
                    case 1: t = GrayScaleType.SingleInput; break;
                }

                switch (t)
                {
                    case GrayScaleType.SingleInput:
                        {
                            if (numSects != 1) return ToolboxReturn.SpecialError;
                            inp.getSectFirst(DataType.Read).CopyTo(OutGray);
                            return ToolboxReturn.Good;
                        }
                    case GrayScaleType.Maximum:
                        {
                            var oD = OutGray._Data;
                            bool first = true;

                            foreach (Sect s in inp.getSects(DataType.Read))
                            {
                                var iD = s._Data;

                                if (first)
                                {
                                    first = false;
                                    s.CopyTo(OutGray);
                                }
                                else
                                {
                                    for (int y = 0; y < outp.Height; y++)
                                    {
                                        for (int x = 0; x < outp.Width; x++)
                                        {
                                            oD[y, x] = Math.Max(iD[y, x], oD[y, x]);
                                        }
                                    }
                                }
                            }

                            return ToolboxReturn.Good;
                        }
                    case GrayScaleType.Minimum:
                        {
                            var oD = OutGray._Data;
                            bool first = true;

                            foreach (Sect s in inp.getSects(DataType.Read))
                            {
                                if (first)
                                {
                                    first = false;
                                    s.CopyTo(OutGray);
                                }
                                else
                                {
                                    var iD = s._Data;
                                    for (int y = 0; y < outp.Height; y++)
                                    {
                                        for (int x = 0; x < outp.Width; x++)
                                        {
                                            oD[y, x] = Math.Min(iD[y, x], oD[y, x]);
                                        }
                                    }
                                }
                            }

                            return ToolboxReturn.Good;
                        }
                    case GrayScaleType.Mean:
                        {
                            var oD = OutGray._Data;
                            Single mult = 1.0f / Math.Max(1, numSects);
                            bool first = true;

                            foreach (Sect s in inp.getSects(DataType.Read))
                            {
                                var iD = s._Data;

                                if (first)
                                {
                                    first = false;
                                    for (int y = 0; y < outp.Height; y++)
                                    {
                                        for (int x = 0; x < outp.Width; x++)
                                        {
                                            oD[y, x] = iD[y, x] * mult;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int y = 0; y < outp.Height; y++)
                                    {
                                        for (int x = 0; x < outp.Width; x++)
                                        {
                                            oD[y, x] += iD[y, x] * mult;
                                        }
                                    }
                                }
                            }

                            return ToolboxReturn.Good;
                        }
                    default: return ToolboxReturn.SpecialError;
                }
            }
        }

        /// <summary>
        /// Modifies an input ImageData
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ToolboxReturn GrayScale_S(ref ImageData inp, GrayScaleType t)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (inp.getSectCount() == 1)
            {
                inp.getSectFirst(DataType.Read)._Type = SectType.Gray;
                return ToolboxReturn.Good;
            }
            else if (t == GrayScaleType.SingleInput) return ToolboxReturn.SpecialError;
            else
            {
                throw new NotImplementedException();
            }
        }




























        public enum ResizeType { NearestNeighbor, Bilinear };
        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="res"></param>
        /// <param name="t"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Resize_O(ref ImageData inp, Size res, ResizeType t, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (res.Width <= 0)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else if (res.Height <= 0)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                outp = new ImageData(res, inp.SizeOriginal);
                return ImageAlgorithms.Resize_R(ref inp, res, t, ref outp);
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="res"></param>
        /// <param name="t"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Resize_R(ref ImageData inp, Size res, ResizeType t, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.Resize_O(ref inp, res, t, out outp);
            else if (res != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int h = res.Height;
                int w = res.Width;

                int xUp, xDown, yUp, yDown;
                float xAdj, yAdj;

                int refH = inp.Height;
                int refW = inp.Width;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    Sect outSect = outp.getSect(inSect._Type, DataType.Write);
                    var inpArray = inSect._Data;
                    var outArray = outSect._Data;

                    switch (t)
                    {
                        case ResizeType.NearestNeighbor:
                            {
                                for (int y = 0; y < h; y++)
                                {
                                    yAdj = y * (refH - 1);
                                    yAdj /= (h - 1);
                                    yUp = (int)Math.Round(yAdj, 0);

                                    for (int x = 0; x < w; x++)
                                    {
                                        xAdj = x * (refW - 1);
                                        xAdj /= (w - 1);
                                        xUp = (int)Math.Round(xAdj, 0);

                                        outArray[y, x] = inpArray[yUp, xUp];
                                    }
                                }
                                break;
                            }
                        case ResizeType.Bilinear:
                            {
                                for (int y = 0; y < h; y++)
                                {
                                    yAdj = y * (refH - 1);
                                    yAdj /= (h - 1);
                                    yUp = (int)Math.Ceiling((double)yAdj);
                                    yDown = (int)yAdj;

                                    for (int x = 0; x < w; x++)
                                    {
                                        xAdj = x * (refW - 1);
                                        xAdj /= Math.Max(1, (w - 1));
                                        xUp = (int)Math.Ceiling((double)xAdj);
                                        xDown = (int)xAdj;

                                        if (xUp == xDown && yUp == yDown)
                                        {
                                            outArray[y, x] = inpArray[yUp, xUp];
                                        }
                                        else if (xUp == xDown)
                                        {
                                            outArray[y, x] = ImageData.getLinearEstimate(
                                                inpArray[yDown, xUp],
                                                inpArray[yUp, xUp],
                                                yAdj % 1);
                                        }
                                        else if (yUp == yDown)
                                        {
                                            outArray[y, x] = ImageData.getLinearEstimate(
                                                inpArray[yUp, xDown],
                                                inpArray[yUp, xUp],
                                                xAdj % 1);
                                        }
                                        else
                                        {
                                            outArray[y, x] = ImageData.getLinearEstimate(
                                                ImageData.getLinearEstimate(
                                                    inpArray[yDown, xDown],
                                                    inpArray[yUp, xDown],
                                                    yAdj % 1),
                                                ImageData.getLinearEstimate(
                                                    inpArray[yDown, xUp],
                                                    inpArray[yUp, xUp],
                                                    yAdj % 1),
                                                xAdj % 1);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                return ToolboxReturn.Good;
            }
        }
 





























        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn HistogramEqualize_O(ref ImageData inp, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageAlgorithms.HistogramEqualize_R(ref inp, ref outp);
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn HistogramEqualize_R(ref ImageData inp, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.HistogramEqualize_O(ref inp, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                int sum, sumLast, temp;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    Sect outSect = outp.getSect(inSect._Type, DataType.ReadWrite);
                    var inpArray = inSect._Data;
                    var outArray = outSect._Data;

                    int[] counts = new int[511];
                    Byte[] _Bytes = new Byte[511];

                    for (int i = 0; i < counts.Length; i++) counts[i] = 0;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            counts[255 + ImageAlgorithms.cast(inpArray[y, x] * 255)]++;
                        }
                    }

                    sum = 0;
                    sumLast = 0;
                    for (int i = 0; i < counts.Length; i++)
                    {
                        temp = counts[i];
                        if (temp != 0)
                        {
                            sumLast = sum;
                            sum += temp;
                        }
                    }

                    sum = 0;
                    sumLast = Math.Max(1, sumLast);
                    for (int i = 0; i < counts.Length; i++)
                    {
                        _Bytes[i] = (Byte)((255 * sum) / (sumLast));
                        sum += counts[i];
                    }

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            outArray[y, x] = _Bytes[255 + ImageAlgorithms.cast(inpArray[y, x] * 255)] / 255.0f;
                        }
                    }
                }

                return ToolboxReturn.Good;
            }
        }




















        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="indata"></param>
        /// <param name="bpp"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn BitPerPixel_O(ref ImageData inp, int bpp, out ImageData outp)
        {
            if (bpp < 1)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else if (bpp > 8)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageAlgorithms.BitPerPixel_R(ref inp, bpp, ref outp);
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="bpp"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn BitPerPixel_R(ref ImageData inp, int bpp, ref ImageData outp)
        {
            if (bpp < 1) return ToolboxReturn.SpecialError;
            else if (bpp > 8) return ToolboxReturn.SpecialError;
            else if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.BitPerPixel_O(ref inp, bpp, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                float mult = (float)Math.Pow(2, bpp);
                float div = mult - 1;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    Sect outSect = outp.getSect(inSect._Type, DataType.Write);

                    var inpArray = inSect._Data;
                    var outArray = outSect._Data;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            outArray[y, x] = Clamp(((Single)Math.Round(inpArray[y, x] * mult - 0.5f)) / div, 0, 1);
                        }
                    }
                }

                return ToolboxReturn.Good;
            }
        }
        /// <summary>
        /// Modifies an input ImageData
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="bpp"></param>
        /// <returns></returns>
        public static ToolboxReturn BitPerPixel_S(ref ImageData inp, int bpp)
        {
            if (bpp < 1) return ToolboxReturn.SpecialError;
            else if (bpp > 8) return ToolboxReturn.SpecialError;
            else if (inp == null) return ToolboxReturn.NullInput;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                float mult = (float)Math.Pow(2, bpp);
                float div = mult - 1;

                foreach (Sect inSect in inp.getSects(DataType.ReadWrite))
                {
                    var ar = inSect._Data;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            ar[y, x] = Clamp(((Single)Math.Round(ar[y, x] * mult - 0.5f)) / div, 0, 1);
                        }
                    }
                }

                return ToolboxReturn.Good;
            }
        }














        private static float Clamp(float val, float min, float max)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Clamp_O(ref ImageData inp, Single min, Single max, out ImageData outp)
        {
            if (min > max)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageAlgorithms.Clamp_R(ref inp, min, max, ref outp);
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Clamp_R(ref ImageData inp, Single min, Single max, ref ImageData outp)
        {
            if (min > max) return ToolboxReturn.SpecialError;
            else if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.Clamp_O(ref inp, min, max, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    Sect outSect = outp.getSect(inSect._Type, DataType.Write);

                    var inpArray = inSect._Data;
                    var outArray = outSect._Data;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            outArray[y, x] = Math.Min(max, Math.Max(min, inpArray[y, x]));
                        }
                    }
                }

                return ToolboxReturn.Good;
            }
        }

        /// <summary>
        /// Modifies an input ImageData
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static ToolboxReturn Clamp_S(ref ImageData inp, Single min, Single max)
        {
            if (min > max) return ToolboxReturn.SpecialError;
            else if (inp == null) return ToolboxReturn.NullInput;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                foreach (Sect inSect in inp.getSects(DataType.ReadWrite))
                {
                    var ar = inSect._Data;
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            ar[y, x] = Math.Min(max, Math.Max(min, ar[y, x]));
                        }
                    }
                }

                return ToolboxReturn.Good;
            }
        }












        public enum ColorMapType { Cold_Hot, Hue };

        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Colormap_O(ref ImageData inp, ColorMapType c, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp.Size, inp.SizeOriginal, new SectType[] { SectType.RGB_Red, SectType.RGB_Green, SectType.RGB_Blue });
                return ImageAlgorithms.Colormap_R(ref inp, c, ref outp);
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Colormap_R(ref ImageData inp, ColorMapType c, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.Colormap_O(ref inp, c, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                Single min = inp.min;
                Single range = inp.max - min;
                if (range == 0) return ToolboxReturn.SpecialError;

                Single mult = 1.0f / Math.Max(1, inp.getSectCount());
                Single lum;

                Single[,] r, g, b;
                outp.getRGB(out r, out g, out b, DataType.Write);

                var ch = ImageAlgorithms.getColdHot();
                var lens = ch.GetLength(0) - 1;
                int cur = 0;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        lum = 0;
                        foreach (Sect s in inp.getSects(DataType.Read)) lum += s._Data[y, x];
                        lum = (lum * mult - min) / range;
                        lum = Math.Max(0, Math.Min(1, lum)); // Shouldn't need this but what the hell
                        switch (c)
                        {
                            case ColorMapType.Hue:
                                ColorMethods.hsv2rgb(lum, 1, 1,
                                    out r[y, x],
                                    out g[y, x],
                                    out b[y, x]);
                                break;
                            case ColorMapType.Cold_Hot:
                                cur = (int)(lum * lens);
                                r[y, x] = ch[cur, 0];
                                g[y, x] = ch[cur, 1];
                                b[y, x] = ch[cur, 2];
                                break;
                        }                        
                    }
                }
                
                return ToolboxReturn.Good;
            }
        }

        /// <summary>
        /// Modifies an input ImageData
        /// </summary>
        /// <param name="inp"></param>
        /// <returns></returns>
        public static ToolboxReturn Colormap_S(ref ImageData inp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else
            {
                throw new NotImplementedException();
            }
        }

        private static Single[,] arrayColdHot = null;
        private static unsafe Single[,] getColdHot()
        {
            if (ImageAlgorithms.arrayColdHot == null)
            {
                var im = Properties.Resources.HeatmapHotCold;

                int w = im.Size.Width;

                var bits = im.LockBits(
                    new Rectangle(0, 0, w, im.Size.Height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                Byte* row;
                int xx = 0, x;
                const int y = 0;

                row = (Byte*)bits.Scan0 + (y * bits.Stride);

                var ia = new Single[w, 3];

                for (x = 0, xx = 0; x < w; x++, xx += 3)
                {
                    ia[x, 0] = row[xx + 2] / 255.0f;
                    ia[x, 1] = row[xx + 1] / 255.0f;
                    ia[x, 2] = row[xx + 0] / 255.0f;
                }

                im.UnlockBits(bits);

                ImageAlgorithms.arrayColdHot = ia;
            }
            return ImageAlgorithms.arrayColdHot;
        }

















        /// Creates a new ImageData class
        public static ToolboxReturn Hue_O(ref ImageData inp, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp.Size, inp.SizeOriginal, new SectType[] { SectType.HSL_H });
                return ImageAlgorithms.Hue_R(ref inp, ref outp);
            }
        }
        /// Modifies an existing ImageData class
        public static ToolboxReturn Hue_R(ref ImageData inp, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.Hue_O(ref inp, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                Single[, ] r, g, b;

                float fr = 0;
                float fg = 0;
                float fb = 0;

                const float PIF = (float) Math.PI;
                const float PIF2 = PIF * 2;

                
                inp.getRGB(out r, out g, out b, DataType.Read);
                Single[, ] hue = outp.getSect(SectType.HSL_H, DataType.Write)._Data;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        fr = Math.Max(0, Math.Min(1, r[y, x]));
                        fg = Math.Max(0, Math.Min(1, g[y, x]));
                        fb = Math.Max(0, Math.Min(1, b[y, x]));

                        hue[y, x] = (float)(Math.Atan2(1.73205080757f * (fg - fb), 2*fr - fg - fb)/PIF2);
                    }
                }

                return ToolboxReturn.Good;
            }
        }
        /// Modifies an input ImageData
        public static ToolboxReturn Hue_S(ref ImageData inp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                Single[, ] r, g, b;

                float fr = 0;
                float fg = 0;
                float fb = 0;

                const float PIF = (float)Math.PI;
                const float PIF2 = PIF * 2;

                inp.getRGB(out r, out g, out b, DataType.Read);
                Single[,] hue = inp.getSect(SectType.HSL_H, DataType.Write)._Data;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        fr = Math.Max(0, Math.Min(1, r[y, x]));
                        fg = Math.Max(0, Math.Min(1, g[y, x]));
                        fb = Math.Max(0, Math.Min(1, b[y, x]));

                        hue[y, x] = (float)(Math.Atan2(1.73205080757f * (fg - fb), 2 * fr - fg - fb) / PIF2);
                    }
                }

                return ToolboxReturn.Good;
            }
        }













        /// Creates a new ImageData class
        public static ToolboxReturn HSV_O(ref ImageData inp, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp.Size, inp.SizeOriginal, new SectType[] { SectType.HSL_H, SectType.HSV_S, SectType.HSV_V });
                return ImageAlgorithms.HSV_R(ref inp, ref outp);
            }
        }
        /// Modifies an existing ImageData class
        public static ToolboxReturn HSV_R(ref ImageData inp, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.HSV_O(ref inp, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                Single[, ] r, g, b;
                inp.getRGB(out r, out g, out b, DataType.Read);
                Single[,] hue = outp.getSect(SectType.HSL_H, DataType.Write)._Data;
                Single[,] sat = outp.getSect(SectType.HSV_S, DataType.Write)._Data;
                Single[,] val = outp.getSect(SectType.HSV_V, DataType.Write)._Data;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        ColorMethods.rgb2hsv(
                            Math.Max(0, Math.Min(1, r[y, x])),
                            Math.Max(0, Math.Min(1, g[y, x])),
                            Math.Max(0, Math.Min(1, b[y, x])),
                            out hue[y, x],
                            out sat[y, x],
                            out val[y, x]);

                    }
                }

                return ToolboxReturn.Good;
            }
        }
        /// Modifies an input ImageData
        public static ToolboxReturn HSV_S(ref ImageData inp)
        {
            return ImageAlgorithms.HSV_R(ref inp, ref inp);
        }






        /// Creates a new ImageData class
        public static ToolboxReturn HSL_O(ref ImageData inp, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp.Size, inp.SizeOriginal, new SectType[] { SectType.HSL_H, SectType.HSV_S, SectType.HSV_V });
                return ImageAlgorithms.HSL_R(ref inp, ref outp);
            }
        }
        /// Modifies an existing ImageData class
        public static ToolboxReturn HSL_R(ref ImageData inp, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.HSL_O(ref inp, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                Single[,] r, g, b;
                inp.getRGB(out r, out g, out b, DataType.Read);
                Single[,] hue = outp.getSect(SectType.HSL_H, DataType.Write)._Data;
                Single[,] sat = outp.getSect(SectType.HSL_S, DataType.Write)._Data;
                Single[,] lum = outp.getSect(SectType.HSL_L, DataType.Write)._Data;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        ColorMethods.rgb2hsl(
                            Math.Max(0, Math.Min(1, r[y, x])),
                            Math.Max(0, Math.Min(1, g[y, x])),
                            Math.Max(0, Math.Min(1, b[y, x])),
                            out hue[y, x],
                            out sat[y, x],
                            out lum[y, x]);
                    }
                }

                return ToolboxReturn.Good;
            }
        }
        /// Modifies an input ImageData
        public static ToolboxReturn HSL_S(ref ImageData inp)
        {
            return ImageAlgorithms.HSL_R(ref inp, ref inp);
        }










        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="threshold"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn TwoTone_O(ref ImageData inp, Single threshold, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageAlgorithms.TwoTone_R(ref inp, threshold, ref outp);
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="threshold"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn TwoTone_R(ref ImageData inp, Single threshold, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ToolboxReturn.NullInput;
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    Sect outSect = outp.getSect(inSect._Type, DataType.Write);
                    var inpArray = inSect._Data;
                    var outArray = outSect._Data;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            outArray[y, x] = inpArray[y, x] < threshold ? 0 : 1;
                        }
                    }
                }
                return ToolboxReturn.Good;
            }
        }

        /// <summary>
        /// Modifies an input ImageData
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static ToolboxReturn TwoTone_S(ref ImageData inp, Single threshold)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else
            {
                int w = inp.Width;
                int h = inp.Height;

                foreach (Sect s in inp.getSects(DataType.ReadWrite))
                {
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            s._Data[y, x] = s._Data[y, x] < threshold ? 0 : 1;
                        }
                    }
                }

                return ToolboxReturn.Good;
            }
        }

















        /*














        public static ImageData erode(ImageData indata, Boolean[, ] grid, int cycles)
        {
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.erode(indata, grid);

            return indata;
        }

        public static ImageData erode(ImageData indata, Boolean[, ] grid)
        {
            int w = indata.Width;
            int h = indata.Height;

            ImageData _SpecialBitmap = new ImageData();
            _SpecialBitmap.CopyTraits(indata);
            _SpecialBitmap.CreateNonNullArrays(indata);

            var sourceList = indata.getNonNullArrays();
            var targetList = _SpecialBitmap.getNonNullArrays();
           
            int cols = grid.Length;
            int rows = grid[0].Length;

            int c2 = cols / 2;
            int r2 = rows / 2;

            int x, y, i, j;
            int tx, ty;
            bool contains;

            for (int index = 0; index < sourceList.Count; index++)
            {
                var inpArray = sourceList[index];
                var outArray = targetList[index];

                for (y = 0; y < h; y++)
                {
                    for (x = 0, x = 0; x < w; x++)
                    {
                        contains = true;

                        for (i = 0; contains && i < rows; i++)
                        {
                            ty = y + i - r2;
                            if (ty >= 0 && ty < h)
                            {
                                for (j = 0; contains && j < cols; j++)
                                {
                                    tx = x + j - c2;
                                    if (tx >= 0 && tx < w)
                                    {
                                        if (grid[j, i])
                                        {
                                            if (inpArray[ty, tx] < 128)
                                            {
                                                contains = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (contains) outArray[y, x] = 255;
                        else outArray[y, x] = 0;
                    }
                }
            }

            return _SpecialBitmap;
        }

        public static ImageData dilate(ImageData indata, Boolean[, ] grid, int cycles)
        {
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.dilate(indata, grid);

            return indata;
        }
        
        public static ImageData dilate(ImageData indata, Boolean[, ] grid)
        {
            int w = indata.Width;
            int h = indata.Height;

            ImageData _SpecialBitmap = new ImageData();
            _SpecialBitmap.CopyTraits(indata);
            _SpecialBitmap.CreateNonNullArrays(indata);

            var sourceList = indata.getNonNullArrays();
            var targetList = _SpecialBitmap.getNonNullArrays();

            int cols = grid.Length;
            int rows = grid[0].Length;

            int c2 = cols / 2;
            int r2 = rows / 2;

            int x, y, i, j;
            int tx, ty;
            bool contains;

            for (int index = 0; index < sourceList.Count; index++)
            {
                var inpArray = sourceList[index];
                var outArray = targetList[index];

                for (y = 0; y < h; y++)
                {
                    for (x = 0, x = 0; x < w; x++)
                    {
                        contains = true;

                        for (i = 0; contains && i < rows; i++)
                        {
                            ty = y + i - r2;
                            if (ty >= 0 && ty < h)
                            {
                                for (j = 0; contains && j < cols; j++)
                                {
                                    tx = x + j - c2;
                                    if (tx >= 0 && tx < w)
                                    {
                                        if (grid[j, i])
                                        {
                                            if (inpArray[ty, tx] > 127)
                                            {
                                                contains = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (!contains) outArray[y, x] = 255;
                        else outArray[y, x] = 0;
                    }
                }
            }

            return _SpecialBitmap; 
        }

        public static ImageData open(ImageData indata, Boolean[, ] grid, int cycles)
        {
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.erode(indata, grid);
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.dilate(indata, grid);
            return indata;
        }

        public static ImageData close(ImageData indata, Boolean[, ] grid, int cycles)
        {
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.dilate(indata, grid);
            for (int i = 0; i < cycles; i++)
                indata = ImageToolbox.erode(indata, grid);
            return indata;
        }
        */

















































        private static Random rand = new Random(); //reuse this if you are generating many

        private static float nextGaussian(Single std)
        {
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return std * (float) randStdNormal;
//            double randNormal = mean + stdDev * randStdNormal;
        }

        private static float nextNormal(Single std)
        {
            return std * (1 - 2 * ((float)(rand.NextDouble())));
        }

        public enum NoiseType { Gaussian, Uniform, SaltAndPepper};

        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="t"></param>
        /// <param name="p"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Noise_O(ref ImageData inp, NoiseType t, Single p, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageAlgorithms.Noise_R(ref inp, t, p, ref outp);
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="t"></param>
        /// <param name="p"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Noise_R(ref ImageData inp, NoiseType t, Single p, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ToolboxReturn.NullInput;
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int h = inp.Height;
                int w = inp.Width;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    Sect outSect = outp.getSect(inSect._Type, DataType.Write);
                    var inpArray = inSect._Data;
                    var outArray = outSect._Data;

                    switch (t)
                    {
                        case NoiseType.Gaussian:
                            {
                                for (int y = 0; y < h; y++)
                                {
                                    for (int x = 0; x < w; x++)
                                    {
                                        outArray[y, x] = inpArray[y, x] + ImageAlgorithms.nextGaussian(p);
                                    }
                                }
                                break;
                            }
                        case NoiseType.Uniform:
                            {
                                for (int y = 0; y < h; y++)
                                {
                                    for (int x = 0; x < w; x++)
                                    {
                                        outArray[y, x] = inpArray[y, x] + ImageAlgorithms.nextNormal(p);
                                    }
                                }
                                break;
                            }
                        case NoiseType.SaltAndPepper:
                            {
                                for (int y = 0; y < h; y++)
                                {
                                    for (int x = 0; x < w; x++)
                                    {
                                        float rand = (float)ImageAlgorithms.rand.NextDouble();
                                        outArray[y, x] = (rand * 2 < p) ? 1 : (rand < p) ? 0 : inpArray[y, x];
                                    }
                                }
                                break;
                            }
                    }
                }
                return ToolboxReturn.Good;
            }
        }

        /// <summary>
        /// Modifies an input ImageData
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="t"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static ToolboxReturn Noise_S(ref ImageData inp, NoiseType t, Single p)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else
            {
                int h = inp.Height;
                int w = inp.Width;

                p = Math.Min(1, p);

                foreach (Sect inSect in inp.getSects(DataType.ReadWrite))
                {
                    var inpArray = inSect._Data;

                    switch (t)
                    {
                        case NoiseType.Gaussian:
                            {
                                for (int y = 0; y < h; y++)
                                {
                                    for (int x = 0; x < w; x++)
                                    {
                                        inpArray[y, x] += ImageAlgorithms.nextGaussian(p);
                                    }
                                }
                                break;
                            }
                        case NoiseType.Uniform:
                            {
                                for (int y = 0; y < h; y++)
                                {
                                    for (int x = 0; x < w; x++)
                                    {
                                        inpArray[y, x] += p * ImageAlgorithms.nextNormal(p);
                                    }
                                }
                                break;
                            }
                        case NoiseType.SaltAndPepper:
                            {
                                for (int y = 0; y < h; y++)
                                {
                                    for (int x = 0; x < w; x++)
                                    {
                                        float rand = (float)ImageAlgorithms.rand.NextDouble();
                                        inpArray[y, x] = (rand * 2 < p) ? 255 : (rand < p) ? 0 : inpArray[y, x];
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            return ToolboxReturn.Good;       
        }










                /// Creates a new ImageData class
        public static ToolboxReturn Abs_O(ref ImageData inp, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageAlgorithms.Abs_R(ref inp, ref outp);
            }
        }
        /// Modifies an existing ImageData class
        public static ToolboxReturn Abs_R(ref ImageData inp, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.Abs_O(ref inp, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int h = inp.Height;
                int w = inp.Width;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    Sect outSect = outp.getSect(inSect._Type, DataType.Write);

                    var inpArray = inSect._Data;
                    var outArray = outSect._Data;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            outArray[y, x] = Math.Abs(inpArray[y, x]);
                        }
                    }
                }

                return ToolboxReturn.Good;            
            }
        }
        /// Modifies an input ImageData
        public static ToolboxReturn Abs_S(ref ImageData inp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else
            {
                int h = inp.Height;
                int w = inp.Width;

                foreach (Sect inSect in inp.getSects(DataType.ReadWrite))
                {
                    var inpArray = inSect._Data;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            inpArray[y, x] = Math.Abs(inpArray[y, x]);
                        }
                    }
                }
                return ToolboxReturn.Good;            
            }
        }








                /// Creates a new ImageData class
        public static ToolboxReturn MultiplyConstant_O(ref ImageData inp, Single p, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageAlgorithms.MultiplyConstant_R(ref inp, p, ref outp);
            }
        }
        /// Modifies an existing ImageData class
        public static ToolboxReturn MultiplyConstant_R(ref ImageData inp, Single p, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.MultiplyConstant_O(ref inp, p, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
                int h = inp.Height;
                int w = inp.Width;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    Sect outSect = outp.getSect(inSect._Type, DataType.Write);
                    var inpArray = inSect._Data;
                    var outArray = outSect._Data;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            outArray[y, x] = inpArray[y, x] * p;
                        }
                    }
                }
                return ToolboxReturn.Good;            
            }
        }
        /// Modifies an input ImageData
        public static ToolboxReturn MultiplyConstant_S(ref ImageData inp, Single p)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else
            {
                int h = inp.Height;
                int w = inp.Width;

                foreach (Sect inSect in inp.getSects(DataType.ReadWrite))
                {
                    var inpArray = inSect._Data;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            inpArray[y, x] *= p;
                        }
                    }
                }
                return ToolboxReturn.Good;            
            }
        }







        

        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp1"></param>
        /// <param name="inp2"></param>
        /// <param name="outp"></param>
        public static ToolboxReturn Multiply_O(ref ImageData inp1, ref ImageData inp2, Single p, out ImageData outp)
        {
            if (inp1 == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inp2 == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                return ImageAlgorithms.Multiply_O(new ImageData[] { inp1, inp2 }, p, out outp);
            }        
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp1"></param>
        /// <param name="inp2"></param>
        /// <param name="outp"></param>
        public static ToolboxReturn Multiply_R(ref ImageData inp1, ref ImageData inp2, Single p, ref ImageData outp)
        {
            if (inp1 == null) return ToolboxReturn.NullInput;
            else if (inp2 == null) return ToolboxReturn.NullInput;
            else return ImageAlgorithms.Multiply_R(new ImageData[] { inp1, inp2 }, p, ref outp);
        }

        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp1"></param>
        /// <param name="inp2"></param>
        /// <param name="outp"></param>
        public static ToolboxReturn Multiply_O(ref ImageData inp1, ref ImageData inp2, out ImageData outp)
        {
            return ImageAlgorithms.Multiply_O(ref inp1, ref inp2, 1.0f, out outp);

        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inp1"></param>
        /// <param name="inp2"></param>
        /// <param name="outp"></param>
        public static ToolboxReturn Multiply_R(ref ImageData inp1, ref ImageData inp2, ref ImageData outp)
        {
            return ImageAlgorithms.Multiply_R(ref inp1, ref inp2, 1.0f, ref outp);
        }









        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp1"></param>
        /// <param name="inp2"></param>
        /// <param name="outp"></param>
        public static ToolboxReturn Multiply_O(ImageData[] inps, Single p, out ImageData outp)
        {
            if (inps == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else if (inps.Length == 0)
            {
                outp = null;
                return ToolboxReturn.SpecialError;
            }
            else
            {
                var ls1 = new List<ImageData>();
                var lsN1 = new List<ImageData>();

                bool first = true;
                bool bso = true;

                Size s = Size.Empty;
                Size so = Size.Empty;

                foreach (ImageData dat in inps)
                {
                    if (first)
                    {
                        s = dat.Size;
                        so = dat.SizeOriginal;
                        first = false;
                    }
                    else
                    {
                        if (s != dat.Size)
                        {
                            outp = null;
                            return ToolboxReturn.ImageSizeMismatch;
                        }
                        else
                        {
                            bso &= (so == dat.SizeOriginal);
                        }
                    }

                    switch (dat.getSectCount())
                    {
                        case 0:
                            {
                                outp = null; 
                                return ToolboxReturn.SpecialError;
                            }
                        case 1: ls1.Add(dat); break;
                        default: lsN1.Add(dat); break;
                    }
                }

                var dict = new Dictionary<SectType, Boolean>();

                switch (lsN1.Count)
                {
                    case 0:
                        {
                            foreach (ImageData dat in ls1)
                            {
                                foreach (Sect ect in dat.getSects(DataType.Read))
                                {
                                    dict[ect._Type] = true;
                                }
                            }
                            if (dict.Count != 1)
                            {
                                dict.Clear();
                                dict[SectType.Gray] = true;
                            }
                            break;
                        }
                    default:
                        {
                            foreach (ImageData dat in lsN1)
                            {
                                foreach (Sect ect in dat.getSects(DataType.Read))
                                {
                                    dict[ect._Type] = true;
                                }
                            }
                            break;
                        }
                }

                // If size originals don't match, but sizes do, create a new size original
                if (!bso) so = s;

                var ls = dict.Keys.ToArray();

                if (ls.Length == 0)
                {
                    outp = null;
                    return ToolboxReturn.SpecialError;
                }
                else
                {
                    outp = new ImageData(s, so, ls);
                    return ImageAlgorithms.Multiply_R(inps, p, ref outp);
                }
            }
        }

        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inps"></param>
        /// <param name="signs"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Multiply_R(ImageData[] inps, Single p, ref ImageData outp)
        {
            if (inps == null) return ToolboxReturn.NullInput;
            else if (inps.Length == 0) return ToolboxReturn.SpecialError;
            else if (outp == null) return ImageAlgorithms.Multiply_O(inps, p, out outp);
            else
            {
                var ls1 = new List<ImageData>();
                var lsN1 = new List<ImageData>();

                Size s = outp.Size;

                foreach (ImageData dat in inps)
                {
                    if (s != dat.Size)
                    {
                        outp = null;
                        return ToolboxReturn.ImageSizeMismatch;
                    }

                    switch (dat.getSectCount())
                    {
                        case 0:
                            {
                                outp = null;
                                return ToolboxReturn.SpecialError;
                            }
                        case 1: ls1.Add(dat); break;
                        default: lsN1.Add(dat); break;
                    }
                }

                int h = s.Height;
                int w = s.Width;

                switch (lsN1.Count)
                {
                    case 0:
                        {
                            if (outp.getSectCount() != 1) return ToolboxReturn.SpecialError;
                            else
                            {
                                var outArray = outp.getSectFirst(DataType.Write)._Data;

                                bool first = true;
                                foreach (ImageData dat in ls1)
                                {
                                    var inpArray = dat.getSectFirst(DataType.Read)._Data;

                                    if (first)
                                    {
                                        first = false;
                                        for (int y = 0; y < h; y++)
                                        {
                                            for (int x = 0; x < w; x++)
                                            {
                                                outArray[y, x] = inpArray[y, x] * p;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int y = 0; y < h; y++)
                                        {
                                            for (int x = 0; x < w; x++)
                                            {
                                                outArray[y, x] *= inpArray[y, x];
                                            }
                                        }
                                    }
                                }

                                return ToolboxReturn.Good;
                            }
                        }
                    case 1:
                        {
                            ImageData inp = lsN1.First();

                            foreach (Sect outSect in outp.getSects(DataType.Write))
                            {
                                var outArray = outSect._Data;

                                if (inp.checkSect(outSect._Type))
                                {
                                    var inpArray = inp.getSect(outSect._Type, DataType.Read)._Data;

                                    for (int y = 0; y < h; y++)
                                    {
                                        for (int x = 0; x < w; x++)
                                        {
                                            outArray[y, x] = inpArray[y, x] * p;
                                        }
                                    }

                                    foreach (ImageData inpSingle in ls1)
                                    {
                                        inpArray = inpSingle.getSectFirst(DataType.Read)._Data;

                                        for (int y = 0; y < h; y++)
                                        {
                                            for (int x = 0; x < w; x++)
                                            {
                                                outArray[y, x] *= inpArray[y, x];
                                            }
                                        }
                                    }
                                }
                                else return ToolboxReturn.SpecialError; // Output sect not found on input
                            }
                            return ToolboxReturn.Good;
                        }
                    default:
                        {
                            foreach (Sect outSect in outp.getSects(DataType.Write))
                            {
                                outSect.setValue(p);

                                var outArray = outSect._Data;

                                foreach (ImageData inp in inps)
                                {
                                    if (inp.checkSect(outSect._Type))
                                    {
                                        var inpArray = inp.getSect(outSect._Type, DataType.Read)._Data;

                                        for (int y = 0; y < h; y++)
                                        {
                                            for (int x = 0; x < w; x++)
                                            {
                                                outArray[y, x] *= inpArray[y, x];
                                            }
                                        }
                                    }
                                }
                            }
                            return ToolboxReturn.Good;
                        }
                }
            }
        }
        
        /// <summary>
        /// Modifies an existing ImageData class
        /// </summary>
        /// <param name="inps"></param>
        /// <param name="signs"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn Multiply_R(ImageData[] inps, ref ImageData outp)
        {
            return ImageAlgorithms.Multiply_R(inps, 1.0f, ref outp);
        }

        /// <summary>
        /// Creates a new ImageData class
        /// </summary>
        /// <param name="inp1"></param>
        /// <param name="inp2"></param>
        /// <param name="outp"></param>
        public static ToolboxReturn Multiply_O(ImageData[] inps, out ImageData outp)
        {
            return ImageAlgorithms.Multiply_O(inps, 1.0f, out outp);
        }











        /// Creates a new ImageData class
        public static ToolboxReturn Gradient_O(ref ImageData inp, out ImageData outp)
        {
            if (inp == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                outp = new ImageData(inp);
                return ImageAlgorithms.Gradient_R(ref inp, ref outp);
            }
        }
        /// Modifies an existing ImageData class
        public static ToolboxReturn Gradient_R(ref ImageData inp, ref ImageData outp)
        {
            if (inp == null) return ToolboxReturn.NullInput;
            else if (outp == null) return ImageAlgorithms.Gradient_O(ref inp, out outp);
            else if (inp.Size != outp.Size) return ToolboxReturn.ImageSizeMismatch;
            else
            {
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

                int w = inp.Width;
                int h = inp.Height;

                const int dim = 3;

                foreach (Sect inSect in inp.getSects(DataType.Read))
                {
                    Sect outSect = outp.getSect(inSect._Type, DataType.Write);

                    var inpArray = inSect._Data;
                    var outArray = outSect._Data;

                    int ty;
                    float val;
                    float gx;
                    float gy;

                    for (int y = 1; y < h - 1; y++)
                    {
                        for (int x = 1; x < w - 1; x++)
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
                            outArray[y, x] = (Single)Math.Sqrt(gx * gx + gy * gy);
                        }
                    }

                    for (int y = 0; y < h; y++)
                    {
                        outArray[y, 0] = 0;
                        outArray[y, w - 1] = 0;
                    }

                    for (int x = 0; x < w; x++)
                    {
                        outArray[0, x] = 0;
                        outArray[h - 1, x] = 0;
                    }
                }

                return ToolboxReturn.Good;
            }
        }




        












        /*
        public delegate float StandardFunctionComparator(float mvalue);
        public static ImageData StandardFunctionModify(ImageData indata, StandardFunctionComparator func)
        {
            int w = indata.Width;
            int h = indata.Height;

            ImageData _SpecialBitmap = new ImageData();
            _SpecialBitmap.CopyTraits(indata);
            _SpecialBitmap.CreateNonNullArrays(indata);

            var sourceList = indata.getNonNullArrays();
            var targetList = _SpecialBitmap.getNonNullArrays();

            int x, y;

            for (int index = 0; index < sourceList.Count; index++)
            {
                var inpArray = sourceList[index];
                var outArray = targetList[index];

                for (y = 0; y < h; y++)
                {
                    for (x = 0; x < w; x++)
                    {
                        outArray[y, x] = func(inpArray[y, x]);
                    }
                }
            }

            return _SpecialBitmap;
        }*/



        /*
        public delegate float RegionPropsComparator(float mvalue, SpecialBitmap.RegionProps rp);
        public static SpecialBitmap RegionPropsModify(SpecialBitmap indata, RegionPropsComparator func)
        {
            int w = indata.Width;
            int h = indata.Height;

            SpecialBitmap _SpecialBitmap = new SpecialBitmap();
            _SpecialBitmap.CopyTraits(indata);
            _SpecialBitmap.CreateNonNullArrays(indata);

            var sourceList = indata.getNonNullArrays();
            var targetList = _SpecialBitmap.getNonNullArrays();

            int x, y;

            for (int index = 0; index < sourceList.Count; index++)
            {
                var inpArray = sourceList[index];
                var outArray = targetList[index];

                for (y = 0; y < h; y++)
                {
                    for (x = 0; x < w; x++)
                    {
                        outArray[y, x] = func(inpArray[y, x], indata._RegionProps[indata._IntRegions[y, x]]);
                    }
                }
            }

            return _SpecialBitmap;
        }*/
    }
}
