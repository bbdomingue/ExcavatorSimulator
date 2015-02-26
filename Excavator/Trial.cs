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
using SamSeifert.GLE;
using SamSeifert.GLE.CadViewer;

using OpenTK.Graphics.OpenGL;

namespace Excavator
{
    public partial class Trial : UserControl
    {
        public static Trial _Trial { get; private set; }
        private static Trial _TrialNext;

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
        private Stopwatch _StopWatchGui = new Stopwatch();






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

        public Trial(FormBase fb)
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

            this.nudCab.Enabled = false;
            this.nudSwing.Enabled = false;
            this.nudBoom.Enabled = false;
            this.nudArm.Enabled = false;
            this.nudBucket.Enabled = false;

            this.QS_Max = (float)this.nudSwing.Maximum; this.QS_Min = (float)this.nudSwing.Minimum;
            this.Q1_Max = (float)this.nudCab.Maximum; this.Q1_Min = (float)this.nudCab.Minimum;
            this.Q2_Max = (float)this.nudBoom.Maximum; this.Q2_Min = (float)this.nudBoom.Minimum;
            this.Q3_Max = (float)this.nudArm.Maximum; this.Q3_Min = (float)this.nudArm.Minimum;
            this.Q4_Max = (float)this.nudBucket.Maximum; this.Q4_Min = (float)this.nudBucket.Minimum;

            this.Q2_Max_Radians = StaticMethods.toRadiansD(this.nudBoom.Maximum);
            this.Q2_Min_Radians = StaticMethods.toRadiansD(this.nudBoom.Minimum);
            this.Q3_Max_Radians = StaticMethods.toRadiansD(this.nudArm.Maximum);
            this.Q3_Min_Radians = StaticMethods.toRadiansD(this.nudArm.Minimum);

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

            if (Trial._Trial == null)
            {
                Trial._Trial = this;
                FormBase._FormBase.RefreshTrial();
            }
            else Trial._TrialNext = this;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Trial
            // 
            this.Name = "Trial";
            this.Size = new System.Drawing.Size(213, 248);
            this.ResumeLayout(false);
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

        public virtual void updateTime()
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
        /// Called every 309 ms.  When overrode, call base
        /// </summary>
        public virtual void Gui_Draw_Tick()
        {
        }

        /// <summary>
        /// Called every 30 ms.  When overrode, call base
        /// </summary>
        public virtual void Gui_Label_Tick(float accumulator)
        {
            if (Trial._TrialNext == null)
            {
                this.fpsGL = this.accumulatorCounter / accumulator;

                const float alpha = 0.05f;

                this.fpsGL = this.accumulatorCounter / accumulator;
                if (this.fpsGL > this.fpsGLF * 1.1 || this.fpsGL < this.fpsGLF * 0.9) this.fpsGLF = this.fpsGL;
                else this.fpsGLF = this.fpsGLF * (1 - alpha) + this.fpsGL * alpha;
                this.labelFPS.Text = this.fpsGLF.ToString("00");

                this.accumulatorCounter = 0;
            }
            else
            {
                this.GLDelete();
                this.Deconstruct();
                Trial._Trial = Trial._TrialNext;
                Trial._TrialNext = null;
                Trial._Trial.updateTime();
                FormBase._FormBase.RefreshTrial();
            }
        }



        /// <summary>
        /// When overrode, don't call base.
        /// </summary>
        public virtual void updateSim()
        {
        }



        public const float _FloatGroundPlaneDim = 600;
        public const float _FloatGroundPlaneDim2 = _FloatGroundPlaneDim * 2;
        public const int _IntTextureDensity = 32;

        public virtual void drawObjectsInShadow(bool ShadowBufferDraw)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Trial._IntTextureGrass);

