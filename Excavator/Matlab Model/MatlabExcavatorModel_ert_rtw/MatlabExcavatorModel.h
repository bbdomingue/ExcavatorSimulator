/*
 * File: MatlabExcavatorModel.h
 *
 * Code generated for Simulink model 'MatlabExcavatorModel'.
 *
 * Model version                  : 1.203
 * Simulink Coder version         : 8.5 (R2013b) 08-Aug-2013
 * C/C++ source code generated on : Wed Feb 25 22:12:56 2015
 *
 * Target selection: ert.tlc
 * Embedded hardware selection: Generic->Unspecified (assume 32-bit Generic)
 * Code generation objectives: Unspecified
 * Validation result: Not run
 */

#ifndef RTW_HEADER_MatlabExcavatorModel_h_
#define RTW_HEADER_MatlabExcavatorModel_h_
#ifndef MatlabExcavatorModel_COMMON_INCLUDES_
# define MatlabExcavatorModel_COMMON_INCLUDES_
#include <stddef.h>
#include <math.h>
#include <string.h>
#include "rtwtypes.h"
#include "simstruc.h"
#include "fixedpoint.h"
#include "rtGetInf.h"
#include "rtGetNaN.h"
#include "rt_nonfinite.h"
#endif                                 /* MatlabExcavatorModel_COMMON_INCLUDES_ */

#include "MatlabExcavatorModel_types.h"

/* Macros for accessing real-time model data structure */
#ifndef rtmGetFinalTime
# define rtmGetFinalTime(rtm)          ((rtm)->Timing.tFinal)
#endif

#ifndef rtmGetSampleHitArray
# define rtmGetSampleHitArray(rtm)     ((rtm)->Timing.sampleHitArray)
#endif

#ifndef rtmGetStepSize
# define rtmGetStepSize(rtm)           ((rtm)->Timing.stepSize)
#endif

#ifndef rtmGet_TimeOfLastOutput
# define rtmGet_TimeOfLastOutput(rtm)  ((rtm)->Timing.timeOfLastOutput)
#endif

#ifndef rtmGetErrorStatus
# define rtmGetErrorStatus(rtm)        ((rtm)->errorStatus)
#endif

#ifndef rtmSetErrorStatus
# define rtmSetErrorStatus(rtm, val)   ((rtm)->errorStatus = (val))
#endif

#ifndef rtmGetStopRequested
# define rtmGetStopRequested(rtm)      ((rtm)->Timing.stopRequestedFlag)
#endif

#ifndef rtmSetStopRequested
# define rtmSetStopRequested(rtm, val) ((rtm)->Timing.stopRequestedFlag = (val))
#endif

#ifndef rtmGetStopRequestedPtr
# define rtmGetStopRequestedPtr(rtm)   (&((rtm)->Timing.stopRequestedFlag))
#endif

#ifndef rtmGetT
# define rtmGetT(rtm)                  (rtmGetTPtr((rtm))[0])
#endif

#ifndef rtmGetTFinal
# define rtmGetTFinal(rtm)             ((rtm)->Timing.tFinal)
#endif

#ifndef rtmGetTStart
# define rtmGetTStart(rtm)             ((rtm)->Timing.tStart)
#endif

#ifndef rtmGetTimeOfLastOutput
# define rtmGetTimeOfLastOutput(rtm)   ((rtm)->Timing.timeOfLastOutput)
#endif

/* Block signals (auto storage) */
typedef struct {
  real_T q[4];                         /* '<S4>/Integrator (qd to q)' */
  real_T qdot[4];                      /* '<S4>/Integrator (qdd to qd)' */
  real_T UnitDelay1[4];                /* '<Root>/Unit Delay1' */
  real_T limiter_o3[4];                /* '<S4>/limiter' */
  real_T TmpSignalConversionAtSFunctionI[8];
  real_T SFunction_o1[8];              /* '<S6>/S-Function' */
  real_T SFunction_o2[4];              /* '<S6>/S-Function' */
  real_T SFunction_o1_i[4];            /* '<S3>/S-Function' */
  real_T SFunction_o2_h[4];            /* '<S3>/S-Function' */
  real_T SFunction_o4;                 /* '<S3>/S-Function' */
  real_T SFunction_o5[4];              /* '<S3>/S-Function' */
  real_T SFunction;                    /* '<S5>/S-Function' */
  real_T SFunction1;                   /* '<S5>/S-Function1' */
  real_T SFunction2;                   /* '<S5>/S-Function2' */
  real_T SFunction3;                   /* '<S5>/S-Function3' */
} B_MatlabExcavatorModel_T;

