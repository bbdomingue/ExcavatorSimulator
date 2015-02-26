using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SamSeifert.GLE;
using SamSeifert.GLE.CadViewer;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using PhysX;

using Matrix_PhysX = PhysX.Math.Matrix;
using Vector3_PhysX = PhysX.Math.Vector3;
using Vector4_PhysX = PhysX.Math.Vector4;

namespace Excavator
{
    public class EmbeddedSoilModel
    {
        // LOCK OBJECTS:
        // SoilHeight (Locked when reseting soil, locked when simulating inside matlab thread)
        // _V3SoilPoints (Locked when drawing, and on every 30th simulate inside matlab thread)

        private static EmbeddedSoilModel _Instance = null;
        internal static EmbeddedSoilModel Instance
        {
            get
            {
                if (_Instance == null) _Instance = new EmbeddedSoilModel();
                return _Instance;
            }
        }

        private EmbeddedSoilModel()
        {
        }

        const float TrenchSectionsPerInch = 1;
        const float InchesPerTrenchSection = 1 / TrenchSectionsPerInch;
        const int TrenchSections = (int)(TrenchLength * TrenchSectionsPerInch);
        const float TrenchLength = 240;
        public const float TrenchWidth = 24;

        const int DrawPointsPerSide = (1 + TrenchSections);
        const int DrawPoints = DrawPointsPerSide * 2; // Both Sides Side

        const float TrenchLeftX = -TrenchWidth / 2;
        const float TrenchRightX = -TrenchLeftX;

        public const float XTextL = Trial._IntTextureDensity * (0.5f + (TrenchLeftX / (2 * Trial._FloatGroundPlaneDim)));
        public const float XTextR = Trial._IntTextureDensity * (0.5f + (TrenchRightX / (2 * Trial._FloatGroundPlaneDim)));
        public const float ZTextBehind = Trial._IntTextureDensity * (0.5f);
        public const float ZTextFront = Trial._IntTextureDensity * (0.5f - (TrenchLength / (2 * Trial._FloatGroundPlaneDim)));

        private readonly float[] SoilHeight = new float[DrawPointsPerSide];

        const float BucketVolumeMax = 7200;
        private float BucketVolume = 0;

        /// <summary>
        /// Only Update Soil Heights Every 30 ms
        /// </summary>
        static int UpdateSoilCount = 0;
        
        /// <summary>
        /// Used in draw thread and simulate thread
        /// </summary>
        static volatile bool UpdateSoilCountB = false;
        
        private Vector3_PhysX _LastEntryPoint = Vector3_PhysX.Zero;
        private bool _ValidLastEntry = false;

