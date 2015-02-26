using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.GlobalEvents;

namespace Excavator
{
    public partial class ControlKeyboardCylinder : UserControl
    {
        public ControlKeyboardCylinder()
        {
            InitializeComponent();

            this.buttonKeyboardControl.GotFocus += new EventHandler(this.buttonKeyboardControl_GotFocus);
            this.buttonKeyboardControl.LostFocus += new EventHandler(this.buttonKeyboardControl_LostFocus);
        }

        public void Deconstruct()
        {
            this.buttonKeyboardControl.GotFocus -= new EventHandler(this.buttonKeyboardControl_GotFocus);
            this.buttonKeyboardControl.LostFocus -= new EventHandler(this.buttonKeyboardControl_LostFocus);
        }

        public void updateGui()
        {
            Color c1 = this._BoolIsKeyboarding ? Color.LawnGreen : Color.LightGray;
            Color c2 = Color.LightGray;

            this.labelKeyCab1.BackColor = GlobalEventHandler.isKeyPressed(Keys.D1) ? c1 : c2;
            this.labelKeyCab2.BackColor = GlobalEventHandler.isKeyPressed(Keys.D2) ? c1 : c2;

            this.labelKeyBucket1.BackColor = GlobalEventHandler.isKeyPressed(Keys.E) ? c1 : c2;
            this.labelKeyBucket2.BackColor = GlobalEventHandler.isKeyPressed(Keys.D) ? c1 : c2;

            this.labelKeyUp.BackColor = GlobalEventHandler.isKeyPressed(Keys.Up) ? c1 : c2;
            this.labelKeyDown.BackColor = GlobalEventHandler.isKeyPressed(Keys.Down) ? c1 : c2;

            this.labelKeyLeft.BackColor = GlobalEventHandler.isKeyPressed(Keys.Left) ? c1 : c2;
            this.labelKeyRight.BackColor = GlobalEventHandler.isKeyPressed(Keys.Right) ? c1 : c2;
        }

        private bool _BoolIsKeyboarding = false;

        private void buttonKeyboardControl_GotFocus(object sender, EventArgs e)
        {
            this._BoolIsKeyboarding = true;
            this.buttonKeyboardControl.Invalidate();
        }

        private void buttonKeyboardControl_LostFocus(object sender, EventArgs e)
        {
            this._BoolIsKeyboarding = false;
            this.buttonKeyboardControl.Invalidate();
        }

        private void buttonKeyboardControl_Paint(object sender, PaintEventArgs e)
        {
            Color c = this._BoolIsKeyboarding ? Color.Green : Color.Red;

            int border = 6;
            int dim = this.buttonKeyboardControl.Height - border * 2;
            Rectangle r = new Rectangle(border, border, dim, dim);

            e.Graphics.FillEllipse(new SolidBrush(c), r);
        }

        /// <summary>
        /// Cab, Bucket, U/D, L/R
        /// </summary>
        /// <returns></returns>
        public int[] getInts()
        {
            var ret = new int[] {0, 0, 0, 0, 0};

            if (this._BoolIsKeyboarding)
            {
                if (GlobalEventHandler.isKeyPressed(Keys.D1)) ret[1]++;
                if (GlobalEventHandler.isKeyPressed(Keys.D2)) ret[1]--;

                if (GlobalEventHandler.isKeyPressed(Keys.E)) ret[2]++;
                if (GlobalEventHandler.isKeyPressed(Keys.D)) ret[2]--;

                if (GlobalEventHandler.isKeyPressed(Keys.Up)) ret[3]++;
                if (GlobalEventHandler.isKeyPressed(Keys.Down)) ret[3]--;

                if (GlobalEventHandler.isKeyPressed(Keys.Left)) ret[4]++;
                if (GlobalEventHandler.isKeyPressed(Keys.Right)) ret[4]--;

            }

            return ret;
        }
    }
}
