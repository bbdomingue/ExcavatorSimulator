using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HeadTrackerWrapper;

namespace SamSeifert.HeadTracker
{
    internal partial class HeadTrackerForm : Form
    {
        internal HeadTrackerForm()
        {
            InitializeComponent();
        }

        unsafe private void timer1_Tick(object sender, EventArgs e)
        {
            if (HeadTrackerManager._BoolKeepRunning)
            {
                int count = HeadTrackerManager._DataReturn.Length;
                int update = 0;

                fixed (float* data = HeadTrackerManager._DataReturn)
                {
                    fixed (uint* parms = HeadTrackerManager._ImageParams)
                    {
                        BitmapData bmpd = HeadTrackerManager._Bitmap.LockBits(
                            new Rectangle(Point.Empty, HeadTrackerManager._Bitmap.Size),
                            ImageLockMode.ReadWrite, HeadTrackerManager._Bitmap.PixelFormat);

                        update = HeadTrackerManager._HeadTrackerClass.update(
                            data, &count,
                            parms, (void*)(bmpd.Scan0));

                        HeadTrackerManager._Bitmap.UnlockBits(bmpd);

                        this.pictureBox1.Image = HeadTrackerManager._Bitmap;
                    }
                }

                OpenTK.Vector3 hd = HeadTrackerManager.update(update, count);

                this.label2.Text = hd.X.ToString("0.00");
                this.label4.Text = hd.Y.ToString("0.00");
                this.label6.Text = hd.Z.ToString("0.00");
            }
            else
            {
                this.timer1.Enabled = false;
                this.Close();
            }
        }

    }
}
