/************************************************************
				p2torq4_435.c
/************************************************************
		
	Author:		Mark Elton
	Last Edit:		Aug 9, 2008

	This code takes the headside and rodside pressures and the
	bucket cylinder velocity of a Bobcat 435 mini-excavator
	and calculates the torque about the bucket joint.

	Thanks to Chris Williamson for providing the frictional coeff	

/************************************************************
	Based on sfuntmpl_basic.c: Basic 'C' template for a level 2 S-function
		*	Copyright 1990-2002 The MathWorks, Inc.
************************************************************/			

#define S_FUNCTION_NAME		p2torq4_435
#define S_FUNCTION_LEVEL 2

#include "simstruc.h"			// definition of the SimStruct
#include <math.h>
#include "bobcat_param.h"		// parameters of the bobcat 435 mini-excavator

static void mdlInitializeSizes(SimStruct *S)
{
    /* See sfuntmpl_doc.c for more details on the macros below */
    int_T k;

    ssSetNumSFcnParams(S, 3);  /* Number of expected parameters */
    if (ssGetNumSFcnParams(S) != ssGetSFcnParamsCount(S)) {
        /* Return if number of expected != number of actual parameters */
        return;
    }
	
    ssSetNumContStates(S, 0);		// set number of continuous states = 0
	ssSetNumDiscStates(S, 1);	// set number of discrete states = 1

    if (!ssSetNumInputPorts(S, 4)) return;
	// if the number of input ports != 4 then return

	k = 0;			// head side pressure, phead (psi)
    ssSetInputPortWidth(S, k, 1);				// set port to accept only one number
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// port type is real_T

	k = k+1;		// pressure rod side, prod (psi)
	ssSetInputPortWidth(S, k, 1);				// set port to accept only one number
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// port type is real_T

	k = k+1;		// bucket cylinder velocity, v4 (in/s)
	ssSetInputPortWidth(S, k, 1);				// set port to accept only one number
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// port type is real_T

	k = k+1;		// C4 - see c2j_435.c
	ssSetInputPortWidth(S, k, 1);				// set port to accept only one number
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// port type is real_T


    if (!ssSetNumOutputPorts(S, 1)) return;
	//if the number of output ports != 1 then return
	
	k = 0;		// torque (ftlbs)
    ssSetOutputPortWidth(S, k, 1);			// output only one number 
	ssSetOutputPortDataType(S, k, SS_DOUBLE);	// of type real_T

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
    time_T Ts = (time_T) *mxGetPr(ssGetSFcnParam(S, 0));			// get first parameter - sampling time
	int_T k;

	ssSetSampleTime(S, 0, Ts);
    ssSetOffsetTime(S, 0, 0.0);
		
	for(k = 0; k < ssGetNumInputPorts(S); k++) {
		ssSetInputPortSampleTime(S, k, Ts);
		ssSetInputPortOffsetTime(S, k, 0.0);
	}

	for(k = 0; k < ssGetNumOutputPorts(S); k++) {
		ssSetOutputPortSampleTime(S, k, Ts);
		ssSetOutputPortOffsetTime(S, k, 0.0);
	}	
}

#define MDL_INITIALIZE_CONDITIONS   /* Change to #undef to remove function */
static void mdlInitializeConditions(SimStruct *S)
{
    real_T *STATES = ssGetRealDiscStates(S);
    STATES[0] = 0;
}

#define MDL_START  /* Change to #undef to remove function */
static void mdlStart(SimStruct *S)
{
}

#undef MDL_DERIVATIVES  /* Change to #undef to remove function */

static void mdlOutputs(SimStruct *S, int_T tid)
{
    const real_T *pA = (const real_T*) ssGetInputPortSignal(S,0);		// get head side pressure (psi) - first input port
    const real_T *pB = (const real_T*) ssGetInputPortSignal(S,1);		// get rod side pressure (psi) - second input port
    const real_T *v4 = (const real_T*) ssGetInputPortSignal(S,2);		// get bucket cylinder velocity (in/s) - 3rd input port
    const real_T *C4 = (const real_T*) ssGetInputPortSignal(S,3);		// get C4 - fourth input port
    real_T *tau = (real_T*) ssGetOutputPortSignal(S,0);			// set tau to be output variable
    
    real_T *STATES = ssGetRealDiscStates(S);

    time_T Ts = (time_T) *mxGetPr(ssGetSFcnParam(S, 0));			// get first parameter - sampling time (s)
    real_T c_cyl_Bk = (real_T) *mxGetPr(ssGetSFcnParam(S, 1));			// get second parameter - friction coeff
    real_T fc_Bk = (real_T) *mxGetPr(ssGetSFcnParam(S, 2));			// get second parameter - friction coeff
    real_T f4, fc, timeconst = 20;
    //c_cyl_Bm = 3e4;  %[kg/s] coeff of viscous friction for cylinder seals and bearing 
    
    real_T v = v4[0]*in_2_m;	// convert cylinder velocity from in/s -> m/s
     //real_T stiction = 5e3*exp(-10*abs(v));
    real_T stiction = 0*5e3*exp(-10*abs(v));
    
    real_T signv4 = 1;
    if (v4[0] < 0)
        signv4 = -1;
    
    fc = STATES[0] + timeconst*Ts*(signv4*fc_Bk- STATES[0]);
    STATES[0] = fc;
    
    f4 = pA[0]*area_head4 - pB[0]*area_rod4 - (v*c_cyl_Bk)*N_2_lb - signv4*fc_Bk; // magnitude of cyl force
    //f4 = pA[0]*area_head4 - pB[0]*area_rod4 - (v*c_cyl_Bk)*N_2_lb - signv4*fc; // magnitude of cyl force
    
	if (abs(stiction) > abs(f4))
    {
        f4 = 0;
    }
    else
    {
        if (f4 > 0)
        {
            f4 = f4 - stiction;
        }
        else
        {
            f4 = f4 + stiction;
        }
    }
    
    
    tau[0] = C4[0]*f4/12;    // inlbs/12 = ftlbs

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
