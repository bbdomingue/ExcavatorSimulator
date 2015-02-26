using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public partial class HistogramViewer
    {
        internal static unsafe Bitmap histogram(ImageData indata, int testHeight, Boolean autoscale)
        {
            int[,] histogram;

            HistogramViewer.fillHistogram(indata, out histogram);

            int x;
            const int picWidth = 511;

            testHeight = Math.Max(1, testHeight);
            Bitmap _Bitmap = new Bitmap(picWidth, testHeight, PixelFormat.Format24bppRgb);

            int max = 1;

            if (autoscale)
            {
                for (int c = 0; c < 3; c++)
                    for (x = 0; x < picWidth; x++)
                        max = Math.Max(max, histogram[c, x]);
            }
            else max = Math.Max(max, indata.Width * indata.Height * 10 / 255);
            
            for (int c = 0; c < 3; c++)
                for (x = 0; x < picWidth; x++)
                    histogram[c, x] = ((histogram[c, x]) * testHeight) / max;

            BitmapData bmd = _Bitmap.LockBits(
                new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, _Bitmap.PixelFormat);

            byte* row;
            int xx = 0;

            for (int c = 0; c < 3; c++)
            {
                for (int y = 0; y < bmd.Height; y++)
                {
                    row = (byte*)bmd.Scan0 + (y * bmd.Stride);

                    for (x = 0, xx = 2 - c; x < bmd.Width; x++, xx += 3)
                    {
                        if (y < histogram[c, x]) row[xx] = 255;
                        else row[xx] = 0;
                    }
                }
            }

            _Bitmap.UnlockBits(bmd);

            _Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return _Bitmap;
        }

        private static void fillHistogram(ImageData indata, out int[, ] counts)
        {
            int x, y;
            int w = indata.Width;
            int h = indata.Height;

            counts = new int[3, 511];

            var ts = indata.getImageSectTypes();

            if (!ts.valid) return;

            Single[,] r = null;
            Single[,] g = null;
            Single[,] b = null;

            indata.getImageSect(ts.r, ref r);
            indata.getImageSect(ts.g, ref g);
            indata.getImageSect(ts.b, ref b);

            for (int c = 0; c < 3; c++)
            {
                var inpArray = (c == 0) ? r : (c == 1) ? g : b;

                if (inpArray != null)
                {
                    for (y = 0; y < h; y++)
                    {
                        for (x = 0; x < w; x++)
                        {
                            counts[c, 255 + ImageAlgorithms.cast(inpArray[y, x] * 255.0f)]++;
                        }
                    }
                }
            }
        }
    }
}