/* Block states (auto storage) for system '<Root>' */
typedef struct {
  real_T Integratorqdtoq_DSTATE[4];    /* '<S4>/Integrator (qd to q)' */
  real_T IntegerDelay_DSTATE[4];       /* '<S4>/Integer Delay' */
  real_T Integratorqddtoqd_DSTATE[4];  /* '<S4>/Integrator (qdd to qd)' */
  real_T UnitDelay1_DSTATE[4];         /* '<Root>/Unit Delay1' */
  real_T limiter_DSTATE[4];            /* '<S4>/limiter' */
  real_T IntegerDelay3_DSTATE;         /* '<S7>/Integer Delay3' */
  real_T IntegerDelay1_DSTATE[4];      /* '<S7>/Integer Delay1' */
  real_T IntegerDelay2_DSTATE[4];      /* '<S7>/Integer Delay2' */
  real_T SFunction_DSTATE[8];          /* '<S3>/S-Function' */
  real_T SFunction3_DSTATE;            /* '<S5>/S-Function3' */
  int8_T Integratorqddtoqd_PrevResetStat[4];/* '<S4>/Integrator (qdd to qd)' */
} DW_MatlabExcavatorModel_T;

/* External inputs (root inport signals with auto storage) */
typedef struct {
  real_T Flow1;                        /* '<Root>/Flow1' */
  real_T Flow2;                        /* '<Root>/Flow2' */
  real_T Flow3;                        /* '<Root>/Flow3' */
  real_T Flow4;                        /* '<Root>/Flow4' */
  real_T Soil_Dig_Force[3];            /* '<Root>/Soil_Dig_Force' */
  real_T Soil_Wrist_Force[3];          /* '<Root>/Soil_Wrist_Force' */
  real_T Soil_Dig_Moment[3];           /* '<Root>/Soil_Dig_Moment' */
  real_T Soil_Bucket_Mass;             /* '<Root>/Soil_Bucket_Mass' */
} ExtU_MatlabExcavatorModel_T;

/* External outputs (root outports fed by signals with auto storage) */
typedef struct {
  real_T Time;                         /* '<Root>/Time' */
  real_T Q[4];                         /* '<Root>/Q ' */
  real_T Qd[4];                        /* '<Root>/Qd ' */
  real_T FuelTotal;                    /* '<Root>/FuelTotal' */
  real_T FuelInstant;                  /* '<Root>/FuelInstant' */
  uint16_T Volume;                     /* '<Root>/Volume ' */
  real_T BreakOn[4];                   /* '<Root>/BreakOn' */
  real_T Flow[4];                      /* '<Root>/Flow' */
  real_T CylinderPosition[4];          /* '<Root>/CylinderPosition' */
  real_T CylinderVelocity[4];          /* '<Root>/CylinderVelocity' */
} ExtY_MatlabExcavatorModel_T;

