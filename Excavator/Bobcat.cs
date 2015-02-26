//#define Laptop

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using SamSeifert.GLE.CadViewer;
using SamSeifert.HeadTracker;

namespace Excavator
{
    internal static class Bobcat
    {
        internal const float dimOffsetCabY_Inches = 31.0f;
        internal const float dimOffsetSwing2Y_Inches = 1.36f;
        internal static readonly Vector3 vecOffsetCab_Inches = new Vector3(0, Bobcat.dimOffsetCabY_Inches, 0);
        internal static readonly Vector3 vecOffsetSwing1_Inches = new Vector3(-1.7f, 0, 0.3f);
        internal static readonly Vector3 vecOffsetSwing2_Inches = new Vector3(1.31f, Bobcat.dimOffsetSwing2Y_Inches, -24.41f);
        internal static readonly Vector3 vecOffsetSwing3_Inches = new Vector3(a1 - 1, 5, 2);
        internal static readonly Vector3 vecOffsetSwing3Rot_Inches = new Vector3(vecOffsetSwing3_Inches.Z, vecOffsetSwing3_Inches.Y, -vecOffsetSwing3_Inches.X);


//        internal static readonly Vector3 _
        
        private static readonly Color _ColorPiston = Color.DimGray;
        private static readonly Color _ColorCylinder = Color.LightGray;

        internal static readonly Vector3 _TV_V3_Lift = new Vector3(-10.3f, 28.24f, -18.10f + Bobcat._TV_ZOFF);
        
#if Laptop
        internal const bool _BoolLaptop = true;
        internal const float _TV_ZOFF = 10; 
        internal const float _TV_Width_Inches = 10.625f * (16.0f / 9.0f);
        internal const float _TV_Height_Inches = 10.625f;
#else
        internal const bool _BoolLaptop = false;
        internal const float _TV_ZOFF = 0; 
        internal const float _TV_Width_Inches = 24.2679f;
        internal const float _TV_Height_Inches = 43.14f;
#endif

        internal const float _TV_Width_Inches2 = Bobcat._TV_Width_Inches / 2;
        internal const float _TV_Height_Inches2 = Bobcat._TV_Height_Inches / 2;

        internal const float _Float_Head_Tracker_Ball_Size = 1;








        public static Vector3 _SensorLocationTV = new Vector3();
        public static Vector3 _SensorDirectionTV = new Vector3();

        public static Vector3 convertToCatFromTV(Vector3 v) // Not Pass By Reference
        {
            v.X += Bobcat._TV_V3_Lift.X;
            v.Y += Bobcat._TV_V3_Lift.Y;
            v.Z += Bobcat._TV_V3_Lift.Z;

            return v;
        }

        public static Vector3 convertToRealWorldFromTV(Vector3 v, float cab)
        {
            float sinr = (float)Math.Sin(cab);
            float cosr = (float)Math.Cos(cab);

            Vector3 tv = Bobcat.convertToCatFromTV(v);

            Vector3 ret = new Vector3(
                tv.X * cosr + tv.Z * sinr,
                tv.Y,
                tv.Z * cosr - tv.X * sinr);

            ret.X += Bobcat.vecOffsetCab_Inches.X;
            ret.Y += Bobcat.vecOffsetCab_Inches.Y;
            ret.Z += Bobcat.vecOffsetCab_Inches.Z;

            return ret;
        }
















        internal class BobcatAngles
        {
            internal static BobcatAngles Zero = new BobcatAngles();
            /// <summary>
            /// Copies argument to target
            /// </summary>
            /// <param name="FROM"></param>
            internal void copy(BobcatAngles from)
            {
                this.cab = from.cab;
                this.swi = from.swi;
                this.boo = from.boo;
                this.arm = from.arm;
                this.buc = from.buc;
            }

            internal float cab;
            internal float swi;
            internal float boo;
            internal float arm;
            internal float buc;
        }





        private static CadObject _COArm = null;
        private static CadObject _COArmCyl = null;
        private static CadObject _COArmPiston = null;

        private static CadObject _COBoom = null;
        private static CadObject _COBoomCyl = null;
        private static CadObject _COBoomPiston = null;

        private static CadObject _COBucketSimple = null;

        private static CadObject _COBucket = null;
        private static CadObject _COBucketCyl = null;
        private static CadObject _COBucketPiston = null;

        private static CadObject _COBucketLink = null;
        private static CadObject _COBucketLink2 = null;

        private static CadObject _COSwingFrame = null;

        private static CadObject _COExchange = null;

        private static CadObject _COButt = null;
        private static CadObject _COCab = null;

        private static CadObject _CO_TV = null;
        private static CadObject _CO_Head = null;

        private static CadObject _HeadAll;
        private static CadObject _HT1;
        private static CadObject _HT2;
        private static CadObject _Head;
        private static CadObject _H1;
        private static CadObject _H2;
        private static CadObject _H3;

        private static bool _BoolCadObjectsUninit = true;

