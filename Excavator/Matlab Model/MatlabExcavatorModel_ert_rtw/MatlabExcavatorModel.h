/*
 * File: MatlabExcavatorModel.h
 *
 * Code generated for Simulink model 'MatlabExcavatorModel'.
 *
 * Model version                  : 1.227
 * Simulink Coder version         : 8.5 (R2013b) 08-Aug-2013
 * C/C++ source code generated on : Wed Feb 25 23:07:32 2015
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
  real_T TmpSignalConversionAtSFunctionI[8];
  real_T SFunction_o1[8];              /* '<S4>/S-Function' */
  real_T SFunction_o2[4];              /* '<S4>/S-Function' */
  real_T SFunction_o1_i[4];            /* '<S1>/S-Function' */
  real_T SFunction_o2_h[4];            /* '<S1>/S-Function' */
  real_T SFunction_o4;                 /* '<S1>/S-Function' */
  real_T SFunction_o5[4];              /* '<S1>/S-Function' */
  real_T SFunction;                    /* '<S2>/S-Function' */
  real_T SFunction1;                   /* '<S2>/S-Function1' */
  real_T SFunction2;                   /* '<S2>/S-Function2' */
  real_T SFunction3;                   /* '<S2>/S-Function3' */
} B_MatlabExcavatorModel_T;

/* Block states (auto storage) for system '<Root>' */
typedef struct {
  real_T IntegerDelay3_DSTATE;         /* '<S3>/Integer Delay3' */
  real_T IntegerDelay1_DSTATE[4];      /* '<S3>/Integer Delay1' */
  real_T IntegerDelay2_DSTATE[4];      /* '<S3>/Integer Delay2' */
  real_T SFunction_DSTATE[8];          /* '<S1>/S-Function' */
  real_T SFunction3_DSTATE;            /* '<S2>/S-Function3' */
} DW_MatlabExcavatorModel_T;