/* Parameters (auto storage) */
struct P_MatlabExcavatorModel_T_ {
  real_T Integratorqdtoq_gainval;      /* Computed Parameter: Integratorqdtoq_gainval
                                        * Referenced by: '<S4>/Integrator (qd to q)'
                                        */
  real_T Integratorqdtoq_IC[4];        /* Expression: qinit
                                        * Referenced by: '<S4>/Integrator (qd to q)'
                                        */
  real_T IntegerDelay_InitialCondition;/* Expression: 0.0
                                        * Referenced by: '<S4>/Integer Delay'
                                        */
  real_T Transform1_Gain;              /* Expression: 1
                                        * Referenced by: '<S4>/Transform1'
                                        */
  real_T Integratorqddtoqd_gainval;    /* Computed Parameter: Integratorqddtoqd_gainval
                                        * Referenced by: '<S4>/Integrator (qdd to qd)'
                                        */
  real_T Integratorqddtoqd_IC[4];      /* Expression: [0 0 0 0]
                                        * Referenced by: '<S4>/Integrator (qdd to qd)'
                                        */
  real_T Integratorqddtoqd_UpperSat[4];/* Expression: [.75 inf inf inf]
                                        * Referenced by: '<S4>/Integrator (qdd to qd)'
                                        */
  real_T Integratorqddtoqd_LowerSat[4];/* Expression: [.75 inf inf inf]*(-1)
                                        * Referenced by: '<S4>/Integrator (qdd to qd)'
                                        */
  real_T Constant_Value[4];            /* Expression: zeros(4,1)
                                        * Referenced by: '<S4>/Constant'
                                        */
  real_T UnitDelay1_InitialCondition;  /* Expression: 0
                                        * Referenced by: '<Root>/Unit Delay1'
                                        */
  real_T limiter_P1_Size[2];           /* Computed Parameter: limiter_P1_Size
                                        * Referenced by: '<S4>/limiter'
                                        */
  real_T limiter_P1;                   /* Expression: Ts
                                        * Referenced by: '<S4>/limiter'
                                        */
  real_T inrpm1_Value;                 /* Expression: enginespeed
                                        * Referenced by: '<S7>/in rpm1'
                                        */
  real_T inNm1_Value[51];              /* Expression: torqscale
                                        * Referenced by: '<S7>/in Nm1'
                                        */
  real_T inrpms1_Value[57];            /* Expression: speedscale
                                        * Referenced by: '<S7>/in rpms1'
                                        */
  real_T fuelratematrixkghr_Value[2907];/* Expression: fuelmatrix
                                         * Referenced by: '<S7>/fuel rate  matrix (kg//hr)'
                                         */
  real_T insec_Value;                  /* Expression: Ts
                                        * Referenced by: '<S7>/in sec'
                                        */
  real_T IntegerDelay3_InitialCondition;/* Expression: 0
                                         * Referenced by: '<S7>/Integer Delay3'
                                         */
  real_T IntegerDelay1_InitialCondition[4];/* Expression: pheadinit
                                            * Referenced by: '<S7>/Integer Delay1'
                                            */
  real_T IntegerDelay2_InitialCondition[4];/* Expression: prodinit
                                            * Referenced by: '<S7>/Integer Delay2'
                                            */
  real_T MaximumPumpFlowin3_Value[4];  /* Expression: maxpflow
                                        * Referenced by: '<S7>/Maximum Pump Flow (in^3)'
                                        */
  real_T MaxDispperRevolution_Value;   /* Expression: maxflow_per_rev
                                        * Referenced by: '<S7>/Max Disp per Revolution'
                                        */
  real_T maxtorqatsetenginespeed_Value[57];/* Expression: maxincol
                                            * Referenced by: '<S7>/max torq at set engine speed'
                                            */
  real_T Nmtoinlbconversion_Value;     /* Expression: N_m_2_lb_in
                                        * Referenced by: '<S7>/Nm to inlb conversion'
                                        */
  real_T SFunction_P1_Size[2];         /* Computed Parameter: SFunction_P1_Size
                                        * Referenced by: '<S6>/S-Function'
                                        */
  real_T SFunction_P1;                 /* Expression: Ts
                                        * Referenced by: '<S6>/S-Function'
                                        */
  real_T SFunction_P1_Size_e[2];       /* Computed Parameter: SFunction_P1_Size_e
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P1_h;               /* Expression: Ts
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P2_Size[2];         /* Computed Parameter: SFunction_P2_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P2;                 /* Expression: ptank
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P3_Size[2];         /* Computed Parameter: SFunction_P3_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P3;                 /* Expression: bulk_mod
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P4_Size[2];         /* Computed Parameter: SFunction_P4_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P4;                 /* Expression: lvolh1
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P5_Size[2];         /* Computed Parameter: SFunction_P5_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P5;                 /* Expression: lvolh2
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P6_Size[2];         /* Computed Parameter: SFunction_P6_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P6;                 /* Expression: lvolh3
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P7_Size[2];         /* Computed Parameter: SFunction_P7_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P7;                 /* Expression: lvolh4
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P8_Size[2];         /* Computed Parameter: SFunction_P8_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P8;                 /* Expression: lvolr1
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P9_Size[2];         /* Computed Parameter: SFunction_P9_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P9;                 /* Expression: lvolr2
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P10_Size[2];        /* Computed Parameter: SFunction_P10_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P10;                /* Expression: lvolr3
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P11_Size[2];        /* Computed Parameter: SFunction_P11_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P11;                /* Expression: lvolr4
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P12_Size[2];        /* Computed Parameter: SFunction_P12_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P12;                /* Expression: head1
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P13_Size[2];        /* Computed Parameter: SFunction_P13_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P13;                /* Expression: head2
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P14_Size[2];        /* Computed Parameter: SFunction_P14_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P14;                /* Expression: head3
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P15_Size[2];        /* Computed Parameter: SFunction_P15_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P15;                /* Expression: head4
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P16_Size[2];        /* Computed Parameter: SFunction_P16_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P16;                /* Expression: rod1
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P17_Size[2];        /* Computed Parameter: SFunction_P17_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P17;                /* Expression: rod2
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P18_Size[2];        /* Computed Parameter: SFunction_P18_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P18;                /* Expression: rod3
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P19_Size[2];        /* Computed Parameter: SFunction_P19_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P19;                /* Expression: rod4
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P20_Size[2];        /* Computed Parameter: SFunction_P20_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P20;                /* Expression: prelief1
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P21_Size[2];        /* Computed Parameter: SFunction_P21_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P21;                /* Expression: prelief2
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P22_Size[2];        /* Computed Parameter: SFunction_P22_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P22;                /* Expression: prelief3
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P23_Size[2];        /* Computed Parameter: SFunction_P23_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P23;                /* Expression: prelief4
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P24_Size[2];        /* Computed Parameter: SFunction_P24_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P24;                /* Expression: leak1
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P25_Size[2];        /* Computed Parameter: SFunction_P25_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P25;                /* Expression: leak2
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P26_Size[2];        /* Computed Parameter: SFunction_P26_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P26;                /* Expression: leak3
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P27_Size[2];        /* Computed Parameter: SFunction_P27_Size
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P27;                /* Expression: leak4
                                        * Referenced by: '<S3>/S-Function'
                                        */
  real_T SFunction_P1_Size_c[2];       /* Computed Parameter: SFunction_P1_Size_c
                                        * Referenced by: '<S5>/S-Function'
                                        */
  real_T SFunction_P1_f;               /* Expression: Ts
                                        * Referenced by: '<S5>/S-Function'
                                        */
  real_T SFunction_P2_Size_n[2];       /* Computed Parameter: SFunction_P2_Size_n
                                        * Referenced by: '<S5>/S-Function'
                                        */
  real_T SFunction_P2_i;               /* Expression: fv_swing
                                        * Referenced by: '<S5>/S-Function'
                                        */
  real_T SFunction_P3_Size_j[2];       /* Computed Parameter: SFunction_P3_Size_j
                                        * Referenced by: '<S5>/S-Function'
                                        */
  real_T SFunction_P3_k;               /* Expression: fc_swing
                                        * Referenced by: '<S5>/S-Function'
                                        */
  real_T SFunction_P4_Size_g[2];       /* Computed Parameter: SFunction_P4_Size_g
                                        * Referenced by: '<S5>/S-Function'
                                        */
  real_T SFunction_P4_c;               /* Expression: fbrake
                                        * Referenced by: '<S5>/S-Function'
                                        */
  real_T SFunction1_P1_Size[2];        /* Computed Parameter: SFunction1_P1_Size
                                        * Referenced by: '<S5>/S-Function1'
                                        */
  real_T SFunction1_P1;                /* Expression: Ts
                                        * Referenced by: '<S5>/S-Function1'
                                        */
  real_T SFunction1_P2_Size[2];        /* Computed Parameter: SFunction1_P2_Size
                                        * Referenced by: '<S5>/S-Function1'
                                        */
  real_T SFunction1_P2;                /* Expression: fv_boom
                                        * Referenced by: '<S5>/S-Function1'
                                        */
  real_T SFunction1_P3_Size[2];        /* Computed Parameter: SFunction1_P3_Size
                                        * Referenced by: '<S5>/S-Function1'
                                        */
  real_T SFunction1_P3;                /* Expression: fc_boom
                                        * Referenced by: '<S5>/S-Function1'
                                        */
  real_T SFunction2_P1_Size[2];        /* Computed Parameter: SFunction2_P1_Size
                                        * Referenced by: '<S5>/S-Function2'
                                        */
  real_T SFunction2_P1;                /* Expression: Ts
                                        * Referenced by: '<S5>/S-Function2'
                                        */
  real_T SFunction2_P2_Size[2];        /* Computed Parameter: SFunction2_P2_Size
                                        * Referenced by: '<S5>/S-Function2'
                                        */
  real_T SFunction2_P2;                /* Expression: fv_arm
                                        * Referenced by: '<S5>/S-Function2'
                                        */
  real_T SFunction2_P3_Size[2];        /* Computed Parameter: SFunction2_P3_Size
                                        * Referenced by: '<S5>/S-Function2'
                                        */
  real_T SFunction2_P3;                /* Expression: fc_arm
                                        * Referenced by: '<S5>/S-Function2'
                                        */
  real_T SFunction3_P1_Size[2];        /* Computed Parameter: SFunction3_P1_Size
                                        * Referenced by: '<S5>/S-Function3'
                                        */
  real_T SFunction3_P1;                /* Expression: Ts
                                        * Referenced by: '<S5>/S-Function3'
                                        */
  real_T SFunction3_P2_Size[2];        /* Computed Parameter: SFunction3_P2_Size
                                        * Referenced by: '<S5>/S-Function3'
                                        */
  real_T SFunction3_P2;                /* Expression: fv_bucket
                                        * Referenced by: '<S5>/S-Function3'
                                        */
  real_T SFunction3_P3_Size[2];        /* Computed Parameter: SFunction3_P3_Size
                                        * Referenced by: '<S5>/S-Function3'
                                        */
  real_T SFunction3_P3;                /* Expression: fc_bucket
                                        * Referenced by: '<S5>/S-Function3'
                                        */
  real_T Transform3_Gain;              /* Expression: 1/Nm2ftlb
                                        * Referenced by: '<Root>/Transform3'
                                        */
  real_T Transform2_Gain;              /* Expression: 1
                                        * Referenced by: '<Root>/Transform2'
                                        */
  real_T Transform10_Gain;             /* Expression: 30
                                        * Referenced by: '<Root>/Transform10'
                                        */
  real_T MaximumPumpTorque_Value;      /* Expression: torqmax
                                        * Referenced by: '<S7>/Maximum Pump Torque'
                                        */
  real_T inrpm_Value;                  /* Expression: enginespeed
                                        * Referenced by: '<S7>/in rpm'
                                        */
  real_T inNm_Value[51];               /* Expression: torqscale
                                        * Referenced by: '<S7>/in Nm'
                                        */
  real_T inrpms_Value[57];             /* Expression: speedscale
                                        * Referenced by: '<S7>/in rpms'
                                        */
  real_T Transform4_Gain;              /* Expression: 10
                                        * Referenced by: '<Root>/Transform4'
                                        */
  uint32_T IntegerDelay_DelayLength;   /* Computed Parameter: IntegerDelay_DelayLength
                                        * Referenced by: '<S4>/Integer Delay'
                                        */
  uint32_T IntegerDelay3_DelayLength;  /* Computed Parameter: IntegerDelay3_DelayLength
                                        * Referenced by: '<S7>/Integer Delay3'
                                        */
  uint32_T IntegerDelay1_DelayLength;  /* Computed Parameter: IntegerDelay1_DelayLength
                                        * Referenced by: '<S7>/Integer Delay1'
                                        */
  uint32_T IntegerDelay2_DelayLength;  /* Computed Parameter: IntegerDelay2_DelayLength
                                        * Referenced by: '<S7>/Integer Delay2'
                                        */
  uint8_T TorquelimitSwitch_CurrentSettin;/* Computed Parameter: TorquelimitSwitch_CurrentSettin
                                           * Referenced by: '<S2>/Torque limit Switch'
                                           */
};

