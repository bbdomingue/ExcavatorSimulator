using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.GLE;
using SamSeifert.GlobalEvents;
using SamSeifert.GLE.CadViewer;

using OpenTK.Graphics.OpenGL;

namespace Excavator
{
    internal partial class Trial : UserControl
    {
        internal static Trial _Trial;

        internal Bobcat.BobcatAngles ActualAngles = new Bobcat.BobcatAngles();
        internal Bobcat.BobcatAngles GhostAngles = new Bobcat.BobcatAngles();

        internal NumericUpDown nudCab;
        internal NumericUpDown nudSwing;
        internal NumericUpDown nudBoom;
        internal NumericUpDown nudArm;
        internal NumericUpDown nudBucket;
        internal Label labelFPS;

        private float fpsGL = 0;
        private float fpsGLF = 0;
        internal float _TimeSpanF = 0;
        private float accumulator = 0;
        private int accumulatorCounter = 0;
        private Stopwatch _StopWatchSimulate = new Stopwatch();
        private Stopwatch _StopWatchFPS = new Stopwatch();



        /// <summary>
        /// ONLY FOR INTERFACE BUILDER NEVER USE THIS SUCKER
        /// </summary>
        internal Trial()
        {
        }

        internal Trial(FormBase fb)
        {
            InitializeComponent();

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

            this._StopWatchSimulate.Start();
            this._StopWatchFPS.Start();



            fb.loadTrial(this);
            Trial._Trial = this;
        }


















        internal virtual bool hasGhost()
        {
            return false;
        }

        internal virtual string getName()
        {
            return "Unnamed";
        }

        internal virtual void updateTime()
        {
            this._TimeSpanF = (float)this._StopWatchSimulate.Elapsed.TotalSeconds;
            this._StopWatchSimulate.Restart();

            this.accumulator = (float)this._StopWatchFPS.Elapsed.TotalSeconds;
            this.accumulatorCounter++;

            if (this.accumulator > 0.33f)
            {
                this._StopWatchFPS.Restart();
                this.fpsGL = this.accumulatorCounter / this.accumulator;

                const float alpha = 0.05f;

                this.fpsGL = this.accumulatorCounter / this.accumulator;
                if (this.fpsGL > this.fpsGLF * 1.1 || this.fpsGL < this.fpsGLF * 0.9) this.fpsGLF = this.fpsGL;
                else this.fpsGLF = this.fpsGLF * (1 - alpha) + this.fpsGL * alpha;
                this.labelFPS.Text = this.fpsGLF.ToString("00");

                this.accumulatorCounter = 0;
            }
        }

        internal virtual void backgroundThreadsChanged()
        {
        }


        internal virtual void updateSim()
        {
        }



        internal const float _FloatGroundPlaneDim = 600;
        internal const float _FloatGroundPlaneDim2 = _FloatGroundPlaneDim * 2;
        internal const int _IntTextureDensity = 32;

        internal virtual void drawObjectsInShadow(bool uselighting)
        {
            if (uselighting)
            {
                if (Trial._IntTextureGrass == 0)
                {
                    Trial._IntTextureGrass = Textures.getGLTexture(Properties.Resources.grass1);
                }
                else
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
            }
            else
            {
                GL.Normal3(OpenTK.Vector3.UnitY);
                GL.Begin(BeginMode.Quads);
                {
                    GL.Vertex3(Trial._FloatGroundPlaneDim, 0, Trial._FloatGroundPlaneDim);
                    GL.Vertex3(Trial._FloatGroundPlaneDim, 0, -Trial._FloatGroundPlaneDim);
                    GL.Vertex3(-Trial._FloatGroundPlaneDim, 0, -Trial._FloatGroundPlaneDim);
                    GL.Vertex3(-Trial._FloatGroundPlaneDim, 0, Trial._FloatGroundPlaneDim);
                }
                GL.End();
            }
        }

