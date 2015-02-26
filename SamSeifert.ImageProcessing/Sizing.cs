using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SamSeifert.ImageProcessing
{
    public class Sizing
    {
        public static Rectangle fitAinB(Size A, Size B)
        {
            Rectangle _Rectangle = new Rectangle();

            if (A.Width * A.Height == 0) return new Rectangle();

            if (A.Width * B.Height > B.Width * A.Height)
            {
                // image wider than screen
                _Rectangle = new Rectangle(0, 0, B.Width, B.Width * A.Height / A.Width);
                _Rectangle.Y = (B.Height - _Rectangle.Height) / 2;
            }
            else
            {
                // image taller than screen
                _Rectangle = new Rectangle(0, 0, B.Height * A.Width / A.Height, B.Height);
                _Rectangle.X = (B.Width - _Rectangle.Width) / 2;
            }

            return _Rectangle;
        }

/*
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Image != null)
            {
                if (this.Width > this.Image.Width || this.Height > this.Image.Height)
                    e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                else
                    e.Graphics.InterpolationMode = InterpolationMode.Bilinear;

                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                Rectangle r = Sizing.fitAinB(this.Image.Size, this.Size);
                e.Graphics.DrawImage(this.Image, r);
            }
        }
*/
    }
}
