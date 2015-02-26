/************************************************************
				p2torq3_435.c
/************************************************************
		
	Author:		Mark Elton
	Last Edit:		Aug 8, 2008

	This code takes the head side and rod side pressures and 
	Stick cylinder velocity for a bobcat 435 mini-excavator

	Thanks to Chris Williamson for providing the frictional coeffs
	
/************************************************************
	Based on sfuntmpl_basic.c: Basic 'C' template for a level 2 S-function
		*	Copyright 1990-2002 The MathWorks, Inc.
************************************************************/			

#define S_FUNCTION_NAME		p2torq3_435
#define S_FUNCTION_LEVEL 2

#include "simstruc.h"			//  definition of the SimStruct
#include <math.h>
#include "bobcat_param.h"		// bobcat 435 parameters

static void mdlInitializeSizes(SimStruct *S)
{
    /* See sfuntmpl_doc.c for more details on the macros below */
    int_T k;

    ssSetNumSFcnParams(S, 3);  /* Number of expected parameters */
    if (ssGetNumSFcnParams(S) != ssGetSFcnParamsCount(S)) {
        /* Return if number of expected != number of actual parameters */
        return;
    }
	
    ssSetNumContStates(S, 0);			// set number of continous states = 0
	ssSetNumDiscStates(S, 0);		// set number of discrete states = 0

    if (!ssSetNumInputPorts(S, 4)) return;
        /* Return if number of input ports != 4 */

	k = 0;			//  head side pressure, phead (psi)
    ssSetInputPortWidth(S, k, 1);				// set port to accept only one number
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double or real_T	

	k = k+1;		// rod side pressure, prod (psi)
	ssSetInputPortWidth(S, k, 1);				// set port to accept only one number
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double or real_T 

	k = k+1;		// velocity of stick cylinder, v3 (in/s)
	ssSetInputPortWidth(S, k, 1);				// set port to accept only one number
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// port type double

	k = k+1;		// C3 - see c2j_435.c for definition
	ssSetInputPortWidth(S, k, 1);				// set input port to accept only one number
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// port type double


    if (!ssSetNumOutputPorts(S, 1)) return;
	// if number of output ports !=1 then return

	k = 0;		// pressure head side
    ssSetOutputPortWidth(S, k, 1);			//output one number
	ssSetOutputPortDataType(S, k, SS_DOUBLE);		// of type double

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
}

#define MDL_START  /* Change to #undef to remove function */
static void mdlStart(SimStruct *S)
{
}

#undef MDL_DERIVATIVES  /* Change to #undef to remove function */

static void mdlOutputs(SimStruct *S, int_T tid)
{
    const real_T *pA = (const real_T*) ssGetInputPortSignal(S,0);		// get headside pressure - first input port
    const real_T *pB = (const real_T*) ssGetInputPortSignal(S,1);		// get rodside pressure - second input port
    const real_T *v3 = (const real_T*) ssGetInputPortSignal(S,2);		// get cylinder velocity - third input port
    const real_T *C3 = (const real_T*) ssGetInputPortSignal(S,3);		// get C3 - fourth input port
    real_T *tau = (real_T*) ssGetOutputPortSignal(S,0);			// set tau to be first output variable

    time_T Ts = (time_T) *mxGetPr(ssGetSFcnParam(S, 0));			// first parameter - sampling time
    real_T c_cyl_St = (real_T) *mxGetPr(ssGetSFcnParam(S, 1));		// second parameter - friction coeff
    real_T fc_St = (real_T) *mxGetPr(ssGetSFcnParam(S, 2));		// second parameter - friction coeff    
    real_T f3;		// cylinder force

    //c_cyl_St = 3e4;  %[kg/s] coeff of viscous friction for cylinder seals and bearing 

    real_T v = v3[0]*in_2_m;	// convert v3 from in/s -> m/s
    //real_T stiction = 5e3*exp(-10*abs(v));
    real_T stiction = 0*5e3*exp(-10*abs(v));
    
    real_T signv3 = 1;
    if (v3[0] < 0)
        signv3 = -1;      
    
    f3 = pA[0]*area_head3 - pB[0]*area_rod3 - (v*c_cyl_St)*N_2_lb - signv3*fc_St; // magnitude of cyl force

    if (abs(stiction) > abs(f3))
    {
        f3 = 0;
    }
    else
    {
        if (f3 > 0)
        {
            f3 = f3 - stiction;
        }
        else
        {
            f3 = f3 + stiction;
        }
    }
    
    tau[0] = C3[0]*f3/12;    // inlbs/12 = ftlbs
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
