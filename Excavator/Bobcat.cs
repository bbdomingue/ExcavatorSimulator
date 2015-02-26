using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using SamSeifert.GLE.CadViewer;

namespace Excavator
{
    internal static class Bobcat
    {
        internal const float _SamCatLiftHeight = 31.0f;
        internal static readonly Vector3 _V3CatLift = new Vector3(0, Bobcat._SamCatLiftHeight, 0);
        internal const float _V3RotationOffsetY = 1.36f;
        internal static readonly Vector3 _V3RotationOffset = new Vector3(1.31f, Bobcat._V3RotationOffsetY, -24.41f);

        
        private static readonly Color _ColorPiston = Color.DimGray;
        private static readonly Color _ColorCylinder = Color.LightGray;

        internal static readonly Vector3 _TV_V3_Lift = new Vector3(-5.3f, 22.24f, -16.10f);
        internal const float _TV_Float_Rotate = 4.95f;
        internal const float _TV_Width_Inches = 24.2679f;
        internal const float _TV_Height_Inches = 43.14f;
//        internal const int _TV_Width_Pixels = 1080;
//        internal const int _TV_Height_Pixels = 1920;


        internal class BobcatAngles
        {
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

        private static bool _BoolCadObjectsUninit = true;







        #region DrawConstants

        static readonly float pi = 3.1415926535897931f;

        static readonly float a1 = 5.60224f;
        static readonly float a2 = 104.0f;
        static readonly float a3 = 54.0f;
        static readonly float a4 = 31.33f;

        static readonly float r01 = a1;
        static readonly float r12 = a2;
        static readonly float r23 = a3;
        static readonly float r34 = a4;

        static readonly float ro1 = 6.76942f;
        static readonly float roA = 15.6884f;
        static readonly float r1A = 14.8486f;
        static readonly float r1B = 54.4949f;
        static readonly float r1C = 69.6644f;
        static readonly float r2B = 54.4949f;
        static readonly float r2C = 53.1519f;
        static readonly float r2D = 14.8011f;
        static readonly float r2E = 13.5656f;
        static readonly float r3D = 66.3633f;
        static readonly float r3E = 49.7071f;
        static readonly float r3F = 9.05512f;
        static readonly float r3G = 8.65592f;
        static readonly float rEF = 40.8434f;
        static readonly float rFH = 12.5984f;
        static readonly float rFI = 0.576462f;
        static readonly float r3I = (float)Math.Sqrt(-(rFI*rFI)+(r3F*r3F));

        static readonly float rGH = 12.3942f;
        static readonly float r4G = 35.9193f;

        static readonly float t01o = (float) Math.Acos(r01/ro1);
        static readonly float to1A = (float) Math.Acos((ro1*ro1+r1A*r1A-roA*roA)/(2*ro1*r1A));

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

        static readonly float dmax = (float) (a2 + a3 * Math.Cos(t3max));
        static readonly float dmin = (float) (a2 + a3 * Math.Cos(t3min));

        static readonly float t312min = (float) Math.Atan((a3 * (-Math.Sin(t3max)))/(a2 + a3*Math.Cos(t3max)));
        static readonly float t312max = StaticMethods.toRadiansF(27.5f);
        static readonly float Ymin4 = StaticMethods.toRadiansF(23.375f);
        static readonly float Ymax4 = StaticMethods.toRadiansF(37.125f);
        static readonly float t4max = StaticMethods.toRadiansF(30);
        static readonly float t4min = StaticMethods.toRadiansF(-153);
        static readonly float t3min_rmin_t2min = StaticMethods.toRadiansF(-150);
        static readonly float zmax_rmin_t2min = (float)(a2 * Math.Sin(t2min) + a3 * Math.Sin(t2min + t3min));
        static readonly float zmin_rmin_t3min = (float)(a2 * Math.Sin(t2min) + a3 * Math.Sin(t2min + t3max));
        static readonly float ztop = (float)(a2 * Math.Sin(t2max) + a3 * Math.Sin(t2max + t3max));
        static readonly float zbottom = (float)(a2 * Math.Sin(t2min) + a3 * Math.Sin(t2min + t3min));
        static readonly float rtop = (float)(a1 + a2 * Math.Cos(t2max) + a3 * Math.Cos(t2max + t3max));
        static readonly float rbottom = (float)(a1 + a2 * Math.Cos(t2min) + a3 * Math.Cos(t2min + t3min));

