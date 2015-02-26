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
        public static Trial _Trial { get; private set; }

        internal Bobcat.BobcatAngles ActualAnglesMatlabThread = new Bobcat.BobcatAngles();
        internal Bobcat.BobcatAngles ActualAngles = new Bobcat.BobcatAngles();
        internal Bobcat.BobcatAngles GhostAngles = new Bobcat.BobcatAngles();

        public NumericUpDown nudCab;
        public NumericUpDown nudSwing;
        public NumericUpDown nudBoom;
        public NumericUpDown nudArm;
        public NumericUpDown nudBucket;
        public Label labelFPS;

        private float fpsGL = 0;
        private float fpsGLF = 0;
        public float _TimeSpanF = 0;
        private int accumulatorCounter = 0;
        public Stopwatch _StopWatchSimulateTotal = new Stopwatch();
        private Stopwatch _StopWatchSimulateDelta = new Stopwatch();
        private Stopwatch _StopWatchFPS = new Stopwatch();
        private Label labelFuelEfficiency;
        private Label labelSimUpdate;
        private Stopwatch _StopWatchGui = new Stopwatch();

        const float FuelMin = 0.864f;
        const float FuelMax = 2.182f;
        const float FuelRange = Trial.FuelMax - Trial.FuelMin;

        private volatile float _FuelInstant = Trial.FuelMin;
        private volatile float _FuelFiltered = Trial.FuelMin;
        private ExcavatorSound _ExcavatorSound;

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

        const float _FrictionCab = 1000;
        static readonly float _FrictionCutoff = StaticMethods.toRadiansF(20);

        /// <summary>
        /// Gets total time from start of trial
        /// </summary>
        public double ElapsedTime { get { return this._StopWatchSimulateTotal.Elapsed.TotalSeconds; } }

        /// <summary>
        /// ONLY FOR INTERFACE BUILDER NEVER USE THIS SUCKER
        /// </summary>
        public Trial()
        {
            this.InitializeComponent();
        }

        ~Trial()
        {
            Console.WriteLine("Garbage Collecting Trial");
        }

        public float QS_Max = 0, QS_Min = 0;
        public float Q1_Max = 0, Q1_Min = 0;
        public float Q2_Max = 0, Q2_Min = 0;
        public float Q3_Max = 0, Q3_Min = 0;
        public float Q4_Max = 0, Q4_Min = 0;

        
        public Trial(
            FormBase fb,
            bool nudAcess,
            string saveFile,
            int minutes)
        {
            this.InitializeComponent();

            if (this.DesignMode) return;
              
            Trial.staticInit();

            this.nudCab = fb.numericUpDownCabRotation;
            this.nudSwing = fb.numericUpDownSwing;
            this.nudBoom = fb.numericUpDownBoom;
            this.nudArm = fb.numericUpDownArm;
            this.nudBucket = fb.numericUpDownBucket;
            this.labelFPS = fb.labelFPS;

            this.QS_Max = (float)this.nudSwing.Maximum; this.QS_Min = (float)this.nudSwing.Minimum;
            this.Q1_Max = (float)this.nudCab.Maximum; this.Q1_Min = (float)this.nudCab.Minimum;
            this.Q2_Max = (float)this.nudBoom.Maximum; this.Q2_Min = (float)this.nudBoom.Minimum;
            this.Q3_Max = (float)this.nudArm.Maximum; this.Q3_Min = (float)this.nudArm.Minimum;
            this.Q4_Max = (float)this.nudBucket.Maximum; this.Q4_Min = (float)this.nudBucket.Minimum;

            this.Q2_Max_Radians = StaticMethods.toRadiansD(this.nudBoom.Maximum);
            this.Q2_Min_Radians = StaticMethods.toRadiansD(this.nudBoom.Minimum);
            this.Q3_Max_Radians = StaticMethods.toRadiansD(this.nudArm.Maximum);
            this.Q3_Min_Radians = StaticMethods.toRadiansD(this.nudArm.Minimum);
            this.Q4_Max_Radians = StaticMethods.toRadiansD(this.nudBucket.Maximum);
            this.Q4_Min_Radians = StaticMethods.toRadiansD(this.nudBucket.Minimum);

            this.GhostAngles.swi = 0;
            this.GhostAngles.cab = 0;
            this.GhostAngles.boo = 40;
            this.GhostAngles.arm = -90;
            this.GhostAngles.buc = 0;

            StaticMethods.setNudValue(this.nudSwing, this.GhostAngles.swi);
            StaticMethods.setNudValue(this.nudCab, this.GhostAngles.cab);
            StaticMethods.setNudValue(this.nudBoom, this.GhostAngles.boo);
            StaticMethods.setNudValue(this.nudArm, this.GhostAngles.arm);
            StaticMethods.setNudValue(this.nudBucket, this.GhostAngles.buc);

            this.ActualAngles.copy(this.GhostAngles);

            this._StopWatchSimulateTotal.Start();
            this._StopWatchSimulateDelta.Start();
            this._StopWatchFPS.Start();
            this._StopWatchGui.Start();

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

            this.updateTime();

            this._ExcavatorSound = new ExcavatorSound();
            this._EmbeddedSoilModel = new EmbeddedSoilModel();
            this.setupPhysics();


            Trial last = Trial._Trial;
            Trial._Trial = this;
            FormBase._FormBase.RefreshTrial();

            if (last != null)
            {
                last.GLDelete();
                last.Deconstruct();
            }

            new Thread(new ThreadStart(this.MultiThread)).Start();
        }




        public float clampQS(float val) { return Math.Max(this.QS_Min, Math.Min(this.QS_Max, val)); }
        public float clampQ1(float val) { return Math.Max(this.Q1_Min, Math.Min(this.Q1_Max, val)); }
        public float clampQ2(float val) { return Math.Max(this.Q2_Min, Math.Min(this.Q2_Max, val)); }
        public float clampQ3(float val) { return Math.Max(this.Q3_Min, Math.Min(this.Q3_Max, val)); }
        public float clampQ4(float val) { return Math.Max(this.Q4_Min, Math.Min(this.Q4_Max, val)); }

        public double clampQ2_Radians(Double val)
        {
            return Math.Max(this.Q2_Min_Radians, Math.Min(this.Q2_Max_Radians, val));
        }

        public double clampQ3_Radians(Double val)
        {
            return Math.Max(this.Q3_Min_Radians, Math.Min(this.Q3_Max_Radians, val));
        }

        private double Q2_Max_Radians = 0, Q2_Min_Radians = 0;
        private double Q3_Max_Radians = 0, Q3_Min_Radians = 0;
        private double Q4_Max_Radians = 0, Q4_Min_Radians = 0;
















        /// <summary>
        /// When overrode, do not call base
        /// </summary>
        public virtual bool hasGhost()
        {
            return false;
        }

        /// <summary>
        /// When overrode, do not call base
        /// </summary>
        public virtual string getName()
        {
            return "Unnamed";
        }

        public void updateTime()
        {
            this._TimeSpanF = (float)this._StopWatchSimulateDelta.Elapsed.TotalSeconds;
            this._StopWatchSimulateDelta.Restart();

            this.accumulatorCounter++;

            if (this._StopWatchFPS.ElapsedMilliseconds > 300)
            {
                this.Gui_Label_Tick((float)this._StopWatchFPS.Elapsed.TotalSeconds);
                this._StopWatchFPS.Restart();
            }

            if (this._StopWatchGui.ElapsedMilliseconds > 30)
            {
                this._StopWatchGui.Restart();
                this.Gui_Draw_Tick();
            }
        }

        /// <summary>
        /// Called every 300 ms.  When overrode, call base
        /// </summary>
        public virtual void Gui_Draw_Tick()
        {
            float fuel = this._FuelInstant;
            float filt = this._FuelFiltered;

            if (FormBase._BoolThreadAlive)
            {
                if (this._ExcavatorSound != null)
                {
                    if (FormBase._BoolMute)
                    {
                        this._ExcavatorSound.setVolume(0.0f);
                    }
                    else
                    {
                        float adj1 = (fuel - Trial.FuelMin) / Trial.FuelRange;
                        this._ExcavatorSound.setVolume(0.6f +  0.4f * adj1);
                        float adj2 = (filt - Trial.FuelMin) / Trial.FuelRange;
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
        }

        /// <summary>
        /// Called every 30 ms.  When overrode, call base
        /// </summary>
        public virtual void Gui_Label_Tick(float accumulator)
        {
            this.fpsGL = this.accumulatorCounter / accumulator;

            const float alpha = 0.05f;

            this.fpsGL = this.accumulatorCounter / accumulator;
            if (this.fpsGL > this.fpsGLF * 1.1 || this.fpsGL < this.fpsGLF * 0.9) this.fpsGLF = this.fpsGL;
            else this.fpsGLF = this.fpsGLF * (1 - alpha) + this.fpsGL * alpha;
            this.labelFPS.Text = this.fpsGLF.ToString("00");

            this.accumulatorCounter = 0;
        }



        /// <summary>
        /// Called On Main Thread
        /// </summary>
        public virtual void updateSim()
        {
            lock (this.ActualAnglesMatlabThread)
            {
                this.ActualAngles.copy(this.ActualAnglesMatlabThread);
            }
        }



        public const float _FloatGroundPlaneDim = 600;
        public const float _FloatGroundPlaneDim2 = _FloatGroundPlaneDim * 2;
        public const int _IntTextureDensity = 32;


        private EmbeddedSoilModel _EmbeddedSoilModel = null;
        public virtual void drawObjectsInShadow(bool ShadowBufferDraw)
        {
            if (FormBase._BoolDrawPhysX) this.drawPhysX();
            else
            {
                this._EmbeddedSoilModel.drawTrench(ShadowBufferDraw);
                this._EmbeddedSoilModel.drawBins(ShadowBufferDraw);
            }
            this._EmbeddedSoilModel.drawPiles((float)this._StopWatchSimulateTotal.Elapsed.TotalSeconds);
        }

        public virtual void drawObjectsNotInShadow()
        {
            float skyboxd = FormBase._FloatSkyBoxDim;

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, Trial.colorA);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, Trial.colorD);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, Trial.color0);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, Trial.color0);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, new float[] { 0 });

            Trial._Heightmap.GLDraw(this.ActualAngles.cab + 180, Trial.AnglecutOff);

            GL.Disable(EnableCap.CullFace);
            Trial.staticInitTrees();
            foreach (TreeObject to in Trial._Trees) to.GLDraw(ref this.ActualAngles.cab);
            GL.Enable(EnableCap.CullFace);

        }

        public virtual void drawOrtho()
        {
            this._EmbeddedSoilModel.drawTrenchOrtho();
        }

        public volatile bool _BoolAlive = true;
        public virtual void Deconstruct()
        {
            if (this._BoolAlive)
            {
                // Sometimes Called Before Close
                if (this._ExcavatorSound != null)
                {
                    this._ExcavatorSound.Stop();
                    this._ExcavatorSound = null;
                }

                // Sometimes no Trial Saver
                if (this._TrialSaver != null)
                {
                    this._TrialSaver.Stop();
                    this._TrialSaver = null;
                }

//                this._Scene.Dispose();
                this._BoolAlive = false;
            }
        }

        public virtual void GLDelete()
        {
            this._EmbeddedSoilModel.GLDelete();
        }

        private TrialSaver _TrialSaver = null;
        private float _TimeSavingSeconds = 0;































        private SamSeifert.GLE.Color_GL _ColorGL = new Color_GL(Color.White);

        private Scene _Scene;

        RigidDynamic xCab, xBoom, xArm, xBucketLink, xBucket;

        FixedJoint jntCabBoom1, jntCabBoom2;
        RigidDynamic actorCabBoom1, actorCabBoom2;

        FixedJoint jntBoomArm1, jntBoomArm2;
        RigidDynamic actorBoomArm1, actorBoomArm2;

        FixedJoint jntArmBucketLink1, jntArmBucketLink2;
        RigidDynamic actorArmBucketLink1, actorArmBucketLink2;