/* Real-time Model Data Structure */
struct tag_RTM_MatlabExcavatorModel_T {
  struct SimStruct_tag * *childSfunctions;
  const char_T * volatile errorStatus;
  SS_SimMode simMode;
  RTWSolverInfo solverInfo;
  RTWSolverInfo *solverInfoPtr;
  void *sfcnInfo;

  /*
   * NonInlinedSFcns:
   * The following substructure contains information regarding
   * non-inlined s-functions used in the model.
   */
  struct {
    RTWSfcnInfo sfcnInfo;
    time_T *taskTimePtrs[2];
    SimStruct childSFunctions[7];
    SimStruct *childSFunctionPtrs[7];
    struct _ssBlkInfo2 blkInfo2[7];
    struct _ssSFcnModelMethods2 methods2[7];
    struct _ssSFcnModelMethods3 methods3[7];
    struct _ssStatesInfo2 statesInfo2[7];
    struct {
      time_T sfcnPeriod[1];
      time_T sfcnOffset[1];
      int_T sfcnTsMap[1];
      struct _ssPortInputs inputPortInfo[4];
      struct _ssPortOutputs outputPortInfo[3];
      uint_T attribs[1];
      mxArray *params[1];
      struct _ssDWorkRecord dWork[1];
      struct _ssDWorkAuxRecord dWorkAux[1];
    } Sfcn0;

