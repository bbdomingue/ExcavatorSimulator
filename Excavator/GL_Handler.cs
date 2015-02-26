using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.GlobalEvents;
using SamSeifert.GLE;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Excavator
{
    internal class GL_Handler
    {
        public SkyBox _SkyBox = null;

        private GLControl _GLControl;

        private bool _BoolFake3DLR = true;
        private bool _BoolVSync = false;
        private bool _Bool3D = false;
        private bool _BoolGlNotLoaded = true;
        internal bool _BoolShowSide = false;

        private const int DEPTH_BITS = 24;
        private const int STENCIL_BITS = 8;

        internal enum DrawType
        {
            Normal,

            Cat,
            CatSplitVertical,
            CatFlipFlop,
            CatLaces,

            Outside,
            ShadowBuffer,
            ShadowBufferGraphics,

            Rift
        }

        internal GL_Handler()
        {
        }

        internal void setControl(Control c)
        {
            this.createGLControl(c);
        }

        internal bool IsIdle
        {
            get
            {
                if (this._GLControl == null) return false;
                else return this._GLControl.IsIdle;
            }
        }

        private DrawType __DrawType = DrawType.Outside;
        internal DrawType _DrawType
        {
            set
            {
                this.__DrawType = value;

                this._BoolVSync = this.__DrawType == DrawType.CatFlipFlop;

                if (this._GLControl != null) this._GLControl.VSync = this._BoolVSync;

/*                if (this.__DrawType == DrawType.Stereo != this._Bool3D)
                {
                    this._Bool3D = !this._Bool3D;
                    this.createGLControl(null);
                }*/
            }
        }

        private void createGLControl(Control c)
        {
            this._BoolGlNotLoaded = true; 

            if (this._GLControl != null)
            {
                this.resetGL();

                if (this._GLControl.Parent != null) this._GLControl.Parent.Controls.Remove(this._GLControl);
                if (c == null) c = this._GLControl.Parent;

                this._GLControl.Dispose();

                this._GLControl = null;
            }

            if (c == null) Console.WriteLine("No parent control to add GLControl to");
            else
            {
                GraphicsMode g = GraphicsMode.Default;
                Console.WriteLine("Defualt Depth: " + g.Depth + ", Stencil: " + g.Stencil);

                g = new GraphicsMode(g.ColorFormat, DEPTH_BITS, STENCIL_BITS, g.Samples, g.ColorFormat, 2, this._Bool3D);
                
                this._GLControl = new GLControl(g);

                this._GLControl.Load += new EventHandler(this._GLControl_Load);
                this._GLControl.Resize += new EventHandler(this._GLControl_Resize);
                this._GLControl.Paint += new PaintEventHandler(this._Paint);
                this._GLControl.Dock = DockStyle.Fill;

                c.Controls.Add(this._GLControl);
            }
        }

        private void resetGL_BuffersOnly()
        {
            this.deleteFBO(ref this._ShadowIntFrameBuffer);
            this.deleteText(ref this._ShadowIntTexture);

            this.deleteFBO(ref this._GhostIntFBO);
            this.deleteText(ref this._GhostIntTextColor);
            this.deleteText(ref this._GhostIntTextDepth);

            this.deleteFBO(ref this._OrthoIntFBO);
            this.deleteText(ref this._OrthoIntTextColor);
            this.deleteText(ref this._OrthoIntTextDepth);

            this._BoolStencilNeedsSetup = true;
            this._BoolStencilNeedsSetupGhost = true;
            this._BoolOrthoNeedsInit = true;

            this._EnumShadowSetup = EnumGLSetup.not;
            this._EnumGhostSetup = EnumGLSetup.not;
        }

        private void deleteFBO(ref int fbo) { if (fbo != 0) GL.DeleteFramebuffers(1, ref fbo); fbo = 0; }
        private void deleteText(ref int text) { if (text != 0) GL.DeleteTexture(text); text = 0; }

        private void resetGL()
        {
            if (GL_Handler._ShadowIntProgram != 0) GL.DeleteProgram(GL_Handler._ShadowIntProgram);
            if (GL_Handler._ShadowIntShaderV != 0) GL.DeleteShader(GL_Handler._ShadowIntShaderV);
            if (GL_Handler._ShadowIntShaderF != 0) GL.DeleteShader(GL_Handler._ShadowIntShaderF);

            GL_Handler._ShadowIntProgram = 0;
            GL_Handler._ShadowIntShaderV = 0;
            GL_Handler._ShadowIntShaderF = 0;

            if (this._GhostIntProgram != 0) GL.DeleteProgram(this._GhostIntProgram);
            if (this._GhostIntShaderF != 0) GL.DeleteShader(this._GhostIntShaderF);
            if (this._GhostIntShaderV != 0) GL.DeleteShader(this._GhostIntShaderV);

            this._GhostIntProgram = 0;
            this._GhostIntShaderF = 0;
            this._GhostIntShaderV = 0;

            Bobcat.GLDelete();
            Trial.GLDelete_Static();
            EmbeddedSoilModel.GLDelete_Static();
            if (this._SkyBox != null) this._SkyBox.GLDelete();

            this.resetGL_BuffersOnly();
        }



















        private void _GLControl_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Created Stencil: " + this._GLControl.GraphicsMode.Stencil);

            this._BoolGlNotLoaded = false;

            this._GLControl.VSync = this._BoolVSync;

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.DepthTest);

            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            float diffuse = 1.0f;
            float specular = 1.0f;
            float ambient = 1.0f;

            this.setLightPos();
            GL.Light(LightName.Light0, LightParameter.Diffuse,
                new float[] { diffuse, diffuse, diffuse, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Ambient,
                new float[] { ambient, ambient, ambient, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Specular,
                new float[] { specular, specular, specular, 1.0f });
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Lighting);
        }

        internal void _GLControl_Resize(object sender, EventArgs e)
        {
            int sw = this._GLControl.Width;
            int sh = this._GLControl.Height;

            this._FloatAspectControl = sw;
            this._FloatAspectControl /= sh;

            this._RectangleViewportBasic = new Rectangle(0, 0, sw, sh);

            int ow = sw / 2;
            int oh = ow;
            int oh2 = oh / 2;
            int sh2 = sh / 2;
            this._RectangleOrtho = new Rectangle(0, sh - oh, ow, oh);
            this._RectangleOrthoS1 = new Rectangle(0, sh - oh2, ow, oh2);
            this._RectangleOrthoS2 = new Rectangle(0, sh2 - oh2, ow, oh2);

            if (this._BoolGlNotLoaded) return;

            this.resetGL_BuffersOnly();
        }

        internal void SkyBoxDimChanged()
        {
            if (this._SkyBox != null) this._SkyBox.resetDistance();
        }

        private float _FloatAspectControl;
        private Rectangle _RectangleViewportBasic;

        private Rectangle _RectangleViewportR = new Rectangle();
        private Rectangle _RectangleViewportL = new Rectangle();
        private Rectangle _RectangleViewportM = new Rectangle();

        private Rectangle _RectangleScissorR = new Rectangle();
        private Rectangle _RectangleScissorL = new Rectangle();

        private Matrix4 _MatrixProjectR;
        private Matrix4 _MatrixProjectL;
        private Matrix4 _MatrixProjectM;

        private Matrix4 _MatrixModelR;
        private Matrix4 _MatrixModelL;
        private Matrix4 _MatrixModelM;

        private float _FloatCabYaw = 0;
        private Bobcat.BobcatAngles ActualAngles = Bobcat.BobcatAngles.Zero;
        private Bobcat.BobcatAngles GhostAngles = Bobcat.BobcatAngles.Zero;
        private float _FloatDrawTime = 0;
        public static volatile bool DrawGhost = false;

        public float _FloatHeadYaw = 0;
        public float _FloatHeadPitch = 0;
        public float _FloatOutsideDistance = 350;

        private void updateMatrices()
        {
            FormBase.HeadLocationData dat = FormBase._HeadLocationCurrent;

            float rot = StaticMethods.toRadiansF(this._FloatCabYaw);
            float INCHES_2_PIXELS = this._RectangleViewportBasic.Width / Bobcat._TV_Width_Inches;

            switch (this.__DrawType)
            {
                case DrawType.Normal:
                    {
                        Vector3 eyeM_World = Bobcat.convertToRealWorldFromTV(dat.EyeM, rot);
                        Vector3 t = Vector3.Transform(new Vector3(0, 0, -1), Matrix4.CreateRotationY(rot));
                        this._MatrixModelM = Matrix4.LookAt(eyeM_World, Vector3.Add(eyeM_World, t), Vector3.UnitY);
                        this._RectangleViewportM = this._RectangleViewportBasic;

                        float fov = (float)Math.Atan2(Bobcat._TV_Height_Inches / 2, dat.EyeM.Z);
                        fov *= 2;

                        this._MatrixProjectM = Matrix4.CreatePerspectiveFieldOfView(
                            fov,
                            this._FloatAspectControl,
                            0.1f,
                            FormBase._FloatSkyBoxDim * 2f);
                        
                        break;
                    }
                case DrawType.Cat:
                    {
                        Vector3 eyeM_World = Bobcat.convertToRealWorldFromTV(dat.EyeM, rot);
                        Vector3 t = Vector3.Transform(new Vector3(0, 0, -1), Matrix4.CreateRotationY(rot));
                        this._MatrixModelM = Matrix4.LookAt(eyeM_World, Vector3.Add(eyeM_World, t), Vector3.UnitY);

                        float w = Bobcat._TV_Width_Inches + 2 * Math.Abs(dat.EyeM.X);
                        float h = Bobcat._TV_Height_Inches + 2 * Math.Abs(dat.EyeM.Y);

                        float aspect = w;
                        aspect /= h;

                        float x = (dat.EyeM.X < 0) ? Bobcat._TV_Width_Inches - w : 0;
                        float y = (dat.EyeM.Y < 0) ? Bobcat._TV_Height_Inches - h : 0;

                        float fov = (float)Math.Atan2(h / 2, dat.EyeM.Z);
                        fov *= 2;

                        this._RectangleViewportM = new Rectangle(
                            (int)Math.Round(INCHES_2_PIXELS * x),
                            (int)Math.Round(INCHES_2_PIXELS * y),
                            (int)Math.Round(INCHES_2_PIXELS * w),
                            (int)Math.Round(INCHES_2_PIXELS * h));

                        this._MatrixProjectM = Matrix4.CreatePerspectiveFieldOfView(
                            fov,
                            aspect,
                            0.1f,
                            FormBase._FloatSkyBoxDim * 2f);

                        break;
                    }
                case DrawType.CatSplitVertical:
                    {
                        float x, y, w, h, aspect, fov;

                        Vector3 t = Vector3.Transform(new Vector3(0, 0, -1), Matrix4.CreateRotationY(rot));

                        int h2 = (int) Math.Round(this._RectangleViewportBasic.Height / 2.0f);
                        this._RectangleScissorL = new Rectangle(0, 0, this._RectangleViewportBasic.Width, h2);
                        this._RectangleScissorR = new Rectangle(0, h2, this._RectangleViewportBasic.Width, h2);

                        Vector3 eyeL_World = Bobcat.convertToRealWorldFromTV(dat.EyeL, rot);
                        this._MatrixModelL = Matrix4.LookAt(eyeL_World, Vector3.Add(eyeL_World, t), Vector3.UnitY);

                        w = Bobcat._TV_Width_Inches + 2 * Math.Abs(dat.EyeL.X);
                        h = Bobcat._TV_Height_Inches + 2 * Math.Abs(dat.EyeL.Y);
                        x = (dat.EyeL.X < 0) ? Bobcat._TV_Width_Inches - w : 0;
                        y = (dat.EyeL.Y < 0) ? Bobcat._TV_Height_Inches - h : 0;
                        aspect = w; aspect /= h;
                        fov = 2 * (float)Math.Atan2(h / 2, dat.EyeL.Z);

                        this._RectangleViewportL = new Rectangle(
                            (int)Math.Round(INCHES_2_PIXELS * x),
                            (int)Math.Round(INCHES_2_PIXELS * y / 2),
                            (int)Math.Round(INCHES_2_PIXELS * w),
                            (int)Math.Round(INCHES_2_PIXELS * h / 2));

                        this._MatrixProjectL = Matrix4.CreatePerspectiveFieldOfView(
                            fov,
                            aspect,
                            0.1f,
                            FormBase._FloatSkyBoxDim * 2f);

                        Vector3 eyeR_World = Bobcat.convertToRealWorldFromTV(dat.EyeR, rot);
                        this._MatrixModelR = Matrix4.LookAt(eyeR_World, Vector3.Add(eyeR_World, t), Vector3.UnitY);

                        w = Bobcat._TV_Width_Inches + 2 * Math.Abs(dat.EyeR.X);
                        h = Bobcat._TV_Height_Inches + 2 * Math.Abs(dat.EyeR.Y);
                        x = (dat.EyeR.X < 0) ? Bobcat._TV_Width_Inches - w : 0;
                        y = (dat.EyeR.Y < 0) ? Bobcat._TV_Height_Inches - h : 0;
                        aspect = w; aspect /= h;
                        fov = 2 * (float)Math.Atan2(h / 2, dat.EyeR.Z);

                        this._RectangleViewportR = new Rectangle(
                            (int)Math.Round(INCHES_2_PIXELS * x),
                            (int)Math.Round(INCHES_2_PIXELS * y / 2) + h2,
                            (int)Math.Round(INCHES_2_PIXELS * w),
                            (int)Math.Round(INCHES_2_PIXELS * h / 2));

                        this._MatrixProjectR = Matrix4.CreatePerspectiveFieldOfView(
                            fov,
                            aspect,
                            0.1f,
                            FormBase._FloatSkyBoxDim * 2f);

                        break;
                    }
                case DrawType.CatFlipFlop:
                case DrawType.CatLaces:
                    {
                        float x, y, w, h, aspect, fov;

                        Vector3 t = Vector3.Transform(new Vector3(0, 0, -1), Matrix4.CreateRotationY(rot));

                        Vector3 eyeL_World = Bobcat.convertToRealWorldFromTV(dat.EyeL, rot);
                        this._MatrixModelL = Matrix4.LookAt(eyeL_World, Vector3.Add(eyeL_World, t), Vector3.UnitY);

                        w = Bobcat._TV_Width_Inches + 2 * Math.Abs(dat.EyeL.X);
                        h = Bobcat._TV_Height_Inches + 2 * Math.Abs(dat.EyeL.Y);
                        x = (dat.EyeL.X < 0) ? Bobcat._TV_Width_Inches - w : 0;
                        y = (dat.EyeL.Y < 0) ? Bobcat._TV_Height_Inches - h : 0;
                        aspect = w; aspect /= h;
                        fov = 2 * (float)Math.Atan2(h / 2, dat.EyeL.Z);

                        this._RectangleViewportL = new Rectangle(
                            (int)Math.Round(INCHES_2_PIXELS * x),
                            (int)Math.Round(INCHES_2_PIXELS * y),
                            (int)Math.Round(INCHES_2_PIXELS * w),
                            (int)Math.Round(INCHES_2_PIXELS * h));

                        this._MatrixProjectL = Matrix4.CreatePerspectiveFieldOfView(
                            fov,
                            aspect,
                            0.1f,
                            FormBase._FloatSkyBoxDim * 2f);

                        Vector3 eyeR_World = Bobcat.convertToRealWorldFromTV(dat.EyeR, rot);
                        this._MatrixModelR = Matrix4.LookAt(eyeR_World, Vector3.Add(eyeR_World, t), Vector3.UnitY);

                        w = Bobcat._TV_Width_Inches + 2 * Math.Abs(dat.EyeR.X);
                        h = Bobcat._TV_Height_Inches + 2 * Math.Abs(dat.EyeR.Y);
                        x = (dat.EyeR.X < 0) ? Bobcat._TV_Width_Inches - w : 0;
                        y = (dat.EyeR.Y < 0) ? Bobcat._TV_Height_Inches - h : 0;
                        aspect = w; aspect /= h;
                        fov = 2 * (float)Math.Atan2(h / 2, dat.EyeR.Z);

                        this._RectangleViewportR = new Rectangle(
                            (int)Math.Round(INCHES_2_PIXELS * x),
                            (int)Math.Round(INCHES_2_PIXELS * y),
                            (int)Math.Round(INCHES_2_PIXELS * w),
                            (int)Math.Round(INCHES_2_PIXELS * h));

                        this._MatrixProjectR = Matrix4.CreatePerspectiveFieldOfView(
                            fov,
                            aspect,
                            0.1f,
                            FormBase._FloatSkyBoxDim * 2f); 
                        
                        break;
                    }
                case DrawType.Outside:
                    {
                        Vector4 eP = new Vector4(0, 0, this._FloatOutsideDistance, 1);
                        eP = Vector4.Transform(eP, Matrix4.CreateRotationX(StaticMethods.toRadiansF(this._FloatHeadPitch)));
                        eP = Vector4.Transform(eP, Matrix4.CreateRotationY(StaticMethods.toRadiansF(this._FloatCabYaw + this._FloatHeadYaw)));
                        this._RectangleViewportM = this._RectangleViewportBasic;
                        this._MatrixModelM = Matrix4.LookAt(new Vector3(eP), new Vector3(0, 0, 0), Vector3.UnitY);
                        this._MatrixProjectM = Matrix4.CreatePerspectiveFieldOfView(
                            StaticMethods.toRadiansF(55),
                            this._FloatAspectControl, 
                            0.1f, 
                            FormBase._FloatSkyBoxDim * 2);
                        break;
                    }
                case DrawType.ShadowBuffer:
                    {
                        this._RectangleViewportM = this._RectangleViewportBasic;
                        if (this._RectangleViewportM.Width > this._RectangleViewportM.Height)
                        {
                            this._RectangleViewportM.X = (this._RectangleViewportM.Width - this._RectangleViewportM.Height) / 2;
                            this._RectangleViewportM.Width = this._RectangleViewportM.Height;
                        }
                        else if (this._RectangleViewportM.Height > this._RectangleViewportM.Width)
                        {
                            this._RectangleViewportM.Y = (this._RectangleViewportM.Height - this._RectangleViewportM.Width) / 2;
                            this._RectangleViewportM.Height = this._RectangleViewportM.Width;
                        } 
                        
                        this._MatrixProjectM = Matrix4.CreateOrthographic(1.0f, 1.0f, -1.0f, 1.0f);
                        break;
                    }
                case DrawType.ShadowBufferGraphics:
                    {
                        this._RectangleViewportM = this._RectangleViewportBasic;
                        if (this._RectangleViewportM.Width > this._RectangleViewportM.Height)
                        {
                            this._RectangleViewportM.X = (this._RectangleViewportM.Width - this._RectangleViewportM.Height) / 2;
                            this._RectangleViewportM.Width = this._RectangleViewportM.Height;
                        }
                        else if (this._RectangleViewportM.Height > this._RectangleViewportM.Width)
                        {
                            this._RectangleViewportM.Y = (this._RectangleViewportM.Height - this._RectangleViewportM.Width) / 2;
                            this._RectangleViewportM.Height = this._RectangleViewportM.Width;
                        }

                        this._MatrixProjectM = this._ShadowMatrix4MapPerspective;
                        this._MatrixModelM = this._ShadowMatrix4MapModelView;
                        break;
                    }
                case DrawType.Rift:
                    {
                        break;
                    }
            }
/*                case DrawType.Cat:
                    {
                        Matrix4 rotY = Matrix4.CreateRotationY(StaticMethods.toRadiansF(this._FloatCabYaw));

                        Vector4 eD = Vector4.Transform(new Vector4(0, 0, -1, 1), rotY);

                        eP = Vector4.Transform(this._Vector3EyePositionInCab, rotY);

                        eP.Y += Bobcat._SamCatLiftHeight;

                        fPM = new Vector3(Vector4.Add(this._Vector3EyeDirection, eP));
                        eM = new Vector3(eP);

                        this._Matrix4EyeM = Matrix4.LookAt(eM, fPM, Vector3.UnitY);

                        break;
                    }
                case DrawType.CatStereo:
                    {
                        Matrix4 rotY = Matrix4.CreateRotationY(StaticMethods.toRadiansF(this._FloatCabYaw));

                        this._Vector3EyeDirection = new Vector4(0, 0, -1, 1);
                        this._Vector3EyeDirection = Vector4.Transform(this._Vector3EyeDirection,
                            Matrix4.CreateRotationX(StaticMethods.toRadiansF(Bobcat._TV_Float_Rotate)));
                        this._Vector3EyeDirection = Vector4.Transform(this._Vector3EyeDirection, rotY);

                        eP = Vector4.Transform(
                            Vector4.Add(this._Vector3EyePositionInCab, new Vector4(this._ViewEyeDistanceHalf, 0, 0, 1)),
                            rotY);
                        eR = new Vector3(eP);
                        eR.Y += Bobcat._SamCatLiftHeight;
                        fPR = (Vector3.Add(new Vector3(this._Vector3EyeDirection), eR));
                        this._Matrix4EyeR = Matrix4.LookAt(eR, fPR, Vector3.UnitY);

                        eP = Vector4.Transform(
                            Vector4.Add(this._Vector3EyePositionInCab, new Vector4(-this._ViewEyeDistanceHalf, 0, 0, 1)),
                            rotY);
                        eL = new Vector3(eP);
                        eL.Y += Bobcat._SamCatLiftHeight;
                        fPL = (Vector3.Add(new Vector3(this._Vector3EyeDirection), eL));
                        this._Matrix4EyeL = Matrix4.LookAt(eL, fPL, Vector3.UnitY);

                        break;
                    }
                default:
                    {
                        this._Vector3EyeDirection = new Vector4(0, 0, -1, 1);
                        this._Vector3EyeDirection = Vector4.Transform(this._Vector3EyeDirection,
                            Matrix4.CreateRotationX(StaticMethods.toRadiansF(this._FloatHeadPitch)));
                        this._Vector3EyeDirection = Vector4.Transform(this._Vector3EyeDirection,
                            Matrix4.CreateRotationY(StaticMethods.toRadiansF(this._FloatCabYaw + this._FloatHeadYaw)));

                        eP = Vector4.Transform(this._Vector3EyePositionInCab,
                            Matrix4.CreateRotationY(StaticMethods.toRadiansF(this._FloatCabYaw)));

                        eP.Y += Bobcat._SamCatLiftHeight;

                        fPM = new Vector3(Vector4.Add(eP,
                            Vector4.Multiply(this._Vector3EyeDirection, this._ViewFocalDistance)));

                        eR = Vector3.Add(
                            new Vector3(eP),
                            Vector3.Cross(
                                new Vector3(this._Vector3EyeDirection),
                                new Vector3(0, this._ViewEyeDistanceHalf, 0)));

                        eL = Vector3.Add(
                            new Vector3(eP),
                            Vector3.Cross(
                                new Vector3(this._Vector3EyeDirection),
                                new Vector3(0, -this._ViewEyeDistanceHalf, 0)));

                        eM = new Vector3(eP);

                        this._Matrix4EyeL = Matrix4.LookAt(eL, fPM, Vector3.UnitY);
                        this._Matrix4EyeR = Matrix4.LookAt(eR, fPM, Vector3.UnitY);
                        this._Matrix4EyeM = Matrix4.LookAt(eM, fPM, Vector3.UnitY);

                        break;
                    }*/
        }






















        internal void Invalidate() { this._GLControl.Invalidate(); }

        private void _Paint(object sender, PaintEventArgs e)
        {
            if (this._BoolGlNotLoaded) return;

            this._FloatCabYaw = Bobcat.ActualAngles.cab;
            this.ActualAngles = Bobcat.ActualAngles;
            this.GhostAngles = Bobcat.GhostAngles;
            this._FloatDrawTime = Trial.getTime();

            this.updateMatrices();

            this.drawShadowAndAll();

            this._GLControl.SwapBuffers();
        }

        private enum DrawStage { Ghost, NormalInsideShadow, NormalOutsideShadow };
        private void drawModelPreType(DrawStage s)
        {
            switch (this.__DrawType)
            {
                case DrawType.CatLaces:
                case DrawType.CatFlipFlop:
                    {
                        if (this._BoolFake3DLR)
                        {
                            GL.Viewport(this._RectangleViewportL);
                            GL.MatrixMode(MatrixMode.Projection);
                            GL.LoadMatrix(ref this._MatrixProjectL);
                            GL.MatrixMode(MatrixMode.Modelview);
                            GL.LoadMatrix(ref this._MatrixModelL);
                        }
                        else
                        {
                            GL.Viewport(this._RectangleViewportR);
                            GL.MatrixMode(MatrixMode.Projection);
                            GL.LoadMatrix(ref this._MatrixProjectR);
                            GL.MatrixMode(MatrixMode.Modelview);
                            GL.LoadMatrix(ref this._MatrixModelR);
                        }
                        this.drawModelPostType(s);
                        break;
                    }
                case DrawType.ShadowBuffer:
                    {
                        if (s != DrawStage.NormalOutsideShadow) break;

                        GL.Viewport(this._RectangleViewportM);
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.LoadIdentity();
                        GL.Ortho(0, 1, 0, 1, -1, 1);

                        GL.MatrixMode(MatrixMode.Texture);
                        GL.LoadIdentity();
                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.LoadIdentity();
                        GL.Color3(Color.White);

                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, this._ShadowIntTexture);

                        GL.Disable(EnableCap.Lighting);
                        GL.Disable(EnableCap.CullFace);
                        GL.Disable(EnableCap.DepthTest);
                        GL.Enable(EnableCap.Texture2D);
                        {
                            GL.Begin(BeginMode.Quads);
                            {
                                float mn = 0.0f;
                                float mx = 1.0f;
                                GL.TexCoord2(0, 0); GL.Vertex3(mn, mn, 0);
                                GL.TexCoord2(1, 0); GL.Vertex3(mx, mn, 0);
                                GL.TexCoord2(1, 1); GL.Vertex3(mx, mx, 0);
                                GL.TexCoord2(0, 1); GL.Vertex3(mn, mx, 0);
                            }
                            GL.End();
                        }
                        GL.Disable(EnableCap.Texture2D);
                        GL.Enable(EnableCap.DepthTest);
                        GL.Enable(EnableCap.CullFace);
                        GL.Enable(EnableCap.Lighting);

                        GL.BindTexture(TextureTarget.Texture2D, 0);

                        break;
                    }
                case DrawType.ShadowBufferGraphics:
                    {
                        if (s != DrawStage.NormalOutsideShadow) break;
                        GL.Viewport(this._RectangleViewportM);
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.LoadMatrix(ref this._MatrixProjectM);
                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.LoadMatrix(ref this._MatrixModelM);
                        this.drawModelPostType(DrawStage.NormalInsideShadow);
                        break;
                    }
                default: // Outside, Cat, Normals
                    {
                        GL.Viewport(this._RectangleViewportM);
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.LoadMatrix(ref this._MatrixProjectM);
                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.LoadMatrix(ref this._MatrixModelM);
                        this.drawModelPostType(s);
                        break;
                    }
                case DrawType.Rift:
                case DrawType.CatSplitVertical:
                    {
                        int w = this._GLControl.Width;
                        int h = this._GLControl.Height;

                        GL.Enable(EnableCap.ScissorTest);
                        {
                            GL.Viewport(this._RectangleViewportL);
                            GL.Scissor(this._RectangleScissorL);
                            GL.MatrixMode(MatrixMode.Projection); GL.LoadMatrix(ref this._MatrixProjectL);
                            GL.MatrixMode(MatrixMode.Modelview);  GL.LoadMatrix(ref this._MatrixModelL);
                            this.drawModelPostType(s);

                            GL.Viewport(this._RectangleViewportR);
                            GL.Scissor(this._RectangleScissorR);
                            GL.MatrixMode(MatrixMode.Projection); GL.LoadMatrix(ref this._MatrixProjectR);
                            GL.MatrixMode(MatrixMode.Modelview);  GL.LoadMatrix(ref this._MatrixModelR);
                            this.drawModelPostType(s);
                        }
                        GL.Disable(EnableCap.ScissorTest);

                        break;
                    }
            }
        }

        private void drawModelPostType(DrawStage s)
        {
            GL.Light(LightName.Light0, LightParameter.Position, this._FloatsLightPos);

            bool drawCab = !FormBase.Instance._BoolFullScreen || (
                this.__DrawType == DrawType.Outside ||
                this.__DrawType == DrawType.ShadowBufferGraphics);

            switch (s)
            {
                case DrawStage.Ghost:
                    {
                        Bobcat.Draw(this.GhostAngles, false, false);
                        break;
                    }
                case DrawStage.NormalInsideShadow:
                    {
                        GL_Handler._BoolMatrixModeModelTexture = true;
                        GL_Handler._BoolTextureAlso = true;
                        Trial.drawObjectsInShadow(false, this._FloatDrawTime);
                        Bobcat.Draw(this.ActualAngles, drawCab, false);                        
                        GL_Handler._BoolTextureAlso = false;
                        break;
                    }
                case DrawStage.NormalOutsideShadow:
                    {
                        Trial.drawObjectsNotInShadow(this.ActualAngles);

                        if (this._SkyBox != null)
                        {
                            GL.Disable(EnableCap.Lighting);
                            this._SkyBox.Draw(FormBase._FloatSkyBoxDim);
                            GL.Enable(EnableCap.Lighting);
                        }
                        break;
                    }
            }
        }
















        ///////////////////////////////////////////
        // Lighting
        ///////////////////////////////////////////

        private readonly float[] _FloatsLightPos = new float[] { 8000, 15000, 4000, 0 };
        private void setLightPos()
        {
            Vector3 lightPos = new Vector3(this._FloatsLightPos[0], this._FloatsLightPos[1], this._FloatsLightPos[2]);

            lightPos.Normalize();
            lightPos = lightPos * GL_Handler._IntLightRenderDistance;

            this._ShadowMatrix4MapModelView = Matrix4.LookAt(
                new Vector3(lightPos),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0));
        }

        ///////////////////////////////////////////
        // Ortho View
        ///////////////////////////////////////////

        private int _OrthoIntFBO;
        private int _OrthoIntTextColor;
        private int _OrthoIntTextDepth;

        private Rectangle _RectangleOrtho = new Rectangle(0, 0, 100, 100);
        private Rectangle _RectangleOrthoS1 = new Rectangle(0, 0, 100, 100);
        private Rectangle _RectangleOrthoS2 = new Rectangle(0, 0, 100, 100);

        private bool _BoolOrthoNeedsInit = true;

        private void initOrtho()
        {
            // Create Color Texture
            GL.GenTextures(1, out this._OrthoIntTextColor);
            GL.BindTexture(TextureTarget.Texture2D, this._OrthoIntTextColor);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexImage2D(
                TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba8,
                this._RectangleOrtho.Width,
                this._RectangleOrtho.Height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.GenTextures(1, out this._OrthoIntTextDepth);
            GL.BindTexture(TextureTarget.Texture2D, this._OrthoIntTextDepth);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Depth24Stencil8,
                this._RectangleOrtho.Width,
                this._RectangleOrtho.Height,
                0,
                (PixelFormat)All.DepthStencilExt,
                PixelType.UnsignedInt248,
                IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.GenFramebuffers(1, out this._OrthoIntFBO);
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, this._OrthoIntFBO);
            GL.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt,
                FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D,
                this._OrthoIntTextColor,
                0);
            GL.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt,
                FramebufferAttachment.DepthStencilAttachment,
                TextureTarget.Texture2D,
                this._OrthoIntTextDepth,
                0);
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }

        private void drawOrthoBuffer()
        {
            if (this._BoolOrthoNeedsInit)
            {
                this.initOrtho();
                this._BoolOrthoNeedsInit = false;
            }
            else
            {
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, this._OrthoIntFBO);
                GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0Ext);

                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Back);

                GL.ClearColor(Color.White);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                int border = 2;
                GL.Viewport(
                    border,
                    border,
                    this._RectangleOrtho.Width - 2 * border,
                    this._RectangleOrtho.Height - 2 * border);

                GL.Enable(EnableCap.ScissorTest);
                GL.Scissor(
                    border,
                    border,
                    this._RectangleOrtho.Width - 2 * border,
                    this._RectangleOrtho.Height - 2 * border);
                GL.ClearColor(Color.FromArgb(101, 139, 186));
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Disable(EnableCap.ScissorTest);


                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(
                    -50,
                    300,
                    -175,
                    175,
                    -300,
                    300);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                GL.Rotate(- 90 - this.ActualAngles.cab, Vector3.UnitY);

                GL.Disable(EnableCap.Lighting);
                EmbeddedSoilModel.drawTrenchOrtho();

                GL.Enable(EnableCap.Lighting);
                GL.Light(LightName.Light0, LightParameter.Position, this._FloatsLightPos);

                Bobcat.Draw(this.ActualAngles, true, true);

                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer
                GL.DrawBuffer(DrawBufferMode.Back);
            }
        }

        private void drawOrthoPre()
        {
            GL.Disable(EnableCap.StencilTest);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            GL.Color3(Color.White);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 1, 0, 1, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.BindTexture(TextureTarget.Texture2D, this._OrthoIntTextColor);
        }

        private void drawOrtho(Rectangle r)
        {
            GL.Viewport(r);

            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
            GL.TexCoord2(1, 0); GL.Vertex2(1, 0);
            GL.TexCoord2(1, 1); GL.Vertex2(1, 1);
            GL.TexCoord2(0, 1); GL.Vertex2(0, 1);
            GL.End();
        }

        private void drawOrthoPost()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Lighting);
        }




        ///////////////////////////////////////////
        // Ghost
        ///////////////////////////////////////////
        
        private int _GhostIntProgram = 0;
        private int _GhostIntShaderV = 0;
        private int _GhostIntShaderF = 0;
        private int _GhostIntFBO;
        private int _GhostIntTextColor;
        private int _GhostIntTextDepth;

        private EnumGLSetup _EnumGhostSetup = EnumGLSetup.not;

        private bool initGhost()
        {
            // Create Color Texture
            GL.GenTextures(1, out this._GhostIntTextColor);
            GL.BindTexture(TextureTarget.Texture2D, this._GhostIntTextColor);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexImage2D(
                TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba8,
                this._RectangleViewportBasic.Width,
                this._RectangleViewportBasic.Height, 
                0, 
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.GenTextures(1, out this._GhostIntTextDepth);
            GL.BindTexture(TextureTarget.Texture2D, this._GhostIntTextDepth);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexImage2D(
                TextureTarget.Texture2D, 
                0, 
                PixelInternalFormat.Depth24Stencil8,
                this._RectangleViewportBasic.Width,
                this._RectangleViewportBasic.Height,
                0,
                (PixelFormat) All.DepthStencilExt,
//                PixelFormat.DepthStencil, 
                PixelType.UnsignedInt248, 
                IntPtr.Zero);

            GL.BindTexture(TextureTarget.Texture2D, 0);


            GL.GenFramebuffers(1, out _GhostIntFBO);
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, _GhostIntFBO);
            GL.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt,
                FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D,
                this._GhostIntTextColor, 
                0);
            GL.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt, 
                FramebufferAttachment.DepthStencilAttachment, 
                TextureTarget.Texture2D,
                this._GhostIntTextDepth, 
                0);

            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);

            return true;
        }

        void drawGhost(bool copybuffer, bool clear)
        {
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, _GhostIntFBO);
            GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0Ext);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            if (this._BoolStencilNeedsSetupGhost)
            {
                this.initStencilBuffer();
                this._BoolStencilNeedsSetupGhost = false;
            }

            if (clear) GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            this.drawModelPreType(DrawStage.Ghost);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.UseProgram(0);

            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer
            GL.DrawBuffer(DrawBufferMode.Back);

            if (copybuffer)
            {
                GL_Handler._BoolTextureAlso = false;

                GL.Disable(EnableCap.StencilTest);
                GL.Disable(EnableCap.CullFace);
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);

                GL.Color3(Color.White);

                GL.Viewport(this._RectangleViewportBasic);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, 1, 0, 1, 0, 1);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                GL.UseProgram(this._GhostIntProgram);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, this._GhostIntTextColor);
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, this._GhostIntTextDepth);

                GL.Begin(BeginMode.Quads);
                GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
                GL.TexCoord2(1, 0); GL.Vertex2(1, 0);
                GL.TexCoord2(1, 1); GL.Vertex2(1, 1);
                GL.TexCoord2(0, 1); GL.Vertex2(0, 1);
                GL.End();

                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                GL.UseProgram(0);

                GL.Disable(EnableCap.Blend);
                GL.Disable(EnableCap.Texture2D);
                GL.Enable(EnableCap.CullFace);
            }
        }

        ///////////////////////////////////////////
        // Shadow
        ///////////////////////////////////////////

        private int _ShadowIntFrameBuffer = 0;
        public static int _ShadowIntProgram {get; private set;}
        public static int _ShadowIntShaderV { get; private set; }
        public static int _ShadowIntShaderF { get; private set; }
        private int _ShadowIntTexture = 0;

        const int SHADOW_DIM = 1024*4;
        const int SHADOW_MAP_WIDTH = SHADOW_DIM;
        const int SHADOW_MAP_HEIGHT = SHADOW_DIM;
        const float SHADOW_MAP_ASPECT = ((float)SHADOW_MAP_WIDTH) / ((float)SHADOW_MAP_HEIGHT);
        const float SHADOW_MAP_FOV = (float)(22.0 * Math.PI / 180.0);

        private EnumGLSetup _EnumShadowSetup = EnumGLSetup.not;
        private enum EnumGLSetup { not, tried, good }

        private Matrix4 _ShadowMatrix4MapPerspective;
        private Matrix4 _ShadowMatrix4MapModelView;

        //Light position
        const int _IntLightRenderDistance = 1500;

        private bool initShadow()
        {
            // Try to use a texture depth component
            GL.GenTextures(1, out this._ShadowIntTexture);
            GL.BindTexture(TextureTarget.Texture2D, this._ShadowIntTexture);

            // GL_LINEAR does not make sense for depth texture. However, next tutorial shows usage of GL_LINEAR and PCF
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            // Remove artefact on the edges of the shadowmap
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            //glTexParameterfv( GL_TEXTURE_2D, GL_TEXTURE_BORDER_COLOR, borderColor );

            // No need to force GL_DEPTH_COMPONENT24, drivers usually give you the max precision if available 
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.DepthComponent,
                SHADOW_MAP_WIDTH,
                SHADOW_MAP_HEIGHT,
                0,
                PixelFormat.DepthComponent,
                PixelType.UnsignedByte,
                (IntPtr)0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // create a framebuffer object
            GL.GenFramebuffers(1, out this._ShadowIntFrameBuffer);
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, this._ShadowIntFrameBuffer);

            // Instruct openGL that we won't bind a color texture with the currently binded FBO
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer((int)DrawBufferMode.None);

            // attach the texture to FBO depth attachment point
            GL.FramebufferTexture2D(
                FramebufferTarget.FramebufferExt,
                FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D,
                this._ShadowIntTexture,
                0);

            // check FBO status            
            if (GL.CheckFramebufferStatus(FramebufferTarget.FramebufferExt) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("GL_FRAMEBUFFER_COMPLETE_EXT failed, CANNOT use FBO\n");
                return false;
            }

            // This is important, if not here, FBO's depthbuffer won't be populated.
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            // switch back to window-system-provided framebuffer
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);

            return true;
        }














        internal static void Rotate(float val, Vector3 v)
        {
            GL.Rotate(val, v);
            if (GL_Handler._BoolTextureAlso)
            {
                GL_Handler.FlipFlopMatrices();
                GL.Rotate(val, v);
            }
        }

        internal static void Rotate(float a, float x, float y, float z)
        {
            GL.Rotate(a, x, y, z);
            if (GL_Handler._BoolTextureAlso)
            {
                GL_Handler.FlipFlopMatrices();
                GL.Rotate(a, x, y, z);
            }
        }

        internal static void MultiplyMatrix(float[] arg)
        {
            GL.MultMatrix(arg);
            if (GL_Handler._BoolTextureAlso)
            {
                GL_Handler.FlipFlopMatrices();
                GL.MultMatrix(arg);
            }
        }

        internal static void Translate(float x, float y, float z)
        {
            GL.Translate(x, y, z);
            if (GL_Handler._BoolTextureAlso)
            {
                GL_Handler.FlipFlopMatrices();
                GL.Translate(x, y, z);
            }
        }

        internal static void Translate(Vector3 v)
        {
            GL.Translate(v);
            if (GL_Handler._BoolTextureAlso)
            {
                GL_Handler.FlipFlopMatrices();
                GL.Translate(v);
            }
        }

        internal static void PushMatrix()
        {
            GL.PushMatrix();
            if (GL_Handler._BoolTextureAlso)
            {
                GL_Handler.FlipFlopMatrices();
                GL.PushMatrix();
            }
        }

        internal static void PopMatrix()
        {
            GL.PopMatrix();
            if (GL_Handler._BoolTextureAlso)
            {
                GL_Handler.FlipFlopMatrices();
                GL.PopMatrix();
            }
        }

        private static void FlipFlopMatrices()
        {
            GL.MatrixMode(GL_Handler._BoolMatrixModeModelTexture ? MatrixMode.Texture : MatrixMode.Modelview);
            GL_Handler._BoolMatrixModeModelTexture = !GL_Handler._BoolMatrixModeModelTexture;
        }

        private static bool _BoolTextureAlso = false;
        private static bool _BoolMatrixModeModelTexture = false;



















        private float[] drawStart_mv = new float[16];
        private float[] drawStart_pv = new float[16];

        private Matrix4 drawStart_bias = new Matrix4(
            0.5f, 0.0f, 0.0f, 0.0f,
            0.0f, 0.5f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.5f, 0.0f,
            0.5f, 0.5f, 0.5f, 1.0f
        );

        private int zzzzzzzzzzzzz = 0;
        private void drawShadowAndAll()
        {
            if (this.__DrawType == DrawType.CatLaces) this._BoolFake3DLR = true;
            else if (zzzzzzzzzzzzz != 3) zzzzzzzzzzzzz++;
            else
            {
                zzzzzzzzzzzzz = 0;
                this._BoolFake3DLR = !this._BoolFake3DLR;
            }

            if (this._BoolStencilNeedsSetup)
            {
                this.initStencilBuffer();
                this._BoolStencilNeedsSetup = false;
            }

            if (this._BoolShowSide)
            {
                this.drawOrthoBuffer();
            }

            if (this._EnumGhostSetup == EnumGLSetup.not)
            {
                this._EnumGhostSetup = EnumGLSetup.tried;
                if (this.initGhost())
                {
                    bool a =
                        this._GhostIntProgram != 0 &&
                        this._GhostIntShaderV != 0 &&
                        this._GhostIntShaderF != 0;

                    if (!a) a = Shaders.CreateShaders(
                        Properties.Resources.GhostVertex,
                        Properties.Resources.GhostFragment,
                        out this._GhostIntProgram,
                        out this._GhostIntShaderV,
                        out this._GhostIntShaderF);

                    if (a)
                    {
                        this._EnumGhostSetup = EnumGLSetup.good;

                        GL.UseProgram(this._GhostIntProgram);
                        Textures.BindTexture(this._GhostIntProgram, TextureUnit.Texture0, "tex0");
                        Textures.BindTexture(this._GhostIntProgram, TextureUnit.Texture1, "tex1");
                        GL.UseProgram(0);
                    }
                }
            }

            if (this._EnumShadowSetup == EnumGLSetup.not)
            {
                this._EnumShadowSetup = EnumGLSetup.tried;

                if (this.initShadow())
                {
                    bool a =
                        GL_Handler._ShadowIntProgram != 0 &&
                        GL_Handler._ShadowIntShaderV != 0 &&
                        GL_Handler._ShadowIntShaderF != 0;


                    if (!a)
                    {
                        int sp, sv, sf;
                        a = Shaders.CreateShaders(
                           Properties.Resources.ShadowVertex,
                           Properties.Resources.ShadowFragment,
                           out sp,
                           out sv,
                           out sf);

                        GL_Handler._ShadowIntProgram = sp;
                        GL_Handler._ShadowIntShaderV = sv;
                        GL_Handler._ShadowIntShaderF = sf;
                    }

                    if (a)
                    {
                        this._EnumShadowSetup = EnumGLSetup.good;
                        this._ShadowMatrix4MapPerspective = Matrix4.CreatePerspectiveFieldOfView(
                            SHADOW_MAP_FOV,
                            SHADOW_MAP_ASPECT,
                            100, 40000);

                        GL.UseProgram(GL_Handler._ShadowIntProgram);
                        Textures.BindTexture(GL_Handler._ShadowIntProgram, TextureUnit.Texture0, "tex0");
                        Textures.BindTexture(GL_Handler._ShadowIntProgram, TextureUnit.Texture7, "ShadowMap");
                        GL.UseProgram(0);
                    }
                }

                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            }

            if (this._EnumShadowSetup == EnumGLSetup.good && this._EnumGhostSetup == EnumGLSetup.good)
            {
                GL.ClearColor(Color.Black);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.ActiveTexture(TextureUnit.Texture7);
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, this._ShadowIntFrameBuffer);
                GL.ColorMask(false, false, false, false);

                GL.Clear(ClearBufferMask.DepthBufferBit);
                GL.Viewport(0, 0, SHADOW_MAP_WIDTH, SHADOW_MAP_HEIGHT);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref this._ShadowMatrix4MapPerspective);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref this._ShadowMatrix4MapModelView);

                Trial.drawObjectsInShadow(true, this._FloatDrawTime);
                Bobcat.Draw(this.ActualAngles, true, true);// draw cab, in shadow buffer

                // This is matrix transform every coordinate x,y,z Moving from unit cube [-1,1] to [0,1]  

                GL.GetFloat(GetPName.ModelviewMatrix, this.drawStart_mv);
                GL.GetFloat(GetPName.ProjectionMatrix, this.drawStart_pv);

                GL.MatrixMode(MatrixMode.Texture);
                GL.LoadMatrix(ref this.drawStart_bias);
                GL.MultMatrix(this.drawStart_pv);
                GL.MultMatrix(this.drawStart_mv);
                GL.MatrixMode(MatrixMode.Modelview);

                GL.ColorMask(true, true, true, true);
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
                GL.ActiveTexture(TextureUnit.Texture0);

                bool drawGhost = (GL_Handler.DrawGhost) && (this._EnumGhostSetup == EnumGLSetup.good);

                if (this.__DrawType == DrawType.CatLaces)
                {
                    GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
                    GL.Enable(EnableCap.StencilTest);

                    this.drawModelPreType(DrawStage.NormalOutsideShadow);

                    GL.UseProgram(GL_Handler._ShadowIntProgram);
                    GL.ActiveTexture(TextureUnit.Texture7);
                    GL.BindTexture(TextureTarget.Texture2D, this._ShadowIntTexture);
                    this.drawModelPreType(DrawStage.NormalInsideShadow);
                    if (drawGhost) this.drawGhost(false, true);
                    else this.dontDrawGhost();

                    GL.StencilFunc(StencilFunction.Equal, 1, 0xFF);
                    this._BoolFake3DLR = false;

                    this.drawModelPreType(DrawStage.NormalOutsideShadow);

                    GL.UseProgram(GL_Handler._ShadowIntProgram);
                    GL.ActiveTexture(TextureUnit.Texture7);
                    GL.BindTexture(TextureTarget.Texture2D, this._ShadowIntTexture);
                    this.drawModelPreType(DrawStage.NormalInsideShadow);

                    if (drawGhost) this.drawGhost(true, false);
                    else this.dontDrawGhost();

                    GL.Disable(EnableCap.StencilTest);
                }
                else // Normal (not laced)
                {
                    this.drawModelPreType(DrawStage.NormalOutsideShadow);

                    GL.UseProgram(GL_Handler._ShadowIntProgram);
                    GL.ActiveTexture(TextureUnit.Texture7);
                    GL.BindTexture(TextureTarget.Texture2D, this._ShadowIntTexture);

                    this.drawModelPreType(DrawStage.NormalInsideShadow);
                    if (drawGhost) this.drawGhost(true, true);
                    else this.dontDrawGhost();
                }
                if (this._BoolShowSide)
                {
                    this.drawOrthoPre();

                    if (this.__DrawType == DrawType.CatSplitVertical)
                    {
                        this.drawOrtho(this._RectangleOrthoS1);
                        this.drawOrtho(this._RectangleOrthoS2);
                    }
                    else this.drawOrtho(this._RectangleOrtho);

                    this.drawOrthoPost();
                }
            }
            else
            {
                Console.WriteLine("GL Shadow Exception");
                this.resetGL();
            }
        }

        private void dontDrawGhost()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.UseProgram(0);
        }

        private bool _BoolStencilNeedsSetup = true;
        private bool _BoolStencilNeedsSetupGhost = true;
        private void initStencilBuffer()  //  Lacing Only
        {
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.StencilTest);

            GL.ClearStencil(0);
            GL.Clear(
                ClearBufferMask.StencilBufferBit |
                ClearBufferMask.StencilBufferBit |
                ClearBufferMask.ColorBufferBit);

            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
            GL.StencilMask(0xFF);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, this._RectangleViewportM.Width, 0, 1, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Viewport(this._RectangleViewportM);

            GL.Begin(BeginMode.Quads);
            for (int i = 0; i < this._RectangleViewportM.Width; i += 2)
            {
                GL.Vertex2(i + 0, 0);
                GL.Vertex2(i + 0, 1);
                GL.Vertex2(i + 1, 1);
                GL.Vertex2(i + 1, 0);
            }
            GL.End();


            GL.StencilFunc(StencilFunction.Equal, 1, 0xFF);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            GL.StencilMask(0);

            GL.Disable(EnableCap.StencilTest);
            GL.Enable(EnableCap.CullFace);
        }





    }
}