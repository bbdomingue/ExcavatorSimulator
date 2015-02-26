using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SamSeifert.HeadTracker
{
    internal partial class HeadTrackerTester : Form
    {
        public HeadTrackerTester()
        {
            InitializeComponent();
            HeadTrackerManager.ThreadStarted += new HeadTrackEventHandler(this.HeadTrackerManager_ThreadStarted);
            HeadTrackerManager.ThreadEnded += new HeadTrackEventHandler(this.HeadTrackerManager_ThreadEnded);
        }

        private int threadCount = 0;
        private bool _BoolFormClosing = false;

        private void HeadTrackerManager_ThreadStarted() // Not this thread
        {
            this.Invoke((MethodInvoker)delegate
            {
                threadCount++;
            });
        }

        private void HeadTrackerManager_ThreadEnded() // Not this thread
        {
            this.Invoke((MethodInvoker)delegate
            {
                threadCount--;
                if (this.threadCount == 0 && this._BoolFormClosing)
                {
                    this.Close();
                }
            });
        }

        private void FormTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            HeadTrackerManager.Stop();
            this._BoolFormClosing = true;
            this.Enabled = false;
            e.Cancel = this.threadCount > 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HeadTrackerManager.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HeadTrackerManager.Stop();
        }

        private void FormTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            HeadTrackerManager.End();
        }
    }
}