        /// <summary>
        /// PhysX is locked when called!
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="bucket_tip"></param>
        /// <param name="cab"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        internal unsafe TrialSaver.File2_DataType ForceModel_ReturnDump(
            RigidDynamic bucket,
            Shape bucket_tip,
            RigidDynamic cab,
            Scene s,
            float simtime)
        {
            var ret = new TrialSaver.File2_DataType() { Size = -1 };

            lock (this.SoilHeight)
            {
                var tip_pose = bucket_tip.GlobalPose;

                var tip_vel_abs = StaticMethods.Conversion_Meters_To_Inches * bucket.LinearVelocity;
                var tip_vel_rel = Vector4_PhysX.Transform(new Vector4_PhysX(tip_vel_abs, 0), tip_pose).xyz(); // Relative Tip Velocity (OWN CS)

                var tip_pos_meters = tip_pose.xyz();
                var tip_pos = tip_pos_meters * StaticMethods.Conversion_Meters_To_Inches;

                var x_pos = StaticMethods.Conversion_Meters_To_Inches *
                    Vector4_PhysX.Transform(new Vector4_PhysX(0, 0, 0.5f, 1), tip_pose).xyz();

                var y_pos = StaticMethods.Conversion_Meters_To_Inches *
                    Vector4_PhysX.Transform(new Vector4_PhysX(0, -0.181f, 0.65f, 1), tip_pose).xyz();

                const float soil_density = 0.03149594f;
                const float gravity = -9.8f;

                int pointY_closest_point_dex = (int)Math.Round(-y_pos.Z * TrenchSectionsPerInch);
                int pointX_closest_point_dex = (int)Math.Round(-x_pos.Z * TrenchSectionsPerInch);
                int tip_closest_point_dex = (int)Math.Round(-tip_pos.Z * TrenchSectionsPerInch);

                int tip_closest_span_dex = (int)Math.Round(-tip_pos.Z * TrenchSectionsPerInch - 0.5f);


                //////////////////////////// If the c's are the bucket
                //////cccccccccc//////////// T is the tip
                /////cccccccccc///////////// X is the pointX (back on level)
                ////Yccccccccccc//////////// Y is the pointY
                /////cccccccccccc///////////
                /////cccccccccccccc/////////
                ///////XcccccccccccccT//////
                ////////////////////////////

                var angle_of_attack_vec = Vector4_PhysX.Transform(new Vector4_PhysX(0, 0, -1, 0), tip_pose);
                var angle_of_attack = (float)Math.Atan2(-angle_of_attack_vec.Y, angle_of_attack_vec.Z);

                // CHECK IF X IS IN TRENCH AND IF Z IS RIGHT DIRECTION
                if ((Math.Abs(tip_pos.X) * 2 < (TrenchWidth / 2)) && (tip_closest_point_dex > 0))
                {
                    const int two_inches_section_count = (int)(2 * TrenchSectionsPerInch);
                    const int fifteen_inches_section_count = (int)(15 * TrenchSectionsPerInch);

                    bool tip_under = tip_pos.Y - 3 < this.SoilHeight[tip_closest_point_dex]; // inches

                    if (tip_under)
                    {
                        if (this._ValidLastEntry)
                        {
                            // Angle of attack is defined in paper.
                            //                        FormBase.floatRandom = this._LastEntryPoint.Z - tip_pos.Z;

                            var soil_max_height_leading_bucket = float.MinValue;
                            for (int i = Math.Max(0, tip_closest_point_dex - two_inches_section_count); i <= tip_closest_point_dex; i++)
                                soil_max_height_leading_bucket = Math.Max(soil_max_height_leading_bucket, this.SoilHeight[i]);

                            soil_max_height_leading_bucket = Math.Max(0, soil_max_height_leading_bucket - tip_pos.Y);


                            // Buckets full, got to push up to twice as hard!
                            float shear_dist_bucket_vol = 1 + this.BucketVolume / (BucketVolumeMax); // Full backet gives 2 difficulty
                            float shear_dist_soil_h = 1 + soil_max_height_leading_bucket / 0.75f; // 0.75 meter deep soil doubles value.


                            bucket.AddForce(new Vector3_PhysX(
                                //                        bucket.AddForceAtLocalPosition(new Vector3_PhysX(
                                0,
                                // (1 - cos) will zero term when angle of attack is negative, which we need cause soil model handles that case
                                2000 * (this._LastEntryPoint.Y - tip_pos.Y) * (1 - (float)Math.Cos(angle_of_attack)),
                                500 * shear_dist_bucket_vol * shear_dist_soil_h * (this._LastEntryPoint.Z - tip_pos.Z))
                                );
                            //                            ,bucket_tip.LocalPose.xyz());

                            if (tip_pos.Z - this._LastEntryPoint.Z > 2) // 4 Inches For Break Distance
                                this._LastEntryPoint = tip_pos;
                        }
                        else
                        {
                            this._LastEntryPoint = tip_pos;
                            this._ValidLastEntry = true;
                        }
                    }
                    else // not tip under
                    {
                        this._ValidLastEntry = false;
                        if (angle_of_attack > StaticMethods.toRadiansF(45) && (this.BucketVolume != 0))
                        {
                            this.NewPileDataComing(ref ret, tip_pos, simtime);
                        }
                    }

                    float eat_height = this.SoilHeight[tip_closest_point_dex] - tip_pos.Y;

                    if (eat_height > 0)
                    {
                        this.SoilHeight[tip_closest_point_dex] = tip_pos.Y;

                        float splooge_height = 0.1f * eat_height; // Controls how much soil is pushed out of way, and how much is put in bucket
                        eat_height -= splooge_height;

                        float max_eatable_height = (BucketVolumeMax - this.BucketVolume) / (InchesPerTrenchSection * TrenchWidth);

                        if (max_eatable_height > 0)
                        {
                            if ((tip_vel_abs.Z > 0) && (eat_height > 0))
                            {
                                if (eat_height < max_eatable_height)
                                {
                                    this.BucketVolume += eat_height * InchesPerTrenchSection * TrenchWidth;
                                    max_eatable_height -= eat_height;
                                    eat_height = 0;
                                }
                                else
                                {
                                    this.BucketVolume += max_eatable_height * InchesPerTrenchSection * TrenchWidth;
                                    eat_height -= max_eatable_height;
                                    max_eatable_height = 0;
                                }
                            }
                        }

                        eat_height += splooge_height;

                        int start_z_dex = tip_closest_point_dex - 1;
                        int z_dex = start_z_dex;

                        while (true)
                        {
                            const float add_height = InchesPerTrenchSection / 3; // Controls how far away from tip soil is pushed up

                            if (eat_height <= add_height)
                            {
                                this.SoilHeight[z_dex] += eat_height;
                                eat_height = 0;
                                break;
                            }
                            else
                            {
                                this.SoilHeight[z_dex] += add_height;
                                eat_height -= add_height;
                            }

                            if (--z_dex < 0) z_dex = start_z_dex;
                            else if (this.SoilHeight[z_dex] > this.SoilHeight[z_dex + 1]) z_dex++;

                        }
                    }

                    // 0.25 = meters below bucket tip
                    var targ_h = tip_pos_meters.Y - 0.25f;

                    float bucket_min_z = Math.Min(tip_pos.Z, Math.Min(y_pos.Z, x_pos.Z));
                    float bucket_max_z = Math.Max(tip_pos.Z, Math.Max(y_pos.Z, x_pos.Z));

                    int bucket_max_closest_span_dex = (int)Math.Round(-bucket_min_z * TrenchSectionsPerInch - 0.5f);
                    int bucket_min_closest_span_dex = (int)Math.Round(-bucket_max_z * TrenchSectionsPerInch - 0.5f);

                    int min_15 = Math.Max(0, bucket_min_closest_span_dex - fifteen_inches_section_count);
                    int plus_15 = Math.Min(TrenchSections - 1, bucket_max_closest_span_dex + fifteen_inches_section_count);

                    int critical = Math.Max(0, Math.Min(TrenchSections, tip_closest_span_dex + two_inches_section_count));

                    for (int i = 0; i < min_15; i++) EmbeddedSoilModel.TurnOffSoilAt(i, s);
                    for (int i = min_15; i < critical; i++)
                    {
                        var hh = -Math.Min(StaticMethods.Conversion_Inches_To_Meters * (this.SoilHeight[i] + this.SoilHeight[i + 1]) / 2, targ_h);
                        EmbeddedSoilModel.TurnOnSoilAt(i, s, hh);
                        EmbeddedSoilModel.SoilConstraints[i].DrivePosition = Matrix_PhysX.Translation(0, hh, 0);
                    }
                    for (int i = critical; i <= plus_15; i++)
                    {
                        var hh = -StaticMethods.Conversion_Inches_To_Meters * (this.SoilHeight[i] + this.SoilHeight[i + 1]) / 2;
                        EmbeddedSoilModel.TurnOnSoilAt(i, s, hh);
                        EmbeddedSoilModel.SoilConstraints[i].DrivePosition = Matrix_PhysX.Translation(0, hh, 0);
                    }
                    for (int i = plus_15 + 1; i < TrenchLength; i++) EmbeddedSoilModel.TurnOffSoilAt(i, s);
                }
                else // Not in trench x
                {
                    this._ValidLastEntry = false;
                    if (angle_of_attack > StaticMethods.toRadiansF(45) && (this.BucketVolume != 0))
                    {
                        this.NewPileDataComing(ref ret, tip_pos, simtime);
                    }
                }

                bucket.AddForce(new PhysX.Math.Vector3(0, gravity * soil_density * this.BucketVolume, 0));
                Bobcat._FloatBucketSoilVolume = this.BucketVolume;

                if (++EmbeddedSoilModel.UpdateSoilCount == 30)
                {
                    EmbeddedSoilModel.UpdateSoilCount = 0;

                    lock (EmbeddedSoilModel._V3SoilPoints)
                    {
                        UpdateSoilCountB = true;
                        float Ym = 0;
                        float Y0 = this.SoilHeight[0];
                        float Yp = this.SoilHeight[1];

                        for (int k = 1; k < EmbeddedSoilModel.DrawPointsPerSide - 1; k++)
                        {
                            Ym = Y0;
                            Y0 = Yp;
                            Yp = this.SoilHeight[k + 1];

                            Vector3 norm = new Vector3(0, 2 * InchesPerTrenchSection, Yp - Ym);
                            norm.NormalizeFast();

                            EmbeddedSoilModel._V3SoilPoints[EmbeddedSoilModel.DrawPoints + 2 * k + 0].Y = Y0;
                            EmbeddedSoilModel._V3SoilPoints[EmbeddedSoilModel.DrawPoints + 2 * k + 1].Y = Y0;
                            EmbeddedSoilModel._V3SoilPoints[2 * k + 0] = norm;
                            EmbeddedSoilModel._V3SoilPoints[2 * k + 1] = norm;
                        }
                    }

                    int last_dex = Math.Max(pointX_closest_point_dex, pointY_closest_point_dex);
                    float hh = Math.Min(tip_pos.Y, Math.Min(x_pos.Y, y_pos.Y));
                    for (int k = tip_closest_point_dex; k <= last_dex; k++)
                    {
                        // Just do for zero cause should be the same.
                        float h = Math.Min(EmbeddedSoilModel._V3SoilPoints[EmbeddedSoilModel.DrawPoints + 2 * k + 0].Y, hh);
                        EmbeddedSoilModel._V3SoilPoints[EmbeddedSoilModel.DrawPoints + 2 * k + 0].Y = h;
                        EmbeddedSoilModel._V3SoilPoints[EmbeddedSoilModel.DrawPoints + 2 * k + 1].Y = h;
                    }
                }
            }

            return ret;
        }

/*        private float SoilHeightAtInchesZ(float z)
        {
            z *= -TrenchSectionsPerInch;
            int high_z = (int)Math.Ceiling(z);
            int low_z = (int)Math.Floor(z);
            if (high_z > TrenchSections) return 0;
            else if (low_z < 0) return 0;
            else if (high_z == low_z) return this.SoilHeight[high_z];
            else return this.SoilHeight[low_z] + (z - low_z) * (this.SoilHeight[high_z] - this.SoilHeight[low_z]) / InchesPerTrenchSection;
        }*/



















































