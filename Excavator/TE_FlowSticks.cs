using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MatlabExcavatorWrapper;

namespace Excavator
{
    public class TE_FlowSticks :  TrialEmbedBase
    {
        private volatile float T1 = 0.0f;
        private volatile float T2 = 0.0f;
        private volatile float T3 = 0.0f;
        private volatile float T4 = 0.0f;

        private Panel panelSpacer1 = new Panel();

        public TE_FlowSticks()
        {
        }

        public TE_FlowSticks(FormBase fb, string saveFile, int minutes)
            : base(fb, false, saveFile, minutes)
        {
            this.panelSpacer1.Size = new Size(100, 3);
            this.Controls.Add(this.panelSpacer1);
            this.panelSpacer1.Dock = DockStyle.Top;
            this.panelSpacer1.SendToBack();
            this.panelSpacer1.BackColor = fb.BackColor;

            this.Controls.Add(ControlStick._ControlStick);
            ControlStick._ControlStick.Dock = DockStyle.Top;
            ControlStick._ControlStick.SendToBack();
        }

        public override bool hasGhost()
        {
            return false;
        }

        public override string getName()
        {
            return "Flow with Sticks";
        }




        public override void Gui_Label_Tick(float accumulator)
        {
            base.Gui_Label_Tick(accumulator);
            ControlStick._ControlStick.updateFPS(accumulator);
        }

        public override void Gui_Draw_Tick()
        {
            base.Gui_Draw_Tick();
            ControlStick._ControlStick.updateControlGUI();
        }


        public override void Deconstruct()
        {
            base.Deconstruct();
        }






        public override void MatlabUpdateSim()
        {
            base.MatlabUpdateSim();

            this.T1 = -ControlStick._ControlStick.getValForStick(ControlStick.l_LR, true);
            this.T2 = ControlStick._ControlStick.getValForStick(ControlStick.r_FB, true);
            this.T3 = ControlStick._ControlStick.getValForStick(ControlStick.l_FB, true);
            this.T4 = -ControlStick._ControlStick.getValForStick(ControlStick.r_LR, true);
        }

        public override unsafe void fillControlFloats(float* f)
        {
            f[0] = this.T1;
            f[1] = this.T2;
            f[2] = this.T3;
            f[3] = this.T4;
        }

        public override void MatlabUpdateSimInputs(ref float[] flows)
        {
            Bobcat.PumpModelFlow(this.T1, ref flows, 0);
            Bobcat.PumpModelFlow(this.T2, ref flows, 1);
            Bobcat.PumpModelFlow(this.T3, ref flows, 2);
            Bobcat.PumpModelFlow(this.T4, ref flows, 3);
        }

        public override void MatlabUpdateGui()
        {
            StaticMethods.setNudValue(this.nudCab, this.ActualAngles.cab);
            StaticMethods.setNudValue(this.nudBoom, this.ActualAngles.boo);
            StaticMethods.setNudValue(this.nudArm, this.ActualAngles.arm);
            StaticMethods.setNudValue(this.nudBucket, this.ActualAngles.buc);
        }
    }
}
