/************************************************************
				cylmodel_435_ryder.c
/************************************************************
		
	Author:		Mark Elton
	Last Edit:	Aug 21, 2008

	This code models the swing motor and 3 arm cylinders of
	the bobcat 435 mini-excavator. The inputs are pump flows,
	and the outputs are headside and rodside pressures

	
/************************************************************
	Based on sfuntmpl_basic.c: Basic 'C' template for a level 2 S-function
		*	Copyright 1990-2002 The MathWorks, Inc.
************************************************************/			
 
#define S_FUNCTION_NAME		cylmodel_435_ryder
#define S_FUNCTION_LEVEL 2

#include "simstruc.h"			//  definition of the SimStruct
#include <math.h>
#include "bobcat_param.h"		// includes parameters of the bobcat 435


static void mdlInitializeSizes(SimStruct *S)
{
    /* See sfuntmpl_doc.c for more details on the macros below */
    int_T k;
	int_T NumStates = 8;		//4 headside and 4 rodside pressures

    ssSetNumSFcnParams(S, 27);  /* Number of expected parameters */
    if (ssGetNumSFcnParams(S) != ssGetSFcnParamsCount(S)) 
    {
        /* Return if number of expected != number of actual parameters */
        return;
    }
	
    ssSetNumContStates(S, 0);			//set the number of continuous states = 0
	ssSetNumDiscStates(S, NumStates);	//set the number of discrete states = NumStates

    if (!ssSetNumInputPorts(S, 3)) return;
        /* Return if number of input ports != 3*/

	k = 0;			// Cylinder position in inches
    ssSetInputPortWidth(S, k, 4);                   // accept 4 numbers
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double (or real_T)

	k = k+1;		// Cylinder velocity in inches/sec
	ssSetInputPortWidth(S, k, 4);                   // accept 4 numbers
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double or real_T

	k = k+1;		// Pump flow in in^3/s
    ssSetInputPortWidth(S, k, 4);                   // accept 4 numbers
    ssSetInputPortRequiredContiguous(S, k, true);	// direct input signal access
    ssSetInputPortDirectFeedThrough(S, k, 1);		// direct feedthrough flag (1=yes, 0=no)
	ssSetInputPortDataType(S, k, SS_DOUBLE);		// type double or real_T

    if (!ssSetNumOutputPorts(S, 5)) return;
	/* Return if number of output ports != 2*/

	k = 0;		// pressure head side
    ssSetOutputPortWidth(S, k, 4);				// output 4 numbers
	ssSetOutputPortDataType(S, k, SS_DOUBLE);	// of type real_T	
	
	k = k+1;	// pressure rod side
    ssSetOutputPortWidth(S, k, 4);				// output 4 numbers
	ssSetOutputPortDataType(S, k, SS_DOUBLE);	// of type real_T	
    
	k = k+1;	// speaker volume
    ssSetOutputPortWidth(S, k, 1);				// output 1 number
	ssSetOutputPortDataType(S, k, SS_UINT16);	// type unsigned int -> 16bits    

    k = k+1;	// motor flow
    ssSetOutputPortWidth(S, k, 1);				// output 1 number
	ssSetOutputPortDataType(S, k, SS_DOUBLE);	// of type real_T	

    k = k+1;	// motor flow
    ssSetOutputPortWidth(S, k, 4);				// output 4 numbers
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
	// States (headside and rodside pressures)
	real_T *STATES = ssGetRealDiscStates(S);

	real_T Ts = (real_T) *mxGetPr(ssGetSFcnParam(S, 0));			// get sampling time - first function param
	real_T ptank = (real_T) *mxGetPr(ssGetSFcnParam(S, 1));			// get tank pressure (psi) - second param
	real_T bulk_mod = (real_T) *mxGetPr(ssGetSFcnParam(S, 2));			// get effective bulk modulus - third param

	int_T k;

	real_T area_head[4] = {0, area_head2, area_head3, area_head4};	// cylinder head areas on the head side - defined in bobcat_param.h
	real_T area_rod[4] = {0, area_rod2, area_rod3, area_rod4};		// cylinder head areas on the rod side - defined in bobcat_param.h

	
	real_T linevol_head[4], linevol_rod[4]; 		// line volumes (head and rodside) in cubic in 
	real_T head_length[4];					// length of cylinder head in inches
	real_T end_rod[4];				 	// length of rod sticking out when fully retracted (in)
	real_T p_relief[4], leakcoeff[4];			// pressure relief valve settings (psi), leakage coeffs

	real_T ymin[4] = {0, Ymin2, Ymin3, Ymin4};   	// minimum length of cylinder (in)
	real_T ymax[4] = {0, Ymax2, Ymax3, Ymax4};   	// maximum cylinder length (in) (this is not the stroke)
	real_T stroke[4] = {0, stroke2, stroke3, stroke4};  // cylinder stroke (in)

	for (k = 0; k < 4; k++) {
		STATES[k] = ptank;		//headside pressures
		STATES[k+4] = ptank;		//rodside pressures
		linevol_head[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+3));	
		linevol_rod[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+7));	
		head_length[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+11));
		end_rod[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+15));
		p_relief[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+19));
		leakcoeff[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+23));
	}
}