        public static void drawTrench(bool ShadowBufferDraw)
        {
            UInt32 uip = GL.CurrentProgram;

            if (EmbeddedSoilModel._IntTextureDirt == 0)
                EmbeddedSoilModel._IntTextureDirt = Textures.getGLTexture(Properties.Resources.dirt4);

            if (EmbeddedSoilModel._IntTextureGrass == 0)
                EmbeddedSoilModel._IntTextureGrass = Textures.getGLTexture(Properties.Resources.grass1);
            
            if (EmbeddedSoilModel._ShadowIntProgram != 0 &&
                EmbeddedSoilModel._ShadowIntShaderV != 0 &&
                EmbeddedSoilModel._ShadowIntShaderF != 0)
            {
                if (EmbeddedSoilModel._BoolSetupGL4)
                {
                    EmbeddedSoilModel.updateGL4();

                    const int trench_depth = -200;

                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, EmbeddedSoilModel.ColorFrom(EmbeddedSoilModel._ColorDirt, 0.25f));
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, EmbeddedSoilModel.ColorFrom(EmbeddedSoilModel._ColorDirt, 1.0f));

                    if (!ShadowBufferDraw)
                    {
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, EmbeddedSoilModel._IntTextureDirt);
                        GL.UseProgram(EmbeddedSoilModel._ShadowIntProgram);
                    }