        internal static void Draw(
            BobcatAngles ang,
            bool drawCab,
            bool ShadowBufferDraw)
        {
            bool useC = !ShadowBufferDraw;

            if (Bobcat._BoolCadObjectsUninit)
            {
                Bobcat.SetupObjects();
                Bobcat._BoolCadObjectsUninit = false;
            }

            float cab = ang.cab;
            float thetaSwing = ang.swi;
            float theta2 = ang.boo;
            float theta3 = ang.arm;
            float theta4 = ang.buc;


            float temp;
            const float rotatelink = -30;	//bucket link origin angle offset

            float tA1B, rAB, tBA1;				// boom cylinder calculations
            float tC2D, rCD, t2CD;				// stick cylinder calculations
            float Gx, Gy, beta, rFG;		   // bucket cylinder calculations
            float tHFG, tEFH, rEH, tFEH, tFHG;

            //////////////////////////////////////////////////////////////
            /*					Draw Arm								*/
            //////////////////////////////////////////////////////////////

            GL_Handler.PushMatrix();
            GL_Handler.Translate(Bobcat.vecOffsetCab_Inches);
            {
                if (drawCab)
                {
                    GL.Disable(EnableCap.CullFace);
                    Bobcat._COButt.draw(useC);

                    GL_Handler.PushMatrix();
                    {
                        GL_Handler.Rotate(cab, Vector3.UnitY);
                        if (Bobcat._CO_TV._Display)
                        {
                            GL_Handler.PushMatrix();
                            {
                                GL_Handler.Translate(Bobcat._TV_V3_Lift);
                                Bobcat._CO_TV.draw(useC);
                            }
                            GL_Handler.PopMatrix();
                        }

                        GL_Handler.Rotate(90, Vector3.UnitY);
                        Bobcat._COCab.draw(useC);

                        GL.Enable(EnableCap.CullFace);

                        if (Bobcat._CO_Head._Display)
                        {
                            GL_Handler.Rotate(-90, Vector3.UnitY);

                            if (HeadTrackerManager.IsTracking)
                            {
                                if (FormBase._HeadTrackerDraw)
                                {
                                    FormBase.HeadLocationData dat = FormBase._HeadLocationCurrent;

                                    Vector3 v;

                                    v = Bobcat.convertToCatFromTV(dat.EyeM);
                                    GL_Handler.Translate(v);
                                    Bobcat._Head.draw(useC);
                                    GL_Handler.Translate(Vector3.Multiply(v, -1));

                                    v = Bobcat.convertToCatFromTV(dat.Reflect1);
                                    GL_Handler.Translate(v);
                                    Bobcat._H1.draw(useC);
                                    GL_Handler.Translate(Vector3.Multiply(v, -1));

                                    v = Bobcat.convertToCatFromTV(dat.Reflect2);
                                    GL_Handler.Translate(v);
                                    Bobcat._H2.draw(useC);
                                    GL_Handler.Translate(Vector3.Multiply(v, -1));

                                    v = Bobcat.convertToCatFromTV(dat.Reflect3);
                                    GL_Handler.Translate(v);
                                    Bobcat._H3.draw(useC);
                                    GL_Handler.Translate(Vector3.Multiply(v, -1));

                                    v = Bobcat.convertToCatFromTV(Bobcat._SensorLocationTV);
                                    GL_Handler.Translate(v);
                                    Bobcat._HT1.draw(useC);
                                    GL_Handler.Translate(Vector3.Multiply(v, -1));

                                    v = Bobcat.convertToCatFromTV(Bobcat._SensorDirectionTV);
                                    GL_Handler.Translate(v);
                                    Bobcat._HT2.draw(useC);
                                    GL_Handler.Translate(Vector3.Multiply(v, -1));
                                }
                            }
                            else
                            {
                                GL_Handler.Translate(Bobcat.convertToCatFromTV(FormBase._HeadLocationCurrent.EyeM));
                                Bobcat._CO_Head.draw(useC);
                            }
                        }
                    }
                    GL_Handler.PopMatrix();
                }


                GL_Handler.PushMatrix();
                {
                    GL_Handler.Rotate(cab + thetaSwing, Vector3.UnitY);
                    GL_Handler.Translate(Bobcat.vecOffsetSwing1_Inches);
                    GL_Handler.Rotate(-thetaSwing, Vector3.UnitY);
                    GL_Handler.Translate(Bobcat.vecOffsetSwing2_Inches);

                    GL_Handler.PushMatrix();
                    {
                        GL_Handler.Rotate(90, 0, 1, 0);
                        GL_Handler.Rotate(thetaSwing, 0, 1, 0);	// unexplained origin offset in the .slp file
                        GL_Handler.Translate(-33, 0, 0);		// 33 is a fudge factor to adjust for an 
                        Bobcat._COSwingFrame.draw(useC);
                    }
                    GL_Handler.PopMatrix();

                    // draw boom
                    GL_Handler.Rotate(-90, 1, 0, 0);
                    GL_Handler.Rotate(thetaSwing, 0, 0, 1);

                    GL_Handler.Rotate(90, 1, 0, 0);
                    GL_Handler.Rotate(90, 0, 1, 0);
                    GL_Handler.Translate(Bobcat.vecOffsetSwing3_Inches);	//adjust offset of origin with origin of .slp file
                    GL_Handler.Rotate(theta2, 0, 0, 1);
                    Bobcat._COBoom.draw(useC);

                    //GL.Color(1,0,1);
                    //DrawGLObject(boomshadow);

                    //draw boom cylinder
                    GL_Handler.PushMatrix();
                    {
                        tA1B = tB12 + theta2 * pi / 180 + pi - t01A;
                        rAB = (float)Math.Sqrt(r1A * r1A + r1B * r1B - 2 * r1A * r1B * Math.Cos(tA1B));
                        tBA1 = (float)Math.Acos((r1A * r1A + rAB * rAB - r1B * r1B) / (2 * r1A * rAB));
                        GL_Handler.Rotate((t01A * 180 / pi) - 90 - theta2, 0, 0, 1);
                        GL_Handler.Translate(0, -.5f, 0);	//fudge factors (didn't get the above offset right on?)
                        GL_Handler.Translate(0, -r1A, 0);
                        GL_Handler.Rotate(180 + 180 - t01A * 180 / pi, 0, 0, 1);
                        GL_Handler.Rotate((-t01A + tBA1) * 180 / pi, 0, 0, -1);
                        Bobcat._COBoomCyl.draw(useC);
                        GL_Handler.Translate(0, rAB + 2, 0);	//another offset
                        GL_Handler.Rotate(180, 0, 0, 1);
                        Bobcat._COBoomPiston.draw(useC);
                    }
                    GL_Handler.PopMatrix();

                    // draw stick
                    GL_Handler.Translate(LensBoom, 0, 0);
                    GL_Handler.Rotate(theta3, 0, 0, 1);
                    Bobcat._COArm.draw(useC);

                    //GL.Color(0,1,0);
                    //DrawGLObject(armshadow);		

                    // draw stick cylinder
                    GL_Handler.PushMatrix();
                    {
                        tC2D = pi - t12C - tD23 - theta3 * pi / 180;
                        rCD = (float)Math.Sqrt(r2C * r2C + r2D * r2D - 2 * r2C * r2D * Math.Cos(tC2D));
                        t2CD = (float)Math.Acos((r2C * r2C + rCD * rCD - r2D * r2D) / (2 * r2C * rCD));
                        GL_Handler.Rotate(-theta3, 0, 0, 1);
                        GL_Handler.Rotate(90, 0, 0, 1);
                        GL_Handler.Rotate(t12C * 180 / pi, 0, 0, -1);
                        GL_Handler.Translate(0, r2C, 0);
                        GL_Handler.Rotate(180 + t2CD * 180 / pi, 0, 0, 1);
                        Bobcat._COArmCyl.draw(useC);
                        GL_Handler.Translate(0, rCD, 0);
                        GL_Handler.Rotate(180, 0, 0, 1);
                        Bobcat._COArmPiston.draw(useC);
                    }
                    GL_Handler.PopMatrix();

                    GL_Handler.Translate(LensArm, 0, 0);
                    //display bucket
                    GL_Handler.PushMatrix();
                    {
                        GL_Handler.Rotate(theta4, 0, 0, 1);

                        GL_Handler.PushMatrix();
                        {
                            GL_Handler.Rotate(20, 0, 0, 1);
                            GL_Handler.Translate(-0.771f, 2.11f - 3.67f, 0);	//correct for the slp origin
                            display_bucket_dirt(/*data*/);
                        }
                        GL_Handler.PopMatrix();

                        GL_Handler.Rotate(135 - 15, 0, 0, 1);		//135-18.9 (about) is the bucket origin angle offset in the slp file
                        GL_Handler.PushMatrix();
                        {
                            GL_Handler.Translate(8.461f, 1.768f, 0);
                            Bobcat._COExchange.draw(useC);
                        }
                        GL_Handler.PopMatrix();

                        GL_Handler.Rotate(35, 0, 0, 1);
                        GL_Handler.Translate(slidex / 2 - 0.365f, slidey / 2 + 2.11f, 0);	//correct for the slp origin

                        Bobcat._COBucket.draw(useC);
//                        Bobcat._COBucketSimple.draw(useC);
//
                    }
                    GL_Handler.PopMatrix();

                    // draw bucket cylinder
                    GL_Handler.PushMatrix();		//for some reason, the compiler didn't like long algebraic statements
                    {
                        //Gx = r3G*cos(theta4*pi/180+tG34);
                        Gx = theta4 * pi / 180 + tG34;
                        Gx = (float)(r3G * Math.Cos(Gx));
                        Gx = Gx + r3I;

                        //Gy = r3G*sin(theta4*pi/180+tG34);
                        Gy = theta4 * pi / 180 + tG34;
                        Gy = (float)(r3G * Math.Sin(Gy));
                        Gy = Gy - rFI;

                        //beta = atan2((Gy-rFI),(Gx+r3I));
                        beta = (float)Math.Atan2(Gy, Gx);

                        //rFG = sqrt((Gy-rFI)*(Gy-rFI) + (Gx+r3I)*(Gx+r3I));
                        rFG = (float)Math.Sqrt(Gy * Gy + Gx * Gx);
                        tHFG = (float)Math.Acos((rFH * rFH + rFG * rFG - rGH * rGH) / (2 * rFH * rFG));

                        //tEFH = pi-alpha-beta-tHFG;
                        tEFH = pi - alfa - beta;
                        tEFH = tEFH - tHFG;

                        //rEH = sqrt(rEF*rEF + rFH*rFH - 2*rEF*rFH*cos(tEFH));
                        rEH = rEF * rEF + rFH * rFH;
                        rEH = (float)(rEH - 2 * rEF * rFH * Math.Cos(tEFH));
                        rEH = (float)Math.Sqrt(rEH);

                        //tFEH = acos((rEH*rEH + rEF*rEF - rFH*rFH)/(2*rEH*rEF));
                        tFEH = rEH * rEH + rEF * rEF;
                        tFEH = tFEH - rFH * rFH;
                        tFEH = (float)Math.Acos(tFEH / (2 * rEH * rEF));
                        tFHG = (float)Math.Acos((rFH * rFH + rGH * rGH - rFG * rFG) / (2 * rFH * rGH));

                        GL_Handler.Translate(-r3I, -rFI + 1, 0); //1 is a fudge factor

                        GL_Handler.PushMatrix();
                        {
                            GL_Handler.Rotate(-90, 0, 0, 1);
                            //temp = 180-alpha*180/pi;
                            temp = -alfa;
                            temp = 180 + temp * 180 / pi;
                            GL_Handler.Rotate(temp, 0, 0, 1);
                            GL_Handler.Translate(0, rEF, 0);
                            GL_Handler.Rotate(180 + tFEH * 180 / pi, 0, 0, 1);
                            Bobcat._COBucketCyl.draw(useC);
                            GL_Handler.Translate(0, rEH, 0);
                            GL_Handler.Rotate(180, 0, 0, 1);
                            Bobcat._COBucketPiston.draw(useC);
                        }

                        GL_Handler.PopMatrix();

                        GL_Handler.Rotate((beta + tHFG) * 180 / pi, 0, 0, 1);
                        Bobcat._COBucketLink2.draw(useC);
                        GL_Handler.Translate(rFH, 0, 0);
                        GL_Handler.Rotate(tFHG * 180 / pi, 0, 0, 1);
                        GL_Handler.Rotate(90, 0, 1, 0);
                        GL_Handler.Rotate(rotatelink, 1, 0, 0);
                        GL_Handler.Translate(3.1f, 0, 0);
                        Bobcat._COBucketLink.draw(useC);
                    }
                    GL_Handler.PopMatrix();
                }
                GL_Handler.PopMatrix();
            }
            GL_Handler.PopMatrix();

            if (ShadowBufferDraw) GL.MatrixMode(MatrixMode.Modelview);
        }