    struct {
      time_T sfcnPeriod[1];
      time_T sfcnOffset[1];
      int_T sfcnTsMap[1];
      struct _ssPortInputs inputPortInfo[1];
      struct _ssPortOutputs outputPortInfo[2];
      uint_T attribs[1];
      mxArray *params[1];
    } Sfcn1;

    struct {
      time_T sfcnPeriod[1];
      time_T sfcnOffset[1];
      int_T sfcnTsMap[1];
      struct _ssPortInputs inputPortInfo[3];
      struct _ssPortOutputs outputPortInfo[5];
      uint_T attribs[27];
      mxArray *params[27];
      struct _ssDWorkRecord dWork[1];
      struct _ssDWorkAuxRecord dWorkAux[1];
    } Sfcn2;

    struct {
      time_T sfcnPeriod[1];
      time_T sfcnOffset[1];
      int_T sfcnTsMap[1];
      struct _ssPortInputs inputPortInfo[4];
      struct _ssPortOutputs outputPortInfo[1];
      uint_T attribs[4];
      mxArray *params[4];
    } Sfcn3;

    struct {
      time_T sfcnPeriod[1];
      time_T sfcnOffset[1];
      int_T sfcnTsMap[1];
      struct _ssPortInputs inputPortInfo[4];
      struct _ssPortOutputs outputPortInfo[1];
      uint_T attribs[3];
      mxArray *params[3];
    } Sfcn4;

