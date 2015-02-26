using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Excavator
{
    public class T_VelocityCylinderKeyboard : Trial
    {
        private ControlKeyboardCylinder controlKeyboard1 = new ControlKeyboardCylinder();

        /// <summary>
        /// ONLY FOR INTERFACE BUILDER NEVER USE THIS SUCKER
        /// </summary>
        public T_VelocityCylinderKeyboard() : base()
        {
        }

        public override void Deconstruct()
        {
            base.Deconstruct();
            this.controlKeyboard1.Deconstruct();
        }

        public T_VelocityCylinderKeyboard(FormBase fb) : base(fb)
        {
            this.controlKeyboard1.Dock = DockStyle.Top;
            this.Controls.Add(this.controlKeyboard1);

            this.nudArm.Enabled = false;
            this.nudBoom.Enabled = false;
            this.nudBucket.Enabled = false;
            this.nudCab.Enabled = false;
            this.nudSwing.Enabled = false;

            foreach (Label l in new Label[] { l3, l2, l1, l0 })
            {
                this.Controls.Add(l);
                l.Dock = DockStyle.Top;
                l.SendToBack();
            }
        }







        private Label l1 = new Label();
        private Label l2 = new Label();
        private Label l3 = new Label();
        private Label l0 = new Label();
        private volatile float qd_d0;
        private volatile float qd_d1;
        private volatile float qd_d2;
        private volatile float qd_d3;


        public override bool hasGhost()
        {
            return false;
        }

        public override string getName()
        {
            return "Velocity Control Cylinder Keyboard";
        }





        private int _TrialUpdates = 0;
        private Vector2 _LastDelta = new Vector2();

        public override void updateSim()
        {
            var fts = this.controlKeyboard1.getInts();

            const float MAX_DEGS_PER_SECOND = 60;
            const float ABSMX = MAX_DEGS_PER_SECOND * (StaticMethods._PIF / 180000.0f);
            const float TRAVEL_SPEED = 0.3f;
            const float LIM_SPEED = 0.5f * ABSMX;

            Vector2 p, partialA = Vector2.Zero, partialB = Vector2.Zero;
            Vector2 travel = new Vector2(fts[3] * TRAVEL_SPEED, -fts[4] * TRAVEL_SPEED);

            int timeSpanner = (int)(1000 * this.ElapsedTime) - this._TrialUpdates;

            Single inBoom = this.GhostAngles.boo;
            Single inArm = this.GhostAngles.arm;
            Double A = StaticMethods.toRadiansD(this.GhostAngles.boo); // Boom
            Double B = StaticMethods.toRadiansD(this.GhostAngles.arm); // Arm

            // This Loop Iterates at 1 Mili Second Intervals
            for (int i = 0; i < timeSpanner; i++)
            {
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

                A = this.clampQ2_Radians(A + dA);
                B = this.clampQ3_Radians(B + dB);

                this.qd_d1 = (float)dA * 1000;
                this.qd_d2 = (float)dB * 1000;

            }

            this.GhostAngles.boo = StaticMethods.toDegreesF(A);
            this.GhostAngles.arm = StaticMethods.toDegreesF(B);

            this._TrialUpdates += timeSpanner;




            float inc = this._TimeSpanF * 50.0f;

            this.GhostAngles.cab = this.GhostAngles.cab + fts[1] * inc;
            if (this.GhostAngles.cab < -180) this.GhostAngles.cab += 360;
            if (this.GhostAngles.cab > 180) this.GhostAngles.cab -= 360;

            this.GhostAngles.buc = this.clampQ4(
                this.GhostAngles.buc + fts[2] * inc +
                (inArm - this.GhostAngles.arm) + 
                (inBoom - this.GhostAngles.boo));

            this.ActualAngles.copy(this.GhostAngles);
        }



        public override void  Gui_Draw_Tick()
        {
            this.controlKeyboard1.updateGui();

            l0.Text = qd_d0.ToString();
            l1.Text = qd_d1.ToString();
            l2.Text = qd_d2.ToString();
            l3.Text = qd_d3.ToString();

            StaticMethods.setNudValue(this.nudSwing, this.GhostAngles.swi);
            StaticMethods.setNudValue(this.nudCab, this.GhostAngles.cab);
            StaticMethods.setNudValue(this.nudBoom, this.GhostAngles.boo);
            StaticMethods.setNudValue(this.nudArm, this.GhostAngles.arm);
            StaticMethods.setNudValue(this.nudBucket, this.GhostAngles.buc);
        }
    }
}