        private static float[] _ColorCylA = new float[] { 0.30f, 0.30f, 0.30f, 1.00f };
        private static float[] _ColorCylD = new float[] { 1.00f, 1.00f, 1.00f, 1.00f };
        private static float[] _ColorCylE = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorCylS = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorCylH = new float[] { 0.00f };

        private static float[] _ColorPistonA = new float[] { 0.30f, 0.15f, 0.05f, 1.00f };
        private static float[] _ColorPistonD = new float[] { 0.60f, 0.30f, 0.10f, 1.00f };
        private static float[] _ColorPistonE = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorPistonS = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorPistonH = new float[] { 0.00f };

        private static float[] _ColorBoomA = new float[] { 0.00f, 0.05f, 0.10f, 1.00f };
        private static float[] _ColorBoomD = new float[] { 0.00f, 0.10f, 0.20f, 1.00f };
        private static float[] _ColorBoomE = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorBoomS = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorBoomH = new float[] { 0.00f };

        private static float[] _ColorArmA = new float[] { 0.50f, 0.50f, 0.40f, 1.00f };
        private static float[] _ColorArmD = new float[] { 0.50f, 0.50f, 0.30f, 1.00f };
        private static float[] _ColorArmE = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorArmS = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorArmH = new float[] { 0.00f };

        private static float[] _ColorSwingFrameA = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorSwingFrameD = new float[] { 0.80f, 0.80f, 0.80f, 1.00f };
        private static float[] _ColorSwingFrameE = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorSwingFrameS = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorSwingFrameH = new float[] { 0.00f };

        private static float[] _ColorBucketA = new float[] { 0.05f, 0.05f, 0.05f, 1.00f };
        private static float[] _ColorBucketD = new float[] { 0.50f, 0.50f, 0.50f, 1.00f };
        private static float[] _ColorBucketE = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorBucketS = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorBucketH = new float[] { 0.00f };

        private static float[] _ColorExchangeA = new float[] { 0.00f, 0.10f, 0.25f, 1.00f };
        private static float[] _ColorExchangeD = new float[] { 0.00f, 0.40f, 0.50f, 1.00f };
        private static float[] _ColorExchangeE = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorExchangeS = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorExchangeH = new float[] { 0.00f };

        private static float[] _ColorBucketLink2A = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorBucketLink2D = new float[] { 0.25f, 0.25f, 0.25f, 1.00f };
        private static float[] _ColorBucketLink2E = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorBucketLink2S = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorBucketLink2H = new float[] { 0.00f };

        private static float[] _ColorButtA = new float[] { 0.03f, 0.03f, 0.03f, 1.00f };
        private static float[] _ColorButtD = new float[] { 0.10f, 0.10f, 0.10f, 1.00f };
        private static float[] _ColorButtE = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorButtS = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorButtH = new float[] { 0.00f };

        private static float[] _ColorCabA = new float[] { 0.20f, 0.20f, 0.00f, 1.00f };
        private static float[] _ColorCabD = new float[] { 0.70f, 0.70f, 0.00f, 1.00f };
        private static float[] _ColorCabE = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorCabS = new float[] { 0.00f, 0.00f, 0.00f, 1.00f };
        private static float[] _ColorCabH = new float[] { 0.00f };

        private static void SetupObjects()
        {
            const float cyl = 1.5f;

            Bobcat._COArm = CadObjectGenerator.fromXAML(Properties.Resources.Arm, ObjectName: "Arm");
            Bobcat._COArmCyl = CadObjectGenerator.fromXAML(Properties.Resources.ArmCyl, ObjectName: "Arm Cylinder");
            Bobcat._COArmPiston = CadObjectGenerator.fromXAML(Properties.Resources.ArmPiston, ObjectName: "Arm Piston", yScale: cyl);

            Bobcat._COBoom = CadObjectGenerator.fromXAML(Properties.Resources.Boom, ObjectName: "Boom");
            Bobcat._COBoomCyl = CadObjectGenerator.fromXAML(Properties.Resources.BoomCyl, ObjectName: "Boom Cylinder");
            Bobcat._COBoomPiston = CadObjectGenerator.fromXAML(Properties.Resources.BoomPiston, ObjectName: "Boom Piston", yScale: cyl);

            Bobcat._COBucketSimple = CadObjectGenerator.fromXAML(Properties.Resources.BucketSimple, ObjectName: "Bucket Simple");

            Bobcat._COBucket = CadObjectGenerator.fromXAML(Properties.Resources.Bucket, ObjectName: "Bucket");
            Bobcat._COBucketCyl = CadObjectGenerator.fromXAML(Properties.Resources.BucketCyl, ObjectName: "Bucket Cylinder");
            Bobcat._COBucketPiston = CadObjectGenerator.fromXAML(Properties.Resources.BucketPiston, ObjectName: "Bucket Piston", yScale: cyl);

            Bobcat._COBucketLink = CadObjectGenerator.fromXAML(Properties.Resources.BucketLink, ObjectName: "Bucket Link");
            Bobcat._COBucketLink._Display = false;
            Bobcat._COBucketLink2 = CadObjectGenerator.fromXAML(Properties.Resources.BucketLink2, ObjectName: "Bucket Link 2");

            Bobcat._COSwingFrame = CadObjectGenerator.fromXAML(Properties.Resources.SwingFrame, ObjectName: "Swing Frame");

            Bobcat._COExchange = CadObjectGenerator.fromXAML(Properties.Resources.Exchange, ObjectName: "Exchange");

/*            float sc = 30.48f; // 30.48 cm in a foot
            //float sc = 39.37f; // 39.37 inches in a meter

            Bobcat._COCab = CadObjectGenerator.fromXAML(
                Properties.Resources.Dull_Cab,
                ObjectName:"Cab",
                xScale:sc,
                yScale:sc,
                zScale:sc,
                useAmbient:true,
                useDiffuse:true,
                useSpecular:false,
                useEmission:false);
            Bobcat._COButt = CadObjectGenerator.fromXAML(Properties.Resources.Dull_Butt, "Butt", sc, sc, sc);*/

            float sc = 39.37f / 30.48f;
            sc = 1.0f;
            Bobcat._COCab = CadObjectGenerator.fromXAML(Properties.Resources.Dull_Cab,
                ObjectName: "Cab", 
                xScale:sc,
                yScale:sc,
                zScale:sc);
            Bobcat._COButt = CadObjectGenerator.fromXAML(Properties.Resources.Dull_Butt, ObjectName: "Butt");

            const float r = Bobcat._Float_Head_Tracker_Ball_Size;

            Bobcat.SetupHeadAndTV();

            Bobcat._H1 = CadObjectGenerator.CreateSphere(Vector3.Zero, 1, Color.Red.ToArgb());
            Bobcat._H2 = CadObjectGenerator.CreateSphere(Vector3.Zero, 1, Color.Green.ToArgb());
            Bobcat._H3 = CadObjectGenerator.CreateSphere(Vector3.Zero, 1, Color.Blue.ToArgb());
            Bobcat._HT1 = CadObjectGenerator.CreateSphere(Vector3.Zero, 1, Color.Orange.ToArgb());
            Bobcat._HT2 = CadObjectGenerator.CreateSphere(Vector3.Zero, 1, Color.Yellow.ToArgb());
            Bobcat._Head = CadObjectGenerator.CreateSphere(Vector3.Zero, 1, Color.Pink.ToArgb());
            Bobcat._HeadAll = new CadObject(new CadObject[]
            {
                Bobcat._H1,
                Bobcat._H2,
                Bobcat._H3,
                Bobcat._Head,
                Bobcat._HT1,
                Bobcat._HT2
            }, "Head Tracker Setup");

            FormBase._FormBase.addCadItems(Bobcat.objects());

            foreach (var co in new CadObject[] {
                    Bobcat._COArmCyl,
                    Bobcat._COBoomCyl,
                    Bobcat._COBucketCyl,
                    Bobcat._CO_Head})
                co.setColor(
                    Bobcat._ColorCylA,
                    Bobcat._ColorCylD,
                    Bobcat._ColorCylE,
                    Bobcat._ColorCylS,
                    Bobcat._ColorCylH
                );

            foreach (var co in new CadObject[] {
                    Bobcat._COArmPiston,
                    Bobcat._COBoomPiston,
                    Bobcat._COBucketPiston,
                    Bobcat._CO_TV})
                co.setColor(
                    Bobcat._ColorPistonA,
                    Bobcat._ColorPistonD,
                    Bobcat._ColorPistonE,
                    Bobcat._ColorPistonS,
                    Bobcat._ColorPistonH
                );

            foreach (var co in new CadObject[] {
                    Bobcat._COBucket,
                    Bobcat._COBucketSimple})
                co.setColor(
                    Bobcat._ColorBucketA,
                    Bobcat._ColorBucketD,
                    Bobcat._ColorBucketE,
                    Bobcat._ColorBucketS,
                    Bobcat._ColorBucketH
                    );

            Bobcat._COSwingFrame.setColor(
                Bobcat._ColorSwingFrameA,
                Bobcat._ColorSwingFrameD,
                Bobcat._ColorSwingFrameE,
                Bobcat._ColorSwingFrameS,
                Bobcat._ColorSwingFrameH
            );

            Bobcat._COBoom.setColor(
                Bobcat._ColorBoomA,
                Bobcat._ColorBoomD,
                Bobcat._ColorBoomE,
                Bobcat._ColorBoomS,
                Bobcat._ColorBoomH
            );

            Bobcat._COArm.setColor(
                Bobcat._ColorArmA,
                Bobcat._ColorArmD,
                Bobcat._ColorArmE,
                Bobcat._ColorArmS,
                Bobcat._ColorArmH
            );

            Bobcat._COBucketLink2.setColor(
                Bobcat._ColorBucketLink2A,
                Bobcat._ColorBucketLink2D,
                Bobcat._ColorBucketLink2E,
                Bobcat._ColorBucketLink2S,
                Bobcat._ColorBucketLink2H
                );

            Bobcat._COExchange.setColor(
                Bobcat._ColorExchangeA,
                Bobcat._ColorExchangeD,
                Bobcat._ColorExchangeE,
                Bobcat._ColorExchangeS,
                Bobcat._ColorExchangeH
                );

            Bobcat._COButt.setColor(
                Bobcat._ColorButtA,
                Bobcat._ColorButtD,
                Bobcat._ColorButtE,
                Bobcat._ColorButtS,
                Bobcat._ColorButtH
                );

            Bobcat._COCab.setColor(
                Bobcat._ColorCabA,
                Bobcat._ColorCabD,
                Bobcat._ColorCabE,
                Bobcat._ColorCabS,
                Bobcat._ColorCabH
                );

        }





