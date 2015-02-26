using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.GlobalEvents;
using MatlabExcavatorWrapper;

namespace Excavator
{
    public class TrialEmbedBase : Trial
    {
        private TrialSaver _TrialSaver = null;
        private float _TimeSavingSeconds = 0;

        public TrialEmbedBase()
            : base()
        {
            InitializeComponent();
        }

        public TrialEmbedBase(
            FormBase fb,
            bool nudAcess,
            string saveFile,
            int minutes)
            : base(fb)
        {
            InitializeComponent();

            if (this.DesignMode) return;

            if (saveFile != null)
            {
                this._TimeSavingSeconds = -minutes * 60;
                this._TrialSaver = TrialSaver.FromFile(saveFile, minutes);
            }

            this.nudArm.Enabled = nudAcess;
            this.nudBoom.Enabled = nudAcess;
            this.nudBucket.Enabled = nudAcess;
            this.nudCab.Enabled = nudAcess;
            this.nudSwing.Enabled = false;

            StaticMethods.setNudValue(this.nudBoom, 0.0f);
        }

        public override void Deconstruct()
        {
            base.Deconstruct();
            if (this._ExcavatorSound != null)
            {
                this._ExcavatorSound.Stop();
                this._ExcavatorSound = null;
            }
            if (this._TrialSaver != null)
            {
                this._TrialSaver.Stop();
                this._TrialSaver = null;
            }
        }









        /// <summary>
        /// Called on Main Thread
        /// When overrode, do not call base
        /// </summary>
        public virtual void MatlabUpdateSim() { }

        /// <summary>
        /// Called on Main Thread
        /// </summary>
        public override void updateSim()        
        {
            if (this._BoolFirstTick) return;

            base.updateSim();

            this.MatlabUpdateSim();

            this.ActualAngles.cab = this.Q1;
            this.ActualAngles.swi = 0;
            this.ActualAngles.boo = this.Q2;
            this.ActualAngles.arm = this.Q3;
            this.ActualAngles.buc = this.Q4;
        }


        const float FuelMin = 0.864f;
        const float FuelMax = 2.182f;
        const float FuelRange = TrialEmbedBase.FuelMax - TrialEmbedBase.FuelMin;
        private bool _BoolFirstTick = true;

        private volatile float _FuelInstant = TrialEmbedBase.FuelMin;
        private volatile float _FuelFiltered = TrialEmbedBase.FuelMin;
        private ExcavatorSound _ExcavatorSound;


        /// <summary>
        /// When overrode, do not call base
        /// </summary>
        public virtual void MatlabUpdateGui() { }
        public override void Gui_Draw_Tick()
        {
            float fuel = this._FuelInstant;
            float filt = this._FuelFiltered;

            if (this._BoolFirstTick)
            {
                this._ExcavatorSound = new ExcavatorSound();
                this._EmbeddedSoilModel = new EmbeddedSoilModel();

                new Thread(new ThreadStart(this.MultiThread)).Start();

                this._BoolFirstTick = false;
            }
            else if (FormBase._BoolThreadAlive)
            {
                if (this._ExcavatorSound != null)
                {
                    if (FormBase._BoolMute)
                    {
                        this._ExcavatorSound.setVolume(0.0f);
                    }
                    else
                    {
                        float adj1 = (fuel - TrialEmbedBase.FuelMin) / TrialEmbedBase.FuelRange;
                        this._ExcavatorSound.setVolume(0.6f +  0.4f * adj1);
                        float adj2 = (filt - TrialEmbedBase.FuelMin) / TrialEmbedBase.FuelRange;
                        this._ExcavatorSound.setSpeed(1.0f + 0.5f * adj2);
                    }
                }
            }
            else
            {
                if (this._ExcavatorSound != null)
                {
                    this._ExcavatorSound.Stop();
                    this._ExcavatorSound = null;
                }
            }


            float act = (float)(this._StopWatchSimulateTotal.Elapsed.TotalSeconds);
            act += this._TimeSavingSeconds;

            this.delayF = (act - this._FloatSimTime);

            this.labelSimUpdate.Text =
                "Delay: " + Math.Max(0, this.delayF).ToString("0.000  ") +
                "Time: " + act.ToString("0.00");

            this.labelFuelEfficiency.Text = "Fuel Consumption: " + (fuel * 1000).ToString("0.00");
            this.MatlabUpdateGui();
        }

