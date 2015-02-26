/*
 * File: MatlabExcavatorModel.cpp
 *
 * Code generated for Simulink model 'MatlabExcavatorModel'.
 *
 * Model version                  : 1.263
 * Simulink Coder version         : 8.5 (R2013b) 08-Aug-2013
 * C/C++ source code generated on : Wed Feb 25 23:52:21 2015
 *
 * Target selection: ert.tlc
 * Embedded hardware selection: Generic->Unspecified (assume 32-bit Generic)
 * Code generation objectives: Unspecified
 * Validation result: Not run
 */

#include "MatlabExcavatorModel.h"
#include "MatlabExcavatorModel_private.h"

/* Block signals (auto storage) */
B_MatlabExcavatorModel_T MatlabExcavatorModel_B;

/* Block states (auto storage) */
DW_MatlabExcavatorModel_T MatlabExcavatorModel_DW;

/* External inputs (root inport signals with auto storage) */
ExtU_MatlabExcavatorModel_T MatlabExcavatorModel_U;

/* External outputs (root outports fed by signals with auto storage) */
ExtY_MatlabExcavatorModel_T MatlabExcavatorModel_Y;

/* Real-time model */
RT_MODEL_MatlabExcavatorModel_T MatlabExcavatorModel_M_;
RT_MODEL_MatlabExcavatorModel_T *const MatlabExcavatorModel_M =
  &MatlabExcavatorModel_M_;

