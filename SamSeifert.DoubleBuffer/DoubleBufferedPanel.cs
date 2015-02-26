using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SamSeifert.DoubleBuffer
{
    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
        }

        private Bitmap _Bitmap;

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            if (this._Bitmap != null) this._Bitmap.Dispose();
            this._Bitmap = new Bitmap(base.Width, base.Height, PixelFormat.Format24bppRgb);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
//            base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var g = Graphics.FromImage(this._Bitmap))
            {
                base.OnPaint(new PaintEventArgs(g,
                    new Rectangle(0, 0, this._Bitmap.Width, this._Bitmap.Height)));
            }

            if (this.DesignMode) return;
            e.Graphics.DrawImage(this._Bitmap, 0, 0);
        }
    }
}