        private static void SetupHeadAndTV()
        {
            Bobcat._CO_TV = CadObjectGenerator.CreateFace(
                new Vector3(-Bobcat._TV_Width_Inches2, -Bobcat._TV_Height_Inches2, 0),
                new Vector3(-Bobcat._TV_Width_Inches2,  Bobcat._TV_Height_Inches2, 0),
                new Vector3( Bobcat._TV_Width_Inches2,  Bobcat._TV_Height_Inches2, 0),
                new Vector3( Bobcat._TV_Width_Inches2, -Bobcat._TV_Height_Inches2, 0),
                new Vector3(0, 1, 0), "TV"); 
            
            Bobcat._CO_TV._Display = false;

            Bobcat._CO_Head = CadObjectGenerator.CreateSphere(Vector3.Zero, 3, Color.White.ToArgb(), 6, "Head");
        }











        internal static void GLDelete()
        {
            foreach (CadObject ob in Bobcat.objects()) if(ob != null) ob.GLDelete();
        }

        private static CadObject[] objects()
        {
            return new CadObject[]
            {
                    Bobcat._COArm,
                    Bobcat._COArmCyl,
                    Bobcat._COArmPiston,

                    Bobcat._COBoom,
                    Bobcat._COBoomCyl,
                    Bobcat._COBoomPiston,
                    
                    Bobcat._COBucketSimple,
                    
                    Bobcat._COBucket,
                    Bobcat._COBucketCyl,
                    Bobcat._COBucketPiston,
                    
                    Bobcat._COBucketLink,
                    Bobcat._COBucketLink2,
                    
                    Bobcat._COSwingFrame,
                    
                    Bobcat._COExchange,

                    Bobcat._COButt,
                    Bobcat._COCab,

                    Bobcat._CO_TV,
                    Bobcat._CO_Head,

                    Bobcat._HeadAll
            };
        }














































































































        static readonly float pi = (float)Math.PI;

        const float a1 = 5.60224f;
        const float LensBoom = 104.0f;
        const float LensArm = 54.0f;
        const float a4 = 31.33f;

        const float r01 = a1;
        const float r12 = LensBoom;
        const float r23 = LensArm;
        const float r34 = a4;

        const float ro1 = 6.76942f;
        const float roA = 15.6884f;
        const float r1A = 14.8486f;
        const float r1B = 54.4949f;
        const float r1C = 69.6644f;
        const float r2B = 54.4949f;
        const float r2C = 53.1519f;
        const float r2D = 14.8011f;
        const float r2E = 13.5656f;
        const float r3D = 66.3633f;
        const float r3E = 49.7071f;
        const float r3F = 9.05512f;
        const float r3G = 8.65592f;
        const float rEF = 40.8434f;
        const float rFH = 12.5984f;
        const float rFI = 0.576462f;
        static readonly float r3I = (float)Math.Sqrt(-(rFI * rFI) + (r3F * r3F));

        const float rGH = 12.3942f;
        const float r4G = 35.9193f;

        static readonly float t01o = (float)Math.Acos(r01 / ro1);
        static readonly float to1A = (float)Math.Acos((ro1 * ro1 + r1A * r1A - roA * roA) / (2 * ro1 * r1A));

        static readonly float t01A = (t01o + to1A);
        static readonly float t12C = (float)Math.Acos((r12 * r12 + r2C * r2C - r1C * r1C) / (2 * r12 * r2C));
        static readonly float t23D = (float)Math.Acos((r23 * r23 + r3D * r3D - r2D * r2D) / (2 * r23 * r3D));
        static readonly float tB12 = (float)Math.Acos((r1B * r1B + r12 * r12 - r2B * r2B) / (2 * r1B * r12));
        static readonly float tD23 = (float)Math.Acos((r2D * r2D + r23 * r23 - r3D * r3D) / (2 * r2D * r23));
        static readonly float tG34 = (float)Math.Acos((r3G * r3G + r34 * r34 - r4G * r4G) / (2 * r3G * r34));
        static readonly float t23E = (float)Math.Acos((r23 * r23 + r3E * r3E - r2E * r2E) / (2 * r23 * r3E));
        static readonly float alfa = StaticMethods.toRadiansF(16.6878f);
        static readonly float t1min = StaticMethods.toRadiansF(-50);
        static readonly float t1max = StaticMethods.toRadiansF(90);

        static readonly float t2max = StaticMethods.toRadiansF(66);
        static readonly float t2min = StaticMethods.toRadiansF(-63);
        static readonly float t3max = StaticMethods.toRadiansF(-22);
        static readonly float t3min = StaticMethods.toRadiansF(-145);

        static readonly float dmax = (float)(LensBoom + LensArm * Math.Cos(t3max));
        static readonly float dmin = (float)(LensBoom + LensArm * Math.Cos(t3min));

