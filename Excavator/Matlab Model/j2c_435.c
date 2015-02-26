/************************************************************
				j2c_435.c
/************************************************************
		
	Author:		Mark Elton
	Last Edit:		Aug 8 2008

	This code converts the joint angles and velocities of a
	Bobcat 435 excavator to cylinder lengths and velocities

	It is based heavily on code written by Matt Kontz for a
	John Deere tractor mounted backhoe
	
/************************************************************
	Based on sfuntmpl_basic.c: Basic 'C' template for a level 2 S-function
		*	Copyright 1990-2002 The MathWorks, Inc.
************************************************************/			

#define S_FUNCTION_NAME		j2c_435
#define S_FUNCTION_LEVEL 2

#include "simstruc.h"			// definition of the SimStruct
#include <math.h>
#include "bobcat_param.h"		// parameters of the bobcat 435 mini-excavator

static void mdlInitializeSizes(SimStruct *S)
{
    /* See sfuntmpl_doc.c for more details on the macros below */

    ssSetNumSFcnParams(S, 1);  /* Number of expected parameters */
    if (ssGetNumSFcnParams(S) != ssGetSFcnParamsCount(S)) {
        /* Return if number of expected != number of actual parameters */
        return;
    }

    ssSetNumContStates(S, 0);		// Set the number of continuous states = 0
    ssSetNumDiscStates(S, 0);		// Set the number of discrete states = 0

    if (!ssSetNumInputPorts(S, 1)) return;
	// if the number of input ports != 1 then return

	// joint angles (rad) and joint velocities (rad/s)
    ssSetInputPortWidth(S, 0, 8);				// first input port takes eight numbers
    ssSetInputPortRequiredContiguous(S, 0, true); /*direct input signal access*/
    ssSetInputPortDirectFeedThrough(S, 0, 1);	// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, 0, SS_DOUBLE);		// type double

    if (!ssSetNumOutputPorts(S, 2)) return;
	// if the number of output ports != 2 then return

	// output port 1 = cylinder lengths (in) and velocities (in/s) and C's
    ssSetOutputPortWidth(S, 0, 8);              // set first output port to send 8 numbers
	ssSetOutputPortDataType(S, 0, SS_DOUBLE);	// of type real_T
    
	// output port 2 = C's
    ssSetOutputPortWidth(S, 1, 4);              // set first output port to send 4 numbers
	ssSetOutputPortDataType(S, 1, SS_DOUBLE);	// of type real_T    

    ssSetNumSampleTimes(S, 1);
    ssSetNumRWork(S, 0);
    ssSetNumIWork(S, 0);
    ssSetNumPWork(S, 0);
    ssSetNumModes(S, 0);
    ssSetNumNonsampledZCs(S, 0);

    ssSetOptions(S, 0);
}

static void mdlInitializeSampleTimes(SimStruct *S)
{
	time_T Ts = (time_T) *mxGetPr(ssGetSFcnParam(S, 0));			// sampling time
	ssSetSampleTime(S, 0, Ts);
    ssSetOffsetTime(S, 0, 0.0);

	ssSetInputPortSampleTime(S, 0, Ts);
	ssSetInputPortOffsetTime(S, 0, 0.0);

	ssSetOutputPortSampleTime(S, 0, Ts);
	ssSetOutputPortOffsetTime(S, 0, 0.0);
}

#undef MDL_INITIALIZE_CONDITIONS   /* Change to #undef to remove function */

#undef MDL_START  /* Change to #undef to remove function */

#undef MDL_UPDATE  /* Change to #undef to remove function */

#undef MDL_DERIVATIVES  /* Change to #undef to remove function */

