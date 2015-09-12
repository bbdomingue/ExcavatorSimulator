using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.GlobalEvents;
using SamSeifert.GLE.CadViewer;
using SamSeifert.GLE;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using MatlabExcavatorWrapper;

using System.Threading;

using PhysX;

using Matrix_PhysX = PhysX.Math.Matrix;
using Vector3_PhysX = PhysX.Math.Vector3;
using Vector4_PhysX = PhysX.Math.Vector4;

namespace Excavator
{
    public partial class Trial : UserControl
    {
        private static Trial _Trial = null;
        private static object _TrialLock = new object();

        protected Bobcat.BobcatAngles ActualAngles = new Bobcat.BobcatAngles();
        protected Bobcat.BobcatAngles GhostAngles = new Bobcat.BobcatAngles();

        public static float FPS = 0;
        private float fpsGL = 0;
        private float fpsGLF = 0;
        public float _TimeSpanF = 0;
        private int accumulatorCounter = 0;
        public Stopwatch _StopWatchSimulateTotal = new Stopwatch();
        private Stopwatch _StopWatchSimulateDelta = new Stopwatch();
        private Stopwatch _StopWatch30 = new Stopwatch();
        private Label labelFuelEfficiency;
        private Label labelSimUpdate;
        private Stopwatch _StopWatch300 = new Stopwatch();

        const float FuelMin = 0.864f;
        const float FuelMax = 2.182f;
        const float FuelRange = Trial.FuelMax - Trial.FuelMin;

        private object FuelLock = new object();
        private float _FuelInstant = Trial.FuelMin;
        private float _FuelFiltered = Trial.FuelMin;
        private bool _FuelValid = true;

        private float delayF = 0;

        // Link Masses (Kg)
        const float m1 = 1000 * 0.9554f;
        const float m2 = 1000 * 0.4211f;
        const float m3 = 1000 * 0.1906f;

        // Link Lengths (M)
        const float a1 = StaticMethods.Conversion_Inches_To_Meters * 31.5423f;
        const float a2 = StaticMethods.Conversion_Inches_To_Meters * 5.6022f;
        const float dim_boom_length_meters = StaticMethods.Conversion_Inches_To_Meters * 104;
        const float dim_arm_length_meters = StaticMethods.Conversion_Inches_To_Meters * 54;
        const float a5 = StaticMethods.Conversion_Inches_To_Meters * 34.6133f;

        /// <summary>
        /// Gets total time from start of trial
        /// </summary>
        public float ElapsedTime { get { return (float) this._StopWatchSimulateTotal.Elapsed.TotalSeconds; } }

        ~Trial()
        {
            Console.WriteLine("Garbage Collecting Trial");
        }

       
        public Trial()
        {
            this.InitializeComponent();

            if (this.DesignMode) return;

            Trial.staticInit();

            this.GhostAngles.swi = 0;
            this.GhostAngles.cab = 0;
            this.GhostAngles.boo = 40;
            this.GhostAngles.arm = -90;
            this.GhostAngles.buc = 0;
            this.ActualAngles = this.GhostAngles;

            this._StopWatchSimulateTotal.Start();
            this._StopWatchSimulateDelta.Start();
            this._StopWatch30.Start();
            this._StopWatch300.Start();

            Trial.ResetSoilModelS();

            GL_Handler.DrawGhost = false;

            FormBase.Instance.RefreshTrial(this);


            lock (Trial._TrialLock)
                Trial._Trial = this;

            new Thread(new ThreadStart(this.MultiThread)).Start();
        }




















        /// <summary>
        /// When overrode, do not call base
        /// </summary>
        public virtual string getName()
        {
            return "Unnamed";
        }

        public static bool UpdateMainThreadS()
        {
            var t = Trial._Trial;
            if (t != null) return t.UpdateMainThread();
            else return false;
        }

        public bool UpdateMainThread()
        {
            bool ret = false;

            this._TimeSpanF = (float)this._StopWatchSimulateDelta.Elapsed.TotalSeconds;
            this._StopWatchSimulateDelta.Restart();

            this.accumulatorCounter++;

            if (this._StopWatch30.ElapsedMilliseconds > 30)
            {
                ret = true;
                this.Gui_30MS_Tick((float)this._StopWatch30.Elapsed.TotalSeconds);
                this._StopWatch30.Restart();
            }

            if (this._StopWatch300.ElapsedMilliseconds > 300)
            {
                this.Gui_300MS_Tick((float)this._StopWatch300.Elapsed.TotalSeconds);
                this._StopWatch300.Restart();
            }

            this.updateSim();

            return ret;
        }

        /// <summary>
        /// Called On Main Thread
        /// </summary>
        public virtual void updateSim()
        {
        }



        /// <summary>
        /// Called every 300 ms
        /// </summary>
        public virtual void Gui_300MS_Tick(float accumulator)
        {
            this.fpsGL = this.accumulatorCounter / accumulator;
            const float alpha = 0.05f;
            this.fpsGL = this.accumulatorCounter / accumulator;
            if (this.fpsGL > this.fpsGLF * 1.1 || this.fpsGL < this.fpsGLF * 0.9) this.fpsGLF = this.fpsGL;
            else this.fpsGLF = this.fpsGLF * (1 - alpha) + this.fpsGL * alpha;
            Trial.FPS = this.fpsGLF;
            this.accumulatorCounter = 0;
        }

        /// <summary>
        /// Called every 30 ms.  When overrode, call base
        /// </summary>
        public virtual void Gui_30MS_Tick(float accumulator)
        {
            float fuel, filt;
            bool valid;

            lock (FuelLock)
            {
                fuel = this._FuelInstant;
                filt = this._FuelFiltered;
                valid = this._FuelValid;
            }

            if (FormBase._BoolThreadAlive)
            {
                float adj1 = (fuel - Trial.FuelMin) / Trial.FuelRange;
                ExcavatorSound.Instance.setVolume(valid ? 0.6f + 0.4f * adj1 : 0);
                float adj2 = (filt - Trial.FuelMin) / Trial.FuelRange;
                ExcavatorSound.Instance.setSpeed(1.0f + 0.5f * adj2);
            }
            else
            {
                ExcavatorSound.Stop();
            }

            float act = (float)(this._StopWatchSimulateTotal.Elapsed.TotalSeconds);
            this.delayF = (act - this._FloatSimTime);
            this.labelSimUpdate.Text =
                "Delay: " + Math.Max(0, this.delayF).ToString("0.000  ") +
                "Time: " + act.ToString("0.00");
            this.labelFuelEfficiency.Text = "Fuel Consumption: " + (fuel * 1000).ToString("0.00");
        }






        public const float _FloatGroundPlaneDim = 600;
        public const float _FloatGroundPlaneDim2 = _FloatGroundPlaneDim * 2;
        public const int _IntTextureDensity = 32;

        public volatile bool _BoolAlive = true;

        internal static float getTime()
        {
            Trial t = Trial._Trial;
            if (t != null) return t.ElapsedTime;
            else return 0;            
        }

        internal static void DeconstructS()
        {
            lock (Trial._TrialLock)
            {
                if (Trial._Trial != null)
                {
                    Trial._Trial.Deconstruct();
                    Trial._Trial = null;
                }
            }
        }

        public virtual void Deconstruct()
        {
            if (this._BoolAlive)
            {
                ExcavatorSound.Stop();
                this._BoolAlive = false;
            }
        }



























































        const float TimeStepPhysX = 0.001f;
        private volatile float _FloatSimTime = 0.0f;