        static readonly float t312min = (float)Math.Atan((LensArm * (-Math.Sin(t3max))) / (LensBoom + LensArm * Math.Cos(t3max)));
        static readonly float t312max = StaticMethods.toRadiansF(27.5f);
        static readonly float Ymin4 = StaticMethods.toRadiansF(23.375f);
        static readonly float Ymax4 = StaticMethods.toRadiansF(37.125f);
        static readonly float t4max = StaticMethods.toRadiansF(30);
        static readonly float t4min = StaticMethods.toRadiansF(-153);
        static readonly float t3min_rmin_t2min = StaticMethods.toRadiansF(-150);
        static readonly float zmax_rmin_t2min = (float)(LensBoom * Math.Sin(t2min) + LensArm * Math.Sin(t2min + t3min));
        static readonly float zmin_rmin_t3min = (float)(LensBoom * Math.Sin(t2min) + LensArm * Math.Sin(t2min + t3max));
        static readonly float ztop = (float)(LensBoom * Math.Sin(t2max) + LensArm * Math.Sin(t2max + t3max));
        static readonly float zbottom = (float)(LensBoom * Math.Sin(t2min) + LensArm * Math.Sin(t2min + t3min));
        static readonly float rtop = (float)(a1 + LensBoom * Math.Cos(t2max) + LensArm * Math.Cos(t2max + t3max));
        static readonly float rbottom = (float)(a1 + LensBoom * Math.Cos(t2min) + LensArm * Math.Cos(t2min + t3min));


        // Sampling Time for Entire Simulation
        const float Ts = 0.001f;

        //1 cc  0.0610237441 cubic inches
        const float cc_2_in3 = 0.0610237441f;
        const float maxflow_per_rev = (18*cc_2_in3); //18cc/rev
        const float  motorspeed = (3442.0f/60.0f); //rpm -> rps
        const float maxpumpflow = (maxflow_per_rev * motorspeed);
        static readonly float[] maxPumpFlowArray = new float[] { 45.7678f, 63.0131f, 63.0131f, 63.0131f };








        const float tau = 13.3333f; // Taken From LoadVars.m

        
        const float a1_sim = 31.5423f;
        public const float a4_sim = 34.6133f; //34.6593
        const float d1 = -1.68f;
        public static readonly float q2 = (float)(-pi / 2 + Math.Acos(d1 / a1_sim));


        /// <summary>
        /// Defined by Elton in in J2T_435.c
        /// Used in Soil Model
        /// Takes Joint Angles and Gives Poistions??? I think
        /// </summary>
        /// <param name="Ybh"></param>
        /// <param name="Vbh"></param>
        /// <param name="Q"></param>
        /// <param name="QD"></param>
        public static void JointToTask435(ref float[] Ybh, ref float[] Vbh, ref float[] Q, ref float[] QD)
        {
            /************************************************************
                        variable and parameters definitions
            ************************************************************/
            float x, y, z, phi;
            float x2, x3, x4, y2, y3, y4, z3, z4;
            float t1, t2, t3, t4;
            float w1, w2, w3, w4;
            float vx, vy, vz, vphi; // float vr, vtheta, wx, wy, wz;
            float c1, c12, c3, c34, c345;
            float s1, s12, s3, s34, s345;
            float J11, J12, J13, J14, J21, J22, J23, J24, J31, J33, J34; // float J15, J25, J32, J35, J41, J42, J43, J44, J45, J51, J52, J53, J54, J55;
            float q1, q3, q4, q5;
            float qd1, qd3, qd4, qd5; // float qd2;

            /************************************************************
                        Joint-Space Input Variables
            /************************************************************
                    x = task space x, y, z and phi (bucket angle) (in)
                    v = task space velocity (in/s)
                    t = theta = joint angle (rad)
                    w = omega = joint velocity (rad/s)
            ************************************************************/

            t1 = Q[0];
            t2 = Q[1];
            t3 = Q[2];
            t4 = Q[3];
            w1 = QD[0];
            w2 = QD[1];
            w3 = QD[2];
            w4 = QD[3];

            q1 = t1 - Bobcat.q2;
            q3 = t2;
            q4 = t3;
            q5 = t4;

            qd1 = w1;
            //            qd2 = 0;
            qd3 = w2;
            qd4 = w3;
            qd5 = w4;

            ///************************************************************
            //			Trig/Link Length Calculations
            //************************************************************/ 

            c1 = (float)Math.Cos(q1);
            c3 = (float)Math.Cos(q3);
            c12 = (float)Math.Cos(q1 + Bobcat.q2);
            c34 = (float)Math.Cos(q3 + q4);
            c345 = (float)Math.Cos(q3 + q4 + q5);

            s1 = (float)Math.Sin(q1);
            s3 = (float)Math.Sin(q3);
            s12 = (float)Math.Sin(q1 + Bobcat.q2);
            s34 = (float)Math.Sin(q3 + q4);
            s345 = (float)Math.Sin(q3 + q4 + q5);

            ///************************************************************
            //			Position Calculations
            //************************************************************/
            //    p02 = [Bobcat.a1_sim*c1-d1*s1 Bobcat.a1_sim*s1+d1*c1 0];
            //    p03 = p02 + [Bobcat.a2*c2*c1 Bobcat.a2*c2*s1 Bobcat.a2*s2];
            //    p04 = p03 + [Bobcat.a3*(c23)*c1 Bobcat.a3*(c23)*s1 Bobcat.a3*(s23)];
            //    //p0tip = p03 + [Bobcat.a1_sim*(c234)*c1 Bobcat.a1_sim*(c234)*s1 Bobcat.a1_sim*(s234)];

            x2 = (Bobcat.a1_sim * c1 + Bobcat.a1 * c12);
            y2 = (Bobcat.a1_sim * s1 + Bobcat.a1 * s12);

            x3 = x2 + Bobcat.LensBoom * c3 * c12;
            y3 = y2 + Bobcat.LensBoom * c3 * s12;
            z3 = Bobcat.LensBoom * s3;

            x4 = x3 + Bobcat.LensArm * c34 * c12;
            y4 = y3 + Bobcat.LensArm * c34 * s12;
            z4 = z3 + Bobcat.LensArm * s34;

            x = x4 + Bobcat.a1_sim * c345 * c12;
            y = y4 + Bobcat.a1_sim * c345 * s12;
            z = z4 + Bobcat.a1_sim * s345;
            phi = t4;

            q1 = q1 + Bobcat.q2;
            c1 = (float)Math.Cos(q1);
            c12 = (float)Math.Cos(q1 + Bobcat.q2);
            s1 = (float)Math.Sin(q1);
            s12 = (float)Math.Sin(q1 + Bobcat.q2);

            // 	/************************************************************
            // 				Velocity Calculations
            // 	*************************************************************
            // 			V = J(th)*omega
            // 	************************************************************/
            //  cartesian jacobian
            //     J11 = -Bobcat.a1_sim*s1-Bobcat.a1*s12-Bobcat.a2*c3*s12-Bobcat.a3*c34*s12-Bobcat.a1_sim*c345*s12;
            //     J12 = -Bobcat.a1*s12-Bobcat.a2*c3*s12-Bobcat.a3*c34*s12-Bobcat.a1_sim*c345*s12;
            //     J13 = -c12*(Bobcat.a2*s3+Bobcat.a3*s34+Bobcat.a1_sim*s345);
            //     J14 = -c12*(Bobcat.a3*s34+Bobcat.a1_sim*s345);
            //     J15 = -c12*Bobcat.a1_sim*s345;
            //     J21 = Bobcat.a1_sim*c1+Bobcat.a1*c12+Bobcat.a2*c3*c12+Bobcat.a3*c34*c12+Bobcat.a1_sim*c345*c12;
            //     J22 = Bobcat.a1*c12+Bobcat.a2*c3*c12+Bobcat.a3*c34*c12+Bobcat.a1_sim*c345*c12;
            //     J23 = -s12*(Bobcat.a2*s3+Bobcat.a3*s34+Bobcat.a1_sim*s345);
            //     J24 = -s12*(Bobcat.a3*s34+Bobcat.a1_sim*s345);
            //     J25 = -s12*Bobcat.a1_sim*s345;
            //     J31 = 0;
            //     J32 = 0;
            //     J33 = s12*(Bobcat.a2*c3*s12+Bobcat.a3*c34*s12+Bobcat.a1_sim*c345*s12)+c12*(Bobcat.a2*c3*c12+Bobcat.a3*c34*c12+Bobcat.a1_sim*c345*c12);
            //     J34 = s12*(Bobcat.a3*c34*s12+Bobcat.a1_sim*c345*s12)+c12*(Bobcat.a3*c34*c12+Bobcat.a1_sim*c345*c12);
            //     J35 = s12*s12*Bobcat.a1_sim*c345+c12*c12*Bobcat.a1_sim*c345;
            //     J41 = 0;
            //     J42 = 0;
            //     J43 = s12;
            //     J44 = s12;
            //     J45 = s12;
            //     J51 = 0;
            //     J52 = 0;
            //     J53 = -c12;
            //     J54 = -c12;
            //     J55 = -c12;
            // 
            //     vx = J11*qd1 + J12*qd2 + J13*qd3 + J14*qd4 + J15*qd5;
            //     vy = J21*qd1 + J22*qd2 + J23*qd3 + J24*qd4 + J25*qd5;
            //     vz = J31*qd1 + J32*qd2 + J33*qd3 + J34*qd4 + J35*qd5;
            //     wx = J41*qd1 + J42*qd2 + J43*qd3 + J44*qd4 + J45*qd5;
            //     wy = J51*qd1 + J52*qd2 + J53*qd3 + J54*qd4 + J55*qd5;
            //     wz = qd1;
            //     vphi = w4;

            J11 = -Bobcat.a1_sim * s1 - Bobcat.a1 * s12 - Bobcat.LensBoom * c3 * s12 - Bobcat.LensArm * c34 * s12 - Bobcat.a1_sim * c345 * s12;
            J12 = -Bobcat.a1 * s12 - Bobcat.LensBoom * c3 * s12 - Bobcat.LensArm * c34 * s12 - Bobcat.a1_sim * c345 * s12;
            J13 = -c12 * (Bobcat.LensBoom * s3 + Bobcat.LensArm * s34 + Bobcat.a1_sim * s345);
            J14 = -c12 * (Bobcat.LensArm * s34 + Bobcat.a1_sim * s345);
            //J15 = -c12*Bobcat.a1_sim*s345;
            J21 = Bobcat.a1_sim * c1 + Bobcat.a1 * c12 + Bobcat.LensBoom * c3 * c12 + Bobcat.LensArm * c34 * c12 + Bobcat.a1_sim * c345 * c12;
            J22 = Bobcat.a1 * c12 + Bobcat.LensBoom * c3 * c12 + Bobcat.LensArm * c34 * c12 + Bobcat.a1_sim * c345 * c12;
            J23 = -s12 * (Bobcat.LensBoom * s3 + Bobcat.LensArm * s34 + Bobcat.a1_sim * s345);
            J24 = -s12 * (Bobcat.LensArm * s34 + Bobcat.a1_sim * s345);
            //J25 = -s12*Bobcat.a1_sim*s345;
            J31 = 0;
            //            J32 = 0;
            J33 = s12 * (Bobcat.LensBoom * c3 * s12 + Bobcat.LensArm * c34 * s12 + Bobcat.a1_sim * c345 * s12) + c12 * (Bobcat.LensBoom * c3 * c12 + Bobcat.LensArm * c34 * c12 + Bobcat.a1_sim * c345 * c12);
            J34 = s12 * (Bobcat.LensArm * c34 * s12 + Bobcat.a1_sim * c345 * s12) + c12 * (Bobcat.LensArm * c34 * c12 + Bobcat.a1_sim * c345 * c12);
            //J35 = Bobcat.a1_sim*c345;//s12^2*Bobcat.a1_sim*c345+c12^2*Bobcat.a1_sim*c345;
            // J41 = 0;
            // J42 = 0;
            // J43 = s12;
            // J44 = s12;
            // J45 = s12;
            // J51 = 0;
            // J52 = 0;
            // J53 = -c12;
            // J54 = -c12;
            // J55 = -c12;

            vx = J11 * qd1 + J13 * qd3 + J14 * qd4;
            vy = J21 * qd1 + J23 * qd3 + J24 * qd4;
            vz = J31 * qd1 + J33 * qd3 + J34 * qd4;
            vphi = qd5;

            //tau = jacob'*force;  
            //vel = jacob*omega;

            /************************************************************
                        Cylinder-Space Output Variables
            ************************************************************/
            Ybh[0] = x4;
            Ybh[1] = y4;
            Ybh[2] = z4;
            Ybh[3] = phi;
            Vbh[0] = vx;
            Vbh[1] = vy;
            Vbh[2] = vz;
            Vbh[3] = vphi;
        }

