        private float delayF = 0;




















        private volatile float _FloatSimTime = 0.0f;
        private volatile float Q1 = 0.0f;
        private volatile float Q2 = 0.0f;
        private volatile float Q3 = 0.0f;
        private volatile float Q4 = 0.0f;

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
            float[] SD_FORCE = new float[] { 0, 0, 0 };
            float[] SD_MOMENT = new float[] { 0, 0, 0 };
            float[] SW_FORCE = new float[] { 0, 0, 0 };
            float[] FLOW = new float[] { 0, 0, 0, 0 };

            float[] Ybh = new float[] { 0, 0, 0, 0 };
            float[] Vbh = new float[] { 0, 0, 0, 0 };

            float[] CYL_POS = new float[] { 0, 0, 0, 0 };
            float[] CYL_VEL = new float[] { 0, 0, 0, 0 };

            float[] FUEL = new float[] { 0, 0 };

            float[] RAND_BACK = new float[] { 0, 0 };

            float fBucketVolumeLoad = 0;
            float fBucketMassLoad = 0;

            float fuelInstant;
            float fuelFiltered = TrialEmbedBase.FuelMin;

            int centiSeconds = -1;
            int centiSecondsNew = 0;            
            float simTime = 0;


            fixed (float *
                Q_P = Q,
                QD_P = Qd,
                SD_FORCE_P = SD_FORCE,
                SD_MOMENT_P = SD_MOMENT,
                SW_FORCE_P = SW_FORCE,
                FUEL_P = FUEL,
                FLOW_P = FLOW,
                RAND_BACK_P = RAND_BACK,
                CYL_POS_P = CYL_POS,
                CYL_VEL_P = CYL_VEL
                )
            {
                this._FloatSimTime = sim.SamUpdateClass(
                    Q_P, 
                    QD_P,
                    FUEL_P,
                    RAND_BACK_P,
                    CYL_POS_P,
                    CYL_VEL_P,
                    fBucketMassLoad,
                    SD_FORCE_P, 
                    SW_FORCE_P, 
                    SD_MOMENT_P);

                while (FormBase._BoolThreadAlive && this._BoolAlive)
                {
                    if (this._FloatSimTime < wtch.Elapsed.TotalSeconds)
                    {
                        this.MatlabUpdateSimInputs(
                            ref Q,
                            ref Qd,
                            ref CYL_POS,
                            ref CYL_VEL,
                            ref FLOW);

                        sim.setJointFlowsClass(FLOW_P);

                        Bobcat.JointToTask435(
                            ref Ybh,
                            ref Vbh,
                            ref Q,
                            ref Qd);

                        var pl = this._EmbeddedSoilModel.ForceModel_ReturnDump(
                            ref Ybh,
                            ref Vbh,
                            ref Q,
                            ref Qd,
                            ref SD_FORCE,
                            ref SD_MOMENT,
                            ref SW_FORCE,
                            out fBucketVolumeLoad,
                            this._FloatSimTime
                            );

                        if (pl.Size > 0) if (this._TrialSaver != null) this._TrialSaver.update2(pl);

                        Bobcat._FloatBucketSoilVolume = fBucketVolumeLoad;
                        fBucketMassLoad = fBucketVolumeLoad * 0.03149594f;

                        simTime = sim.SamUpdateClass(
                            Q_P, 
                            QD_P, 
                            FUEL_P,
                            RAND_BACK_P,
                            CYL_POS_P,
                            CYL_VEL_P,
                            fBucketMassLoad, 
                            SD_FORCE_P, 
                            SW_FORCE_P,
                            SD_MOMENT_P);

                        CabRotater.setValues(Q[0], Qd[0], RAND_BACK[0], RAND_BACK[1]);

                        float f = CabRotater._CAB_Q_RETURN_VALID_IF_BELOW_1000;
                        if (f < 1000) Q[0] = f;

                        this.Q1 = Q[0] * TrialEmbedBase.QSCALE;
                        this.Q2 = Q[1] * TrialEmbedBase.QSCALE;
                        this.Q3 = Q[2] * TrialEmbedBase.QSCALE;
                        this.Q4 = Q[3] * TrialEmbedBase.QSCALE;

                        fuelInstant = FUEL[0];
                        const float alpha = 0.007f;
                        fuelFiltered = fuelFiltered * (1 - alpha) + alpha * fuelInstant;

                        this._FuelInstant = fuelInstant;
                        this._FuelFiltered = fuelFiltered;
                        this._FloatSimTime = simTime;

                        if (this._TrialSaver != null)
                        {
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
                                dat.BucketSoil = fBucketVolumeLoad;
                                dat.BinSoilRight = this._EmbeddedSoilModel._VolumeRightBin;
                                dat.BinSoilLeft = this._EmbeddedSoilModel._VolumeLeftBin;
                                dat.BinSoilNone = this._EmbeddedSoilModel._VolumeNoBin;
                                dat.Fuel[0] = FUEL[0];
                                dat.Fuel[1] = FUEL[1];
                                this.fillControlFloats(dat.JoyStick);

                                if (this._TrialSaver.update1_ReturnContinue(dat)) this._TrialSaver = null;
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







        private EmbeddedSoilModel _EmbeddedSoilModel = null;
        public override void drawObjectsInShadow(bool ShadowBufferDraw)
        {
            this._EmbeddedSoilModel.drawTrench(ShadowBufferDraw);
            this._EmbeddedSoilModel.drawBins(ShadowBufferDraw);
            this._EmbeddedSoilModel.drawPiles((float)this._StopWatchSimulateTotal.Elapsed.TotalSeconds);
        }

        public override void GLDelete()
        {
            base.GLDelete();
            this._EmbeddedSoilModel.GLDelete();
        }

        public override void drawOrtho()
        {
            base.drawOrtho();
            this._EmbeddedSoilModel.drawTrenchOrtho();
        }




























































































        private Label labelSimUpdate;
        private Label labelFuelEfficiency;
        private void InitializeComponent()
        {
            this.labelSimUpdate = new System.Windows.Forms.Label();
            this.labelFuelEfficiency = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelSimUpdate
            // 
            this.labelSimUpdate.AutoSize = true;
            this.labelSimUpdate.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSimUpdate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSimUpdate.Location = new System.Drawing.Point(0, 0);
            this.labelSimUpdate.Name = "labelSimUpdate";
            this.labelSimUpdate.Padding = new System.Windows.Forms.Padding(4);
            this.labelSimUpdate.Size = new System.Drawing.Size(57, 26);
            this.labelSimUpdate.TabIndex = 115;
            this.labelSimUpdate.Text = "labelSimUpdate";
            // 
            // labelSimUpdate
            // 
            this.labelFuelEfficiency.AutoSize = true;
            this.labelFuelEfficiency.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelFuelEfficiency.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFuelEfficiency.Location = new System.Drawing.Point(0, 0);
            this.labelFuelEfficiency.Name = "labelFuelEfficiency";
            this.labelFuelEfficiency.Padding = new System.Windows.Forms.Padding(4);
            this.labelFuelEfficiency.Size = new System.Drawing.Size(57, 26);
            this.labelFuelEfficiency.TabIndex = 116;
            this.labelFuelEfficiency.Text = "labelFuelEfficiency";
            // 
            // TrialEmbedBase
            // 
            this.Controls.Add(this.labelFuelEfficiency);
            this.Controls.Add(this.labelSimUpdate);
            this.Name = "TrialEmbedBase";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