        public const float QSCALE = 57.2957795f;
        public const float QSCALEI = 0.0174532925f;


        /// <summary>
        /// When overrode, do not call base.  Called on Matlab Thread
        /// </summary>
        public virtual void MatlabUpdateSimInputs(ref float[] flow) { }

        /// <summary>
        /// When overrode, do not call base.  Called on Matlab Thread
        /// </summary>
        public virtual void MatlabUpdateSimInputs(
            ref float[] Q,
            ref float[] Qd,
            ref float[] CYL_POS,
            ref float[] CYL_VEL,
            ref float[] flow)
        {
            this.MatlabUpdateSimInputs(ref flow);
        }

        unsafe private void MultiThread()
        {
            Thread.CurrentThread.Name = "Matlab Simulation Thread";

            FormBase.addThread();

            Console.WriteLine("Start Matlab Simulation");

            var sim = new MatlabSimulation();
            sim.SamInitClass();

            var wtch = new Stopwatch();

            this.Invoke((MethodInvoker)delegate // Important espcially on startup to sync clocks.
            {
                this._StopWatchSimulateTotal.Restart();
            });

            wtch.Restart();

            float[] Q   = new float[] { 0, 0, 0, 0 };
            float[] Qd  = new float[] { 0, 0, 0, 0 };
            float[] FLOW = new float[] { 0, 0, 0, 0 };

            float[] SD_FORCE = new float[] { 0, 0, 0 };
            float[] SD_MOMENT = new float[] { 0, 0, 0 };
            float[] SW_FORCE = new float[] { 0, 0, 0 };

            float[] Ybh = new float[] { 0, 0, 0, 0 };
            float[] Vbh = new float[] { 0, 0, 0, 0 };

            float[] CYL_POS = new float[] { 0, 0, 0, 0 };
            float[] CYL_VEL = new float[] { 0, 0, 0, 0 };

            float[] FUEL = new float[] { 0, 0 };

            float[] CAB_TORQUE = new float[] { 0 };
            float[] JOINT_FORCES = new float[] { 0, 0, 0, 0 };

            int WaitJustOneMinute = -1;

            fixed (float *
                Q_P = Q,
                QD_P = Qd,
                SD_FORCE_P = SD_FORCE,
                SD_MOMENT_P = SD_MOMENT,
                SW_FORCE_P = SW_FORCE,
                FUEL_P = FUEL,
                FLOW_P = FLOW,
                CAB_TORQUE_P = CAB_TORQUE,
                JOINT_FORCES_P = JOINT_FORCES,
                CYL_POS_P = CYL_POS,
                CYL_VEL_P = CYL_VEL
                )
            {
                float fuelInstant;
                float fuelFiltered = Trial.FuelMin;

                int centiSeconds = -1;
                int centiSecondsNew = 0;
                float simTime = 0;

                lock (Trial._Physics) Trial.UpdateMatlabInputs(ref Q, ref Qd, ref CYL_POS, ref CYL_VEL);

                TrialSaver.File2_DataType f2dt = new TrialSaver.File2_DataType()  { Size = 0 };

                while (FormBase._BoolThreadAlive && this._BoolAlive)
                {
                    if (this._FloatSimTime < wtch.Elapsed.TotalSeconds)
                    {
                        bool skip = WaitJustOneMinute < 0 ? false : (Environment.TickCount - WaitJustOneMinute < 60000);                       

                        if (skip)
                        {
                            FLOW[0] = 0;
                            FLOW[1] = 0;
                            FLOW[2] = 0;
                            FLOW[3] = 0;
                        }
                        else this.MatlabUpdateSimInputs(
                            ref Q,
                            ref Qd,
                            ref CYL_POS,
                            ref CYL_VEL,
                            ref FLOW);

                        simTime = sim.SamUpdateClass(
                            CYL_POS_P,
                            CYL_VEL_P,
                            FLOW_P,
                            // Back
                            FUEL_P,
                            CAB_TORQUE_P,
                            JOINT_FORCES_P);

                        lock (Trial._Physics)
                        {
                            Trial.updatePhysXJoints(JOINT_FORCES, CAB_TORQUE);
                            Trial.updatePhysXCenterCab(Q[0]);

                            f2dt = EmbeddedSoilModel.Instance.ForceModel_ReturnDump(
                                Trial.xBucket,
                                Trial.xBucketTip,
                                Trial.xCab,
                                Trial._Scene,
                                this._FloatSimTime);                                                    

                            Trial._Scene.Simulate(TimeStepPhysX);
                            Trial._Scene.FetchResults(true);

                            Trial.UpdateMatlabInputs(ref Q, ref Qd, ref CYL_POS, ref CYL_VEL);

                            // Prevents Bucket Link and Bucket From Switching Sides
                            Trial._RevoluteJoint_Arm_BucketLink.Limit = new JointAngularLimitPair(-StaticMethods._PIF / 3,  Math.Max(0,
                                (StaticMethods.toRadiansF(-65) - Q[3]) * 0.7f + StaticMethods.toRadiansF(5)));
                        }


                        Bobcat.ActualAngles = new Bobcat.BobcatAngles()
                        {
                            swi = 0,
                            cab = Q[0] * Trial.QSCALE,
                            boo = Q[1] * Trial.QSCALE,
                            arm = Q[2] * Trial.QSCALE,
                            buc = Q[3] * Trial.QSCALE
                        };

                        CabRotater.setAngleValues(Q[0], Qd[0]);

                        fuelInstant = FUEL[0];
                        const float alpha = 0.007f;
                        fuelFiltered = fuelFiltered * (1 - alpha) + alpha * fuelInstant;

                        lock (FuelLock)
                        {
                            this._FuelInstant = fuelInstant;
                            this._FuelFiltered = fuelFiltered;
                            this._FuelValid = !skip;
                        }

                        this._FloatSimTime = simTime;

                        if (!skip)
                        {
                            if (f2dt.Size > 0) TrialSaver.update2(f2dt);
                            centiSecondsNew = (int)(simTime * 100);
                            if (centiSecondsNew != centiSeconds)
                            {
                                centiSeconds = centiSecondsNew;
                                var dat = new TrialSaver.File1_DataType();
                                dat.TimeMilis = (int)(simTime * 1000);
                                dat.Q[0] = Q[0];
                                dat.Q[1] = Q[1];
                                dat.Q[2] = Q[2];
                                dat.Q[3] = Q[3];
                                dat.QD[0] = Qd[0];
                                dat.QD[1] = Qd[1];
                                dat.QD[2] = Qd[2];
                                dat.QD[3] = Qd[3];
                                dat.BucketSoil = Bobcat._FloatBucketSoilVolume;
                                dat.BinSoilRight = EmbeddedSoilModel._VolumeRightBin;
                                dat.BinSoilLeft = EmbeddedSoilModel._VolumeLeftBin;
                                dat.BinSoilNone = EmbeddedSoilModel._VolumeNoBin;
                                dat.Fuel[0] = FUEL[0];
                                dat.Fuel[1] = FUEL[1];
                                this.fillControlFloats(dat.JoyStick);

                                if (TrialSaver.update1(dat))
                                {
                                    WaitJustOneMinute = Environment.TickCount;
                                }
                            }
                        }
                    }
                    else Thread.Sleep(TimeSpan.Zero);
                }
            }

            sim.SamTerminateClass();

            Console.WriteLine("Stop Matlab Simulation");

            FormBase.subThread();
        }