        #endregion


        internal static void Draw(
            BobcatAngles ang,
            bool drawCab,
            bool trackTexture)
        {
            StaticMethods._BoolTextureAlso = trackTexture;
            StaticMethods._BoolMatrixModeModelTexture = true;

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
            const float slidey = -36.6f;		//bucket origin x-offset (came w/ bobcat files)
            const float slidex = 5.1f;		//bucket origin y-offset
            const float rotatelink = -30;	//bucket link origin angle offset

            float tA1B, rAB, tBA1;				// boom cylinder calculations
            float tC2D, rCD, t2CD;				// stick cylinder calculations
            float Gx, Gy, beta, rFG;		   // bucket cylinder calculations
            float tHFG, tEFH, rEH, tFEH, tFHG;

            //////////////////////////////////////////////////////////////
            /*					Draw Arm								*/
            //////////////////////////////////////////////////////////////

            StaticMethods.PushMatrix();
            StaticMethods.Translate(Bobcat._V3CatLift);
            {
                if (drawCab)
                {
                    GL.Disable(EnableCap.CullFace);
                    Bobcat._COButt.draw();

                    StaticMethods.PushMatrix();
                    {
                        StaticMethods.Rotate(cab, Vector3.UnitY);
                        if (Bobcat._CO_TV._Display)
                        {
                            StaticMethods.PushMatrix();
                            {
                                StaticMethods.Translate(Bobcat._TV_V3_Lift);
                                StaticMethods.Rotate(Bobcat._TV_Float_Rotate, Vector3.UnitX);
                                Bobcat._CO_TV.draw();
                            }
                            StaticMethods.PopMatrix();
                        }

                        StaticMethods.Rotate(90, Vector3.UnitY);
                        Bobcat._COCab.draw();
                        
                        GL.Enable(EnableCap.CullFace);

                        if (Bobcat._CO_Head._Display)
                        {
                            StaticMethods.Rotate(-90, Vector3.UnitY);
                            StaticMethods.Translate(GLControl3D._Vector3EyePositionInCabS);
                            Bobcat._CO_Head.draw();
                        }
                    }
                    StaticMethods.PopMatrix();
                }


                StaticMethods.PushMatrix();
                {
                    StaticMethods.Rotate(cab + thetaSwing, Vector3.UnitY);
                    StaticMethods.Translate(-1.7f, 0, 0.3f);
                    StaticMethods.Rotate(-thetaSwing, Vector3.UnitY);
                    StaticMethods.Translate(Bobcat._V3RotationOffset);

                    StaticMethods.PushMatrix();
                    {
                        StaticMethods.Rotate(90, 0, 1, 0);
                        StaticMethods.Rotate(thetaSwing, 0, 1, 0);	    // unexplained origin offset in the .slp file
                        StaticMethods.Translate(-33, 0, 0);		// 33 is a fudge factor to adjust for an 
                        Bobcat._COSwingFrame.draw();
                    }
                    StaticMethods.PopMatrix();

                    // draw boom
                    StaticMethods.Rotate(-90, 1, 0, 0);
                    StaticMethods.Rotate(thetaSwing, 0, 0, 1);

                    StaticMethods.Rotate(90, 1, 0, 0);
                    StaticMethods.Rotate(90, 0, 1, 0);
                    StaticMethods.Translate(a1 - 1, 5, 2);	//adjust offset of origin with origin of .slp file
                    StaticMethods.Rotate(theta2, 0, 0, 1);
                    Bobcat._COBoom.draw();

                    //GL.Color(1,0,1);
                    //DrawGLObject(boomshadow);

                    //draw boom cylinder
                    StaticMethods.PushMatrix();
                    {
                        tA1B = tB12 + theta2 * pi / 180 + pi - t01A;
                        rAB = (float)Math.Sqrt(r1A * r1A + r1B * r1B - 2 * r1A * r1B * Math.Cos(tA1B));
                        tBA1 = (float)Math.Acos((r1A * r1A + rAB * rAB - r1B * r1B) / (2 * r1A * rAB));
                        StaticMethods.Rotate((t01A * 180 / pi) - 90 - theta2, 0, 0, 1);
                        StaticMethods.Translate(0, -.5f, 0);	//fudge factors (didn't get the above offset right on?)
                        StaticMethods.Translate(0, -r1A, 0);
                        StaticMethods.Rotate(180 + 180 - t01A * 180 / pi, 0, 0, 1);
                        StaticMethods.Rotate((-t01A + tBA1) * 180 / pi, 0, 0, -1);
                        Bobcat._COBoomCyl.draw();
                        StaticMethods.Translate(0, rAB + 2, 0);	//another offset
                        StaticMethods.Rotate(180, 0, 0, 1);
                        Bobcat._COBoomPiston.draw();
                    }
                    StaticMethods.PopMatrix();

                    // draw stick
                    StaticMethods.Translate(a2, 0, 0);
                    StaticMethods.Rotate(theta3, 0, 0, 1);
                    Bobcat._COArm.draw();

                    //GL.Color(0,1,0);
                    //DrawGLObject(armshadow);		

                    // draw stick cylinder
                    StaticMethods.PushMatrix();
                    {
                        tC2D = pi - t12C - tD23 - theta3 * pi / 180;
                        rCD = (float)Math.Sqrt(r2C * r2C + r2D * r2D - 2 * r2C * r2D * Math.Cos(tC2D));
                        t2CD = (float)Math.Acos((r2C * r2C + rCD * rCD - r2D * r2D) / (2 * r2C * rCD));
                        StaticMethods.Rotate(-theta3, 0, 0, 1);
                        StaticMethods.Rotate(90, 0, 0, 1);
                        StaticMethods.Rotate(t12C * 180 / pi, 0, 0, -1);
                        StaticMethods.Translate(0, r2C, 0);
                        StaticMethods.Rotate(180 + t2CD * 180 / pi, 0, 0, 1);
                        Bobcat._COArmCyl.draw();
                        StaticMethods.Translate(0, rCD, 0);
                        StaticMethods.Rotate(180, 0, 0, 1);
                        Bobcat._COArmPiston.draw();
                    }
                    StaticMethods.PopMatrix();

                    StaticMethods.Translate(a3, 0, 0);
                    //display bucket
                    StaticMethods.PushMatrix();
                    {
                        StaticMethods.Rotate(theta4, 0, 0, 1);
                        //GL.Color(0,1,1);
                        //DrawGLObject(bucketshadow);
                        StaticMethods.PushMatrix();
                        {
                            //GL.Rotate(35,0,0,1);
                            //GL.Rotate(-15,0,0,1);
                            StaticMethods.Rotate(20, 0, 0, 1);
                            StaticMethods.Translate(-0.771f, 2.11f - 3.67f, 0);	//correct for the slp origin
                            display_bucket_dirt(/*data*/);
                        }
                        StaticMethods.PopMatrix();

                        StaticMethods.Rotate(135 - 15, 0, 0, 1);		//135-18.9 (about) is the bucket origin angle offset in the slp file
                        StaticMethods.PushMatrix();
                        {
                            StaticMethods.Translate(8.461f, 1.768f, 0);
                            Bobcat._COExchange.draw();
                        }
                        StaticMethods.PopMatrix();

                        StaticMethods.Rotate(35, 0, 0, 1);
                        StaticMethods.Translate(slidex / 2 - 0.365f, slidey / 2 + 2.11f, 0);	//correct for the slp origin

                        Bobcat._COBucketSimple.draw();
                        Bobcat._COBucket.draw();
                    }
                    StaticMethods.PopMatrix();

                    // draw bucket cylinder
                    StaticMethods.PushMatrix();		//for some reason, the compiler didn't like long algebraic statements
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

                        StaticMethods.Translate(-r3I, -rFI + 1, 0); //1 is a fudge factor

                        StaticMethods.PushMatrix();
                        {
                            StaticMethods.Rotate(-90, 0, 0, 1);
                            //temp = 180-alpha*180/pi;
                            temp = -alfa;
                            temp = 180 + temp * 180 / pi;
                            StaticMethods.Rotate(temp, 0, 0, 1);
                            StaticMethods.Translate(0, rEF, 0);
                            StaticMethods.Rotate(180 + tFEH * 180 / pi, 0, 0, 1);
                            Bobcat._COBucketCyl.draw();
                            StaticMethods.Translate(0, rEH, 0);
                            StaticMethods.Rotate(180, 0, 0, 1);
                            Bobcat._COBucketPiston.draw();
                        }

                        StaticMethods.PopMatrix();

                        StaticMethods.Rotate((beta + tHFG) * 180 / pi, 0, 0, 1);
                        Bobcat._COBucketLink2.draw();
                        StaticMethods.Translate(rFH, 0, 0);
                        StaticMethods.Rotate(tFHG * 180 / pi, 0, 0, 1);
                        StaticMethods.Rotate(90, 0, 1, 0);
                        StaticMethods.Rotate(rotatelink, 1, 0, 0);
                        StaticMethods.Translate(3.1f, 0, 0);
                        Bobcat._COBucketLink.draw();
                    }
                    StaticMethods.PopMatrix();
                }
                StaticMethods.PopMatrix();
            }
            StaticMethods.PopMatrix();

