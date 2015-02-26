/************************************************************
				limiterg_435_brake.c
/************************************************************
		
	Author:		Mark Elton
	Last Edit:	Dec 30, 2008

	This code limits the cylinder states if joint limit is reached
	
/************************************************************
	Based on sfuntmpl_basic.c: Basic 'C' template for a level 2 S-function
		*	Copyright 1990-2002 The MathWorks, Inc.
************************************************************/			
 
#define S_FUNCTION_NAME		limiterg_435_brake
#define S_FUNCTION_LEVEL 2

#include "simstruc.h"			// definition of the SimStruct
#include <math.h>
#include "bobcat_param.h"		// bobcat 435 parameters

static void mdlInitializeSizes(SimStruct *S)
{
    /* See sfuntmpl_doc.c for more details on the macros below */
    int_T k;

    ssSetNumSFcnParams(S, 1);  /* Number of expected parameters */
    if (ssGetNumSFcnParams(S) != ssGetSFcnParamsCount(S)) {
        /* Return if number of expected != number of actual parameters */
        return;
    }
	
    ssSetNumContStates(S, 0);		// set number of continous states = 0
	ssSetNumDiscStates(S, 4);	// set number of discrete states = 0

    if (!ssSetNumInputPorts(S, 4)) return;
	// if the number of input ports != 4 then return

	k = 0;			//	joint acceleration
    ssSetInputPortWidth(S, k, 4);                   // take 4 numbers as input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double

	k = k+1;		// joint velocity
	ssSetInputPortWidth(S, k, 4);                   // take 4 numbers as input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double
    
	k = k+1;		// ground limits
	ssSetInputPortWidth(S, k, 4);                   // take 4 numbers as input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double    

    k = k+1;		// brake
	ssSetInputPortWidth(S, k, 4);                   // take 1 number as input
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double   
    
    if (!ssSetNumOutputPorts(S, 3)) return;
	// if the number of input ports != 2 then return
	
	k = 0;		// joint accelerations
    ssSetOutputPortWidth(S, k, 4);              // output four numbers
	ssSetOutputPortDataType(S, k, SS_DOUBLE);	// of type double
	
	k = k+1;		// joint velocities
    ssSetOutputPortWidth(S, k, 4);              // output four numbers
	ssSetOutputPortDataType(S, k, SS_DOUBLE);	// of type double
    
	k = k+1;		// qd limiters
    ssSetOutputPortWidth(S, k, 4);              // output four numbers
	ssSetOutputPortDataType(S, k, SS_DOUBLE);	// of type double 
    
//     k = k+1;		// qdd limiters
//     ssSetOutputPortWidth(S, k, 4);              // output four numbers
// 	ssSetOutputPortDataType(S, k, SS_DOUBLE);	// of type double   
    
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
    real_T *STATES = ssGetRealDiscStates(S);
    int_T k = 0;
    
    for (k = 0; k < 4; k++)
    {
        STATES[k] = 1;
    }
}

#define MDL_START  /* Change to #undef to remove function */
static void mdlStart(SimStruct *S)
{
    real_T *STATES = ssGetRealDiscStates(S);
    int_T k = 0;
    
    for (k = 0; k < 4; k++)
    {
        STATES[k] = k+1;
    }    
}

#undef MDL_DERIVATIVES  /* Change to #undef to remove function */

static void mdlOutputs(SimStruct *S, int_T tid)
{
    const real_T *q = (const real_T*) ssGetInputPortSignal(S,0);		// get input port 1 values - joint angles
    const real_T *qd = (const real_T*) ssGetInputPortSignal(S,1);		// get input port 2 value - joint vel
    const real_T *glimit = (const real_T*) ssGetInputPortSignal(S,2);		// get input port 3 value - ground limits
    const real_T *brake = (const real_T*) ssGetInputPortSignal(S,3);
    
    real_T *Q = (real_T*) ssGetOutputPortSignal(S,0);		// set QDD to be the first output value - adjusted qdd
    real_T *QD = (real_T*) ssGetOutputPortSignal(S,1);		// set QD to be the second output value - adjusted qd
    //real_T *qddlimit = (real_T*) ssGetOutputPortSignal(S,3);		// stopping accelerations   
    real_T *qdlimit = (real_T*) ssGetOutputPortSignal(S,2);		// reseting integrators
    
    real_T *STATES = ssGetRealDiscStates(S);
    
	real_T tmin[4] = {t1min, t2min, t3min, t4min};   	// minimum length of cylinder (in)
	real_T tmax[4] = {t1max, t2max, t3max, t4max};   	// maximum cylinder length (in) (this is not the stroke)
    
    int_T k = 0;
    
    for (k = 0; k < 4; k++)
    {
        if ( (k == 0) && (brake[0] == 0) )
        {
            QD[k] = 0;
            Q[k] = q[k];
            STATES[k] = -STATES[k];
        }
        else
        {
            STATES[k] = STATES[k];
            if ((q[k] > tmax[k]) || (glimit[k] == 1))
            {
                //qddlimit[k] = 1;
                if (q[k] > tmax[k])
                {
                    Q[k] = tmax[k];  
                }
                if (qd[k] > 0)
                {
                    QD[k] = 0;
                    STATES[k] = -STATES[k];
                }
                else
                {
                    QD[k] = qd[k];
                }
            }
            else
            {
                if ((q[k] < tmin[k]) || (glimit[k] == -1))
                {
                    //qddlimit[k] = -1;                
                    if (q[k] < tmin[k])
                    {
                        Q[k] = tmin[k];
                    }
                    if (qd[k] < 0)
                    {
                        QD[k] = 0;
                        STATES[k] = -STATES[k];
                    }
                    else
                    {
                        QD[k] = qd[k];
                    }
                }
                else
                {
                    Q[k] = q[k];
                    QD[k] = qd[k];
                    STATES[k] = STATES[k];
                    //qddlimit[k] = 0;
                }
            }
        }
        qdlimit[k] = STATES[k];
    }
    
            
//         QDD[k] = qdd[k];
//         QD[k] = qd[k];
//         if (limit[k] == -1)
//         {
//             if (qdd[k] < 0)
//             {
//                 QDD[k] = 0;
//             }
//             if (qd[k] < 0)
//             {
//                 QD[k] = 0;
//             }
//         }
//         if (limit[k] == 1)
//         {
//             if (qdd[k] > 0)
//             {
//                 QDD[k] = 0;
//             }
//             if (qd[k] > 0)
//             {
//                 QD[k] = 0;
//             }
//         }
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