        private static void UpdateMatlabInputs(ref float[] Q, ref float[] Qd, ref float[] CYL_POS, ref float[] CYL_VEL)
        {
            // UPDATE Q

            Vector4_PhysX targ1, targ2, cabPerp = Vector4_PhysX.Transform(Vector4_PhysX.UnitX, Trial.xCab.GlobalPose);
            Vector3_PhysX cross;
            float newQ;

            targ1 = Vector4_PhysX.Transform(Vector4_PhysX.UnitX, Trial.xCab.GlobalPose);
            Q[0] = -(float)Math.Atan2(targ1.Z, targ1.X); ;

            var cabX = (targ1.xyz());

            targ1 = Vector4_PhysX.Transform(Vector4_PhysX.UnitZ, Trial.xCab.GlobalPose);
            targ2 = Vector4_PhysX.Transform(Vector4_PhysX.UnitZ, Trial.xBoom.GlobalPose);
            cross = PhysX.Math.Vector3.Cross(targ1.xyz(), targ2.xyz());
            if (Single.IsNaN(cross.LengthSquared())) newQ = 0;
            else newQ = (float)Math.Acos(Vector4_PhysX.Dot(targ1, targ2)) *
                Math.Sign(Vector4_PhysX.Dot(new Vector4_PhysX(cross, 0), cabPerp));
            Q[1] = newQ;

            targ1 = targ2;
            targ2 = Vector4_PhysX.Transform(Vector4_PhysX.UnitZ, Trial.xArm.GlobalPose);
            cross = PhysX.Math.Vector3.Cross(targ1.xyz(), targ2.xyz());
            if (Single.IsNaN(cross.LengthSquared())) newQ = 0;
            else newQ = (float)Math.Acos(Vector4_PhysX.Dot(targ1, targ2)) *
                Math.Sign(Vector4_PhysX.Dot(new Vector4_PhysX(cross, 0), cabPerp));
            Q[2] = newQ;

            targ1 = targ2;
            targ2 = Vector4_PhysX.Transform(Vector4_PhysX.UnitZ, Trial.xBucket.GlobalPose);
            cross = PhysX.Math.Vector3.Cross(targ1.xyz(), targ2.xyz());
            if (Single.IsNaN(cross.LengthSquared())) newQ = 0;
            else newQ = (float)
                Math.Acos(Math.Max(-1, Math.Min(1, Vector4_PhysX.Dot(targ1, targ2)))) *
                Math.Sign(Vector4_PhysX.Dot(new Vector4_PhysX(cross, 0), cabPerp));
            Q[3] = newQ;

            // Update Qd

            Qd[0] = Vector3_PhysX.Dot(Trial.xCab.AngularVelocity, Vector3_PhysX.UnitY);
            Qd[1] = Vector3_PhysX.Dot(Trial.xBoom.AngularVelocity, cabX);
            Qd[2] = Vector3_PhysX.Dot(Trial.xArm.AngularVelocity, cabX);
            Qd[3] = Vector3_PhysX.Dot(Trial.xBucket.AngularVelocity, cabX);

            // Udate Cyl_Pos

            Vector3_PhysX v3pos1 = Trial.actorCabBoom1.GlobalPose.xyz() - Trial.actorCabBoom2.GlobalPose.xyz();
            Vector3_PhysX v3pos2 = Trial.actorBoomArm1.GlobalPose.xyz() - Trial.actorBoomArm2.GlobalPose.xyz();
            Vector3_PhysX v3pos3 = Trial.actorArmBucketLink1.GlobalPose.xyz() - Trial.actorArmBucketLink2.GlobalPose.xyz();

            CYL_POS[0] = Q[0];
            CYL_POS[1] = v3pos1.Normalize() * StaticMethods.Conversion_Meters_To_Inches;
            CYL_POS[2] = v3pos2.Normalize() * StaticMethods.Conversion_Meters_To_Inches;
            CYL_POS[3] = v3pos3.Normalize() * StaticMethods.Conversion_Meters_To_Inches;

            // Update Cyl_Vel

            CYL_VEL[0] = Qd[0];
            CYL_VEL[1] = StaticMethods.Conversion_Meters_To_Inches *
                Vector3_PhysX.Dot(Trial.actorCabBoom1.LinearVelocity - Trial.actorCabBoom2.LinearVelocity, v3pos1);
            CYL_VEL[2] = StaticMethods.Conversion_Meters_To_Inches *
                Vector3_PhysX.Dot(Trial.actorBoomArm1.LinearVelocity - Trial.actorBoomArm2.LinearVelocity, v3pos2);
            CYL_VEL[3] = StaticMethods.Conversion_Meters_To_Inches *
                Vector3_PhysX.Dot(Trial.actorArmBucketLink1.LinearVelocity - Trial.actorArmBucketLink2.LinearVelocity, v3pos3);

        }


        /// <summary>
        /// When Overrode, do not call base
        /// </summary>
        /// <param name="f"></param>
        public unsafe virtual void fillControlFloats(float * f)
        {
            f[0] = 0;
            f[1] = 0;
            f[2] = 0;
            f[3] = 0;
        }









        private void InitializeComponent()
        {
            this.labelFuelEfficiency = new System.Windows.Forms.Label();
            this.labelSimUpdate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelFuelEfficiency
            // 
            this.labelFuelEfficiency.AutoSize = true;
            this.labelFuelEfficiency.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelFuelEfficiency.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFuelEfficiency.Location = new System.Drawing.Point(0, 0);
            this.labelFuelEfficiency.Margin = new System.Windows.Forms.Padding(0);
            this.labelFuelEfficiency.Name = "labelFuelEfficiency";
            this.labelFuelEfficiency.Padding = new System.Windows.Forms.Padding(4);
            this.labelFuelEfficiency.Size = new System.Drawing.Size(54, 26);
            this.labelFuelEfficiency.TabIndex = 0;
            this.labelFuelEfficiency.Text = "label1";
            // 
            // labelSimUpdate
            // 
            this.labelSimUpdate.AutoSize = true;
            this.labelSimUpdate.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSimUpdate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSimUpdate.Location = new System.Drawing.Point(0, 26);
            this.labelSimUpdate.Margin = new System.Windows.Forms.Padding(0);
            this.labelSimUpdate.Name = "labelSimUpdate";
            this.labelSimUpdate.Padding = new System.Windows.Forms.Padding(4);
            this.labelSimUpdate.Size = new System.Drawing.Size(54, 26);
            this.labelSimUpdate.TabIndex = 1;
            this.labelSimUpdate.Text = "label2";
            // 
            // Trial
            // 
            this.Controls.Add(this.labelSimUpdate);
            this.Controls.Add(this.labelFuelEfficiency);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(210, 0);
            this.MinimumSize = new System.Drawing.Size(210, 0);
            this.Name = "Trial";
            this.Size = new System.Drawing.Size(210, 332);
            this.ResumeLayout(false);
            this.PerformLayout();

        }







































































































































        public const float QS_Max = 90, QS_Min = -90;
        public const float Q2_Max = 66, Q2_Min = -63;
        public const float Q3_Max = -35, Q3_Min = -145;
//        public const float Q4_Max = 33, Q4_Min = -154;
        public const float Q4_Max = 23, Q4_Min = -144;