                    // Draw Sides
                    GL.Begin(BeginMode.Quads);
                    {
                        GL.Normal3(-1, 0, 0);
                        GL.Vertex3(EmbeddedSoilModel.TrenchRightX, 0, -EmbeddedSoilModel.TrenchLength);
                        GL.Vertex3(EmbeddedSoilModel.TrenchRightX, trench_depth, -EmbeddedSoilModel.TrenchLength);
                        GL.Vertex3(EmbeddedSoilModel.TrenchRightX, trench_depth, 0);
                        GL.Vertex3(EmbeddedSoilModel.TrenchRightX, 0, 0);
                        GL.Normal3(1, 0, 0);
                        GL.Vertex3(EmbeddedSoilModel.TrenchLeftX, 0, 0);
                        GL.Vertex3(EmbeddedSoilModel.TrenchLeftX, trench_depth, 0);
                        GL.Vertex3(EmbeddedSoilModel.TrenchLeftX, trench_depth, -EmbeddedSoilModel.TrenchLength);
                        GL.Vertex3(EmbeddedSoilModel.TrenchLeftX, 0, -EmbeddedSoilModel.TrenchLength);
                    }
                    GL.End();

                    // Draw Heights
                    GL.BindBuffer(BufferTarget.ArrayBuffer, EmbeddedSoilModel._IntInterleaveBufferID);
                    GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes * 1, Vector3.SizeInBytes * DrawPoints);
                    GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes * 1, IntPtr.Zero);

                    GL.EnableClientState(ArrayCap.VertexArray);
                    GL.EnableClientState(ArrayCap.NormalArray);

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, EmbeddedSoilModel._IntIndicesBufferID);
                    GL.DrawElements(BeginMode.Triangles, EmbeddedSoilModel._IntElementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

                    if (!ShadowBufferDraw)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, EmbeddedSoilModel._IntTextureGrass);
                    }

                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, EmbeddedSoilModel.ColorFrom(System.Drawing.Color.White, 0.0f));
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, EmbeddedSoilModel.ColorFrom(System.Drawing.Color.White, 1.0f));

                    GL.Begin(BeginMode.Quads);
                    {
                        GL.Vertex3(TrenchLeftX, 0, Trial._FloatGroundPlaneDim);
                        GL.Vertex3(TrenchLeftX, 0, -Trial._FloatGroundPlaneDim);
                        GL.Vertex3(-Trial._FloatGroundPlaneDim, 0, -Trial._FloatGroundPlaneDim);
                        GL.Vertex3(-Trial._FloatGroundPlaneDim, 0, Trial._FloatGroundPlaneDim);

                        GL.Vertex3(Trial._FloatGroundPlaneDim, 0, Trial._FloatGroundPlaneDim);
                        GL.Vertex3(Trial._FloatGroundPlaneDim, 0, -Trial._FloatGroundPlaneDim);
                        GL.Vertex3(TrenchRightX, 0, -Trial._FloatGroundPlaneDim);
                        GL.Vertex3(TrenchRightX, 0, Trial._FloatGroundPlaneDim);

                        GL.Vertex3(TrenchLeftX, 0, 0);
                        GL.Vertex3(TrenchLeftX, 0, Trial._FloatGroundPlaneDim);
                        GL.Vertex3(TrenchRightX, 0, Trial._FloatGroundPlaneDim);
                        GL.Vertex3(TrenchRightX, 0, 0);

                        GL.Vertex3(TrenchLeftX, 0, -Trial._FloatGroundPlaneDim);
                        GL.Vertex3(TrenchLeftX, 0, -TrenchLength);
                        GL.Vertex3(TrenchRightX, 0, -TrenchLength);
                        GL.Vertex3(TrenchRightX, 0, -Trial._FloatGroundPlaneDim);
                    }
                    GL.End();

                    if (!ShadowBufferDraw)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, 0);
                        GL.ActiveTexture(TextureUnit.Texture7);
                        GL.UseProgram(uip);
                    }
                }
                else EmbeddedSoilModel.setupGL4();
            }
            else
            {
                int sp, sv, sf;

                if (Shaders.CreateShaders(
                    Properties.Resources.SoilVertex,
                    Properties.Resources.SoilFragment,
                    out sp,
                    out sv,
                    out sf))
                {
                    EmbeddedSoilModel._ShadowIntProgram = sp;
                    EmbeddedSoilModel._ShadowIntShaderV = sv;
                    EmbeddedSoilModel._ShadowIntShaderF = sf;

                    GL.UseProgram(EmbeddedSoilModel._ShadowIntProgram);
                    Textures.BindTexture(EmbeddedSoilModel._ShadowIntProgram, TextureUnit.Texture0, "tex0");
                    Textures.BindTexture(EmbeddedSoilModel._ShadowIntProgram, TextureUnit.Texture7, "ShadowMap");
                    GL.UseProgram(uip);
                }
            }
        }

        public static void drawTrenchOrtho()
        {
//            GL.Disable(EnableCap.CullFace);
            GL.Color3(EmbeddedSoilModel._ColorDirt);
            GL.BindBuffer(BufferTarget.ArrayBuffer, EmbeddedSoilModel._IntInterleaveBufferID);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes * 1, Vector3.SizeInBytes * DrawPoints);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EmbeddedSoilModel._IntIndicesBufferOrthoID);
            GL.DrawElements(BeginMode.Triangles, EmbeddedSoilModel._IntElementCountOrtho, DrawElementsType.UnsignedInt, IntPtr.Zero);
