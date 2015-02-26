/************************************************************
				p2torq1_435_brake.c
/************************************************************
		
	Author:		Mark Elton
	Last Edit:		Aug 8, 2008

	This code takes the pressure and velocity of the swing
	motor of a Bobcat 435 excavator and calculates the joint
	torque.

	Thanks to Chris Williamson for writing this matlab script
	and for providing frictional coeff
	
/************************************************************
	Based on sfuntmpl_basic.c: Basic 'C' template for a level 2 S-function
		*	Copyright 1990-2002 The MathWorks, Inc.
************************************************************/			
 
#define S_FUNCTION_NAME		p2torq1_435_brake
#define S_FUNCTION_LEVEL 2

#include "simstruc.h"			// definition of the SimStruct
#include <math.h>
#include "bobcat_param.h"		// bobcat 435 excavator parameters


static void mdlInitializeSizes(SimStruct *S)
{
    /* See sfuntmpl_doc.c for more details on the macros below */
    int_T k;

    ssSetNumSFcnParams(S, 4);  /* Number of expected parameters */
    if (ssGetNumSFcnParams(S) != ssGetSFcnParamsCount(S)) {
        /* Return if number of expected != number of actual parameters */
        return;
    }
	
    ssSetNumContStates(S, 0);		// set number of continuous states = 0
    ssSetNumDiscStates(S, 0);		// set number of discrete states = 0

    if (!ssSetNumInputPorts(S, 4)) return;
        /* Return if number of input ports != 2*/

	k = 0;			// headside pressure (psi), or phead
    ssSetInputPortWidth(S, k, 1);				// accept only one number as an input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double	

	k = k+1;		// rodside pressure (psi), or prod
	ssSetInputPortWidth(S, k, 1);				// accept only one number as an input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double
    
    k = k+1;		// cab swing velocity, w1 (rad/s)
	ssSetInputPortWidth(S, k, 1);				// accept only one number as an input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double
    
    k = k+1;		// cab swing brake on or off
	ssSetInputPortWidth(S, k, 1);				// accept only one number as an input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double


    if (!ssSetNumOutputPorts(S, 1)) return;
        /* Return if number of output ports != 1 */
	
	k = 0;		// torque about swing axis (ftlb)
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
    const real_T *phead = (const real_T*) ssGetInputPortSignal(S,0);		// get headside pressure
    const real_T *prod = (const real_T*) ssGetInputPortSignal(S,1);		// get rodside pressure
    const real_T *w1 = (const real_T*) ssGetInputPortSignal(S,2);		// get cab swing velocity  
    const real_T *brake = (const real_T*) ssGetInputPortSignal(S,3);		// get cab swing velocity  
    
	real_T *tau = (real_T*) ssGetOutputPortSignal(S,0);		// swing torque

	real_T Ts = (real_T) *mxGetPr(ssGetSFcnParam(S, 0));			// sampling time
	real_T c_swing = (real_T) *mxGetPr(ssGetSFcnParam(S, 1));			// friction coeff
    real_T fc = (real_T) *mxGetPr(ssGetSFcnParam(S, 2));			// friction coeff
    real_T fbrake = (real_T) *mxGetPr(ssGetSFcnParam(S, 3));			// friction coeff
    
    real_T signw1 = 1;
	
    // Swing
    //Vm = 820e-6;    // [m^3/rev] swing motor displacement
    //c_swing = 600;  // [N*m*s] coeff of viscous friction
    //i_swing = 91/17; // motor to swing crown gear ratio

    real_T pA = phead[0] * psi_2_pa;		// convert to pascals
    real_T pB = prod[0] * psi_2_pa;		// convert to pascals
    
    if (w1[0] < 0)
        signw1 = -1;
    
    tau[0] = (pA - pB)*Vm/(2*pi) - c_swing*w1[0] - fc*signw1 - fbrake*brake[0]*signw1;	// calculate torque in Nm
    tau[0] = tau[0]*Nm_2_ftlb;	// convert from Nm to ftlbs
    //if (brake == 0)
       // tau[0] = (- c_swing*w1[0] - fc*signw1)*5000;	// calculate torque in Nm
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