/* Constant parameters (auto storage) */
typedef struct {
  /* Pooled Parameter (Expression: torqscale)
   * Referenced by:
   *   '<S3>/in Nm'
   *   '<S3>/in Nm1'
   */
  real_T pooled2[51];

  /* Pooled Parameter (Expression: speedscale)
   * Referenced by:
   *   '<S3>/in rpms'
   *   '<S3>/in rpms1'
   */
  real_T pooled3[57];

  /* Expression: maxincol
   * Referenced by: '<S3>/max torq at set engine speed'
   */
  real_T maxtorqatsetenginesp[57];

  /* Pooled Parameter (Expression: )
   * Referenced by:
   *   '<S1>/S-Function'
   *   '<S2>/S-Function'
   *   '<S2>/S-Function1'
   *   '<S2>/S-Function2'
   *   '<S2>/S-Function3'
   *   '<S4>/S-Function'
   */
  real_T pooled5[2];

  /* Pooled Parameter (Expression: Ts)
   * Referenced by:
   *   '<S1>/S-Function'
   *   '<S2>/S-Function'
   *   '<S2>/S-Function1'
   *   '<S2>/S-Function2'
   *   '<S2>/S-Function3'
   *   '<S4>/S-Function'
   */
  real_T pooled6;

  /* Computed Parameter: SFunction_P2_Size
   * Referenced by: '<S1>/S-Function'
   */
  real_T SFunction_P2_Size[2];

  /* Expression: ptank
   * Referenced by: '<S1>/S-Function'
   */
  real_T SFunction_P2;

  /* Computed Parameter: SFunction_P3_Size
   * Referenced by: '<S1>/S-Function'
   */
  real_T SFunction_P3_Size[2];

  /* Expression: bulk_mod
   * Referenced by: '<S1>/S-Function'
   */
  real_T SFunction_P3;

  /* Pooled Parameter (Expression: )
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled7[2];

  /* Pooled Parameter (Mixed Expressions)
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled8;

  /* Pooled Parameter (Expression: )
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled9[2];

  /* Pooled Parameter (Mixed Expressions)
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled10;

  /* Pooled Parameter (Expression: )
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled11[2];

  /* Pooled Parameter (Mixed Expressions)
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled12;

  /* Computed Parameter: SFunction_P11_Size
   * Referenced by: '<S1>/S-Function'
   */
  real_T SFunction_P11_Size[2];

  /* Expression: lvolr4
   * Referenced by: '<S1>/S-Function'
   */
  real_T SFunction_P11;

  /* Pooled Parameter (Expression: )
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled13[2];

  /* Pooled Parameter (Mixed Expressions)
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled14;

  /* Pooled Parameter (Expression: )
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled15[2];

  /* Pooled Parameter (Mixed Expressions)
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled16;

  /* Pooled Parameter (Expression: )
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled17[2];

  /* Pooled Parameter (Mixed Expressions)
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled18;

  /* Computed Parameter: SFunction_P24_Size
   * Referenced by: '<S1>/S-Function'
   */
  real_T SFunction_P24_Size[2];

  /* Expression: leak1
   * Referenced by: '<S1>/S-Function'
   */
  real_T SFunction_P24;

  /* Pooled Parameter (Expression: )
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled19[2];

  /* Pooled Parameter (Mixed Expressions)
   * Referenced by: '<S1>/S-Function'
   */
  real_T pooled20;

  /* Computed Parameter: SFunction_P2_Size_n
   * Referenced by: '<S2>/S-Function'
   */
  real_T SFunction_P2_Size_n[2];

  /* Expression: fv_swing
   * Referenced by: '<S2>/S-Function'
   */
  real_T SFunction_P2_i;

  /* Computed Parameter: SFunction_P3_Size_j
   * Referenced by: '<S2>/S-Function'
   */
  real_T SFunction_P3_Size_j[2];

  /* Expression: fc_swing
   * Referenced by: '<S2>/S-Function'
   */
  real_T SFunction_P3_k;

  /* Computed Parameter: SFunction_P4_Size
   * Referenced by: '<S2>/S-Function'
   */
  real_T SFunction_P4_Size[2];

  /* Expression: fbrake
   * Referenced by: '<S2>/S-Function'
   */
  real_T SFunction_P4;

  /* Computed Parameter: SFunction1_P2_Size
   * Referenced by: '<S2>/S-Function1'
   */
  real_T SFunction1_P2_Size[2];

  /* Expression: fv_boom
   * Referenced by: '<S2>/S-Function1'
   */
  real_T SFunction1_P2;

  /* Computed Parameter: SFunction1_P3_Size
   * Referenced by: '<S2>/S-Function1'
   */
  real_T SFunction1_P3_Size[2];

  /* Expression: fc_boom
   * Referenced by: '<S2>/S-Function1'
   */
  real_T SFunction1_P3;

  /* Computed Parameter: SFunction2_P2_Size
   * Referenced by: '<S2>/S-Function2'
   */
  real_T SFunction2_P2_Size[2];

  /* Expression: fv_arm
   * Referenced by: '<S2>/S-Function2'
   */
  real_T SFunction2_P2;

  /* Computed Parameter: SFunction2_P3_Size
   * Referenced by: '<S2>/S-Function2'
   */
  real_T SFunction2_P3_Size[2];

  /* Expression: fc_arm
   * Referenced by: '<S2>/S-Function2'
   */
  real_T SFunction2_P3;

  /* Computed Parameter: SFunction3_P2_Size
   * Referenced by: '<S2>/S-Function3'
   */
  real_T SFunction3_P2_Size[2];

  /* Expression: fv_bucket
   * Referenced by: '<S2>/S-Function3'
   */
  real_T SFunction3_P2;

  /* Computed Parameter: SFunction3_P3_Size
   * Referenced by: '<S2>/S-Function3'
   */
  real_T SFunction3_P3_Size[2];

  /* Expression: fc_bucket
   * Referenced by: '<S2>/S-Function3'
   */
  real_T SFunction3_P3;
} ConstP_MatlabExcavatorModel_T;

