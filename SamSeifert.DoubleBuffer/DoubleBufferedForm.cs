using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SamSeifert.DoubleBuffer
{
    public partial class DoubleBufferedForm : Form
    {
        public DoubleBufferedForm()
        {
            InitializeComponent();
        }

        private Bitmap _Bitmap;
        private Graphics _Graphics;
        private PaintEventArgs _PaintEventArgs;

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            this._Bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            this._Graphics = Graphics.FromImage(this._Bitmap);
            this._PaintEventArgs = new PaintEventArgs(this._Graphics, new Rectangle(
                0,
                0,
                this.ClientSize.Width,
                this.ClientSize.Height));
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //            base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this._Graphics.ResetClip();
            this._Graphics.ResetTransform();
            base.OnPaint(this._PaintEventArgs);
            e.Graphics.DrawImage(this._Bitmap, 0, 0);
        }
    }
}
