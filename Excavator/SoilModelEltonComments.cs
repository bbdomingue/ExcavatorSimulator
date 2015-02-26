using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SamSeifert.GLE.Generics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Excavator
{
    public static class SoilModel
    {
        const int _IntDiscreteSectionsMatlab = 256;			// number of nodes
        const float GroundLevel = -20.0f;	// inches
        const float	MaxDepth = 150;			// inches
        const float trenchWidth = 24;       // inches
        const float TrenchStart = 0;        // inches
        const float TrenchEnd = 250;        // - 31.5423) // inches	
        const int NumStatesK_c = 3;
        const int NumStatesFilter = 8;
        const int NumStatesLoad = 2;
        const float Max_V_Load = 7200;

        const int sidelen = 60;				//length of the sides of the box

        const float YGround = GroundLevel + Bobcat._SamCatLiftHeight + Bobcat._V3RotationOffsetY;				// inches (defined in SoilParam)
        const float YTrenchBottom = YGround - MaxDepth; // inches (defined in SoilParam)
        const float XhalfL = -trenchWidth / 2;
        const float XhalfR = trenchWidth / 2;
        const float ZStartTrench = TrenchStart;				// inches
        const float ZEndTrench = TrenchEnd;					// inches
        const float dX = (ZEndTrench - ZStartTrench) / 257;			// X resolution

        const int _IntDiscreteSectionsRender = _IntDiscreteSectionsMatlab + 2;
        const int _IntTopDrawPoints = _IntDiscreteSectionsRender * 2;

        public const float XTextL = Trial._IntTextureDensity * (0.5f + (XhalfL / (2 * Trial._FloatGroundPlaneDim)));
        public const float XTextR = Trial._IntTextureDensity * (0.5f - (XhalfL / (2 * Trial._FloatGroundPlaneDim)));
        public const float ZTextBehind = Trial._IntTextureDensity * (0.5f - (ZStartTrench / (2 * Trial._FloatGroundPlaneDim)));
        public const float ZTextFront = Trial._IntTextureDensity * (0.5f - (ZEndTrench / (2 * Trial._FloatGroundPlaneDim)));

        const int NumStates =
            _IntDiscreteSectionsMatlab * 2 +
            NumStatesK_c +
            NumStatesFilter +
            NumStatesLoad + 1 + 3 + 1 + 1;//1 (intrench) 4 pile states 1 reset state


        static Vector3[] idata = new Vector3[_IntTopDrawPoints * 2];
        
        static volatile Byte[] Y_SOIL = new Byte[_IntDiscreteSectionsMatlab];
        static UInt16[] MODE = new UInt16[] { 0, 0, 0, 0 };
        static System.Boolean[] FLAG = new System.Boolean[16];
        static float[] STATES = new float[SoilModel.NumStates];

        static SoilModel()
        {
            int FLAG_INT = (int)(Math.Pow(2, 13) + Math.Pow(2, 14));

            for (int k = 0; k < 16; k++)
            {
                int mask = (int)Math.Pow(2, k);
                SoilModel.FLAG[k] = (FLAG_INT & mask) > 0;
            }
        }

        public unsafe static void Soil_Force_Model(
            ref float[] POS, ref float[] VEL, ref float[] Q, ref float[] QD, // Input
            ref float[] FORCE
            )
        {
            // Inputs
            
	        // States
	        // int numStates = ssGetNumDiscStates(S);

	        // Outputs
	        UInt16[] LOAD = new UInt16[2];
	        float[] MISC = new float[20];
            float[] PILE = new float[4];   

	        // Parameters SAM TOOK OUT OF MATLAB SIMULATION BLOCK PARAMS
	        float Ts = 0.001f;
            float distal_flag = 0;
	        float kt = 200;				// normal stiffness
	        float kn = 200;				// tangential stiffness
	        float ktheta = 500;				// rotational stiffness
	        float bt = 200;				// normal stiffness
	        float bn = 200;				// tangential stiffness
	        float btheta = 400;				// rotational stiffness
	        float ShearStrength = 14.5038f;		// Shear Strength
	        float Tau_rt = 0.1f;			// time constant - tangential
	        float Tau_rn = 0.1f;			// time constant -  normal
	        float k_obj = 1062090.0f;			// stiffness of buried objects													
	        /************************************************************
				        variable and parameters definitions
	        ************************************************************/
	
	        // input variables
	        float x, y, z, phi;
	        float vx, vy, vz, vphi, vr, vn, vk, vt, vtheta;
	        float t1, t2, t3, t4;

	        // Trig and geometric variables
	        float c1, c2, c23, c234;
	        float s1, s2, s23, s234;
	        float ac2, ac3, ac4, ac23, ac34, ac24;
	        float as2, as3, as4, as23, as34, as24;

	        // soil calculation variables
	        float delta_R = (SoilModel.TrenchEnd - SoilModel.TrenchStart)/( (float) (SoilModel._IntDiscreteSectionsMatlab + 1));
	        float R34, Z34, R03, Z03, R04, Z04 = 0.0f, T04, N04, Delta = 0.42479226265574266f, cA, sA;
	        float Sp, Sbucket = 12;
	        float Rs, Zs, ms, bs, dZ_last, Zsoil, r1, r2, z1, z2, r1wrist, r2wrist, z1wrist, z2wrist;
	        int n0 = 0, n1, n2, n3 = 0, np, n1wrist;

            float[] SOIL = new float[SoilModel._IntDiscreteSectionsMatlab];
            float[] SOIL_dump = new float[SoilModel._IntDiscreteSectionsMatlab];

	        // stiffness center and shear variables
	        float X_c, Y_c, Z_c, R_c, N_c, T_c, Theta_c, Alpha_c, a_of_a;
	        float a1t, b0t, b1t, a1n, b0n, b1n;
	        float Vbk = 0, Stall_Ratio;

	        // stiffness variables
	        float Fkn=0, Fkt=0, Tka=0, Fkr=0, Fkz=0, Tkphi=0; 

	        // shear and soil update variables
	        float Vload, Vpile, dp = 1, delta_r, delta_z, Spill_Ratio;

	        // force and torqeu output variables
	        float Fx = 0, Fy = 0, Fr = 0, Fz = 0, TauPhi = 0, temp, Ft, Ftheta;

            float vtipx, vtipy, vtipz, vtipr, vtiptheta, vtipt, vtipn;
    
            float signvtipx = -1;
            float signvtipy = -1;
            float signvtipz = -1;
            float signvphi = -1;
            float signaofa = -1;
            float signxc = -1;
            float signyc = -1;
            float signzc = -1;
            float signac = -1;    
            float signvz = -1;
            float signvy = -1;
            float signvx = -1;   
    
            float signvtipr = -1;
            float signvtipt = -1;
            float signvtipn = -1;
            float signvtiptheta = -1;
            float signtc = -1;
            float signnc = -1;
            float signrc = -1;    
            float signthetac = -1;

	        // misc
	        int k, region, p, count, sub1, sub2, signn12 = 1, steps;
            float soilheight, height, wedgevol, base_var, soilrepose;
            float x04, y04, z04, x34, y34, intrench, repeet, diff;
            float pilex = 0;
            float piley = 0;
            float Fxw = 0;
            float Fyw = 0;
            float Fzw = 0;
            float Mx = 0, Mxw = 0;
            float My = 0, Myw = 0;
            float Mz = 0, Mzw = 0; 
            float kx, ky, kz, ka, bx, by, bz, ba, kxw, kyw = 0, kzw, kaw, bxw, byw = 0, bzw, baw, kk;
            //float kn, kt, ktheta, bn, bt, btheta;
            float k_n = 120000/3*0.0254f/4.448221615f;
            float Fxspring, Fyspring, Fzspring, Fxdamp, Fydamp, Fzdamp;
            float Fnspring = 0, Ftspring = 0, Fthetaspring = 0, Fndamp = 0, Ftdamp = 0, Fthetadamp = 0;    
            float Mxspring = 0, Myspring = 0, Mzspring, Mxdamp = 0, Mydamp = 0, Mzdamp;    
            float Fxwspring = 0, Fywspring = 0, Fzwspring = 0, Fxwdamp = 0, Fywdamp = 0, Fzwdamp = 0;
            float Mxwspring = 0, Mywspring = 0, Mzwspring = 0, Mxwdamp = 0, Mywdamp = 0, Mzwdamp = 0;  
            float Fn, Fnr, Fnz = 0, Fnx = 0, Fny = 0;
    
	        /************************************************************
				        Reset Soil using Omni and MODE flag
	        ************************************************************/
	        if ( MODE[0] == 0 )
	        {
		        if ( Z04 > SoilModel.GroundLevel )
		        {
			        for (k = 0; k < SoilModel._IntDiscreteSectionsMatlab; k++){
				        STATES[k] = SoilModel.GroundLevel;					// Soil States
				        STATES[SoilModel._IntDiscreteSectionsMatlab + k] = SoilModel.GroundLevel;		// Dump Soil States
			        }
			        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 0] = 777;					// X_c
			        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 1] = 777;					// Z_c
			        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 2] = 777;					// Alpha_c
			
			        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 11] = 0;					// Vload 
			        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 12] = 0;					// Vpile
                    STATES[2*SoilModel._IntDiscreteSectionsMatlab + 13] = 0;					// intrench   
                    STATES[2*SoilModel._IntDiscreteSectionsMatlab + 14] = 0;					// xpile
                    STATES[2*SoilModel._IntDiscreteSectionsMatlab + 15] = 0;					// ypile
                    STATES[2*SoilModel._IntDiscreteSectionsMatlab + 16] = 0;					// pile size
                    STATES[2*SoilModel._IntDiscreteSectionsMatlab + 17] = 777;				// Y_c
                    STATES[2*SoilModel._IntDiscreteSectionsMatlab + 18] = SoilModel.GroundLevel - 50;	// Zpile  

		        }



                MODE[0] = 17;// SAM
            }

	        /************************************************************
				        Unpack States
	        ************************************************************/
	        for (k = 0; k < SoilModel._IntDiscreteSectionsMatlab; k++) {
		        SOIL[k] = STATES[k];
		        SOIL_dump[k] = STATES[SoilModel._IntDiscreteSectionsMatlab + k];
	        }

	        // center of stiffness
	        X_c = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 0];
	        Z_c = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 1];
	        Alpha_c = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 2];

	        // relaxation filter parameters
	        a1t = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 5];
	        b0t = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 6];
	        b1t = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 7];
	        a1n = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 8];
	        b0n = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 9];
	        b1n = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 10];

	        // Volume of soil in the bucket
	        Vload = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 11];
	        Vpile = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 12];
            intrench = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 13];
    
            PILE[0] = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 14];									// xpile
            PILE[1] = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 15];									// ypile
            PILE[3] = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 16];								// pile size
            PILE[2] = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 18];								// pile size    
            Y_c = STATES[2*SoilModel._IntDiscreteSectionsMatlab + 17];									// Y_c    

	        /************************************************************
				        Unpack Input Variables
	        ************************************************************/
			
	        // Position
	        x = POS[0];
	        y = POS[1];
	        z = POS[2];
	        phi = POS[3];

	        // Velocity
	        vx = VEL[0];
	        vy = VEL[1];
	        vz = VEL[2];
	        vphi = VEL[3];

	        // joint angles
	        t1 = Q[0];
	        t2 = Q[1];
	        t3 = Q[2];
	        t4 = Q[3];
    
            vtipx = (float)(vx - Math.Sin(t2 + t3 + t4) * Bobcat.a4_sim * Math.Cos(t1) * vphi);
            vtipy = (float)(vy - Math.Sin(t2 + t3 + t4) * Bobcat.a4_sim * Math.Sin(t1) * vphi);
            vtipz = (float)(vz + Math.Cos(t2 + t3 + t4) * vphi * Bobcat.a4_sim);
	
	        /************************************************************
				        Bucket tip location and angle of attach
	        ************************************************************/
	        c1 = (float)Math.Cos(t1);
            s1 = (float)Math.Sin(t1);
            R03 = (float)Math.Sqrt(x * x + y * y);
	        Z03 = z;
            R34 = ((float)Math.Cos(t2 + t3 + t4)) * 30;//a4; //since bucket teeth are removed in graphics simulator use 30 rather than a4
            Z34 = ((float)Math.Sin(t2 + t3 + t4)) * 30;//a4; 
	        R04 = R03 + R34;
	        Z04 = Z03 + Z34;
            a_of_a = (t2 + t3 + t4) + StaticMethods._PIF/2 + Delta;     //angle of attack
            cA = (float)Math.Cos(a_of_a);
            sA = (float)Math.Sin(a_of_a);

            x34 = R34*c1;
            y34 = R34*s1;
    
            x04 = x34 + x;
            y04 = y34 + y;
            z04 = Z04;
    
            r1 = R04 - SoilModel.TrenchStart;
            r2 = R04 + Sbucket*cA - SoilModel.TrenchStart;
            n1 = (int)Math.Floor(r1/delta_R);// - 1;
            n2 = (int)Math.Ceiling(r2/delta_R);// - 1;

            r1wrist = R03 - SoilModel.TrenchStart;
            n1wrist = (int)Math.Floor(r1wrist/delta_R);// - 1;    
    
           /*//////////////////////////////////////////////////////*
                            determine region
           /*//////////////////////////////////////////////////////*    
            region = Math.Abs(y04 * 2) > SoilModel.trenchWidth ? 3 : 1;
            region *= z04 < SoilModel.GroundLevel ? -1 : 1;
    
            if ((region == 1) && (z04 < SOIL[n1])) region = -1;         
            if ((intrench == 1) && (region == -3)) region = -2;

            intrench = (region == -1) || (region == -2) ? 1 : 0;
      
            /************************************************************
                        Calculate contact surface length: Sp
            ************************************************************/
            ms = (float)Math.Tan(a_of_a);       // sA/cA;				// slope: tan(Alpha)
            bs = Z04 - ms*R04;		// intercept
            np = 0;

            if ((n2-n1) == 0)
                { signn12 = 1; }    //stop divide by zero err
            else
                { signn12 = (n2-n1)/Math.Abs(n2-n1); }

            for (k = n1; k <= n2; k = k+signn12) 
            {
                Zs = ms*(SoilModel.TrenchStart + (k)*delta_R) + bs;
                if (k == n1)
                    { Zs = Z04; }
                if (k == n2)
                    { Zs = Z03; }            
                if (k > 255) 
                    {k = 255;}     ///hmmmmmm...........this should never happen in the first place

                Zsoil = SOIL[k]; //k+1?

                if (Zs <= Zsoil)
                    { np = np+1; }      // number of points of contact;
            }

            if ((n2-n1) == 0) //stop divide by zero
                { n2 = n1+1; }

            Sp = Sbucket*np/(n2-n1)*signn12;
            if ( Sp > Sbucket )     //happens when teeth are vertical
                { Sp = Sbucket; }

            /************************************************************
                        Update center of Stiffness & calculate stiffness force
            ************************************************************/
            //n1 = (int_T) floor((R04 - SoilModel.TrenchStart)/delta_R) - 1;
            n2 = n1+1;
            r1 = SoilModel.TrenchStart + (n1)*delta_R;
            r2 = r1 + delta_R;

            r1wrist = SoilModel.TrenchStart + (n1wrist)*delta_R;
            r2wrist = r1wrist + delta_R;        

            if ((intrench == 1) || (region == -1))
            {
                z1 = SOIL[n1];
                z2 = SOIL[n2];
                Zsoil = z1 + (R04 - r1)*(z2 - z1)/(r2 - r1);
                z1wrist = SOIL[n1wrist];
                z2wrist = SOIL[n1wrist+1];
                z1wrist = z1wrist + (R03 - r1wrist)*(z2wrist - z1wrist)/(r2wrist - r1wrist);
            }
            else
            {
                z1 = SoilModel.GroundLevel;//SOIL_Old[n1];
                z2 = SoilModel.GroundLevel;//SOIL_Old[n2];
                Zsoil = SoilModel.GroundLevel;//z1 + (R04 - r1)*(z2 - z1)/(r2 - r1);
                z1wrist = SoilModel.GroundLevel;
            }

            if ( Z_c == 777 ) // if Z_c = 777 then there was no contact point in z^{-1}
            {
                if ( Zsoil >= Z04 )  // bucket tip enters the soil
                {
                    Z_c = Zsoil;
                    X_c = x04;
                    Y_c = y04;
                    Alpha_c = a_of_a;
                }
            }
            else if ( Zsoil < Z04 ) //tip out of soil
            {
                X_c = 777;
                Y_c = 777;
                Z_c = 777;
                Alpha_c = 777;     
            }
            else if ((intrench == 1) || (region == -1))//(( Tau_rt >= 0 ) && ( Tau_rn >= 0 )) // if either is negative, then no relaxation // bucket tip continues through soil
            {
                    // adjust update based on amount of soil in bucket
                    Vbk = 0;
                    for ( k = 0; k < SoilModel._IntDiscreteSectionsMatlab; k++ )
                    {
                        // trapezoidal integration
                        Vbk = Vbk + ((float) SoilModel.trenchWidth)*delta_R*(SOIL_dump[k] - SOIL[k]);
                    }
                    Vload = Vbk;
            
                    if ( Vload <= ( SoilModel.Max_V_Load ) )  // all the dirt stays in the bucket
                    {
                    }
                    else	// some dirt spills from bucket due to over filling
                    {
        //                 soilrepose = pi/12;
        //                 wedgevol = (Vload - SoilModel.Max_V_Load);
        //                 if (vtipx < 0)
        //                 {
        //                     for (k = n1-1; k >= 0; k--)
        //                     {
        //                         if ( SOIL[k] > SOIL_dump[k] )
        //                         {
        //                             wedgevol = wedgevol + ((SOIL[k] - SOIL_dump[k])*((float) SoilModel.Width)*delta_R);
        //                         }
        //                         else
        //                         {
        //                             break;
        //                         }
        //                     }
        //                 }
        //                 else
        //                 {  
        //                      for (k = n1+1; k < SoilModel.NumOutputs; k++)
        //                     {
        //                         if ( SOIL[k] > SOIL_dump[k] )
        //                         {
        //                             wedgevol = wedgevol + ((SOIL[k] - SOIL_dump[k])*((float) SoilModel.Width)*delta_R);
        //                         }
        //                         else
        //                         {
        //                             break;
        //                         }
        //                     }
        //                 }
        //                 base_var = sqrt(wedgevol/SoilModel.Width*2/tan(soilrepose));
        //                 height = base_var*tan(soilrepose);
        //                 if (vtipx < 0)
        //                 {
        //                     steps = floor(base_var/delta_R);
        //                     for (k = 0; k < steps; k++)
        //                     {
        //                         if ((n1-1-k) > 0)
        //                         {
        //                             SOIL[n1-1-k] = SOIL_dump[n1-1-k] + height*(steps-k)/steps;
        //                         }
        //                     }
        //                 }
        //                 if (vtipx > 0)
        //                 {
        //                     steps = floor(base_var/delta_R);
        //                     for (k = 0; k < steps; k++)
        //                     {
        //                         if ((n1+1+k) < SoilModel.NumOutputs)
        //                         {
        //                             SOIL[n1+1+k] = SOIL_dump[n1+1+k] + height*(steps-k)/steps;
        //                         }
        //                     }
        //                 }
        //                 // calculate any extra leftover
        //                 Vbk = 0;
        //                 for ( k = 0; k < SoilModel.NumOutputs; k++ )
        //                 {
        //                     // trapezoidal integration
        //                     Vbk = Vbk + ((float) SoilModel.Width)*delta_R*(SOIL_dump[k] - SOIL[k]);
        //                 }
        //                 Vload = Vbk;   
                
                
        //                     height =(Vload - SoilModel.Max_V_Load)/(SoilModel.Width*delta_R);
        // //                     if (height > 50)
        // //                         {height = 50;}
        //                     if ( vtipx < 0)
        //                     {
        //                         SOIL[n1-1] =  SOIL[n1-1] + height;//(Vload - SoilModel.Max_V_Load)/(SoilModel.Width*delta_R);
        //                     }
        //                     else
        //                     {
        //                          SOIL[n1+1] =  SOIL[n1+1] + height;//(Vload - SoilModel.Max_V_Load)/(SoilModel.Width*delta_R);                       
        //                     }
        //                     repeet = 1;
        //                     count = 0;
        //                     while (repeet == 1)
        //                     {
        //                         repeet = 0;
        //                         if (vtipx < 0)
        //                         {
        //                             if ((n1 - 1) > 0)
        //                             {   
        //                                 for (p = n1-1; p > 0; p--)
        //                                 {
        //                                     if ((SOIL[p]-1) > SOIL[p-1])
        //                                     {
        //                                         diff = SOIL[p] - SOIL[p-1];
        //                                         SOIL[p] = SOIL[p] - diff/2;
        //                                         SOIL[p-1] = SOIL[p-1] + diff/2;
        //                                     }
        //                                     if (p < n1 - 50)
        //                                     {
        //                                         break;  // break loop
        //                                     }
        //                                 }
        //                                 for (p = n1-1; p > 0; p--)
        //                                 {
        //                                     if ((SOIL[p]-1) > SOIL[p-1])
        //                                     {
        //                                         repeet = 1;
        //                                         count++;
        //                                         if (count > 50)
        //                                         {
        //                                             repeet = 0;
        //                                             break;
        //                                         }
        //                                     }
        //                                     if (p < n1 - 50)
        //                                     {
        //                                         break;  // break loop
        //                                     }                                    
        //                                 }   //for
        //                             }   //if
        //                         }
        //                         else
        //                         {
        //                             for (p = n1+1; p < SoilModel.NumOutputs-1; p++)
        //                             {
        //                                 if ((SOIL[p]-1) > SOIL[p+1])
        //                                 {
        //                                     diff = SOIL[p] - SOIL[p+1];
        //                                     SOIL[p] = SOIL[p] - diff/2;
        //                                     SOIL[p+1] = SOIL[p+1] + diff/2;
        //                                 }
        //                                 if (p > n1 + 50)
        //                                 {
        //                                     break;//p = SoilModel.NumOutputs;  // break loop
        //                                 }                                  
        //                             }
        //                             count = 0;
        //                             for (p = n1+1; p < SoilModel.NumOutputs-1; p++)
        //                             {
        //                                 if ((SOIL[p]-1) > SOIL[p+1])
        //                                 {
        //                                     repeet = 1;
        //                                     count++;
        //                                     if (count > 50)
        //                                     {
        //                                         repeet = 0;
        //                                         break;
        //                                     }
        //                                 }
        //                                 if (p > n1 + 50)
        //                                 {
        //                                     break;//p = SoilModel.NumOutputs;  // break loop
        //                                 }                                
        //                             }   //for
        //                         }   // else                            
        //                     }   // while
                
                
                
                        // add extra dirt evenly across trench compared to where is it was when last dumped
                        Spill_Ratio = (( Vload) - ( SoilModel.Max_V_Load))/( Vload);
                        for ( k = 0; k < SoilModel._IntDiscreteSectionsMatlab; k++ )
                        {
                            SOIL[k] = SOIL[k] + (SOIL_dump[k] - SOIL[k])*Spill_Ratio;
                        }
                        Vload = ( SoilModel.Max_V_Load );
                    }   // dirt spills
                    Vbk = Vload;

                /************************************************************
                            Calculate Shear Plane - Update soil
                ************************************************************/
                    //n0 = (int_T) floor((R04 + (Z04 - SoilModel.GroundLevel)*sA/cA - SoilModel.TrenchStart)/delta_R);// - 1;
                    n0 = (int)Math.Floor((R04 + (Z04 - SoilModel.GroundLevel)*cA/sA - SoilModel.TrenchStart)/delta_R);// - 1;
                    n1 = (int)Math.Floor((R04 - SoilModel.TrenchStart) / delta_R);// - 1;
                    n2 = (int)Math.Ceiling((R04 + Sbucket * cA - SoilModel.TrenchStart) / delta_R);//- 1;
                    if ( n2 >= SoilModel._IntDiscreteSectionsMatlab )
                        n2 = SoilModel._IntDiscreteSectionsMatlab-1;

                    if ((region == -1) || (region == -2))
                    {
                        //update soil "cutting path"
                        Zs = ms*(SoilModel.TrenchStart + (n1+1)*delta_R) + bs;
                        if (Zs < z04)
                            Zs = z04;
                        if (Zs < SOIL[n1])
                            SOIL[n1] = Zs;
                    }
                    //find shear point
                    if ( n0 >= n1 )
                    {
                        n0 = n1;
                    }
                    else if ( n0 < 0 )
                    {
                        n0 = 0;
                    }
                    else
                    {
                        np = n0;
                        temp = 0;
                        for(k = n0; k <= n1; k++) //check to make sure you don't pop out of the soil early?
                        {
                            delta_r = SoilModel.TrenchStart + (k+1)*delta_R - R04;
                            delta_z = SOIL[n0] - Z04;
                            dp = (float)Math.Sqrt(delta_r*delta_r + delta_z*delta_z);
                            if (dp == 0) { dp = 1;} //no divide by zero
                            //take dot product to find most orthogonal
                            delta_r = delta_r/dp;				// normalize
                            delta_z = delta_z/dp;				// normalize
                            dp = -sA*delta_r + cA*delta_z;		// dot product	
                            if (dp > temp) {
                                temp = dp;
                                np = k;
                            }
                        }
                        n0 = np;								// most orthogonal point
                    }

                    delta_r = SoilModel.TrenchStart + (n0+1)*delta_R - R04;
                    delta_z = SOIL[n0] - Z04;
                    dp = (float)Math.Sqrt(delta_r*delta_r + delta_z*delta_z);

                    if (( SoilModel.trenchWidth*dp*ShearStrength < -Fkn ) && (intrench == 1)) //Soil never shears outside trench
                    {
                        X_c = x04;
                        Z_c = Z04;
                        Y_c = y04;
                        Alpha_c = a_of_a;
                    }  
            }

            /************************************************************
                        Calculate - stiffness force
            ************************************************************/
            if ( Z_c != 777 )   // bucket is in the soil
            {
                Fkn = k_n * (-sA * (float)Math.Sqrt((X_c - x04) * (X_c - x04) + (Y_c - y04) * (Y_c - y04)) + cA * (Z_c - Z04));
        
                vr = vx*c1 + vy*s1;
                vtheta = -vx*s1 + vy*c1;
                vtipr = vtipx*c1 + vtipy*s1;
                vtiptheta = -vtipx*s1 + vtipy*c1;
               
                vt =  cA*vr + sA*vz;
                vn = -sA*vr + cA*vz;
                vtipt =  cA*vtipr + sA*vtipz;
                vtipn = -sA*vtipr + cA*vtipz;        

                R_c = (float)Math.Sqrt(X_c*X_c + Y_c*Y_c);
                N_c = -sA*R_c + cA*Z_c;
                T_c =  cA*R_c + sA*Z_c;  
                N04 = -sA*R04 + cA*Z04;
                T04 =  cA*R04 + sA*Z04;
                Theta_c = (float)Math.Atan2(Y_c,X_c);
        
                /************************************************************
                            Calculate forces
                ************************************************************/
                if (vtipt >= 0) {signvtipt = 1;}
                if (vtipn >= 0) {signvtipn = 1;}
                if (vtiptheta >= 0) {signvtiptheta = 1;}
        
                if ((Theta_c - (t1 + Math.Atan2(y04,x04))) >= 0) {signthetac = 1;}
                if ((T_c - T04) >= 0) {signtc = 1;}
                if ((N_c - N04) >= 0) {signnc = 1;}
        
                if (vphi >= 0) {signvphi = 1;}            
                if ((Alpha_c - a_of_a) >= 0) {signac = 1;}           

                if (signvtipt == signtc)
                {
                    kt = kt/10; //0?
                    bt = bt/100;
                }
        //         else 
        //         {
        //             kt = 100;
        //             bt = 100;
        //         }
                if (signvtipn == signnc)
                {
                    kn = kn/10;
                    bn = bn/100;
                }
        //         else 
        //         {
        //             kn = 100;
        //             bn = 100;
        //         }
                if (signvtiptheta == signthetac)
                {
                    ktheta = ktheta/10;
                    btheta = btheta/100;
                }
        //         else 
        //         {
        //             ktheta = 100;
        //             btheta = 100;
        //         }
                if (signvphi == signac)
                {
                    ka = 10;//was 10
                    ba = 1;//was 1
                }
                else 
                {
                    ka = 100;
                    ba = 100;
                }
                
                //spring force
                Fnspring = kn*(N_c - N04); //x
                Ftspring = kt*(T_c - T04); //y
                Fthetaspring = ktheta * (Theta_c - (t1 + (float)Math.Atan2(y04, x04))); //z
                Mxspring =  s1*ka*(Alpha_c - a_of_a); //x
                Myspring = -c1*ka*(Alpha_c - a_of_a); //y 
                Mzspring = 0; //z 

                //damping force
                Fndamp = (-bn*vtipn);
                Ftdamp = (-bt*vtipt);
                Fthetadamp = (-btheta*vtiptheta);
                Mxdamp = -c1*vphi*ba; //x
                Mydamp =  s1*vphi*ba; //y 
                Mzdamp = 0; //z 
        
        //         //spring force
        //         Fxspring = kx*(X_c - x04); //x
        //         Fyspring = ky*(Y_c - y04); //y
        //         Fzspring = kz*(Z_c - z04); //z
        //         Mxspring =  s1*ka*(Alpha_c - a_of_a); //x
        //         Myspring = -c1*ka*(Alpha_c - a_of_a); //y 
        //         Mzspring = 0; //z 
        // 
        //         //damping force
        //         Fxdamp = (-bx*vtipx);
        //         Fydamp = (-by*vtipy);
        //         Fzdamp = (-bz*vtipz);
        //         Mxdamp = -c1*vphi*ba; //x
        //         Mydamp =  s1*vphi*ba; //y 
        //         Mzdamp = 0; //z  
            }

            /************************************************************
                        Force Sensor calculations
            ************************************************************/
            if ( Z_c == 777 ) 
            {
                // set forces to zero, bucket is not in soil
                Ft = 0;
                Fn = 0;
                Ftheta = 0;
                TauPhi = 0;
            }
            else 
            {
                // add up all force components
                Fn = (Fnspring + Fndamp)*Sp;//+Fnx;
                Ft = (Ftspring + Ftdamp)*Sp;//+Fny;
                Ftheta = (Fthetaspring + Fthetadamp)*Sp;//+Fnz;
                Mx = (Mxspring + Mxdamp);//*Sp;
                My = (Myspring + Mydamp);//*Sp;
            }

            if (((t2 + t3 + t4) > (-100 * StaticMethods._PIF / 180)) && (Vload > 0)) // dirt falls out of bucket if you reach -100deg
            {
                if ((intrench == 0) && (region != 1))       // dump outside trench
                {
                    Vpile = Vpile + (Vload/5);
                    PILE[3] = Vload/5;
                    if (PILE[3] == 2222)    // 2222 is code for dumping in trench - see piles.h in graphics simulator
                    {
                        PILE[3] = 2223;
                    }
                    PILE[2] = Z04;
                    PILE[1] = y04 - (0.5f * Bobcat.a4_sim) * s1;    //put pile in front of bucket tip
                    PILE[0] = x04 - (0.5f * Bobcat.a4_sim) * c1;    //put pile in front of bucket tip
                    Vload = 0;
                    Vbk = 0;
                    for ( k = 0; k < SoilModel._IntDiscreteSectionsMatlab; k++ )
                    {
                        SOIL_dump[k] = SOIL[k];
                    }            
                }
                if ((intrench == 0) && (region == 1))       // dump in trench
                {   
                    soilheight = Vload/(SoilModel.trenchWidth*delta_R);
                    n3 = (int) Math.Floor((r1+r1wrist)/2);
                    for(k = 1; k <= 25; k++)
                    {
                            sub1 = n3 + k;
                            sub2 = n3 - k;
                            if (sub1 > 255)
                            {
                                sub1 = sub2;
                            }
                            if (sub2 < 0)
                            {
                                sub2 = sub1;
                            }
                            SOIL[sub1] = SOIL[sub1] + (.485f / 25 + (13 - k) * 0.001f) * soilheight; //48.5% behind the middle of the stack
                            SOIL[sub2] = SOIL[sub2] + (.485f / 25 + (13 - k) * 0.001f) * soilheight; //48.5% in front of the middle
                    }
                    SOIL[n3] = SOIL[n3] + .03f * soilheight; // middle has 3% of the dump amt         
                    Vload = 0;
                    Vbk = 0;
                    for ( k = 0; k < SoilModel._IntDiscreteSectionsMatlab; k++ )
                    {
                        SOIL_dump[k] = SOIL[k];
                    }
                    PILE[3] = 2222;
                    PILE[2] = Z04;
                    PILE[1] = y04 - (.5f * Bobcat.a4_sim) * s1;    //put pile in front of bucket tip
                    PILE[0] = x04 - (.5f * Bobcat.a4_sim) * c1;    //put pile in front of bucket tip
                }
	        }
    
            // wrist force
            if (vz >= 0) {signvz = 1;}
            if (Z03 <= z1wrist) 
            {
                if (signvz == 1)
                {
                    kzw = 1;
                    bzw = 1;
                }
                else 
                {
                    kzw = 100;
                    bzw = 10;
                }
                Fzwspring = kzw*(z1wrist - z);  //spring force  
                Fzwdamp = (-bzw*vz);            //damping force
            }
            else
            {
                Fzwspring = 0;
                Fzwdamp = 0;     
            }
    
            if ((region == -2) || ((region == -1) && (Math.Abs(t1) > .1))) 
            {
                if (vy >= 0) {signvy = 1;}
                if (y04 > 0)
                {   
                    if (signvy < 0)
                    {
                        kyw = 10;
                        byw = 1;
                    }
                    else 
                    {
                        kyw = 100;
                        byw = 1;
                    }
                }
                if (y04 < 0)
                {   
                    if (signvy < 0)
                    {
                        kyw = 100;
                        byw = 1;
                    }
                    else 
                    {
                        kyw = 10;
                        byw = 1;
                    }
                } 
                Fywspring = kyw*(-y);
                Fywdamp = (-byw*vy);
            }
            else
            { 
                Fywspring = 0;
                Fywdamp = 0;         
            }    
    
            Fzw = Fzwspring + Fzwdamp;
            Fyw = Fywspring + Fywdamp;
    
            Fr = Ft*cA - Fn*sA;
            Fz = Ft*sA + Fn*cA;
            Fx = Fr * (float)Math.Cos(t1 - Bobcat.q2) - Ftheta * (float)Math.Sin(t1 - Bobcat.q2);
            Fy = Fr * (float)Math.Sin(t1 - Bobcat.q2) + Ftheta * (float)Math.Cos(t1 - Bobcat.q2);

	        /************************************************************
				        Update Force Output Vector
	        ************************************************************/
            // negative for newton euler eqtns (force of machine on soil)
	        FORCE[0] = -Fx;
	        FORCE[1] = -Fy;
	        FORCE[2] = -Fz;
	        FORCE[3] = -Mx;
	        FORCE[4] = -My;
	        FORCE[5] = -Mz;
	        FORCE[6] = -Fxw;
	        FORCE[7] = -Fyw;
	        FORCE[8] = -Fzw;
	        FORCE[9] = -Mxw;
	        FORCE[10] = -Myw;

	        /************************************************************
				        Update Force Output Vector
	        ************************************************************/
	        MISC[0] = Vpile;
	        MISC[1] = PILE[0];
	        MISC[2] = PILE[1];
	        MISC[3] = region;
	        MISC[4] = x04;
	        MISC[5] = y04;
	        MISC[6] = z04;
	        MISC[7] = intrench;
	        MISC[8] = n0;
	        MISC[9] = n1;
	        MISC[10] = n2;
	        MISC[11] = n3;
	        MISC[12] = (X_c-x04);
	        MISC[13] = (Y_c-y04);
	        MISC[14] = (Z_c-Z04);
	        MISC[15] = (Alpha_c-a_of_a);
	        MISC[16] = vtipx;
	        MISC[17] = vtipy;
	        MISC[18] = vtipz;
	        MISC[19] = vphi;

	        /************************************************************
				        Soil Outputs
	        ************************************************************/
	        for(k = 0; k < SoilModel._IntDiscreteSectionsMatlab; k++) 
	        {
		        //temp = ((SoilModel.GroundLevel-SOIL_Old[k])*255)/SoilModel.MaxDepth;
                temp = ((SoilModel.GroundLevel-SOIL[k])) + 50;
		        if ( temp < 0 ) 
			        temp = 0;
		        else if ( 255 < temp )
			        temp = 255;
		        SoilModel.Y_SOIL[k] = (byte) temp;
	        /************************************************************
				        Update States
	        ************************************************************/
		        STATES[k] = SOIL[k];
		        STATES[SoilModel._IntDiscreteSectionsMatlab + k] = SOIL_dump[k];
	        }

	        // store new center
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 0] = X_c;
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 1] = Z_c;
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 2] = Alpha_c;

	        // store tip position
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 3] = R04;
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 4] = Z04;  
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 5] = a1t;
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 6] = b0t;
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 7] = b1t;
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 8] = a1n;
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 9] = b0n;
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 10] = b1n;    
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 11] = Math.Min((float) SoilModel.Max_V_Load, Math.Max(Vbk, Vload));//Vload;
	        STATES[2*SoilModel._IntDiscreteSectionsMatlab + 12] = Vpile;
            STATES[2*SoilModel._IntDiscreteSectionsMatlab + 13] = intrench;									// intrench
    
            //pile states
            STATES[2*SoilModel._IntDiscreteSectionsMatlab + 14] = PILE[0];									// xpile
            STATES[2*SoilModel._IntDiscreteSectionsMatlab + 15] = PILE[1];									// ypile
            STATES[2*SoilModel._IntDiscreteSectionsMatlab + 16] = PILE[3];									// pile size
            STATES[2*SoilModel._IntDiscreteSectionsMatlab + 17] = Y_c;									// Y_c   
            STATES[2*SoilModel._IntDiscreteSectionsMatlab + 18] = PILE[2];									// Y_c       
    
	        /************************************************************
				        Soil Outputs
	        ************************************************************/
	        // LOAD[0] = (uint16_T) Vload;
	        LOAD[0] = (UInt16) Math.Min(SoilModel.Max_V_Load, Math.Max(Vbk, Vload));
            LOAD[1] = (UInt16)Vpile; 
	        /************************************************************
				        The end!!
	        ************************************************************/
        }




















































        public static void drawGL(bool uselighting)
        {
            if (SoilModel._BoolSetupGL4) SoilModel.updateGL4();
            else SoilModel.setupGL4();

            if (SoilModel._BoolSetupGL4) SoilModel.drawGL4();

            //                display_piles(xpilearray, ypilearray, vpilearray, pilecount, zpile, falltime);// draw piles

            //SAM            display_box(boxx, boxy, boxheight, soilheightL, soilheightR);

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
                        GL.TexCoord2(TrialMarkElton.XTextL, Trial._IntTextureDensity);
                        GL.Vertex3(XhalfL, YGround, Trial._FloatGroundPlaneDim);
                        GL.TexCoord2(TrialMarkElton.XTextL, 0);
                        GL.Vertex3(XhalfL, YGround, -Trial._FloatGroundPlaneDim);
                        GL.TexCoord2(0, 0);
                        GL.Vertex3(-Trial._FloatGroundPlaneDim, YGround, -Trial._FloatGroundPlaneDim);
                        GL.TexCoord2(0, Trial._IntTextureDensity);
                        GL.Vertex3(-Trial._FloatGroundPlaneDim, YGround, Trial._FloatGroundPlaneDim);

                        GL.TexCoord2(Trial._IntTextureDensity, Trial._IntTextureDensity);
                        GL.Vertex3(Trial._FloatGroundPlaneDim, YGround, Trial._FloatGroundPlaneDim);
                        GL.TexCoord2(Trial._IntTextureDensity, 0);
                        GL.Vertex3(Trial._FloatGroundPlaneDim, YGround, -Trial._FloatGroundPlaneDim);
                        GL.TexCoord2(TrialMarkElton.XTextR, 0);
                        GL.Vertex3(XhalfR, YGround, -Trial._FloatGroundPlaneDim);
                        GL.TexCoord2(TrialMarkElton.XTextR, Trial._IntTextureDensity);
                        GL.Vertex3(XhalfR, YGround, Trial._FloatGroundPlaneDim);

                        GL.TexCoord2(TrialMarkElton.XTextL, TrialMarkElton.ZTextBehind);
                        GL.Vertex3(XhalfL, YGround, -ZStartTrench);
                        GL.TexCoord2(TrialMarkElton.XTextL, Trial._IntTextureDensity);
                        GL.Vertex3(XhalfL, YGround, Trial._FloatGroundPlaneDim);
                        GL.TexCoord2(TrialMarkElton.XTextR, Trial._IntTextureDensity);
                        GL.Vertex3(XhalfR, YGround, Trial._FloatGroundPlaneDim);
                        GL.TexCoord2(TrialMarkElton.XTextR, TrialMarkElton.ZTextBehind);
                        GL.Vertex3(XhalfR, YGround, -ZStartTrench);

                        GL.TexCoord2(TrialMarkElton.XTextL, 0);
                        GL.Vertex3(XhalfL, YGround, -Trial._FloatGroundPlaneDim);
                        GL.TexCoord2(TrialMarkElton.XTextL, TrialMarkElton.ZTextFront);
                        GL.Vertex3(XhalfL, YGround, -ZEndTrench);
                        GL.TexCoord2(TrialMarkElton.XTextR, TrialMarkElton.ZTextFront);
                        GL.Vertex3(XhalfR, YGround, -ZEndTrench);
                        GL.TexCoord2(TrialMarkElton.XTextR, 0);
                        GL.Vertex3(XhalfR, YGround, -Trial._FloatGroundPlaneDim);
                    }
                    GL.End();

                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GL.ActiveTexture(TextureUnit.Texture7);
                }
            }
            else
            {
                GL.Begin(BeginMode.Quads);
                {
                    GL.Vertex3(XhalfL, YGround, Trial._FloatGroundPlaneDim);
                    GL.Vertex3(XhalfL, YGround, -Trial._FloatGroundPlaneDim);
                    GL.Vertex3(-Trial._FloatGroundPlaneDim, YGround, -Trial._FloatGroundPlaneDim);
                    GL.Vertex3(-Trial._FloatGroundPlaneDim, YGround, Trial._FloatGroundPlaneDim);

                    GL.Vertex3(Trial._FloatGroundPlaneDim, YGround, Trial._FloatGroundPlaneDim);
                    GL.Vertex3(Trial._FloatGroundPlaneDim, YGround, -Trial._FloatGroundPlaneDim);
                    GL.Vertex3(XhalfR, YGround, -Trial._FloatGroundPlaneDim);
                    GL.Vertex3(XhalfR, YGround, Trial._FloatGroundPlaneDim);

                    GL.Vertex3(XhalfL, YGround, -ZStartTrench);
                    GL.Vertex3(XhalfL, YGround, Trial._FloatGroundPlaneDim);
                    GL.Vertex3(XhalfR, YGround, Trial._FloatGroundPlaneDim);
                    GL.Vertex3(XhalfR, YGround, -ZStartTrench);
                }
                GL.End();
            }
        }

        private static void drawGL4()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, SoilModel._IntInterleaveBufferID);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes * 2, IntPtr.Zero);
            GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes * 2, Vector3.SizeInBytes);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);
            GL.DrawElements(BeginMode.Triangles, _IntElementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        private static bool _BoolSetupGL4 = false;
        private static int _IntInterleaveBufferID;
        private static int _IntIndicesBufferID;
        private static int _IntElementCount = 0;

        public static void GLDelete()
        {
            if (SoilModel._BoolSetupGL4)
            {
                SoilModel._BoolSetupGL4 = false;
                GL.DeleteBuffers(1, ref SoilModel._IntIndicesBufferID);
                GL.DeleteBuffers(1, ref SoilModel._IntInterleaveBufferID);
            }
        }

        private static void setupGL4()
        {
            idata[(0) * 2] = new Vector3(XhalfL, YGround, -(ZStartTrench));
            idata[(1) * 2] = new Vector3(XhalfR, YGround, -(ZStartTrench));
            idata[(0) * 2 + 1] = Vector3.UnitY;
            idata[(1) * 2 + 1] = Vector3.UnitY;

            float Ym = 0;
            float Y0 = YGround;
            float Yp = YGround - SoilModel.Y_SOIL[0] + 50;
            float Z0;

            for (int k = 0; k < _IntDiscreteSectionsMatlab; k++)
            {
                Ym = Y0;
                Y0 = Yp;
                Yp = YGround - ((k == _IntDiscreteSectionsMatlab - 1) ? 0 : SoilModel.Y_SOIL[k + 1] - 50);

                Z0 = -(ZStartTrench + (k + 1) * dX);

                Vector3 norm = new Vector3(0, 2 * dX, Ym - Yp);
                norm.NormalizeFast();

                idata[(2 * k + 2) * 2] = new Vector3(XhalfL, Y0, Z0);
                idata[(2 * k + 3) * 2] = new Vector3(XhalfR, Y0, Z0);
                idata[(2 * k + 2) * 2 + 1] = norm;
                idata[(2 * k + 3) * 2 + 1] = norm;
            }

            idata[(_IntTopDrawPoints - 2) * 2] = new Vector3(XhalfL, YGround, -(ZEndTrench));
            idata[(_IntTopDrawPoints - 1) * 2] = new Vector3(XhalfR, YGround, -(ZEndTrench));
            idata[(_IntTopDrawPoints - 2) * 2 + 1] = Vector3.UnitY;
            idata[(_IntTopDrawPoints - 1) * 2 + 1] = Vector3.UnitY;

            int[] triangs = new int[(_IntDiscreteSectionsRender - 1) * 6];

            for (int i = 0; i < _IntDiscreteSectionsRender - 1; i++)
            {
                int i6 = i * 6;
                int i2 = i * 2;

                triangs[i6 + 0] = i2;
                triangs[i6 + 1] = i2 + 1;
                triangs[i6 + 2] = i2 + 2;
                triangs[i6 + 3] = i2 + 2;
                triangs[i6 + 4] = i2 + 1;
                triangs[i6 + 5] = i2 + 3;
            }

            int bufferSize;

            bufferSize = idata.Length * Vector3.SizeInBytes;
            GL.GenBuffers(1, out SoilModel._IntInterleaveBufferID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, SoilModel._IntInterleaveBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferSize), idata, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            bufferSize = triangs.Length * sizeof(uint);
            GL.GenBuffers(1, out SoilModel._IntIndicesBufferID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, SoilModel._IntIndicesBufferID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSize), triangs, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            SoilModel._IntElementCount = triangs.Length;

            SoilModel._BoolSetupGL4 = true;
        }

        private static void updateGL4()
        {

            idata[(0) * 2] = new Vector3(XhalfL, YGround, -(ZStartTrench));
            idata[(1) * 2] = new Vector3(XhalfR, YGround, -(ZStartTrench));
            idata[(0) * 2 + 1] = Vector3.UnitY;
            idata[(1) * 2 + 1] = Vector3.UnitY;

            float Ym = 0;
            float Y0 = YGround;
            float Yp = YGround - SoilModel.Y_SOIL[0] + 50;
            float Z0;

            for (int k = 0; k < _IntDiscreteSectionsMatlab; k++)
            {
                Ym = Y0;
                Y0 = Yp;
                Yp = YGround - ((k == _IntDiscreteSectionsMatlab - 1) ? 0 : SoilModel.Y_SOIL[k + 1] - 50);

                Z0 = -(ZStartTrench + (k + 1) * dX);

                Vector3 norm = new Vector3(0, 2 * dX, Ym - Yp);
                norm.NormalizeFast();

                idata[(2 * k + 2) * 2].Y = Y0;
                idata[(2 * k + 3) * 2].Y = Y0;
                idata[(2 * k + 2) * 2 + 1] = norm;
                idata[(2 * k + 3) * 2 + 1] = norm;
            }

            idata[(_IntTopDrawPoints - 2) * 2] = new Vector3(XhalfL, YGround, -(ZEndTrench));
            idata[(_IntTopDrawPoints - 1) * 2] = new Vector3(XhalfR, YGround, -(ZEndTrench));
            idata[(_IntTopDrawPoints - 2) * 2 + 1] = Vector3.UnitY;
            idata[(_IntTopDrawPoints - 1) * 2 + 1] = Vector3.UnitY;

            int bufferSize = idata.Length * Vector3.SizeInBytes;
            GL.BindBuffer(BufferTarget.ArrayBuffer, SoilModel._IntInterleaveBufferID);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(bufferSize), idata);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }





    }
}
