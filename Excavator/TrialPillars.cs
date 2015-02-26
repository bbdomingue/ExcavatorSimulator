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

using PhysX;

using Matrix = PhysX.Math.Matrix;
using V3 = PhysX.Math.Vector3;

namespace Excavator
{
    internal partial class TrialPillars : Trial
    {
        private Physics _Physics;
        private Scene _Scene;

        const float pilWidth = 20;

        internal TrialPillars(FormBase fb)
            : base(fb, false, null, -1)
        {
            this._Physics = new Physics(Trial._Foundation, false);
            this._Scene = this._Physics.CreateScene(new SceneDesc()
            {
                Gravity = new V3(0, -20, 0)
            });

            var _MaterialCage = this._Physics.CreateMaterial(0.1f, 0.1f, 0.0f);
            var groundPlane = this._Physics.CreateRigidStatic();
            groundPlane.CreateShape(
                new PlaneGeometry(),
                _MaterialCage,
                Matrix.RotationAxis(new V3(0, 0, 1), (float)System.Math.PI * 0.5f));
            this._Scene.AddActor(groundPlane);

            var BallMaterial = this._Physics.CreateMaterial(0.0f, 0.0f, 0.0f);

            this.WreckingBall = this._Physics.CreateRigidDynamic(Matrix.Translation(0, 1000, 0));
            this.WreckingBall.CreateShape(new SphereGeometry(12), BallMaterial);
            this.WreckingBall.Flags = RigidDynamicFlags.Kinematic;
            this._Scene.AddActor(this.WreckingBall);

            int len = this.Pillars.Length;
            float inc = 2 * StaticMethods._PIF / len;

            for (int i = 0; i < len; i++)
            {
                float theta = (i + 0.5f) * inc;

                float dist = 200;

                float height = 50;

                this.PillarHeight[i] = height / 2;

                this.Pillars[i] = this._Physics.CreateRigidDynamic(
                    Matrix.Translation(
                    dist * (float)Math.Sin(theta),
                    height / 2,
                    dist * -(float)Math.Cos(theta)));

                this.Pillars[i].CreateShape(new BoxGeometry(pilWidth / 2, height / 2, pilWidth / 2), BallMaterial);
                this.Pillars[i].SetMassAndUpdateInertia(40);

                this.Pillars[i].LinearDamping = 0.01f;
                this.Pillars[i].AngularDamping = 0.01f;
                this._Scene.AddActor(this.Pillars[i]);
            }
        }














        public override void updateSim()
        {
            base.updateSim();

            if (this._TimeSpanF > 0)
            {
                var ABM = Matrix4.CreateTranslation(AB) * Matrix4.CreateRotationY(StaticMethods.toRadiansF(this.ActualAngles.cab));
                var BCM = Matrix4.Mult(Matrix4.CreateTranslation(BC), Matrix4.CreateRotationY(StaticMethods.toRadiansF(this.ActualAngles.swi)));
                var CDM = Matrix4.Mult(Matrix4.CreateTranslation(CD), Matrix4.CreateRotationX(StaticMethods.toRadiansF(this.ActualAngles.boo)));
                var DEM = Matrix4.Mult(Matrix4.CreateTranslation(DE), Matrix4.CreateRotationX(StaticMethods.toRadiansF(this.ActualAngles.arm)));
                var EFM = Matrix4.Mult(Matrix4.CreateTranslation(EF), Matrix4.CreateRotationX(StaticMethods.toRadiansF(this.ActualAngles.buc)));

                var A = Matrix4.CreateTranslation(0, 0, 0);
                var B = ABM * A;
                var C = BCM * B;
                var D = CDM * C;
                var E = DEM * D;
                var F = EFM * E;

                this.WreckingBall.SetKinematicTarget(parseMatM(ref F));

                this._Scene.Simulate(this._TimeSpanF);
                this._Scene.FetchResults(true);
            }
        }


        public override void Deconstruct()
        {
            base.Deconstruct();
            if (this._Physics != null) if (!this._Physics.Disposed) this._Physics.Dispose();
        }




























        RigidDynamic WreckingBall = null;

        RigidDynamic[] Pillars = new RigidDynamic[6];
        float[] PillarHeight = new float[6];

        Vector3 AB = new Vector3(Bobcat.vecOffsetSwing2_Inches.X, 0, Bobcat.vecOffsetSwing2_Inches.Z);
        Vector3 BC = new Vector3(0, 15, -4);
        Vector3 CD = new Vector3(0, 0, -105);
        Vector3 DE = new Vector3(0, 0, -54);
        Vector3 EF = new Vector3(0, 11, -9);

        private Matrix4 parseMatM(ref Matrix m)
        {
            return new Matrix4(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44);
        }

