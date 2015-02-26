using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public class Blob
    {
        public Size Size = new Size(0, 0);

        public int Height { get { return this.Size.Height; } }
        public int Width { get { return this.Size.Width; } }

        public const byte RegionBackground = 0;


        public class RegionProps
        {
            public byte type;
            public int target;

            public int PixelCount = 0;
            public long AverageX = 0;
            public long AverageY = 0;

            public void add(RegionProps r)
            {
                if (r.type != this.type) Console.WriteLine("DUCK");

                this.PixelCount += r.PixelCount;
                this.AverageX += r.AverageX;
                this.AverageY += r.AverageY;

                r.PixelCount = 0;
                r.AverageX = 0;
                r.AverageY = 0;
            }
        }

        public Byte[,] _ColData;
        public UInt16[,] _RegionData;
        public RegionProps[] _RegionProps;

        public Blob(Size s)
        {
            this.Size = s;

            this._ColData = new Byte[this.Height, this.Width];
            this._RegionData = new UInt16[this.Height, this.Width];
        }


        public delegate Byte getRegionType(Single[][,] data, int y, int x);

        public static bool getBlobFor(ImageData id, SectType[] sTypes, Blob.getRegionType rm, ref Blob b)
        {
            if (id == null) return false;
            else if (sTypes == null) return false;

            Single[][,] data = new Single[sTypes.Length][,];

            for (int i = 0; i < sTypes.Length; i++) data[i] = id.getSect(sTypes[i], DataType.Read)._Data;

            bool createNew = false;

            if (b == null) createNew = true;
            else if (b.Size != id.Size) createNew = true;
            
            if (createNew) b = new Blob(id.Size);


            UInt16 pmatchy = 0;
            UInt16 pmatchx = 0;
            const UInt16 int16_0 = 0;
            UInt16 outp = 0;

            var masks = new List<UInt16>();
            var rprops = new List<RegionProps>();
            masks.Add(int16_0);
            RegionProps temp = new RegionProps();
            rprops.Add(temp);

            Byte r;

            for (int y = 0; y < b.Height; y++)
            {
                int x8 = 0;
                int x16 = 0;

                for (int x = 0; x < b.Width; x++)
                {
                    r = rm(data, y, x);

                    if (r == Blob.RegionBackground)
                    {
                        b._ColData[y, x] = 0;
                        b._RegionData[y, x] = int16_0;
                    }
                    else
                    {
                        pmatchy = (y > 0) ? (r == b._ColData[y - 1, x] ? b._RegionData[y - 1, x] : int16_0) : int16_0;
                        pmatchx = (x > 0) ? (r == b._ColData[y, x - 1] ? b._RegionData[y, x - 1] : int16_0) : int16_0;

                        if (pmatchx == int16_0 && pmatchy == int16_0)
                        {
                            outp = (UInt16)masks.Count;
                            masks.Add(outp);
                            temp = new RegionProps();
                            temp.type = r;
                            rprops.Add(temp);
                        }
                        else if (pmatchx == int16_0) outp = pmatchy;
                        else if (pmatchy == int16_0) outp = pmatchx;
                        else if (pmatchx == pmatchy) outp = pmatchx;
                        else
                        {
                            UInt16 bigger = Math.Max(pmatchx, pmatchy);
                            outp = Math.Min(pmatchx, pmatchy);

                            while (true)
                            {
                                UInt16 biggerLast = bigger;
                                bigger = masks[bigger];

                                if (outp == bigger) break;
                                else if (bigger == biggerLast)
                                {
                                    masks[bigger] = outp;
                                    break;
                                }
                                else
                                {
                                    if (outp > bigger)
                                    {
                                        UInt16 t16 = masks[outp];

                                        masks[biggerLast] = outp;
                                        masks[outp] = bigger;
                                        bigger = outp;

                                        if (outp == t16) break;
                                        else outp = t16;
                                    }
                                }
                            }
                        }

                        while (outp != masks[outp]) outp = masks[outp];

                        temp = rprops[outp];

                        temp.PixelCount++;
                        temp.AverageX += x;
                        temp.AverageY += y;

                        b._ColData[y, x] = r;
                        b._RegionData[y, x] = outp;
                    }

                    x8 += 4;
                    x16 += 2;
                }
            }

            var masksa = masks.ToArray();

            for (UInt16 i = (UInt16)(masksa.Length - 1); true; i--)
            {
                UInt16 mk = masksa[i];

                if (mk != i)
                {
                    rprops[mk].add(rprops[i]);
                    rprops.RemoveAt(i);

                    for (int ii = i; ii < masksa.Length; ii++)
                    {
                        UInt16 mka = masksa[ii];
                        if (mka == i) masksa[ii] = mk;
                        else if (mka > i) masksa[ii]--;
                    }
                }

                if (i == int16_0) break;
            }

            b._RegionProps = rprops.ToArray();

            for (int i = 1; i < b._RegionProps.Length; i++)
            {
                RegionProps rp = b._RegionProps[i];
                var c = Math.Max(1, rp.PixelCount);
                rp.AverageX /= c;
                rp.AverageY /= c;
            }

            for (int y = 0; y < b.Height; y++)
            {
                for (int x = 0; x < b.Width; x++)
                {
                    b._RegionData[y, x] = masksa[b._RegionData[y, x]];
                }
            }


            return true;

/*            temp = new RegionProps();
            temp.count = 1;
            RegionProps biggestRed = temp;
            RegionProps biggestRed2 = temp;
            RegionProps biggestGreen = temp;
            RegionProps biggestGreen2 = temp;
            RegionProps biggestBlue = temp;
            RegionProps biggestBlue2 = temp;

            foreach (RegionProps rp in rprops)
            {
                switch (rp.type)
                {
                    case regionR:
                        {
                            if (rp.count > biggestRed.count)
                            {
                                biggestRed2 = biggestRed;
                                biggestRed = rp;
                            }
                            else if (rp.count > biggestRed2.count)
                            {
                                biggestRed2 = rp;
                            }
                            break;
                        }
                    case regionG:
                        {
                            if (rp.count > biggestGreen.count)
                            {
                                biggestGreen2 = biggestGreen;
                                biggestGreen = rp;
                            }
                            else if (rp.count > biggestGreen2.count)
                            {
                                biggestGreen2 = rp;
                            }
                            break;
                        }
                    case regionB:
                        {
                            if (rp.count > biggestBlue.count)
                            {
                                biggestBlue2 = biggestBlue;
                                biggestBlue = rp;
                            }
                            else if (rp.count > biggestBlue2.count)
                            {
                                biggestBlue2 = rp;
                            }
                            break;
                        }
                }
            }

            this.biggestRedPoint = new PointD(biggestRed.aX, biggestRed.aY);
            this.biggestRedPoint.divide(biggestRed.count);
            this.biggestRed2Point = new PointD(biggestRed2.aX, biggestRed2.aY);
            this.biggestRed2Point.divide(biggestRed2.count);
            this.biggestGreenPoint = new PointD(biggestGreen.aX, biggestGreen.aY);
            this.biggestGreenPoint.divide(biggestGreen.count);
            this.biggestGreen2Point = new PointD(biggestGreen2.aX, biggestGreen2.aY);
            this.biggestGreen2Point.divide(biggestGreen2.count);
            this.biggestBluePoint = new PointD(biggestBlue.aX, biggestBlue.aY);
            this.biggestBluePoint.divide(biggestBlue.count);
            this.biggestBlue2Point = new PointD(biggestBlue2.aX, biggestBlue2.aY);
            this.biggestBlue2Point.divide(biggestBlue2.count);*/
        }

    }
}