        public static float clampQS(float val) { return Math.Max(Trial.QS_Min, Math.Min(Trial.QS_Max, val)); }
        public static float clampQ2(float val) { return Math.Max(Trial.Q2_Min, Math.Min(Trial.Q2_Max, val)); }
        public static float clampQ3(float val) { return Math.Max(Trial.Q3_Min, Math.Min(Trial.Q3_Max, val)); }
        public static float clampQ4(float val) { return Math.Max(Trial.Q4_Min, Math.Min(Trial.Q4_Max, val)); }

        public static double clampQ2_Radians(Double val)
        {
            return Math.Max(Trial.Q2_Min_Radians, Math.Min(Trial.Q2_Max_Radians, val));
        }

        public static double clampQ3_Radians(Double val)
        {
            return Math.Max(Trial.Q3_Min_Radians, Math.Min(Trial.Q3_Max_Radians, val));
        }

        private static readonly double Q2_Max_Radians = StaticMethods.toRadiansD(Q2_Max), Q2_Min_Radians = StaticMethods.toRadiansD(Q2_Min);
        private static readonly double Q3_Max_Radians = StaticMethods.toRadiansD(Q3_Max), Q3_Min_Radians = StaticMethods.toRadiansD(Q3_Min);
        private static readonly double Q4_Max_Radians = StaticMethods.toRadiansD(Q4_Max), Q4_Min_Radians = StaticMethods.toRadiansD(Q4_Min);

        public static HeightMap _Heightmap = null;

//        private static readonly Color colorC = Color.FromArgb(89, 113, 38);
        private static readonly Color colorC = Color.FromArgb(76, 96, 35);
        private static float[] colorA = new float[] { 0.25f, 0.25f, 0.25f, 1.0f };
        private static float[] colorD = new float[] { 0.50f, 0.50f, 0.50f, 1.0f };
        private static float[] color0 = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };
        private static readonly float[] colorW = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };

        private static CadObject _CadObjectTree1 = null;
        private static CadObject _CadObjectTree2 = null;
        private static CadObject _CadObjectTree3 = null;
        private static CadObject _CadObjectTree4 = null;
        private static CadObject _CadObjectTree5 = null;
        private static CadObject _CadObjectTree6 = null;

        private static TreeObject[] _Trees;
        private const int TreeAnglecutOff = 80;

        private static SamSeifert.GLE.Color_GL _ColorGL = new Color_GL(Color.White);
        
        private static Scene _Scene;
        protected static Foundation _Foundation = new Foundation(new UserOutput());
        private static Physics _Physics = new Physics(Trial._Foundation, false);
        private static RigidDynamic xCab, xBoom, xArm, xBucketLink, xBucket;
        private static FixedJoint jntCabBoom1, jntCabBoom2;
        private static RigidDynamic actorCabBoom1, actorCabBoom2;
        private static FixedJoint jntBoomArm1, jntBoomArm2;
        private static RigidDynamic actorBoomArm1, actorBoomArm2;
        private static FixedJoint jntArmBucketLink1, jntArmBucketLink2;
        private static RigidDynamic actorArmBucketLink1, actorArmBucketLink2;
        private static DistanceJoint jntBucketLinkBucket;
        private static Shape xBucketTip;
        private static RevoluteJoint _RevoluteJoint_Arm_BucketLink;

        private static Dictionary<RigidDynamic, Matrix_PhysX> StartingPoses = new Dictionary<RigidDynamic, Matrix_PhysX>();


