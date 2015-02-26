using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public struct PointD
    {
        public double X;
        public double Y;

        public PointD(double theta)
        {
            this.X = Math.Cos(theta);
            this.Y = Math.Sin(theta);
        }

        public PointD(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public static PointD Empty
        {
            get
            {
                return new PointD();
            }
        }

        public double getLength()
        {
            return Math.Sqrt(this.getLength2());
        }

        public double getLength2()
        {
            return this.X * this.X + this.Y * this.Y;
        }

        public double Normalize()
        {
            double f = this.getLength();
            if (f > 0)
            {
                this.X /= f;
                this.Y /= f;
            }
            return f;
        }

        public double NormalizeIfGreaterThanUnit()
        {
            double f = this.getLength();
            if (f > 1)
            {
                this.X /= f;
                this.Y /= f;
                return 1;
            }
            else return f;
        }


        public override string ToString()
        {
            return "PointD(" + this.X + "," + this.Y + ")";
        }



        public static double Dot(PointD a, PointD b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static double Cross(PointD a, PointD b)
        {
            return a.X * b.Y - b.X * a.Y;
        }

        public static PointD SubtractAFromB(PointD a, PointD b)
        {
            return new PointD(b.X - a.X, b.Y - a.Y);
        }

        public static PointD AddAToB(PointD a, PointD b)
        {
            return new PointD(b.X + a.X, b.Y + a.Y);
        }











        public void add(PointD p)
        {
            this.X += p.X;
            this.Y += p.Y;
        }

        public void multiply(double d)
        {
            this.X *= d;
            this.Y *= d;
        }

        public void divide(double d)
        {
            this.X /= d;
            this.Y /= d;
        }
    }
}