//         RigidDynamic actorTEMP; FixedJoint fixedJOINTEMP;

        DistanceJoint jntBucketLinkBucket;

        private void setupPhysics()
        {
            this._Scene = Trial._Physics.CreateScene(new SceneDesc()
            {
                Gravity = new Vector3_PhysX(0, -9.8f, 0),
            });

            Vector3[] vss;
            int[] iss;
            var cat_material = Trial._Physics.CreateMaterial(0.1f, 0.1f, 0.0f);
            var bin_material = Trial._Physics.CreateMaterial(1.0f, 1.0f, 0);
            var grnd_material = Trial._Physics.CreateMaterial(1.0f, 1.0f, 0);
            var cooking = Trial._Physics.CreateCooking();

            var tw = EmbeddedSoilModel.TRENCH_WIDTH * StaticMethods.Conversion_Inches_To_Meters * 1.2f; // 1.2 IS SAFETY BUFFER
            const int depth = 1;
            const int hw = 10; // Half Width of The Covered Area

            var ground = Trial._Physics.CreateRigidStatic();
            ground.CreateShape(new BoxGeometry((hw - tw) / 2, depth, hw), grnd_material, Matrix_PhysX.Translation(hw / 2, -depth, 0));
            ground.CreateShape(new BoxGeometry((hw - tw) / 2, depth, hw), grnd_material, Matrix_PhysX.Translation(-hw / 2, -depth, 0));
            ground.CreateShape(new BoxGeometry(tw, depth, hw / 2), grnd_material, Matrix_PhysX.Translation(0, -depth, hw / 2));
            this._Scene.AddActor(ground);





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

            this.xCab = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, height - dim_cab_thickness_o2, 0));
            this.xCab.CreateShape(
                new CapsuleGeometry(dim_cab_radius, dim_cab_thickness_o2),
                cat_material,
                Matrix_PhysX.RotationZ(StaticMethods._PIF / 2));
            this.xCab.CreateShape(
                new BoxGeometry(dim_cab_radius_o2, dim_cab_thickness_o2, dim_cab_radius_o2),
                cat_material,
                Matrix_PhysX.Translation(0, 0, -dim_cab_radius_o2));
            var jnt1 = this._Scene.CreateJoint<RevoluteJoint>(
                this.xCab,
                Matrix_PhysX.RotationZ(StaticMethods._PIF / 2),
                null,
                Matrix_PhysX.Multiply(
                     Matrix_PhysX.RotationZ(StaticMethods._PIF / 2),
                     Matrix_PhysX.Translation(xset, height - dim_cab_thickness_o2, 0)));
            this.xCab.SetMassAndUpdateInertia(Trial.m1);
            this._Scene.AddActor(this.xCab);




            const float middleDist = 0.5f * dim_boom_length_meters;
            const float middleHeight = 0.25f * dim_boom_length_meters;
            float ang1 = (float)Math.Atan2(middleHeight, middleDist);
            float ang2 = (float)Math.Atan2(middleHeight, dim_boom_length_meters - middleDist);
            float ang3 = 180 - ang1 - ang2;
            float leg1 = (float)Math.Sqrt(Math.Pow(middleHeight, 2) + Math.Pow(middleDist, 2));
            float leg2 = (float)Math.Sqrt(Math.Pow(middleHeight, 2) + Math.Pow((dim_boom_length_meters - middleDist), 2));
            const float boomdim = 0.1f;
            float boomdimsq2 = boomdim * (float) Math.Sqrt(2);
            this.xBoom = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(xset, height, zset));
            this.xBoom.CreateShape(
                new BoxGeometry(boomdim, boomdimsq2 / 2, boomdimsq2 / 2),
                cat_material,
                Matrix_PhysX.Multiply(Matrix_PhysX.Multiply(
                    Matrix_PhysX.RotationX(StaticMethods._PIF / 4),
                    Matrix_PhysX.Translation(0, 0, -boomdim)),
                    Matrix_PhysX.RotationX(ang1)));
            this.xBoom.CreateShape(
                new BoxGeometry(boomdim, boomdim, (leg1 - boomdim) / 2),
                cat_material,
                Matrix_PhysX.Multiply(
                     Matrix_PhysX.Translation(0, 0, -(boomdim + leg1) / 2),
                     Matrix_PhysX.RotationX(ang1)));
            this.xBoom.CreateShape(
                new CapsuleGeometry(boomdim, boomdim),
                cat_material,
                Matrix_PhysX.Translation(0, middleHeight, -middleDist));
            this.xBoom.CreateShape(
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
            var jnt2 = this._Scene.CreateJoint<RevoluteJoint>(
                this.xBoom,
                Matrix_PhysX.Identity,
                this.xCab,
                Matrix_PhysX.Translation(xset, dim_cab_thickness_o2, -dim_cab_radius));
            jnt2.Limit = new JointAngularLimitPair(-(float)this.Q2_Max_Radians, -(float)this.Q2_Min_Radians);
            jnt2.Flags |= RevoluteJointFlag.LimitEnabled;
            this.xBoom.SetMassAndUpdateInertia(Trial.m2);
            this._Scene.AddActor(xBoom);

            zset -= dim_boom_length_meters;
            const float armdim = boomdim;
            float armdimsq2 = armdim * (float) Math.Sqrt(2);
            const float dim_arm_length_meters_o2 = dim_arm_length_meters / 2;
            this.xArm = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(xset, height, zset));
            this.xArm.CreateShape(
                new BoxGeometry(armdim, armdim, dim_arm_length_meters_o2 - armdim),
                cat_material,
                Matrix_PhysX.Translation(0, 0, - dim_arm_length_meters_o2));
            this.xArm.CreateShape(
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
            var jnt3 = this._Scene.CreateJoint<RevoluteJoint>(
                this.xArm,
                Matrix_PhysX.Identity,
                this.xBoom,
                Matrix_PhysX.Translation(0, 0, -dim_boom_length_meters));
            jnt3.Limit = new JointAngularLimitPair(-(float)this.Q3_Max_Radians, -(float)this.Q3_Min_Radians);
            jnt3.Flags |= RevoluteJointFlag.LimitEnabled;
            this.xArm.SetMassAndUpdateInertia(Trial.m3);
            this._Scene.AddActor(this.xArm);

            zset -= dim_arm_length_meters;
            this.xBucket = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(xset, height, zset));
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
            var convexMeshGeom = this.xBucket.CreateShape(
                new ConvexMeshGeometry(Trial._Physics.CreateConvexMesh(stream))
                {
                    Scale = new MeshScale(new PhysX.Math.Vector3(1f, 1f, 1f), PhysX.Math.Quaternion.Identity)
                },
                cat_material,
                Matrix_PhysX.Multiply(Matrix_PhysX.Multiply(
                    Matrix_PhysX.RotationY(StaticMethods._PIF / 2),
                    Matrix_PhysX.Translation(StaticMethods.Conversion_Inches_To_Meters * new Vector3_PhysX(0, (Bobcat.slidey / 2 + 2.11f), -(Bobcat.slidex / 2 - 0.365f)))),
                    Matrix_PhysX.RotationX(StaticMethods.toRadiansF(135 - 15 + 35))));
            var jnt4 = this._Scene.CreateJoint<RevoluteJoint>(
                this.xBucket,
                Matrix_PhysX.Identity,
                this.xArm,
                Matrix_PhysX.Translation(0, 0, -dim_arm_length_meters));
            jnt4.Limit = new JointAngularLimitPair(-(float)this.Q4_Max_Radians, -(float)this.Q4_Min_Radians);
            jnt4.Flags |= RevoluteJointFlag.LimitEnabled;
            this.xBucket.SetMassAndUpdateInertia(Trial.m3);
            this._Scene.AddActor(this.xBucket);

            float linkOffset = 0.23f;
            const float linkLength = 0.33f;
            this.xBucketLink = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(xset, height, zset + linkOffset));
            this.xBucketLink.CreateShape(
                new SphereGeometry(linkLength * 0.14f),
                cat_material,
                Matrix_PhysX.Translation(0, linkLength * 0.65f, 0));
            var jnt5 = this._Scene.CreateJoint<RevoluteJoint>(
                this.xBucketLink,
                Matrix_PhysX.Identity,
                this.xArm,
                Matrix_PhysX.Translation(0, 0, linkOffset - dim_arm_length_meters));
            jnt5.Limit = new JointAngularLimitPair(-StaticMethods._PIF / 3, StaticMethods._PIF / 3);
            jnt5.Flags |= RevoluteJointFlag.LimitEnabled;
            this.xBucketLink.SetMassAndUpdateInertia(5);
            this._Scene.AddActor(this.xBucketLink);














            var driveMass = 5.5f;
            var bg = new SphereGeometry(0.06f);

            // Boom Driver
            this.actorCabBoom1 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 0, 0)); ;
            this.actorCabBoom1.CreateShape(bg, cat_material, Matrix_PhysX.Identity);
            this.actorCabBoom1.SetMassAndUpdateInertia(driveMass);
            this._Scene.AddActor(actorCabBoom1);
            this.actorCabBoom2 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 0, -1));
            this.actorCabBoom2.CreateShape(bg, cat_material, Matrix_PhysX.Identity);
            this.actorCabBoom2.SetMassAndUpdateInertia(driveMass);
            this._Scene.AddActor(actorCabBoom2);
            this.jntCabBoom1 = this._Scene.CreateJoint<FixedJoint>(
                this.actorCabBoom1,
                Matrix_PhysX.Identity,
                this.xCab,
                Matrix_PhysX.Translation(xset, -.288f, -.914f)
                );
            this.jntCabBoom2 = this._Scene.CreateJoint<FixedJoint>(
                this.actorCabBoom2,
                Matrix_PhysX.Identity,
                this.xBoom,
                Matrix_PhysX.Translation(0, 0.438f, -1.386f)
                );

            // Arm Driver
            this.actorBoomArm1 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 10, 0));
            this.actorBoomArm1.CreateShape(bg, /*new SphereGeometry(driveRad),//*/
                cat_material,
                Matrix_PhysX.Identity);
            this.actorBoomArm1.SetMassAndUpdateInertia(driveMass);
            this._Scene.AddActor(actorBoomArm1);
            this.actorBoomArm2 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 10, -1));
            this.actorBoomArm2.CreateShape(bg, /*new SphereGeometry(driveRad),//*/
                cat_material,
                Matrix_PhysX.Identity);
            this.actorBoomArm2.SetMassAndUpdateInertia(driveMass);
            this._Scene.AddActor(actorBoomArm2);
            this.jntBoomArm1 = this._Scene.CreateJoint<FixedJoint>(
                this.actorBoomArm1,
                Matrix_PhysX.Identity,
                this.xBoom,
                Matrix_PhysX.Translation(0, 0.828f, -1.570f)
                );
            this.jntBoomArm2 = this._Scene.CreateJoint<FixedJoint>(
                this.actorBoomArm2,
                Matrix_PhysX.Identity,
                this.xArm,
                Matrix_PhysX.Translation(0, 0.229f, 0.282f)
                );

            // Bucket Link Driver
            this.actorArmBucketLink1 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 10, -2));
            this.actorArmBucketLink1.CreateShape(bg,
                cat_material,
                Matrix_PhysX.Identity);
            this.actorArmBucketLink1.SetMassAndUpdateInertia(driveMass);
            this._Scene.AddActor(actorArmBucketLink1);
            this.actorArmBucketLink2 = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 10, -3));
            this.actorArmBucketLink2.CreateShape(bg,
                cat_material,
                Matrix_PhysX.Identity);
            this.actorArmBucketLink2.SetMassAndUpdateInertia(driveMass);
            this._Scene.AddActor(actorArmBucketLink2);
            this.jntArmBucketLink1 = this._Scene.CreateJoint<FixedJoint>(
                this.actorArmBucketLink1,
                Matrix_PhysX.Identity,
                this.xArm,
                Matrix_PhysX.Translation(0, 0.313f, -0.137f)
                );
            this.jntArmBucketLink2 = this._Scene.CreateJoint<FixedJoint>(
                this.actorArmBucketLink2,
                Matrix_PhysX.Identity,
                this.xBucketLink,
                Matrix_PhysX.Translation(0, linkLength, 0)
                );

            /*
            this.actorTEMP = Trial._Physics.CreateRigidDynamic(Matrix_PhysX.Translation(0, 10, -3));
            this.actorTEMP.CreateShape(bg,
                cat_material,
                Matrix_PhysX.Identity);
            this.actorTEMP.SetMassAndUpdateInertia(driveMass);
            this._Scene.AddActor(actorTEMP);
            this.fixedJOINTEMP = this._Scene.CreateJoint<FixedJoint>(
                this.actorTEMP,
                Matrix_PhysX.Identity,
                this.xBucket,
                Matrix_PhysX.Translation(0, 0.165f, 0.148f)
                );*/

            this.jntBucketLinkBucket = this._Scene.CreateJoint<DistanceJoint>(
                this.actorArmBucketLink2,
                Matrix_PhysX.Identity,
                this.xBucket,
                Matrix_PhysX.Translation(0, 0.165f, 0.148f)
                );
            this.jntBucketLinkBucket.Flags = (DistanceJointFlag.MinimumDistanceEnabled | DistanceJointFlag.MaximumDistanceEnabled);
            this.jntBucketLinkBucket.MinimumDistance = 0.25f;
            this.jntBucketLinkBucket.MaximumDistance = 0.25f;








            for (int i = 0; i < 2; i++)
            {
                var conv2 = StaticMethods.Conversion_Inches_To_Meters / 2;
                var bin = Trial._Physics.CreateRigidStatic(
                    Matrix_PhysX.Translation(
                        EmbeddedSoilModel.dimBoxX_Inches * StaticMethods.Conversion_Inches_To_Meters * (i * 2 - 1),
                        EmbeddedSoilModel.dimBoxHeight_Inches * conv2,
                        - EmbeddedSoilModel.dimBoxZ_Inches * StaticMethods.Conversion_Inches_To_Meters
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
                this._Scene.AddActor(bin);               
            }

            var ls = new RigidDynamic[]
            {
                this.actorBoomArm1,
                this.actorBoomArm2,
                this.actorCabBoom1,
                this.actorCabBoom2,
                this.actorArmBucketLink1,
                this.actorArmBucketLink2,
                this.xCab,
                this.xBoom,
                this.xArm,
                this.xBucketLink,
                this.xBucket
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
                    this._Scene.Simulate(TimeStepPhysX);
                    this._Scene.FetchResults(true);
                }
                foreach (var rig in ls)
                {
                    rig.LinearVelocity = Vector3_PhysX.Zero;
                    rig.AngularVelocity = Vector3_PhysX.Zero;
                }
            }

//            this.xBucketLink.AngularDamping = 100;
//            this.xBoom.AngularDamping = 100;
//            this.xArm.AngularDamping = 80;
//            this.xBucket.AngularDamping = 40;
        }
        public static volatile float f1 = 1, f2 = -1, f3, f4;


        const float damping_cab_boom = 25000;
        const float damping_boom_arm = 25000;
        const float damping_arm_bucket = 25000;
        const float maxf_cab_boom = 0;
        const float maxf_boom_arm = 0;
        const float maxf_arm_bucket = 0;

        private void updatePhysXJoints(float[] JOINT_FORCES, float[] CAB_TORQUE)
        {
//            this.jntBucketLinkBucket.MinimumDistance = Trial.f1;
//            this.jntBucketLinkBucket.MaximumDistance = Trial.f1;

//            this.jntDArmBucket1.LocalPose1 = Matrix_PhysX.Translation(new Vector3_PhysX(0, Trial.f1, Trial.f2));
//            this.jntDArmBucket2.LocalPose1 = Matrix_PhysX.Translation(new Vector3_PhysX(0, Trial.f3, Trial.f4));

//            this.fixedJOINTEMP.LocalPose1 = Matrix_PhysX.Translation(new Vector3_PhysX(0, Trial.f1, Trial.f2));

            Vector3_PhysX frcedir;

            frcedir = this.actorCabBoom1.GlobalPose.xyz() - this.actorCabBoom2.GlobalPose.xyz();
            frcedir.Normalize();
            this.actorCabBoom1.AddForce(frcedir * JOINT_FORCES[1]);
            this.actorCabBoom2.AddForce(-frcedir * JOINT_FORCES[1]);

            frcedir = this.actorBoomArm1.GlobalPose.xyz() - this.actorBoomArm2.GlobalPose.xyz();
            frcedir.Normalize();
            this.actorBoomArm1.AddForce(frcedir * JOINT_FORCES[2]);
            this.actorBoomArm2.AddForce(-frcedir * JOINT_FORCES[2]);

            frcedir = this.actorArmBucketLink1.GlobalPose.xyz() - this.actorArmBucketLink2.GlobalPose.xyz();
            frcedir.Normalize();
            this.actorArmBucketLink1.AddForce(frcedir * JOINT_FORCES[3]);
            this.actorArmBucketLink2.AddForce(-frcedir * JOINT_FORCES[3]);

            var cab = this.xCab.GlobalPose;
            this.xCab.AddTorque(new Vector3_PhysX(0, CAB_TORQUE[0], 0));

//            this.xArm.AddTorque(Vector4_PhysX.Transform(new Vector4_PhysX(JOINT_TORQUES[2], 0, 0, 0), cab).xyz());
            
            /*this.xBucket.AddTorque(Vector4_PhysX.Transform(
                new Vector4_PhysX(JOINT_TORQUES[3], 0, 0, 0), cab).xyz());

            this.xCab.AddTorque(new Vector3_PhysX(
                0,
                -_FrictionCab * Math.Sign(Qd[0]) * Math.Max(Math.Abs(Qd[0]),
                _FrictionCutoff), 0));*/
                        
        }

        /// <summary>
        /// 
        /// </summary>
        private void drawPhysX()
        {
            this._ColorGL.SendToGL();

            lock (Trial._Physics)
            {
                this._Scene.FetchResults(true);
                foreach (var a in this._Scene.Actors)
                {
                    var v = a as RigidActor;
                    if (v != null)
                    this.draw(v.Shapes);
                }
            }
        }

        private void draw(IEnumerable<Shape> enumerable)
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
                        case GeometryType.Plane:
                            const float p = 100 * StaticMethods.Conversion_Meters_To_Inches;
                            GL.Begin(BeginMode.Quads);
                            GL.Normal3(Vector3.UnitX);
                            GL.Vertex3(0, p, p);  
                            GL.Vertex3(0, -p, p);  
                            GL.Vertex3(0, -p, -p);  
                            GL.Vertex3(0, p, -p);  
                            GL.End();
                            break;
                        case GeometryType.ConvexMesh:
                            Bobcat.DrawSimpleBucket();