//        private static FixedJoint TEST_FIX_JOINT;

        public static volatile float f1 = 1, f2 = -1, f3, f4;

        public class TreeObject
        {
            public CadObject co;
            public float angle;
            public float distance;

            public TreeObject(float a, float d, CadObject o) { this.angle = a; this.distance = d; this.co = o; }

            public void GLDraw(float cab)
            {
                float dif = Math.Abs(this.angle - cab);
                if (dif < Trial.TreeAnglecutOff || dif > (360 - Trial.TreeAnglecutOff))
                {
                    GL_Handler.PushMatrix();
                    {
                        GL_Handler.Rotate(angle, OpenTK.Vector3.UnitY);
                        GL_Handler.Translate(0, 0, -distance);
                        co.draw(true);
                    }
                    GL_Handler.PopMatrix();
                }
            }
        }

        private static bool _BoolNeedsStaticInit = true;
        private static bool _BoolNeedsStaticInitTrees = true;

        private static void staticInit()
        {
            if (Trial._BoolNeedsStaticInit)
            {
                Trial._BoolNeedsStaticInit = false;

                Trial._Heightmap = new HeightMapCircle(Properties.Resources.HeightMap, 500, 2300, 4, 200, 0.25f);

                Trial.colorA[0] = Trial.colorC.R / 255.0f;
                Trial.colorA[1] = Trial.colorC.G / 255.0f;
                Trial.colorA[2] = Trial.colorC.B / 255.0f;
                Trial.colorD[0] = Trial.colorC.R / 255.0f;
                Trial.colorD[1] = Trial.colorC.G / 255.0f;
                Trial.colorD[2] = Trial.colorC.B / 255.0f;

                for (int i = 0; i < 3; i++)
                {
                    Trial.colorA[i] *= 0.2f;
                    Trial.colorD[i] *= 1.0f;
                }

                Trial._Scene = Trial._Physics.CreateScene(new SceneDesc()
                {
                    Gravity = new Vector3_PhysX(0, -9.8f, 0),
                });

                Vector3[] vss;
                int[] iss;
                var cat_material = Trial._Physics.CreateMaterial(0.0f, 0.0f, 0.0f);
                var bin_material = Trial._Physics.CreateMaterial(1.0f, 1.0f, 0);
                var grnd_material = Trial._Physics.CreateMaterial(1.0f, 1.0f, 0);

                cat_material.FrictionCombineMode = CombineMode.Max;
                bin_material.FrictionCombineMode = CombineMode.Max;
                grnd_material.FrictionCombineMode = CombineMode.Max;


                var cooking = Trial._Physics.CreateCooking();

                var tw = EmbeddedSoilModel.TrenchWidth * StaticMethods.Conversion_Inches_To_Meters * 1.2f; // 1.2 IS SAFETY BUFFER
                const int depth = 3;
                const int hw = 10; // Half Width of The Covered Area

                var ground = Trial._Physics.CreateRigidStatic();
                ground.CreateShape(new BoxGeometry((hw - tw) / 2, depth, hw), grnd_material, Matrix_PhysX.Translation(hw / 2, -depth, 0));
                ground.CreateShape(new BoxGeometry((hw - tw) / 2, depth, hw), grnd_material, Matrix_PhysX.Translation(-hw / 2, -depth, 0));
                ground.CreateShape(new BoxGeometry(tw, depth, hw / 2), grnd_material, Matrix_PhysX.Translation(0, -depth, hw / 2));
                Trial._Scene.AddActor(ground);





                var vec1 = Bobcat.vecOffsetCab_Inches;
                var vec2 = Bobcat.vecOffsetSwing1_Inches;
                var vec3 = Bobcat.vecOffsetSwing2_Inches;
                var vec4 = Bobcat.vecOffsetSwing3Rot_Inches;

                float height = (vec1.Y + vec2.Y + vec3.Y + vec4.Y) * StaticMethods.Conversion_Inches_To_Meters;
                float xset = (vec1.X + vec2.X + vec3.X + vec4.X) * StaticMethods.Conversion_Inches_To_Meters;
                const float dim_cab_thickness = 0.1f;
                const float dim_cab_thickness_o2 = dim_cab_thickness / 2;
                float dim_cab_radius = -(vec1.Z + vec2.Z + vec3.Z + vec4.Z) * StaticMethods.Conversion_Inches_To_Meters;
                float dim_cab_radius_o2 = dim_cab_radius / 2;

                float zset = -dim_cab_radius;

                Trial.xCab = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, height - dim_cab_thickness_o2, 0));
                Trial.xCab.CreateShape(
                    new CapsuleGeometry(dim_cab_radius, dim_cab_thickness_o2),
                    cat_material,
                    Matrix_PhysX.RotationZ(StaticMethods._PIF / 2));
                Trial.xCab.CreateShape(
                    new BoxGeometry(dim_cab_radius_o2, dim_cab_thickness_o2, dim_cab_radius_o2),
                    cat_material,
                    Matrix_PhysX.Translation(0, 0, -dim_cab_radius_o2));
                var jnt1 = Trial._Scene.CreateJoint<RevoluteJoint>(
                    Trial.xCab,
                    Matrix_PhysX.RotationZ(StaticMethods._PIF / 2),
                    null,
                    Matrix_PhysX.Multiply(
                         Matrix_PhysX.RotationZ(StaticMethods._PIF / 2),
                         Matrix_PhysX.Translation(xset, height - dim_cab_thickness_o2, 0)));
                Trial.xCab.SetMassAndUpdateInertia(Trial.m1);
                Trial._Scene.AddActor(Trial.xCab);




                const float middleDist = 0.5f * dim_boom_length_meters;
                const float middleHeight = 0.25f * dim_boom_length_meters;
                float ang1 = (float)Math.Atan2(middleHeight, middleDist);
                float ang2 = (float)Math.Atan2(middleHeight, dim_boom_length_meters - middleDist);
                float ang3 = 180 - ang1 - ang2;
                float leg1 = (float)Math.Sqrt(Math.Pow(middleHeight, 2) + Math.Pow(middleDist, 2));
                float leg2 = (float)Math.Sqrt(Math.Pow(middleHeight, 2) + Math.Pow((dim_boom_length_meters - middleDist), 2));
                const float boomdim = 0.1f;
                float boomdimsq2 = boomdim * (float)Math.Sqrt(2);
                Trial.xBoom = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(xset, height, zset));
                Trial.xBoom.CreateShape(
                    new BoxGeometry(boomdim, boomdimsq2 / 2, boomdimsq2 / 2),
                    cat_material,
                    Matrix_PhysX.Multiply(Matrix_PhysX.Multiply(
                        Matrix_PhysX.RotationX(StaticMethods._PIF / 4),
                        Matrix_PhysX.Translation(0, 0, -boomdim)),
                        Matrix_PhysX.RotationX(ang1)));
                Trial.xBoom.CreateShape(
                    new BoxGeometry(boomdim, boomdim, (leg1 - boomdim) / 2),
                    cat_material,
                    Matrix_PhysX.Multiply(
                         Matrix_PhysX.Translation(0, 0, -(boomdim + leg1) / 2),
                         Matrix_PhysX.RotationX(ang1)));
                Trial.xBoom.CreateShape(
                    new CapsuleGeometry(boomdim, boomdim),
                    cat_material,
                    Matrix_PhysX.Translation(0, middleHeight, -middleDist));
                Trial.xBoom.CreateShape(
                    new BoxGeometry(boomdim, boomdim, (leg2 - boomdim) / 2),
                    cat_material,
                    Matrix_PhysX.Multiply(Matrix_PhysX.Multiply(
                         Matrix_PhysX.Translation(0, 0, -(leg2 - boomdim) / 2),
                         Matrix_PhysX.RotationX(-ang2)),
                         Matrix_PhysX.Translation(0, middleHeight, -middleDist)
                         ));
                /*            this.xBoom.CreateShape( // Boom - Arm Square on Boom
                                new BoxGeometry(boomdim, boomdimsq2 / 2, boomdimsq2 / 2),
                                cat_material,
                                Matrix_PhysX.Multiply(Matrix_PhysX.Multiply(Matrix_PhysX.Multiply(
                                    Matrix_PhysX.RotationX(StaticMethods._PIF / 4),
                                    Matrix_PhysX.Translation(0, 0, boomdim)),
                                    Matrix_PhysX.RotationX(-ang2)),
                                    Matrix_PhysX.Translation(0,0,-dim_boom_length_meters)));*/
                var jnt2 = Trial._Scene.CreateJoint<RevoluteJoint>(
                    Trial.xBoom,
                    Matrix_PhysX.Identity,
                    Trial.xCab,
                    Matrix_PhysX.Translation(xset, dim_cab_thickness_o2, -dim_cab_radius));
                jnt2.Limit = new JointAngularLimitPair(-(float)Trial.Q2_Max_Radians, -(float)Trial.Q2_Min_Radians);
                jnt2.Flags |= RevoluteJointFlag.LimitEnabled;
                Trial.xBoom.SetMassAndUpdateInertia(Trial.m2);
                Trial._Scene.AddActor(xBoom);

                zset -= dim_boom_length_meters;
                const float armdim = boomdim;
                float armdimsq2 = armdim * (float)Math.Sqrt(2);
                const float dim_arm_length_meters_o2 = dim_arm_length_meters / 2;
                Trial.xArm = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(xset, height, zset));
                Trial.xArm.CreateShape(
                    new BoxGeometry(armdim, armdim, dim_arm_length_meters_o2 - armdim),
                    cat_material,
                    Matrix_PhysX.Translation(0, 0, -dim_arm_length_meters_o2));
                Trial.xArm.CreateShape(
                    new BoxGeometry(armdim, armdimsq2 / 2, armdimsq2 / 2),
                    cat_material,
                    Matrix_PhysX.Multiply(
                         Matrix_PhysX.RotationX(StaticMethods._PIF / 4),
                         Matrix_PhysX.Translation(0, 0, armdim - dim_arm_length_meters)));
                /*            this.xArm.CreateShape(// Boom - Arm Square on Arm
                                new BoxGeometry(armdim, armdimsq2 / 2, armdimsq2 / 2),
                                cat_material,
                                Matrix_PhysX.Multiply(
                                     Matrix_PhysX.RotationX(StaticMethods._PIF / 4),
                                     Matrix_PhysX.Translation(0, 0, - armdim)));*/
                var jnt3 = Trial._Scene.CreateJoint<RevoluteJoint>(
                    Trial.xArm,
                    Matrix_PhysX.Identity,
                    Trial.xBoom,
                    Matrix_PhysX.Translation(0, 0, -dim_boom_length_meters));
                jnt3.Limit = new JointAngularLimitPair(-(float)Trial.Q3_Max_Radians, -(float)Trial.Q3_Min_Radians);
                jnt3.Flags |= RevoluteJointFlag.LimitEnabled;
                Trial.xArm.SetMassAndUpdateInertia(Trial.m3);
                Trial._Scene.AddActor(Trial.xArm);

                zset -= dim_arm_length_meters;
                Trial.xBucket = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(xset, height, zset));
                CadObjectGenerator.TrianglesFromXAML(
                    out vss,
                    out iss,
                    Properties.Resources.BucketSimple,
                    xScale: StaticMethods.Conversion_Inches_To_Meters,
                    yScale: StaticMethods.Conversion_Inches_To_Meters,
                    zScale: StaticMethods.Conversion_Inches_To_Meters,
                    xOff: 0,
                    yOff: 0,
                    zOff: 0);
                var stream = new System.IO.MemoryStream();
                var meshde = new ConvexMeshDesc() { Flags = ConvexFlag.ComputeConvex };
                var vss2 = new Vector3_PhysX[vss.Length];
                for (int i = 0; i < vss.Length; i++) vss2[i] = vss[i].toPhysX();
                meshde.SetPositions(vss2);
                meshde.SetTriangles(iss);
                cooking.CookConvexMesh(meshde, stream);
                stream.Position = 0;
                var convexMeshGeom = Trial.xBucket.CreateShape(
                    new ConvexMeshGeometry(Trial._Physics.CreateConvexMesh(stream))
                    {
                        Scale = new MeshScale(new PhysX.Math.Vector3(1f, 1f, 1f), PhysX.Math.Quaternion.Identity)
                    },
                    cat_material,
                    Matrix_PhysX.Multiply(Matrix_PhysX.Multiply(
                        Matrix_PhysX.RotationY(StaticMethods._PIF / 2),
                        Matrix_PhysX.Translation(StaticMethods.Conversion_Inches_To_Meters * new Vector3_PhysX(0, (Bobcat.slidey / 2 + 2.11f), -(Bobcat.slidex / 2 - 0.365f)))),
                        Matrix_PhysX.RotationX(StaticMethods.toRadiansF(135 - 15 + 35))));
                var jnt4 = Trial._Scene.CreateJoint<RevoluteJoint>(
                    Trial.xBucket,
                    Matrix_PhysX.Identity,
                    Trial.xArm,
                    Matrix_PhysX.Translation(0, 0, -dim_arm_length_meters));
                jnt4.Limit = new JointAngularLimitPair(-(float)Trial.Q4_Max_Radians, -(float)Trial.Q4_Min_Radians);
                jnt4.Flags |= RevoluteJointFlag.LimitEnabled;
                Trial.xBucket.SetMassAndUpdateInertia(Trial.m3);
                Trial._Scene.AddActor(Trial.xBucket);

                float linkOffset = 0.23f;
                const float linkLength = 0.33f;
                Trial.xBucketLink = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(xset, height, zset + linkOffset));
                Trial.xBucketLink.CreateShape(
                    new SphereGeometry(linkLength * 0.14f),
                    cat_material,
                    Matrix_PhysX.Translation(0, linkLength * 0.65f, 0));
                Trial._RevoluteJoint_Arm_BucketLink = Trial._Scene.CreateJoint<RevoluteJoint>(
                    Trial.xBucketLink,
                    Matrix_PhysX.Identity,
                    Trial.xArm,
                    Matrix_PhysX.Translation(0, 0, linkOffset - dim_arm_length_meters));
                Trial._RevoluteJoint_Arm_BucketLink.Limit = new JointAngularLimitPair(-StaticMethods._PIF / 3, 0);
                Trial._RevoluteJoint_Arm_BucketLink.Flags |= RevoluteJointFlag.LimitEnabled;
                Trial.xBucketLink.SetMassAndUpdateInertia(5);
                Trial._Scene.AddActor(Trial.xBucketLink);














                var driveMass = 3.5f;
                var bg = new SphereGeometry(0.02f);

                // Boom Driver
                Trial.actorCabBoom1 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 0, 0)); ;
                Trial.actorCabBoom1.CreateShape(bg, cat_material, Matrix_PhysX.Identity);
                Trial.actorCabBoom1.SetMassAndUpdateInertia(driveMass);
                Trial._Scene.AddActor(actorCabBoom1);
                Trial.actorCabBoom2 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 0, -1));
                Trial.actorCabBoom2.CreateShape(bg, cat_material, Matrix_PhysX.Identity);
                Trial.actorCabBoom2.SetMassAndUpdateInertia(driveMass);
                Trial._Scene.AddActor(actorCabBoom2);
                Trial.jntCabBoom1 = Trial._Scene.CreateJoint<FixedJoint>(
                    Trial.actorCabBoom1,
                    Matrix_PhysX.Identity,
                    Trial.xCab,
                    Matrix_PhysX.Translation(xset, -.288f, -.914f)
                    );
                Trial.jntCabBoom2 = Trial._Scene.CreateJoint<FixedJoint>(
                    Trial.actorCabBoom2,
                    Matrix_PhysX.Identity,
                    Trial.xBoom,
                    Matrix_PhysX.Translation(0, 0.438f, -1.386f)
                    );

                // Arm Driver
                Trial.actorBoomArm1 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 10, 0));
                Trial.actorBoomArm1.CreateShape(bg, /*new SphereGeometry(driveRad),//*/
                    cat_material,
                    Matrix_PhysX.Identity);
                Trial.actorBoomArm1.SetMassAndUpdateInertia(driveMass);
                Trial._Scene.AddActor(actorBoomArm1);
                Trial.actorBoomArm2 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 10, -1));
                Trial.actorBoomArm2.CreateShape(bg, /*new SphereGeometry(driveRad),//*/
                    cat_material,
                    Matrix_PhysX.Identity);
                Trial.actorBoomArm2.SetMassAndUpdateInertia(driveMass);
                Trial._Scene.AddActor(actorBoomArm2);
                Trial.jntBoomArm1 = Trial._Scene.CreateJoint<FixedJoint>(
                    Trial.actorBoomArm1,
                    Matrix_PhysX.Identity,
                    Trial.xBoom,
                    Matrix_PhysX.Translation(0, 0.828f, -1.570f)
                    );
                Trial.jntBoomArm2 = Trial._Scene.CreateJoint<FixedJoint>(
                    Trial.actorBoomArm2,
                    Matrix_PhysX.Identity,
                    Trial.xArm,
                    Matrix_PhysX.Translation(0, 0.229f, 0.282f)
                    );

                // Bucket Link Driver
                Trial.actorArmBucketLink1 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 10, -2));
                Trial.actorArmBucketLink1.CreateShape(bg,
                    cat_material,
                    Matrix_PhysX.Identity);
                Trial.actorArmBucketLink1.SetMassAndUpdateInertia(driveMass);
                Trial._Scene.AddActor(actorArmBucketLink1);
                Trial.actorArmBucketLink2 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 10, -3));
                Trial.actorArmBucketLink2.CreateShape(bg,
                    cat_material,
                    Matrix_PhysX.Identity);
                Trial.actorArmBucketLink2.SetMassAndUpdateInertia(driveMass);
                Trial._Scene.AddActor(actorArmBucketLink2);
                Trial.jntArmBucketLink1 = Trial._Scene.CreateJoint<FixedJoint>(
                    Trial.actorArmBucketLink1,
                    Matrix_PhysX.Identity,
                    Trial.xArm,
                    Matrix_PhysX.Translation(0, 0.313f, -0.137f)
                    );
                Trial.jntArmBucketLink2 = Trial._Scene.CreateJoint<FixedJoint>(
                    Trial.actorArmBucketLink2,
                    Matrix_PhysX.Identity,
                    Trial.xBucketLink,
                    Matrix_PhysX.Translation(0, linkLength, 0)
                    );

                Trial.jntBucketLinkBucket = Trial._Scene.CreateJoint<DistanceJoint>(
                    Trial.actorArmBucketLink2,
                    Matrix_PhysX.Identity,
                    Trial.xBucket,
                    Matrix_PhysX.Translation(0, 0.165f, 0.148f)
                    );

                Trial.jntBucketLinkBucket.Flags = DistanceJointFlag.MinimumDistanceEnabled | DistanceJointFlag.MaximumDistanceEnabled;
                Trial.jntBucketLinkBucket.MinimumDistance = 0.25f;
                Trial.jntBucketLinkBucket.MaximumDistance = 0.25f;

                Trial.xBucketTip = Trial.xBucket.CreateShape(new BoxGeometry(0.01f, 0.01f, 0.01f),
                    cat_material,
                    Matrix_PhysX.Multiply(Matrix_PhysX.RotationX(StaticMethods._PIF * -0.13f), Matrix_PhysX.Translation(0, 0.442f, -0.605f))
                    );