/* Constant parameters with dynamic initialization (auto storage) */
typedef struct {
  /* Expression: fuelmatrix
   * Referenced by: '<S3>/fuel rate  matrix (kg//hr)'
   */
  real_T fuelratematrixkghr[2907];
} ConstInitP_MatlabExcavatorModel_T;

/* External inputs (root inport signals with auto storage) */
typedef struct {
  real_T Flow[4];                      /* '<Root>/Flow ' */
  real_T Qd[4];                        /* '<Root>/Qd' */
  real_T Q[4];                         /* '<Root>/Q' */
} ExtU_MatlabExcavatorModel_T;

/* External outputs (root outports fed by signals with auto storage) */
typedef struct {
  real_T Time;                         /* '<Root>/Time' */
  real_T FuelTotal;                    /* '<Root>/FuelTotal' */
  real_T FuelInstant;                  /* '<Root>/FuelInstant' */
  uint16_T Volume;                     /* '<Root>/Volume ' */
  real_T FlowSent[4];                  /* '<Root>/FlowSent' */
  real_T CylinderPosition[4];          /* '<Root>/CylinderPosition' */
  real_T CylinderVelocity[4];          /* '<Root>/CylinderVelocity' */
  real_T JointTorques[4];              /* '<Root>/JointTorques' */
} ExtY_MatlabExcavatorModel_T;

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
    SimStruct childSFunctions[6];
    SimStruct *childSFunctionPtrs[6];
    struct _ssBlkInfo2 blkInfo2[6];
    struct _ssSFcnModelMethods2 methods2[6];
    struct _ssSFcnModelMethods3 methods3[6];
    struct _ssStatesInfo2 statesInfo2[6];
    struct {
      time_T sfcnPeriod[1];
      time_T sfcnOffset[1];
      int_T sfcnTsMap[1];
      struct _ssPortInputs inputPortInfo[1];
      struct _ssPortOutputs outputPortInfo[2];
      uint_T attribs[1];
      mxArray *params[1];
    } Sfcn0;

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
    } Sfcn1;

    struct {
      time_T sfcnPeriod[1];
      time_T sfcnOffset[1];
      int_T sfcnTsMap[1];
      struct _ssPortInputs inputPortInfo[4];
      struct _ssPortOutputs outputPortInfo[1];
      uint_T attribs[4];
      mxArray *params[4];
    } Sfcn2;

    struct {
      time_T sfcnPeriod[1];
      time_T sfcnOffset[1];
      int_T sfcnTsMap[1];
      struct _ssPortInputs inputPortInfo[4];
      struct _ssPortOutputs outputPortInfo[1];
      uint_T attribs[3];
      mxArray *params[3];
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
      struct _ssDWorkRecord dWork[1];
      struct _ssDWorkAuxRecord dWorkAux[1];
    } Sfcn5;
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

/* Constant parameters (auto storage) */
extern const ConstP_MatlabExcavatorModel_T MatlabExcavatorModel_ConstP;

/* Constant parameters with dynamic initialization (auto storage) */
extern ConstInitP_MatlabExcavatorModel_T MatlabExcavatorModel_ConstInitP;/* constant parameters */

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
 * '<S1>'   : 'MatlabExcavatorModel/Cylinder Model'
 * '<S2>'   : 'MatlabExcavatorModel/Pressure to Torque Calculation'
 * '<S3>'   : 'MatlabExcavatorModel/Torque limiter'
 * '<S4>'   : 'MatlabExcavatorModel/j2c'
 * '<S5>'   : 'MatlabExcavatorModel/Torque limiter/Limit torque at given engine speed'
 * '<S6>'   : 'MatlabExcavatorModel/Torque limiter/swashflow'
 */
#endif                                 /* RTW_HEADER_MatlabExcavatorModel_h_ */

/*
 * File trailer for generated code.
 *
 * [EOF]
 */