//                            var m = shp.GetConvexMeshGeometry();
//                            var vrts = m.GetVertices();
//                            var inds = m.GetIndexBuffer();
//                            Console.WriteLine(inds.Length);
                            break;
                    }
                }
                GL_Handler.PopMatrix();
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
                float fBucketVolumeLoad = 0;
                float fBucketMassLoad = 0;

                float fuelInstant;
                float fuelFiltered = Trial.FuelMin;

                int centiSeconds = -1;
                int centiSecondsNew = 0;
                float simTime = 0;

                this.UpdateMatlabInputs(ref Q, ref Qd, ref CYL_POS, ref CYL_VEL);

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

//                        if (pl.Size > 0) if (this._TrialSaver != null) this._TrialSaver.update2(pl);

                        Bobcat._FloatBucketSoilVolume = fBucketVolumeLoad;
                        fBucketMassLoad = fBucketVolumeLoad * 0.03149594f;

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
                            this.updatePhysXJoints(JOINT_FORCES, CAB_TORQUE);

                            this._Scene.Simulate(TimeStepPhysX);
                            this._Scene.FetchResults(true);

                            this.UpdateMatlabInputs(ref Q, ref Qd, ref CYL_POS, ref CYL_VEL);
                        }


                        lock (this.ActualAnglesMatlabThread)
                        {
                            this.ActualAnglesMatlabThread.cab = Q[0] * Trial.QSCALE;
                            this.ActualAnglesMatlabThread.boo = Q[1] * Trial.QSCALE;
                            this.ActualAnglesMatlabThread.arm = Q[2] * Trial.QSCALE;
                            this.ActualAnglesMatlabThread.buc = Q[3] * Trial.QSCALE;
                        }

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

        private void UpdateMatlabInputs(ref float[] Q, ref float[] Qd, ref float[] CYL_POS, ref float[] CYL_VEL)
        {
            // UPDATE Q

            Vector4_PhysX targ1, targ2, cabPerp = Vector4_PhysX.Transform(Vector4_PhysX.UnitX, this.xCab.GlobalPose);
            Vector3_PhysX cross;
            float newQ;

            targ1 = Vector4_PhysX.Transform(Vector4_PhysX.UnitX, this.xCab.GlobalPose);
            Q[0] = -(float)Math.Atan2(targ1.Z, targ1.X); ;

            var cabX = (targ1.xyz());

            targ1 = Vector4_PhysX.Transform(Vector4_PhysX.UnitZ, this.xCab.GlobalPose);
            targ2 = Vector4_PhysX.Transform(Vector4_PhysX.UnitZ, this.xBoom.GlobalPose);
            cross = PhysX.Math.Vector3.Cross(targ1.xyz(), targ2.xyz());
            if (Single.IsNaN(cross.LengthSquared())) newQ = 0;
            else newQ = (float)Math.Acos(Vector4_PhysX.Dot(targ1, targ2)) *
                Math.Sign(Vector4_PhysX.Dot(new Vector4_PhysX(cross, 0), cabPerp));
            Q[1] = newQ;

            targ1 = targ2;
            targ2 = Vector4_PhysX.Transform(Vector4_PhysX.UnitZ, this.xArm.GlobalPose);
            cross = PhysX.Math.Vector3.Cross(targ1.xyz(), targ2.xyz());
            if (Single.IsNaN(cross.LengthSquared())) newQ = 0;
            else newQ = (float)Math.Acos(Vector4_PhysX.Dot(targ1, targ2)) *
                Math.Sign(Vector4_PhysX.Dot(new Vector4_PhysX(cross, 0), cabPerp));
            Q[2] = newQ;

            targ1 = targ2;
            targ2 = Vector4_PhysX.Transform(Vector4_PhysX.UnitZ, this.xBucket.GlobalPose);
            cross = PhysX.Math.Vector3.Cross(targ1.xyz(), targ2.xyz());
            if (Single.IsNaN(cross.LengthSquared())) newQ = 0;
            else newQ = (float)
                Math.Acos(Math.Max(-1, Math.Min(1, Vector4_PhysX.Dot(targ1, targ2)))) *
                Math.Sign(Vector4_PhysX.Dot(new Vector4_PhysX(cross, 0), cabPerp));
            Q[3] = newQ;

            // Update Qd

            Qd[0] = Vector3_PhysX.Dot(this.xCab.AngularVelocity, Vector3_PhysX.UnitY);
            Qd[1] = Vector3_PhysX.Dot(this.xBoom.AngularVelocity, cabX);
            Qd[2] = Vector3_PhysX.Dot(this.xArm.AngularVelocity, cabX);
            Qd[3] = Vector3_PhysX.Dot(this.xBucket.AngularVelocity, cabX);

            // Udate Cyl_Pos

            Vector3_PhysX v3pos1 = this.actorCabBoom1.GlobalPose.xyz() - this.actorCabBoom2.GlobalPose.xyz();
            Vector3_PhysX v3pos2 = this.actorBoomArm1.GlobalPose.xyz() - this.actorBoomArm2.GlobalPose.xyz();
            Vector3_PhysX v3pos3 = this.actorArmBucketLink1.GlobalPose.xyz() - this.actorArmBucketLink2.GlobalPose.xyz();

            CYL_POS[0] = Q[0];
            CYL_POS[1] = v3pos1.Normalize() * StaticMethods.Conversion_Meters_To_Inches;
            CYL_POS[2] = v3pos2.Normalize() * StaticMethods.Conversion_Meters_To_Inches;
            CYL_POS[3] = v3pos3.Normalize() * StaticMethods.Conversion_Meters_To_Inches;

            // Update Cyl_Vel

            CYL_VEL[0] = Qd[0];
            CYL_VEL[1] = StaticMethods.Conversion_Meters_To_Inches *
                Vector3_PhysX.Dot(this.actorCabBoom1.LinearVelocity - this.actorCabBoom2.LinearVelocity, v3pos1);
            CYL_VEL[2] = StaticMethods.Conversion_Meters_To_Inches *
                Vector3_PhysX.Dot(this.actorBoomArm1.LinearVelocity - this.actorBoomArm2.LinearVelocity, v3pos2);
            CYL_VEL[3] = StaticMethods.Conversion_Meters_To_Inches *
                Vector3_PhysX.Dot(this.actorArmBucketLink1.LinearVelocity - this.actorArmBucketLink2.LinearVelocity, v3pos3);
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
















































































































































        public static HeightMap _Heightmap = null;

        private static readonly Color colorC = Color.FromArgb(89, 113, 38);
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

        private const int AnglecutOff = 80;

        public class TreeObject
        {
            public CadObject co;
            public float angle;
            public float distance;

            public TreeObject(float a, float d, CadObject o) { this.angle = a; this.distance = d; this.co = o; }

            public void GLDraw(ref float cab)
            {
                float dif = Math.Abs(this.angle - cab);
                if (dif < Trial.AnglecutOff || dif > (360 - Trial.AnglecutOff))
                {
                    GL.PushMatrix();
                    {
                        GL.Rotate(angle, OpenTK.Vector3.UnitY);
                        GL.Translate(0, 0, -distance);
                        co.draw(true);
                    }
                    GL.PopMatrix();
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

        protected static Foundation _Foundation = new Foundation(new UserOutput());
        private static Physics _Physics = new Physics(Trial._Foundation, false);
        private class UserOutput : ErrorCallback
        {
            public override void ReportError(PhysX.ErrorCode errorCode, string message, string file, int lineNumber)
            {
                Console.WriteLine("Phys X Error:");
                Console.WriteLine(message);
                Console.WriteLine(file);
                Console.WriteLine(lineNumber);
            }
        }
    }
}
