using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Excavator
{
    public class TE_FlowKeyboard : TrialEmbedBase
    {
        private volatile float T1 = 0.0f;
        private volatile float T2 = 0.0f;
        private volatile float T3 = 0.0f;
        private volatile float T4 = 0.0f;


        private ControlKeyboard controlKeyboard1 = new ControlKeyboard();
        private Panel panelSpacer1 = new Panel();

        public TE_FlowKeyboard()
        {
        }

        public override void Deconstruct()
        {
            base.Deconstruct();
            this.controlKeyboard1.Deconstruct();
        }

        public TE_FlowKeyboard(FormBase fb, string saveFile, int minutes)
            : base(fb, false, saveFile, minutes)
        {
            if (this.DesignMode) return;

            this.panelSpacer1.Size = new Size(100, 3);
            this.Controls.Add(this.panelSpacer1);
            this.panelSpacer1.Dock = DockStyle.Top;
            this.panelSpacer1.SendToBack();
            this.panelSpacer1.BackColor = fb.BackColor;

            this.Controls.Add(this.controlKeyboard1);
            this.controlKeyboard1.Dock = DockStyle.Top;
            this.controlKeyboard1.SendToBack();

            this.controlKeyboard1.noSwing();
        }

        public override bool hasGhost()
        {
            return false;
        }

        public override string getName()
        {
            return "Flow with Keyboard";
        }

        public override void MatlabUpdateSim()
        {
            var fts = this.controlKeyboard1.getInts();

            this.T1 = fts[1];
            this.T2 = fts[2];
            this.T3 = -fts[3];
            this.T4 = -fts[4];
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

            this.controlKeyboard1.updateGui();
        }
    }
}