            GL.Begin(BeginMode.Quads);
            {
                GL.TexCoord2(Trial._IntTextureDensity, Trial._IntTextureDensity);
                GL.Vertex3(Trial._FloatGroundPlaneDim, 0, Trial._FloatGroundPlaneDim);
                GL.TexCoord2(Trial._IntTextureDensity, 0);
                GL.Vertex3(Trial._FloatGroundPlaneDim, 0, -Trial._FloatGroundPlaneDim);
                GL.TexCoord2(0, 0);
                GL.Vertex3(-Trial._FloatGroundPlaneDim, 0, -Trial._FloatGroundPlaneDim);
                GL.TexCoord2(0, Trial._IntTextureDensity);
                GL.Vertex3(-Trial._FloatGroundPlaneDim, 0, Trial._FloatGroundPlaneDim);
            }
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture7);
        }

        public void drawObjectsNotInShadow()
        {
            float skyboxd = FormBase._FloatSkyBoxDim;

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, Trial.colorA);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, Trial.colorD);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, Trial.color0);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, Trial.color0);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, new float[] { 0 });

            Trial._Heightmap.GLDraw(this.ActualAngles.cab + 180, Trial.AnglecutOff);

            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);

            if (Trial._IntTextureGrass == 0)
                Trial._IntTextureGrass = Textures.getGLTexture(Properties.Resources.grass1);

            GL.Color3(Color.White);
            GL.BindTexture(TextureTarget.Texture2D, Trial._IntTextureGrass);

/*            GL.Begin(BeginMode.Quads);
            {
                float xa;
                float za;

                for (int x = -1; x < 2; x++)
                {
                    xa = Trial._FloatGroundPlaneDim2 * x;
                    for (int z = -1; z < 2; z++)
                    {
                        za = Trial._FloatGroundPlaneDim2 * z;
                        if (x != 0 || z != 0)
                        {
                            GL.TexCoord2(Trial._IntTextureDensity, Trial._IntTextureDensity);
                            GL.Vertex3(Trial._FloatGroundPlaneDim + xa, 0, Trial._FloatGroundPlaneDim + za);
                            GL.TexCoord2(Trial._IntTextureDensity, 0);
                            GL.Vertex3(Trial._FloatGroundPlaneDim + xa, 0, -Trial._FloatGroundPlaneDim + za);
                            GL.TexCoord2(0, 0);
                            GL.Vertex3(-Trial._FloatGroundPlaneDim + xa, 0, -Trial._FloatGroundPlaneDim + za);
                            GL.TexCoord2(0, Trial._IntTextureDensity);
                            GL.Vertex3(-Trial._FloatGroundPlaneDim + xa, 0, Trial._FloatGroundPlaneDim + za);
                        }
                    }
                }
            }
            GL.End();*/

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);

            GL.Disable(EnableCap.CullFace);

            Trial.staticInitTrees();

            foreach (TreeObject to in Trial._Trees) to.GLDraw(ref this.ActualAngles.cab);

            GL.Enable(EnableCap.CullFace);
            
        }

        public virtual void drawOrtho()
        {
        }

        public volatile bool _BoolAlive = true;
        public virtual void Deconstruct()
        {
            this._BoolAlive = false;
        }

        public virtual void GLDelete()
        {
        }
















        public static int _IntTextureGrass {get; private set;}
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

            if (Trial._IntTextureGrass != 0)
            {
                GL.DeleteTexture(Trial._IntTextureGrass);
                Trial._IntTextureGrass = 0;
            }

            if (Trial._CadObjectTree1 != null) Trial._CadObjectTree1.GLDelete();
            if (Trial._CadObjectTree2 != null) Trial._CadObjectTree2.GLDelete();
            if (Trial._CadObjectTree3 != null) Trial._CadObjectTree3.GLDelete();
            if (Trial._CadObjectTree4 != null) Trial._CadObjectTree4.GLDelete();
            if (Trial._CadObjectTree5 != null) Trial._CadObjectTree5.GLDelete();
            if (Trial._CadObjectTree6 != null) Trial._CadObjectTree6.GLDelete();
        }
    }
}