/* Model step function */
void MatlabExcavatorModel_step(void)
{
  /* local block i/o variables */
  real_T rtb_Clock;
  real_T rtb_totalfuel;
  real_T torq_reduce;
  real_T rtb_flow_idx;
  real_T rtb_flow_idx_0;
  real_T rtb_flow_idx_1;
  real_T rtb_flow_idx_2;

  /* Clock: '<Root>/Clock' */
  rtb_Clock = MatlabExcavatorModel_M->Timing.t[0];

  /* Outport: '<Root>/Time' */
  MatlabExcavatorModel_Y.Time = rtb_Clock;

  /* MATLAB Function: '<S4>/swashflow' incorporates:
   *  Constant: '<S4>/Max Disp per Revolution'
   *  Constant: '<S4>/Maximum Pump Flow (in^3)'
   *  Constant: '<S4>/Nm to inlb conversion'
   *  Constant: '<S4>/fuel rate  matrix (kg//hr)'
   *  Constant: '<S4>/in Nm1'
   *  Constant: '<S4>/in rpm1'
   *  Constant: '<S4>/in rpms1'
   *  Constant: '<S4>/in sec'
   *  Constant: '<S4>/max torq at set engine speed'
   *  Delay: '<S4>/Integer Delay1'
   *  Delay: '<S4>/Integer Delay2'
   *  Delay: '<S4>/Integer Delay3'
   *  Inport: '<Root>/Flow '
   */
  /* MATLAB Function 'Torque limiter/swashflow': '<S6>:1' */
  /*  This function reads in swashplate data and interpolates and then calculates the flow */
  /* function [flow, totaltorq] = torq_limited( maxtorq, ) */
  /* '<S6>:1:6' */
  /* '<S6>:1:7' */
  /* '<S6>:1:15' */
  /* '<S6>:1:19' */
  rtb_flow_idx = MatlabExcavatorModel_U.Flow[0];
  rtb_flow_idx_0 = MatlabExcavatorModel_U.Flow[1];
  rtb_flow_idx_1 = MatlabExcavatorModel_U.Flow[2];
  rtb_flow_idx_2 = MatlabExcavatorModel_U.Flow[3];

  /* '<S6>:1:20' */
  /* '<S6>:1:21' */
  /* '<S6>:1:22' */
  /* '<S6>:1:23' */
  /* '<S6>:1:24' */
  /* '<S6>:1:22' */
  /* '<S6>:1:23' */
  /* '<S6>:1:24' */
  /* '<S6>:1:22' */
  /* '<S6>:1:23' */
  /* '<S6>:1:24' */
  /* '<S6>:1:22' */
  /* '<S6>:1:23' */
  /* '<S6>:1:24' */
  /* '<S6>:1:22' */
  /*  calculate needed torq for current demand */
  /* '<S6>:1:27' */
  /* '<S6>:1:28' */
  /* '<S6>:1:29' */
  /* .001/8.851; */
  /* '<S6>:1:28' */
  /* '<S6>:1:29' */
  /* .001/8.851; */
  /* '<S6>:1:28' */
  /* '<S6>:1:29' */
  /* .001/8.851; */
  /* '<S6>:1:28' */
  /* '<S6>:1:29' */
  torq_reduce = ((MatlabExcavatorModel_U.Flow[0] / 45.767808075 * 1.0984273938 *
                  (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[0] -
                   MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[0]) /
                  6.2831853071795862 + MatlabExcavatorModel_U.Flow[1] /
                  63.01311815766 * 1.0984273938 *
                  (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[1] -
                   MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[1]) /
                  6.2831853071795862) + MatlabExcavatorModel_U.Flow[2] /
                 63.01311815766 * 1.0984273938 *
                 (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[2] -
                  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[2]) /
                 6.2831853071795862) + MatlabExcavatorModel_U.Flow[3] /
    63.01311815766 * 1.0984273938 *
    (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[3] -
     MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[3]) / 6.2831853071795862;

  /* .001/8.851; */
  /* '<S6>:1:28' */
  /* '<S6>:1:31' */
  if (torq_reduce > MatlabExcavatorModel_ConstP.pooled2[(int32_T)
      MatlabExcavatorModel_ConstP.maxtorqatsetenginesp[48] - 1]) {
    /* '<S6>:1:33' */
    /* ( isnan(fuelmap(spdindex,torqindex)) ) % need to limit torque */
    /* torqindex = maxincol(spdindex); */
    /* torq = torqscale(torqindex); */
    /*  calculate % reduction rate based on torque limit */
    /* if (totaltorq > maxtorq) */
    /* '<S6>:1:39' */
    torq_reduce /= MatlabExcavatorModel_ConstP.pooled2[(int32_T)
      MatlabExcavatorModel_ConstP.maxtorqatsetenginesp[48] - 1];

    /* else */
    /*    torq_reduce = 1; */
    /* end */
    /*  reduce flow evenly for all cylinders */
    /* '<S6>:1:45' */
    /* flowout(k) = q(k)/hp_reduce; */
    /* '<S6>:1:47' */
    rtb_flow_idx = MatlabExcavatorModel_U.Flow[0] / torq_reduce;

    /* '<S6>:1:45' */
    /* flowout(k) = q(k)/hp_reduce; */
    /* '<S6>:1:47' */
    rtb_flow_idx_0 = MatlabExcavatorModel_U.Flow[1] / torq_reduce;

    /* '<S6>:1:45' */
    /* flowout(k) = q(k)/hp_reduce; */
    /* '<S6>:1:47' */
    rtb_flow_idx_1 = MatlabExcavatorModel_U.Flow[2] / torq_reduce;

    /* '<S6>:1:45' */
    /* flowout(k) = q(k)/hp_reduce; */
    /* '<S6>:1:47' */
    rtb_flow_idx_2 = MatlabExcavatorModel_U.Flow[3] / torq_reduce;

    /* '<S6>:1:45' */
  }

  /*  calculate limited torque (to check that it's < max torque) */
  /* '<S6>:1:51' */
  /* '<S6>:1:52' */
  /* '<S6>:1:53' */
  /* '<S6>:1:54' */
  /* '<S6>:1:55' */
  /* '<S6>:1:52' */
  /* '<S6>:1:53' */
  /* '<S6>:1:54' */
  /* '<S6>:1:55' */
  /* '<S6>:1:52' */
  /* '<S6>:1:53' */
  /* '<S6>:1:54' */
  /* '<S6>:1:55' */
  /* '<S6>:1:52' */
  /* '<S6>:1:53' */
  /* '<S6>:1:54' */
  /* '<S6>:1:55' */
  /* '<S6>:1:52' */
  /* '<S6>:1:58' */
  /* '<S6>:1:60' */
  rtb_flow_idx = (((rtb_flow_idx / 45.767808075 * 1.0984273938 *
                    (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[0] -
                     MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[0]) /
                    6.2831853071795862 + rtb_flow_idx_0 / 63.01311815766 *
                    1.0984273938 *
                    (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[1] -
                     MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[1]) /
                    6.2831853071795862) + rtb_flow_idx_1 / 63.01311815766 *
                   1.0984273938 * (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE
    [2] - MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[2]) / 6.2831853071795862)
                  + rtb_flow_idx_2 / 63.01311815766 * 1.0984273938 *
                  (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[3] -
                   MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[3]) /
                  6.2831853071795862) / 8.85075;
  if (rtb_flow_idx <= 0.0) {
    /* '<S6>:1:62' */
    /* '<S6>:1:63' */
    rtb_flow_idx = 0.0;
  }

  if (rtb_flow_idx <= 0.0) {
    /* '<S6>:1:65' */
    /* '<S6>:1:66' */
    rtb_flow_idx_0 = 1.0;
  } else if (rtb_flow_idx >= 169.0) {
    /* '<S6>:1:68' */
    /* '<S6>:1:69' */
    rtb_flow_idx_0 = 51.0;
  } else {
    /* '<S6>:1:71' */
    rtb_flow_idx_0 = floor(rtb_flow_idx / 3.38) + 1.0;
  }

  if (rtIsNaN(MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[((int32_T)
        rtb_flow_idx_0 - 1) * 57 + 49])) {
    /* '<S6>:1:76' */
    rtb_flow_idx_1 = (MatlabExcavatorModel_ConstInitP.fuelratematrixkghr
                      [((int32_T)rtb_flow_idx_0 - 1) * 57 + 48] -
                      MatlabExcavatorModel_ConstInitP.fuelratematrixkghr
                      [((int32_T)rtb_flow_idx_0 - 1) * 57 + 47]) / 25.0;
  } else {
    /* '<S6>:1:78' */
    rtb_flow_idx_1 = (MatlabExcavatorModel_ConstInitP.fuelratematrixkghr
                      [((int32_T)rtb_flow_idx_0 - 1) * 57 + 49] -
                      MatlabExcavatorModel_ConstInitP.fuelratematrixkghr
                      [((int32_T)rtb_flow_idx_0 - 1) * 57 + 48]) / 25.0;
  }

  if (rtIsNaN(MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[((int32_T)
        (rtb_flow_idx_0 + 1.0) - 1) * 57 + 48])) {
    /* '<S6>:1:82' */
    torq_reduce = (MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[((int32_T)
      rtb_flow_idx_0 - 1) * 57 + 48] -
                   MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[((int32_T)
                    (rtb_flow_idx_0 - 1.0) - 1) * 57 + 48]) / 3.38;
  } else {
    /* '<S6>:1:84' */
    torq_reduce = (MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[((int32_T)
                    (rtb_flow_idx_0 + 1.0) - 1) * 57 + 48] -
                   MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[((int32_T)
      rtb_flow_idx_0 - 1) * 57 + 48]) / 3.38;
  }

  /* '<S6>:1:87' */
  /* '<S6>:1:88' */
  /* '<S6>:1:90' */
  torq_reduce = (MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[((int32_T)
    rtb_flow_idx_0 - 1) * 57 + 48] + (2500.0 -
    MatlabExcavatorModel_ConstP.pooled3[48]) * rtb_flow_idx_1) + (rtb_flow_idx -
    MatlabExcavatorModel_ConstP.pooled2[(int32_T)rtb_flow_idx_0 - 1]) *
    torq_reduce;

  /* '<S6>:1:92' */
  rtb_totalfuel = torq_reduce * 0.001 / 3600.0 +
    MatlabExcavatorModel_DW.IntegerDelay3_DSTATE;

  /* Outport: '<Root>/FuelTotal' */
  MatlabExcavatorModel_Y.FuelTotal = rtb_totalfuel;

  /* Outport: '<Root>/FuelInstant' incorporates:
   *  MATLAB Function: '<S4>/swashflow'
   */
  MatlabExcavatorModel_Y.FuelInstant = torq_reduce;

  /* Level2 S-Function Block: '<S1>/S-Function' (cylmodel_435_ryder) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[0];
    ssSetOutputPortSignal(rts, 2, &MatlabExcavatorModel_Y.Volume);
    sfcnOutputs(rts, 1);
  }

  /* Outport: '<Root>/FlowSent' incorporates:
   *  Inport: '<Root>/Flow '
   */
  MatlabExcavatorModel_Y.FlowSent[0] = MatlabExcavatorModel_U.Flow[0];
  MatlabExcavatorModel_Y.FlowSent[1] = MatlabExcavatorModel_U.Flow[1];
  MatlabExcavatorModel_Y.FlowSent[2] = MatlabExcavatorModel_U.Flow[2];
  MatlabExcavatorModel_Y.FlowSent[3] = MatlabExcavatorModel_U.Flow[3];

  /* Outport: '<Root>/JointForce' incorporates:
   *  Gain: '<Root>/Gain'
   *  MATLAB Function: '<Root>/Get Force On Piston'
   */
  /* MATLAB Function 'Get Force On Piston': '<S2>:1' */
  /*  This function reads in swashplate data and interpolates and then calculates the flow */
  /* '<S2>:1:8' */
  /* '<S2>:1:9' */
  /* '<S2>:1:10' */
  /* '<S2>:1:11' */
  MatlabExcavatorModel_Y.JointForce[0] = (MatlabExcavatorModel_B.SFunction_o1[0]
    * 0.0 - MatlabExcavatorModel_B.SFunction_o2[0] * 0.0) * 4.4482216172334388;
  MatlabExcavatorModel_Y.JointForce[1] = (MatlabExcavatorModel_B.SFunction_o1[1]
    * 11.044661672776616 - MatlabExcavatorModel_B.SFunction_o2[1] *
    7.9030690191868231) * 4.4482216172334388;
  MatlabExcavatorModel_Y.JointForce[2] = (MatlabExcavatorModel_B.SFunction_o1[2]
    * 8.2957681008855477 - MatlabExcavatorModel_B.SFunction_o2[2] *
    5.1541754472957546) * 4.4482216172334388;
  MatlabExcavatorModel_Y.JointForce[3] = (MatlabExcavatorModel_B.SFunction_o1[3]
    * 7.0685834705770345 - MatlabExcavatorModel_B.SFunction_o2[3] *
    4.6633015951723493) * 4.4482216172334388;

  /* Level2 S-Function Block: '<S3>/S-Function' (p2torq1_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[1];
    ssSetOutputPortSignal(rts, 0, &MatlabExcavatorModel_Y.CabTorque);
    sfcnOutputs(rts, 1);
  }

  /* MATLAB Function 'Torque limiter/Limit torque at given engine speed': '<S5>:1' */
  /*  this function limits the pump output evenly for all 4 pumps when the */
  /*  maximum allowable torque is exceeded */
  /*  phead is head side pressure in psi */
  /*  prod is rod side pressure in psi */
  /*  q is flow in in^3/sec */
  /*  maxpumpflow is the max displacement of the pumps being driven at pumpspeed */
  /*  maxtorq is the max torque rating of the diesel engine in inch lbs */
  /*  maxflow_per_rev is the full displacement of the pump in in^3/rev */
  /* '<S5>:1:13' */
  /* '<S5>:1:14' */
  /* '<S5>:1:15' */
  /*  maxhp = 49; */
  /* '<S5>:1:18' */
  /*  calculate needed torq for current demand */
  /* '<S5>:1:24' */
  /* '<S5>:1:25' */
  /*  calculate % reduction rate based on torque limit */
  /*  % calculate needed hp for current demand */
  /*  totalhp = 0; */
  /*  for k=1:4 */
  /*      totalhp = totalhp + (phead(k)-prod(k))*q(k)/12/550; */
  /*  end */
  /*   */
  /*  % calculate % reduction rate off hp limit */
  /*  if (totalhp > maxhp) */
  /*      hp_reduce = totalhp/maxhp; */
  /*  else */
  /*      hp_reduce = 1; */
  /*  end */
  /*  reduce flow evenly for all cylinders */
  /* '<S5>:1:50' */
  /*  % calculate limited hp (to check that it's < max hp) */
  /*  totalhp = 0; */
  /*  for k = 1:4 */
  /*      totalhp = totalhp + (phead(k)-prod(k))*flowout(k)/12/550; */
  /*  end */
  /*  calculate limited torque (to check that it's < max torque) */
  /* '<S5>:1:62' */
  /* '<S5>:1:63' */
  /* '<S5>:1:69' */

  /* Update for Delay: '<S4>/Integer Delay3' */
  MatlabExcavatorModel_DW.IntegerDelay3_DSTATE = rtb_totalfuel;

  /* Update for Delay: '<S4>/Integer Delay1' */
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[0] =
    MatlabExcavatorModel_B.SFunction_o1[0];
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[1] =
    MatlabExcavatorModel_B.SFunction_o1[1];
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[2] =
    MatlabExcavatorModel_B.SFunction_o1[2];
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[3] =
    MatlabExcavatorModel_B.SFunction_o1[3];

  /* Update for Delay: '<S4>/Integer Delay2' */
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[0] =
    MatlabExcavatorModel_B.SFunction_o2[0];
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[1] =
    MatlabExcavatorModel_B.SFunction_o2[1];
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[2] =
    MatlabExcavatorModel_B.SFunction_o2[2];
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[3] =
    MatlabExcavatorModel_B.SFunction_o2[3];

  /* Update absolute time for base rate */
  /* The "clockTick0" counts the number of times the code of this task has
   * been executed. The absolute time is the multiplication of "clockTick0"
   * and "Timing.stepSize0". Size of "clockTick0" ensures timer will not
   * overflow during the application lifespan selected.
   * Timer of this task consists of two 32 bit unsigned integers.
   * The two integers represent the low bits Timing.clockTick0 and the high bits
   * Timing.clockTickH0. When the low bit overflows to 0, the high bits increment.
   */
  if (!(++MatlabExcavatorModel_M->Timing.clockTick0)) {
    ++MatlabExcavatorModel_M->Timing.clockTickH0;
  }

  MatlabExcavatorModel_M->Timing.t[0] =
    MatlabExcavatorModel_M->Timing.clockTick0 *
    MatlabExcavatorModel_M->Timing.stepSize0 +
    MatlabExcavatorModel_M->Timing.clockTickH0 *
    MatlabExcavatorModel_M->Timing.stepSize0 * 4294967296.0;

  {
    /* Update absolute timer for sample time: [0.001s, 0.0s] */
    /* The "clockTick1" counts the number of times the code of this task has
     * been executed. The absolute time is the multiplication of "clockTick1"
     * and "Timing.stepSize1". Size of "clockTick1" ensures timer will not
     * overflow during the application lifespan selected.
     * Timer of this task consists of two 32 bit unsigned integers.
     * The two integers represent the low bits Timing.clockTick1 and the high bits
     * Timing.clockTickH1. When the low bit overflows to 0, the high bits increment.
     */
    if (!(++MatlabExcavatorModel_M->Timing.clockTick1)) {
      ++MatlabExcavatorModel_M->Timing.clockTickH1;
    }

    MatlabExcavatorModel_M->Timing.t[1] =
      MatlabExcavatorModel_M->Timing.clockTick1 *
      MatlabExcavatorModel_M->Timing.stepSize1 +
      MatlabExcavatorModel_M->Timing.clockTickH1 *
      MatlabExcavatorModel_M->Timing.stepSize1 * 4294967296.0;
  }
}