//            GL.Enable(EnableCap.CullFace);
        }

        private static bool _BoolSetupGL4 = false;

        private static int _IntInterleaveBufferID;
        private static int _IntIndicesBufferID = 0;
        private static int _IntElementCount = 0;
        private static  int _IntIndicesBufferOrthoID = 0;
        private static  int _IntElementCountOrtho = 0;

        public static int _ShadowIntProgram { get; private set; }
        public static int _ShadowIntShaderV { get; private set; }
        public static int _ShadowIntShaderF { get; private set; }
        private static int _IntTextureDirt = 0;
        private static int _IntTextureGrass = 0;

        public static void GLDelete_Static()
        {
            if (EmbeddedSoilModel._CadObjectBoxL != null) EmbeddedSoilModel._CadObjectBoxL.GLDelete();
            if (EmbeddedSoilModel._CadObjectBoxR != null) EmbeddedSoilModel._CadObjectBoxR.GLDelete();

            if (EmbeddedSoilModel._ShadowIntProgram != 0) GL.DeleteProgram(EmbeddedSoilModel._ShadowIntProgram);
            if (EmbeddedSoilModel._ShadowIntShaderV != 0) GL.DeleteShader(EmbeddedSoilModel._ShadowIntShaderV);
            if (EmbeddedSoilModel._ShadowIntShaderF != 0) GL.DeleteShader(EmbeddedSoilModel._ShadowIntShaderF);

            EmbeddedSoilModel._ShadowIntProgram = 0;
            EmbeddedSoilModel._ShadowIntShaderV = 0;
            EmbeddedSoilModel._ShadowIntShaderF = 0;

            if (EmbeddedSoilModel._IntTextureDirt != 0)
            {
                GL.DeleteTexture(EmbeddedSoilModel._IntTextureDirt);
                EmbeddedSoilModel._IntTextureDirt = 0;
            }

            if (EmbeddedSoilModel._IntTextureGrass != 0)
            {
                GL.DeleteTexture(EmbeddedSoilModel._IntTextureGrass);
                EmbeddedSoilModel._IntTextureGrass = 0;
            }

            if (EmbeddedSoilModel._BoolSetupGL4)
            {
                EmbeddedSoilModel._BoolSetupGL4 = false;
                GL.DeleteBuffers(1, ref EmbeddedSoilModel._IntIndicesBufferID);
                GL.DeleteBuffers(1, ref EmbeddedSoilModel._IntIndicesBufferOrthoID);
                GL.DeleteBuffers(1, ref EmbeddedSoilModel._IntInterleaveBufferID);
            }
        }

        private static void setupGL4()
        {
            lock (EmbeddedSoilModel._V3SoilPoints)
            {
                EmbeddedSoilModel._V3SoilPoints[0] = Vector3.UnitY;
                EmbeddedSoilModel._V3SoilPoints[1] = Vector3.UnitY;
                EmbeddedSoilModel._V3SoilPoints[DrawPoints - 1] = Vector3.UnitY;
                EmbeddedSoilModel._V3SoilPoints[DrawPoints - 2] = Vector3.UnitY;

                EmbeddedSoilModel._V3SoilPoints[1] = Vector3.UnitY;

                for (int k = 0; k < EmbeddedSoilModel.DrawPointsPerSide; k++)
                {
                    float z = -k * InchesPerTrenchSection;
                    EmbeddedSoilModel._V3SoilPoints[k * 2 + 0] = Vector3.UnitY;
                    EmbeddedSoilModel._V3SoilPoints[k * 2 + 1] = Vector3.UnitY;
                    EmbeddedSoilModel._V3SoilPoints[k * 2 + 0 + DrawPoints] = new Vector3(TrenchLeftX, 0, z);
                    EmbeddedSoilModel._V3SoilPoints[k * 2 + 1 + DrawPoints] = new Vector3(TrenchRightX, 0, z);
                    EmbeddedSoilModel._V3SoilPoints[k + DrawPoints + DrawPoints] = new Vector3(TrenchLeftX, 0, z);
                }

                int bufferSize = EmbeddedSoilModel._V3SoilPoints.Length * Vector3.SizeInBytes;
                GL.GenBuffers(1, out EmbeddedSoilModel._IntInterleaveBufferID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, EmbeddedSoilModel._IntInterleaveBufferID);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferSize), EmbeddedSoilModel._V3SoilPoints, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                uint[] triangs = new uint[EmbeddedSoilModel.TrenchSections * 6];

                for (uint i = 0; i < EmbeddedSoilModel.TrenchSections; i++)
                {
                    uint i6 = i * 6;
                    uint i2 = i * 2;
                    triangs[i6 + 0] = i2;
                    triangs[i6 + 1] = i2 + 1;
                    triangs[i6 + 2] = i2 + 2;
                    triangs[i6 + 3] = i2 + 2;
                    triangs[i6 + 4] = i2 + 1;
                    triangs[i6 + 5] = i2 + 3;
                }

                bufferSize = triangs.Length * sizeof(uint);
                GL.GenBuffers(1, out EmbeddedSoilModel._IntIndicesBufferID);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EmbeddedSoilModel._IntIndicesBufferID);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSize), triangs, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                EmbeddedSoilModel._IntElementCount = triangs.Length;

                for (uint i = 0; i < EmbeddedSoilModel.TrenchSections; i++)
                {
                    uint i6 = i * 6;
                    uint t1 = i * 2;
                    uint b1 = EmbeddedSoilModel.DrawPoints + i;

                    triangs[i6 + 0] = t1;
                    triangs[i6 + 1] = b1;
                    triangs[i6 + 2] = b1 + 1;
                    triangs[i6 + 3] = b1 + 1;
                    triangs[i6 + 4] = t1 + 2;
                    triangs[i6 + 5] = t1;
                }

                bufferSize = triangs.Length * sizeof(uint);
                GL.GenBuffers(1, out EmbeddedSoilModel._IntIndicesBufferOrthoID);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EmbeddedSoilModel._IntIndicesBufferOrthoID);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSize), triangs, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                EmbeddedSoilModel._IntElementCountOrtho = triangs.Length;

                EmbeddedSoilModel._BoolSetupGL4 = true;
            }
        }

        private static void updateGL4()
        {
            if (UpdateSoilCountB)
                lock (EmbeddedSoilModel._V3SoilPoints)
                {
                    UpdateSoilCountB = false;
                    int bufferSize = EmbeddedSoilModel._V3SoilPoints.Length * Vector3.SizeInBytes;
                    GL.BindBuffer(BufferTarget.ArrayBuffer, EmbeddedSoilModel._IntInterleaveBufferID);
                    GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(bufferSize), EmbeddedSoilModel._V3SoilPoints);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                }
        }





















































































        public const float dimBoxWidth_Inches = 60, dimBoxHeight_Inches = 18, dimBoxThickness_Inches = 1;
        public const float dimBoxZ_Inches = -100, dimBoxX_Inches = 70;

        public static void drawBins(bool ShadowBufferDraw)
        {
            if (EmbeddedSoilModel._CadObjectBoxL == null)
            {
                EmbeddedSoilModel._CadObjectBoxL = CadObjectGenerator.fromXAML(
                    Properties.Resources.Box,
                    ObjectName: "Box Left",
                    xScale: StaticMethods.Conversion_Meters_To_Inches,
                    yScale: StaticMethods.Conversion_Meters_To_Inches,
                    zScale: StaticMethods.Conversion_Meters_To_Inches,
                    xOff: -EmbeddedSoilModel.dimBoxX_Inches,
                    yOff: 0.0f,
                    zOff: EmbeddedSoilModel.dimBoxZ_Inches);

                EmbeddedSoilModel._CadObjectBoxL.setColor(System.Drawing.Color.White, 0.15f, 0.5f, 0f, 0f);
                FormBase.Instance.addCadItem(EmbeddedSoilModel._CadObjectBoxL);            
            }
            else EmbeddedSoilModel._CadObjectBoxL.draw(!ShadowBufferDraw);

            if (EmbeddedSoilModel._CadObjectBoxR == null)
            {
                EmbeddedSoilModel._CadObjectBoxR = CadObjectGenerator.fromXAML(
                    Properties.Resources.Box,
                    ObjectName: "Box Right",
                    xScale: StaticMethods.Conversion_Meters_To_Inches,
                    yScale: StaticMethods.Conversion_Meters_To_Inches,
                    zScale: StaticMethods.Conversion_Meters_To_Inches,
                    xOff: EmbeddedSoilModel.dimBoxX_Inches,
                    yOff: 0.0f,
                    zOff: EmbeddedSoilModel.dimBoxZ_Inches);

                EmbeddedSoilModel._CadObjectBoxR.setColor(System.Drawing.Color.White, 0.15f, 0.5f, 0f, 0f);
                FormBase.Instance.addCadItem(EmbeddedSoilModel._CadObjectBoxR);
            }
            else EmbeddedSoilModel._CadObjectBoxR.draw(!ShadowBufferDraw);

            const float sidelen2 = dimBoxWidth_Inches / 2f;
            const float z = -EmbeddedSoilModel.dimBoxZ_Inches;
            float x = EmbeddedSoilModel.dimBoxX_Inches;

            EmbeddedSoilModel.setColorDirt();

            GL.Normal3(0, 1, 0);

            float h;

            GL.Begin(BeginMode.Quads);
            {
                h = EmbeddedSoilModel.soilheightL();
                GL.Vertex3(-(x - sidelen2), h, -(z + sidelen2));
                GL.Vertex3(-(x + sidelen2), h, -(z + sidelen2));
                GL.Vertex3(-(x + sidelen2), h, -(z - sidelen2));
                GL.Vertex3(-(x - sidelen2), h, -(z - sidelen2));

                x = -x;
                h = EmbeddedSoilModel.soilheightR();
                GL.Vertex3(-(x - sidelen2), h, -(z + sidelen2));
                GL.Vertex3(-(x + sidelen2), h, -(z + sidelen2));
                GL.Vertex3(-(x + sidelen2), h, -(z - sidelen2));
                GL.Vertex3(-(x - sidelen2), h, -(z - sidelen2));
            }
            GL.End();
        }

        public static void setColorDirt()
        {
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, EmbeddedSoilModel.ColorFrom(EmbeddedSoilModel._ColorDirt, 0.4f));
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, EmbeddedSoilModel.ColorFrom(EmbeddedSoilModel._ColorDirt, 0.6f));
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, EmbeddedSoilModel.ColorEmpty());
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, EmbeddedSoilModel.ColorEmpty());
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, new float[] { 0 });
        }

        private static float[] ColorEmpty() { return new float[] {0, 0, 0, 1}; }
        private static float[] ColorFrom(System.Drawing.Color a, float s)
        {
            return new float[] { (s * a.R) / 255, (s * a.G) / 255, (s * a.B) / 255, 1 };
        }


        private static CadObject _CadObjectBoxL = null;
        private static CadObject _CadObjectBoxR = null;


        // Accessed by Draw Thread Only
        // normals, vertices soil line, vertices along top (for ortho, only back side), 
        private static readonly Vector3[] _V3SoilPoints = new Vector3[DrawPoints + DrawPoints + DrawPointsPerSide];
        private static readonly RigidDynamic[] SoilBlocks = new RigidDynamic[TrenchSections];
        private static readonly bool[] SoilBlocksInScene = new bool[TrenchSections];
        private static readonly D6Joint[] SoilConstraints = new D6Joint[TrenchSections];
        static readonly System.Drawing.Color _ColorDirt = System.Drawing.Color.FromArgb(100, 80, 0);


        public static void InitializePhysX(Physics p, Scene s)
        {
            var grnd_material = p.CreateMaterial(0.2f, 0.1f, 0);
            grnd_material.FrictionCombineMode = CombineMode.Max;

            var d = new D6JointDrive(20000, 2000, float.MaxValue / 2, false);
            for (int i = 0; i < TrenchSections; i++)
            {
                var st = new Vector3_PhysX(0, 0, - StaticMethods.Conversion_Inches_To_Meters * InchesPerTrenchSection * (i + 0.5f));

                EmbeddedSoilModel.SoilBlocks[i] = p.CreateRigidDynamic(Matrix_PhysX.Translation(st));
                EmbeddedSoilModel.SoilBlocks[i].CreateShape(
                    new CapsuleGeometry(
                        StaticMethods.Conversion_Inches_To_Meters * InchesPerTrenchSection / 2 * 0.8f,
                        StaticMethods.Conversion_Inches_To_Meters * TrenchWidth / 2),
                    grnd_material,
                    Matrix_PhysX.Identity
                    );
                EmbeddedSoilModel.SoilBlocks[i].SetMassAndUpdateInertia(1);
                EmbeddedSoilModel.SoilBlocks[i].SleepThreshold = 0;

                EmbeddedSoilModel.SoilConstraints[i] = s.CreateJoint<D6Joint>(
                    EmbeddedSoilModel.SoilBlocks[i],
                    Matrix_PhysX.Identity,
                    null,
                    Matrix_PhysX.Translation(st));

                EmbeddedSoilModel.SoilConstraints[i].SetMotion(D6Axis.Y, D6Motion.Free);
                EmbeddedSoilModel.SoilConstraints[i].SetDrive(D6Drive.Y, d);
                EmbeddedSoilModel.SoilConstraints[i].DriveLinearVelocity = new Vector3_PhysX(0, 0, 0);
                EmbeddedSoilModel.SoilConstraints[i].DrivePosition = Matrix_PhysX.Identity;

                EmbeddedSoilModel.SoilBlocksInScene[i] = false;
            }
        }

        public static void TurnOnSoilAt(int i, Scene s, float hh)
        {
            if (!EmbeddedSoilModel.SoilBlocksInScene[i])
            {
                EmbeddedSoilModel.SoilBlocksInScene[i] = true;

                EmbeddedSoilModel.SoilBlocks[i].GlobalPose = Matrix_PhysX.Translation(0, -hh, 
                    -StaticMethods.Conversion_Inches_To_Meters * InchesPerTrenchSection * (i + 0.5f));

                s.AddActor(EmbeddedSoilModel.SoilBlocks[i]);
            }
        }

        public static void TurnOffSoilAt(int i, Scene s)
        {
            if (EmbeddedSoilModel.SoilBlocksInScene[i])
            {
                EmbeddedSoilModel.SoilBlocksInScene[i] = false;
                s.RemoveActor(EmbeddedSoilModel.SoilBlocks[i]);
            }
        }











































        private static readonly List<PileData> _PileData = new List<PileData>();

        public static volatile float _VolumeLeftBin;
        public static volatile float _VolumeRightBin;
        public static volatile float _VolumeNoBin;

        private static float soilheightL()
        {
            return 0.25f + Math.Min(dimBoxHeight_Inches - 1, _VolumeLeftBin / (dimBoxWidth_Inches * dimBoxWidth_Inches));
        }

        private static float soilheightR()
        {
            return 0.25f + Math.Min(dimBoxHeight_Inches - 1, _VolumeRightBin / (dimBoxWidth_Inches * dimBoxWidth_Inches));
        }

        private void NewPileDataComing(ref TrialSaver.File2_DataType data, Vector3_PhysX loc, float simtime)
        {
            if (Math.Abs(loc.X) < TrenchWidth / 2)
            {
                var eat_area = this.BucketVolume / TrenchWidth;
                this.BucketVolume = 0;
                var eat_length = 2 * (((int)Math.Round(Math.Sqrt(eat_area))) / 2);

                var eat_height = eat_area / (eat_length + 1);

                int zdex = (int)Math.Round(-loc.Z * TrenchSectionsPerInch);

                this.SoilHeight[zdex] += eat_height;
                for (int i = 1; i < eat_length; i++)
                {
                    var adh = (eat_height * (eat_length + 1 - i)) / (eat_length + 1);
                    this.SoilHeight[Math.Min(DrawPointsPerSide, zdex + i)] += adh;
                    this.SoilHeight[Math.Max(0, zdex - i)] += adh;
                }
            }
            else
            {
                var amtpile = this.BucketVolume;
                this.BucketVolume = 0;

                data.TimeMilis = (int)(simtime * 1000);
                data.Size = amtpile;
                data.dX = loc.X;
                data.dY = loc.Y;
                data.dZ = loc.Z;

                var zdiff = loc.Z - EmbeddedSoilModel.dimBoxZ_Inches;
                var xdiff = loc.X - EmbeddedSoilModel.dimBoxX_Inches * (loc.X < 0 ? -1 : 1);

                PileData dat = new PileData()
                {
                    loc = loc,
                    _FloatDropTime = simtime,
                    _FloatSoilAmount = amtpile / 5,
                    _EndY = 0,
                };

                //                amtpile *= 5;


                if ((Math.Abs(zdiff) < dimBoxWidth_Inches / 2) && (Math.Abs(xdiff) < dimBoxWidth_Inches / 2))
                {
                    if (loc.X < 0)
                    {
                        EmbeddedSoilModel._VolumeLeftBin += amtpile;
                        dat._EndY = EmbeddedSoilModel.soilheightL();
                        data.inBin = TrialSaver.File2_DataType.inBinLeft;
                    }
                    else if (loc.X > 0)
                    {
                        EmbeddedSoilModel._VolumeRightBin += amtpile;
                        dat._EndY = EmbeddedSoilModel.soilheightR();
                        data.inBin = TrialSaver.File2_DataType.inBinRight;
                    }
                }
                else
                {
                    EmbeddedSoilModel._VolumeNoBin += amtpile;
                    data.inBin = TrialSaver.File2_DataType.inBinMiss;
                }

                lock (EmbeddedSoilModel._PileData)
                    EmbeddedSoilModel._PileData.Add(dat);
            }
        }

        public static void drawPiles(float drawtime)
        {
            EmbeddedSoilModel.setColorDirt();

            float d_pile, temp;
            int k, n;

            lock (EmbeddedSoilModel._PileData)
            {
                foreach (var pd in EmbeddedSoilModel._PileData)
                {
                    temp = (3 * pd._FloatSoilAmount) / 4;
                    d_pile = 40;

                    // take cube root using Newton-Raphson method 
                    // should converge in less than 10 steps
                    // d_pile = 40;
                    for (n = 0; n < 30; n++) d_pile -= (d_pile * d_pile * d_pile - temp) / (3 * d_pile * d_pile);

                    // red clay / dirt
                    GL.Color3(0.5, 0.4, 0.4);

                    var dt = drawtime - pd._FloatDropTime;
                    var eye = pd._EndY;

                    GL_Handler.PushMatrix();
                    GL_Handler.Translate(pd.loc.X, (dt > 10) ? eye : Math.Max(eye, pd.loc.Y - dt * dt * 193.04f), pd.loc.Z);

                    for (k = 0; k < 4; k++)
                    {
                        GL_Handler.Rotate(22.5f, 0, 1, 0);

                        GL.Begin(BeginMode.Triangles);
                        {
                            PileData.glNormal3f(1, 0, 1);
                            PileData.glVertex3f(0, 0, d_pile);
                            PileData.glNormal3f(1, 0, 1);
                            PileData.glVertex3f(d_pile, -d_pile, 0);
                            PileData.glNormal3f(1, 0, 1);
                            PileData.glVertex3f(d_pile, d_pile, 0);

                            PileData.glNormal3f(-1, 0, 1);
                            PileData.glVertex3f(-d_pile, d_pile, 0);
                            PileData.glNormal3f(1, 0, 1);
                            PileData.glVertex3f(-d_pile, -d_pile, 0);
                            PileData.glNormal3f(1, 0, 1);
                            PileData.glVertex3f(0, 0, d_pile);

                            PileData.glNormal3f(0, 1, 1);
                            PileData.glVertex3f(d_pile, d_pile, 0);
                            PileData.glNormal3f(0, 1, 1);
                            PileData.glVertex3f(-d_pile, d_pile, 0);
                            PileData.glNormal3f(0, 1, 1);
                            PileData.glVertex3f(0, 0, d_pile);

                            PileData.glNormal3f(0, -1, 1);
                            PileData.glVertex3f(0, 0, d_pile);
                            PileData.glNormal3f(0, -1, 1);
                            PileData.glVertex3f(-d_pile, -d_pile, 0);
                            PileData.glNormal3f(0, -1, 1);
                            PileData.glVertex3f(d_pile, -d_pile, 0);
                        }
                        GL.End();
                    }
                    GL_Handler.PopMatrix();
                }
            }
        }

        private class PileData
        {
            internal Vector3_PhysX loc;
            internal float _EndY;
            internal float _FloatSoilAmount = 0;
            internal float _FloatDropTime = 0;

            internal static void glVertex3f(float a, float b, float c) { GL.Vertex3(-b, c, -a); }
            internal static void glNormal3f(float a, float b, float c) { GL.Normal3(-b, c, -a); }
            internal static void glRotatef(float g, float a, float b, float c) {  }        
        }

        internal void Reset()
        {
            lock (this.SoilHeight)
            {
                for (int i = 0; i < DrawPointsPerSide; i++)
                {
                    this.SoilHeight[i] = 0;
                }
            }

            this.BucketVolume = 0;

            EmbeddedSoilModel._VolumeLeftBin = 0;
            EmbeddedSoilModel._VolumeNoBin = 0;
            EmbeddedSoilModel._VolumeRightBin = 0;

            lock (EmbeddedSoilModel._PileData)
            {
                EmbeddedSoilModel._PileData.Clear();
            }
        }
    }
}