        public override void drawObjectsNotInShadow()
        {
            base.drawObjectsNotInShadow();

            GL.Disable(EnableCap.Lighting);
            GL.Color3(Color.White);
            GL.LineWidth(5);

            float f = 5f;
            Vector3 p1 = new Vector3(f, f, f);
            Vector3 p2 = new Vector3(-f, f, f);
            Vector3 p3 = new Vector3(-f, -f, f);
            Vector3 p4 = new Vector3(f, -f, f);

            Vector3 p5 = new Vector3(f, f, -f);
            Vector3 p6 = new Vector3(-f, f, -f);
            Vector3 p7 = new Vector3(-f, -f, -f);
            Vector3 p8 = new Vector3(f, -f, -f);

            int len = this.Pillars.Length;
            for (int i = 0; i < len; i++)
            {
                var dyo2 = this.Pillars[i];

                p1 = new Vector3(pilWidth / 2, PillarHeight[i], pilWidth / 2);
                p2 = new Vector3(-pilWidth / 2, PillarHeight[i], pilWidth / 2);
                p3 = new Vector3(-pilWidth / 2, -PillarHeight[i], pilWidth / 2);
                p4 = new Vector3(pilWidth / 2, -PillarHeight[i], pilWidth / 2);

                p5 = new Vector3(pilWidth / 2, PillarHeight[i], -pilWidth / 2);
                p6 = new Vector3(-pilWidth / 2, PillarHeight[i], -pilWidth / 2);
                p7 = new Vector3(-pilWidth / 2, -PillarHeight[i], -pilWidth / 2);
                p8 = new Vector3(pilWidth / 2, -PillarHeight[i], -pilWidth / 2);

                GL.PushMatrix();
                {
                    var mat1 = dyo2.GlobalPose;
                    var mat2 = parseMatM(ref mat1);
                    GL.MultMatrix(ref mat2);

                    GL.Begin(BeginMode.Lines);
                    {
                        GL.Vertex3(p1); GL.Vertex3(p2);
                        GL.Vertex3(p2); GL.Vertex3(p3);
                        GL.Vertex3(p3); GL.Vertex3(p4);
                        GL.Vertex3(p4); GL.Vertex3(p1);

                        GL.Vertex3(p5); GL.Vertex3(p6);
                        GL.Vertex3(p6); GL.Vertex3(p7);
                        GL.Vertex3(p7); GL.Vertex3(p8);
                        GL.Vertex3(p8); GL.Vertex3(p5);

                        GL.Vertex3(p5); GL.Vertex3(p1);
                        GL.Vertex3(p6); GL.Vertex3(p2);
                        GL.Vertex3(p7); GL.Vertex3(p3);
                        GL.Vertex3(p8); GL.Vertex3(p4);
                    }
                    GL.End();
                }
                GL.PopMatrix();
            }

            if (true)
            {

                var ABM = Matrix4.CreateTranslation(AB) * Matrix4.CreateRotationY(StaticMethods.toRadiansF(this.ActualAngles.cab));
                var BCM = Matrix4.Mult(Matrix4.CreateTranslation(BC), Matrix4.CreateRotationY(StaticMethods.toRadiansF(this.ActualAngles.swi)));
                var CDM = Matrix4.Mult(Matrix4.CreateTranslation(CD), Matrix4.CreateRotationX(StaticMethods.toRadiansF(this.ActualAngles.boo)));
                var DEM = Matrix4.Mult(Matrix4.CreateTranslation(DE), Matrix4.CreateRotationX(StaticMethods.toRadiansF(this.ActualAngles.arm)));
                var EFM = Matrix4.Mult(Matrix4.CreateTranslation(EF), Matrix4.CreateRotationX(StaticMethods.toRadiansF(this.ActualAngles.buc)));

                var A = Matrix4.CreateTranslation(0, 0, 0);
                var B = ABM * A;
                var C = BCM * B;
                var D = CDM * C;
                var E = DEM * D;
                var F = EFM * E;

                var lsM = new List<Matrix4>();

                lsM.Add(A);
                lsM.Add(B);
                lsM.Add(C);
                lsM.Add(D);
                lsM.Add(E);
                lsM.Add(F);



                f = 5f;
                p1 = new Vector3(f, f, f);
                p2 = new Vector3(-f, f, f);
                p3 = new Vector3(-f, -f, f);
                p4 = new Vector3(f, -f, f);

                p5 = new Vector3(f, f, -f);
                p6 = new Vector3(-f, f, -f);
                p7 = new Vector3(-f, -f, -f);
                p8 = new Vector3(f, -f, -f);

                foreach (var dyo in lsM)
                {
                    GL.PushMatrix();
                    {
                        var mate = dyo;
                        GL.MultMatrix(ref mate);

                        GL.Color3(Color.White);
                        GL.Begin(BeginMode.Lines);
                        {
                            GL.Vertex3(p1); GL.Vertex3(p2);
                            GL.Vertex3(p2); GL.Vertex3(p3);
                            GL.Vertex3(p3); GL.Vertex3(p4);
                            GL.Vertex3(p4); GL.Vertex3(p1);

                            GL.Vertex3(p5); GL.Vertex3(p6);
                            GL.Vertex3(p6); GL.Vertex3(p7);
                            GL.Vertex3(p7); GL.Vertex3(p8);
                            GL.Vertex3(p8); GL.Vertex3(p5);

                            GL.Vertex3(p5); GL.Vertex3(p1);
                            GL.Vertex3(p6); GL.Vertex3(p2);
                            GL.Vertex3(p7); GL.Vertex3(p3);
                            GL.Vertex3(p8); GL.Vertex3(p4);
                        }
                        GL.End();
                    }
                    GL.PopMatrix();
                }
            }

            GL.Enable(EnableCap.Lighting);

            GL.LineWidth(1);

            GL.PushMatrix();
            {
                //                GL.MultMatrix(ref F);
                //                GLSphere.drawSphere(0, 0, 0, 12);
            }
            GL.PopMatrix();
        }

        private Matrix parseMatM(ref Matrix4 m)
        {
            return new Matrix(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44);
        }
    }
}