/* Model initialize function */
void MatlabExcavatorModel_initialize(void)
{
  /* Registration code */

  /* initialize non-finites */
  rt_InitInfAndNaN(sizeof(real_T));

  /* non-finite (run-time) assignments */
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[854] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[911] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[968] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1025] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1082] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1138] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1139] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1195] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1196] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1252] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1253] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1309] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1310] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1365] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1366] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1367] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1422] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1423] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1424] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1479] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1480] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1481] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1536] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1537] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1538] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1592] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1593] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1594] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1595] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1649] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1650] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1651] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1652] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1653] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1706] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1707] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1708] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1709] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1710] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1711] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1762] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1763] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1764] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1765] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1766] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1767] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1768] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1769] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1819] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1820] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1821] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1822] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1823] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1824] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1825] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1826] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1827] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1876] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1877] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1878] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1879] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1880] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1881] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1882] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1883] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1884] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1933] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1934] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1935] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1936] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1937] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1938] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1939] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1940] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1941] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1942] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1990] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1991] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1992] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1993] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1994] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1995] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1996] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1997] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1998] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[1999] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2000] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2046] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2047] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2048] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2049] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2050] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2051] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2052] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2053] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2054] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2055] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2056] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2057] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2103] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2104] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2105] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2106] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2107] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2108] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2109] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2110] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2111] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2112] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2113] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2114] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2115] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2160] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2161] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2162] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2163] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2164] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2165] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2166] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2167] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2168] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2169] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2170] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2171] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2172] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2217] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2218] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2219] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2220] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2221] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2222] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2223] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2224] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2225] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2226] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2227] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2228] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2229] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2230] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2273] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2274] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2275] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2276] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2277] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2278] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2279] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2280] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2281] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2282] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2283] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2284] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2285] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2286] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2287] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2330] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2331] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2332] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2333] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2334] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2335] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2336] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2337] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2338] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2339] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2340] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2341] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2342] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2343] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2344] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2387] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2388] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2389] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2390] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2391] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2392] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2393] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2394] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2395] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2396] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2397] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2398] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2399] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2400] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2401] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2402] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2403] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2443] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2444] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2445] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2446] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2447] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2448] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2449] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2450] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2451] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2452] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2453] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2454] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2455] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2456] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2457] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2458] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2459] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2460] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2461] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2500] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2501] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2502] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2503] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2504] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2505] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2506] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2507] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2508] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2509] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2510] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2511] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2512] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2513] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2514] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2515] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2516] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2517] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2518] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2519] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2557] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2558] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2559] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2560] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2561] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2562] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2563] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2564] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2565] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2566] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2567] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2568] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2569] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2570] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2571] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2572] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2573] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2574] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2575] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2576] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2614] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2615] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2616] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2617] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2618] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2619] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2620] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2621] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2622] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2623] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2624] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2625] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2626] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2627] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2628] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2629] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2630] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2631] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2632] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2633] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2634] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2635] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2670] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2671] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2672] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2673] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2674] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2675] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2676] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2677] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2678] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2679] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2680] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2681] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2682] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2683] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2684] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2685] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2686] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2687] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2688] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2689] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2690] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2691] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2692] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2693] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2715] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2716] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2717] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2718] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2719] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2720] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2721] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2722] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2723] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2724] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2725] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2726] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2727] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2728] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2729] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2730] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2731] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2732] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2733] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2734] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2735] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2736] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2737] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2738] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2739] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2740] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2741] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2742] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2743] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2744] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2745] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2746] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2747] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2748] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2749] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2750] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2751] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2760] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2761] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2762] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2771] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2772] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2773] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2774] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2775] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2776] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2777] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2778] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2779] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2780] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2781] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2782] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2783] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2784] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2785] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2786] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2787] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2788] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2789] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2790] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2791] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2792] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2793] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2794] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2795] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2796] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2797] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2798] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2799] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2800] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2801] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2802] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2803] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2804] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2805] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2806] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2807] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2808] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2809] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2810] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2811] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2812] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2813] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2814] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2815] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2816] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2817] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2818] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2819] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2820] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2827] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2828] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2829] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2830] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2831] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2832] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2833] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2834] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2835] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2836] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2837] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2838] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2839] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2840] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2841] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2842] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2843] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2844] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2845] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2846] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2847] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2848] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2849] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2850] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2851] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2852] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2853] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2854] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2855] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2856] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2857] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2858] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2859] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2860] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2861] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2862] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2863] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2864] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2865] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2866] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2867] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2868] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2869] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2870] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2871] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2872] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2873] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2874] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2875] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2876] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2877] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2879] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2880] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2881] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2882] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2883] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2884] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2885] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2886] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2887] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2888] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2889] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2890] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2891] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2892] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2893] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2894] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2895] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2896] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2897] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2898] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2899] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2900] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2901] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2902] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2903] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2904] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2905] = rtNaN;
  MatlabExcavatorModel_ConstInitP.fuelratematrixkghr[2906] = rtNaN;

  /* initialize real-time model */
  (void) memset((void *)MatlabExcavatorModel_M, 0,
                sizeof(RT_MODEL_MatlabExcavatorModel_T));

  {
    /* Setup solver object */
    rtsiSetSimTimeStepPtr(&MatlabExcavatorModel_M->solverInfo,
                          &MatlabExcavatorModel_M->Timing.simTimeStep);
    rtsiSetTPtr(&MatlabExcavatorModel_M->solverInfo, &rtmGetTPtr
                (MatlabExcavatorModel_M));
    rtsiSetStepSizePtr(&MatlabExcavatorModel_M->solverInfo,
                       &MatlabExcavatorModel_M->Timing.stepSize0);
    rtsiSetErrorStatusPtr(&MatlabExcavatorModel_M->solverInfo, ((const char_T **)
                           (&rtmGetErrorStatus(MatlabExcavatorModel_M))));
    rtsiSetRTModelPtr(&MatlabExcavatorModel_M->solverInfo,
                      MatlabExcavatorModel_M);
  }

  rtsiSetSimTimeStep(&MatlabExcavatorModel_M->solverInfo, MAJOR_TIME_STEP);
  rtsiSetSolverName(&MatlabExcavatorModel_M->solverInfo,"FixedStepDiscrete");
  MatlabExcavatorModel_M->solverInfoPtr = (&MatlabExcavatorModel_M->solverInfo);

  /* Initialize timing info */
  {
    int_T *mdlTsMap = MatlabExcavatorModel_M->Timing.sampleTimeTaskIDArray;
    mdlTsMap[0] = 0;
    mdlTsMap[1] = 1;
    MatlabExcavatorModel_M->Timing.sampleTimeTaskIDPtr = (&mdlTsMap[0]);
    MatlabExcavatorModel_M->Timing.sampleTimes =
      (&MatlabExcavatorModel_M->Timing.sampleTimesArray[0]);
    MatlabExcavatorModel_M->Timing.offsetTimes =
      (&MatlabExcavatorModel_M->Timing.offsetTimesArray[0]);

    /* task periods */
    MatlabExcavatorModel_M->Timing.sampleTimes[0] = (0.0);
    MatlabExcavatorModel_M->Timing.sampleTimes[1] = (0.001);

    /* task offsets */
    MatlabExcavatorModel_M->Timing.offsetTimes[0] = (0.0);
    MatlabExcavatorModel_M->Timing.offsetTimes[1] = (0.0);
  }

  rtmSetTPtr(MatlabExcavatorModel_M, &MatlabExcavatorModel_M->Timing.tArray[0]);

  {
    int_T *mdlSampleHits = MatlabExcavatorModel_M->Timing.sampleHitArray;
    mdlSampleHits[0] = 1;
    mdlSampleHits[1] = 1;
    MatlabExcavatorModel_M->Timing.sampleHits = (&mdlSampleHits[0]);
  }

  rtmSetTFinal(MatlabExcavatorModel_M, -1);
  MatlabExcavatorModel_M->Timing.stepSize0 = 0.001;
  MatlabExcavatorModel_M->Timing.stepSize1 = 0.001;
  MatlabExcavatorModel_M->solverInfoPtr = (&MatlabExcavatorModel_M->solverInfo);
  MatlabExcavatorModel_M->Timing.stepSize = (0.001);
  rtsiSetFixedStepSize(&MatlabExcavatorModel_M->solverInfo, 0.001);
  rtsiSetSolverMode(&MatlabExcavatorModel_M->solverInfo,
                    SOLVER_MODE_SINGLETASKING);

  /* block I/O */
  (void) memset(((void *) &MatlabExcavatorModel_B), 0,
                sizeof(B_MatlabExcavatorModel_T));

  /* states (dwork) */
  (void) memset((void *)&MatlabExcavatorModel_DW, 0,
                sizeof(DW_MatlabExcavatorModel_T));

  /* external inputs */
  (void) memset((void *)&MatlabExcavatorModel_U, 0,
                sizeof(ExtU_MatlabExcavatorModel_T));

  /* external outputs */
  (void) memset((void *)&MatlabExcavatorModel_Y, 0,
                sizeof(ExtY_MatlabExcavatorModel_T));

  /* child S-Function registration */
  {
    RTWSfcnInfo *sfcnInfo = &MatlabExcavatorModel_M->NonInlinedSFcns.sfcnInfo;
    MatlabExcavatorModel_M->sfcnInfo = (sfcnInfo);
    rtssSetErrorStatusPtr(sfcnInfo, ((const char_T **)(&rtmGetErrorStatus
      (MatlabExcavatorModel_M))));
    rtssSetNumRootSampTimesPtr(sfcnInfo,
      &MatlabExcavatorModel_M->Sizes.numSampTimes);
    MatlabExcavatorModel_M->NonInlinedSFcns.taskTimePtrs[0] = &(rtmGetTPtr
      (MatlabExcavatorModel_M)[0]);
    MatlabExcavatorModel_M->NonInlinedSFcns.taskTimePtrs[1] = &(rtmGetTPtr
      (MatlabExcavatorModel_M)[1]);
    rtssSetTPtrPtr(sfcnInfo,MatlabExcavatorModel_M->NonInlinedSFcns.taskTimePtrs);
    rtssSetTStartPtr(sfcnInfo, &rtmGetTStart(MatlabExcavatorModel_M));
    rtssSetTFinalPtr(sfcnInfo, &rtmGetTFinal(MatlabExcavatorModel_M));
    rtssSetTimeOfLastOutputPtr(sfcnInfo, &rtmGetTimeOfLastOutput
      (MatlabExcavatorModel_M));
    rtssSetStepSizePtr(sfcnInfo, &MatlabExcavatorModel_M->Timing.stepSize);
    rtssSetStopRequestedPtr(sfcnInfo, &rtmGetStopRequested
      (MatlabExcavatorModel_M));
    rtssSetDerivCacheNeedsResetPtr(sfcnInfo,
      &MatlabExcavatorModel_M->ModelData.derivCacheNeedsReset);
    rtssSetZCCacheNeedsResetPtr(sfcnInfo,
      &MatlabExcavatorModel_M->ModelData.zCCacheNeedsReset);
    rtssSetBlkStateChangePtr(sfcnInfo,
      &MatlabExcavatorModel_M->ModelData.blkStateChange);
    rtssSetSampleHitsPtr(sfcnInfo, &MatlabExcavatorModel_M->Timing.sampleHits);
    rtssSetPerTaskSampleHitsPtr(sfcnInfo,
      &MatlabExcavatorModel_M->Timing.perTaskSampleHits);
    rtssSetSimModePtr(sfcnInfo, &MatlabExcavatorModel_M->simMode);
    rtssSetSolverInfoPtr(sfcnInfo, &MatlabExcavatorModel_M->solverInfoPtr);
  }

  MatlabExcavatorModel_M->Sizes.numSFcns = (2);

  /* register each child */
  {
    (void) memset((void *)
                  &MatlabExcavatorModel_M->NonInlinedSFcns.childSFunctions[0], 0,
                  2*sizeof(SimStruct));
    MatlabExcavatorModel_M->childSfunctions =
      (&MatlabExcavatorModel_M->NonInlinedSFcns.childSFunctionPtrs[0]);
    MatlabExcavatorModel_M->childSfunctions[0] =
      (&MatlabExcavatorModel_M->NonInlinedSFcns.childSFunctions[0]);
    MatlabExcavatorModel_M->childSfunctions[1] =
      (&MatlabExcavatorModel_M->NonInlinedSFcns.childSFunctions[1]);

    /* Level2 S-Function Block: MatlabExcavatorModel/<S1>/S-Function (cylmodel_435_ryder) */
    {
      SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[0];

      /* timing info */
      time_T *sfcnPeriod =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn0.sfcnPeriod;
      time_T *sfcnOffset =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn0.sfcnOffset;
      int_T *sfcnTsMap = MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn0.sfcnTsMap;
      (void) memset((void*)sfcnPeriod, 0,
                    sizeof(time_T)*1);
      (void) memset((void*)sfcnOffset, 0,
                    sizeof(time_T)*1);
      ssSetSampleTimePtr(rts, &sfcnPeriod[0]);
      ssSetOffsetTimePtr(rts, &sfcnOffset[0]);
      ssSetSampleTimeTaskIDPtr(rts, sfcnTsMap);

      /* Set up the mdlInfo pointer */
      {
        ssSetBlkInfo2Ptr(rts, &MatlabExcavatorModel_M->NonInlinedSFcns.blkInfo2
                         [0]);
      }

      ssSetRTWSfcnInfo(rts, MatlabExcavatorModel_M->sfcnInfo);

      /* Allocate memory of model methods 2 */
      {
        ssSetModelMethods2(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods2[0]);
      }

      /* Allocate memory of model methods 3 */
      {
        ssSetModelMethods3(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods3[0]);
      }

      /* Allocate memory for states auxilliary information */
      {
        ssSetStatesInfo2(rts,
                         &MatlabExcavatorModel_M->NonInlinedSFcns.statesInfo2[0]);
      }

      /* inputs */
      {
        _ssSetNumInputPorts(rts, 3);
        ssSetPortInfoForInputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn0.inputPortInfo[0]);

        /* port 0 */
        {
          ssSetInputPortRequiredContiguous(rts, 0, 1);
          ssSetInputPortSignal(rts, 0, &MatlabExcavatorModel_U.CylinderPosition
                               [0]);
          _ssSetInputPortNumDimensions(rts, 0, 1);
          ssSetInputPortWidth(rts, 0, 4);
        }

        /* port 1 */
        {
          ssSetInputPortRequiredContiguous(rts, 1, 1);
          ssSetInputPortSignal(rts, 1, &MatlabExcavatorModel_U.CylinderVelocity
                               [0]);
          _ssSetInputPortNumDimensions(rts, 1, 1);
          ssSetInputPortWidth(rts, 1, 4);
        }

        /* port 2 */
        {
          ssSetInputPortRequiredContiguous(rts, 2, 1);
          ssSetInputPortSignal(rts, 2, &MatlabExcavatorModel_U.Flow[0]);
          _ssSetInputPortNumDimensions(rts, 2, 1);
          ssSetInputPortWidth(rts, 2, 4);
        }
      }

      /* outputs */
      {
        ssSetPortInfoForOutputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn0.outputPortInfo[0]);
        _ssSetNumOutputPorts(rts, 5);

        /* port 0 */
        {
          _ssSetOutputPortNumDimensions(rts, 0, 1);
          ssSetOutputPortWidth(rts, 0, 4);
          ssSetOutputPortSignal(rts, 0, ((real_T *)
            MatlabExcavatorModel_B.SFunction_o1));
        }

        /* port 1 */
        {
          _ssSetOutputPortNumDimensions(rts, 1, 1);
          ssSetOutputPortWidth(rts, 1, 4);
          ssSetOutputPortSignal(rts, 1, ((real_T *)
            MatlabExcavatorModel_B.SFunction_o2));
        }

        /* port 2 */
        {
          _ssSetOutputPortNumDimensions(rts, 2, 1);
          ssSetOutputPortWidth(rts, 2, 1);
          ssSetOutputPortSignal(rts, 2, ((uint16_T *)
            &MatlabExcavatorModel_Y.Volume));
        }

        /* port 3 */
        {
          _ssSetOutputPortNumDimensions(rts, 3, 1);
          ssSetOutputPortWidth(rts, 3, 1);
          ssSetOutputPortSignal(rts, 3, ((real_T *)
            &MatlabExcavatorModel_B.SFunction_o4));
        }

        /* port 4 */
        {
          _ssSetOutputPortNumDimensions(rts, 4, 1);
          ssSetOutputPortWidth(rts, 4, 4);
          ssSetOutputPortSignal(rts, 4, ((real_T *)
            MatlabExcavatorModel_B.SFunction_o5));
        }
      }

      /* states */
      ssSetDiscStates(rts, (real_T *) &MatlabExcavatorModel_DW.SFunction_DSTATE
                      [0]);

      /* path info */
      ssSetModelName(rts, "S-Function");
      ssSetPath(rts, "MatlabExcavatorModel/Cylinder Model/S-Function");
      ssSetRTModel(rts,MatlabExcavatorModel_M);
      ssSetParentSS(rts, (NULL));
      ssSetRootSS(rts, rts);
      ssSetVersion(rts, SIMSTRUCT_VERSION_LEVEL2);

      /* parameters */
      {
        mxArray **sfcnParams = (mxArray **)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn0.params;
        ssSetSFcnParamsCount(rts, 27);
        ssSetSFcnParamsPtr(rts, &sfcnParams[0]);
        ssSetSFcnParam(rts, 0, (mxArray*)MatlabExcavatorModel_ConstP.pooled5);
        ssSetSFcnParam(rts, 1, (mxArray*)
                       MatlabExcavatorModel_ConstP.SFunction_P2_Size);
        ssSetSFcnParam(rts, 2, (mxArray*)
                       MatlabExcavatorModel_ConstP.SFunction_P3_Size);
        ssSetSFcnParam(rts, 3, (mxArray*)MatlabExcavatorModel_ConstP.pooled7);
        ssSetSFcnParam(rts, 4, (mxArray*)MatlabExcavatorModel_ConstP.pooled9);
        ssSetSFcnParam(rts, 5, (mxArray*)MatlabExcavatorModel_ConstP.pooled7);
        ssSetSFcnParam(rts, 6, (mxArray*)MatlabExcavatorModel_ConstP.pooled11);
        ssSetSFcnParam(rts, 7, (mxArray*)MatlabExcavatorModel_ConstP.pooled7);
        ssSetSFcnParam(rts, 8, (mxArray*)MatlabExcavatorModel_ConstP.pooled11);
        ssSetSFcnParam(rts, 9, (mxArray*)MatlabExcavatorModel_ConstP.pooled9);
        ssSetSFcnParam(rts, 10, (mxArray*)
                       MatlabExcavatorModel_ConstP.SFunction_P11_Size);
        ssSetSFcnParam(rts, 11, (mxArray*)MatlabExcavatorModel_ConstP.pooled13);
        ssSetSFcnParam(rts, 12, (mxArray*)MatlabExcavatorModel_ConstP.pooled13);
        ssSetSFcnParam(rts, 13, (mxArray*)MatlabExcavatorModel_ConstP.pooled13);
        ssSetSFcnParam(rts, 14, (mxArray*)MatlabExcavatorModel_ConstP.pooled13);
        ssSetSFcnParam(rts, 15, (mxArray*)MatlabExcavatorModel_ConstP.pooled15);
        ssSetSFcnParam(rts, 16, (mxArray*)MatlabExcavatorModel_ConstP.pooled15);
        ssSetSFcnParam(rts, 17, (mxArray*)MatlabExcavatorModel_ConstP.pooled15);
        ssSetSFcnParam(rts, 18, (mxArray*)MatlabExcavatorModel_ConstP.pooled15);
        ssSetSFcnParam(rts, 19, (mxArray*)MatlabExcavatorModel_ConstP.pooled17);
        ssSetSFcnParam(rts, 20, (mxArray*)MatlabExcavatorModel_ConstP.pooled17);
        ssSetSFcnParam(rts, 21, (mxArray*)MatlabExcavatorModel_ConstP.pooled17);
        ssSetSFcnParam(rts, 22, (mxArray*)MatlabExcavatorModel_ConstP.pooled17);
        ssSetSFcnParam(rts, 23, (mxArray*)
                       MatlabExcavatorModel_ConstP.SFunction_P24_Size);
        ssSetSFcnParam(rts, 24, (mxArray*)MatlabExcavatorModel_ConstP.pooled19);
        ssSetSFcnParam(rts, 25, (mxArray*)MatlabExcavatorModel_ConstP.pooled19);
        ssSetSFcnParam(rts, 26, (mxArray*)MatlabExcavatorModel_ConstP.pooled19);
      }

      /* work vectors */
      {
        struct _ssDWorkRecord *dWorkRecord = (struct _ssDWorkRecord *)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn0.dWork;
        struct _ssDWorkAuxRecord *dWorkAuxRecord = (struct _ssDWorkAuxRecord *)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn0.dWorkAux;
        ssSetSFcnDWork(rts, dWorkRecord);
        ssSetSFcnDWorkAux(rts, dWorkAuxRecord);
        _ssSetNumDWork(rts, 1);

        /* DSTATE */
        ssSetDWorkWidth(rts, 0, 8);
        ssSetDWorkDataType(rts, 0,SS_DOUBLE);
        ssSetDWorkComplexSignal(rts, 0, 0);
        ssSetDWorkUsedAsDState(rts, 0, 1);
        ssSetDWork(rts, 0, &MatlabExcavatorModel_DW.SFunction_DSTATE[0]);
      }

      /* registration */
      cylmodel_435_ryder(rts);
      sfcnInitializeSizes(rts);
      sfcnInitializeSampleTimes(rts);

      /* adjust sample time */
      ssSetSampleTime(rts, 0, 0.001);
      ssSetOffsetTime(rts, 0, 0.0);
      sfcnTsMap[0] = 1;

      /* set compiled values of dynamic vector attributes */
      ssSetNumNonsampledZCs(rts, 0);

      /* Update connectivity flags for each port */
      _ssSetInputPortConnected(rts, 0, 1);
      _ssSetInputPortConnected(rts, 1, 1);
      _ssSetInputPortConnected(rts, 2, 1);
      _ssSetOutputPortConnected(rts, 0, 1);
      _ssSetOutputPortConnected(rts, 1, 1);
      _ssSetOutputPortConnected(rts, 2, 1);
      _ssSetOutputPortConnected(rts, 3, 1);
      _ssSetOutputPortConnected(rts, 4, 0);
      _ssSetOutputPortBeingMerged(rts, 0, 0);
      _ssSetOutputPortBeingMerged(rts, 1, 0);
      _ssSetOutputPortBeingMerged(rts, 2, 0);
      _ssSetOutputPortBeingMerged(rts, 3, 0);
      _ssSetOutputPortBeingMerged(rts, 4, 0);

      /* Update the BufferDstPort flags for each input port */
      ssSetInputPortBufferDstPort(rts, 0, -1);
      ssSetInputPortBufferDstPort(rts, 1, -1);
      ssSetInputPortBufferDstPort(rts, 2, -1);
    }

    /* Level2 S-Function Block: MatlabExcavatorModel/<S3>/S-Function (p2torq1_435_brake) */
    {
      SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[1];

      /* timing info */
      time_T *sfcnPeriod =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn1.sfcnPeriod;
      time_T *sfcnOffset =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn1.sfcnOffset;
      int_T *sfcnTsMap = MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn1.sfcnTsMap;
      (void) memset((void*)sfcnPeriod, 0,
                    sizeof(time_T)*1);
      (void) memset((void*)sfcnOffset, 0,
                    sizeof(time_T)*1);
      ssSetSampleTimePtr(rts, &sfcnPeriod[0]);
      ssSetOffsetTimePtr(rts, &sfcnOffset[0]);
      ssSetSampleTimeTaskIDPtr(rts, sfcnTsMap);

      /* Set up the mdlInfo pointer */
      {
        ssSetBlkInfo2Ptr(rts, &MatlabExcavatorModel_M->NonInlinedSFcns.blkInfo2
                         [1]);
      }

      ssSetRTWSfcnInfo(rts, MatlabExcavatorModel_M->sfcnInfo);

      /* Allocate memory of model methods 2 */
      {
        ssSetModelMethods2(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods2[1]);
      }

      /* Allocate memory of model methods 3 */
      {
        ssSetModelMethods3(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods3[1]);
      }

      /* Allocate memory for states auxilliary information */
      {
        ssSetStatesInfo2(rts,
                         &MatlabExcavatorModel_M->NonInlinedSFcns.statesInfo2[1]);
      }

      /* inputs */
      {
        _ssSetNumInputPorts(rts, 4);
        ssSetPortInfoForInputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn1.inputPortInfo[0]);

        /* port 0 */
        {
          ssSetInputPortRequiredContiguous(rts, 0, 1);
          ssSetInputPortSignal(rts, 0, &MatlabExcavatorModel_B.SFunction_o1[0]);
          _ssSetInputPortNumDimensions(rts, 0, 1);
          ssSetInputPortWidth(rts, 0, 1);
        }

        /* port 1 */
        {
          ssSetInputPortRequiredContiguous(rts, 1, 1);
          ssSetInputPortSignal(rts, 1, &MatlabExcavatorModel_B.SFunction_o2[0]);
          _ssSetInputPortNumDimensions(rts, 1, 1);
          ssSetInputPortWidth(rts, 1, 1);
        }

        /* port 2 */
        {
          ssSetInputPortRequiredContiguous(rts, 2, 1);
          ssSetInputPortSignal(rts, 2, &MatlabExcavatorModel_U.CylinderPosition
                               [0]);
          _ssSetInputPortNumDimensions(rts, 2, 1);
          ssSetInputPortWidth(rts, 2, 1);
        }

        /* port 3 */
        {
          ssSetInputPortRequiredContiguous(rts, 3, 1);
          ssSetInputPortSignal(rts, 3, &MatlabExcavatorModel_B.SFunction_o4);
          _ssSetInputPortNumDimensions(rts, 3, 1);
          ssSetInputPortWidth(rts, 3, 1);
        }
      }

      /* outputs */
      {
        ssSetPortInfoForOutputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn1.outputPortInfo[0]);
        _ssSetNumOutputPorts(rts, 1);

        /* port 0 */
        {
          _ssSetOutputPortNumDimensions(rts, 0, 1);
          ssSetOutputPortWidth(rts, 0, 1);
          ssSetOutputPortSignal(rts, 0, ((real_T *)
            &MatlabExcavatorModel_Y.CabTorque));
        }
      }

      /* path info */
      ssSetModelName(rts, "S-Function");
      ssSetPath(rts,
                "MatlabExcavatorModel/Pressure to Torque Calculation/S-Function");
      ssSetRTModel(rts,MatlabExcavatorModel_M);
      ssSetParentSS(rts, (NULL));
      ssSetRootSS(rts, rts);
      ssSetVersion(rts, SIMSTRUCT_VERSION_LEVEL2);

      /* parameters */
      {
        mxArray **sfcnParams = (mxArray **)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn1.params;
        ssSetSFcnParamsCount(rts, 4);
        ssSetSFcnParamsPtr(rts, &sfcnParams[0]);
        ssSetSFcnParam(rts, 0, (mxArray*)MatlabExcavatorModel_ConstP.pooled5);
        ssSetSFcnParam(rts, 1, (mxArray*)
                       MatlabExcavatorModel_ConstP.SFunction_P2_Size_n);
        ssSetSFcnParam(rts, 2, (mxArray*)
                       MatlabExcavatorModel_ConstP.SFunction_P3_Size_j);
        ssSetSFcnParam(rts, 3, (mxArray*)
                       MatlabExcavatorModel_ConstP.SFunction_P4_Size);
      }

      /* registration */
      p2torq1_435_brake(rts);
      sfcnInitializeSizes(rts);
      sfcnInitializeSampleTimes(rts);

      /* adjust sample time */
      ssSetSampleTime(rts, 0, 0.001);
      ssSetOffsetTime(rts, 0, 0.0);
      sfcnTsMap[0] = 1;

      /* set compiled values of dynamic vector attributes */
      ssSetNumNonsampledZCs(rts, 0);

      /* Update connectivity flags for each port */
      _ssSetInputPortConnected(rts, 0, 1);
      _ssSetInputPortConnected(rts, 1, 1);
      _ssSetInputPortConnected(rts, 2, 1);
      _ssSetInputPortConnected(rts, 3, 1);
      _ssSetOutputPortConnected(rts, 0, 1);
      _ssSetOutputPortBeingMerged(rts, 0, 0);

      /* Update the BufferDstPort flags for each input port */
      ssSetInputPortBufferDstPort(rts, 0, -1);
      ssSetInputPortBufferDstPort(rts, 1, -1);
      ssSetInputPortBufferDstPort(rts, 2, -1);
      ssSetInputPortBufferDstPort(rts, 3, -1);
    }
  }

  /* Level2 S-Function Block: '<S3>/S-Function' (p2torq1_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[1];
    sfcnStart(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* InitializeConditions for Delay: '<S4>/Integer Delay1' */
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[0] = 435.11321399999997;
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[1] = 435.11321399999997;
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[2] = 435.11321399999997;
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[3] = 435.11321399999997;

  /* InitializeConditions for Delay: '<S4>/Integer Delay2' */
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[0] = 435.11321399999997;
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[1] = 435.11321399999997;
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[2] = 435.11321399999997;
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[3] = 435.11321399999997;

  /* Level2 S-Function Block: '<S1>/S-Function' (cylmodel_435_ryder) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[0];
    sfcnInitializeConditions(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* Level2 S-Function Block: '<S3>/S-Function' (p2torq1_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[1];
    sfcnInitializeConditions(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }
}

/* Model terminate function */
void MatlabExcavatorModel_terminate(void)
{
  /* Level2 S-Function Block: '<S1>/S-Function' (cylmodel_435_ryder) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[0];
    sfcnTerminate(rts);
  }

  /* Level2 S-Function Block: '<S3>/S-Function' (p2torq1_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[1];
    sfcnTerminate(rts);
  }
}

/*
 * File trailer for generated code.
 *
 * [EOF]
 */
