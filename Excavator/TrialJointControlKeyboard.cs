using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.GlobalEvents;

using OpenTK.Graphics.OpenGL;

namespace Excavator
{
    internal partial class TrialJointControlKeyboard : Trial
    {
        private bool _BoolIsKeyboarding = false;

        private bool _BoolUseGhost = false;
        private bool _BoolDynamic = false;


        /// <summary>
        /// ONLY FOR INTERFACE BUILDER NEVER USE THIS SUCKER
        /// </summary>
        public TrialJointControlKeyboard() : base()
        {
            InitializeComponent();
        }

        public TrialJointControlKeyboard(FormBase fb) : base(fb)
        {
            InitializeComponent();

            this.buttonKeyboardControl.GotFocus += new EventHandler(this.buttonKeyboardControl_GotFocus);
            this.buttonKeyboardControl.LostFocus += new EventHandler(this.buttonKeyboardControl_LostFocus);

            this.nudArm.Enabled = true;
            this.nudBoom.Enabled = true;
            this.nudBucket.Enabled = true;
            this.nudCab.Enabled = true;
            this.nudSwing.Enabled = true;
        }

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












        internal override bool hasGhost()
        {
            return this._BoolUseGhost;
        }

        internal override string getName()
        {
            return "Joint Control Keyboard";
        }










        private int _IntCountUpdateGui = 0;
        private int _IntCountUpdateGuiRefresh = 10;
        internal override void updateSim()
        {
            if (this._IntCountUpdateGui++ == this._IntCountUpdateGuiRefresh)
            {
                this._IntCountUpdateGui = 0;
                this.updateControlGUI();
            }

            if (this._BoolIsKeyboarding)
            {
                decimal inc = (decimal)(this._TimeSpanF * 50.0);

                decimal cR = this.nudCab.Value;
                decimal t1 = this.nudSwing.Value;
                decimal t2 = this.nudBoom.Value;
                decimal t3 = this.nudArm.Value;
                decimal t4 = this.nudBucket.Value;

                if (GlobalEventHandler.isKeyPressed(Keys.D3)) t1 += inc;
                if (GlobalEventHandler.isKeyPressed(Keys.D4)) t1 -= inc;

                if (GlobalEventHandler.isKeyPressed(Keys.D1)) cR += inc;
                if (GlobalEventHandler.isKeyPressed(Keys.D2)) cR -= inc;

                if (GlobalEventHandler.isKeyPressed(Keys.Q)) t2 += inc;
                if (GlobalEventHandler.isKeyPressed(Keys.W)) t2 -= inc;

                if (GlobalEventHandler.isKeyPressed(Keys.A)) t3 += inc;
                if (GlobalEventHandler.isKeyPressed(Keys.S)) t3 -= inc;

                if (GlobalEventHandler.isKeyPressed(Keys.Z)) t4 += inc;
                if (GlobalEventHandler.isKeyPressed(Keys.X)) t4 -= inc;

                this.nudCab.Value =
                    Math.Max(this.nudCab.Minimum,
                    Math.Min(this.nudCab.Maximum, cR));

                this.nudSwing.Value =
                    Math.Max(this.nudSwing.Minimum,
                    Math.Min(this.nudSwing.Maximum, t1));

                this.nudBoom.Value =
                    Math.Max(this.nudBoom.Minimum,
                    Math.Min(this.nudBoom.Maximum, t2));

                this.nudArm.Value =
                    Math.Max(this.nudArm.Minimum,
                    Math.Min(this.nudArm.Maximum, t3));

                this.nudBucket.Value =
                    Math.Max(this.nudBucket.Minimum,
                    Math.Min(this.nudBucket.Maximum, t4));

                this.GhostAngles.cab = (float)this.nudCab.Value;
                this.GhostAngles.swi = (float)this.nudSwing.Value;
                this.GhostAngles.boo = (float)this.nudBoom.Value;
                this.GhostAngles.arm = (float)this.nudArm.Value;
                this.GhostAngles.buc = (float)this.nudBucket.Value;

                if (this._BoolDynamic)
                {
                    const float offset = 6;
                    float temp;
                    
                    temp = this.GhostAngles.cab - this.ActualAngles.cab;
                    if (temp != 0)
                    {
                        this.ActualAngles.cab += (temp + offset * Math.Sign(temp)) * this._TimeSpanF;
                        if (temp > 0 & this.ActualAngles.cab > this.GhostAngles.cab) this.ActualAngles.cab = this.GhostAngles.cab;
                        if (temp < 0 & this.ActualAngles.cab < this.GhostAngles.cab) this.ActualAngles.cab = this.GhostAngles.cab;
                    }
                    temp = this.GhostAngles.swi - this.ActualAngles.swi;
                    if (temp != 0)
                    {
                        this.ActualAngles.swi += (temp + offset * Math.Sign(temp)) * this._TimeSpanF;
                        if (temp > 0 & this.ActualAngles.swi > this.GhostAngles.swi) this.ActualAngles.swi = this.GhostAngles.swi;
                        if (temp < 0 & this.ActualAngles.swi < this.GhostAngles.swi) this.ActualAngles.swi = this.GhostAngles.swi;
                    }
                    temp = this.GhostAngles.boo - this.ActualAngles.boo;
                    if (temp != 0)
                    {
                        this.ActualAngles.boo += (temp + offset * Math.Sign(temp)) * this._TimeSpanF;
                        if (temp > 0 & this.ActualAngles.boo > this.GhostAngles.boo) this.ActualAngles.boo = this.GhostAngles.boo;
                        if (temp < 0 & this.ActualAngles.boo < this.GhostAngles.boo) this.ActualAngles.boo = this.GhostAngles.boo;
                    }
                    temp = this.GhostAngles.arm - this.ActualAngles.arm;
                    if (temp != 0)
                    {
                        this.ActualAngles.arm += (temp + offset * Math.Sign(temp)) * this._TimeSpanF;
                        if (temp > 0 & this.ActualAngles.arm > this.GhostAngles.arm) this.ActualAngles.arm = this.GhostAngles.arm;
                        if (temp < 0 & this.ActualAngles.arm < this.GhostAngles.arm) this.ActualAngles.arm = this.GhostAngles.arm;
                    }
                    temp = this.GhostAngles.buc - this.ActualAngles.buc;
                    if (temp != 0)
                    {
                        this.ActualAngles.buc += (temp + offset * Math.Sign(temp)) * this._TimeSpanF;
                        if (temp > 0 & this.ActualAngles.buc > this.GhostAngles.buc) this.ActualAngles.buc = this.GhostAngles.buc;
                        if (temp < 0 & this.ActualAngles.buc < this.GhostAngles.buc) this.ActualAngles.buc = this.GhostAngles.buc;
                    }

                }
                else this.ActualAngles.copy(this.GhostAngles);
            }
        }