    struct {
      time_T sfcnPeriod[1];
      time_T sfcnOffset[1];
      int_T sfcnTsMap[1];
      struct _ssPortInputs inputPortInfo[4];
      struct _ssPortOutputs outputPortInfo[1];
      uint_T attribs[3];
      mxArray *params[3];
    } Sfcn5;

    struct {
      time_T sfcnPeriod[1];
      time_T sfcnOffset[1];
      int_T sfcnTsMap[1];
      struct _ssPortInputs inputPortInfo[4];
      struct _ssPortOutputs outputPortInfo[1];
      uint_T attribs[3];
      mxArray *params[3];
      struct _ssDWorkRecord dWork[1];
      struct _ssDWorkAuxRecord dWorkAux[1];
    } Sfcn6;
  } NonInlinedSFcns;

  /*
   * ModelData:
   * The following substructure contains information regarding
   * the data used in the model.
   */
  struct {
    boolean_T zCCacheNeedsReset;
    boolean_T derivCacheNeedsReset;
    boolean_T blkStateChange;
  } ModelData;

  /*
   * Sizes:
   * The following substructure contains sizes information
   * for many of the model attributes such as inputs, outputs,
   * dwork, sample times, etc.
   */
  struct {
    uint32_T options;
    int_T numContStates;
    int_T numU;
    int_T numY;
    int_T numSampTimes;
    int_T numBlocks;
    int_T numBlockIO;
    int_T numBlockPrms;
    int_T numDwork;
    int_T numSFcnPrms;
    int_T numSFcns;
    int_T numIports;
    int_T numOports;
    int_T numNonSampZCs;
    int_T sysDirFeedThru;
    int_T rtwGenSfcn;
  } Sizes;