        /// <summary>
        /// Defined by Elton in in j2c_435.c
        /// Used in inverse kinematics
        /// Takes Joint Angles and Cylinder Poistions??? I think
        /// Cab angles don't have cylinders so inputs/outputs are special???
        /// </summary>
        /// <param name="Q">4</param>
        /// <param name="Qd">4</param>
        /// <param name="Y">4</param>
        /// <param name="V">4</param>
        /// <param name="C">4</param>
        public static void JointToCylinder(
            int i,
            // Inputs
            ref float[] Q, 
            ref float[] Qd, 
            // Outputs
            ref float[] Y, 
            ref float[] V)
        {

            /************************************************************
                        Joint-Space Input Variables
            /************************************************************
                    t = theta = joint angle	(rad)
                    w = omega = joint velocity (rad/s)
            ************************************************************/

            switch (i)
            {
                case 0: return;
                case 1:
                    {
                        double t2 = Q[1];
                        double w2 = Qd[1];
                        ///************************************************************
                        //				Boom - axis #2
                        //************************************************************/

                        //tA1B = pi - t01A - t2 - tB12;
                        double tA1B = tB12 + t2 + pi - t01A;
                        double y2 = Math.Sqrt(r1A * r1A + r1B * r1B - 2 * r1A * r1B * Math.Cos(tA1B));
                        double C2 = r1A * r1B * Math.Sin(tA1B) / y2;
                        //t1BA = acos((r1A*r1A - r1B*r1B - y2*y2)/(-2*y2*r1B));
                        //C2 = r1B*sin(t1BA);
                        double v2 = C2 * w2;
                        //f2 = T2/C2;
                        Y[1] = (float)y2;
                        V[1] = (float)v2;
                        break;
                    }
                case 2:
                    {
                        double t3 = Q[2];
                        double w3 = Qd[2];
                        ///************************************************************
                        //				Stick - axis #3
                        //************************************************************/
                        //tC2D = pi - t12C - t3 - tD23;
                        //tC2D = pi - t12C - t3 - tD23;
                        double tC2D = pi - t12C - t3 - tD23;
                        double y3 = Math.Sqrt(r2C * r2C + r2D * r2D - 2 * r2C * r2D * Math.Cos(tC2D));
                        double C3 = -r2C * r2D * Math.Sin(tC2D) / y3;
                        double v3 = C3 * w3;
                        //f3 = T3/C3;
                        Y[2] = (float)y3;
                        V[2] = (float)v3;
                        break;
                    }
                case 3:
                    {
                        double w4 = Qd[3];
                        double t4 = Q[3];
                        ///************************************************************
                        //				Bucket - axis #4
                        //************************************************************/
                        double xg = r3G * Math.Cos(t4 + tG34);
                        double yg = r3G * Math.Sin(t4 + tG34);
                        double rFG_2 = (yg - rFI) * (yg - rFI) + (r3I + xg) * (r3I + xg);
                        double rFG = Math.Sqrt(rFG_2);
                        double sBeta = (yg - rFI) / rFG; //sin(beta)
                        double beta = Math.Asin(sBeta); //t3FG?
                        double cGFH = (rFH * rFH + rFG * rFG - rGH * rGH) / (2 * rFH * rFG);
                        double tGFH = Math.Acos(cGFH);
                        double sGFH = Math.Sin(tGFH);
                        //tEFH = pi - tGFH - beta - tGFH;
                        //tEFH = pi - alpha;
                        double tEFH = pi - alfa;
                        tEFH = tEFH - beta - tGFH;
                        double y4 = Math.Sqrt(rEF * rEF + rFH * rFH - 2 * rEF * rFH * Math.Cos(tEFH));
                        double rC = r3G * Math.Cos(t4 + tG34);
                        double rS = r3G * Math.Sin(t4 + tG34);
                        double beta_dot = ((rFG + rFI * sBeta) * rC + r3I * sBeta * rS) / (rFG_2 * Math.Cos(beta));

                        double wGFH = -(rFH * cGFH - rFG) * (rFI * rC + r3I * rS) / (rFH * rFG_2 * sGFH);
                        double wEFH = -beta_dot - wGFH;
                        double C4 = rEF * rFH * Math.Sin(tEFH) / y4 * wEFH;


                        //     wGFH = -(rFH*cGFH - rFG)*(rFI*rC +r3I*rS)/(rFH*rFG_2*sGFH);
                        //     if (w4 != 0)
                        //     {
                        //         wEFH = -beta_dot/w4 - wGFH;
                        //     }
                        //     else
                        //     {
                        //         wEFH = -beta_dot;
                        //     }        
                        // 	C4 = rEF*rFH*sin(tEFH)/y4*wEFH;    

                        double v4 = C4 * w4;
                        Y[3] = (float)y4;
                        V[3] = (float)v4;
                        break;
                    }
            }



            /************************************************************
                        Cylinder-Space Output Variables
            ************************************************************/



            /*
            C[0] = 0;
            C[1] = (float)C2;
            C[2] = (float)C3;
            C[3] = (float)C4;
             * */
        }




