/*
 * File: MatlabExcavatorModel_private.h
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

#ifndef RTW_HEADER_MatlabExcavatorModel_private_h_
#define RTW_HEADER_MatlabExcavatorModel_private_h_
#include "rtwtypes.h"

/* Private macros used by the generated code to access rtModel */
#ifndef rtmIsMajorTimeStep
# define rtmIsMajorTimeStep(rtm)       (((rtm)->Timing.simTimeStep) == MAJOR_TIME_STEP)
#endif

#ifndef rtmIsMinorTimeStep
# define rtmIsMinorTimeStep(rtm)       (((rtm)->Timing.simTimeStep) == MINOR_TIME_STEP)
#endif

#ifndef rtmSetTFinal
# define rtmSetTFinal(rtm, val)        ((rtm)->Timing.tFinal = (val))
#endif

#ifndef rtmGetTPtr
# define rtmGetTPtr(rtm)               ((rtm)->Timing.t)
#endif

#ifndef rtmSetTPtr
# define rtmSetTPtr(rtm, val)          ((rtm)->Timing.t = (val))
#endif

#ifndef __RTWTYPES_H__
#error This file requires rtwtypes.h to be included
#else
#ifdef TMWTYPES_PREVIOUSLY_INCLUDED
#error This file requires rtwtypes.h to be included before tmwtypes.h
#endif                                 /* TMWTYPES_PREVIOUSLY_INCLUDED */
#endif                                 /* __RTWTYPES_H__ */

extern "C" void j2c_435(SimStruct *rts);
extern "C" void cylmodel_435_ryder(SimStruct *rts);
extern "C" void p2torq1_435_brake(SimStruct *rts);
extern "C" void p2torq2_435(SimStruct *rts);
extern "C" void p2torq3_435(SimStruct *rts);
extern "C" void p2torq4_435(SimStruct *rts);

#endif                                 /* RTW_HEADER_MatlabExcavatorModel_private_h_ */

/*
 * File trailer for generated code.
 *
 * [EOF]
 */
