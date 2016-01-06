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
    public class TE_VelocityCylinderSticksBeau : Trial
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

        public TE_VelocityCylinderSticksBeau()
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
            //return "Cylinder Flow with Sticks - BEAU";
            return "BEAU's mapping";
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
            /*
            Sam's Mapping

            temp = -ControlStick._ControlStick.getValForStick(ControlStick.l_LR, true);
            this.T1 = temp * Math.Abs(temp);
            temp = -ControlStick._ControlStick.getValForStick(ControlStick.l_FB, true);
            this.T2 = temp * Math.Abs(temp);
            temp = ControlStick._ControlStick.getValForStick(ControlStick.r_LR, true);
            this.T3 = temp * Math.Abs(temp);
            temp = ControlStick._ControlStick.getValForStick(ControlStick.r_FB, true);
            this.T4 = temp * Math.Abs(temp);
            */

            temp = -ControlStick._ControlStick.getValForStick(ControlStick.r_LR, true);
            this.T1 = temp * Math.Abs(temp);
            temp = -ControlStick._ControlStick.getValForStick(ControlStick.l_LR, true);
            this.T2 = temp * Math.Abs(temp);
            temp = ControlStick._ControlStick.getValForStick(ControlStick.l_FB, true);
            this.T3 = temp * Math.Abs(temp);
            temp = ControlStick._ControlStick.getValForStick(ControlStick.r_FB, true);
            this.T4 = temp * Math.Abs(temp);

            /*
                   T3
                    |             |
             T2 --- o ---     --- o --- T1
                    |             |
                                  T4
            */

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
            const float ABSMX = MAX_DEGS_PER_SECOND * (StaticMethods._PIF / 180000.0f); //max rate (1.04 rad/sec) the joints can move
            const float TRAVEL_SPEED = 0.3f;
            const float LIM_SPEED = 0.5f * ABSMX;

            Vector2 p, partialA = Vector2.Zero, partialB = Vector2.Zero;
            Vector2 travel = new Vector2(-this.T3 * TRAVEL_SPEED, this.T4 * TRAVEL_SPEED); //B: travel is a 2D vector with T3(foreward/back) and T4(up/down) * speed (.3), which are the joystick values from above... 

            Single inBoom = Q[1];           //The first element : joint position of joint 1/Boom
            Single inArm = Q[2];            //The second element: joint position of joint 2/arm
            Double A = inBoom; // Boom        A=anglur pos joint 1
            Double B = inArm; // Arm          B=anglur pos joint 2

            p = Bobcat.CalcBucketPostition(A, B, ref partialA, ref partialB); //calculates bucket position from joint1 and joint2 angles, and partial derivitives (for x and y) for joints 1 and 2.. but we set those to 0?.
            //p is a 2D vector with x,y pos of bucket 
            //the partial derivitives are also calculated, and stored in the same name as given, partialA and partialB 

            Matrix2 m = Matrix2.fromCols(partialA, partialB); //Matrix2 is a custom class used create methods for matrix algebra/operations. From cols creates 2x2 matrix from the two 2d vectors
            Matrix2 mInv;

            if (m.invert(out mInv))  //If the matrix is invertible (nonsingular)
            {
                this._LastDelta = mInv.Transform(travel); //LastDelta is [dTheta1;dTheta2]
                this._LastDelta = Vector2.Multiply(this._LastDelta, 180 / StaticMethods._PIF); //changes into degrees?
            }

            var mx = Math.Max(Math.Abs(this._LastDelta.X), Math.Abs(this._LastDelta.Y)); //determines whether change of A or B was greater (the x and y just access the 1st and 2nd elements)

            this._LastDelta = Vector2.Multiply(this._LastDelta, mx > ABSMX ? ABSMX / mx : 1); //if mx>absmx then value = absmx/mx, else value = 1

            double dA = Trial.clampQ2_Radians(A + this._LastDelta.X) - A;   //clamp method determines whether angle is within the MIN and MAX angle for the joint. If it is, it uses the angle. Thus it ensures dA is the true change in angle by making sure the LastDelta is within range of motion.
            double dB = Trial.clampQ3_Radians(B + this._LastDelta.Y) - B;

            const float cutoff = 0.001f * ABSMX; //cutoff = (.001 sec) * (1.04 rad/sec) = .001 rad ....  cutoff = .06 degrees [[.001 s or 1 ms is the sampling time]]

            bool adjA = Math.Abs(dA) < cutoff; //true if dA is less than cutoff (.06 deegres)
            bool adjB = Math.Abs(dB) < cutoff; //true if dB is less than cutoff (.06 deegres)


            //THE FOLLOWING SERIES OF IF ELSE ARE JUST CHECKING FOR SMALL CHANGES IN A AND B
            if (Math.Min(Math.Abs(this._LastDelta.X), Math.Abs(this._LastDelta.Y)) < cutoff) // if Minimum of (dA,dB) < .06 deg, not moving much
            {
                // Not Moving That Much
            }
            if (adjA && adjB)
            {
                // Both Limits Switches Hit
            }
            else if (adjA) //if A is not changing by much, calculates the change in B. 
            //**It seems to me this is where wierd inverse kinematic issue comes from. If A is no longer changing, it just keeps calculating the change in the B angle.
            {
                float d = Vector2.Dot(Vector2.Normalize(travel), Vector2.Normalize(partialB));
                if (d > cutoff) dB = LIM_SPEED;
                else if (d < -cutoff) dB = -LIM_SPEED;
                else dB = 0;
            }
            else if (adjB) //if B is not changing by much, calculates the change in A
            //if commanding foreward, and arm_angle not changing, do not go down. stop moving. (do we limit range of motion?)
            {
                float d = Vector2.Dot(Vector2.Normalize(travel), Vector2.Normalize(partialA));
                if (d > cutoff) dA = LIM_SPEED;
                else if (d < -cutoff) dA = -LIM_SPEED;
                else dA = 0;
            }

            this._LastDelta.X = (float)dA;
            this._LastDelta.Y = (float)dB;

            dA *= 1000; //dA = dA * 1000
            dB *= 1000;

            this.Qd_Desired[1] = (Single)dA;
            this.Qd_Desired[2] = (Single)dB;
            this.Qd_Desired[3] = /* -(Single)(dA + dB)*/ +20.0f * this.T2;  //THIS IS THE BUCKET! All we do is scale by 20

            Bobcat.PumpModelFlow(this.T1, ref flows, 0); //output is a new array of flows. so input is flows, does manipulation, outputs new flows

            Bobcat.JointToCylinder(1, ref Q, ref Qd_Desired, ref CYL_POS_DESIRED, ref CYL_VEL_DESIRED); //returns boom cyl pos and cyl vel
            Bobcat.JointToCylinder(2, ref Q, ref Qd_Desired, ref CYL_POS_DESIRED, ref CYL_VEL_DESIRED); //returns arm cyl pos and cyl vel
            Bobcat.PumpModelVelocity(ref CYL_VEL_DESIRED, ref CYL_VEL, ref flows);  //returns flows

            Bobcat.PumpModelFlow(-this.T2, ref flows, 3);
        }




    }
}
