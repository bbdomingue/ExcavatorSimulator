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
    public class T_FlowKeyboard : Trial
    {
        private bool _BoolUseGhost = false;
        private bool _BoolDynamic = false;

        private ControlKeyboard controlKeyboard1 = new ControlKeyboard();

        private CheckBox checkBoxGhost = new CheckBox();
        private CheckBox checkBoxDynamic = new CheckBox();


        /// <summary>
        /// ONLY FOR INTERFACE BUILDER NEVER USE THIS SUCKER
        /// </summary>
        public T_FlowKeyboard() : base()
        {
        }

        public override void Deconstruct()
        {
            base.Deconstruct();
            this.controlKeyboard1.Deconstruct();

            this.checkBoxDynamic.CheckedChanged -= new EventHandler(this.checkBoxDynamic_CheckedChanged);
            this.checkBoxGhost.CheckedChanged -= new EventHandler(this.checkBoxGhost_CheckedChanged);
        }

        public T_FlowKeyboard(FormBase fb) : base(fb)
        {
            this.checkBoxDynamic.CheckedChanged += new EventHandler(this.checkBoxDynamic_CheckedChanged);
            this.checkBoxGhost.CheckedChanged += new EventHandler(this.checkBoxGhost_CheckedChanged);

            int i = 0;
            foreach (CheckBox cb in new CheckBox[] { this.checkBoxGhost, this.checkBoxDynamic })
            {
                switch (i++)
                {
                    case 0:
                        cb.Text = "Ghost";
                        cb.Enabled = false;
                        break;
                    case 1:
                        cb.Text = "Dynamic";
                        break;
                }

                cb.AutoSize = false;
                cb.Height = 30;
                cb.Padding = new Padding(10, 5, 10, 5);
                cb.Dock = DockStyle.Top;
                this.Controls.Add(cb);
            }

            this.controlKeyboard1.Dock = DockStyle.Top;
            this.controlKeyboard1.noSwing();
            this.Controls.Add(this.controlKeyboard1);

            this.nudArm.Enabled = true;
            this.nudBoom.Enabled = true;
            this.nudBucket.Enabled = true;
            this.nudCab.Enabled = true;
            this.nudSwing.Enabled = true;
        }










        public override bool hasGhost()
        {
            return this._BoolUseGhost;
        }

        public override string getName()
        {
            return "Flow Control Keyboard";
        }










        public override void updateSim()
        {
            this.GhostAngles.swi = (float)this.nudSwing.Value;
            this.GhostAngles.cab = (float)this.nudCab.Value;
            this.GhostAngles.boo = (float)this.nudBoom.Value;
            this.GhostAngles.arm = (float)this.nudArm.Value;
            this.GhostAngles.buc = (float)this.nudBucket.Value;

            float inc = this._TimeSpanF * 50.0f;

            var fts = this.controlKeyboard1.getInts();

            this.GhostAngles.swi = this.clampQS(this.GhostAngles.swi + fts[0] * inc);

            this.GhostAngles.cab = this.GhostAngles.cab + fts[1] * inc;
            if (this.GhostAngles.cab < -180) this.GhostAngles.cab += 360;
            if (this.GhostAngles.cab > 180) this.GhostAngles.cab -= 360;

            this.GhostAngles.boo = this.clampQ2(this.GhostAngles.boo + fts[2] * inc);
            this.GhostAngles.arm = this.clampQ3(this.GhostAngles.arm + fts[3] * inc);
            this.GhostAngles.buc = this.clampQ4(this.GhostAngles.buc + fts[4] * inc);

            this.nudSwing.Value = (decimal)this.GhostAngles.swi;
            this.nudCab.Value = (decimal)this.GhostAngles.cab;
            this.nudBoom.Value = (decimal)this.GhostAngles.boo;
            this.nudArm.Value = (decimal)this.GhostAngles.arm;
            this.nudBucket.Value = (decimal)this.GhostAngles.buc;

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

        public override void  Gui_Draw_Tick()
        {
            this.controlKeyboard1.updateGui();
        }
    }
}