/*                var temp_object = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 10f, -5));
                temp_object.CreateShape(new SphereGeometry(0.1f), cat_material);
                temp_object.SetMassAndUpdateInertia(1);
                Trial._Scene.AddActor(temp_object);

                Trial.TEST_FIX_JOINT = Trial._Scene.CreateJoint<FixedJoint>(
                    temp_object,
                    Matrix_PhysX.Identity,
                    Trial.xBucketTip,
                    Matrix_PhysX.Identity
                    );
                */






                for (int i = 0; i < 2; i++)
                {
                    var conv2 = StaticMethods.Conversion_Inches_To_Meters / 2;
                    var bin = Trial._Physics.CreateRigidStatic(
                        Matrix_PhysX.Translation(
                            EmbeddedSoilModel.dimBoxX_Inches * StaticMethods.Conversion_Inches_To_Meters * (i * 2 - 1),
                            EmbeddedSoilModel.dimBoxHeight_Inches * conv2,
                            EmbeddedSoilModel.dimBoxZ_Inches * StaticMethods.Conversion_Inches_To_Meters
                        ));

                    for (int j = 0; j < 4; j++)
                    {
                        bin.CreateShape(
                            new BoxGeometry(
                                EmbeddedSoilModel.dimBoxWidth_Inches * conv2,
                                EmbeddedSoilModel.dimBoxHeight_Inches * conv2,
                                EmbeddedSoilModel.dimBoxThickness_Inches * conv2
                                ),
                            bin_material,
                            Matrix_PhysX.Multiply(
                                Matrix_PhysX.Translation(
                                    0,
                                    0,
                                    (EmbeddedSoilModel.dimBoxWidth_Inches - EmbeddedSoilModel.dimBoxThickness_Inches) * conv2),
                                Matrix_PhysX.RotationY(j * StaticMethods._PIF / 2)));
                    }
                    Trial._Scene.AddActor(bin);
                }

                var ls = new RigidDynamic[]
                {
                    Trial.actorBoomArm1,
                    Trial.actorBoomArm2,
                    Trial.actorCabBoom1,
                    Trial.actorCabBoom2,
                    Trial.actorArmBucketLink1,
                    Trial.actorArmBucketLink2,
                    Trial.xCab,
                    Trial.xBoom,
                    Trial.xArm,
                    Trial.xBucketLink,
                    Trial.xBucket
                };

                foreach (var rig in ls)
                {
                    rig.LinearDamping = 0;
                    rig.AngularDamping = 0;
                }

                for (int j = 0; j < 5; j++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Trial._Scene.Simulate(TimeStepPhysX);
                        Trial._Scene.FetchResults(true);
                    }
                    foreach (var rig in ls)
                    {
                        rig.LinearVelocity = Vector3_PhysX.Zero;
                        rig.AngularVelocity = Vector3_PhysX.Zero;
                    }
                }

                Trial.xCab.AngularDamping = 175;
                Trial.xBoom.AngularDamping = 150;
                Trial.xArm.AngularDamping = 125;
                Trial.xBucket.AngularDamping = 25;

                foreach (Scene s in Trial._Physics.Scenes)
                {
                    foreach (var a in s.Actors)
                    {
                        var v = a as RigidDynamic;
                        if (v != null)
                        {
                            Trial.StartingPoses[v] = v.GlobalPose;
                        }
                    }
                }

                EmbeddedSoilModel.InitializePhysX(Trial._Physics, Trial._Scene);
            }
        }

        private static void staticInitTrees()
        {
            if (Trial._BoolNeedsStaticInitTrees)
            {
                Trial._BoolNeedsStaticInitTrees = false;

                Trial._CadObjectTree1 = CadObjectGenerator.fromXAML(
                    Properties.Resources.tree1,
                    ObjectName: "Tree 1",
                    xScale: 1.8f,
                    yScale: 1.8f,
                    zScale: 1.8f,
                    useAmbient: true,
                    useDiffuse: true,
                    useSpecular: true,
                    useEmission: true);

                Trial._CadObjectTree2 = CadObjectGenerator.fromXAML(
                    Properties.Resources.tree2,
                    ObjectName: "Tree 2",
                    xScale: 30.0f,
                    yScale: 30.0f,
                    zScale: 30.0f,
                    useAmbient: true,
                    useDiffuse: true,
                    useSpecular: true,
                    useEmission: true);

                Trial._CadObjectTree3 = CadObjectGenerator.fromXAML(
                    Properties.Resources.tree3,
                    ObjectName: "Tree 3",
                    xScale: 5.0f,
                    yScale: 5.0f,
                    zScale: 5.0f,
                    useAmbient: true,
                    useDiffuse: true,
                    useSpecular: true,
                    useEmission: true);

                Trial._CadObjectTree4 = CadObjectGenerator.fromXAML(
                    Properties.Resources.tree4,
                    ObjectName: "Tree 4",
                    xScale: 5.0f,
                    yScale: 5.0f,
                    zScale: 5.0f,
                    useAmbient: true,
                    useDiffuse: true,
                    useSpecular: true,
                    useEmission: true);

                Trial._CadObjectTree5 = CadObjectGenerator.fromXAML(
                    Properties.Resources.tree5,
                    ObjectName: "Tree 5",
                    xScale: 6.0f,
                    yScale: 6.0f,
                    zScale: 6.0f,
                    useAmbient: true,
                    useDiffuse: true,
                    useSpecular: true,
                    useEmission: true);

                Trial._CadObjectTree6 = CadObjectGenerator.fromXAML(
                    Properties.Resources.tree6,
                    ObjectName: "Tree 6",
                    xScale: 10.0f,
                    yScale: 10.0f,
                    zScale: 10.0f,
                    useAmbient: true,
                    useDiffuse: true,
                    useSpecular: true,
                    useEmission: true);

                var ls = new List<TreeObject>();


                ls.Add(new TreeObject(180, _FloatGroundPlaneDim * 0.5f, Trial._CadObjectTree1));
                ls.Add(new TreeObject(150, _FloatGroundPlaneDim * 1.4f, Trial._CadObjectTree2));
                ls.Add(new TreeObject(120, _FloatGroundPlaneDim * 0.9f, Trial._CadObjectTree4));
                ls.Add(new TreeObject(90, _FloatGroundPlaneDim * 0.7f, Trial._CadObjectTree6));
                ls.Add(new TreeObject(60, _FloatGroundPlaneDim * 1.1f, Trial._CadObjectTree1));
                ls.Add(new TreeObject(40, _FloatGroundPlaneDim * 2.0f, Trial._CadObjectTree2));
                ls.Add(new TreeObject(30, _FloatGroundPlaneDim * 0.9f, Trial._CadObjectTree4));
                ls.Add(new TreeObject(10, _FloatGroundPlaneDim * 0.8f, Trial._CadObjectTree6));
                ls.Add(new TreeObject(-10, _FloatGroundPlaneDim * 1.5f, Trial._CadObjectTree1));
                ls.Add(new TreeObject(-30, _FloatGroundPlaneDim * 1.1f, Trial._CadObjectTree2));
                ls.Add(new TreeObject(-60, _FloatGroundPlaneDim * 0.8f, Trial._CadObjectTree4));
                ls.Add(new TreeObject(-90, _FloatGroundPlaneDim * 0.7f, Trial._CadObjectTree6));
                ls.Add(new TreeObject(-100, _FloatGroundPlaneDim * 2.0f, Trial._CadObjectTree1));
                ls.Add(new TreeObject(-120, _FloatGroundPlaneDim * 1.0f, Trial._CadObjectTree2));
                ls.Add(new TreeObject(-130, _FloatGroundPlaneDim * 2.0f, Trial._CadObjectTree4));
                ls.Add(new TreeObject(-150, _FloatGroundPlaneDim * 1.5f, Trial._CadObjectTree6));

                Trial._Trees = ls.ToArray();
            }
        }

        public static void GLDelete_Static()
        {
            if (Trial._Heightmap != null) Trial._Heightmap.GLDelete();

            if (Trial._CadObjectTree1 != null) Trial._CadObjectTree1.GLDelete();
            if (Trial._CadObjectTree2 != null) Trial._CadObjectTree2.GLDelete();
            if (Trial._CadObjectTree3 != null) Trial._CadObjectTree3.GLDelete();
            if (Trial._CadObjectTree4 != null) Trial._CadObjectTree4.GLDelete();
            if (Trial._CadObjectTree5 != null) Trial._CadObjectTree5.GLDelete();
            if (Trial._CadObjectTree6 != null) Trial._CadObjectTree6.GLDelete();
        }

        public static void drawObjectsInShadow(bool ShadowBufferDraw, float draw_time)
        {
            if (FormBase._BoolDrawPhysX) Trial.drawPhysX();
            else
            {
                EmbeddedSoilModel.drawTrench(ShadowBufferDraw);
                EmbeddedSoilModel.drawBins(ShadowBufferDraw);
                EmbeddedSoilModel.drawPiles(draw_time);
            }
        }

        public static void drawObjectsNotInShadow(Bobcat.BobcatAngles ActualAngles)
        {
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, Trial.colorA);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, Trial.colorD);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, Trial.color0);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, Trial.color0);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, new float[] { 0 });

            Trial._Heightmap.GLDraw(ActualAngles.cab + 180, Trial.TreeAnglecutOff);

            GL.Disable(EnableCap.CullFace);
            Trial.staticInitTrees();
            foreach (TreeObject to in Trial._Trees) to.GLDraw(ActualAngles.cab);
            GL.Enable(EnableCap.CullFace);
        }

        private class UserOutput : ErrorCallback
        {
            public override void ReportError(PhysX.ErrorCode errorCode, string message, string file, int lineNumber)
            {
                if (FormBase._BoolThreadAlive)
                {
                    if (lineNumber != 187)
                        Console.WriteLine("Phys X Error: " + lineNumber + " " + file + " " + message);
                }
            }
        }

        internal static void ResetSoilModelS()
        {
            EmbeddedSoilModel.Instance.Reset();

            lock (Trial._Physics)
            {
                foreach (var ls in Trial.StartingPoses)
                {
                    ls.Key.LinearVelocity = Vector3_PhysX.Zero;
                    ls.Key.AngularVelocity = Vector3_PhysX.Zero;
                    ls.Key.GlobalPose = ls.Value;
                }
            }
        }

        internal static void SetupNone()
        {
            var t = Trial._Trial;
            Trial._Trial = null;

            if (t != null)
            {
                t.Deconstruct();
            }
        }

        private static void updatePhysXJoints(float[] JOINT_FORCES, float[] CAB_TORQUE)
        {
//            Trial.TEST_FIX_JOINT.LocalPose0 = Matrix_PhysX.Translation(new Vector3_PhysX(0, Trial.f1, Trial.f2));

            Vector3_PhysX frcedir;

            frcedir = Trial.actorCabBoom1.GlobalPose.xyz() - Trial.actorCabBoom2.GlobalPose.xyz();
            frcedir.Normalize();
            Trial.actorCabBoom1.AddForce(frcedir * JOINT_FORCES[1]);
            Trial.actorCabBoom2.AddForce(-frcedir * JOINT_FORCES[1]);

            frcedir = Trial.actorBoomArm1.GlobalPose.xyz() - Trial.actorBoomArm2.GlobalPose.xyz();
            frcedir.Normalize();
            Trial.actorBoomArm1.AddForce(frcedir * JOINT_FORCES[2]);
            Trial.actorBoomArm2.AddForce(-frcedir * JOINT_FORCES[2]);

            frcedir = Trial.actorArmBucketLink1.GlobalPose.xyz() - Trial.actorArmBucketLink2.GlobalPose.xyz();
            frcedir.Normalize();
            Trial.actorArmBucketLink1.AddForce(frcedir * JOINT_FORCES[3]);
            Trial.actorArmBucketLink2.AddForce(-frcedir * JOINT_FORCES[3]);

            var cab = Trial.xCab.GlobalPose;
            Trial.xCab.AddTorque(new Vector3_PhysX(0, CAB_TORQUE[0] * 20, 0));
        }

        private static void updatePhysXCenterCab(float Q)
        {
            float max_angle = StaticMethods.toRadiansF(4);
            float max_angle2 = max_angle / 2;

            var signQ = Math.Sign(Q);
            Q *= signQ;

            if (Q < max_angle)
                Trial.xCab.AddTorque(new Vector3_PhysX(0, (Math.Abs(max_angle2 - Q) - max_angle2) * signQ * 500000, 0));
            

        }

        /// <summary>
        /// 
        /// </summary>
        private static void drawPhysX()
        {
            Trial._ColorGL.SendToGL();

            lock (Trial._Physics)
            {
                Trial._Scene.FetchResults(true);
                foreach (var a in Trial._Scene.Actors)
                {                    
                    var v = a as RigidActor;
                    if (v != null)
                    {
                        if (v.Scene != null)
                        {
                            Trial.draw(v.Shapes);
                        }
                    }
                }
            }
        }

        private static void draw(IEnumerable<Shape> enumerable)
        {
            foreach (var shp in enumerable)
            {
                GL_Handler.PushMatrix();
                {
                    var arg = shp.GlobalPose.ToArray();
                    arg[12] *= StaticMethods.Conversion_Meters_To_Inches;
                    arg[13] *= StaticMethods.Conversion_Meters_To_Inches;
                    arg[14] *= StaticMethods.Conversion_Meters_To_Inches;
                    GL_Handler.MultiplyMatrix(arg);

                    switch (shp.GeometryType)
                    {
                        case GeometryType.Box:
                            var b = shp.GetBoxGeometry().HalfExtents;
                            SamSeifert.GLE.Draw.RectangularPrism(b.X, b.Y, b.Z, StaticMethods.Conversion_Meters_To_Inches);
                            break;
                        case GeometryType.Capsule:
                            var c = shp.GetCapsuleGeometry();
                            SamSeifert.GLE.Draw.CylinderPrism(c.Radius, c.HalfHeight, StaticMethods.Conversion_Meters_To_Inches);
                            break;
                        case GeometryType.Sphere:
                            var s = shp.GetSphereGeometry();
                            SamSeifert.GLE.Draw.Sphere(s.Radius * StaticMethods.Conversion_Meters_To_Inches);
                            break;
                        case GeometryType.ConvexMesh:
                            Bobcat.DrawBucketSimple();
                            break;
                    }
                }
                GL_Handler.PopMatrix();
            }
        }

    }
}