        private void ledKeyboard_Paint(object sender, PaintEventArgs e)
        {
            Color c = this._BoolIsKeyboarding ? Color.Green : Color.Red;

            int border = 6;
            int dim = this.buttonKeyboardControl.Height - border * 2;
            Rectangle r = new Rectangle(border, border, dim, dim);

            e.Graphics.FillEllipse(new SolidBrush(c), r);
        }

        private void updateControlGUI()
        {

            Color c1 = this._BoolIsKeyboarding ? Color.LawnGreen : Color.LightGray;
            Color c2 = Color.LightGray;

            this.labelKeySwing1.BackColor = GlobalEventHandler.isKeyPressed(Keys.D3) ? c1 : c2;
            this.labelKeySwing2.BackColor = GlobalEventHandler.isKeyPressed(Keys.D4) ? c1 : c2;

            this.labelKeyCab1.BackColor = GlobalEventHandler.isKeyPressed(Keys.D1) ? c1 : c2;
            this.labelKeyCab2.BackColor = GlobalEventHandler.isKeyPressed(Keys.D2) ? c1 : c2;

            this.labelKeyBoom1.BackColor = GlobalEventHandler.isKeyPressed(Keys.Q) ? c1 : c2;
            this.labelKeyBoom2.BackColor = GlobalEventHandler.isKeyPressed(Keys.W) ? c1 : c2;

            this.labelKeyArm1.BackColor = GlobalEventHandler.isKeyPressed(Keys.A) ? c1 : c2;
            this.labelKeyArm2.BackColor = GlobalEventHandler.isKeyPressed(Keys.S) ? c1 : c2;

            this.labelKeyBucket1.BackColor = GlobalEventHandler.isKeyPressed(Keys.Z) ? c1 : c2;
            this.labelKeyBucket2.BackColor = GlobalEventHandler.isKeyPressed(Keys.X) ? c1 : c2;
        }

        private void checkBoxGhost_CheckedChanged(object sender, EventArgs e)
        {
            this._BoolUseGhost = this.checkBoxGhost.Checked;
        }

        private void checkBoxDynamic_CheckedChanged(object sender, EventArgs e)
        {
            this._BoolDynamic = this.checkBoxDynamic.Checked;

            this.checkBoxGhost.Checked &= this._BoolDynamic;
            this.checkBoxGhost.Enabled = this._BoolDynamic;
            this._BoolUseGhost = this.checkBoxGhost.Checked;
        }
    }
}