static void mdlOutputs(SimStruct *S, int_T tid)
{
    const real_T *U = (const real_T*) ssGetInputPortSignal(S,0);
	real_T *Y = (real_T*) ssGetOutputPortSignal(S,0);       // first output port
    real_T *C = (real_T*) ssGetOutputPortSignal(S,1);       // second output port

	// variable and parameters definitions
	real_T t1, t2, t3, t4;
	real_T w1, w2, w3, w4;
	real_T y1, y2, y3, y4;
	real_T v1, v2, v3, v4;
	real_T tA1B, tC2D, tEFH;
	real_T C2, C3, C4;
	real_T cGFH, sGFH, tGFH;
	real_T sBeta, beta, beta_dot;
	real_T xg, yg, rC, rS;
	real_T rFG_2, rFG, wGFH, wEFH; 

	/************************************************************
				Joint-Space Input Variables
	/************************************************************
			y = cylinder length (inches)
			v = cylinder velocity (in/s)
			t = theta = joint angle	(rad)
			w = omega = joint velocity (rad/s)
	************************************************************/

	y1 = U[0];	// y1 = t1 since there is no cylinder length for the swing motor
	t2 = U[1];
	t3 = U[2];
	t4 = U[3];
	v1 = U[4];	// v1 = w1 since swing motor has no cylinder velocity
	w2 = U[5];
	w3 = U[6];
	w4 = U[7];

	///************************************************************
	//				Boom - axis #2
	//************************************************************/
 
    //tA1B = pi - t01A - t2 - tB12;
    tA1B = tB12 + t2 + pi - t01A;
	y2 = sqrt(r1A*r1A + r1B*r1B - 2*r1A*r1B*cos(tA1B));
	C2 = r1A*r1B*sin(tA1B)/y2;
	//t1BA = acos((r1A*r1A - r1B*r1B - y2*y2)/(-2*y2*r1B));
    //C2 = r1B*sin(t1BA);
    v2 = C2*w2;
	//f2 = T2/C2;

	///************************************************************
	//				Stick - axis #3
	//************************************************************/
	//tC2D = pi - t12C - t3 - tD23;
    //tC2D = pi - t12C - t3 - tD23;
    tC2D = pi - t12C - t3 - tD23;
	y3 = sqrt(r2C*r2C + r2D*r2D - 2*r2C*r2D*cos(tC2D));
	C3 = -r2C*r2D*sin(tC2D)/y3;
	v3 = C3*w3;
	//f3 = T3/C3;

	///************************************************************
	//				Bucket - axis #4
	//************************************************************/
	xg = r3G*cos(t4 + tG34);
	yg = r3G*sin(t4 + tG34);
	rFG_2 = (yg - rFI)*(yg - rFI) + (r3I + xg)*(r3I + xg);
	rFG = sqrt(rFG_2);
	sBeta = (yg - rFI)/rFG; //sin(beta)
	beta = asin(sBeta); //t3FG?
	cGFH = (rFH*rFH + rFG*rFG - rGH*rGH)/(2*rFH*rFG);
	tGFH = acos(cGFH);
	sGFH = sin(tGFH);
    //tEFH = pi - tGFH - beta - tGFH;
	//tEFH = pi - alpha;
    tEFH = pi - alfa;
    tEFH = tEFH - beta - tGFH;
	y4 = sqrt(rEF*rEF + rFH*rFH - 2*rEF*rFH*cos(tEFH));
	rC = r3G*cos(t4 + tG34);
	rS = r3G*sin(t4 + tG34);
	beta_dot = ((rFG + rFI*sBeta)*rC + r3I*sBeta*rS)/(rFG_2*cos(beta));
	
    wGFH = -(rFH*cGFH - rFG)*(rFI*rC +r3I*rS)/(rFH*rFG_2*sGFH);
	wEFH = -beta_dot - wGFH;
	C4 = rEF*rFH*sin(tEFH)/y4*wEFH;
    
    
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
    
	v4 = C4*w4;

	/************************************************************
				Cylinder-Space Output Variables
	************************************************************/

    Y[0] = y1;
	Y[1] = y2;
	Y[2] = y3;
	Y[3] = y4;
	Y[4] = v1;
	Y[5] = v2;
	Y[6] = v3;
	Y[7] = v4;
	
    C[0] = 0;
	C[1] = C2;
	C[2] = C3;
	C[3] = C4;
}

static void mdlTerminate(SimStruct *S)
{
}


/*======================================================*
 * See sfuntmpl_doc.c for the optional S-function methods *
 *======================================================*/

/*=============================*
 * Required S-function trailer *
 *=============================*/

#ifdef  MATLAB_MEX_FILE    /* Is this file being compiled as a MEX-file? */
#include "simulink.c"      /* MEX-file interface mechanism */
#else
#include "cg_sfun.h"       /* Code generation registration function */
#endif
