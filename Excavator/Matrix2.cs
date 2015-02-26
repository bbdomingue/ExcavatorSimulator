using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace Excavator
{
    public class Matrix2
    {
        // [a b]
        // [c d]
        private float a, b, c, d;

        public Vector2 Transform(Vector2 inp)
        {
            return new Vector2(inp.X * this.a + inp.Y * this.b, inp.X * this.c + inp.Y * this.d);
        }

        private Matrix2(float A, float B, float C, float D)
        {
            this.a = A;
            this.b = B;
            this.c = C;
            this.d = D;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1">First Row of Matrix</param>
        /// <param name="v2">Second Row of Matrix</param>
        /// <returns></returns>
        public static Matrix2 fromRows(Vector2 v1, Vector2 v2)
        {
            return new Matrix2(v1.X, v1.Y, v2.X, v2.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1">First Column of Matrix</param>
        /// <param name="v2">Second Column of Matrix</param>
        /// <returns></returns>
        public static Matrix2 fromCols(Vector2 v1, Vector2 v2)
        {
            return new Matrix2(v1.X, v2.X, v1.Y, v2.Y);
        }

        private void Multiply(float m)
        {
            this.a *= m;
            this.b *= m;
            this.c *= m;
            this.d *= m;
        }

        public bool invert(out Matrix2 m)
        {
            float det = this.a * this.d - this.b * this.c;

            if (det == 0)
            {
                m = new Matrix2(0, 0, 0, 0);
                return false;
            }
            else
            {
                m = new Matrix2(this.d, -this.b, -this.c, this.a);
                m.Multiply(1 / det);
                return true;
            }
        }
    }
}
