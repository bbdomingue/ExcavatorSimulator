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
    public partial class ControlKeyboard : UserControl
    {
        public ControlKeyboard()
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

            if (this._BoolSwing)
            {
                this.labelKeySwing1.BackColor = GlobalEventHandler.isKeyPressed(Keys.D3) ? c1 : c2;
                this.labelKeySwing2.BackColor = GlobalEventHandler.isKeyPressed(Keys.D4) ? c1 : c2;
            }

            this.labelKeyCab1.BackColor = GlobalEventHandler.isKeyPressed(Keys.D1) ? c1 : c2;
            this.labelKeyCab2.BackColor = GlobalEventHandler.isKeyPressed(Keys.D2) ? c1 : c2;

            this.labelKeyBoom1.BackColor = GlobalEventHandler.isKeyPressed(Keys.Q) ? c1 : c2;
            this.labelKeyBoom2.BackColor = GlobalEventHandler.isKeyPressed(Keys.A) ? c1 : c2;

            this.labelKeyArm1.BackColor = GlobalEventHandler.isKeyPressed(Keys.W) ? c1 : c2;
            this.labelKeyArm2.BackColor = GlobalEventHandler.isKeyPressed(Keys.S) ? c1 : c2;

            this.labelKeyBucket1.BackColor = GlobalEventHandler.isKeyPressed(Keys.E) ? c1 : c2;
            this.labelKeyBucket2.BackColor = GlobalEventHandler.isKeyPressed(Keys.D) ? c1 : c2;
        }

        public void noSwing()
        {
            int alpha = 150;
            this._BoolSwing = false;
            this.labelKeySwing1.BackColor = Color.FromArgb(alpha, Color.LightGray);
            this.labelKeySwing2.BackColor = Color.FromArgb(alpha, Color.LightGray);
            this.labelKeySwing1.Enabled = false;
            this.labelKeySwing2.Enabled = false;
        }

        private bool _BoolSwing = true;
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

        public int[] getInts()
        {
            var ret = new int[] {0, 0, 0, 0, 0};

            if (this._BoolIsKeyboarding)
            {
                if (this._BoolSwing)
                {
                    if (GlobalEventHandler.isKeyPressed(Keys.D3)) ret[0]++;
                    if (GlobalEventHandler.isKeyPressed(Keys.D4)) ret[0]--;
                }

                if (GlobalEventHandler.isKeyPressed(Keys.D1)) ret[1]++;
                if (GlobalEventHandler.isKeyPressed(Keys.D2)) ret[1]--;

                if (GlobalEventHandler.isKeyPressed(Keys.Q)) ret[2]++;
                if (GlobalEventHandler.isKeyPressed(Keys.A)) ret[2]--;

                if (GlobalEventHandler.isKeyPressed(Keys.W)) ret[3]++;
                if (GlobalEventHandler.isKeyPressed(Keys.S)) ret[3]--;

                if (GlobalEventHandler.isKeyPressed(Keys.E)) ret[4]++;
                if (GlobalEventHandler.isKeyPressed(Keys.D)) ret[4]--;
            }

            return ret;
        }
    }
}
