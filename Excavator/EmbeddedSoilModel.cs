using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SamSeifert.GLE;
using SamSeifert.GLE.CadViewer;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Excavator
{
    public class EmbeddedSoilModel
    {
        const int _IntDiscreteSectionsMatlab = 256;			// number of nodes
        const float GroundLevel = - (Bobcat._SamCatLiftHeight + Bobcat._V3RotationOffsetY + 3);	// inches
        const float	MaxDepth = 150;			// inches
        const int TRENCH_WIDTH = 24;       // inches
        const float TrenchStart = 0;        // inches
        const float TrenchEnd = 250;        // - 31.5423) // inches	
        const int NumStatesK_c = 3;
        const int NumStatesFilter = 8;
        const int NumStatesLoad = 2;
        const float Max_V_Load = 7200;

        const int sidelen = 60;				//length of the sides of the box

        const float YGround = 0;
        const float YTrenchBottom = YGround - MaxDepth; // inches (defined in SoilParam)
        const float XhalfL = -TRENCH_WIDTH / 2f;
        const float XhalfR = TRENCH_WIDTH / 2f;
        const float ZStartTrench = TrenchStart;				// inches
        const float ZEndTrench = TrenchEnd;					// inches
        const float dX = (ZEndTrench - ZStartTrench) / 257;			// X resolution

        const int _IntDiscreteSectionsRender = _IntDiscreteSectionsMatlab + 2;
        const int _IntTopDrawPoints = _IntDiscreteSectionsRender * 2;

        public const float XTextL = Trial._IntTextureDensity * (0.5f + (XhalfL / (2 * Trial._FloatGroundPlaneDim)));
        public const float XTextR = Trial._IntTextureDensity * (0.5f - (XhalfL / (2 * Trial._FloatGroundPlaneDim)));
        public const float ZTextBehind = Trial._IntTextureDensity * (0.5f - (ZStartTrench / (2 * Trial._FloatGroundPlaneDim)));
        public const float ZTextFront = Trial._IntTextureDensity * (0.5f - (ZEndTrench / (2 * Trial._FloatGroundPlaneDim)));

        private Byte[] Y_SOIL = new Byte[_IntDiscreteSectionsMatlab]; // Set at end of matlab thread
        private float[] SOIL = new float[EmbeddedSoilModel._IntDiscreteSectionsMatlab];
        private float[] SOIL_dump = new float[EmbeddedSoilModel._IntDiscreteSectionsMatlab];
        private float X_c, Y_c, Z_c, Alpha_c;
        private float Vload, Vpile, intrench;
        private float[] PILE = new float[4];   
        const int _IntNoRead = 777;

        // Accessed by Draw Thread Only
        private Vector3[] _V3SoilPoints = new Vector3[EmbeddedSoilModel._IntTopDrawPoints * 2 + EmbeddedSoilModel._IntDiscreteSectionsRender];

        static readonly System.Drawing.Color _ColorDirt = System.Drawing.Color.FromArgb(100, 80, 0);




        public EmbeddedSoilModel()
        {
            for (int k = 0; k < EmbeddedSoilModel._IntDiscreteSectionsMatlab; k++)
            {
                this.SOIL[k] = EmbeddedSoilModel.GroundLevel;					// Soil States
                this.SOIL_dump[k] = EmbeddedSoilModel.GroundLevel;		// Dump Soil States
            }

            this.X_c = EmbeddedSoilModel._IntNoRead;
            this.Z_c = EmbeddedSoilModel._IntNoRead;
            this.Alpha_c = EmbeddedSoilModel._IntNoRead;

            // STATES[2*SoilModel._IntDiscreteSectionsMatlab + 3] = R04;
            // STATES[2*SoilModel._IntDiscreteSectionsMatlab + 4] = Z04;  

            // Volume of soil in the bucket
            this.Vload = 0;
            this.Vpile = 0;
            this.intrench = 0;

            this.PILE[0] = 0;								// xpile
            this.PILE[1] = 0;								// ypile
            this.PILE[2] = EmbeddedSoilModel.GroundLevel - 50;		// pile size    
            this.PILE[3] = 0;								// pile size

            this.Y_c = EmbeddedSoilModel._IntNoRead;									// Y_c  

            this._VolumeLeftBin = 0;
            this._VolumeRightBin = 0;
            this._VolumeNoBin = 0;
        }

        public unsafe TrialSaver.File2_DataType ForceModel_ReturnDump(
            ref float[] POS,
            ref float[] VEL,
            ref float[] Q,
            ref float[] QD, // Input
            ref float[] SD_FORCE,
            ref float[] SD_MOMENT,
            ref float[] SW_FORCE,
            out float OutVLoad,
            float simtime
            )
        {
            // Inputs
            
	        // States
	        // int numStates = ssGetNumDiscStates(S);

	        // Outputs
	        UInt16[] LOAD = new UInt16[2];

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
	        float delta_R = (EmbeddedSoilModel.TrenchEnd - EmbeddedSoilModel.TrenchStart)/( (float) (EmbeddedSoilModel._IntDiscreteSectionsMatlab + 1));
	        float R34, Z34, R03, Z03, R04, Z04 = 0.0f, T04, N04, Delta = 0.42479226265574266f, cA, sA;
	        float Sp, Sbucket = 12;
	        float Rs, Zs, ms, bs, dZ_last, Zsoil, r1, r2, z1, z2, r1wrist, r2wrist, z1wrist, z2wrist;
	        int n0 = 0, n1, n2, n3 = 0, np, n1wrist;


	        // stiffness center and shear variables
	        float R_c, N_c, T_c, Theta_c, a_of_a;
	        float Vbk = 0, Stall_Ratio;

	        // stiffness variables
	        float Fkn=0, Fkt=0, Tka=0, Fkr=0, Fkz=0, Tkphi=0; 

	        // shear and soil update variables
	        float dp = 1, delta_r, delta_z, Spill_Ratio;

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
            float x04, y04, z04, x34, y34, repeet, diff;
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
				        Unpack Input Variables
	        ************************************************************/

            this.PILE[0] = 0;
            this.PILE[1] = 0;
            this.PILE[2] = 0;
            this.PILE[3] = 0;

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
    
            r1 = R04 - EmbeddedSoilModel.TrenchStart;
            r2 = R04 + Sbucket*cA - EmbeddedSoilModel.TrenchStart;
            n1 = (int)Math.Floor(r1/delta_R);// - 1;
            n2 = (int)Math.Ceiling(r2/delta_R);// - 1;

            r1wrist = R03 - EmbeddedSoilModel.TrenchStart;
            n1wrist = (int)Math.Floor(r1wrist/delta_R);// - 1;    
    
           /*//////////////////////////////////////////////////////*
                            determine region
           /*//////////////////////////////////////////////////////*    
            region = Math.Abs(y04 * 2) > EmbeddedSoilModel.TRENCH_WIDTH ? 3 : 1;
            region *= z04 < EmbeddedSoilModel.GroundLevel ? -1 : 1;
    
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
                Zs = ms*(EmbeddedSoilModel.TrenchStart + (k)*delta_R) + bs;
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
            r1 = EmbeddedSoilModel.TrenchStart + (n1)*delta_R;
            r2 = r1 + delta_R;

            r1wrist = EmbeddedSoilModel.TrenchStart + (n1wrist)*delta_R;
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
                z1 = EmbeddedSoilModel.GroundLevel;//SOIL_Old[n1];
                z2 = EmbeddedSoilModel.GroundLevel;//SOIL_Old[n2];
                Zsoil = EmbeddedSoilModel.GroundLevel;//z1 + (R04 - r1)*(z2 - z1)/(r2 - r1);
                z1wrist = EmbeddedSoilModel.GroundLevel;
            }

            if ( Z_c == EmbeddedSoilModel._IntNoRead ) // if Z_c = 777 then there was no contact point in z^{-1}
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
                X_c = EmbeddedSoilModel._IntNoRead;
                Y_c = EmbeddedSoilModel._IntNoRead;
                Z_c = EmbeddedSoilModel._IntNoRead;
                Alpha_c = EmbeddedSoilModel._IntNoRead;     
            }
            else if ((intrench == 1) || (region == -1))//(( Tau_rt >= 0 ) && ( Tau_rn >= 0 )) // if either is negative, then no relaxation // bucket tip continues through soil
            {
                    // adjust update based on amount of soil in bucket
                    Vbk = 0;
                    for ( k = 0; k < EmbeddedSoilModel._IntDiscreteSectionsMatlab; k++ )
                    {
                        // trapezoidal integration
                        Vbk = Vbk + EmbeddedSoilModel.TRENCH_WIDTH * delta_R * (SOIL_dump[k] - SOIL[k]);
                    }
                    Vload = Vbk;
            
                    if ( Vload <= ( EmbeddedSoilModel.Max_V_Load ) )  // all the dirt stays in the bucket
                    {
                    }
                    else	// some dirt spills from bucket due to over filling
                    {
                       // add extra dirt evenly across trench compared to where is it was when last dumped
                        Spill_Ratio = (( Vload) - ( EmbeddedSoilModel.Max_V_Load))/( Vload);
                        for ( k = 0; k < EmbeddedSoilModel._IntDiscreteSectionsMatlab; k++ )
                        {
                            SOIL[k] = SOIL[k] + (SOIL_dump[k] - SOIL[k])*Spill_Ratio;
                        }
                        Vload = ( EmbeddedSoilModel.Max_V_Load );
                    }   // dirt spills
                    Vbk = Vload;

                /************************************************************
                            Calculate Shear Plane - Update soil
                ************************************************************/
                    //n0 = (int_T) floor((R04 + (Z04 - SoilModel.GroundLevel)*sA/cA - SoilModel.TrenchStart)/delta_R);// - 1;
                    n0 = (int)Math.Floor((R04 + (Z04 - EmbeddedSoilModel.GroundLevel)*cA/sA - EmbeddedSoilModel.TrenchStart)/delta_R);// - 1;
                    n1 = (int)Math.Floor((R04 - EmbeddedSoilModel.TrenchStart) / delta_R);// - 1;
                    n2 = (int)Math.Ceiling((R04 + Sbucket * cA - EmbeddedSoilModel.TrenchStart) / delta_R);//- 1;
                    if ( n2 >= EmbeddedSoilModel._IntDiscreteSectionsMatlab )
                        n2 = EmbeddedSoilModel._IntDiscreteSectionsMatlab-1;

                    if ((region == -1) || (region == -2))
                    {
                        //update soil "cutting path"
                        Zs = ms*(EmbeddedSoilModel.TrenchStart + (n1+1)*delta_R) + bs;
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
                            delta_r = EmbeddedSoilModel.TrenchStart + (k+1)*delta_R - R04;
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

                    delta_r = EmbeddedSoilModel.TrenchStart + (n0+1)*delta_R - R04;
                    delta_z = SOIL[n0] - Z04;
                    dp = (float)Math.Sqrt(delta_r*delta_r + delta_z*delta_z);

                    if (( EmbeddedSoilModel.TRENCH_WIDTH*dp*ShearStrength < -Fkn ) && (intrench == 1)) //Soil never shears outside trench
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
            if ( Z_c != EmbeddedSoilModel._IntNoRead )   // bucket is in the soil
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
                if (signvtipn == signnc)
                {
                    kn = kn/10;
                    bn = bn/100;
                }
                if (signvtiptheta == signthetac)
                {
                    ktheta = ktheta/10;
                    btheta = btheta/100;
                }
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
            }

            /************************************************************
                        Force Sensor calculations
            ************************************************************/
            if ( Z_c == EmbeddedSoilModel._IntNoRead ) 
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
                    if (PILE[3] == -1)    // 2222 is code for dumping in trench - see piles.h in graphics simulator
                    {
                        PILE[3] = -2;
                    }
                    PILE[2] = Z04;
                    PILE[1] = y04 - (0.5f * Bobcat.a4_sim) * s1;    //put pile in front of bucket tip
                    PILE[0] = x04 - (0.5f * Bobcat.a4_sim) * c1;    //put pile in front of bucket tip
                    Vload = 0;
                    Vbk = 0;
                    for ( k = 0; k < EmbeddedSoilModel._IntDiscreteSectionsMatlab; k++ )
                    {
                        SOIL_dump[k] = SOIL[k];
                    }            
                }
                if ((intrench == 0) && (region == 1))       // dump in trench
                {   
                    soilheight = Vload/(EmbeddedSoilModel.TRENCH_WIDTH*delta_R);
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
                    for ( k = 0; k < EmbeddedSoilModel._IntDiscreteSectionsMatlab; k++ )
                    {
                        SOIL_dump[k] = SOIL[k];
                    }
                    PILE[3] = -1;
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

            SD_FORCE[0] = -Fx;
            SD_FORCE[1] = -Fy;
            SD_FORCE[2] = -Fz;

            SD_MOMENT[0] = -Mx;
            SD_MOMENT[1] = -My;
            SD_MOMENT[2] = -Mz;

            SW_FORCE[0] = -Fxw;
            SW_FORCE[1] = -Fyw;
            SW_FORCE[2] = -Fzw;

            //FORCE[9] = -Mxw;
            //FORCE[10] = -Myw;


	        /************************************************************
				        Update Force Output Vector
	        ************************************************************/
/*	        MISC[0] = Vpile;
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
	        MISC[19] = vphi;*/

	        /************************************************************
				        Soil Outputs
	        ************************************************************/
	        for(k = 0; k < EmbeddedSoilModel._IntDiscreteSectionsMatlab; k++)
                this.Y_SOIL[k] = (byte)Math.Max(0, Math.Min(255, EmbeddedSoilModel.GroundLevel - SOIL[k] + 50));
            /************************************************************
				        Update States
	        ************************************************************/
            Vload = Math.Min((float)EmbeddedSoilModel.Max_V_Load, Math.Max(Vbk, Vload));//Vload; 
            OutVLoad = Vload;
	        /************************************************************
				        Soil Outputs
	        ************************************************************/
	        // LOAD[0] = (uint16_T) Vload;
	        LOAD[0] = (UInt16) Math.Min(EmbeddedSoilModel.Max_V_Load, Math.Max(Vbk, Vload));
            LOAD[1] = (UInt16)Vpile; 
	        /************************************************************
				        The end!!
	        ************************************************************/

            TrialSaver.File2_DataType pl = new TrialSaver.File2_DataType();

            if (this.PILE[3] > 0)
            {
                pl.TimeMilis = (int)(1000 * simtime);
                pl.dX = this.PILE[0];
                pl.dY = this.PILE[1];
                pl.dZ = this.PILE[2];
                pl.Size = this.PILE[3];

                pl.inBin = this.NewPileDataComing(
                    this.PILE[0],
                    this.PILE[1],
                    this.PILE[2],
                    this.PILE[3],
                    simtime);
            }
            else pl.Size = -1;

            return pl;
        }






















































        public void drawTrench(bool ShadowBufferDraw)
        {
            if (this._BoolSetupGL4)
            {
                this.updateGL4();
                EmbeddedSoilModel.setColorDirt();
                this.drawGL4(ShadowBufferDraw);
            }
            else this.setupGL4();

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

        private void drawGL4(bool ShadowBufferDraw)
        {
            UInt32 uip = GL.CurrentProgram;

            if (!ShadowBufferDraw)
            {
                bool a =
                    EmbeddedSoilModel._ShadowIntProgram != 0 &&
                    EmbeddedSoilModel._ShadowIntShaderV != 0 &&
                    EmbeddedSoilModel._ShadowIntShaderF != 0;

                if (!a)
                {
                    int sp, sv, sf;

                    a = Shaders.CreateShaders(
                        Properties.Resources.SoilVertex,
                        Properties.Resources.SoilFragment,
                        out sp,
                        out sv,
                        out sf);

                    if (a)
                    {
                        EmbeddedSoilModel._ShadowIntProgram = sp;
                        EmbeddedSoilModel._ShadowIntShaderV = sv;
                        EmbeddedSoilModel._ShadowIntShaderF = sf;

                        GL.UseProgram(EmbeddedSoilModel._ShadowIntProgram);
                        Textures.BindTexture(EmbeddedSoilModel._ShadowIntProgram, TextureUnit.Texture0, "tex0");
                        Textures.BindTexture(EmbeddedSoilModel._ShadowIntProgram, TextureUnit.Texture7, "ShadowMap");
                        GL.UseProgram(0);
                    }
                }

                if (a)
                {
                    if (EmbeddedSoilModel._IntTextureDirt == 0)
                        EmbeddedSoilModel._IntTextureDirt = Textures.getGLTexture(Properties.Resources.dirt4);

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, EmbeddedSoilModel._IntTextureDirt);
                    GL.UseProgram(EmbeddedSoilModel._ShadowIntProgram);
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, this._IntInterleaveBufferID);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes * 1, Vector3.SizeInBytes * EmbeddedSoilModel._IntTopDrawPoints);
            GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes * 1, IntPtr.Zero);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._IntIndicesBufferID);
            GL.DrawElements(BeginMode.Triangles, this._IntElementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

            if (!ShadowBufferDraw)
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.ActiveTexture(TextureUnit.Texture7);
                GL.UseProgram(uip);
            }

            // Draw Sides
            GL.Begin(BeginMode.Quads);
            {
                GL.Normal3(-1, 0, 0);
                GL.Vertex3(EmbeddedSoilModel.XhalfR, EmbeddedSoilModel.YGround, -EmbeddedSoilModel.ZEndTrench);
                GL.Vertex3(EmbeddedSoilModel.XhalfR, EmbeddedSoilModel.YTrenchBottom, -EmbeddedSoilModel.ZEndTrench);
                GL.Vertex3(EmbeddedSoilModel.XhalfR, EmbeddedSoilModel.YTrenchBottom, -EmbeddedSoilModel.ZStartTrench);
                GL.Vertex3(EmbeddedSoilModel.XhalfR, EmbeddedSoilModel.YGround, -EmbeddedSoilModel.ZStartTrench);
                GL.Normal3(1, 0, 0);
                GL.Vertex3(EmbeddedSoilModel.XhalfL, EmbeddedSoilModel.YGround, -EmbeddedSoilModel.ZStartTrench);
                GL.Vertex3(EmbeddedSoilModel.XhalfL, EmbeddedSoilModel.YTrenchBottom, -EmbeddedSoilModel.ZStartTrench);
                GL.Vertex3(EmbeddedSoilModel.XhalfL, EmbeddedSoilModel.YTrenchBottom, -EmbeddedSoilModel.ZEndTrench);
                GL.Vertex3(EmbeddedSoilModel.XhalfL, EmbeddedSoilModel.YGround, -EmbeddedSoilModel.ZEndTrench);
            }
            GL.End();
        }

        internal void drawTrenchOrtho()
        {
            GL.Color3(EmbeddedSoilModel._ColorDirt);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._IntInterleaveBufferID);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes * 1, Vector3.SizeInBytes * EmbeddedSoilModel._IntTopDrawPoints);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._IntIndicesBufferOrthoID);
            GL.DrawElements(BeginMode.Triangles, this._IntElementCountOrtho, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        private bool _BoolSetupGL4 = false;
        
        private int _IntInterleaveBufferID;

        private int _IntIndicesBufferID = 0;
        private int _IntElementCount = 0;

        private int _IntIndicesBufferOrthoID = 0;
        private int _IntElementCountOrtho = 0;

        public static int _ShadowIntProgram { get; private set; }
        public static int _ShadowIntShaderV { get; private set; }
        public static int _ShadowIntShaderF { get; private set; }
        private static int _IntTextureDirt = 0;

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
        }

        public void GLDelete()
        {
            if (this._BoolSetupGL4)
            {
                this._BoolSetupGL4 = false;
                GL.DeleteBuffers(1, ref this._IntIndicesBufferID);
                GL.DeleteBuffers(1, ref this._IntIndicesBufferOrthoID);

                GL.DeleteBuffers(1, ref this._IntInterleaveBufferID);
            }
        }

        private void setupGL4()
        {

            this._V3SoilPoints[EmbeddedSoilModel._IntTopDrawPoints + 0] = new Vector3(
                EmbeddedSoilModel.XhalfL,
                EmbeddedSoilModel.YGround, 
                -EmbeddedSoilModel.ZStartTrench);

            this._V3SoilPoints[EmbeddedSoilModel._IntTopDrawPoints + 1] = new Vector3(
                EmbeddedSoilModel.XhalfR, 
                EmbeddedSoilModel.YGround,
                -EmbeddedSoilModel.ZStartTrench);

            this._V3SoilPoints[0] = Vector3.UnitY;
            this._V3SoilPoints[1] = Vector3.UnitY;

            float Ym = 0;
            float Y0 = EmbeddedSoilModel.YGround;
            float Yp = EmbeddedSoilModel.YGround - this.Y_SOIL[0] + 50;
            float Z0;

            for (int k = 0; k < EmbeddedSoilModel._IntDiscreteSectionsMatlab; k++)
            {
                Ym = Y0;
                Y0 = Yp;
                Yp = YGround - ((k == EmbeddedSoilModel._IntDiscreteSectionsMatlab - 1) ? 0 : this.Y_SOIL[k + 1] - 50);

                Z0 = -(ZStartTrench + (k + 1) * dX);

                Vector3 norm = new Vector3(0, 2 * dX, Ym - Yp);
                norm.NormalizeFast();

                this._V3SoilPoints[EmbeddedSoilModel._IntTopDrawPoints + (2 * k + 2)] = new Vector3(
                    EmbeddedSoilModel.XhalfL, 
                    Y0, 
                    Z0);

                this._V3SoilPoints[EmbeddedSoilModel._IntTopDrawPoints + (2 * k + 3)] = new Vector3(
                    EmbeddedSoilModel.XhalfR, 
                    Y0,
                    Z0);

                this._V3SoilPoints[(2 * k + 2)] = norm;
                this._V3SoilPoints[(2 * k + 3)] = norm;
            }

            this._V3SoilPoints[EmbeddedSoilModel._IntTopDrawPoints * 2 - 2] = new Vector3(
                EmbeddedSoilModel.XhalfL,
                EmbeddedSoilModel.YGround, 
                -EmbeddedSoilModel.ZEndTrench);

            this._V3SoilPoints[EmbeddedSoilModel._IntTopDrawPoints * 2 - 1] = new Vector3(
                EmbeddedSoilModel.XhalfR, 
                EmbeddedSoilModel.YGround,
                -EmbeddedSoilModel.ZEndTrench);

            this._V3SoilPoints[EmbeddedSoilModel._IntTopDrawPoints - 2] = Vector3.UnitY;
            this._V3SoilPoints[EmbeddedSoilModel._IntTopDrawPoints - 1] = Vector3.UnitY;

            for (int k = 0; k < EmbeddedSoilModel._IntDiscreteSectionsRender; k++)
            {
                this._V3SoilPoints[EmbeddedSoilModel._IntTopDrawPoints * 2 + k] = new Vector3(
                    XhalfL,
                    EmbeddedSoilModel.YTrenchBottom / 2,
                    -(ZStartTrench + k * dX));
            }


            int bufferSize = this._V3SoilPoints.Length * Vector3.SizeInBytes;
            GL.GenBuffers(1, out this._IntInterleaveBufferID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._IntInterleaveBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferSize), this._V3SoilPoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            
            int[] triangs = new int[(EmbeddedSoilModel._IntDiscreteSectionsRender - 1) * 6];
            
            for (int i = 0; i < EmbeddedSoilModel._IntDiscreteSectionsRender - 1; i++)
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

            bufferSize = triangs.Length * sizeof(uint);
            GL.GenBuffers(1, out this._IntIndicesBufferID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._IntIndicesBufferID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSize), triangs, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            this._IntElementCount = triangs.Length;

            for (int i = 0; i < EmbeddedSoilModel._IntDiscreteSectionsRender - 1; i++)
            {
                int i6 = i * 6;
                int t1 = i * 2;
                int b1 = EmbeddedSoilModel._IntTopDrawPoints + i;

                triangs[i6 + 0] = t1;
                triangs[i6 + 1] = b1;
                triangs[i6 + 2] = b1 + 1;
                triangs[i6 + 3] = b1 + 1;
                triangs[i6 + 4] = t1 + 2;
                triangs[i6 + 5] = t1;
            }

            bufferSize = triangs.Length * sizeof(uint);
            GL.GenBuffers(1, out this._IntIndicesBufferOrthoID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._IntIndicesBufferOrthoID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSize), triangs, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            this._IntElementCountOrtho = triangs.Length;


            this._BoolSetupGL4 = true;
        }

        private void updateGL4()
        {
            float Ym = 0;
            float Y0 = YGround;
            float Yp = YGround - this.Y_SOIL[0] + 50;
            float Z0;

            for (int k = 0; k < _IntDiscreteSectionsMatlab; k++)
            {
                Ym = Y0;
                Y0 = Yp;
                Yp = YGround - ((k == _IntDiscreteSectionsMatlab - 1) ? 0 : this.Y_SOIL[k + 1] - 50);

                Z0 = -(ZStartTrench + (k + 1) * dX);

                Vector3 norm = new Vector3(0, 2 * dX, Ym - Yp);
                norm.NormalizeFast();

                this._V3SoilPoints[EmbeddedSoilModel._IntTopDrawPoints + (2 * k + 2)].Y = Y0;
                this._V3SoilPoints[EmbeddedSoilModel._IntTopDrawPoints + (2 * k + 3)].Y = Y0;
                this._V3SoilPoints[(2 * k + 2)] = norm;
                this._V3SoilPoints[(2 * k + 3)] = norm;
            }

            int bufferSize = this._V3SoilPoints.Length * Vector3.SizeInBytes;
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._IntInterleaveBufferID);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(bufferSize), this._V3SoilPoints);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }






















































































        const float _FloatBoxZ = 100, _FloatBoxX = 70, boxheight = 18;
        private volatile float soilheightL = 1;
        private volatile float soilheightR = 1;

        public void drawBins(bool ShadowBufferDraw)
        {
            const float sc = 39.37f;

            if (EmbeddedSoilModel._CadObjectBoxL == null)
            {
                EmbeddedSoilModel._CadObjectBoxL = CadObjectGenerator.fromXAML(
                    Properties.Resources.Box,
                    ObjectName: "Box Left",
                    xScale: sc,
                    yScale: sc,
                    zScale: sc,
                    xOff: -EmbeddedSoilModel._FloatBoxX,
                    yOff: 0.0f,
                    zOff: -EmbeddedSoilModel._FloatBoxZ);

                EmbeddedSoilModel._CadObjectBoxL.setColor(System.Drawing.Color.White, 0.15f, 0.5f, 0f, 0f);
                FormBase._FormBase.addCadItem(EmbeddedSoilModel._CadObjectBoxL);            
            }
            else EmbeddedSoilModel._CadObjectBoxL.draw(!ShadowBufferDraw);

            if (EmbeddedSoilModel._CadObjectBoxR == null)
            {
                EmbeddedSoilModel._CadObjectBoxR = CadObjectGenerator.fromXAML(
                    Properties.Resources.Box,
                    ObjectName: "Box Right",
                    xScale: sc,
                    yScale: sc,
                    zScale: sc,
                    xOff: EmbeddedSoilModel._FloatBoxX,
                    yOff: 0.0f,
                    zOff: -EmbeddedSoilModel._FloatBoxZ);

                EmbeddedSoilModel._CadObjectBoxR.setColor(System.Drawing.Color.White, 0.15f, 0.5f, 0f, 0f);
                FormBase._FormBase.addCadItem(EmbeddedSoilModel._CadObjectBoxR);
            }
            else EmbeddedSoilModel._CadObjectBoxR.draw(!ShadowBufferDraw);

            const float sidelen2 = sidelen / 2f;
            const float z = EmbeddedSoilModel._FloatBoxZ;
            float x = EmbeddedSoilModel._FloatBoxX;

            EmbeddedSoilModel.setColorDirt();

            GL.Normal3(0, 1, 0);

            float h;

            GL.Begin(BeginMode.Quads);
            {
                h = this.soilheightL;
                GL.Vertex3(-(x - sidelen2), h, -(z + sidelen2));
                GL.Vertex3(-(x + sidelen2), h, -(z + sidelen2));
                GL.Vertex3(-(x + sidelen2), h, -(z - sidelen2));
                GL.Vertex3(-(x - sidelen2), h, -(z - sidelen2));

                x = -x;
                h = this.soilheightR;
                GL.Vertex3(-(x - sidelen2), h, -(z + sidelen2));
                GL.Vertex3(-(x + sidelen2), h, -(z + sidelen2));
                GL.Vertex3(-(x + sidelen2), h, -(z - sidelen2));
                GL.Vertex3(-(x - sidelen2), h, -(z - sidelen2));
            }
            GL.End();
        }

        public static void setColorDirt()
        {
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, EmbeddedSoilModel.ColorFrom(EmbeddedSoilModel._ColorDirt, 0.6f));
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, EmbeddedSoilModel.ColorFrom(EmbeddedSoilModel._ColorDirt, 0.4f));
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

        private PileData[] _PileData = new PileData[100];
        private volatile int _IntPileDex = 0;

        private float _PileLastX = 0;
        private float _PileLastY = 0;

        public float _VolumeLeftBin { get; private set; }
        public float _VolumeRightBin { get; private set; }
        public float _VolumeNoBin { get; private set; }

        internal int NewPileDataComing(float xpile, float ypile, float z, float amtpile, float simtime)
        {
            if ((this._PileLastX != xpile) || (this._PileLastY != ypile))
            {
                bool handled = false;

                float xdiff = 0;
                float ydiff = 0;
                float rdiff = 0;

                if (amtpile > 0)	// if not dumping in trench, then look to combine piles
                {
                    for (int p = 0; p < this._IntPileDex; p++)
                    {
                        PileData d = this._PileData[p];

                        ydiff = d.Y - ypile;
                        xdiff = d.X - xpile;
                        rdiff = (float)Math.Sqrt(xdiff * xdiff + ydiff * ydiff);

                        if ((rdiff < 8) && (d._FloatSoilAmount > 0))		// don't combine with other trench dumps
                        {
                            d._FloatSoilAmount += amtpile;	// combine piles
                            handled = true;
                            break;
                        }
                    }

                    xdiff = xpile - EmbeddedSoilModel._FloatBoxZ;
                    ydiff = ypile - EmbeddedSoilModel._FloatBoxX * (ypile < 0 ? -1 : 1);
                    rdiff = (float)Math.Sqrt(xdiff * xdiff + ydiff * ydiff);

                    PileData dat = new PileData();
                    dat.X = xpile;
                    dat.Y = ypile;
                    dat._FloatSoilAmount = amtpile;
                    dat._FloatDropTime = simtime;

                    amtpile *= 5;

                    if (!handled)
                    {
                        this._PileData[this._IntPileDex] = dat;
                        this._IntPileDex++;
                        this._IntPileDex %= this._PileData.Length;
                    }

                    if ((xpile > (EmbeddedSoilModel._FloatBoxZ - (sidelen / 2))) &&
                        (xpile < (EmbeddedSoilModel._FloatBoxZ + (sidelen / 2))) &&
                        (ypile > (EmbeddedSoilModel._FloatBoxX - (sidelen / 2))) &&
                        (ypile < (EmbeddedSoilModel._FloatBoxX + (sidelen / 2))))
                    {
                        soilheightL += amtpile / (sidelen * sidelen);

                        if (soilheightL >= boxheight) soilheightL = boxheight;

                        this._VolumeLeftBin += amtpile;

                        return TrialSaver.File2_DataType.inBinLeft;
                    }
                    else if ((xpile > (EmbeddedSoilModel._FloatBoxZ - (sidelen / 2))) &&
                            (xpile < (EmbeddedSoilModel._FloatBoxZ + (sidelen / 2))) &&
                            (ypile > (-EmbeddedSoilModel._FloatBoxX - (sidelen / 2))) &&
                            (ypile < (-EmbeddedSoilModel._FloatBoxX + (sidelen / 2))))
                    {
                        soilheightR += amtpile / (sidelen * sidelen);

                        if (soilheightR >= boxheight) soilheightR = boxheight;

                        this._VolumeRightBin += amtpile;

                        return TrialSaver.File2_DataType.inBinRight;
                    }
                    else
                    {
                        this._VolumeNoBin += amtpile;
                        return TrialSaver.File2_DataType.inBinMiss;
                    }
                }
                else //dumped into trench (amtpile is negative)
                {
                    xdiff = xpile - EmbeddedSoilModel._FloatBoxZ;
                    ydiff = ypile - EmbeddedSoilModel._FloatBoxX;
                    rdiff = (float)Math.Sqrt(xdiff * xdiff + ydiff * ydiff);
                    if (amtpile > 144)
                    {

                    }
                    return TrialSaver.File2_DataType.inBinTrench;
                }
            }
            else return Int32.MinValue;
        }

        public void drawPiles(float drawtime)
        {
            EmbeddedSoilModel.setColorDirt();

            float d_pile, temp, signy;
            float a = 12 * 32.174f;				//acceleration due to gravity in in/s^2
            float xoffset, yoffset, zoffset, dustnum;
            int k, n, i;

            ////////////////////////////////////////////////////////////////////////////////
            //				Display falling dirt										  //
            ////////////////////////////////////////////////////////////////////////////////
            if (this._IntPileDex > 0)
            {
                PileData pd = this._PileData[this._IntPileDex - 1];

                float deltat = drawtime - pd._FloatDropTime;
                deltat *= 1000;

                float z = pd.Z - (a * deltat * deltat) / 2.0f;	//z = z0 - 1/2*a*t^2  --> t is in ms

                dustnum = 900 * pd._FloatSoilAmount / EmbeddedSoilModel.Max_V_Load;	// % of maximum

                for (i = 0; i < dustnum; i++)
                {
                    float TRENCH_WIDTH2 = EmbeddedSoilModel.TRENCH_WIDTH / 2f;

                    xoffset = TRENCH_WIDTH2 * PileData.RandomM1toP1();
                    yoffset = TRENCH_WIDTH2 * PileData.RandomM1toP1();
                    zoffset = 30 * PileData.RandomM1toP1();

                    GL.Begin(BeginMode.Triangles);
                    {
                        PileData.glNormal3f(-1, 0, 1);
                        PileData.glVertex3f(pd.X + xoffset + 1, (pd.Y + yoffset + 1), z);//+zoffset);
                        PileData.glNormal3f(-1, 0, 1);
                        PileData.glVertex3f(pd.X + xoffset - 1, (pd.Y + yoffset), z + zoffset - 1);
                        PileData.glNormal3f(-1, 0, 1);
                        PileData.glVertex3f(pd.X + xoffset, (pd.Y + yoffset - 1), z + zoffset + 1);
                    }
                    GL.End();
                }

                // Actual piles

                int max = this._IntPileDex - (z < 0 ? 0 : 1);

                for (i = 0; i < max; i++)
                {
                    pd = this._PileData[i];

                    if (pd._FloatSoilAmount != 2222)
                    {
                        temp = (3 * pd._FloatSoilAmount) / 4;
                        d_pile = 40;

                        // take cube root using Newton-Raphson method 
                        // should converge in less than 10 steps
                        // d_pile = 40;
                        for (n = 0; n < 40; n++) d_pile -= (d_pile * d_pile * d_pile - temp) / (3 * d_pile * d_pile);

                        // red clay / dirt
                        GL.Color3(0.5, 0.4, 0.4);

                        signy = (pd.Y < 0) ? -1 : 1;

                        float Y_pile = ((Math.Abs(pd.Y) - Math.Abs(d_pile * 1.4142f)) < 12) ? (12 + d_pile * 1.4142f) * signy : pd.Y; //26.5*signy;// + d1;

                        GL_Handler.PushMatrix();
                        GL_Handler.Translate(-pd.Y, 0, -pd.X);

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
        }


        private class PileData
        {
            internal float X = 0;
            internal float Y = 0;
            internal float Z = 0;
            internal float _FloatSoilAmount = 0;
            internal float _FloatDropTime = 0;

            internal static void glVertex3f(float a, float b, float c) { GL.Vertex3(-b, c, -a); }
            internal static void glNormal3f(float a, float b, float c) { GL.Normal3(-b, c, -a); }
            internal static void glRotatef(float g, float a, float b, float c) {  }
            private static Random _Random = new Random();

            public static float RandomM1toP1() { return (((float)PileData._Random.Next()) * 2 / int.MaxValue) - 1; }
        }
    }
}
