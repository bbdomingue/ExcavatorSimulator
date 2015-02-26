﻿using System;
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
    public class TE_VelocityCylinderKeyboard : TrialEmbedBase
    {
        private volatile float T1 = 0.0f;
        private volatile float T2 = 0.0f;
        private volatile float T3 = 0.0f;
        private volatile float T4 = 0.0f;


        private ControlKeyboardCylinder controlKeyboard1 = new ControlKeyboardCylinder();
        private Panel panelSpacer1 = new Panel();      

        public TE_VelocityCylinderKeyboard()
        {
        }

        public override void Deconstruct()
        {
            base.Deconstruct();
            this.controlKeyboard1.Deconstruct();
        }

        public TE_VelocityCylinderKeyboard(FormBase fb, string saveFile, int minutes)
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

            double dA = this.clampQ2_Radians(A + this._LastDelta.X) - A;
            double dB = this.clampQ3_Radians(B + this._LastDelta.Y) - B;

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
            this.Qd_Desired[3] = /* -(Single)(dA + dB)*/ + 20.0f * this.T2;

            Bobcat.PumpModelFlow(this.T1, ref flows, 0);

            Bobcat.JointToCylinder(1, ref Q, ref Qd_Desired, ref CYL_POS_DESIRED, ref CYL_VEL_DESIRED);
            Bobcat.PumpModelVelocity(1, ref CYL_VEL_DESIRED, ref CYL_VEL, ref flows);

            Bobcat.JointToCylinder(2, ref Q, ref Qd_Desired, ref CYL_POS_DESIRED, ref CYL_VEL_DESIRED);
            Bobcat.PumpModelVelocity(2, ref CYL_VEL_DESIRED, ref CYL_VEL, ref flows);

            Bobcat.PumpModelFlow(-this.T2, ref flows, 3);
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