        /// <summary>
        /// i cant be zero
        /// </summary>
        /// <param name="i"></param>
        /// <param name="CylinderVelocitiesDesired"></param>
        /// <param name="CylinderVelocities"></param>
        /// <param name="LAST_FLOW"></param>
        /// <param name="FLOW"></param>
        public static void PumpModelVelocity(
            int i,
            // Input
            ref float[] CylinderVelocitiesDesired, 
            ref float[] CylinderVelocities,
            // Output
            ref float[] FLOW
            )
        {
            float[] vgain = new float[] { 0, -10f, -10f, -10f };

            float cmd = vgain[i] * (CylinderVelocities[i] - CylinderVelocitiesDesired[i]);

            cmd /= maxPumpFlowArray[i];

            float fuf = FLOW[i];

            Bobcat.PumpModelFlow(cmd, ref FLOW, i);

            const float alph = 0.90f;
            const float alph1m = 1 - alph;
            FLOW[i] = fuf * alph + FLOW[i] * alph1m;


            /*           
             * 
             *             float pumpflow = LAST_FLOW[i];
            if (pumpflowcmd > maxpumpflow)	// if commanded flow is greater than the maximum
                        {
                            pumpflowcmd = maxpumpflow;	// set command to the maximum pump flow
                        }
                        else
                        {
                            if (pumpflowcmd < -maxpumpflow)	// if the command flow is less than the minimum
                            {
                                pumpflowcmd = -maxpumpflow;	// then set the command = minimum pump flow
                            }
                        }

                        FLOW[i] = pumpflow + tau * Ts * (pumpflowcmd - pumpflow); 	// actual delivered flow (in^3/s)
                        LAST_FLOW[i] = FLOW[i];		// save flow values as states */


            /*
            OUT[0] = v[0] - vdes[0];
            OUT[1] = vdes[0];
            OUT[2] = v[0];// - vdes[k]);
            OUT[3] = pumpflowcmd[0];   
            */
        }

        public static void PumpModelFlow(float cmd, ref float[] flow, int i)
        {
            float pumpslope = maxPumpFlowArray[i] / 1;
            float flowcmd = maxPumpFlowArray[i] * cmd;
            float pumpflow = flow[i];

            const float tstep = Ts;

            float flowslope = tau * (flowcmd - pumpflow);
            float deltaflow = flowslope * tstep;

            if (Math.Abs(flowslope) < pumpslope) deltaflow = pumpslope * Math.Sign(flowslope) * tstep;

            if (Math.Abs(pumpflow - flowcmd) < Math.Abs(deltaflow)) pumpflow = flowcmd;
            else pumpflow = pumpflow + deltaflow;

            //flow(k) = pumpflow(k) + tau*tstep*(flowcmd(k) - pumpflow(k));
            float mpf = maxPumpFlowArray[i];
            flow[i] = Math.Max(-mpf, Math.Min(mpf, pumpflow));
        }














        /// <summary>
        /// Get Bucket Position, calculate Jacobian (partial derivitives) for Boom and Arm angles
        /// </summary>
        /// <param name="radiansA"></param>
        /// <param name="radiansB"></param>
        /// <param name="partialA">Partial derivitives for Boom</param>
        /// <param name="partialB">Partial derivitives for Arm</param>
        /// <returns></returns>
        public static Vector2 CalcBucketPostition(Double radiansA, Double radiansB,
            ref Vector2 partialA, ref Vector2 partialB)
        {
            Single cosA = (Single)Math.Cos(radiansA);
            Single cosB = (Single)Math.Cos(radiansB);

            Single sinA = (Single)Math.Sin(radiansA);
            Single sinB = (Single)Math.Sin(radiansB);

            partialA.X = -sinA * (LensBoom + LensArm * cosB) - LensArm * cosA * sinB;
            partialA.Y = cosA * (LensBoom + LensArm * cosB) - LensArm * sinA * sinB;

            partialB.X = -LensArm * cosA * sinB - LensArm * cosB * sinA;
            partialB.Y = LensArm * cosA * cosB - LensArm * sinA * sinB;

            return new Vector2(
                cosA * (LensBoom + LensArm * cosB) - LensArm * sinA * sinB,
                sinA * (LensBoom + LensArm * cosB) + LensArm * cosA * sinB);
        }



















        public const float slidey = -36.6f;		//bucket origin x-offset (came w/ bobcat files)
        public const float slidex = 5.1f;		//bucket origin y-offset

        const float Z_BUCKET_MIN = -10.4f;//-5.8;
        const float Z_BUCKET_MAX = 10.4f;//5.8;

        static readonly float[] X_BUCKET_OUTSIDE = new float[]
        {

            2.7408670000000002f,
            3.1411720000000001f,
            3.606948f,
            4.1341950000000001f,
            4.7167070000000004f,
            5.3499889999999999f,
            6.0287430000000004f,
            6.7448899999999998f,
            7.4928169999999996f,
            8.2664460000000002f,
            9.0565099999999994f,
            9.8566780000000005f,
            10.66062f,
            11.45871f,
            12.24437f,
            13.011520000000001f,
            13.751060000000001f,
            14.45669f,
            15.12303f,
            15.742319999999999f,
            16.309069999999998f,
            16.819019999999998f,
            17.266349999999999f,
            17.646619999999999f,
            17.958020000000001f,
            18.774419999999999f,
            20.067959999999999f
        };

        static readonly float[] Y_BUCKET_OUTSIDE = new float[]
        {
            5.854177f,
            6.5223319999999996f,
            7.2181300000000004f,
            7.8725680000000002f,
            8.4794070000000001f,
            9.0317860000000003f,
            9.5257170000000002f,
            9.9566110000000005f,
            10.31948f,
            10.61176f,
            10.83071f,
            10.97376f,
            11.04007f,
            11.02881f,
            10.940060000000001f,
            10.77491f,
            10.534380000000001f,
            10.22118f,
            9.8383889999999994f,
            9.3886889999999994f,
            8.8771020000000007f,
            8.3086199999999995f,
            7.6873379999999996f,
            7.019965f,
            6.3139900000000004f,
            5.5722680000000002f,
            4.4402790000000003f,
            0.90769449999999996f
        };

        const int NUMBER_OUTSIDE_POINTS = 28;

        static readonly float[] X_BUCKET_INSIDE = new float[]
        {    
	        0.76f,
	        2.0962f,
            3.8324f,
            5.0686f,
            6.2048f,
	        7.2410f,
            8.1772f,
            9.0134f,
           9.9497f,
           10.6859f,
           11.4221f,
           12.2583f,
           13.1945f,
           14.2669f,
           15.31f,
           16.493f,
           17.55f,
           18.6117f,
           19.7479f,
           20.8841f,
           21.9203f,
           23.0566f,
           24.1928f,
           24.8f,
           25.5f,
           26.290f,
           27.3652f,
           27.854f,
           28.412f,
           28.943f,
        };

        static readonly float[] Y_BUCKET_INSIDE = new float[]
        {
           15.100f, //ok
           16.6565f, //ok
           19.5482f, //ok
           21.1930f,	//ok
           21.9197f,	//ok
           22.4055f,	//ok
           22.8569f,	//ok
           23.1320f,	//ok
           23.3118f, //ok
           23.3386f,	
           23.3400f,
           23.3118f,
           23.1320f,//ok
           22.8669f,//ok
           22.3701f,//ok
           21.6455f,//ok
           20.7074f,//ok
           19.700f,//ok
           18.4882f,//ok
           17.3565f,//ok
           16.3500f,
           15.0940f,
           14.080f,
           13.4569f,
           12.8511f,
           12.0320f,
           11.3120f,
           10.8050f,
           10.2420f,
           9.9280f
        };

        const int NUMBER_INSIDE_POINTS = 30;

        static readonly float[] Cos_Theta = new float[]
        {
            1f,
            0.98480775301220802f,
            0.93969262078590843f,
            0.86602540378443871f,
            0.76604444311897801f,
            0.64278760968653936f,
            0.50000000000000011f,
            0.34202014332566882f,
            0.17364817766693041f,
            0f,
            -0.1736481776669303f,
            -0.34202014332566871f,
            -0.49999999999999978f,
            -0.64278760968653936f,
            -0.7660444431189779f,
            -0.86602540378443871f,
            -0.93969262078590832f,
            -0.98480775301220802f,
            -1f
        };

