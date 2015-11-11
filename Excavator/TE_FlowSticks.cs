using System;
using System.Drawing;
using System.Windows.Forms;

namespace Excavator
{
    public class TE_FlowSticks :  Trial
    {
        private volatile float T1 = 0.0f;
        private volatile float T2 = 0.0f;
        private volatile float T3 = 0.0f;
        private volatile float T4 = 0.0f;

        private Panel panelSpacer1 = new Panel();

        public TE_FlowSticks()
            : base()
        {
            this.panelSpacer1.Size = new Size(100, 3);
            this.Controls.Add(this.panelSpacer1);
            this.panelSpacer1.Dock = DockStyle.Top;
            this.panelSpacer1.SendToBack();
            this.panelSpacer1.BackColor = FormBase.Instance.BackColor;

            this.Controls.Add(ControlStick._ControlStick);
            ControlStick._ControlStick.Dock = DockStyle.Top;
            ControlStick._ControlStick.SendToBack();
        }

        public override string getName()
        {
            return "Flow with Sticks";
        }




        public override void Gui_30MS_Tick(float accumulator)
        {
            base.Gui_30MS_Tick(accumulator);
            ControlStick._ControlStick.updateFPS(accumulator);
            ControlStick._ControlStick.updateControlGUI();
        }

        public override void Deconstruct()
        {
            base.Deconstruct();
        }









        public override void updateSim()
        {
            base.updateSim();

            float temp;
            
            temp = -ControlStick._ControlStick.getValForStick(ControlStick.l_LR, true);
            this.T1 = temp * Math.Abs(temp);
            temp = ControlStick._ControlStick.getValForStick(ControlStick.r_FB, true);
            this.T2 = temp * Math.Abs(temp);
            temp = ControlStick._ControlStick.getValForStick(ControlStick.l_FB, true);
            this.T3 = temp * Math.Abs(temp);
            temp = -ControlStick._ControlStick.getValForStick(ControlStick.r_LR, true);
            this.T4 = temp * Math.Abs(temp);
        }

        public override unsafe void fillControlFloats(float* f) //Creates an array 
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
        


    }
}
