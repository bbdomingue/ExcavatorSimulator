using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public class ImageData
    {
        public ImageData Clone()
        {
            ImageData ret = new ImageData(this.Size, this.SizeOriginal);

            foreach (Sect s in this.getSects(DataType.Read)) ret._Data[s._Type] = s.Clone();

            return ret;
        }

        private Dictionary<SectType, Sect> _Data = new Dictionary<SectType, Sect>();

        public Size SizeOriginal = new Size(0, 0);
        public Size Size = new Size(0, 0);

        public int Height { get { return this.Size.Height; } }
        public int Width { get { return this.Size.Width; } }

        public Single min
        {
            get
            {
                Single mn = Single.MaxValue;
                foreach (Sect s in this.getSects(DataType.Read)) mn = Math.Min(s.min, mn);
                return mn;
            }
        }

        public Single max
        {
            get
            {
                Single mx = Single.MinValue;
                foreach (Sect s in this.getSects(DataType.Read)) mx = Math.Max(s.max, mx);
                return mx;
            }
        }















        public unsafe ImageData(Image input)
            : this(input, input.Width, input.Height)
        {
        }

        public unsafe ImageData(Image input, int w, int h)
        {
            SectType t;

            this.Size.Width = w;
            this.Size.Height = h;

            this.SizeOriginal = this.Size;

            t = SectType.RGB_Red; this._Data[t] = new Sect(t, this.Width, this.Height);
            t = SectType.RGB_Green; this._Data[t] = new Sect(t, this.Width, this.Height);
            t = SectType.RGB_Blue; this._Data[t] = new Sect(t, this.Width, this.Height);

            bool create = false;

            Bitmap b = input as Bitmap;


            if (b == null) create = true;
            else if (b.Size != this.Size) create = true;

            if (create) b = new Bitmap(input, this.Width, this.Height);

            this.setImage(b);
        }

        private unsafe void setImage(Bitmap input)
        {
            if (input == null) return;
            else if (this.Size != input.Size) ;
            else
            {
                Single[,] r, g, b;

                this.getRGB(out r, out g, out b, DataType.Write);

                BitmapData bmd = input.LockBits(
                new Rectangle(0, 0, input.Width, input.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                Byte* row;
                int xx = 0, x;

                for (int y = 0; y < this.Height; y++)
                {
                    row = (Byte*)bmd.Scan0 + (y * bmd.Stride);

                    for (x = 0, xx = 0; x < this.Width; x++, xx += 3)
                    {
                        r[y, x] = row[xx + 2] / 255.0f;
                        g[y, x] = row[xx + 1] / 255.0f;
                        b[y, x] = row[xx + 0] / 255.0f;
                    }
                }
                input.UnlockBits(bmd);
            }
        }

        public static unsafe void fromColorImage(Bitmap input, ref ImageData dat)
        {
            if (input == null) ;
            else if (dat == null) dat = new ImageData(input);
            else dat.setImage(input);
        }


        public ImageData(Size size, Size sizeOriginal)
        {
            this.Size = size;
            this.SizeOriginal = sizeOriginal;
        }

        public ImageData(ImageData input)
            : this(input.Size, input.SizeOriginal)
        {
        }

        public ImageData(Size size, Size sizeOriginal, SectType[] s)
            : this(size, sizeOriginal)
        {
            foreach (SectType t in s)
            {
                this._Data[t] = new Sect(t, this.Width, this.Height);
            }
        }






        ~ImageData()
        {
            this._Data = null;
        }


















        public static SectType[] rgb
        {
            get
            {
                return new SectType[] { SectType.RGB_Red, SectType.RGB_Green, SectType.RGB_Blue };
            }
        }

        public void getRGB(out Single[, ] r, out Single[, ] g, out Single[, ] b, DataType t)
        {
            r = this.getSect(SectType.RGB_Red, t)._Data;
            g = this.getSect(SectType.RGB_Green, t)._Data;
            b = this.getSect(SectType.RGB_Blue, t)._Data;
        }

        public bool checkSect(SectType t)
        {
            return this._Data.ContainsKey(t);
        }

        public Sect getSect(SectType t, DataType dt)
        {
            Sect s;

            if (this._Data.TryGetValue(t, out s))
            {
                if (dt.isWrite())
                {
                    s.resetStats();
                }
            }
            else
            {
                s = new Sect(t, this.Width, this.Height);
                this._Data[t] = s;
            }

            return s;
        }

        public int getSectCount() { return this._Data.Count; }

        public Sect[] getSects(DataType dt)
        {
            var data = this._Data.Values.ToArray();

            if (dt.isWrite()) foreach (var ins in data) ins.resetStats();

            return data;
        }

        public SectType[] getSectTypes()
        {
            return this._Data.Keys.ToArray();
        }

        internal Sect getSectFirst(DataType dt)
        {
            var s = this._Data.First().Value;
            if (dt.isWrite()) s.resetStats();
            return s;
        }

        public bool addSect(Sect s)
        {
            if (this.Size == s._Size)
            {
                this._Data[s._Type] = s;
                return true;
            }
            else return false;
        }











        public static Byte castByte(float f)
        {
            return (Byte)Math.Max(0, Math.Min(255, f));
        }

        public struct ImageSectTypes
        {
            public SectType r, g, b;
            public Boolean valid;
            public ImageSectTypes(SectType R, SectType G, SectType B)
            {
                this.r = R;
                this.g = G; 
                this.b = B;
                this.valid = (R != SectType.NaN) || (G != SectType.NaN) || (B != SectType.NaN); 
            }
        }

        public bool getImageSect(SectType t, ref Single[,] dat)
        {
            Sect ect;
            if (t != SectType.NaN)
                if (this._Data.TryGetValue(t, out ect))
                {
                    dat = ect._Data;
                    return true;
                }
            return false;
        }

        public ImageSectTypes getImageSectTypes()
        {
            bool hR = this.checkSect(SectType.RGB_Red);
            bool hG = this.checkSect(SectType.RGB_Green);
            bool hB = this.checkSect(SectType.RGB_Blue);

            SectType t = SectType.NaN;

            if (hR || hG || hB)
            {
                return new ImageSectTypes(
                    hR ? SectType.RGB_Red : SectType.NaN,
                    hG ? SectType.RGB_Green : SectType.NaN,
                    hB ? SectType.RGB_Blue : SectType.NaN);
            }

            if (this.checkSect(SectType.Gray))
            {
                t = SectType.Gray;
            }
            else if (this.getSectCount() == 1)
            {
                t = this.getSectFirst(DataType.Read)._Type;
            }

            return new ImageSectTypes(t, t, t);
        }

        public unsafe Bitmap getImage(bool forceZero = true)
        {
            var t = this.getImageSectTypes();
            if (t.valid) return this.getImage(t.r, t.g, t.b, forceZero);
            else return null;
        }

        public unsafe Bitmap getImage(SectType r_t, SectType g_t, SectType b_t, bool forceZero = true)
        {
            Bitmap newB = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);

            this.setImage(ref newB, r_t, g_t, b_t, forceZero);

            return newB;
        }

        public unsafe Bitmap getImage(int w, int h, bool forceZero = true)
        {
            Bitmap newB = new Bitmap(w, h, PixelFormat.Format24bppRgb);

            var t = this.getImageSectTypes();

            if (t.valid)
            {
                this.setImage(ref newB, t.r, t.g, t.b, forceZero);
            }

            return newB;
        }

        public unsafe void setImage(ref Bitmap bmp)
        {
            var t = this.getImageSectTypes();
            if (t.valid) this.setImage(ref bmp, t.r, t.g, t.b, true);
        }


        public unsafe void setImage(ref Bitmap bmp, SectType r_t, SectType g_t, SectType b_t, bool forceZero = true)
        {
            if (bmp == null) bmp = this.getImage(r_t, g_t, b_t, forceZero);
            else
            {
                Single[,] r = null;
                Single[,] g = null;
                Single[,] b = null;

                Boolean hR = this.getImageSect(r_t, ref r);
                Boolean hG = this.getImageSect(g_t, ref g);
                Boolean hB = this.getImageSect(b_t, ref b);

                Single mult, offset;

                if (forceZero)
                {
                    mult = 255.0f;
                    offset = 0;
                }
                else
                {
                    Boolean p = this.min >= 0;
                    Boolean n = this.max <= 0;
                    mult = n ? -255.0f : 255.0f;
                    offset = (p || n) ? 0 : 128;
                }

                if (bmp.Size != this.Size)
                {
                    Rectangle rect = Sizing.fitAinB(new Size(this.Width, this.Height), bmp.Size);

                    BitmapData bmdNew = bmp.LockBits(
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);

                    byte* row;
                    int xx = 0, x;

                    float yAdj;
                    int yUp, yDown;

                    float xAdj;
                    int xUp, xDown;


                    // Nearest Neighbor [Expansion]
                    if (rect.Width > this.Width || rect.Height > this.Height)
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            yAdj = y * (this.Height - 1);
                            yAdj /= (bmp.Height - 1);
                            yUp = (int)Math.Round(yAdj, 0);

                            row = (Byte*)bmdNew.Scan0 + (y * bmdNew.Stride);

                            for (x = 0, xx = 0; x < bmp.Width; x++, xx += 3)
                            {
                                xAdj = x * (this.Width - 1);
                                xAdj /= (bmp.Width - 1);
                                xUp = (int)Math.Round(xAdj, 0);

                                row[xx + 2] = ImageData.castByte((hR ? r[yUp, xUp] : 0) * mult + offset);
                                row[xx + 1] = ImageData.castByte((hG ? g[yUp, xUp] : 0) * mult + offset);
                                row[xx + 0] = ImageData.castByte((hB ? b[yUp, xUp] : 0) * mult + offset);
                            }
                        }
                    }
                    // Bilinear [Compression]
                    else
                    {
                        Single fR = 0;
                        Single fG = 0;
                        Single fB = 0;
                        Single yAdj2 = 0;
                        Single xAdj2 = 0;

                        for (int y = 0; y < bmp.Height; y++)
                        {
                            yAdj = y * (this.Height - 1);
                            yAdj /= (bmp.Height - 1);
                            yAdj2 = yAdj % 1;

                            yUp = (int)Math.Ceiling((double)yAdj);
                            yDown = (int)yAdj;

                            row = (Byte*)bmdNew.Scan0 + (y * bmdNew.Stride);

                            for (x = 0, xx = 0; x < bmp.Width; x++, xx += 3)
                            {
                                xAdj = x * (this.Width - 1);
                                xAdj /= (bmp.Width - 1);
                                xAdj2 = xAdj % 1;
                                xUp = (int)Math.Ceiling((double)xAdj);
                                xDown = (int)xAdj;

                                if (xUp == xDown && yUp == yDown)
                                {
                                    fR = (hR ? r[yUp, xUp] : 0);
                                    fG = (hG ? g[yUp, xUp] : 0);
                                    fB = (hB ? b[yUp, xUp] : 0);
                                }
                                else if (xUp == xDown)
                                {
                                    fR = (hR ? ImageData.getLinearEstimate(r[yDown, xUp], r[yUp, xUp], yAdj2) : 0);
                                    fG = (hG ? ImageData.getLinearEstimate(g[yDown, xUp], g[yUp, xUp], yAdj2) : 0);
                                    fB = (hB ? ImageData.getLinearEstimate(b[yDown, xUp], b[yUp, xUp], yAdj2) : 0);
                                }
                                else if (yUp == yDown)
                                {
                                    fR = (hR ? ImageData.getLinearEstimate(r[yUp, xDown], r[yUp, xUp], xAdj2) : 0);
                                    fG = (hG ? ImageData.getLinearEstimate(g[yUp, xDown], g[yUp, xUp], xAdj2) : 0);
                                    fB = (hB ? ImageData.getLinearEstimate(b[yUp, xDown], b[yUp, xUp], xAdj2) : 0);
                                }
                                else
                                {
                                    fR = (hR ? ImageData.getLinearEstimate(
                                         ImageData.getLinearEstimate(r[yDown, xDown], r[yUp, xDown], yAdj2),
                                         ImageData.getLinearEstimate(r[yDown, xUp], r[yUp, xUp], yAdj2),
                                         xAdj2) : 0);
                                    fG = (hG ? ImageData.getLinearEstimate(
                                         ImageData.getLinearEstimate(g[yDown, xDown], g[yUp, xDown], yAdj2),
                                         ImageData.getLinearEstimate(g[yDown, xUp], g[yUp, xUp], yAdj2),
                                         xAdj2) : 0);
                                    fB = (hB ? ImageData.getLinearEstimate(
                                         ImageData.getLinearEstimate(b[yDown, xDown], b[yUp, xDown], yAdj2),
                                         ImageData.getLinearEstimate(b[yDown, xUp], b[yUp, xUp], yAdj2),
                                         xAdj2) : 0);
                                }

                                row[xx + 2] = ImageData.castByte(fR * mult + offset);
                                row[xx + 1] = ImageData.castByte(fG * mult + offset);
                                row[xx + 0] = ImageData.castByte(fB * mult + offset);
                            }
                        }
                    }

                    bmp.UnlockBits(bmdNew);
                }
                else
                {

                    BitmapData bmd = bmp.LockBits(
                    new Rectangle(0, 0, this.Width, this.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                    Byte* rowNew;
                    for (int y = 0; y < this.Height; y++)
                    {
                        rowNew = (byte*)bmd.Scan0 + (y * bmd.Stride);

                        for (int x = 0, xx = 0; x < this.Width; x++, xx += 3)
                        {
                            rowNew[xx + 2] = ImageData.castByte((hR ? r[y, x] : 0) * mult + offset);
                            rowNew[xx + 1] = ImageData.castByte((hG ? g[y, x] : 0) * mult + offset);
                            rowNew[xx + 0] = ImageData.castByte((hB ? b[y, x] : 0) * mult + offset);
                        }
                    }

                    bmp.UnlockBits(bmd);
                }
            }
        }




        private Image _ImageHist = null;
        private Boolean _ImageHistAuto = false;

        public Image Histogram(int height, bool autoscale)
        {
            if (this._ImageHist != null)
            {
                if (this._ImageHist.Height == height && autoscale == this._ImageHistAuto)
                    return this._ImageHist;

                this._ImageHist.Dispose();

            }

            this._ImageHist = HistogramViewer.histogram(this, height, autoscale);
            this._ImageHistAuto = autoscale;

            return this._ImageHist;
        }












        

























        /// <summary>
        /// Gets the linear estimate of y for an x value between 0 and 1
        /// </summary>
        /// <param name="y0">Value of function at x = 0</param>
        /// <param name="y1">Value of function at x = 1</param>
        /// <param name="x">X value between 0 and 1</param>
        /// <returns></returns>
        internal static float getLinearEstimate(float y0, float y1, float x)
        {
            return y0 + x * (y1 - y0);
        }

    }
}