        internal virtual void drawObjectsNotInShadow()
        {
            float skyboxd = FormBase._FloatSkyBoxDim;

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, Trial.colorA);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, Trial.colorD);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, Trial.color0);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, Trial.color0);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, new float[] { 0 });

            Trial._Heightmap.GLDraw();

            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);

            if (Trial._IntTextureGrass == 0)
            {
                Trial._IntTextureGrass = Textures.getGLTexture(Properties.Resources.grass1);
            }
            else
            {
                GL.Color3(Color.White);
                GL.BindTexture(TextureTarget.Texture2D, Trial._IntTextureGrass);

                GL.Begin(BeginMode.Quads);
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
                GL.End();

                GL.BindTexture(TextureTarget.Texture2D, 0);
            }

            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Lighting);

            GL.Disable(EnableCap.CullFace);

            Trial.staticInitTrees();

            foreach (TreeObject to in Trial._Trees) to.GLDraw(ref this.ActualAngles.cab);

            GL.Enable(EnableCap.CullFace);
            
        }

        internal virtual void Deconstruct()
        {
        }

        internal virtual void GLDelete()
        {
            Trial.staticGLDelete();
        }
















        internal static int _IntTextureGrass = 0;
        internal static HeightMap _Heightmap = null;

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

        private class TreeObject
        {
            public CadObject co;
            public float angle;
            public float distance;

            public TreeObject(float a, float d, CadObject o) { this.angle = a; this.distance = d; this.co = o; }

            public void GLDraw(ref float cab)
            {
                float dif = Math.Abs(this.angle - cab);
                const float comp = 55;
                if (dif < comp || dif > (360 - comp))
                {
                    GL.PushMatrix();
                    {
                        GL.Rotate(angle, OpenTK.Vector3.UnitY);
                        GL.Translate(0, 0, -distance);
                        co.draw();
                    }
                    GL.PopMatrix();
                }
            }
        }

        private static TreeObject[] _Trees; 

        private static bool _BoolNeedsStaticInit = true;
        private static bool _BoolNeedsStaticInitTrees = true;

        private static void staticInit()
        {
            if (Trial._BoolNeedsStaticInit)
            {
                Trial._BoolNeedsStaticInit = false;

                Trial._Heightmap = new HeightMapRectangle(
                    Properties.Resources.HeightMap,
                    null,
                    10, // make wider
                    100 // make taller
                    ); // make smoother

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
                    ObjectName: "Tree 1",
                    xScale: 30.0f,
                    yScale: 30.0f,
                    zScale: 30.0f,
                    useAmbient: true,
                    useDiffuse: true,
                    useSpecular: true,
                    useEmission: true);

                Trial._CadObjectTree3 = CadObjectGenerator.fromXAML(
                    Properties.Resources.tree3,
                    ObjectName: "Tree 1",
                    xScale: 5.0f,
                    yScale: 5.0f,
                    zScale: 5.0f,
                    useAmbient: true,
                    useDiffuse: true,
                    useSpecular: true,
                    useEmission: true);

                Trial._CadObjectTree4 = CadObjectGenerator.fromXAML(
                    Properties.Resources.tree4,
                    ObjectName: "Tree 1",
                    xScale: 5.0f,
                    yScale: 5.0f,
                    zScale: 5.0f,
                    useAmbient: true,
                    useDiffuse: true,
                    useSpecular: true,
                    useEmission: true);

                Trial._CadObjectTree5 = CadObjectGenerator.fromXAML(
                    Properties.Resources.tree5,
                    ObjectName: "Tree 1",
                    xScale: 6.0f,
                    yScale: 6.0f,
                    zScale: 6.0f,
                    useAmbient: true,
                    useDiffuse: true,
                    useSpecular: true,
                    useEmission: true);

                Trial._CadObjectTree6 = CadObjectGenerator.fromXAML(
                    Properties.Resources.tree6,
                    ObjectName: "Tree 1",
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

        private static void staticGLDelete()
        {
            if (Trial._Heightmap != null) Trial._Heightmap.GLDelete();
            if (Trial._IntTextureGrass != 0) GL.DeleteTexture(Trial._IntTextureGrass);
            Trial._IntTextureGrass = 0;

            if (Trial._CadObjectTree1 != null) Trial._CadObjectTree1.GLDelete();
            if (Trial._CadObjectTree2 != null) Trial._CadObjectTree2.GLDelete();
            if (Trial._CadObjectTree3 != null) Trial._CadObjectTree3.GLDelete();
            if (Trial._CadObjectTree4 != null) Trial._CadObjectTree4.GLDelete();
            if (Trial._CadObjectTree5 != null) Trial._CadObjectTree5.GLDelete();
            if (Trial._CadObjectTree6 != null) Trial._CadObjectTree6.GLDelete();
        }








    }
}