#undef MDL_START  /* Change to #undef to remove function */
// static void mdlStart(SimStruct *S)
// {
// 	// States (headside and rodside pressures)
// 	real_T *STATES = ssGetRealDiscStates(S);
// 
// 	real_T Ts = (real_T) *mxGetPr(ssGetSFcnParam(S, 0));			// get sampling time - first function param
// 	real_T ptank = (real_T) *mxGetPr(ssGetSFcnParam(S, 1));			// get tank pressure (psi) - second param
// 	real_T bulk_mod = (real_T) *mxGetPr(ssGetSFcnParam(S, 2));			// get effective bulk modulus - third param
// 
// 	int_T k;
// 
// 	real_T area_head[4] = {0, area_head2, area_head3, area_head4};	// cylinder head areas on the head side - defined in bobcat_param.h
// 	real_T area_rod[4] = {0, area_rod2, area_rod3, area_rod4};		// cylinder head areas on the rod side - defined in bobcat_param.h
// 
// 	
// 	real_T linevol_head[4], linevol_rod[4]; 		// line volumes (head and rodside) in cubic in 
// 	real_T head_length[4];					// length of cylinder head in inches
// 	real_T end_rod[4];				 	// length of rod sticking out when fully retracted (in)
// 	real_T p_relief[4], leakcoeff[4];			// pressure relief valve settings (psi), leakage coeffs
// 
// 	real_T ymin[4] = {0, Ymin2, Ymin3, Ymin4};   	// minimum length of cylinder (in)
// 	real_T ymax[4] = {0, Ymax2, Ymax3, Ymax4};   	// maximum cylinder length (in) (this is not the stroke)
// 	real_T stroke[4] = {0, stroke2, stroke3, stroke4};  // cylinder stroke (in)
// 
// 	for (k = 0; k < 4; k++) {
// 		STATES[k] = 0;		//headside pressures
// 		STATES[k+4] = 0;		//rodside pressures
// 		linevol_head[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+3));	
// 		linevol_rod[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+7));	
// 		head_length[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+11));
// 		end_rod[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+15));
// 		p_relief[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+19));
// 		leakcoeff[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+23));
// 	}
// }

#undef MDL_DERIVATIVES  /* Change to #undef to remove function */

