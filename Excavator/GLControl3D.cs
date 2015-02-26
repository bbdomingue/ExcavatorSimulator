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
    internal class GLControl3D
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

        internal static readonly Color _ColorClearDirt = Color.FromArgb(255, 77, 48, 0);

        internal enum DrawType
        {
            Normal,

            Outside,
            ShadowBuffer,

            Stereo,
            VerticalSync,
            SplitHorizontal,
            SplitVertical,
            Laces,

            Rift,
            Cat,
            CatStereo,
            ShadowBufferGraphics
        }

        internal GLControl3D(Panel p)
        {
            this.createGLControl(p);
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

        private DrawType __DrawType = DrawType.Normal;
        internal DrawType _DrawType
        {
            set
            {
                this.__DrawType = value;

                this.updateProjectionMatrix();
                this.updateModelMatrix();

                this._BoolVSync = this.__DrawType == DrawType.VerticalSync;
                this._GLControl.VSync = this._BoolVSync;

                if (this.__DrawType == DrawType.Stereo != this._Bool3D)
                {
                    this._Bool3D = !this._Bool3D;
                    this.createGLControl(null);
                }

            }
        }

        private void createGLControl(Control c)
        {
            this._BoolGlNotLoaded = true; 

            if (this._GLControl != null)
            {
                this.resetGL();
                FormBase._FormBase.resetGL();

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
            if (this._ShadowIntFrameBuffer != 0) GL.DeleteFramebuffers(1, ref this._ShadowIntFrameBuffer);
            this._ShadowIntFrameBuffer = 0;

            if (this._ShadowIntTexture != 0) GL.DeleteTexture(this._ShadowIntTexture);
            this._ShadowIntTexture = 0;

            if (this._GhostIntFBO != 0) GL.DeleteFramebuffers(1, ref this._GhostIntFBO);
            this._GhostIntFBO = 0;

            if (this._GhostIntTextColor != 0) GL.DeleteTexture(this._GhostIntTextColor);
            this._GhostIntTextColor = 0;

            if (this._GhostIntTextDepth != 0) GL.DeleteTexture(this._GhostIntTextDepth);
            this._GhostIntTextDepth = 0;

            this._BoolStencilNeedsSetup = true;
            this._BoolStencilNeedsSetupGhost = true;

            this._EnumShadowSetup = EnumGLSetup.not;
            this._EnumGhostSetup = EnumGLSetup.not;
        }

        private void resetGL()
        {
            if (this._ShadowIntProgram != 0) GL.DeleteProgram(this._ShadowIntProgram);
            if (this._ShadowIntShaderV != 0) GL.DeleteShader(this._ShadowIntShaderV);
            if (this._ShadowIntShaderF != 0) GL.DeleteShader(this._ShadowIntShaderF);

            this._ShadowIntProgram = 0;
            this._ShadowIntShaderV = 0;
            this._ShadowIntShaderF = 0;

            if (this._GhostIntProgram != 0) GL.DeleteProgram(this._GhostIntProgram);
            if (this._GhostIntShaderF != 0) GL.DeleteShader(this._GhostIntShaderF);
            if (this._GhostIntShaderV != 0) GL.DeleteShader(this._GhostIntShaderV);

            this._GhostIntProgram = 0;
            this._GhostIntShaderF = 0;
            this._GhostIntShaderV = 0;

            Trial._Trial.GLDelete();
            Bobcat.GLDelete();
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

            this.setLightPos(this._FloatsLightPos);
            GL.Light(LightName.Light0, LightParameter.Diffuse,
                new float[] { diffuse, diffuse, diffuse, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Ambient,
                new float[] { ambient, ambient, ambient, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Specular,
                new float[] { specular, specular, specular, 1.0f });
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Lighting);

            this.updateModelMatrix();
            this._GLControl_Resize(sender, e);
        }

        internal void _GLControl_Resize(object sender, EventArgs e)
        {
            if (this._BoolGlNotLoaded) return;

            int sw = this._GLControl.Width;
            int sh = this._GLControl.Height;

            this._RectangleViewportBasic = new Rectangle(0, 0, sw, sh);

            this.updateProjectionMatrix();

            this.resetGL_BuffersOnly();
        }

        internal void SkyBoxDimChanged()
        {
            this.updateProjectionMatrix();

            if (this._SkyBox != null) this._SkyBox.resetDistance();
        }

        public void updateProjectionMatrix()
        {
            switch (this.__DrawType)
            {
                case DrawType.Cat:
                    {
                        int sw = this._GLControl.Width;
                        int sh = this._GLControl.Height;
                        float aspect;

                        int w = (int)(sw * (1 + Math.Abs(this._Vector3EyeMLocationRelativeToScreen.X)));
                        int h = (int)(sh * (1 + Math.Abs(this._Vector3EyeMLocationRelativeToScreen.Y)));

                        aspect = w;
                        aspect /= h;

                        int x = (this._Vector3EyeMLocationRelativeToScreen.X < 0) ? sw - w : 0;
                        int y = (this._Vector3EyeMLocationRelativeToScreen.Y < 0) ? 0 : sh - h;
                        // Note change in orientation.
                        // head x and head y are -1 in top left corner, + 1 in bottom right
                        // GL Viewport is from bottom left

                        float fov = h;
                        fov /= sh * 2;
                        fov = (float)Math.Atan2(fov, this._Vector3EyeMLocationRelativeToScreen.Z);
                        fov *= 2;

                        this._RectangleViewportM = new Rectangle(x, y, w, h);

                        this._Matrix4ViewPortM = Matrix4.CreatePerspectiveFieldOfView(
                            fov,
                            aspect,
                            0.1f,
                            FormBase._FloatSkyBoxDim * 2f);

                        break;
                    }
                case DrawType.CatStereo:
                    {
                        int sw = this._GLControl.Width;
                        int sh = this._GLControl.Height;
                        float aspect;

                        for (int i = 0; i < 2; i++)
                        {
                            Vector3 vec = new Vector3();

                            switch (i)
                            {
                                case 0: vec = this._Vector3EyeRLocationRelativeToScreen; break;
                                case 1: vec = this._Vector3EyeLLocationRelativeToScreen; break;
                            }

                            int w = (int)(sw * (1 + Math.Abs(vec.X)));
                            int h = (int)(sh * (1 + Math.Abs(vec.Y)));

                            aspect = w;
                            aspect /= h;

                            int x = (vec.X < 0) ? sw - w : 0;
                            int y = (vec.Y < 0) ? 0 : sh - h;
                            // Note change in orientation.
                            // head x and head y are -1 in top left corner, + 1 in bottom right
                            // GL Viewport is from bottom left

                            float fov = h;
                            fov /= sh * 2;
                            fov = (float)Math.Atan2(fov, vec.Z);
                            fov *= 2;

                            switch (i)
                            {
                                case 0:
                                    {
                                        this._RectangleViewportR = new Rectangle(x, (sh + y)/2, w, h/2);
                                        this._Matrix4ViewPortR = Matrix4.CreatePerspectiveFieldOfView(
                                            fov,
                                            aspect,
                                            0.1f,
                                            FormBase._FloatSkyBoxDim * 2f); 
                                        break;
                                    }
                                case 1:
                                    {
                                        this._RectangleViewportL = new Rectangle(x, y/2, w, h/2);
                                        this._Matrix4ViewPortL = Matrix4.CreatePerspectiveFieldOfView(
                                            fov,
                                            aspect,
                                            0.1f,
                                            FormBase._FloatSkyBoxDim * 2f);
                                        break;
                                    }
                            }


                        }
                        break;
                    }
                default:
                    {
                        int w = this._GLControl.Width;
                        int h = this._GLControl.Height;

                        this._RectangleViewportM = new Rectangle(0, 0, w, h);

                        float aspect = w;
                        aspect /= h;

                        this._Matrix4ViewPortM = Matrix4.CreatePerspectiveFieldOfView(
                        StaticMethods.toRadiansF(55.0f),
                        aspect,
                        0.1f,
                        FormBase._FloatSkyBoxDim * 2f);

                        break;
                     }
            }
        }

        private Rectangle _RectangleViewportBasic;
        private Rectangle _RectangleViewportR;
        private Rectangle _RectangleViewportL;
        private Rectangle _RectangleViewportM;

        private Matrix4 _Matrix4ViewPortR;
        private Matrix4 _Matrix4ViewPortL;
        private Matrix4 _Matrix4ViewPortM;

        private Matrix4 _Matrix4EyeR;
        private Matrix4 _Matrix4EyeL;
        private Matrix4 _Matrix4EyeM;
        private Vector4 _Vector3EyePositionInCab = new Vector4(0, 30, 00, 1);

        internal static Vector3 _Vector3EyePositionInCabS = new Vector3();

        private float _FloatHeadYaw = 0;
        private float _FloatHeadPitch = 0;

        private float _FloatCabYaw = 0;
        private Vector4 _Vector3EyeDirection = new Vector4();

        private float _ViewFocalDistance = 0;
        private float _ViewEyeDistanceHalf = 0;

        private Vector3 _Vector3EyeRLocationRelativeToScreen;
        private Vector3 _Vector3EyeLLocationRelativeToScreen;
        private Vector3 _Vector3EyeMLocationRelativeToScreen;

        internal void setHeadYawAndPitch(float y, float p)
        {
            this._FloatHeadYaw = y;
            this._FloatHeadPitch = p;
            this.updateModelMatrix();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f1">_ViewEyeDistanceHalf</param>
        /// <param name="f2">_ViewFocalDistance</param>
        /// <param name="CenterWC">World Coordinates</param>
        /// <param name="eyeR">Screen Coordinates.  X and Y bound -1 and 1 for TV. Z is in units of screen heights</param>
        /// <param name="eyeL">Screen Coordinates.  X and Y bound -1 and 1 for TV. Z is in units of screen heights</param>
        /// <param name="eyeM">Screen Coordinates.  X and Y bound -1 and 1 for TV. Z is in units of screen heights</param>
        internal void setViewEyePositionInCab(float f1, float f2, Vector3 CenterWC, Vector3 eyeR, Vector3 eyeL, Vector3 eyeM)
        {
            this._ViewEyeDistanceHalf = f1;
            this._ViewFocalDistance = f2;

            this._Vector3EyePositionInCab = new Vector4(CenterWC, 1);
            GLControl3D._Vector3EyePositionInCabS = CenterWC;

            this._Vector3EyeRLocationRelativeToScreen = eyeR;
            this._Vector3EyeLLocationRelativeToScreen = eyeL;
            this._Vector3EyeMLocationRelativeToScreen = eyeM;

            this.updateModelMatrix();

            switch (this.__DrawType)
            {
                case DrawType.Cat:
                case DrawType.CatStereo:
                    this.updateProjectionMatrix();
                    break;
            }
        }

        private void updateModelMatrix()
        {
            Vector4 eP;
            Vector3 eR, eL, eM;
            Vector3 fPR, fPL, fPM;

            switch (this.__DrawType)
            {
                case DrawType.Outside:
                    {
                        eP = new Vector4(0, 0, 350, 1);
                        eP = Vector4.Transform(eP,
                            Matrix4.CreateRotationX(StaticMethods.toRadiansF(this._FloatHeadPitch)));
                        eP = Vector4.Transform(eP,
                            Matrix4.CreateRotationY(StaticMethods.toRadiansF(this._FloatCabYaw + this._FloatHeadYaw)));

                        this._Vector3EyeDirection = Vector4.Multiply(eP, -1);
                        this._Vector3EyeDirection.Normalize();

                        fPM = new Vector3(0, 0, 0);

                        eM = new Vector3(eP);

                        this._Matrix4EyeM = Matrix4.LookAt(eM, fPM, Vector3.UnitY);

                        break;
                    }
                case DrawType.Cat:
                    {
                        Matrix4 rotY = Matrix4.CreateRotationY(StaticMethods.toRadiansF(this._FloatCabYaw));

                        this._Vector3EyeDirection = new Vector4(0, 0, -1, 1);
                        this._Vector3EyeDirection = Vector4.Transform(this._Vector3EyeDirection,
                            Matrix4.CreateRotationX(StaticMethods.toRadiansF(Bobcat._TV_Float_Rotate)));
                        this._Vector3EyeDirection = Vector4.Transform(this._Vector3EyeDirection, rotY);

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
                    }
            }
        }






















        internal void Invalidate() { this._GLControl.Invalidate(); }

        private void _Paint(object sender, PaintEventArgs e)
        {
            if (this._BoolGlNotLoaded) return;

            Trial._Trial.updateTime();

            Trial._Trial.updateSim();

            if (this._FloatCabYaw != Trial._Trial.ActualAngles.cab)
            {
                this._FloatCabYaw = Trial._Trial.ActualAngles.cab;
                this.updateModelMatrix();
            }

            this.drawStart();

            this._GLControl.SwapBuffers();
        }

        private enum DrawStage { WithShadow, ExcavatorGhost, WithoutShadow };
        private void drawModelPreType(DrawStage s)
        {
            switch (this.__DrawType)
            {
                case DrawType.Normal:
                case DrawType.Outside:
                    {
                        GL.Viewport(this._RectangleViewportM);
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.LoadMatrix(ref this._Matrix4ViewPortM);
                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.LoadMatrix(ref this._Matrix4EyeM);
                        this.drawModelPostType(s);
                        break;
                    }
                case DrawType.ShadowBuffer:
                    {
                        if (s == DrawStage.WithoutShadow)
                        {
                            GL.Viewport(this._RectangleViewportM);
                            GL.MatrixMode(MatrixMode.Projection);
                            GL.LoadIdentity();
                            GL.Ortho(0.0, 1.0, 0.0, 1.0, -1.0, 1.0);
                            GL.MatrixMode(MatrixMode.Texture);
                            GL.LoadIdentity();

                            GL.MatrixMode(MatrixMode.Modelview);
                            GL.LoadIdentity();

                            GL.ActiveTexture(TextureUnit.Texture0);
                            GL.BindTexture(TextureTarget.Texture2D, this._ShadowIntTexture);

                            GL.Color3(Color.White);
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
                        }
                        break;
                    }
                case DrawType.ShadowBufferGraphics:
                    {
                        if (s == DrawStage.WithoutShadow)
                        {
                            GL.Viewport(this._RectangleViewportBasic);
                            GL.MatrixMode(MatrixMode.Projection);
                            GL.LoadMatrix(ref this._ShadowMatrix4MapPerspective);
                            GL.MatrixMode(MatrixMode.Modelview);
                            GL.LoadMatrix(ref this._ShadowMatrix4MapModelView);
                            this.drawModelPostType(DrawStage.WithShadow);
                        }
                        break;
                    }
                case DrawType.Stereo:
                    {
                        GL.Viewport(this._RectangleViewportM);
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.LoadMatrix(ref this._Matrix4ViewPortM);
                        GL.MatrixMode(MatrixMode.Modelview);

                        GL.DrawBuffer(DrawBufferMode.BackLeft);
                        GL.LoadMatrix(ref this._Matrix4EyeL);
                        this.drawModelPostType(s);

                        GL.DrawBuffer(DrawBufferMode.BackRight);
                        GL.LoadMatrix(ref this._Matrix4EyeR);
                        this.drawModelPostType(s);
                        break;
                    }
                case DrawType.Laces:
                case DrawType.VerticalSync:
                    {
                        GL.Viewport(this._RectangleViewportM);
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.LoadMatrix(ref this._Matrix4ViewPortM);
                        GL.MatrixMode(MatrixMode.Modelview);
                        if (this._BoolFake3DLR) GL.LoadMatrix(ref this._Matrix4EyeL);
                        else GL.LoadMatrix(ref this._Matrix4EyeR);
                        this.drawModelPostType(s);
                        break;
                    }
                case DrawType.Rift:
                    {
                        break;
                    }
                case DrawType.SplitHorizontal:
                    {
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.LoadMatrix(ref this._Matrix4ViewPortM);
                        GL.MatrixMode(MatrixMode.Modelview);

                        int w = this._GLControl.Width;
                        int h = this._GLControl.Height;

                        GL.Viewport(0, 0, w / 2, h);
                        GL.LoadMatrix(ref this._Matrix4EyeL);
                        this.drawModelPostType(s);

                        GL.Viewport(w / 2, 0, w / 2, h);
                        GL.LoadMatrix(ref this._Matrix4EyeR);
                        this.drawModelPostType(s);

                        break;
                    }
                case DrawType.SplitVertical:
                    {
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.LoadMatrix(ref this._Matrix4ViewPortM);
                        GL.MatrixMode(MatrixMode.Modelview);

                        int w = this._GLControl.Width;
                        int h = this._GLControl.Height;

                        GL.Viewport(0, 0, w, h / 2);
                        GL.LoadMatrix(ref this._Matrix4EyeL);
                        this.drawModelPostType(s);

                        GL.Viewport(0, h / 2, w, h / 2);
                        GL.LoadMatrix(ref this._Matrix4EyeR);
                        this.drawModelPostType(s);

                        break;
                    }
                case DrawType.Cat:
                    {
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.LoadMatrix(ref this._Matrix4ViewPortM);

                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.LoadMatrix(ref this._Matrix4EyeM);

                        GL.Viewport(this._RectangleViewportM);

                        this.drawModelPostType(s);

                        break;
                    }
                case DrawType.CatStereo:
                    {
                        int w = this._GLControl.Width;
                        int h = this._GLControl.Height;

                        GL.Enable(EnableCap.ScissorTest);
                        {
                            GL.Scissor(0, 0, w, h / 2);
                            GL.MatrixMode(MatrixMode.Projection);
                            GL.LoadMatrix(ref this._Matrix4ViewPortL);
                            GL.MatrixMode(MatrixMode.Modelview);
                            GL.LoadMatrix(ref this._Matrix4EyeL);
                            GL.Viewport(this._RectangleViewportL);
                            this.drawModelPostType(s);

                            GL.Scissor(0, h/2, w, h / 2);
                            GL.MatrixMode(MatrixMode.Projection);
                            GL.LoadMatrix(ref this._Matrix4ViewPortR);
                            GL.MatrixMode(MatrixMode.Modelview);
                            GL.LoadMatrix(ref this._Matrix4EyeR);
                            GL.Viewport(this._RectangleViewportR);
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

            switch (s)
            {
                case DrawStage.WithShadow:
                    {
                        Trial._Trial.drawObjectsInShadow(true);
                        bool drawCab = !FormBase._FormBase._BoolFullScreen || (
                            this.__DrawType == DrawType.Outside ||
                            this.__DrawType == DrawType.ShadowBufferGraphics);

                        Bobcat.Draw(Trial._Trial.ActualAngles, drawCab , true);
                        break;
                    }
                case DrawStage.ExcavatorGhost:
                    {
                        Bobcat.Draw(Trial._Trial.GhostAngles, false, true);
                        break;
                    }
                case DrawStage.WithoutShadow:
                    {
                        Trial._Trial.drawObjectsNotInShadow();
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

        private float[] _FloatsLightPos = new float[] { 500, 1500, -100, 1 };
        private void setLightPos(float[] thereHadBetterBe4ValuesInThisArrayYouTwat)
        {
            // And Last value should be 0 (1 specifies light location, 0 specifies infinity)

            this._FloatsLightPos = thereHadBetterBe4ValuesInThisArrayYouTwat;

            Vector3 lightPos = new Vector3(this._FloatsLightPos[0], this._FloatsLightPos[1], this._FloatsLightPos[2]);

            lightPos.Normalize();
            lightPos = lightPos * GLControl3D._IntLightRenderDistance;

            this._ShadowMatrix4MapModelView = Matrix4.LookAt(
                new Vector3(lightPos),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0));
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

        private void BindTexture(int program, TextureUnit textureUnit, string UniformName)
        {
            GL.Uniform1(GL.GetUniformLocation(program, UniformName), textureUnit - TextureUnit.Texture0);
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

            this.drawModelPreType(DrawStage.ExcavatorGhost);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.UseProgram(0);

            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer
            GL.DrawBuffer(DrawBufferMode.Back);

            if (copybuffer)
            {
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
        private int _ShadowIntProgram = 0;
        private int _ShadowIntShaderV = 0;
        private int _ShadowIntShaderF = 0;
        private int _ShadowIntTexture = 0;

        const int SHADOW_DIM = 1024*4;
        const int SHADOW_MAP_WIDTH = SHADOW_DIM;
        const int SHADOW_MAP_HEIGHT = SHADOW_DIM;
        const float SHADOW_MAP_ASPECT = ((float)SHADOW_MAP_WIDTH) / ((float)SHADOW_MAP_HEIGHT);
        const float SHADOW_MAP_FOV = (float)(42.0 * Math.PI / 180.0);

        private EnumGLSetup _EnumShadowSetup = EnumGLSetup.not;
        private enum EnumGLSetup { not, tried, good }

        private Matrix4 _ShadowMatrix4MapPerspective;
        private Matrix4 _ShadowMatrix4MapModelView;

        //Light position
        const int _IntLightRenderDistance = 750;

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






















        private float[] drawStart_mv = new float[16];
        private float[] drawStart_pv = new float[16];

        private Matrix4 drawStart_bias = new Matrix4(
            0.5f, 0.0f, 0.0f, 0.0f,
            0.0f, 0.5f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.5f, 0.0f,
            0.5f, 0.5f, 0.5f, 1.0f
        );

        private void drawStart()
        {
            if (this.__DrawType == DrawType.Laces) this._BoolFake3DLR = true;
            else this._BoolFake3DLR = !this._BoolFake3DLR;

            if (this._BoolStencilNeedsSetup)
            {
                this.initStencilBuffer();
                this._BoolStencilNeedsSetup = false;
            }

            if (this._EnumShadowSetup == EnumGLSetup.not)
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
                        this.BindTexture(this._GhostIntProgram, TextureUnit.Texture0, "tex0");
                        this.BindTexture(this._GhostIntProgram, TextureUnit.Texture1, "tex1");
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
                        this._ShadowIntProgram != 0 &&
                        this._ShadowIntShaderV != 0 &&
                        this._ShadowIntShaderF != 0;

                    if (!a) a = Shaders.CreateShaders(
                        Properties.Resources.ShadowVertex,
                        Properties.Resources.ShadowFragment,
                        out this._ShadowIntProgram,
                        out this._ShadowIntShaderV,
                        out this._ShadowIntShaderF);

                    if (a)
                    {
                        this._EnumShadowSetup = EnumGLSetup.good;
                        this._ShadowMatrix4MapPerspective = Matrix4.CreatePerspectiveFieldOfView(
                            SHADOW_MAP_FOV,
                            SHADOW_MAP_ASPECT,
                            100, 40000);

                            GL.UseProgram(this._ShadowIntProgram);
                            this.BindTexture(this._ShadowIntProgram, TextureUnit.Texture0, "tex0");
                            this.BindTexture(this._ShadowIntProgram, TextureUnit.Texture7, "ShadowMap");
                            GL.UseProgram(0);
                    }
                }

                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            }

            if (this._EnumShadowSetup != EnumGLSetup.good)
            {
                Console.WriteLine("GL Shadow Exception");
                this.resetGL();
                return;
            }

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

            Trial._Trial.drawObjectsInShadow(false);
            Bobcat.Draw(Trial._Trial.ActualAngles, true, false);

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

            bool drawGhost = (Trial._Trial.hasGhost()) && (this._EnumGhostSetup == EnumGLSetup.good);

            if (this.__DrawType == DrawType.Laces)
            {
                GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
                GL.Enable(EnableCap.StencilTest);

                this.drawModelPreType(DrawStage.WithoutShadow);

                GL.UseProgram(this._ShadowIntProgram);
                GL.ActiveTexture(TextureUnit.Texture7);
                GL.BindTexture(TextureTarget.Texture2D, this._ShadowIntTexture);
                this.drawModelPreType(DrawStage.WithShadow);
                if (drawGhost) this.drawGhost(false, true);
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.UseProgram(0);
                }
                GL.StencilFunc(StencilFunction.Equal, 1, 0xFF);
                this._BoolFake3DLR = false;

                this.drawModelPreType(DrawStage.WithoutShadow);

                GL.UseProgram(this._ShadowIntProgram);
                GL.ActiveTexture(TextureUnit.Texture7);
                GL.BindTexture(TextureTarget.Texture2D, this._ShadowIntTexture);
                this.drawModelPreType(DrawStage.WithShadow);
                if (drawGhost) this.drawGhost(true, false);
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.UseProgram(0);

                    GL.Disable(EnableCap.StencilTest);
                }

            }
            else
            {
                this.drawModelPreType(DrawStage.WithoutShadow);

                GL.UseProgram(this._ShadowIntProgram);
                GL.ActiveTexture(TextureUnit.Texture7);
                GL.BindTexture(TextureTarget.Texture2D, this._ShadowIntTexture);
                this.drawModelPreType(DrawStage.WithShadow);
                if (drawGhost) this.drawGhost(true, true);
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.UseProgram(0);
                }

            }
        }

        private bool _BoolStencilNeedsSetup = true;
        private bool _BoolStencilNeedsSetupGhost = true;
        private void initStencilBuffer()
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