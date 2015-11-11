using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;

namespace Excavator
{
    public class TE_VelocityCylinderSticks : Trial
    {
        private volatile float T1 = 0.0f;
        private volatile float T2 = 0.0f;
        private volatile float T3 = 0.0f;
        private volatile float T4 = 0.0f;

        private Panel panelSpacer1 = new Panel();

        public override void Deconstruct()
        {
            base.Deconstruct();
        }

        public TE_VelocityCylinderSticks() //A method with the same name as the class..
            : base()
        {
            if (this.DesignMode) return;

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
            return "Cylinder Flow with Sticks";
        }

        public override void Gui_30MS_Tick(float accumulator)
        {
            base.Gui_30MS_Tick(accumulator);
            ControlStick._ControlStick.updateFPS(accumulator);
            ControlStick._ControlStick.updateControlGUI();
        }







        public override void updateSim()
        {
            base.updateSim();

            float temp;
            
            //Note that in TE_FlowStick the negative are placed in front of temp1 and temp4, here it is temp1 and temp2
            temp = -ControlStick._ControlStick.getValForStick(ControlStick.l_LR, true);   //I believe ControlStick.l_LR = 1
            this.T1 = temp * Math.Abs(temp);
            temp = -ControlStick._ControlStick.getValForStick(ControlStick.l_FB, true);
            this.T2 = temp * Math.Abs(temp);
            temp = ControlStick._ControlStick.getValForStick(ControlStick.r_FB, true);
            this.T3 = temp * Math.Abs(temp);
            temp = ControlStick._ControlStick.getValForStick(ControlStick.r_LR, true);
            this.T4 = temp * Math.Abs(temp);
        }

        //I believe everything that follow this point is the inverse kinematics calc. Thus, it is absent form TE_FlowSticks
        private Vector2 _LastDelta = new Vector2();
        private float[] CYL_POS_DESIRED = new float[4];
        private float[] CYL_VEL_DESIRED = new float[4];
        private float[] Qd_Desired = new float[4];

        public override void MatlabUpdateSimInputs(
            ref float[] Q,
            ref float[] Qd,
            ref float[] CYL_POS,
            ref float[] CYL_VEL,
            ref float[] flows)
        {
            const float MAX_DEGS_PER_SECOND = 60;
            const float ABSMX = MAX_DEGS_PER_SECOND * (StaticMethods._PIF / 180000.0f);
            const float TRAVEL_SPEED = 0.3f;
            const float LIM_SPEED = 0.5f * ABSMX;

            Vector2 p, partialA = Vector2.Zero, partialB = Vector2.Zero;
            Vector2 travel = new Vector2(-this.T3 * TRAVEL_SPEED, this.T4 * TRAVEL_SPEED);

            Single inBoom = Q[1];
            Single inArm = Q[2];
            Double A = inBoom; // Boom
            Double B = inArm; // Arm

            p = Bobcat.CalcBucketPostition(A, B, ref partialA, ref partialB);

            Matrix2 m = Matrix2.fromCols(partialA, partialB);
            Matrix2 mInv;

            if (m.invert(out mInv))
            {
                this._LastDelta = mInv.Transform(travel);
                this._LastDelta = Vector2.Multiply(this._LastDelta, 180 / StaticMethods._PIF);
            }

            var mx = Math.Max(Math.Abs(this._LastDelta.X), Math.Abs(this._LastDelta.Y));

            this._LastDelta = Vector2.Multiply(this._LastDelta, mx > ABSMX ? ABSMX / mx : 1);

            double dA = Trial.clampQ2_Radians(A + this._LastDelta.X) - A;
            double dB = Trial.clampQ3_Radians(B + this._LastDelta.Y) - B;

            const float cutoff = 0.001f * ABSMX;

            bool adjA = Math.Abs(dA) < cutoff;
            bool adjB = Math.Abs(dB) < cutoff;

            if (Math.Min(Math.Abs(this._LastDelta.X), Math.Abs(this._LastDelta.Y)) < cutoff)
            {
                // Not Moving That Much
            }
            if (adjA && adjB)
            {
                // Both Limits Switches Hit
            }
            else if (adjA)
            {
                float d = Vector2.Dot(Vector2.Normalize(travel), Vector2.Normalize(partialB));
                if (d > cutoff) dB = LIM_SPEED;
                else if (d < -cutoff) dB = -LIM_SPEED;
                else dB = 0;
            }
            else if (adjB)
            {
                float d = Vector2.Dot(Vector2.Normalize(travel), Vector2.Normalize(partialA));
                if (d > cutoff) dA = LIM_SPEED;
                else if (d < -cutoff) dA = -LIM_SPEED;
                else dA = 0;
            }

            this._LastDelta.X = (float)dA;
            this._LastDelta.Y = (float)dB;

            dA *= 1000;
            dB *= 1000;

            this.Qd_Desired[1] = (Single)dA;
            this.Qd_Desired[2] = (Single)dB;
            this.Qd_Desired[3] = /* -(Single)(dA + dB)*/ +20.0f * this.T2;

            Bobcat.PumpModelFlow(this.T1, ref flows, 0);

            Bobcat.JointToCylinder(1, ref Q, ref Qd_Desired, ref CYL_POS_DESIRED, ref CYL_VEL_DESIRED);
            Bobcat.JointToCylinder(2, ref Q, ref Qd_Desired, ref CYL_POS_DESIRED, ref CYL_VEL_DESIRED);
            Bobcat.PumpModelVelocity(ref CYL_VEL_DESIRED, ref CYL_VEL, ref flows);

            Bobcat.PumpModelFlow(-this.T2, ref flows, 3);
        }
    }
}