  /*
   * Timing:
   * The following substructure contains information regarding
   * the timing information for the model.
   */
  struct {
    time_T stepSize;
    uint32_T clockTick0;
    uint32_T clockTickH0;
    time_T stepSize0;
    uint32_T clockTick1;
    uint32_T clockTickH1;
    time_T stepSize1;
    time_T tStart;
    time_T tFinal;
    time_T timeOfLastOutput;
    SimTimeStep simTimeStep;
    boolean_T stopRequestedFlag;
    time_T *sampleTimes;
    time_T *offsetTimes;
    int_T *sampleTimeTaskIDPtr;
    int_T *sampleHits;
    int_T *perTaskSampleHits;
    time_T *t;
    time_T sampleTimesArray[2];
    time_T offsetTimesArray[2];
    int_T sampleTimeTaskIDArray[2];
    int_T sampleHitArray[2];
    int_T perTaskSampleHitsArray[4];
    time_T tArray[2];
  } Timing;
};

/* Block parameters (auto storage) */
#ifdef __cplusplus

extern "C" {

#endif

  extern P_MatlabExcavatorModel_T MatlabExcavatorModel_P;

#ifdef __cplusplus

}
#endif

/* Block signals (auto storage) */
extern B_MatlabExcavatorModel_T MatlabExcavatorModel_B;

/* Block states (auto storage) */
extern DW_MatlabExcavatorModel_T MatlabExcavatorModel_DW;

#ifdef __cplusplus

extern "C" {

#endif

  /* External inputs (root inport signals with auto storage) */
  extern ExtU_MatlabExcavatorModel_T MatlabExcavatorModel_U;

  /* External outputs (root outports fed by signals with auto storage) */
  extern ExtY_MatlabExcavatorModel_T MatlabExcavatorModel_Y;

#ifdef __cplusplus

}
#endif

#ifdef __cplusplus

extern "C" {

#endif

  /* Model entry point functions */
  extern void MatlabExcavatorModel_initialize(void);
  extern void MatlabExcavatorModel_step(void);
  extern void MatlabExcavatorModel_terminate(void);

#ifdef __cplusplus

}
#endif

/* Real-time Model object */
#ifdef __cplusplus

extern "C" {

#endif

  extern RT_MODEL_MatlabExcavatorModel_T *const MatlabExcavatorModel_M;

#ifdef __cplusplus

}
#endif

/*-
 * The generated code includes comments that allow you to trace directly
 * back to the appropriate location in the model.  The basic format
 * is <system>/block_name, where system is the system number (uniquely
 * assigned by Simulink) and block_name is the name of the block.
 *
 * Use the MATLAB hilite_system command to trace the generated code back
 * to the model.  For example,
 *
 * hilite_system('<S3>')    - opens system 3
 * hilite_system('<S3>/Kp') - opens and selects block Kp which resides in S3
 *
 * Here is the system hierarchy for this model
 *
 * '<Root>' : 'MatlabExcavatorModel'
 * '<S1>'   : 'MatlabExcavatorModel/Arm Dynamics'
 * '<S2>'   : 'MatlabExcavatorModel/Command to Flow'
 * '<S3>'   : 'MatlabExcavatorModel/Cylinder Model'
 * '<S4>'   : 'MatlabExcavatorModel/Integration'
 * '<S5>'   : 'MatlabExcavatorModel/Pressure to Torque Calculation'
 * '<S6>'   : 'MatlabExcavatorModel/j2c'
 * '<S7>'   : 'MatlabExcavatorModel/Command to Flow/Torque limiter'
 * '<S8>'   : 'MatlabExcavatorModel/Command to Flow/Torque limiter/Limit torque at given engine speed'
 * '<S9>'   : 'MatlabExcavatorModel/Command to Flow/Torque limiter/swashflow'
 */
#endif                                 /* RTW_HEADER_MatlabExcavatorModel_h_ */

/*
 * File trailer for generated code.
 *
 * [EOF]
 */