        static readonly float[] Sin_Theta = new float[] 
        {
            0f,
            0.17364817766693033f,
            0.34202014332566871f,
            0.49999999999999994f,
            0.64278760968653925f,
            0.76604444311897801f,
            0.8660254037844386f,
            0.93969262078590832f,
            0.98480775301220802f,
            1f,
            0.98480775301220802f,
            0.93969262078590843f,
            0.86602540378443871f,
            0.76604444311897801f,
            0.64278760968653947f,
            0.49999999999999994f,
            0.34202014332566888f,
            0.17364817766693028f,
            0f
        };

        const int NUMBER_THETA_POINTS = 19;

        static float[] Xtemp = new float[NUMBER_INSIDE_POINTS];
        static float[] Ytemp = new float[NUMBER_INSIDE_POINTS];

        static float[] Xarc = new float[NUMBER_THETA_POINTS];
        static float[] Yarc = new float[NUMBER_THETA_POINTS];

        public static volatile float _FloatBucketSoilVolume = 0;

        static private void display_bucket_dirt()
        {
            int n1, n2;
            float[] et = new float[2];
            float[] en = new float[2];
            float dt;


            // These values were tuned by hand 
            // see D:\Backhoe\Test_2007_03\FillBucket .... BkFill.m
            if (_FloatBucketSoilVolume <= 200)
            {
                n1 = NUMBER_INSIDE_POINTS - 1;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 500)
            {
                n1 = NUMBER_INSIDE_POINTS - 2;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 1000)
            {
                n1 = NUMBER_INSIDE_POINTS - 3;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 1500)
            {
                n1 = NUMBER_INSIDE_POINTS - 4;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 2000)
            {
                n1 = NUMBER_INSIDE_POINTS - 5;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 2500)
            {
                n1 = NUMBER_INSIDE_POINTS - 6;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 3000)
            {
                n1 = NUMBER_INSIDE_POINTS - 8;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 3500)
            {
                n1 = NUMBER_INSIDE_POINTS - 10;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 4000)
            {
                n1 = NUMBER_INSIDE_POINTS - 12;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 4500)
            {
                n1 = NUMBER_INSIDE_POINTS - 15;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 5000)
            {
                n1 = NUMBER_INSIDE_POINTS - 18;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 5500)
            {
                n1 = NUMBER_INSIDE_POINTS - 21;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 6000)
            {
                n1 = NUMBER_INSIDE_POINTS - 24;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else if (_FloatBucketSoilVolume <= 6500)
            {
                n1 = NUMBER_INSIDE_POINTS - 27;
                n2 = NUMBER_INSIDE_POINTS;
            }
            else
            {
                n1 = NUMBER_INSIDE_POINTS - 30;		// zero...all point are used
                n2 = NUMBER_INSIDE_POINTS;
            }

            // Puts correct bucket curve point at beginning of temp vectors
            for (int k = 0; k < n2 - n1; k++)
            {
                Xtemp[k] = X_BUCKET_INSIDE[n1 + k];
                Ytemp[k] = Y_BUCKET_INSIDE[n1 + k];
            }

            // This will put the correct value in Xarc and Yarc
            {
                int lenXY = n2 - n1;
                float[] Mid = new float[2];
                float[] p1 = new float[2];
                float[] e1 = new float[2];
                float[] e2 = new float[2];
                float d1;

                float ratio;

                // midpoint vector
                Mid[0] = (Xtemp[0] + Xtemp[lenXY - 1]) / 2;
                Mid[1] = (Ytemp[0] + Ytemp[lenXY - 1]) / 2;

                // Vector from first to last point
                p1[0] = Xtemp[lenXY - 1] - Xtemp[0];
                p1[1] = Ytemp[lenXY - 1] - Ytemp[0];

                // find unit vectors
                d1 = (float)Math.Sqrt(p1[0] * p1[0] + p1[1] * p1[1]);
                e1[0] = p1[0] / d1;
                e1[1] = p1[1] / d1;
                e2[0] = e1[1];
                e2[1] = -e1[0];

                // unscaled curve
                for (int k = 0; k < NUMBER_THETA_POINTS; k++)	// NUMBER_THETA_POINTS = lenARC = 19
                {
                    Xarc[k] = Mid[0] + d1 / 2 * Cos_Theta[k] * e1[0] + Sin_Theta[k] * e2[0];
                    Yarc[k] = Mid[1] + d1 / 2 * Cos_Theta[k] * e1[1] + Sin_Theta[k] * e2[1];
                }

                // calculate ratio
                //AreaInner = CalcPolyArea(X, Y, lenXY);
                //AreaOuter = CalcPolyArea(Xarc, Yarc, NUMBER_THETA_POINTS);
                //ratio = (AreaDes - AreaInner)/AreaOuter;
                ratio = 2 * (float)Math.Sqrt(Xtemp[lenXY - 1] - Xtemp[0]);
                // cout << "Vload=" << Vload << ", Ad=" << AreaDes << ", Ai=" << AreaInner;
                // cout << ", Ao=" << AreaOuter << ", ratio=" << ratio << endl;

                // recalculate outer curve
                for (int k = 0; k < NUMBER_THETA_POINTS; k++)	// NUMBER_THETA_POINTS = lenARC = 19
                {
                    Xarc[k] = Mid[0] + d1 / 2 * Cos_Theta[k] * e1[0] + ratio * Sin_Theta[k] * e2[0];
                    Yarc[k] = Mid[1] + d1 / 2 * Cos_Theta[k] * e1[1] + ratio * Sin_Theta[k] * e2[1];
                }
            }

            // Display surface

            for (int k = 0; k < NUMBER_THETA_POINTS - 1; k++)
            {
                // Calculate normal
                et[0] = Xarc[k + 1] - Xarc[k];
                et[1] = Yarc[k + 1] - Yarc[k];
                dt = (float)Math.Sqrt(et[0] * et[0] + et[1] * et[1]);
                et[0] = et[0] / dt;
                et[1] = et[1] / dt;
                en[0] = -et[1];
                en[1] = et[0];

                // red clay / dirt

                EmbeddedSoilModel.setColorDirt();

                GL.Begin(BeginMode.Triangles);
                {
                    GL.Normal3(en[0], en[1], 0);
                    GL.Vertex3(Xarc[k], Yarc[k], Z_BUCKET_MIN);
                    GL.Normal3(en[0], en[1], 0);
                    GL.Vertex3(Xarc[k], Yarc[k], Z_BUCKET_MAX);
                    GL.Normal3(en[0], en[1], 0);
                    GL.Vertex3(Xarc[k + 1], Yarc[k + 1], Z_BUCKET_MAX);
                    GL.Normal3(en[0], en[1], 0);
                    GL.Vertex3(Xarc[k + 1], Yarc[k + 1], Z_BUCKET_MAX);
                    GL.Normal3(en[0], en[1], 0);
                    GL.Vertex3(Xarc[k + 1], Yarc[k + 1], Z_BUCKET_MIN);
                    GL.Normal3(en[0], en[1], 0);
                    GL.Vertex3(Xarc[k], Yarc[k], Z_BUCKET_MIN);
                    GL.Normal3(0, 0, -1);
                    GL.Vertex3(Xarc[k], Yarc[k], Z_BUCKET_MIN);
                    GL.Normal3(0, 0, -1);
                    GL.Vertex3(Xarc[k + 1], Yarc[k + 1], Z_BUCKET_MIN);
                    GL.Normal3(0, 0, -1);
                    GL.Vertex3(Xarc[0], Yarc[0], Z_BUCKET_MIN);
                    GL.Normal3(0, 0, 1);
                    GL.Vertex3(Xarc[k], Yarc[k], Z_BUCKET_MAX);
                    GL.Normal3(0, 0, 1);
                    GL.Vertex3(Xarc[k + 1], Yarc[k + 1], Z_BUCKET_MAX);
                    GL.Normal3(0, 0, 1);
                    GL.Vertex3(Xarc[0], Yarc[0], Z_BUCKET_MAX);
                }
                GL.End();

            }
        }


        internal static void DrawSimpleBucket()
        {
            if (Bobcat._COBucketSimple != null)
                Bobcat._COBucketSimple.draw(false);
        }
    }
}