            if (trackTexture) GL.MatrixMode(MatrixMode.Modelview);
        }

        static private void display_bucket_dirt()
        {
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
            Bobcat._COBucketSimple._Display = false;

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

            Bobcat._COCab = CadObjectGenerator.fromXAML(Properties.Resources.Dull_Cab,ObjectName: "Cab");
            Bobcat._COButt = CadObjectGenerator.fromXAML(Properties.Resources.Dull_Butt, ObjectName: "Butt");

            Bobcat.SetupHeadAndTV();





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

            List<String> ls1 = new List<String>();
            List<String> ls2 = new List<String>();

            ls1.Add(-Bobcat._TV_Width_Inches / 2 + "," + -Bobcat._TV_Height_Inches / 2 + ",0\n");
            ls1.Add(-Bobcat._TV_Width_Inches / 2 + "," + Bobcat._TV_Height_Inches / 2 + ",0\n");
            ls1.Add(Bobcat._TV_Width_Inches / 2 + "," + Bobcat._TV_Height_Inches / 2 + ",0\n");

            ls1.Add(Bobcat._TV_Width_Inches / 2 + "," + -Bobcat._TV_Height_Inches / 2 + ",0\n");
            ls1.Add(-Bobcat._TV_Width_Inches / 2 + "," + -Bobcat._TV_Height_Inches / 2 + ",0\n");
            ls1.Add(Bobcat._TV_Width_Inches / 2 + "," + Bobcat._TV_Height_Inches / 2 + ",0\n");

            Bobcat._CO_TV = CadObjectGenerator.fromVertexList(ls1.ToArray(), "TV");
            Bobcat._CO_TV._Display = false;

            ls1.Clear();

            float f = 2;

            String p1 = f + "," + f + "," + f;
            String p2 = -f + "," + f + "," + f;
            String p3 = -f + "," + -f + "," + f;
            String p4 = f + "," + -f + "," + f;

            String p5 = f + "," + f + "," + -f;
            String p6 = -f + "," + f + "," + -f;
            String p7 = -f + "," + -f + "," + -f;
            String p8 = f + "," + -f + "," + -f;

            for (int i = 0; i < 6; i++) ls1.Add("0,0,1");
            ls2.Add(p1);
            ls2.Add(p2);
            ls2.Add(p3);
            ls2.Add(p3);
            ls2.Add(p4);
            ls2.Add(p1);

            for (int i = 0; i < 6; i++) ls1.Add("0,1,0");
            ls2.Add(p1);
            ls2.Add(p5);
            ls2.Add(p6);
            ls2.Add(p6);
            ls2.Add(p2);
            ls2.Add(p1);

            for (int i = 0; i < 6; i++) ls1.Add("0,0,-1");
            ls2.Add(p5);
            ls2.Add(p8);
            ls2.Add(p7);
            ls2.Add(p7);
            ls2.Add(p6);
            ls2.Add(p5);

            for (int i = 0; i < 6; i++) ls1.Add("-1,0,0");
            ls2.Add(p2);
            ls2.Add(p6);
            ls2.Add(p7);
            ls2.Add(p7);
            ls2.Add(p3);
            ls2.Add(p2);

            for (int i = 0; i < 6; i++) ls1.Add("0,-1,0");
            ls2.Add(p3);
            ls2.Add(p7);
            ls2.Add(p8);
            ls2.Add(p8);
            ls2.Add(p4);
            ls2.Add(p3);

            for (int i = 0; i < 6; i++) ls1.Add("1,0,0");
            ls2.Add(p4);
            ls2.Add(p8);
            ls2.Add(p5);
            ls2.Add(p5);
            ls2.Add(p1);
            ls2.Add(p4);

            Bobcat._CO_Head = CadObjectGenerator.fromVertexAndNormalList(ls2.ToArray(), ls1.ToArray(), "Head");
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
                    Bobcat._CO_Head
            };
        }
    }
}