static void mdlOutputs(SimStruct *S, int_T tid)
{

    const real_T *y = (const real_T*) ssGetInputPortSignal(S,0);	// first input port is cylinder position
    const real_T *v = (const real_T*) ssGetInputPortSignal(S,1);	// second input port is cylinder velocity
    const real_T *flow = (const real_T*) ssGetInputPortSignal(S,2);	// third input port is pump flow
	real_T *phead = (real_T*) ssGetOutputPortSignal(S,0);			// first output port is headside pressure
	real_T *prod = (real_T*) ssGetOutputPortSignal(S,1);			// second output port is rodside pressure
	uint16_T *VOLUME = ssGetOutputPortSignal(S,2);         			// third output is engine volume
    real_T *motorvalveflow = (real_T*) ssGetOutputPortSignal(S,3);			// second output port is rodside pressure
    real_T *outwithit = (real_T*) ssGetOutputPortSignal(S,4);			// second output port is rodside pressure
    

	// States (headside and rodside pressures)
	real_T *STATES = ssGetRealDiscStates(S);

	real_T Ts = (real_T) *mxGetPr(ssGetSFcnParam(S, 0));			// get sampling time - first function param
	real_T ptank = (real_T) *mxGetPr(ssGetSFcnParam(S, 1));			// get tank pressure (psi) - second param
	real_T bulk_mod = (real_T) *mxGetPr(ssGetSFcnParam(S, 2));		// get effective bulk modulus - third param

	int_T k;

	real_T area_head[4] = {0, area_head2, area_head3, area_head4};	// cylinder head areas on the head side - defined in bobcat_param.h
	real_T area_rod[4] = {0, area_rod2, area_rod3, area_rod4};		// cylinder head areas on the rod side - defined in bobcat_param.h
	
	real_T linevol_head[4], linevol_rod[4]; 	// line volumes (head and rodside) in cubic in 
	real_T head_length[4];                      // length of cylinder head in inches
	real_T end_rod[4];                          // length of rod sticking out when fully retracted (in)
	real_T p_relief[4], leakcoeff[4];			// pressure relief valve settings (psi), leakage coeffs

	real_T ymin[4] = {0, Ymin2, Ymin3, Ymin4};   	// minimum length of cylinder (in)
	real_T ymax[4] = {0, Ymax2, Ymax3, Ymax4};   	// maximum cylinder length (in) (this is not the stroke)
	real_T stroke[4] = {0, stroke2, stroke3, stroke4};  // cylinder stroke (in)
    real_T leakage[4];              // leakage (in^3/s) + means fluid leaks from head side to rod side
    real_T vol_head[4], vol_rod[4];               // volume of fluid on the head and rod sides (in^3)
    real_T pdot_head[4], pdot_rod[4];             // dP/dt on the head and rod sides (psi/s)
    real_T flow_head[4], flow_rod[4];             // flow into the on the head side and rod side (in^3) + means in
    real_T motorv, motor_flow;
    real_T noise = 0, power = 0, basepower; 
    real_T pumpvalveflow;//motorvalveflow

	for (k = 0; k < 4; k++) {
		//STATES[k] = 0;		//headside pressures
        phead[k] = STATES[k];
        prod[k] = STATES[k+4];
		//STATES[k+4] = 0;		//rodside pressures
		linevol_head[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+3));	
		linevol_rod[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+7));	
		head_length[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+11));
		end_rod[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+15));
		p_relief[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+19));
		leakcoeff[k] = (real_T) *mxGetPr(ssGetSFcnParam(S, k+23));
	}

    for (k = 0; k < 4; k++)
    {
        //swing motor
        if (k == 0)
        {
            leakage[k] = (phead[k] - prod[k])*leakcoeff[k];		// calculate leakage
            motorv = v[k]/(2*pi)*gear_ratio;                    // calculate velocity of the motor (rev/s)
            motor_flow = motorv * motor_disp_per_rev;			// calculate flow through the motor (in^3/s)
            
           if (abs(flow[k]) > abs(motor_flow + leakage[k]) )
            //if ( ( (motor_flow > 0) && (phead[k] < ptank) ) || ( (motor_flow < 0) && (prod[k] < ptank) ) )
            {
                pumpvalveflow = abs(flow[k] - (motor_flow + leakage[k]));
                motorvalveflow[0] = 0;
            }
            else
            {
                pumpvalveflow = 0;
                motorvalveflow[0] = abs(-flow[k] + (motor_flow + leakage[k]));
            }
            outwithit[0] = motor_flow;
            outwithit[1] = leakage[k];
            outwithit[2] = flow[k];
            outwithit[3] = v[k];

            flow_head[k] = flow[k] - motor_flow - leakage[k];   // + means in
            vol_head[k] = linevol_head[k];                      // volume on the "head" side - no real headside, but that's what we'll call it	
            pdot_head[k] = bulk_mod/vol_head[k]*flow_head[k];	// pressure change = beta/V*Q
            phead[k] = phead[k] + pdot_head[k]*Ts;              // integrate pressure

            flow_rod[k] = -flow[k] + motor_flow + leakage[k];   // + means in
            vol_rod[k] = linevol_rod[k];                        // volume on the "rod" side (in^3)
            pdot_rod[k] = bulk_mod/vol_rod[k]*flow_rod[k];		// dp/dt = beta/V*Q
            prod[k] = prod[k] + pdot_rod[k]*Ts;                 // integrate pressure
            if ((phead[k] > prod[k]) && (flow[k] > 0))
            {
                noise = floor(flow[k]);
            } 
            //motor_flow = motorvalveflow;
        }
        else
        {
            //3 cylinders
            leakage[k] = (phead[k] - prod[k])*leakcoeff[k];		// calculate leakage (in^3/s)

            flow_head[k] = flow[k] - area_head[k]*v[k] - leakage[k];  //+ means in
            vol_head[k] = area_head[k] * (y[k] - ymax[k] + stroke[k] + head_length[k]) + linevol_head[k];
            pdot_head[k] = bulk_mod/vol_head[k]*flow_head[k];
            phead[k] = phead[k] + pdot_head[k]*Ts;

            flow_rod[k] = -flow[k] + area_rod[k]*v[k] + leakage[k];   //+ means in
            vol_rod[k] = area_rod[k] * (ymin[k] - y[k] + stroke[k]) + linevol_rod[k];
            pdot_rod[k] = bulk_mod/vol_rod[k]*flow_rod[k];
            prod[k] = prod[k] + pdot_rod[k]*Ts;        
        }
        
        if (prod[k] > p_relief[k])	// if relief valve pressure is exceeded than the relief valve opens
        {
            prod[k] = p_relief[k];	// so the pressure = relief valve pressure set point
        }
        if (phead[k] > p_relief[k])	// if relief valve pressure is exceeded than the relief valve opens
        {
	      phead[k] = p_relief[k];	// so the pressure = relief valve pressure set point
        }
        if (prod[k] < ptank)	// if the pressure is below the tank pressure, then flow comes from the tank
        {
            prod[k] = ptank;	// so the pressure = pressure of the tank
        }
        if (phead[k] < ptank)	// if the pressure is below the tank pressure, then flow comes from the tank
        {
	      phead[k] = ptank;     // so the pressure = pressure of the tank
        }
        
        // save the states
        STATES[k] = phead[k];
        STATES[k+4] = prod[k];
    
       power = power + abs((phead[k] - prod[k])*flow[k]);
       //noise = floor(noise + flow[k]);
	}
    
    basepower = 50000;
    power = power - basepower;
    if (power < 0) {power = 0;}
    noise = floor(power/25);
    // base noise = 2^12 = 4096         2^16-1 = 65535
    VOLUME[0] = (uint16_T) min((noise + 4096),65535);
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
