/************************************************************
				p2torq2_435.c
/************************************************************
		
	Author:		Mark Elton
	Last Edit:		Aug 8, 2008

	This code takes the pressure and velocity of the boom cylinder
	of a Bobcat 435 excavator and calculates the joint torque.

	Thanks to Chris Williamson for providing frictional coeff
	
/************************************************************
	Based on sfuntmpl_basic.c: Basic 'C' template for a level 2 S-function
		*	Copyright 1990-2002 The MathWorks, Inc.
************************************************************/			

#define S_FUNCTION_NAME		p2torq2_435
#define S_FUNCTION_LEVEL 2

#include "simstruc.h"			// definition of the SimStruct
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
	
    ssSetNumContStates(S, 0);		// set number of continous states = 0
	ssSetNumDiscStates(S, 0);	// set number of discrete states = 0

    if (!ssSetNumInputPorts(S, 4)) return;
	// if the number of input ports != 4 then return

	k = 0;			// cylinder headside pressure, phead (psi)
    ssSetInputPortWidth(S, k, 1);				// take only one number as input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double

	k = k+1;		// cylinder rodside pressure, prod (psi)
	ssSetInputPortWidth(S, k, 1);				// take only one number as input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double

	k = k+1;		// boom cylinder velocity, v2 (in/s)
	ssSetInputPortWidth(S, k, 1);				// take only one number as input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double

	k = k+1;		// C2 - see j2c_435.c for definition
	ssSetInputPortWidth(S, k, 1);				// take only one number as input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// boolean_T 


    if (!ssSetNumOutputPorts(S, 1)) return;
	// if the number of input ports != 1 then return
	
	k = 0;		// pressure head side
    ssSetOutputPortWidth(S, k, 1);			// output only one number
	ssSetOutputPortDataType(S, k, SS_DOUBLE);	// of type double

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
    const real_T *pA = (const real_T*) ssGetInputPortSignal(S,0);		// get input port 1 value, headside pressure (psi)
    const real_T *pB = (const real_T*) ssGetInputPortSignal(S,1);		// get input port 2 value, rodside pressure (psi)
    const real_T *v2 = (const real_T*) ssGetInputPortSignal(S,2);		// get input port 3 value, cylinder vel (in/s)
    const real_T *C2 = (const real_T*) ssGetInputPortSignal(S,3);		// get input port 4 value, C2 (see j2c_435.c)

    real_T *tau = (real_T*) ssGetOutputPortSignal(S,0);		// set tau to be the output value

    time_T Ts = (time_T) *mxGetPr(ssGetSFcnParam(S, 0));			// get first parameter - sampling time
    real_T c_cyl_Bm = (real_T) *mxGetPr(ssGetSFcnParam(S, 1));			// get second parameter - frictional coeff
    real_T fc_Bm = (real_T) *mxGetPr(ssGetSFcnParam(S, 2));			// get second parameter - frictional coeff
    real_T f2;
    
    //c_cyl_Bm = 3e4;  %[kg/s] coeff of viscous friction for cylinder seals and bearing 
    real_T v = v2[0]*in_2_m;	// convert v2 from in/s to m/s
    //real_T stiction = 5e3*exp(-10*abs(v));
    real_T stiction = 0*5e3*exp(-10*abs(v));
    
    real_T signv2 = 1;
    if (v2[0] < 0)
    {
        signv2 = -1;  
    }
    
    f2 = (pA[0]*area_head2 - pB[0]*area_rod2 - (v*c_cyl_Bm)*N_2_lb - fc_Bm*signv2); // magnitude of cyl force
    
    
	if (abs(stiction) > abs(f2))
    {
        f2 = 0;
    }
    else
    {
        if(f2 > 0)
        {
            f2 = f2 - stiction;
        }
        else
        {
            f2 = f2 + stiction;
        }
    }
    
    tau[0] = C2[0]*f2/12;    //inlbs/12 = ftlbs
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
