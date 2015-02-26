/*
 * File: MatlabExcavatorModel.cpp
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

/* Forward declaration for local functions */
static void MatlabExcavatorModel_rdivide(const real_T x[9], real_T y, real_T z[9]);
static void MatlabExcavatorModel_cross(const real_T a[3], const real_T b[3],
  real_T c[3]);
static void MatlabExcavatorModel_cross_g(const real_T a[3], const real_T b[3],
  real_T c[3]);
static void MatlabExcavatorModel_cross_gz(const real_T a[3], const real_T b[3],
  real_T c[3]);
static void MatlabExcavatorMode_mldivide_ly(const real_T A[16], real_T B[4]);
static void MatlabExcavatorModel_mldivide(const real_T A[9], const real_T B[3],
  real_T Y[3]);
static void MatlabExcavatorModel_mldivide_l(const real_T A[4], const real_T B[2],
  real_T Y[2]);

/* Function for MATLAB Function: '<Root>/Arm Dynamics' */
static void MatlabExcavatorModel_rdivide(const real_T x[9], real_T y, real_T z[9])
{
  int32_T i;
  for (i = 0; i < 9; i++) {
    z[i] = x[i] / y;
  }
}

/* Function for MATLAB Function: '<Root>/Arm Dynamics' */
static void MatlabExcavatorModel_cross(const real_T a[3], const real_T b[3],
  real_T c[3])
{
  c[0] = a[1] * b[2] - a[2] * b[1];
  c[1] = a[2] * b[0] - a[0] * b[2];
  c[2] = a[0] * b[1] - a[1] * b[0];
}

/* Function for MATLAB Function: '<Root>/Arm Dynamics' */
static void MatlabExcavatorModel_cross_g(const real_T a[3], const real_T b[3],
  real_T c[3])
{
  c[0] = a[1] * b[2] - a[2] * b[1];
  c[1] = a[2] * b[0] - a[0] * b[2];
  c[2] = a[0] * b[1] - a[1] * b[0];
}

/* Function for MATLAB Function: '<Root>/Arm Dynamics' */
static void MatlabExcavatorModel_cross_gz(const real_T a[3], const real_T b[3],
  real_T c[3])
{
  c[0] = a[1] * b[2] - a[2] * b[1];
  c[1] = a[2] * b[0] - a[0] * b[2];
  c[2] = a[0] * b[1] - a[1] * b[0];
}

/* Function for MATLAB Function: '<Root>/Arm Dynamics' */
static void MatlabExcavatorMode_mldivide_ly(const real_T A[16], real_T B[4])
{
  real_T temp;
  real_T b_A[16];
  int8_T ipiv[4];
  int32_T j;
  int32_T c;
  int32_T ix;
  real_T s;
  int32_T iy;
  int32_T jA;
  int32_T c_ix;
  int32_T d;
  int32_T ijA;
  memcpy(&b_A[0], &A[0], sizeof(real_T) << 4U);
  ipiv[0] = 1;
  ipiv[1] = 2;
  ipiv[2] = 3;
  for (j = 0; j < 3; j++) {
    c = j * 5;
    jA = 0;
    ix = c;
    temp = fabs(b_A[c]);
    for (iy = 2; iy <= 4 - j; iy++) {
      ix++;
      s = fabs(b_A[ix]);
      if (s > temp) {
        jA = iy - 1;
        temp = s;
      }
    }

    if (b_A[c + jA] != 0.0) {
      if (jA != 0) {
        ipiv[j] = (int8_T)((j + jA) + 1);
        iy = j + jA;
        temp = b_A[j];
        b_A[j] = b_A[iy];
        b_A[iy] = temp;
        ix = j + 4;
        iy += 4;
        temp = b_A[ix];
        b_A[ix] = b_A[iy];
        b_A[iy] = temp;
        ix += 4;
        iy += 4;
        temp = b_A[ix];
        b_A[ix] = b_A[iy];
        b_A[iy] = temp;
        ix += 4;
        iy += 4;
        temp = b_A[ix];
        b_A[ix] = b_A[iy];
        b_A[iy] = temp;
      }

      ix = (c - j) + 4;
      for (iy = c + 1; iy + 1 <= ix; iy++) {
        b_A[iy] /= b_A[c];
      }
    }

    jA = c;
    ix = c + 4;
    for (iy = 1; iy <= 3 - j; iy++) {
      temp = b_A[ix];
      if (b_A[ix] != 0.0) {
        c_ix = c + 1;
        d = (jA - j) + 8;
        for (ijA = 5 + jA; ijA + 1 <= d; ijA++) {
          b_A[ijA] += b_A[c_ix] * -temp;
          c_ix++;
        }
      }

      ix += 4;
      jA += 4;
    }
  }

  if (ipiv[0] != 1) {
    temp = B[0];
    B[0] = B[ipiv[0] - 1];
    B[ipiv[0] - 1] = temp;
  }

  if (ipiv[1] != 2) {
    temp = B[1];
    B[1] = B[ipiv[1] - 1];
    B[ipiv[1] - 1] = temp;
  }

  if (ipiv[2] != 3) {
    temp = B[2];
    B[2] = B[ipiv[2] - 1];
    B[ipiv[2] - 1] = temp;
  }

  if (B[0] != 0.0) {
    for (jA = 2; jA < 5; jA++) {
      B[jA - 1] -= b_A[jA - 1] * B[0];
    }
  }

  if (B[1] != 0.0) {
    for (jA = 3; jA < 5; jA++) {
      B[jA - 1] -= b_A[jA + 3] * B[1];
    }
  }

  if (B[2] != 0.0) {
    jA = 4;
    while (jA < 5) {
      B[3] -= B[2] * b_A[11];
      jA = 5;
    }
  }

  if (B[3] != 0.0) {
    B[3] /= b_A[15];
    for (jA = 0; jA + 1 < 4; jA++) {
      B[jA] -= b_A[jA + 12] * B[3];
    }
  }

  if (B[2] != 0.0) {
    B[2] /= b_A[10];
    for (jA = 0; jA + 1 < 3; jA++) {
      B[jA] -= b_A[jA + 8] * B[2];
    }
  }

  if (B[1] != 0.0) {
    B[1] /= b_A[5];
    for (jA = 0; jA + 1 < 2; jA++) {
      B[jA] -= b_A[jA + 4] * B[1];
    }
  }

  if (B[0] != 0.0) {
    B[0] /= b_A[0];
  }
}

/* Function for MATLAB Function: '<Root>/Arm Dynamics' */
static void MatlabExcavatorModel_mldivide(const real_T A[9], const real_T B[3],
  real_T Y[3])
{
  real_T b_A[9];
  int32_T r;
  int32_T r_0;
  int32_T r_1;
  real_T maxval;
  real_T a;
  int32_T rtemp;
  memcpy(&b_A[0], &A[0], 9U * sizeof(real_T));
  r = 0;
  r_0 = 1;
  r_1 = 2;
  maxval = fabs(A[0]);
  a = fabs(A[1]);
  if (a > maxval) {
    maxval = a;
    r = 1;
    r_0 = 0;
  }

  if (fabs(A[2]) > maxval) {
    r = 2;
    r_0 = 1;
    r_1 = 0;
  }

  b_A[r_0] = A[r_0] / A[r];
  b_A[r_1] /= b_A[r];
  b_A[3 + r_0] -= b_A[3 + r] * b_A[r_0];
  b_A[3 + r_1] -= b_A[3 + r] * b_A[r_1];
  b_A[6 + r_0] -= b_A[6 + r] * b_A[r_0];
  b_A[6 + r_1] -= b_A[6 + r] * b_A[r_1];
  if (fabs(b_A[3 + r_1]) > fabs(b_A[3 + r_0])) {
    rtemp = r_0;
    r_0 = r_1;
    r_1 = rtemp;
  }

  b_A[3 + r_1] /= b_A[3 + r_0];
  b_A[6 + r_1] -= b_A[3 + r_1] * b_A[6 + r_0];
  Y[1] = B[r_0] - B[r] * b_A[r_0];
  Y[2] = (B[r_1] - B[r] * b_A[r_1]) - b_A[3 + r_1] * Y[1];
  Y[2] /= b_A[6 + r_1];
  Y[0] = B[r] - b_A[6 + r] * Y[2];
  Y[1] -= b_A[6 + r_0] * Y[2];
  Y[1] /= b_A[3 + r_0];
  Y[0] -= b_A[3 + r] * Y[1];
  Y[0] /= b_A[r];
}

/* Function for MATLAB Function: '<Root>/Arm Dynamics' */
static void MatlabExcavatorModel_mldivide_l(const real_T A[4], const real_T B[2],
  real_T Y[2])
{
  int32_T r;
  int32_T r_0;
  real_T a;
  if (fabs(A[1]) > fabs(A[0])) {
    r = 1;
    r_0 = 0;
  } else {
    r = 0;
    r_0 = 1;
  }

  a = A[r_0] / A[r];
  Y[1] = (B[r_0] - B[r] * a) / (A[2 + r_0] - A[2 + r] * a);
  Y[0] = (B[r] - A[2 + r] * Y[1]) / A[r];
}

/* Model step function */
void MatlabExcavatorModel_step(void)
{
  /* local block i/o variables */
  real_T rtb_Clock;
  real_T rtb_Transform1[4];
  real_T rtb_Sum[4];
  real_T rtb_totalfuel;
  real_T rtb_qddout[4];
  real_T d_engspd;
  real_T d_torq;
  real_T spdindex;
  real_T torq_reduce;
  real_T torq;
  real_T torqindex;
  real_T spdslope;
  int8_T limitq[4];
  real_T I05c[9];
  real_T A[16];
  real_T A_0[16];
  real_T A_1[16];
  real_T A_2[16];
  real_T A_3[16];
  real_T A_4[16];
  real_T A_5[16];
  real_T A_6[16];
  real_T onew[3];
  real_T A12R[9];
  real_T twow[3];
  real_T twow_0[3];
  real_T A23R[9];
  real_T thrw[3];
  real_T thrw_0[3];
  real_T A34R[9];
  real_T forw[3];
  real_T forw_0[3];
  real_T A04R[9];
  real_T A45R[9];
  real_T fivw[3];
  real_T fivw_0[3];
  real_T A05R[9];
  real_T fivF5c[3];
  real_T fivM5c[3];
  real_T twoalfa02qdd[3];
  real_T twoalfa02qddc[3];
  real_T thralfa03qdd[3];
  real_T thralfa03qddc[3];
  real_T foralfa04qdd[3];
  real_T foralfa04qdd_0[3];
  real_T fivalfa05qdd[3];
  real_T fivalfa05qdd_0[3];
  real_T fivalfa05qdd_1[3];
  real_T fivalfa05qddc[3];
  real_T twoa02qdd[3];
  real_T thra03qdd[3];
  real_T fora04qdd[3];
  real_T fora04qdd_0[3];
  real_T fora04qddc[3];
  real_T thrMc03qdd[3];
  real_T thrMc03qddc[3];
  real_T forMc04qdd[3];
  real_T forMc04qdd_0[3];
  real_T forMc04qddc[3];
  real_T fivMc05qdd[3];
  real_T fivMc05qdd_0[3];
  real_T fivMc05qdd_1[3];
  real_T fivMc05qdd_2[3];
  real_T fivMc05qddc[3];
  real_T twoF02qddc[3];
  real_T thrF03qddc[3];
  real_T forF04qdd[3];
  real_T forF04qdd_0[3];
  real_T forF04qdd_1[3];
  real_T fivF05qdd[3];
  real_T forF3c4qdd[3];
  real_T forF3c4qdd_0[3];
  real_T forF3c4qdd_1[3];
  real_T thrF2c3qdd[3];
  real_T thrF2c3qdd_0[3];
  real_T thrF2c3qdd_1[3];
  real_T thrF2c3qdd_2[3];
  real_T forM3c4qddc[3];
  real_T forM3c4qdd[3];
  real_T forM3c4qdd_0[3];
  real_T forM3c4qdd_1[3];
  real_T forM3c4qdd_2[3];
  real_T t3qdd;
  real_T t3qdd_0;
  real_T t3qdd_1;
  real_T t4qddc;
  real_T t4qdd;
  real_T t4qdd_0;
  real_T t4qdd_1;
  real_T t4qdd_2;
  real_T t5qddc;
  real_T t5qdd;
  real_T t5qdd_0;
  real_T t5qdd_1;
  real_T qddmatnew[4];
  real_T qddcmatnew[2];
  real_T taunew[2];
  int32_T b_max;
  int32_T max;
  int32_T max_0;
  static const real_T b[3] = { 0.0, 0.0, 1.0 };

  static const real_T c[3] = { 0.80117441999999994, 0.0, 0.0 };

  static const real_T d[3] = { 986.10822, 338.03864086799996, 0.0 };

  static const real_T e[3] = { 0.17287441999999997, -0.5043, 0.1554 };

  static const real_T oneMc01qdd[3] = { 29.1453, -10.001, 821.2951 };

  static const real_T f[3] = { 2.6416, 0.0, 0.0 };

  static const real_T thrF03qdd[3] = { -68.73035999999999, 265.86794, 0.0 };

  static const real_T g[3] = { 1.3949, 0.3606, -0.0953 };

  static const real_T thrMc03qdd_0[3] = { 0.1186, 0.0418, 127.3086 };

  static const real_T h[3] = { 1.3716, 0.0, 0.0 };

  static const real_T l[3] = { 0.51609999999999989, 0.0968, -0.0005 };

  static const real_T forMc04qdd_1[3] = { -0.0254, 0.0055, 37.5392 };

  static const real_T m[3] = { 0.26957781999999997, 0.2098, -0.001 };

  static const real_T o[3] = { 0.87917782, 0.0, 0.0 };

  static const real_T a[9] = { 14.1334, 1.1617, 0.1186, 1.1617, 115.4226, 0.0418,
    0.1186, 0.0418, 127.3086 };

  static const real_T b_a[9] = { 2.0862, 2.7973, -0.0254, 2.7973, 36.2725,
    0.0055, -0.0254, 0.0055, 37.5392 };

  static const int8_T c_a[9] = { 1, 0, 0, 0, 1, 0, 0, 0, 1 };

  static const real_T r[3] = { 0.14229587999999999, 0.0, 0.0 };

  static const real_T c_b[9] = { 10.7287, -1.4331, -0.0591, -1.4331, 23.1532,
    -0.0204, -0.0591, -0.0204, 21.7086 };

  static const real_T v[3] = { 0.133, -0.2861, 0.0389 };

  static const real_T w[3] = { -1.2467, 0.3606, -0.0953 };

  static const real_T x[3] = { -0.8555, 0.0968, -0.0005 };

  static const real_T y[3] = { -0.6096, 0.2098, -0.001 };

  static const real_T ab[3] = { 0.27529588, -0.2861, 0.0389 };

  static const real_T d_a[9] = { 541.038, -53.7206, 29.1453, -53.7206, 739.8405,
    -10.001, 29.1453, -10.001, 821.2951 };

  static const real_T bb[3] = { -0.6283, -0.5043, 0.1554 };

  static const real_T e_a[9] = { 9.1682, -0.4466, 1.9129, -0.4466, 8.4159, 0.353,
    1.9129, 0.353, 5.0713 };

  real_T cb[3];
  real_T rtb_TmpSignalConversionAtSFunct[4];
  real_T rtb_flow[4];
  int32_T i;
  real_T taunew_0[2];
  real_T thrw_1[3];
  real_T tmp[3];
  real_T tmp_0[3];
  real_T tmp_1[3];
  real_T tmp_2[3];
  real_T tmp_3[3];
  real_T tmp_4[3];
  real_T b_0[3];
  real_T tmp_5[3];
  real_T d_b[3];
  real_T tmp_6[9];
  real_T A05R_0[3];

  /* Clock: '<Root>/Clock' */
  rtb_Clock = MatlabExcavatorModel_M->Timing.t[0];

  /* Outport: '<Root>/Time' */
  MatlabExcavatorModel_Y.Time = rtb_Clock;

  /* DiscreteIntegrator: '<S4>/Integrator (qd to q)' */
  MatlabExcavatorModel_B.q[0] = MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[0];
  MatlabExcavatorModel_B.q[1] = MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[1];
  MatlabExcavatorModel_B.q[2] = MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[2];
  MatlabExcavatorModel_B.q[3] = MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[3];

  /* Gain: '<S4>/Transform1' incorporates:
   *  Delay: '<S4>/Integer Delay'
   */
  rtb_Transform1[0] = MatlabExcavatorModel_P.Transform1_Gain *
    MatlabExcavatorModel_DW.IntegerDelay_DSTATE[0];
  rtb_Transform1[1] = MatlabExcavatorModel_P.Transform1_Gain *
    MatlabExcavatorModel_DW.IntegerDelay_DSTATE[1];
  rtb_Transform1[2] = MatlabExcavatorModel_P.Transform1_Gain *
    MatlabExcavatorModel_DW.IntegerDelay_DSTATE[2];
  rtb_Transform1[3] = MatlabExcavatorModel_P.Transform1_Gain *
    MatlabExcavatorModel_DW.IntegerDelay_DSTATE[3];

  /* DiscreteIntegrator: '<S4>/Integrator (qdd to qd)' */
  if (((rtb_Transform1[0] > 0.0) &&
       (MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[0] <= 0)) ||
      ((rtb_Transform1[0] <= 0.0) &&
       (MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[0] == 1))) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0] =
      MatlabExcavatorModel_P.Integratorqddtoqd_IC[0];
  }

  if (((rtb_Transform1[1] > 0.0) &&
       (MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[1] <= 0)) ||
      ((rtb_Transform1[1] <= 0.0) &&
       (MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[1] == 1))) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1] =
      MatlabExcavatorModel_P.Integratorqddtoqd_IC[1];
  }

  if (((rtb_Transform1[2] > 0.0) &&
       (MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[2] <= 0)) ||
      ((rtb_Transform1[2] <= 0.0) &&
       (MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[2] == 1))) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2] =
      MatlabExcavatorModel_P.Integratorqddtoqd_IC[2];
  }

  if (((rtb_Transform1[3] > 0.0) &&
       (MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[3] <= 0)) ||
      ((rtb_Transform1[3] <= 0.0) &&
       (MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[3] == 1))) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3] =
      MatlabExcavatorModel_P.Integratorqddtoqd_IC[3];
  }

  if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0] >=
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[0]) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0] =
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[0];
  } else {
    if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0] <=
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[0]) {
      MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0] =
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[0];
    }
  }

  if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1] >=
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[1]) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1] =
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[1];
  } else {
    if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1] <=
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[1]) {
      MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1] =
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[1];
    }
  }

  if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2] >=
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[2]) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2] =
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[2];
  } else {
    if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2] <=
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[2]) {
      MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2] =
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[2];
    }
  }

  if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3] >=
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[3]) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3] =
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[3];
  } else {
    if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3] <=
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[3]) {
      MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3] =
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[3];
    }
  }

  MatlabExcavatorModel_B.qdot[0] =
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0];
  MatlabExcavatorModel_B.qdot[1] =
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1];
  MatlabExcavatorModel_B.qdot[2] =
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2];
  MatlabExcavatorModel_B.qdot[3] =
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3];

  /* End of DiscreteIntegrator: '<S4>/Integrator (qdd to qd)' */

  /* UnitDelay: '<Root>/Unit Delay1' */
  MatlabExcavatorModel_B.UnitDelay1[0] =
    MatlabExcavatorModel_DW.UnitDelay1_DSTATE[0];
  MatlabExcavatorModel_B.UnitDelay1[1] =
    MatlabExcavatorModel_DW.UnitDelay1_DSTATE[1];
  MatlabExcavatorModel_B.UnitDelay1[2] =
    MatlabExcavatorModel_DW.UnitDelay1_DSTATE[2];
  MatlabExcavatorModel_B.UnitDelay1[3] =
    MatlabExcavatorModel_DW.UnitDelay1_DSTATE[3];

  /* Level2 S-Function Block: '<S4>/limiter' (limiterg_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[0];
    ssSetOutputPortSignal(rts, 0, &MatlabExcavatorModel_Y.Q[0]);
    ssSetOutputPortSignal(rts, 1, &MatlabExcavatorModel_Y.Qd[0]);
    sfcnOutputs(rts, 1);
  }

  /* MATLAB Function: '<S7>/swashflow' incorporates:
   *  Constant: '<S7>/Max Disp per Revolution'
   *  Constant: '<S7>/Maximum Pump Flow (in^3)'
   *  Constant: '<S7>/Nm to inlb conversion'
   *  Constant: '<S7>/fuel rate  matrix (kg//hr)'
   *  Constant: '<S7>/in Nm1'
   *  Constant: '<S7>/in rpm1'
   *  Constant: '<S7>/in rpms1'
   *  Constant: '<S7>/in sec'
   *  Constant: '<S7>/max torq at set engine speed'
   *  Delay: '<S7>/Integer Delay1'
   *  Delay: '<S7>/Integer Delay2'
   *  Delay: '<S7>/Integer Delay3'
   *  Inport: '<Root>/Flow1'
   *  Inport: '<Root>/Flow2'
   *  Inport: '<Root>/Flow3'
   *  Inport: '<Root>/Flow4'
   *  SignalConversion: '<S9>/TmpSignal ConversionAt SFunction Inport9'
   */
  /* MATLAB Function 'Command to Flow/Torque limiter/swashflow': '<S9>:1' */
  /*  This function reads in swashplate data and interpolates and then calculates the flow */
  /* function [flow, totaltorq] = torq_limited( maxtorq, ) */
  /* '<S9>:1:6' */
  d_engspd = MatlabExcavatorModel_P.inrpms1_Value[1] -
    MatlabExcavatorModel_P.inrpms1_Value[0];

  /* '<S9>:1:7' */
  d_torq = MatlabExcavatorModel_P.inNm1_Value[1] -
    MatlabExcavatorModel_P.inNm1_Value[0];
  if (MatlabExcavatorModel_P.inrpm1_Value <=
      MatlabExcavatorModel_P.inrpms1_Value[0]) {
    /* '<S9>:1:9' */
    /* '<S9>:1:10' */
    spdindex = 1.0;
  } else if (MatlabExcavatorModel_P.inrpm1_Value >=
             MatlabExcavatorModel_P.inrpms1_Value[56]) {
    /* '<S9>:1:12' */
    /* '<S9>:1:13' */
    spdindex = 57.0;
  } else {
    /* '<S9>:1:15' */
    spdindex = floor((MatlabExcavatorModel_P.inrpm1_Value -
                      MatlabExcavatorModel_P.inrpms1_Value[0]) / d_engspd) + 1.0;
  }

  /* '<S9>:1:19' */
  rtb_flow[0] = MatlabExcavatorModel_U.Flow1;
  rtb_flow[1] = MatlabExcavatorModel_U.Flow2;
  rtb_flow[2] = MatlabExcavatorModel_U.Flow3;
  rtb_flow[3] = MatlabExcavatorModel_U.Flow4;

  /* '<S9>:1:20' */
  /* '<S9>:1:21' */
  /* '<S9>:1:22' */
  /* '<S9>:1:23' */
  /* '<S9>:1:24' */
  /* '<S9>:1:22' */
  /* '<S9>:1:23' */
  /* '<S9>:1:24' */
  /* '<S9>:1:22' */
  /* '<S9>:1:23' */
  /* '<S9>:1:24' */
  /* '<S9>:1:22' */
  /* '<S9>:1:23' */
  /* '<S9>:1:24' */
  /* '<S9>:1:22' */
  /*  calculate needed torq for current demand */
  /* '<S9>:1:27' */
  /* '<S9>:1:28' */
  /* '<S9>:1:29' */
  /* .001/8.851; */
  /* '<S9>:1:28' */
  /* '<S9>:1:29' */
  /* .001/8.851; */
  /* '<S9>:1:28' */
  /* '<S9>:1:29' */
  /* .001/8.851; */
  /* '<S9>:1:28' */
  /* '<S9>:1:29' */
  torq_reduce = ((MatlabExcavatorModel_U.Flow1 /
                  MatlabExcavatorModel_P.MaximumPumpFlowin3_Value[0] *
                  MatlabExcavatorModel_P.MaxDispperRevolution_Value *
                  (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[0] -
                   MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[0]) /
                  6.2831853071795862 + MatlabExcavatorModel_U.Flow2 /
                  MatlabExcavatorModel_P.MaximumPumpFlowin3_Value[1] *
                  MatlabExcavatorModel_P.MaxDispperRevolution_Value *
                  (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[1] -
                   MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[1]) /
                  6.2831853071795862) + MatlabExcavatorModel_U.Flow3 /
                 MatlabExcavatorModel_P.MaximumPumpFlowin3_Value[2] *
                 MatlabExcavatorModel_P.MaxDispperRevolution_Value *
                 (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[2] -
                  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[2]) /
                 6.2831853071795862) + MatlabExcavatorModel_U.Flow4 /
    MatlabExcavatorModel_P.MaximumPumpFlowin3_Value[3] *
    MatlabExcavatorModel_P.MaxDispperRevolution_Value *
    (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[3] -
     MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[3]) / 6.2831853071795862;

  /* .001/8.851; */
  /* '<S9>:1:28' */
  /* '<S9>:1:31' */
  if (torq_reduce > MatlabExcavatorModel_P.inNm1_Value[(int32_T)
      MatlabExcavatorModel_P.maxtorqatsetenginespeed_Value[(int32_T)spdindex - 1]
      - 1]) {
    /* '<S9>:1:33' */
    /* ( isnan(fuelmap(spdindex,torqindex)) ) % need to limit torque */
    /* torqindex = maxincol(spdindex); */
    /* torq = torqscale(torqindex); */
    /*  calculate % reduction rate based on torque limit */
    /* if (totaltorq > maxtorq) */
    /* '<S9>:1:39' */
    torq_reduce /= MatlabExcavatorModel_P.inNm1_Value[(int32_T)
      MatlabExcavatorModel_P.maxtorqatsetenginespeed_Value[(int32_T)spdindex - 1]
      - 1];

    /* else */
    /*    torq_reduce = 1; */
    /* end */
    /*  reduce flow evenly for all cylinders */
    /* '<S9>:1:45' */
    /* flowout(k) = q(k)/hp_reduce; */
    /* '<S9>:1:47' */
    rtb_flow[0] = MatlabExcavatorModel_U.Flow1 / torq_reduce;

    /* '<S9>:1:45' */
    /* flowout(k) = q(k)/hp_reduce; */
    /* '<S9>:1:47' */
    rtb_flow[1] = MatlabExcavatorModel_U.Flow2 / torq_reduce;

    /* '<S9>:1:45' */
    /* flowout(k) = q(k)/hp_reduce; */
    /* '<S9>:1:47' */
    rtb_flow[2] = MatlabExcavatorModel_U.Flow3 / torq_reduce;

    /* '<S9>:1:45' */
    /* flowout(k) = q(k)/hp_reduce; */
    /* '<S9>:1:47' */
    rtb_flow[3] = MatlabExcavatorModel_U.Flow4 / torq_reduce;

    /* '<S9>:1:45' */
  }

  /*  calculate limited torque (to check that it's < max torque) */
  /* '<S9>:1:51' */
  /* '<S9>:1:52' */
  /* '<S9>:1:53' */
  /* '<S9>:1:54' */
  /* '<S9>:1:55' */
  /* '<S9>:1:52' */
  /* '<S9>:1:53' */
  /* '<S9>:1:54' */
  /* '<S9>:1:55' */
  /* '<S9>:1:52' */
  /* '<S9>:1:53' */
  /* '<S9>:1:54' */
  /* '<S9>:1:55' */
  /* '<S9>:1:52' */
  /* '<S9>:1:53' */
  /* '<S9>:1:54' */
  /* '<S9>:1:55' */
  /* '<S9>:1:52' */
  /* '<S9>:1:58' */
  /* '<S9>:1:60' */
  torq = (((rtb_flow[0] / MatlabExcavatorModel_P.MaximumPumpFlowin3_Value[0] *
            MatlabExcavatorModel_P.MaxDispperRevolution_Value *
            (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[0] -
             MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[0]) /
            6.2831853071795862 + rtb_flow[1] /
            MatlabExcavatorModel_P.MaximumPumpFlowin3_Value[1] *
            MatlabExcavatorModel_P.MaxDispperRevolution_Value *
            (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[1] -
             MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[1]) /
            6.2831853071795862) + rtb_flow[2] /
           MatlabExcavatorModel_P.MaximumPumpFlowin3_Value[2] *
           MatlabExcavatorModel_P.MaxDispperRevolution_Value *
           (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[2] -
            MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[2]) /
           6.2831853071795862) + rtb_flow[3] /
          MatlabExcavatorModel_P.MaximumPumpFlowin3_Value[3] *
          MatlabExcavatorModel_P.MaxDispperRevolution_Value *
          (MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[3] -
           MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[3]) / 6.2831853071795862)
    / MatlabExcavatorModel_P.Nmtoinlbconversion_Value;
  if (torq <= 0.0) {
    /* '<S9>:1:62' */
    /* '<S9>:1:63' */
    torq = 0.0;
  }

  if (torq <= MatlabExcavatorModel_P.inNm1_Value[0]) {
    /* '<S9>:1:65' */
    /* '<S9>:1:66' */
    torqindex = 1.0;
  } else if (torq >= MatlabExcavatorModel_P.inNm1_Value[50]) {
    /* '<S9>:1:68' */
    /* '<S9>:1:69' */
    torqindex = 51.0;
  } else {
    /* '<S9>:1:71' */
    torqindex = floor((torq - MatlabExcavatorModel_P.inNm1_Value[0]) / d_torq) +
      1.0;
  }

  if (rtIsNaN(MatlabExcavatorModel_P.fuelratematrixkghr_Value[(((int32_T)
         torqindex - 1) * 57 + (int32_T)(spdindex + 1.0)) - 1])) {
    /* '<S9>:1:76' */
    spdslope = (MatlabExcavatorModel_P.fuelratematrixkghr_Value[(((int32_T)
      torqindex - 1) * 57 + (int32_T)spdindex) - 1] -
                MatlabExcavatorModel_P.fuelratematrixkghr_Value[(((int32_T)
      torqindex - 1) * 57 + (int32_T)(spdindex - 1.0)) - 1]) / d_engspd;
  } else {
    /* '<S9>:1:78' */
    spdslope = (MatlabExcavatorModel_P.fuelratematrixkghr_Value[(((int32_T)
      torqindex - 1) * 57 + (int32_T)(spdindex + 1.0)) - 1] -
                MatlabExcavatorModel_P.fuelratematrixkghr_Value[(((int32_T)
      torqindex - 1) * 57 + (int32_T)spdindex) - 1]) / d_engspd;
  }

  if (rtIsNaN(MatlabExcavatorModel_P.fuelratematrixkghr_Value[(((int32_T)
         (torqindex + 1.0) - 1) * 57 + (int32_T)spdindex) - 1])) {
    /* '<S9>:1:82' */
    torq_reduce = (MatlabExcavatorModel_P.fuelratematrixkghr_Value[(((int32_T)
      torqindex - 1) * 57 + (int32_T)spdindex) - 1] -
                   MatlabExcavatorModel_P.fuelratematrixkghr_Value[(((int32_T)
      (torqindex - 1.0) - 1) * 57 + (int32_T)spdindex) - 1]) / d_torq;
  } else {
    /* '<S9>:1:84' */
    torq_reduce = (MatlabExcavatorModel_P.fuelratematrixkghr_Value[(((int32_T)
      (torqindex + 1.0) - 1) * 57 + (int32_T)spdindex) - 1] -
                   MatlabExcavatorModel_P.fuelratematrixkghr_Value[(((int32_T)
      torqindex - 1) * 57 + (int32_T)spdindex) - 1]) / d_torq;
  }

  /* '<S9>:1:87' */
  /* '<S9>:1:88' */
  /* '<S9>:1:90' */
  torq_reduce = (MatlabExcavatorModel_P.fuelratematrixkghr_Value[(((int32_T)
    torqindex - 1) * 57 + (int32_T)spdindex) - 1] +
                 (MatlabExcavatorModel_P.inrpm1_Value -
                  MatlabExcavatorModel_P.inrpms1_Value[(int32_T)spdindex - 1]) *
                 spdslope) + (torq - MatlabExcavatorModel_P.inNm1_Value[(int32_T)
    torqindex - 1]) * torq_reduce;

  /* '<S9>:1:92' */
  rtb_totalfuel = torq_reduce * MatlabExcavatorModel_P.insec_Value / 3600.0 +
    MatlabExcavatorModel_DW.IntegerDelay3_DSTATE;

  /* Outport: '<Root>/FuelTotal' */
  MatlabExcavatorModel_Y.FuelTotal = rtb_totalfuel;

  /* Outport: '<Root>/FuelInstant' incorporates:
   *  MATLAB Function: '<S7>/swashflow'
   */
  MatlabExcavatorModel_Y.FuelInstant = torq_reduce;

  /* SignalConversion: '<S6>/TmpSignal ConversionAtS-FunctionInport1' */
  MatlabExcavatorModel_B.TmpSignalConversionAtSFunctionI[0] =
    MatlabExcavatorModel_Y.Q[0];
  MatlabExcavatorModel_B.TmpSignalConversionAtSFunctionI[1] =
    MatlabExcavatorModel_Y.Q[1];
  MatlabExcavatorModel_B.TmpSignalConversionAtSFunctionI[2] =
    MatlabExcavatorModel_Y.Q[2];
  MatlabExcavatorModel_B.TmpSignalConversionAtSFunctionI[3] =
    MatlabExcavatorModel_Y.Q[3];
  MatlabExcavatorModel_B.TmpSignalConversionAtSFunctionI[4] =
    MatlabExcavatorModel_Y.Qd[0];
  MatlabExcavatorModel_B.TmpSignalConversionAtSFunctionI[5] =
    MatlabExcavatorModel_Y.Qd[1];
  MatlabExcavatorModel_B.TmpSignalConversionAtSFunctionI[6] =
    MatlabExcavatorModel_Y.Qd[2];
  MatlabExcavatorModel_B.TmpSignalConversionAtSFunctionI[7] =
    MatlabExcavatorModel_Y.Qd[3];

  /* Level2 S-Function Block: '<S6>/S-Function' (j2c_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[1];
    sfcnOutputs(rts, 1);
  }

  /* ManualSwitch: '<S2>/Torque limit Switch' */
  if (MatlabExcavatorModel_P.TorquelimitSwitch_CurrentSettin == 1) {
    /* Outport: '<Root>/Flow' incorporates:
     *  Inport: '<Root>/Flow1'
     *  Inport: '<Root>/Flow2'
     *  Inport: '<Root>/Flow3'
     *  Inport: '<Root>/Flow4'
     */
    MatlabExcavatorModel_Y.Flow[0] = MatlabExcavatorModel_U.Flow1;
    MatlabExcavatorModel_Y.Flow[1] = MatlabExcavatorModel_U.Flow2;
    MatlabExcavatorModel_Y.Flow[2] = MatlabExcavatorModel_U.Flow3;
    MatlabExcavatorModel_Y.Flow[3] = MatlabExcavatorModel_U.Flow4;
  } else {
    /* Outport: '<Root>/Flow' */
    MatlabExcavatorModel_Y.Flow[0] = rtb_flow[0];
    MatlabExcavatorModel_Y.Flow[1] = rtb_flow[1];
    MatlabExcavatorModel_Y.Flow[2] = rtb_flow[2];
    MatlabExcavatorModel_Y.Flow[3] = rtb_flow[3];
  }

  /* End of ManualSwitch: '<S2>/Torque limit Switch' */

  /* Level2 S-Function Block: '<S3>/S-Function' (cylmodel_435_ryder) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[2];
    ssSetOutputPortSignal(rts, 2, &MatlabExcavatorModel_Y.Volume);
    sfcnOutputs(rts, 1);
  }

  /* Outport: '<Root>/BreakOn' incorporates:
   *  Delay: '<S4>/Integer Delay'
   */
  MatlabExcavatorModel_Y.BreakOn[0] =
    MatlabExcavatorModel_DW.IntegerDelay_DSTATE[0];
  MatlabExcavatorModel_Y.BreakOn[1] =
    MatlabExcavatorModel_DW.IntegerDelay_DSTATE[1];
  MatlabExcavatorModel_Y.BreakOn[2] =
    MatlabExcavatorModel_DW.IntegerDelay_DSTATE[2];
  MatlabExcavatorModel_Y.BreakOn[3] =
    MatlabExcavatorModel_DW.IntegerDelay_DSTATE[3];

  /* Outport: '<Root>/CylinderPosition' */
  MatlabExcavatorModel_Y.CylinderPosition[0] =
    MatlabExcavatorModel_B.SFunction_o1[0];
  MatlabExcavatorModel_Y.CylinderPosition[1] =
    MatlabExcavatorModel_B.SFunction_o1[1];
  MatlabExcavatorModel_Y.CylinderPosition[2] =
    MatlabExcavatorModel_B.SFunction_o1[2];
  MatlabExcavatorModel_Y.CylinderPosition[3] =
    MatlabExcavatorModel_B.SFunction_o1[3];

  /* Outport: '<Root>/CylinderVelocity' */
  MatlabExcavatorModel_Y.CylinderVelocity[0] =
    MatlabExcavatorModel_B.SFunction_o1[4];
  MatlabExcavatorModel_Y.CylinderVelocity[1] =
    MatlabExcavatorModel_B.SFunction_o1[5];
  MatlabExcavatorModel_Y.CylinderVelocity[2] =
    MatlabExcavatorModel_B.SFunction_o1[6];
  MatlabExcavatorModel_Y.CylinderVelocity[3] =
    MatlabExcavatorModel_B.SFunction_o1[7];

  /* Level2 S-Function Block: '<S5>/S-Function' (p2torq1_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[3];
    sfcnOutputs(rts, 1);
  }

  /* Level2 S-Function Block: '<S5>/S-Function1' (p2torq2_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[4];
    sfcnOutputs(rts, 1);
  }

  /* Level2 S-Function Block: '<S5>/S-Function2' (p2torq3_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[5];
    sfcnOutputs(rts, 1);
  }

  /* Level2 S-Function Block: '<S5>/S-Function3' (p2torq4_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[6];
    sfcnOutputs(rts, 1);
  }

  /* Gain: '<Root>/Transform3' */
  rtb_TmpSignalConversionAtSFunct[0] = MatlabExcavatorModel_P.Transform3_Gain *
    MatlabExcavatorModel_B.SFunction;
  rtb_TmpSignalConversionAtSFunct[1] = MatlabExcavatorModel_P.Transform3_Gain *
    MatlabExcavatorModel_B.SFunction1;
  rtb_TmpSignalConversionAtSFunct[2] = MatlabExcavatorModel_P.Transform3_Gain *
    MatlabExcavatorModel_B.SFunction2;
  rtb_TmpSignalConversionAtSFunct[3] = MatlabExcavatorModel_P.Transform3_Gain *
    MatlabExcavatorModel_B.SFunction3;

  /* MATLAB Function: '<Root>/Arm Dynamics' incorporates:
   *  Gain: '<Root>/Transform10'
   *  Gain: '<Root>/Transform2'
   *  Inport: '<Root>/Soil_Bucket_Mass'
   *  Inport: '<Root>/Soil_Dig_Force'
   *  Inport: '<Root>/Soil_Dig_Moment'
   *  Inport: '<Root>/Soil_Wrist_Force'
   */
  /* MATLAB Function 'Arm Dynamics': '<S1>:1' */
  /*  tau in Nm */
  /*  q in rad */
  /*  qd in rad/s */
  /* '<S1>:1:7' */
  /* q2 fixed straight ahead */
  /* '<S1>:1:8' */
  /* masses in kg */
  /*  link lengths in m */
  /* '<S1>:1:26' */
  /* '<S1>:1:28' */
  /* '<S1>:1:29' */
  /* '<S1>:1:30' */
  /* '<S1>:1:31' */
  /* '<S1>:1:32' */
  /* '<S1>:1:33' */
  /* -2.188768083034109 */
  /* '<S1>:1:34' */
  /* 0.13725870294037668 */
  /* '<S1>:1:35' */
  /* -2.2578906226452915 */
  /*  t1max = 0*pi/180; */
  /*  t1min = -90*pi/180; */
  /*  t2max = 0*pi/180; */
  /*  t2min = -90*pi/180; */
  /*  t3max = 0*pi/180; */
  /*  t3min = -90*pi/180; */
  /*  t4max = 0*pi/180;  */
  /*  t4min = -90*pi/180;  */
  /* '<S1>:1:46' */
  limitq[0] = 0;
  limitq[1] = 0;
  limitq[2] = 0;
  limitq[3] = 0;
  if (MatlabExcavatorModel_Y.Q[0] >= 1.5707963267948966) {
    /* '<S1>:1:47' */
    /*  - 1e-5)) */
    /* '<S1>:1:48' */
    limitq[0] = 1;
  }

  if (MatlabExcavatorModel_Y.Q[0] <= -1.5707963267948966) {
    /* '<S1>:1:50' */
    /*  + 1e-5)) */
    /* '<S1>:1:51' */
    limitq[0] = -1;
  }

  if (MatlabExcavatorModel_Y.Q[1] >= 1.1519173063162575) {
    /* '<S1>:1:53' */
    /*  - 1e-5)) */
    /* '<S1>:1:54' */
    limitq[1] = 1;
  }

  if (MatlabExcavatorModel_Y.Q[1] <= -1.0995574287564276) {
    /* '<S1>:1:56' */
    /*  + 1e-5)) */
    /* '<S1>:1:57' */
    limitq[1] = -1;
  }

  if (MatlabExcavatorModel_Y.Q[2] >= -0.38397243543875248) {
    /* '<S1>:1:59' */
    /*  - 1e-5)) */
    /* '<S1>:1:60' */
    limitq[2] = 1;
  }

  if (MatlabExcavatorModel_Y.Q[2] <= -2.5307274153917776) {
    /* '<S1>:1:62' */
    /*  + 1e-5)) */
    /* '<S1>:1:63' */
    limitq[2] = -1;
  }

  if (MatlabExcavatorModel_Y.Q[3] >= 0.57595865315812877) {
    /* '<S1>:1:65' */
    /*  - 1e-5)) */
    /* '<S1>:1:66' */
    limitq[3] = 1;
  }

  if (MatlabExcavatorModel_Y.Q[3] <= -2.6878070480712677) {
    /* '<S1>:1:68' */
    /*  + 1e-5)) */
    /* '<S1>:1:69' */
    limitq[3] = -1;
  }

  /*  %moments of inertia of a rod about it's center      */
  /*  I01c = m1*[0     0       0; */
  /*            0   1/12*a1^2  0; */
  /*            0     0   1/12*a1^2]; */
  /*  I02c = m2*[0     0       0; */
  /*            0   1/12*a2^2  0; */
  /*            0     0   1/12*a2^2]; */
  /*  I03c = m3*[0     0       0; */
  /*            0   1/12*a3^2  0; */
  /*            0     0   1/12*a3^2];         */
  /*  I04c = m4*[0     0       0; */
  /*            0   1/12*a4^2  0; */
  /*            0     0   1/12*a4^2];  */
  /*  I05c = m5*[0     0       0; */
  /*            0   1/12*a5^2  0; */
  /*            0     0   1/12*a5^2];  */
  /*   */
  /*  % moments of inertia for point mass about it's cg     */
  /*  I01c = zeros(3,3);       */
  /*  I02c = zeros(3,3);   */
  /*  I03c = zeros(3,3); */
  /*  I01c = ... */
  /*    [541.0380  -53.7206   29.1453; */
  /*    -53.7206  739.8405  -10.0010; */
  /*     29.1453  -10.0010  821.2951]; */
  /*  I02c = ... */
  /*     [9.1682   -0.4466    1.9129; */
  /*     -0.4466    8.4159    0.3530; */
  /*      1.9129    0.3530    5.0713]; */
  /*  I03c = ... */
  /*     [14.1334    1.1617    0.1186; */
  /*      1.1617  115.4226    0.0418; */
  /*      0.1186    0.0418  127.3086]; */
  /*  I04c = ... */
  /*     [2.0862    2.7973   -0.0254; */
  /*      2.7973   36.2725    0.0055; */
  /*     -0.0254    0.0055   37.5392]; */
  /*  I05c = ... */
  /*     [10.7287   -1.4331   -0.0591; */
  /*     -1.4331   23.1532   -0.0204; */
  /*     -0.0591   -0.0204   21.7086]; */
  /*  r = [-a1/2 0 0; */
  /*      -a2/2 0 0; */
  /*  	-a3/2 0 0; */
  /*  	-a4/2 0 0; */
  /*  	-a5/2 0 0]; */
  /*   */
  /*  % radii for new "point mass" links */
  /*  r(1,:) = [-3 2 1]; */
  /*  r(2,:) = [-a2 0 0]; */
  /*  r(3,:) = [1 0 1]; */
  /*  r(1,:) = [ -0.6283   -0.5043    0.1554]; */
  /*  r(2,:) = [  0.1330   -0.2861    0.0389]; */
  /*  r(3,:) = [ -1.2467    0.3606   -0.0953]; */
  /*  r(4,:) = [ -0.8555    0.0968   -0.0005]; */
  /*  r(5,:) = [ -0.6096    0.2098   -0.0010]; */
  /*  output of MOI_bobcat */
  /* '<S1>:1:159' */
  for (b_max = 0; b_max < 9; b_max++) {
    tmp_6[b_max] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * c_b[b_max];
  }

  MatlabExcavatorModel_rdivide(tmp_6, 207.1, I05c);

  /* '<S1>:1:163' */
  /* '<S1>:1:165' */
  A[0] = cos(MatlabExcavatorModel_Y.Q[0]);
  A[4] = -sin(MatlabExcavatorModel_Y.Q[0]);
  A[8] = 0.0;
  A[12] = 0.80117441999999994 * cos(MatlabExcavatorModel_Y.Q[0]);
  A[1] = sin(MatlabExcavatorModel_Y.Q[0]);
  A[5] = cos(MatlabExcavatorModel_Y.Q[0]);
  A[9] = 0.0;
  A[13] = 0.80117441999999994 * sin(MatlabExcavatorModel_Y.Q[0]);
  A[2] = 0.0;
  A[6] = 0.0;
  A[10] = 1.0;
  A[14] = 0.0;
  A[3] = 0.0;
  A[7] = 0.0;
  A[11] = 0.0;
  A[15] = 1.0;

  /* '<S1>:1:170' */
  A_0[0] = 0.99858058240698655;
  A_0[4] = 0.0;
  A_0[8] = 0.053261810330889073;
  A_0[12] = 0.14209390272451466;
  A_0[1] = 0.053261810330889073;
  A_0[5] = 0.0;
  A_0[9] = -0.99858058240698655;
  A_0[13] = 0.0075789361714269513;
  A_0[2] = 0.0;
  A_0[6] = 1.0;
  A_0[10] = 0.0;
  A_0[14] = 0.0;
  A_0[3] = 0.0;
  A_0[7] = 0.0;
  A_0[11] = 0.0;
  A_0[15] = 1.0;

  /* '<S1>:1:175' */
  A_1[0] = cos(MatlabExcavatorModel_Y.Q[1]);
  A_1[4] = -sin(MatlabExcavatorModel_Y.Q[1]);
  A_1[8] = 0.0;
  A_1[12] = 2.6416 * cos(MatlabExcavatorModel_Y.Q[1]);
  A_1[1] = sin(MatlabExcavatorModel_Y.Q[1]);
  A_1[5] = cos(MatlabExcavatorModel_Y.Q[1]);
  A_1[9] = 0.0;
  A_1[13] = 2.6416 * sin(MatlabExcavatorModel_Y.Q[1]);
  A_1[2] = 0.0;
  A_1[6] = 0.0;
  A_1[10] = 1.0;
  A_1[14] = 0.0;
  A_1[3] = 0.0;
  A_1[7] = 0.0;
  A_1[11] = 0.0;
  A_1[15] = 1.0;

  /* '<S1>:1:180' */
  A_2[0] = cos(MatlabExcavatorModel_Y.Q[2]);
  A_2[4] = -sin(MatlabExcavatorModel_Y.Q[2]);
  A_2[8] = 0.0;
  A_2[12] = 1.3716 * cos(MatlabExcavatorModel_Y.Q[2]);
  A_2[1] = sin(MatlabExcavatorModel_Y.Q[2]);
  A_2[5] = cos(MatlabExcavatorModel_Y.Q[2]);
  A_2[9] = 0.0;
  A_2[13] = 1.3716 * sin(MatlabExcavatorModel_Y.Q[2]);
  A_2[2] = 0.0;
  A_2[6] = 0.0;
  A_2[10] = 1.0;
  A_2[14] = 0.0;
  A_2[3] = 0.0;
  A_2[7] = 0.0;
  A_2[11] = 0.0;
  A_2[15] = 1.0;

  /* '<S1>:1:185' */
  A_3[0] = cos(MatlabExcavatorModel_Y.Q[3]);
  A_3[4] = -sin(MatlabExcavatorModel_Y.Q[3]);
  A_3[8] = 0.0;
  A_3[12] = 0.87917782 * cos(MatlabExcavatorModel_Y.Q[3]);
  A_3[1] = sin(MatlabExcavatorModel_Y.Q[3]);
  A_3[5] = cos(MatlabExcavatorModel_Y.Q[3]);
  A_3[9] = 0.0;
  A_3[13] = 0.87917782 * sin(MatlabExcavatorModel_Y.Q[3]);
  A_3[2] = 0.0;
  A_3[6] = 0.0;
  A_3[10] = 1.0;
  A_3[14] = 0.0;
  A_3[3] = 0.0;
  A_3[7] = 0.0;
  A_3[11] = 0.0;
  A_3[15] = 1.0;

  /* '<S1>:1:191' */
  for (b_max = 0; b_max < 4; b_max++) {
    for (i = 0; i < 4; i++) {
      A_4[b_max + (i << 2)] = 0.0;
      A_4[b_max + (i << 2)] += A_0[i << 2] * A[b_max];
      A_4[b_max + (i << 2)] += A_0[(i << 2) + 1] * A[b_max + 4];
      A_4[b_max + (i << 2)] += A_0[(i << 2) + 2] * A[b_max + 8];
      A_4[b_max + (i << 2)] += A_0[(i << 2) + 3] * A[b_max + 12];
    }
  }

  /* '<S1>:1:192' */
  for (b_max = 0; b_max < 4; b_max++) {
    for (i = 0; i < 4; i++) {
      A_5[b_max + (i << 2)] = 0.0;
      A_5[b_max + (i << 2)] += A_1[i << 2] * A_4[b_max];
      A_5[b_max + (i << 2)] += A_1[(i << 2) + 1] * A_4[b_max + 4];
      A_5[b_max + (i << 2)] += A_1[(i << 2) + 2] * A_4[b_max + 8];
      A_5[b_max + (i << 2)] += A_1[(i << 2) + 3] * A_4[b_max + 12];
    }
  }

  /* '<S1>:1:193' */
  for (b_max = 0; b_max < 4; b_max++) {
    for (i = 0; i < 4; i++) {
      A_6[b_max + (i << 2)] = 0.0;
      A_6[b_max + (i << 2)] += A_2[i << 2] * A_5[b_max];
      A_6[b_max + (i << 2)] += A_2[(i << 2) + 1] * A_5[b_max + 4];
      A_6[b_max + (i << 2)] += A_2[(i << 2) + 2] * A_5[b_max + 8];
      A_6[b_max + (i << 2)] += A_2[(i << 2) + 3] * A_5[b_max + 12];
    }
  }

  /* '<S1>:1:194' */
  /* '<S1>:1:196' */
  /* '<S1>:1:197' */
  /* '<S1>:1:199' */
  for (b_max = 0; b_max < 3; b_max++) {
    onew[b_max] = ((A[(b_max << 2) + 1] * 0.0 + A[b_max << 2] * 0.0) + A[(b_max <<
      2) + 2] * 0.0) + MatlabExcavatorModel_Y.Qd[0] * b[b_max];
  }

  /* '<S1>:1:202' */
  /* '<S1>:1:204' */
  for (b_max = 0; b_max < 3; b_max++) {
    A12R[3 * b_max] = A_0[b_max];
    A12R[1 + 3 * b_max] = A_0[b_max + 4];
    A12R[2 + 3 * b_max] = A_0[b_max + 8];
  }

  /* '<S1>:1:205' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A12R[b_max + 6] * onew[2] + (A12R[b_max + 3] * onew[1] +
      A12R[b_max] * onew[0]);
  }

  /* '<S1>:1:207' */
  twow_0[0] = twow[0];
  twow_0[1] = twow[1];
  twow_0[2] = twow[2];

  /* '<S1>:1:210' */
  /* '<S1>:1:212' */
  for (b_max = 0; b_max < 3; b_max++) {
    A23R[3 * b_max] = A_1[b_max];
    A23R[1 + 3 * b_max] = A_1[b_max + 4];
    A23R[2 + 3 * b_max] = A_1[b_max + 8];
  }

  /* '<S1>:1:213' */
  for (b_max = 0; b_max < 3; b_max++) {
    thrw[b_max] = A23R[b_max + 6] * twow[2] + (A23R[b_max + 3] * twow[1] +
      A23R[b_max] * twow[0]);
  }

  /* '<S1>:1:215' */
  thrw_0[0] = MatlabExcavatorModel_Y.Qd[1] * 0.0 + thrw[0];
  thrw_0[1] = MatlabExcavatorModel_Y.Qd[1] * 0.0 + thrw[1];
  thrw_0[2] = thrw[2] + MatlabExcavatorModel_Y.Qd[1];

  /* '<S1>:1:218' */
  /* '<S1>:1:220' */
  for (b_max = 0; b_max < 3; b_max++) {
    A34R[3 * b_max] = A_2[b_max];
    A34R[1 + 3 * b_max] = A_2[b_max + 4];
    A34R[2 + 3 * b_max] = A_2[b_max + 8];
  }

  /* '<S1>:1:221' */
  for (b_max = 0; b_max < 3; b_max++) {
    forw[b_max] = A34R[b_max + 6] * thrw_0[2] + (A34R[b_max + 3] * thrw_0[1] +
      A34R[b_max] * thrw_0[0]);
  }

  /* '<S1>:1:223' */
  forw_0[0] = MatlabExcavatorModel_Y.Qd[2] * 0.0 + forw[0];
  forw_0[1] = MatlabExcavatorModel_Y.Qd[2] * 0.0 + forw[1];
  forw_0[2] = forw[2] + MatlabExcavatorModel_Y.Qd[2];

  /* '<S1>:1:226' */
  for (b_max = 0; b_max < 3; b_max++) {
    A04R[3 * b_max] = A_6[b_max];
    A04R[1 + 3 * b_max] = A_6[b_max + 4];
    A04R[2 + 3 * b_max] = A_6[b_max + 8];
  }

  /* '<S1>:1:228' */
  for (b_max = 0; b_max < 3; b_max++) {
    A45R[3 * b_max] = A_3[b_max];
    A45R[1 + 3 * b_max] = A_3[b_max + 4];
    A45R[2 + 3 * b_max] = A_3[b_max + 8];
  }

  /* '<S1>:1:229' */
  for (b_max = 0; b_max < 3; b_max++) {
    fivw[b_max] = A45R[b_max + 6] * forw_0[2] + (A45R[b_max + 3] * forw_0[1] +
      A45R[b_max] * forw_0[0]);
  }

  /* '<S1>:1:231' */
  fivw_0[0] = MatlabExcavatorModel_Y.Qd[3] * 0.0 + fivw[0];
  fivw_0[1] = MatlabExcavatorModel_Y.Qd[3] * 0.0 + fivw[1];
  fivw_0[2] = fivw[2] + MatlabExcavatorModel_Y.Qd[3];

  /* '<S1>:1:234' */
  for (b_max = 0; b_max < 4; b_max++) {
    for (i = 0; i < 4; i++) {
      A_0[b_max + (i << 2)] = 0.0;
      A_0[b_max + (i << 2)] += A_3[b_max << 2] * A_6[i];
      A_0[b_max + (i << 2)] += A_3[(b_max << 2) + 1] * A_6[i + 4];
      A_0[b_max + (i << 2)] += A_3[(b_max << 2) + 2] * A_6[i + 8];
      A_0[b_max + (i << 2)] += A_3[(b_max << 2) + 3] * A_6[i + 12];
    }
  }

  for (b_max = 0; b_max < 3; b_max++) {
    A05R[3 * b_max] = A_0[b_max << 2];
    A05R[1 + 3 * b_max] = A_0[(b_max << 2) + 1];
    A05R[2 + 3 * b_max] = A_0[(b_max << 2) + 2];
  }

  /* '<S1>:1:237' */
  /* '<S1>:1:238' */
  /*  sixF5c6 = (A05R'*EEforce) */
  /*  sixM5c6 = (A05R'*EEmoment); */
  /* A05*[EEforce; 0] */
  /* '<S1>:1:244' */
  for (b_max = 0; b_max < 3; b_max++) {
    A05R_0[b_max] = A05R[b_max + 6] * (MatlabExcavatorModel_P.Transform2_Gain *
      MatlabExcavatorModel_U.Soil_Dig_Force[2]) + (A05R[b_max + 3] *
      (MatlabExcavatorModel_P.Transform2_Gain *
       MatlabExcavatorModel_U.Soil_Dig_Force[1]) +
      MatlabExcavatorModel_P.Transform2_Gain *
      MatlabExcavatorModel_U.Soil_Dig_Force[0] * A05R[b_max]);
  }

  for (b_max = 0; b_max < 3; b_max++) {
    fivF5c[b_max] = (real_T)c_a[b_max + 6] * A05R_0[2] + ((real_T)c_a[b_max + 3]
      * A05R_0[1] + (real_T)c_a[b_max] * A05R_0[0]);
  }

  /* '<S1>:1:245' */
  for (b_max = 0; b_max < 3; b_max++) {
    A05R_0[b_max] = A05R[b_max + 6] * (MatlabExcavatorModel_P.Transform10_Gain *
      MatlabExcavatorModel_U.Soil_Wrist_Force[2]) + (A05R[b_max + 3] *
      (MatlabExcavatorModel_P.Transform10_Gain *
       MatlabExcavatorModel_U.Soil_Wrist_Force[1]) +
      MatlabExcavatorModel_P.Transform10_Gain *
      MatlabExcavatorModel_U.Soil_Wrist_Force[0] * A05R[b_max]);
  }

  for (b_max = 0; b_max < 3; b_max++) {
    fivM5c[b_max] = (real_T)c_a[b_max + 6] * A05R_0[2] + ((real_T)c_a[b_max + 3]
      * A05R_0[1] + (real_T)c_a[b_max] * A05R_0[0]);
  }

  /*  fivM5c6qddc = A56R*EEmoment; */
  /* onealfa01 = qdd(1)*onekz0; */
  /* '<S1>:1:253' */
  for (b_max = 0; b_max < 3; b_max++) {
    twoalfa02qdd[b_max] = A12R[b_max + 6] + (A12R[b_max + 3] * 0.0 + A12R[b_max]
      * 0.0);
  }

  /* twoalfa02qdd2 = twokz1; */
  /* '<S1>:1:255' */
  d_b[0] = 0.0;
  d_b[1] = 0.0;
  d_b[2] = 0.0;
  MatlabExcavatorModel_cross(twow, d_b, twoalfa02qddc);

  /* twoalfa02 = twoalfa02qdd1*qdd(1) + twoalfa02qdd2*qdd(2)+twoalfa02qddc; */
  /* '<S1>:1:258' */
  for (b_max = 0; b_max < 3; b_max++) {
    thralfa03qdd[b_max] = A23R[b_max + 6] * twoalfa02qdd[2] + (A23R[b_max + 3] *
      twoalfa02qdd[1] + A23R[b_max] * twoalfa02qdd[0]);
  }

  /* thralfa03qdd2 = (A23R*twoalfa02qdd2); */
  /* '<S1>:1:261' */
  b_0[0] = 0.0 * MatlabExcavatorModel_Y.Qd[1];
  b_0[1] = 0.0 * MatlabExcavatorModel_Y.Qd[1];
  b_0[2] = MatlabExcavatorModel_Y.Qd[1];
  MatlabExcavatorModel_cross(thrw, b_0, tmp_5);
  for (b_max = 0; b_max < 3; b_max++) {
    thralfa03qddc[b_max] = ((A23R[b_max + 3] * twoalfa02qddc[1] + A23R[b_max] *
      twoalfa02qddc[0]) + A23R[b_max + 6] * twoalfa02qddc[2]) + tmp_5[b_max];
  }

  /* thralfa03 = thralfa03qdd1*qdd(1) + thralfa03qdd2*qdd(2) + thralfa03qdd3*qdd(3) + thralfa03qddc; */
  /* '<S1>:1:264' */
  for (b_max = 0; b_max < 3; b_max++) {
    foralfa04qdd[b_max] = A34R[b_max + 6] * thralfa03qdd[2] + (A34R[b_max + 3] *
      thralfa03qdd[1] + A34R[b_max] * thralfa03qdd[0]);
  }

  /* foralfa04qdd2 = (A34R*thralfa03qdd2); */
  /* '<S1>:1:266' */
  for (b_max = 0; b_max < 3; b_max++) {
    foralfa04qdd_0[b_max] = A34R[b_max + 6] + (A34R[b_max + 3] * 0.0 +
      A34R[b_max] * 0.0);
  }

  /* '<S1>:1:268' */
  forMc04qddc[0] = 0.0 * MatlabExcavatorModel_Y.Qd[2];
  forMc04qddc[1] = 0.0 * MatlabExcavatorModel_Y.Qd[2];
  forMc04qddc[2] = MatlabExcavatorModel_Y.Qd[2];
  MatlabExcavatorModel_cross(forw, forMc04qddc, tmp_5);
  for (b_max = 0; b_max < 3; b_max++) {
    forw[b_max] = ((A34R[b_max + 3] * thralfa03qddc[1] + A34R[b_max] *
                    thralfa03qddc[0]) + A34R[b_max + 6] * thralfa03qddc[2]) +
      tmp_5[b_max];
  }

  /* foralfa04 = foralfa04qdd1*qdd(1) + foralfa04qdd2*qdd(2) + foralfa04qdd3*qdd(3) + foralfa04qdd4*qdd(4) + foralfa04qddc; */
  /* '<S1>:1:271' */
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qdd[b_max] = A45R[b_max + 6] * foralfa04qdd[2] + (A45R[b_max + 3] *
      foralfa04qdd[1] + A45R[b_max] * foralfa04qdd[0]);
  }

  /* fivalfa05qdd2 = (A45R*foralfa04qdd2); */
  /* '<S1>:1:273' */
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qdd_0[b_max] = A45R[b_max + 6] * foralfa04qdd_0[2] + (A45R[b_max +
      3] * foralfa04qdd_0[1] + A45R[b_max] * foralfa04qdd_0[0]);
  }

  /* '<S1>:1:274' */
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qdd_1[b_max] = A45R[b_max + 6] + (A45R[b_max + 3] * 0.0 +
      A45R[b_max] * 0.0);
  }

  /* '<S1>:1:276' */
  fivalfa05qddc[0] = 0.0 * MatlabExcavatorModel_Y.Qd[3];
  fivalfa05qddc[1] = 0.0 * MatlabExcavatorModel_Y.Qd[3];
  fivalfa05qddc[2] = MatlabExcavatorModel_Y.Qd[3];
  MatlabExcavatorModel_cross(fivw, fivalfa05qddc, tmp_5);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = ((A45R[b_max + 3] * forw[1] + A45R[b_max] * forw[0])
      + A45R[b_max + 6] * forw[2]) + tmp_5[b_max];
  }

  /* fivalfa05 = fivalfa05qdd1*qdd(1) + fivalfa05qdd2*qdd(2) + fivalfa05qdd3*qdd(3) + fivalfa05qdd4*qdd(4) + fivalfa05qdd5*qdd(5) + fivalfa05qddc; */
  /* '<S1>:1:280' */
  MatlabExcavatorModel_cross_g(onew, c, tmp_5);
  MatlabExcavatorModel_cross_g(onew, tmp_5, cb);

  /* onea01 = qdd(1)*onea01qdd1 + onea01qddc; */
  /* '<S1>:1:283' */
  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = A12R[b_max + 6] + (A12R[b_max + 3] * 0.0 + A12R[b_max] * 0.0);
  }

  MatlabExcavatorModel_cross_g(d_b, r, tmp_5);
  for (b_max = 0; b_max < 3; b_max++) {
    twoa02qdd[b_max] = ((A12R[b_max + 3] * 0.80117441999999994 + A12R[b_max] *
                         -0.0) + A12R[b_max + 6] * 0.0) + tmp_5[b_max];
  }

  /* twoa02qdd2 = cross(twoalfa02qdd2,twopstar2)'; */
  /* '<S1>:1:285' */
  MatlabExcavatorModel_cross_g(twoalfa02qddc, r, tmp_5);
  MatlabExcavatorModel_cross_g(twow_0, r, tmp_3);
  MatlabExcavatorModel_cross_g(twow_0, tmp_3, tmp_4);
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = (((A12R[b_max + 3] * cb[1] + A12R[b_max] * cb[0]) + A12R[b_max
                    + 6] * cb[2]) + tmp_5[b_max]) + tmp_4[b_max];
  }

  /* twoa02 = twoa02qdd1*(qdd(1)) + twoa02qdd2*qdd(2) + twoa02qddc; */
  /* '<S1>:1:288' */
  MatlabExcavatorModel_cross_g(thralfa03qdd, f, tmp_5);
  for (b_max = 0; b_max < 3; b_max++) {
    thra03qdd[b_max] = ((A23R[b_max + 3] * twoa02qdd[1] + A23R[b_max] *
                         twoa02qdd[0]) + A23R[b_max + 6] * twoa02qdd[2]) +
      tmp_5[b_max];
  }

  /* thra03qdd2 = A23R*twoa02qdd2 + cross(thralfa03qdd2,thrpstar3)'; */
  /* '<S1>:1:291' */
  MatlabExcavatorModel_cross_g(thralfa03qddc, f, tmp_5);
  MatlabExcavatorModel_cross_g(thrw_0, f, tmp_3);
  MatlabExcavatorModel_cross_g(thrw_0, tmp_3, tmp_4);
  for (b_max = 0; b_max < 3; b_max++) {
    thrw[b_max] = (((A23R[b_max + 3] * twow[1] + A23R[b_max] * twow[0]) +
                    A23R[b_max + 6] * twow[2]) + tmp_5[b_max]) + tmp_4[b_max];
  }

  /* thra03 = thra03qdd1*qdd(1) + thra03qdd2*qdd(2) + thra03qdd3*qdd(3) + thra03qddc; */
  /* '<S1>:1:294' */
  MatlabExcavatorModel_cross_g(foralfa04qdd, h, tmp_5);
  for (b_max = 0; b_max < 3; b_max++) {
    fivw[b_max] = ((A34R[b_max + 3] * thra03qdd[1] + A34R[b_max] * thra03qdd[0])
                   + A34R[b_max + 6] * thra03qdd[2]) + tmp_5[b_max];
  }

  /* fora04qdd2 = A34R*thra03qdd2 + cross(foralfa04qdd2,forpstar4)'; */
  /* '<S1>:1:296' */
  MatlabExcavatorModel_cross_g(foralfa04qdd_0, h, tmp_5);
  for (b_max = 0; b_max < 3; b_max++) {
    fora04qdd[b_max] = ((A34R[b_max + 3] * 2.6416 + A34R[b_max] * 0.0) +
                        A34R[b_max + 6] * 0.0) + tmp_5[b_max];
  }

  /* '<S1>:1:297' */
  MatlabExcavatorModel_cross_g(foralfa04qdd_0, h, fora04qdd_0);

  /* '<S1>:1:298' */
  MatlabExcavatorModel_cross_g(forw, h, tmp_5);
  MatlabExcavatorModel_cross_g(forw_0, h, tmp_3);
  MatlabExcavatorModel_cross_g(forw_0, tmp_3, tmp_4);
  for (b_max = 0; b_max < 3; b_max++) {
    fora04qddc[b_max] = (((A34R[b_max + 3] * thrw[1] + A34R[b_max] * thrw[0]) +
                          A34R[b_max + 6] * thrw[2]) + tmp_5[b_max]) +
      tmp_4[b_max];
  }

  /* fora04 = fora04qdd1*qdd(1) + fora04qdd2*qdd(2) + fora04qdd3*qdd(3) + fora04qdd4*qdd(4) + fora04qddc; */
  /* '<S1>:1:301' */
  /* fiva05qdd2 = A45R*fora04qdd2 + cross(fivalfa05qdd2,fivpstar5)'; */
  /* '<S1>:1:303' */
  /* '<S1>:1:304' */
  /* '<S1>:1:305' */
  /* '<S1>:1:306' */
  /* fiva05 = fiva05qdd1*qdd(1) + fiva05qdd2*qdd(2) + fiva05qdd3*qdd(3) + fiva05qdd4*qdd(4) + fiva05qdd5*qdd(5) + fiva05qddc; */
  /* '<S1>:1:309' */
  /* fiva05Gqdd2 = fiva05qdd2 + cross(fivalfa05qdd2,fivpstar5G)'; */
  /* '<S1>:1:311' */
  /* '<S1>:1:312' */
  /* '<S1>:1:313' */
  /* '<S1>:1:314' */
  /* fiva05G = fiva05Gqdd1*qdd(1) + fiva05Gqdd2*qdd(2) + fiva05Gqdd3*qdd(3) + fiva05Gqdd4*qdd(4) + fiva05Gqdd5*qdd(5) + fiva05Gqddc; */
  /* '<S1>:1:317' */
  /* fora04Gqdd2 = fora04qdd2 + cross(foralfa04qdd2,forpstar4G)'; */
  /* '<S1>:1:319' */
  /* '<S1>:1:320' */
  /* '<S1>:1:321' */
  /* fora04G = fora04Gqdd1*qdd(1) + fora04Gqdd2*qdd(2) + fora04Gqdd3*qdd(3) + fora04Gqdd4*qdd(4) + fora04Gqddc; */
  /* '<S1>:1:324' */
  /* thra03Gqdd2 = thra03qdd2 + cross(thralfa03qdd2,thrpstar3G)'; */
  /* '<S1>:1:327' */
  /* thra03G = thra03Gqdd1*qdd(1) + thra03Gqdd2*qdd(2) + thra03Gqdd3*qdd(3) + thra03Gqddc; */
  /* '<S1>:1:330' */
  /* twoa02Gqdd2 = twoa02qdd2 + cross(twoalfa02qdd2,twopstar2G)'; */
  /* '<S1>:1:332' */
  /* twoa02G = twoa02Gqdd1*qdd(1) + twoa02Gqdd2*qdd(2) + twoa02Gqddc; */
  /* '<S1>:1:336' */
  /* onea01G = onea01Gqdd1*qdd(1) + onea01Gqddc; */
  /* '<S1>:1:339' */
  /* '<S1>:1:340' */
  /* '<S1>:1:341' */
  /* oneM01 = oneMc01qdd1*qdd(1) + oneMc01qddc; */
  /* '<S1>:1:344' */
  /* '<S1>:1:345' */
  /* twoMc02qdd2 = I02c*twoalfa02qdd2; */
  /* '<S1>:1:347' */
  /* twoM02 = twoMc02qdd1*qdd(1) + twoMc02qdd2*qdd(2) + twoMc02qddc; */
  /* '<S1>:1:350' */
  /* '<S1>:1:351' */
  for (b_max = 0; b_max < 3; b_max++) {
    thrMc03qdd[b_max] = a[b_max + 6] * thralfa03qdd[2] + (a[b_max + 3] *
      thralfa03qdd[1] + a[b_max] * thralfa03qdd[0]);
  }

  /* thrMc03qdd2 = I03c*thralfa03qdd2; */
  /* '<S1>:1:353' */
  /* '<S1>:1:354' */
  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = a[b_max + 6] * thrw_0[2] + (a[b_max + 3] * thrw_0[1] + a[b_max]
      * thrw_0[0]);
  }

  MatlabExcavatorModel_cross(thrw_0, d_b, tmp_5);
  for (b_max = 0; b_max < 3; b_max++) {
    thrMc03qddc[b_max] = ((a[b_max + 3] * thralfa03qddc[1] + a[b_max] *
      thralfa03qddc[0]) + a[b_max + 6] * thralfa03qddc[2]) + tmp_5[b_max];
  }

  /* thrM03 = thrMc03qdd1*qdd(1) + thrMc03qdd2*qdd(2) + thrMc03qdd3*qdd(3) + thrMc03qddc; */
  /* '<S1>:1:357' */
  /* '<S1>:1:358' */
  for (b_max = 0; b_max < 3; b_max++) {
    forMc04qdd[b_max] = b_a[b_max + 6] * foralfa04qdd[2] + (b_a[b_max + 3] *
      foralfa04qdd[1] + b_a[b_max] * foralfa04qdd[0]);
  }

  /* forMc04qdd2 = I04c*foralfa04qdd2; */
  /* '<S1>:1:360' */
  for (b_max = 0; b_max < 3; b_max++) {
    forMc04qdd_0[b_max] = b_a[b_max + 6] * foralfa04qdd_0[2] + (b_a[b_max + 3] *
      foralfa04qdd_0[1] + b_a[b_max] * foralfa04qdd_0[0]);
  }

  /* '<S1>:1:361' */
  /* '<S1>:1:362' */
  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = b_a[b_max + 6] * forw_0[2] + (b_a[b_max + 3] * forw_0[1] +
      b_a[b_max] * forw_0[0]);
  }

  MatlabExcavatorModel_cross(forw_0, d_b, tmp_5);
  for (b_max = 0; b_max < 3; b_max++) {
    forMc04qddc[b_max] = ((b_a[b_max + 3] * forw[1] + b_a[b_max] * forw[0]) +
                          b_a[b_max + 6] * forw[2]) + tmp_5[b_max];
  }

  /* forM04 = forMc04qdd1*qdd(1) + forMc04qdd2*qdd(2) + forMc04qdd3*qdd(3) + forMc04qdd4*qdd(4) + forMc04qddc; */
  /* '<S1>:1:365' */
  /* '<S1>:1:366' */
  for (b_max = 0; b_max < 3; b_max++) {
    fivMc05qdd[b_max] = I05c[b_max + 6] * fivalfa05qdd[2] + (I05c[b_max + 3] *
      fivalfa05qdd[1] + I05c[b_max] * fivalfa05qdd[0]);
  }

  /* fivMc05qdd2 = I05c*fivalfa05qdd2; */
  /* '<S1>:1:368' */
  for (b_max = 0; b_max < 3; b_max++) {
    fivMc05qdd_0[b_max] = I05c[b_max + 6] * fivalfa05qdd_0[2] + (I05c[b_max + 3]
      * fivalfa05qdd_0[1] + I05c[b_max] * fivalfa05qdd_0[0]);
  }

  /* '<S1>:1:369' */
  for (b_max = 0; b_max < 3; b_max++) {
    fivMc05qdd_1[b_max] = I05c[b_max + 6] * fivalfa05qdd_1[2] + (I05c[b_max + 3]
      * fivalfa05qdd_1[1] + I05c[b_max] * fivalfa05qdd_1[0]);
  }

  /* '<S1>:1:370' */
  for (b_max = 0; b_max < 3; b_max++) {
    fivMc05qdd_2[b_max] = I05c[b_max + 6] + (I05c[b_max + 3] * 0.0 + I05c[b_max]
      * 0.0);
  }

  /* '<S1>:1:371' */
  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = I05c[b_max + 6] * fivw_0[2] + (I05c[b_max + 3] * fivw_0[1] +
      I05c[b_max] * fivw_0[0]);
  }

  MatlabExcavatorModel_cross(fivw_0, d_b, tmp_5);
  for (b_max = 0; b_max < 3; b_max++) {
    fivMc05qddc[b_max] = ((I05c[b_max + 3] * fivalfa05qddc[1] + I05c[b_max] *
      fivalfa05qddc[0]) + I05c[b_max + 6] * fivalfa05qddc[2]) + tmp_5[b_max];
  }

  /* fivM05 = fivMc05qdd1*qdd(1) + fivMc05qdd2*qdd(2) + fivMc05qdd3*qdd(3) + fivMc05qdd4*qdd(4) + fivMc05qdd5*qdd(5) + fivMc05qddc; */
  /* '<S1>:1:375' */
  /* oneF01 = oneF01qdd1*qdd(1) + oneF01qddc; */
  /* '<S1>:1:378' */
  MatlabExcavatorModel_cross_g(twoalfa02qdd, v, tmp_5);
  twoa02qdd[0] = (twoa02qdd[0] + tmp_5[0]) * 141.1;
  twoa02qdd[1] = (twoa02qdd[1] + tmp_5[1]) * 141.1;
  twoa02qdd[2] = (twoa02qdd[2] + tmp_5[2]) * 141.1;

  /* twoF02qdd2 = m2*(twoa02Gqdd2); */
  /* '<S1>:1:380' */
  MatlabExcavatorModel_cross_g(twoalfa02qddc, v, tmp_5);
  MatlabExcavatorModel_cross_g(twow_0, v, tmp_3);
  MatlabExcavatorModel_cross_g(twow_0, tmp_3, tmp_4);
  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = ((A_4[(b_max << 2) + 1] * 0.0 + A_4[b_max << 2] * 0.0) + A_4
                  [(b_max << 2) + 2] * 9.81) + ((twow[b_max] + tmp_5[b_max]) +
      tmp_4[b_max]);
  }

  twoF02qddc[0] = 141.1 * d_b[0];
  twoF02qddc[1] = 141.1 * d_b[1];
  twoF02qddc[2] = 141.1 * d_b[2];

  /* twoF02 = twoF02qdd1*qdd(1) + twoF02qdd2*qdd(2) + twoF02qddc; */
  /* '<S1>:1:383' */
  MatlabExcavatorModel_cross_g(thralfa03qdd, w, tmp_5);
  thra03qdd[0] = (thra03qdd[0] + tmp_5[0]) * 190.6;
  thra03qdd[1] = (thra03qdd[1] + tmp_5[1]) * 190.6;
  thra03qdd[2] = (thra03qdd[2] + tmp_5[2]) * 190.6;

  /* thrF03qdd2 = m3*(thra03Gqdd2); */
  /* '<S1>:1:385' */
  /* '<S1>:1:386' */
  MatlabExcavatorModel_cross_g(thralfa03qddc, w, tmp_5);
  MatlabExcavatorModel_cross_g(thrw_0, w, tmp_3);
  MatlabExcavatorModel_cross_g(thrw_0, tmp_3, tmp_4);
  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = ((A_5[(b_max << 2) + 1] * 0.0 + A_5[b_max << 2] * 0.0) + A_5
                  [(b_max << 2) + 2] * 9.81) + ((thrw[b_max] + tmp_5[b_max]) +
      tmp_4[b_max]);
  }

  thrF03qddc[0] = 190.6 * d_b[0];
  thrF03qddc[1] = 190.6 * d_b[1];
  thrF03qddc[2] = 190.6 * d_b[2];

  /* thrF03 = thrF03qdd1*qdd(1) + thrF03qdd2*qdd(2) + thrF03qdd3*qdd(3) + thrF03qddc; */
  /* '<S1>:1:389' */
  MatlabExcavatorModel_cross_g(foralfa04qdd, x, tmp_5);
  forF04qdd[0] = (fivw[0] + tmp_5[0]) * 119.0;
  forF04qdd[1] = (fivw[1] + tmp_5[1]) * 119.0;
  forF04qdd[2] = (fivw[2] + tmp_5[2]) * 119.0;

  /* forF04qdd2 = m4*(fora04Gqdd2); */
  /* '<S1>:1:391' */
  MatlabExcavatorModel_cross_g(foralfa04qdd_0, x, tmp_5);
  forF04qdd_0[0] = (fora04qdd[0] + tmp_5[0]) * 119.0;
  forF04qdd_0[1] = (fora04qdd[1] + tmp_5[1]) * 119.0;
  forF04qdd_0[2] = (fora04qdd[2] + tmp_5[2]) * 119.0;

  /* '<S1>:1:392' */
  MatlabExcavatorModel_cross_g(b, x, tmp_5);
  forF04qdd_1[0] = (fora04qdd_0[0] + tmp_5[0]) * 119.0;
  forF04qdd_1[1] = (fora04qdd_0[1] + tmp_5[1]) * 119.0;
  forF04qdd_1[2] = (fora04qdd_0[2] + tmp_5[2]) * 119.0;

  /* '<S1>:1:393' */
  MatlabExcavatorModel_cross_g(forw, x, tmp_5);
  MatlabExcavatorModel_cross_g(forw_0, x, tmp_3);
  MatlabExcavatorModel_cross_g(forw_0, tmp_3, tmp_4);
  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = ((A04R[b_max + 3] * 0.0 + A04R[b_max] * 0.0) + A04R[b_max + 6] *
                  9.81) + ((fora04qddc[b_max] + tmp_5[b_max]) + tmp_4[b_max]);
  }

  forw_0[0] = 119.0 * d_b[0];
  forw_0[1] = 119.0 * d_b[1];
  forw_0[2] = 119.0 * d_b[2];

  /* forF04 = forF04qdd1*qdd(1) + forF04qdd2*qdd(2) + forF04qdd3*qdd(3) + forF04qdd4*qdd(4) + forF04qddc; */
  /* '<S1>:1:396' */
  MatlabExcavatorModel_cross_g(fivalfa05qdd, o, tmp_5);
  MatlabExcavatorModel_cross_g(fivalfa05qdd, y, tmp_3);
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = (((A45R[b_max + 3] * fivw[1] + A45R[b_max] * fivw[0]) +
                   A45R[b_max + 6] * fivw[2]) + tmp_5[b_max]) + tmp_3[b_max];
  }

  fivF05qdd[0] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * b_0[0];
  fivF05qdd[1] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * b_0[1];
  fivF05qdd[2] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * b_0[2];

  /* fivF05qdd2 = m5*(fiva05Gqdd2); */
  /* '<S1>:1:398' */
  MatlabExcavatorModel_cross_g(fivalfa05qdd_0, o, tmp_5);
  MatlabExcavatorModel_cross_g(fivalfa05qdd_0, y, tmp_3);
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = (((A45R[b_max + 3] * fora04qdd[1] + A45R[b_max] * fora04qdd[0])
                   + A45R[b_max + 6] * fora04qdd[2]) + tmp_5[b_max]) +
      tmp_3[b_max];
  }

  foralfa04qdd_0[0] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * b_0[0];
  foralfa04qdd_0[1] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * b_0[1];
  foralfa04qdd_0[2] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * b_0[2];

  /* '<S1>:1:399' */
  MatlabExcavatorModel_cross_g(fivalfa05qdd_1, o, tmp_5);
  MatlabExcavatorModel_cross_g(fivalfa05qdd_1, y, tmp_3);
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = (((A45R[b_max + 3] * fora04qdd_0[1] + A45R[b_max] *
                    fora04qdd_0[0]) + A45R[b_max + 6] * fora04qdd_0[2]) +
                  tmp_5[b_max]) + tmp_3[b_max];
  }

  fivalfa05qdd[0] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * b_0[0];
  fivalfa05qdd[1] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * b_0[1];
  fivalfa05qdd[2] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * b_0[2];

  /* '<S1>:1:400' */
  MatlabExcavatorModel_cross_g(fivalfa05qdd_1, o, tmp_5);
  MatlabExcavatorModel_cross_g(b, y, tmp_3);
  thrw_0[0] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * (tmp_5[0] +
    tmp_3[0]);
  thrw_0[1] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * (tmp_5[1] +
    tmp_3[1]);
  thrw_0[2] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * (tmp_5[2] +
    tmp_3[2]);

  /* '<S1>:1:401' */
  MatlabExcavatorModel_cross_g(fivalfa05qddc, o, tmp_5);
  MatlabExcavatorModel_cross_g(fivw_0, o, tmp_3);
  MatlabExcavatorModel_cross_g(fivw_0, tmp_3, tmp_4);
  MatlabExcavatorModel_cross_g(fivalfa05qddc, y, tmp_3);
  MatlabExcavatorModel_cross_g(fivw_0, y, tmp_1);
  MatlabExcavatorModel_cross_g(fivw_0, tmp_1, tmp_2);
  for (b_max = 0; b_max < 3; b_max++) {
    A05R_0[b_max] = A05R[b_max + 6] * 9.81 + (A05R[b_max + 3] * 0.0 + A05R[b_max]
      * 0.0);
  }

  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = (((((A45R[b_max + 3] * fora04qddc[1] + A45R[b_max] *
                      fora04qddc[0]) + A45R[b_max + 6] * fora04qddc[2]) +
                    tmp_5[b_max]) + tmp_4[b_max]) + tmp_3[b_max]) + tmp_2[b_max];
  }

  thrw[0] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * (A05R_0[0] +
    b_0[0]);
  thrw[1] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * (A05R_0[1] +
    b_0[1]);
  thrw[2] = (207.1 + MatlabExcavatorModel_U.Soil_Bucket_Mass) * (A05R_0[2] +
    b_0[2]);

  /* fivF05 = fivF05qdd1*qdd(1) + fivF05qdd2*qdd(2) + fivF05qdd3*qdd(3) + fivF05qdd4*qdd(4) + fivF05qdd5*qdd(5) + fivF05qddc; */
  /* '<S1>:1:404' */
  twow[0] = fivF5c[0] + thrw[0];
  twow[1] = fivF5c[1] + thrw[1];
  twow[2] = fivF5c[2] + thrw[2];

  /* fivF4c5qdd2 = fivF05qdd2; */
  /* fivF4c5 = fivF4c5qddc + fivF4c5qdd1*qdd(1) + fivF4c5qdd2*qdd(2) + fivF4c5qdd3*qdd(3) + fivF4c5qdd4*qdd(4) + fivF4c5qdd5*qdd(5); */
  /* '<S1>:1:412' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = ((A45R[3 * b_max + 1] * twow[1] + A45R[3 * b_max] * twow[0]) +
                  A45R[3 * b_max + 2] * twow[2]) + forw_0[b_max];
  }

  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = A04R[b_max + 6] * MatlabExcavatorModel_U.Soil_Dig_Moment[2] +
      (A04R[b_max + 3] * MatlabExcavatorModel_U.Soil_Dig_Moment[1] + A04R[b_max]
       * MatlabExcavatorModel_U.Soil_Dig_Moment[0]);
  }

  fivalfa05qddc[0] = b_0[0] + d_b[0];
  fivalfa05qddc[1] = b_0[1] + d_b[1];
  fivalfa05qddc[2] = b_0[2] + d_b[2];

  /* forF3c4qddc = transpose(A45R)*fivF4c5qddc + forF04qddc; */
  /* '<S1>:1:414' */
  for (b_max = 0; b_max < 3; b_max++) {
    fivw_0[b_max] = ((A45R[3 * b_max + 1] * fivF05qdd[1] + A45R[3 * b_max] *
                      fivF05qdd[0]) + A45R[3 * b_max + 2] * fivF05qdd[2]) +
      forF04qdd[b_max];
  }

  /* forF3c4qdd2 = transpose(A45R)*fivF4c5qdd2 + forF04qdd2; */
  /* '<S1>:1:416' */
  for (b_max = 0; b_max < 3; b_max++) {
    forF3c4qdd[b_max] = ((A45R[3 * b_max + 1] * foralfa04qdd_0[1] + A45R[3 *
                          b_max] * foralfa04qdd_0[0]) + A45R[3 * b_max + 2] *
                         foralfa04qdd_0[2]) + forF04qdd_0[b_max];
  }

  /* '<S1>:1:417' */
  for (b_max = 0; b_max < 3; b_max++) {
    forF3c4qdd_0[b_max] = ((A45R[3 * b_max + 1] * fivalfa05qdd[1] + A45R[3 *
      b_max] * fivalfa05qdd[0]) + A45R[3 * b_max + 2] * fivalfa05qdd[2]) +
      forF04qdd_1[b_max];
  }

  /* '<S1>:1:418' */
  for (b_max = 0; b_max < 3; b_max++) {
    forF3c4qdd_1[b_max] = A45R[3 * b_max + 2] * thrw_0[2] + (A45R[3 * b_max + 1]
      * thrw_0[1] + A45R[3 * b_max] * thrw_0[0]);
  }

  /* forF3c4 = forF3c4qddc + forF3c4qdd1*qdd(1) + forF3c4qdd2*qdd(2) + forF3c4qdd3*qdd(3) + forF3c4qdd4*qdd(4) + forF3c4qdd5*qdd(5); */
  /* '<S1>:1:421' */
  for (b_max = 0; b_max < 3; b_max++) {
    fora04qddc[b_max] = ((A34R[3 * b_max + 1] * fivalfa05qddc[1] + A34R[3 *
                          b_max] * fivalfa05qddc[0]) + A34R[3 * b_max + 2] *
                         fivalfa05qddc[2]) + thrF03qddc[b_max];
  }

  /* '<S1>:1:422' */
  for (b_max = 0; b_max < 3; b_max++) {
    thrF2c3qdd[b_max] = ((A34R[3 * b_max + 1] * fivw_0[1] + A34R[3 * b_max] *
                          fivw_0[0]) + A34R[3 * b_max + 2] * fivw_0[2]) +
      thra03qdd[b_max];
  }

  /* thrF2c3qdd2 = transpose(A34R)*forF3c4qdd2 + thrF03qdd2; */
  /* '<S1>:1:424' */
  for (b_max = 0; b_max < 3; b_max++) {
    thrF2c3qdd_0[b_max] = ((A34R[3 * b_max + 1] * forF3c4qdd[1] + A34R[3 * b_max]
      * forF3c4qdd[0]) + A34R[3 * b_max + 2] * forF3c4qdd[2]) + thrF03qdd[b_max];
  }

  /* '<S1>:1:425' */
  for (b_max = 0; b_max < 3; b_max++) {
    thrF2c3qdd_1[b_max] = A34R[3 * b_max + 2] * forF3c4qdd_0[2] + (A34R[3 *
      b_max + 1] * forF3c4qdd_0[1] + A34R[3 * b_max] * forF3c4qdd_0[0]);
  }

  /* '<S1>:1:426' */
  for (b_max = 0; b_max < 3; b_max++) {
    thrF2c3qdd_2[b_max] = A34R[3 * b_max + 2] * forF3c4qdd_1[2] + (A34R[3 *
      b_max + 1] * forF3c4qdd_1[1] + A34R[3 * b_max] * forF3c4qdd_1[0]);
  }

  /* thrF2c3 = thrF2c3qddc + thrF2c3qdd1*qdd(1) + thrF2c3qdd2*qdd(2) + thrF2c3qdd3*qdd(3) + thrF2c3qdd4*qdd(4) + thrF2c3qdd5*qdd(5); */
  /* '<S1>:1:429' */
  /* '<S1>:1:430' */
  /* twoF1c2qdd2 = transpose(A23R)*thrF2c3qdd2 + twoF02qdd2; */
  /* '<S1>:1:432' */
  /* '<S1>:1:433' */
  /* '<S1>:1:434' */
  /* twoF1c2 = twoF1c2qddc + twoF1c2qdd1*qdd(1) + twoF1c2qdd2*qdd(2) + twoF1c2qdd3*qdd(3) + twoF1c2qdd4*qdd(4) + twoF1c2qdd5*qdd(5); */
  /* '<S1>:1:437' */
  MatlabExcavatorModel_cross_gz(o, fivF5c, tmp_5);
  MatlabExcavatorModel_cross_gz(m, thrw, tmp_3);
  forw[0] = ((fivM5c[0] + fivMc05qddc[0]) + tmp_5[0]) + tmp_3[0];
  forw[1] = ((fivM5c[1] + fivMc05qddc[1]) + tmp_5[1]) + tmp_3[1];
  forw[2] = ((fivM5c[2] + fivMc05qddc[2]) + tmp_5[2]) + tmp_3[2];

  /* '<S1>:1:438' */
  MatlabExcavatorModel_cross_gz(m, fivF05qdd, tmp_5);
  fivw[0] = fivMc05qdd[0] + tmp_5[0];
  fivw[1] = fivMc05qdd[1] + tmp_5[1];
  fivw[2] = fivMc05qdd[2] + tmp_5[2];

  /* fivM4c5qdd2 = fivMc05qdd2 + cross((fivpstar5+fivpstar5G),fivF05qdd2)'; */
  /* '<S1>:1:440' */
  MatlabExcavatorModel_cross_gz(m, foralfa04qdd_0, tmp_5);
  thralfa03qddc[0] = fivMc05qdd_0[0] + tmp_5[0];
  thralfa03qddc[1] = fivMc05qdd_0[1] + tmp_5[1];
  thralfa03qddc[2] = fivMc05qdd_0[2] + tmp_5[2];

  /* '<S1>:1:441' */
  MatlabExcavatorModel_cross_gz(m, fivalfa05qdd, tmp_5);
  thralfa03qdd[0] = fivMc05qdd_1[0] + tmp_5[0];
  thralfa03qdd[1] = fivMc05qdd_1[1] + tmp_5[1];
  thralfa03qdd[2] = fivMc05qdd_1[2] + tmp_5[2];

  /* '<S1>:1:442' */
  MatlabExcavatorModel_cross_gz(m, thrw_0, tmp_5);
  foralfa04qdd[0] = fivMc05qdd_2[0] + tmp_5[0];
  foralfa04qdd[1] = fivMc05qdd_2[1] + tmp_5[1];
  foralfa04qdd[2] = fivMc05qdd_2[2] + tmp_5[2];

  /* fivM4c5 = fivM4c5qddc + fivM4c5qdd1*qdd(1) + fivM4c5qdd2*qdd(2) + fivM4c5qdd3*qdd(3) + fivM4c5qdd4*qdd(4) + fivM4c5qdd5*qdd(5); */
  /* '<S1>:1:445' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A45R[3 * b_max + 2] * twow[2] + (A45R[3 * b_max + 1] * twow[1]
      + A45R[3 * b_max] * twow[0]);
  }

  MatlabExcavatorModel_cross_gz(h, b_0, tmp_5);
  MatlabExcavatorModel_cross_gz(l, forw_0, tmp_3);
  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = A04R[b_max + 6] * MatlabExcavatorModel_U.Soil_Dig_Moment[2] +
      (A04R[b_max + 3] * MatlabExcavatorModel_U.Soil_Dig_Moment[1] + A04R[b_max]
       * MatlabExcavatorModel_U.Soil_Dig_Moment[0]);
  }

  MatlabExcavatorModel_cross_gz(h, d_b, tmp_4);
  for (b_max = 0; b_max < 3; b_max++) {
    forM3c4qddc[b_max] = (((((A45R[3 * b_max + 1] * forw[1] + A45R[3 * b_max] *
      forw[0]) + A45R[3 * b_max + 2] * forw[2]) + forMc04qddc[b_max]) +
      tmp_5[b_max]) + tmp_3[b_max]) + tmp_4[b_max];
  }

  /* forM3c4qddc = (A45R)'*fivM4c5qddc + forMc04qddc + cross(forpstar4,A45R'*fivF4c5qddc)' + cross((forpstar4+forpstar4G),forF04qddc)'; */
  /* '<S1>:1:448' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A45R[3 * b_max + 2] * fivF05qdd[2] + (A45R[3 * b_max + 1] *
      fivF05qdd[1] + A45R[3 * b_max] * fivF05qdd[0]);
  }

  MatlabExcavatorModel_cross_gz(h, b_0, tmp_5);
  MatlabExcavatorModel_cross_gz(l, forF04qdd, tmp_3);
  for (b_max = 0; b_max < 3; b_max++) {
    forM3c4qdd[b_max] = ((((A45R[3 * b_max + 1] * fivw[1] + A45R[3 * b_max] *
      fivw[0]) + A45R[3 * b_max + 2] * fivw[2]) + forMc04qdd[b_max]) +
                         tmp_5[b_max]) + tmp_3[b_max];
  }

  /* forM3c4qdd2 = (A45R)'*fivM4c5qdd2 + forMc04qdd2 + cross(forpstar4,A45R'*fivF4c5qdd2)' + cross((forpstar4+forpstar4G),forF04qdd2)'; */
  /* '<S1>:1:450' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A45R[3 * b_max + 2] * foralfa04qdd_0[2] + (A45R[3 * b_max + 1] *
      foralfa04qdd_0[1] + A45R[3 * b_max] * foralfa04qdd_0[0]);
  }

  MatlabExcavatorModel_cross_gz(h, b_0, tmp_5);
  MatlabExcavatorModel_cross_gz(l, forF04qdd_0, tmp_3);
  for (b_max = 0; b_max < 3; b_max++) {
    forM3c4qdd_0[b_max] = ((((A45R[3 * b_max + 1] * thralfa03qddc[1] + A45R[3 *
      b_max] * thralfa03qddc[0]) + A45R[3 * b_max + 2] * thralfa03qddc[2]) +
      forMc04qdd_0[b_max]) + tmp_5[b_max]) + tmp_3[b_max];
  }

  /* '<S1>:1:451' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A45R[3 * b_max + 2] * fivalfa05qdd[2] + (A45R[3 * b_max + 1] *
      fivalfa05qdd[1] + A45R[3 * b_max] * fivalfa05qdd[0]);
  }

  MatlabExcavatorModel_cross_gz(h, b_0, tmp_5);
  MatlabExcavatorModel_cross_gz(l, forF04qdd_1, tmp_3);
  for (b_max = 0; b_max < 3; b_max++) {
    forM3c4qdd_1[b_max] = ((((A45R[3 * b_max + 1] * thralfa03qdd[1] + A45R[3 *
      b_max] * thralfa03qdd[0]) + A45R[3 * b_max + 2] * thralfa03qdd[2]) +
      forMc04qdd_1[b_max]) + tmp_5[b_max]) + tmp_3[b_max];
  }

  /* '<S1>:1:452' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A45R[3 * b_max + 2] * thrw_0[2] + (A45R[3 * b_max + 1] *
      thrw_0[1] + A45R[3 * b_max] * thrw_0[0]);
  }

  MatlabExcavatorModel_cross_gz(h, b_0, tmp_5);
  for (b_max = 0; b_max < 3; b_max++) {
    forM3c4qdd_2[b_max] = ((A45R[3 * b_max + 1] * foralfa04qdd[1] + A45R[3 *
      b_max] * foralfa04qdd[0]) + A45R[3 * b_max + 2] * foralfa04qdd[2]) +
      tmp_5[b_max];
  }

  /* forM3c4 = forM3c4qddc + forM3c4qdd1*qdd(1) + forM3c4qdd2*qdd(2) + forM3c4qdd3*qdd(3) + forM3c4qdd4*qdd(4) + forM3c4qdd5*qdd(5); */
  /* '<S1>:1:455' */
  /* '<S1>:1:456' */
  /* thrM2c3qdd2 = A34R'*forM3c4qdd2 + thrMc03qdd2 + cross(thrpstar3,A34R'*forF3c4qdd2)' + cross((thrpstar3+thrpstar3G),thrF03qdd2)'; */
  /* '<S1>:1:458' */
  /* '<S1>:1:459' */
  /* '<S1>:1:460' */
  /* thrM2c3 = thrM2c3qddc + thrM2c3qdd1*qdd(1) + thrM2c3qdd2*qdd(2) + thrM2c3qdd3*qdd(3) + thrM2c3qdd4*qdd(4) + thrM2c3qdd5*qdd(5); */
  /* '<S1>:1:463' */
  /* '<S1>:1:464' */
  /* twoM1c2qdd2 = A23R'*thrM2c3qdd2 + twoMc02qdd2 + cross(twopstar2,A23R'*thrF2c3qdd2)' + cross((twopstar2+twopstar2G),twoF02qdd2)'; */
  /* '<S1>:1:466' */
  /* '<S1>:1:467' */
  /* '<S1>:1:468' */
  /* twoM1c2 = twoM1c2qddc + twoM1c2qdd1*qdd(1) + twoM1c2qdd2*qdd(2) + twoM1c2qdd3*qdd(3) + twoM1c2qdd4*qdd(4) + twoM1c2qdd5*qdd(5); */
  /* '<S1>:1:471' */
  MatlabExcavatorModel_cross_gz(o, fivF5c, tmp_5);
  MatlabExcavatorModel_cross_gz(m, thrw, tmp_3);
  fivM5c[0] = ((fivM5c[0] + fivMc05qddc[0]) + tmp_5[0]) + tmp_3[0];
  fivM5c[1] = ((fivM5c[1] + fivMc05qddc[1]) + tmp_5[1]) + tmp_3[1];
  t5qdd_1 = ((fivM5c[2] + fivMc05qddc[2]) + tmp_5[2]) + tmp_3[2];

  /* '<S1>:1:472' */
  MatlabExcavatorModel_cross_gz(m, thrw_0, tmp_5);
  fivMc05qdd_2[0] += tmp_5[0];
  fivMc05qdd_2[1] += tmp_5[1];

  /* '<S1>:1:473' */
  MatlabExcavatorModel_cross_gz(m, fivalfa05qdd, tmp_3);
  fivMc05qdd_1[0] += tmp_3[0];
  fivMc05qdd_1[1] += tmp_3[1];

  /* '<S1>:1:474' */
  MatlabExcavatorModel_cross_gz(m, foralfa04qdd_0, tmp_4);
  fivMc05qdd_0[0] += tmp_4[0];
  fivMc05qdd_0[1] += tmp_4[1];

  /* tau5qdd2 = fivMc05qdd2 + (cross((fivpstar5+fivpstar5G),fivF05qdd2))'; */
  /* '<S1>:1:476' */
  MatlabExcavatorModel_cross_gz(m, fivF05qdd, tmp_1);
  fivMc05qdd[0] += tmp_1[0];
  fivMc05qdd[1] += tmp_1[1];

  /*  tau5 = tau5qddc + tau5qdd1*qdd(1)+tau5qdd2*qdd(2)+tau5qdd3*qdd(3)+tau5qdd4*qdd(4)+tau5qdd5*qdd(5); */
  /* '<S1>:1:479' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A45R[3 * b_max + 2] * twow[2] + (A45R[3 * b_max + 1] * twow[1]
      + A45R[3 * b_max] * twow[0]);
  }

  MatlabExcavatorModel_cross_gz(h, b_0, tmp_2);
  MatlabExcavatorModel_cross_gz(l, forw_0, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    fivMc05qddc[b_max] = ((((A45R[3 * b_max + 1] * forw[1] + A45R[3 * b_max] *
      forw[0]) + A45R[3 * b_max + 2] * forw[2]) + forMc04qddc[b_max]) +
                          tmp_2[b_max]) + tmp_0[b_max];
  }

  /* '<S1>:1:480' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A45R[3 * b_max + 2] * fivF05qdd[2] + (A45R[3 * b_max + 1] *
      fivF05qdd[1] + A45R[3 * b_max] * fivF05qdd[0]);
  }

  MatlabExcavatorModel_cross_gz(h, b_0, tmp_2);
  MatlabExcavatorModel_cross_gz(l, forF04qdd, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qdd_1[b_max] = ((((A45R[3 * b_max + 1] * fivw[1] + A45R[3 * b_max] *
      fivw[0]) + A45R[3 * b_max + 2] * fivw[2]) + forMc04qdd[b_max]) +
      tmp_2[b_max]) + tmp_0[b_max];
  }

  /* tau4qdd2 = A45R'*fivM4c5qdd2 + forMc04qdd2 + cross(forpstar4,A45R'*fivF4c5qdd2)' + cross((forpstar4+forpstar4G),forF04qdd2)'; */
  /* '<S1>:1:482' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A45R[3 * b_max + 2] * foralfa04qdd_0[2] + (A45R[3 * b_max + 1] *
      foralfa04qdd_0[1] + A45R[3 * b_max] * foralfa04qdd_0[0]);
  }

  MatlabExcavatorModel_cross_gz(h, b_0, tmp_2);
  MatlabExcavatorModel_cross_gz(l, forF04qdd_0, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    fora04qdd_0[b_max] = ((((A45R[3 * b_max + 1] * thralfa03qddc[1] + A45R[3 *
      b_max] * thralfa03qddc[0]) + A45R[3 * b_max + 2] * thralfa03qddc[2]) +
      forMc04qdd_0[b_max]) + tmp_2[b_max]) + tmp_0[b_max];
  }

  /* '<S1>:1:483' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A45R[3 * b_max + 2] * fivalfa05qdd[2] + (A45R[3 * b_max + 1] *
      fivalfa05qdd[1] + A45R[3 * b_max] * fivalfa05qdd[0]);
  }

  MatlabExcavatorModel_cross_gz(h, b_0, tmp_2);
  MatlabExcavatorModel_cross_gz(l, forF04qdd_1, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    forw_0[b_max] = ((((A45R[3 * b_max + 1] * thralfa03qdd[1] + A45R[3 * b_max] *
                        thralfa03qdd[0]) + A45R[3 * b_max + 2] * thralfa03qdd[2])
                      + forMc04qdd_1[b_max]) + tmp_2[b_max]) + tmp_0[b_max];
  }

  /* '<S1>:1:484' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A45R[3 * b_max + 2] * thrw_0[2] + (A45R[3 * b_max + 1] *
      thrw_0[1] + A45R[3 * b_max] * thrw_0[0]);
  }

  MatlabExcavatorModel_cross_gz(h, b_0, tmp_2);
  for (b_max = 0; b_max < 3; b_max++) {
    fora04qdd[b_max] = ((A45R[3 * b_max + 1] * foralfa04qdd[1] + A45R[3 * b_max]
                         * foralfa04qdd[0]) + A45R[3 * b_max + 2] *
                        foralfa04qdd[2]) + tmp_2[b_max];
  }

  /*  tau4 = tau4qddc + tau4qdd1*qdd(1)+tau4qdd2*qdd(2)+tau4qdd3*qdd(3)+tau4qdd4*qdd(4)+tau4qdd5*qdd(5); */
  /* '<S1>:1:487' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A34R[3 * b_max + 2] * fivalfa05qddc[2] + (A34R[3 * b_max + 1] *
      fivalfa05qddc[1] + A34R[3 * b_max] * fivalfa05qddc[0]);
  }

  MatlabExcavatorModel_cross_gz(f, b_0, tmp_2);
  MatlabExcavatorModel_cross_gz(g, thrF03qddc, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    foralfa04qdd[b_max] = ((((A34R[3 * b_max + 1] * forM3c4qddc[1] + A34R[3 *
      b_max] * forM3c4qddc[0]) + A34R[3 * b_max + 2] * forM3c4qddc[2]) +
      thrMc03qddc[b_max]) + tmp_2[b_max]) + tmp_0[b_max];
  }

  /* '<S1>:1:488' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A34R[3 * b_max + 2] * fivw_0[2] + (A34R[3 * b_max + 1] *
      fivw_0[1] + A34R[3 * b_max] * fivw_0[0]);
  }

  MatlabExcavatorModel_cross_gz(f, b_0, tmp_2);
  MatlabExcavatorModel_cross_gz(g, thra03qdd, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    thrw_0[b_max] = ((((A34R[3 * b_max + 1] * forM3c4qdd[1] + A34R[3 * b_max] *
                        forM3c4qdd[0]) + A34R[3 * b_max + 2] * forM3c4qdd[2]) +
                      thrMc03qdd[b_max]) + tmp_2[b_max]) + tmp_0[b_max];
  }

  /* tau3qdd2 = A34R'*forM3c4qdd2 + thrMc03qdd2 + cross(thrpstar3,A34R'*forF3c4qdd2)' + cross((thrpstar3+thrpstar3G),thrF03qdd2)'; */
  /* '<S1>:1:490' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A34R[3 * b_max + 2] * forF3c4qdd[2] + (A34R[3 * b_max + 1] *
      forF3c4qdd[1] + A34R[3 * b_max] * forF3c4qdd[0]);
  }

  MatlabExcavatorModel_cross_gz(f, b_0, tmp_2);
  MatlabExcavatorModel_cross_gz(g, thrF03qdd, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    foralfa04qdd_0[b_max] = ((((A34R[3 * b_max + 1] * forM3c4qdd_0[1] + A34R[3 *
      b_max] * forM3c4qdd_0[0]) + A34R[3 * b_max + 2] * forM3c4qdd_0[2]) +
      thrMc03qdd_0[b_max]) + tmp_2[b_max]) + tmp_0[b_max];
  }

  /* '<S1>:1:491' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A34R[3 * b_max + 2] * forF3c4qdd_0[2] + (A34R[3 * b_max + 1] *
      forF3c4qdd_0[1] + A34R[3 * b_max] * forF3c4qdd_0[0]);
  }

  MatlabExcavatorModel_cross_gz(f, b_0, tmp_2);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qdd[b_max] = ((A34R[3 * b_max + 1] * forM3c4qdd_1[1] + A34R[3 *
      b_max] * forM3c4qdd_1[0]) + A34R[3 * b_max + 2] * forM3c4qdd_1[2]) +
      tmp_2[b_max];
  }

  /* '<S1>:1:492' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A34R[3 * b_max + 2] * forF3c4qdd_1[2] + (A34R[3 * b_max + 1] *
      forF3c4qdd_1[1] + A34R[3 * b_max] * forF3c4qdd_1[0]);
  }

  MatlabExcavatorModel_cross_gz(f, b_0, tmp_2);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qdd_0[b_max] = ((A34R[3 * b_max + 1] * forM3c4qdd_2[1] + A34R[3 *
      b_max] * forM3c4qdd_2[0]) + A34R[3 * b_max + 2] * forM3c4qdd_2[2]) +
      tmp_2[b_max];
  }

  /*  tau3 = tau3qddc + tau3qdd1*qdd(1)+tau3qdd2*qdd(2)+tau3qdd3*qdd(3)+tau3qdd4*qdd(4)+tau3qdd5*qdd(5); */
  /*  tau2qddc = A23R'*thrM2c3qddc + twoMc02qddc + cross(twopstar2,A23R'*thrF2c3qddc)' + cross((twopstar2+twopstar2G),twoF02qddc)'; */
  /*  tau2qdd1 = A23R'*thrM2c3qdd1 + twoMc02qdd1 + cross(twopstar2,A23R'*thrF2c3qdd1)' + cross((twopstar2+twopstar2G),twoF02qdd1)'; */
  /*  %tau2qdd2 = A23R'*thrM2c3qdd2 + twoMc02qdd2 + cross(twopstar2,A23R'*thrF2c3qdd2)' + cross((twopstar2+twopstar2G),twoF02qdd2)'; */
  /*  tau2qdd3 = A23R'*thrM2c3qdd3 + cross(twopstar2,A23R'*thrF2c3qdd3)'; */
  /*  tau2qdd4 = A23R'*thrM2c3qdd4 + cross(twopstar2,A23R'*thrF2c3qdd4)'; */
  /*  tau2qdd5 = A23R'*thrM2c3qdd5 + cross(twopstar2,A23R'*thrF2c3qdd5)'; */
  /*  tau2 = tau2qddc + tau2qdd1*qdd(1)+tau2qdd2*qdd(2)+tau2qdd3*qdd(3)+tau2qdd4*qdd(4)+tau2qdd5*qdd(5); */
  /* '<S1>:1:503' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A34R[3 * b_max + 2] * fivalfa05qddc[2] + (A34R[3 * b_max + 1] *
      fivalfa05qddc[1] + A34R[3 * b_max] * fivalfa05qddc[0]);
  }

  MatlabExcavatorModel_cross_gz(f, b_0, tmp_2);
  MatlabExcavatorModel_cross_gz(g, thrF03qddc, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = e_a[b_max + 6] * twow_0[2] + (e_a[b_max + 3] * twow_0[1] +
      e_a[b_max] * twow_0[0]);
  }

  MatlabExcavatorModel_cross(twow_0, b_0, forF04qdd);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = A23R[3 * b_max + 2] * fora04qddc[2] + (A23R[3 * b_max
      + 1] * fora04qddc[1] + A23R[3 * b_max] * fora04qddc[0]);
  }

  MatlabExcavatorModel_cross_gz(r, fivalfa05qddc, thrw);
  MatlabExcavatorModel_cross_gz(ab, twoF02qddc, A05R_0);
  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = d_a[b_max + 6] * onew[2] + (d_a[b_max + 3] * onew[1] +
      d_a[b_max] * onew[0]);
  }

  MatlabExcavatorModel_cross(onew, d_b, fivF5c);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = ((A23R[3 * b_max + 1] * fora04qddc[1] + A23R[3 *
      b_max] * fora04qddc[0]) + A23R[3 * b_max + 2] * fora04qddc[2]) +
      twoF02qddc[b_max];
  }

  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = A12R[3 * b_max + 2] * fivalfa05qddc[2] + (A12R[3 * b_max + 1] *
      fivalfa05qddc[1] + A12R[3 * b_max] * fivalfa05qddc[0]);
  }

  MatlabExcavatorModel_cross_gz(c, d_b, forMc04qddc);
  MatlabExcavatorModel_cross_g(onew, bb, d_b);
  MatlabExcavatorModel_cross_g(onew, d_b, b_0);
  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = ((A[(b_max << 2) + 1] * 0.0 + A[b_max << 2] * 0.0) + A[(b_max <<
      2) + 2] * 9.81) + (cb[b_max] + b_0[b_max]);
  }

  tmp[0] = 1955.4 * d_b[0];
  tmp[1] = 1955.4 * d_b[1];
  tmp[2] = 1955.4 * d_b[2];
  MatlabExcavatorModel_cross_gz(e, tmp, d_b);
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = ((((A34R[3 * b_max + 1] * forM3c4qddc[1] + A34R[3 * b_max] *
                     forM3c4qddc[0]) + A34R[3 * b_max + 2] * forM3c4qddc[2]) +
                   thrMc03qddc[b_max]) + tmp_2[b_max]) + tmp_0[b_max];
  }

  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = A23R[3 * b_max + 2] * b_0[2] + (A23R[3 * b_max + 1] *
      b_0[1] + A23R[3 * b_max] * b_0[0]);
  }

  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = ((e_a[b_max + 3] * twoalfa02qddc[1] + e_a[b_max] *
                   twoalfa02qddc[0]) + e_a[b_max + 6] * twoalfa02qddc[2]) +
      forF04qdd[b_max];
  }

  t5qddc = ((fivalfa05qddc[0] + b_0[0]) + thrw[0]) + A05R_0[0];
  t5qdd = ((fivalfa05qddc[1] + b_0[1]) + thrw[1]) + A05R_0[1];
  t5qdd_0 = ((fivalfa05qddc[2] + b_0[2]) + thrw[2]) + A05R_0[2];
  for (b_max = 0; b_max < 3; b_max++) {
    thralfa03qdd[b_max] = ((((A12R[3 * b_max + 1] * t5qdd + A12R[3 * b_max] *
      t5qddc) + A12R[3 * b_max + 2] * t5qdd_0) + fivF5c[b_max]) +
      forMc04qddc[b_max]) + d_b[b_max];
  }

  /* '<S1>:1:504' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A34R[3 * b_max + 2] * fivw_0[2] + (A34R[3 * b_max + 1] *
      fivw_0[1] + A34R[3 * b_max] * fivw_0[0]);
  }

  MatlabExcavatorModel_cross_gz(f, b_0, tmp_2);
  MatlabExcavatorModel_cross_gz(g, thra03qdd, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = A23R[3 * b_max + 2] * thrF2c3qdd[2] + (A23R[3 * b_max
      + 1] * thrF2c3qdd[1] + A23R[3 * b_max] * thrF2c3qdd[0]);
  }

  MatlabExcavatorModel_cross_gz(r, fivalfa05qddc, forF04qdd);
  MatlabExcavatorModel_cross_gz(ab, twoa02qdd, thrw);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = ((A23R[3 * b_max + 1] * thrF2c3qdd[1] + A23R[3 *
      b_max] * thrF2c3qdd[0]) + A23R[3 * b_max + 2] * thrF2c3qdd[2]) +
      twoa02qdd[b_max];
  }

  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = A12R[3 * b_max + 2] * fivalfa05qddc[2] + (A12R[3 * b_max + 1] *
      fivalfa05qddc[1] + A12R[3 * b_max] * fivalfa05qddc[0]);
  }

  MatlabExcavatorModel_cross_gz(c, d_b, A05R_0);
  MatlabExcavatorModel_cross_gz(e, d, fivF5c);
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = ((((A34R[3 * b_max + 1] * forM3c4qdd[1] + A34R[3 * b_max] *
                     forM3c4qdd[0]) + A34R[3 * b_max + 2] * forM3c4qdd[2]) +
                   thrMc03qdd[b_max]) + tmp_2[b_max]) + tmp_0[b_max];
  }

  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = A23R[3 * b_max + 2] * b_0[2] + (A23R[3 * b_max + 1] *
      b_0[1] + A23R[3 * b_max] * b_0[0]);
  }

  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = e_a[b_max + 6] * twoalfa02qdd[2] + (e_a[b_max + 3] *
      twoalfa02qdd[1] + e_a[b_max] * twoalfa02qdd[0]);
  }

  t5qddc = ((fivalfa05qddc[0] + b_0[0]) + forF04qdd[0]) + thrw[0];
  t5qdd = ((fivalfa05qddc[1] + b_0[1]) + forF04qdd[1]) + thrw[1];
  t5qdd_0 = ((fivalfa05qddc[2] + b_0[2]) + forF04qdd[2]) + thrw[2];
  for (b_max = 0; b_max < 3; b_max++) {
    thralfa03qddc[b_max] = ((((A12R[3 * b_max + 1] * t5qdd + A12R[3 * b_max] *
      t5qddc) + A12R[3 * b_max + 2] * t5qdd_0) + oneMc01qdd[b_max]) +
      A05R_0[b_max]) + fivF5c[b_max];
  }

  /* tau1qdd2 = A12R'*twoM1c2qdd2 + cross(onepstar1,A12R'*twoF1c2qdd2)'; */
  /* '<S1>:1:506' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A34R[3 * b_max + 2] * forF3c4qdd[2] + (A34R[3 * b_max + 1] *
      forF3c4qdd[1] + A34R[3 * b_max] * forF3c4qdd[0]);
  }

  MatlabExcavatorModel_cross_gz(f, b_0, tmp_2);
  MatlabExcavatorModel_cross_gz(g, thrF03qdd, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = A23R[3 * b_max + 2] * thrF2c3qdd_0[2] + (A23R[3 *
      b_max + 1] * thrF2c3qdd_0[1] + A23R[3 * b_max] * thrF2c3qdd_0[0]);
  }

  MatlabExcavatorModel_cross_gz(r, fivalfa05qddc, forF04qdd);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = A23R[3 * b_max + 2] * thrF2c3qdd_0[2] + (A23R[3 *
      b_max + 1] * thrF2c3qdd_0[1] + A23R[3 * b_max] * thrF2c3qdd_0[0]);
  }

  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = A12R[3 * b_max + 2] * fivalfa05qddc[2] + (A12R[3 * b_max + 1] *
      fivalfa05qddc[1] + A12R[3 * b_max] * fivalfa05qddc[0]);
  }

  MatlabExcavatorModel_cross_gz(c, d_b, thrw);
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = ((((A34R[3 * b_max + 1] * forM3c4qdd_0[1] + A34R[3 * b_max] *
                     forM3c4qdd_0[0]) + A34R[3 * b_max + 2] * forM3c4qdd_0[2]) +
                   thrMc03qdd_0[b_max]) + tmp_2[b_max]) + tmp_0[b_max];
  }

  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = ((A23R[3 * b_max + 1] * b_0[1] + A23R[3 * b_max] *
      b_0[0]) + A23R[3 * b_max + 2] * b_0[2]) + forF04qdd[b_max];
  }

  for (b_max = 0; b_max < 3; b_max++) {
    fivw[b_max] = ((A12R[3 * b_max + 1] * fivalfa05qddc[1] + A12R[3 * b_max] *
                    fivalfa05qddc[0]) + A12R[3 * b_max + 2] * fivalfa05qddc[2])
      + thrw[b_max];
  }

  /* '<S1>:1:507' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A34R[3 * b_max + 2] * forF3c4qdd_0[2] + (A34R[3 * b_max + 1] *
      forF3c4qdd_0[1] + A34R[3 * b_max] * forF3c4qdd_0[0]);
  }

  MatlabExcavatorModel_cross_gz(f, b_0, tmp_2);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = A23R[3 * b_max + 2] * thrF2c3qdd_1[2] + (A23R[3 *
      b_max + 1] * thrF2c3qdd_1[1] + A23R[3 * b_max] * thrF2c3qdd_1[0]);
  }

  MatlabExcavatorModel_cross_gz(r, fivalfa05qddc, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = A23R[3 * b_max + 2] * thrF2c3qdd_1[2] + (A23R[3 *
      b_max + 1] * thrF2c3qdd_1[1] + A23R[3 * b_max] * thrF2c3qdd_1[0]);
  }

  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = A12R[3 * b_max + 2] * fivalfa05qddc[2] + (A12R[3 * b_max + 1] *
      fivalfa05qddc[1] + A12R[3 * b_max] * fivalfa05qddc[0]);
  }

  MatlabExcavatorModel_cross_gz(c, d_b, forF04qdd);
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = ((A34R[3 * b_max + 1] * forM3c4qdd_1[1] + A34R[3 * b_max] *
                   forM3c4qdd_1[0]) + A34R[3 * b_max + 2] * forM3c4qdd_1[2]) +
      tmp_2[b_max];
  }

  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = ((A23R[3 * b_max + 1] * b_0[1] + A23R[3 * b_max] *
      b_0[0]) + A23R[3 * b_max + 2] * b_0[2]) + tmp_0[b_max];
  }

  for (b_max = 0; b_max < 3; b_max++) {
    forw[b_max] = ((A12R[3 * b_max + 1] * fivalfa05qddc[1] + A12R[3 * b_max] *
                    fivalfa05qddc[0]) + A12R[3 * b_max + 2] * fivalfa05qddc[2])
      + forF04qdd[b_max];
  }

  /* '<S1>:1:508' */
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = A34R[3 * b_max + 2] * forF3c4qdd_1[2] + (A34R[3 * b_max + 1] *
      forF3c4qdd_1[1] + A34R[3 * b_max] * forF3c4qdd_1[0]);
  }

  MatlabExcavatorModel_cross_gz(f, b_0, tmp_2);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = A23R[3 * b_max + 2] * thrF2c3qdd_2[2] + (A23R[3 *
      b_max + 1] * thrF2c3qdd_2[1] + A23R[3 * b_max] * thrF2c3qdd_2[0]);
  }

  MatlabExcavatorModel_cross_gz(r, fivalfa05qddc, tmp_0);
  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = A23R[3 * b_max + 2] * thrF2c3qdd_2[2] + (A23R[3 *
      b_max + 1] * thrF2c3qdd_2[1] + A23R[3 * b_max] * thrF2c3qdd_2[0]);
  }

  for (b_max = 0; b_max < 3; b_max++) {
    d_b[b_max] = A12R[3 * b_max + 2] * fivalfa05qddc[2] + (A12R[3 * b_max + 1] *
      fivalfa05qddc[1] + A12R[3 * b_max] * fivalfa05qddc[0]);
  }

  MatlabExcavatorModel_cross_gz(c, d_b, forF04qdd);
  for (b_max = 0; b_max < 3; b_max++) {
    b_0[b_max] = ((A34R[3 * b_max + 1] * forM3c4qdd_2[1] + A34R[3 * b_max] *
                   forM3c4qdd_2[0]) + A34R[3 * b_max + 2] * forM3c4qdd_2[2]) +
      tmp_2[b_max];
  }

  for (b_max = 0; b_max < 3; b_max++) {
    fivalfa05qddc[b_max] = ((A23R[3 * b_max + 1] * b_0[1] + A23R[3 * b_max] *
      b_0[0]) + A23R[3 * b_max + 2] * b_0[2]) + tmp_0[b_max];
  }

  for (b_max = 0; b_max < 3; b_max++) {
    thrw[b_max] = ((A12R[3 * b_max + 1] * fivalfa05qddc[1] + A12R[3 * b_max] *
                    fivalfa05qddc[0]) + A12R[3 * b_max + 2] * fivalfa05qddc[2])
      + forF04qdd[b_max];
  }

  /*  tau1 = tau1qddc + tau1qdd1*qdd(1)+tau1qdd2*qdd(2)+tau1qdd3*qdd(3)+tau1qdd4*qdd(4)+tau1qdd5*qdd(5); */
  /* '<S1>:1:511' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A[(b_max << 2) + 2] + (A[(b_max << 2) + 1] * 0.0 + A[b_max <<
      2] * 0.0);
  }

  torq_reduce = (thralfa03qdd[0] * twow[0] + thralfa03qdd[1] * twow[1]) +
    thralfa03qdd[2] * twow[2];

  /* '<S1>:1:512' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A[(b_max << 2) + 2] + (A[(b_max << 2) + 1] * 0.0 + A[b_max <<
      2] * 0.0);
  }

  spdslope = (thralfa03qddc[0] * twow[0] + thralfa03qddc[1] * twow[1]) +
    thralfa03qddc[2] * twow[2];

  /* t1qdd2 = tau1qdd2'*(A01(1:3,1:3)'*k(:,1)); */
  /* '<S1>:1:514' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A[(b_max << 2) + 2] + (A[(b_max << 2) + 1] * 0.0 + A[b_max <<
      2] * 0.0);
  }

  d_engspd = (fivw[0] * twow[0] + fivw[1] * twow[1]) + fivw[2] * twow[2];

  /* '<S1>:1:515' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A[(b_max << 2) + 2] + (A[(b_max << 2) + 1] * 0.0 + A[b_max <<
      2] * 0.0);
  }

  torq = (forw[0] * twow[0] + forw[1] * twow[1]) + forw[2] * twow[2];

  /* '<S1>:1:516' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A[(b_max << 2) + 2] + (A[(b_max << 2) + 1] * 0.0 + A[b_max <<
      2] * 0.0);
  }

  torqindex = (thrw[0] * twow[0] + thrw[1] * twow[1]) + thrw[2] * twow[2];

  /*  t1 = t1qddc + t1qdd1*qdd(1)+t1qdd2*qdd(2)+t1qdd3*qdd(3)+t1qdd4*qdd(4)+t1qdd5*qdd(5); */
  /*  t2qddc = tau2qddc'*(A12(1:3,1:3)'*k(:,1)); */
  /*  t2qdd1 = tau2qdd1'*(A12(1:3,1:3)'*k(:,1)); */
  /*  t2qdd2 = tau2qdd2'*(A12(1:3,1:3)'*k(:,1)); */
  /*  t2qdd3 = tau2qdd3'*(A12(1:3,1:3)'*k(:,1)); */
  /*  t2qdd4 = tau2qdd4'*(A12(1:3,1:3)'*k(:,1)); */
  /*  t2qdd5 = tau2qdd5'*(A12(1:3,1:3)'*k(:,1)); */
  /*  t2 = t2qddc + t2qdd1*qdd(1)+t2qdd2*qdd(2)+t2qdd3*qdd(3)+t2qdd4*qdd(4)+t2qdd5*qdd(5); */
  /* '<S1>:1:527' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_1[(b_max << 2) + 2] + (A_1[(b_max << 2) + 1] * 0.0 +
      A_1[b_max << 2] * 0.0);
  }

  d_torq = (foralfa04qdd[0] * twow[0] + foralfa04qdd[1] * twow[1]) +
    foralfa04qdd[2] * twow[2];

  /* '<S1>:1:528' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_1[(b_max << 2) + 2] + (A_1[(b_max << 2) + 1] * 0.0 +
      A_1[b_max << 2] * 0.0);
  }

  spdindex = (thrw_0[0] * twow[0] + thrw_0[1] * twow[1]) + thrw_0[2] * twow[2];

  /* t3qdd2 = tau3qdd2'*(A23(1:3,1:3)'*k(:,1)); */
  /* '<S1>:1:530' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_1[(b_max << 2) + 2] + (A_1[(b_max << 2) + 1] * 0.0 +
      A_1[b_max << 2] * 0.0);
  }

  t3qdd = (foralfa04qdd_0[0] * twow[0] + foralfa04qdd_0[1] * twow[1]) +
    foralfa04qdd_0[2] * twow[2];

  /* '<S1>:1:531' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_1[(b_max << 2) + 2] + (A_1[(b_max << 2) + 1] * 0.0 +
      A_1[b_max << 2] * 0.0);
  }

  t3qdd_0 = (fivalfa05qdd[0] * twow[0] + fivalfa05qdd[1] * twow[1]) +
    fivalfa05qdd[2] * twow[2];

  /* '<S1>:1:532' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_1[(b_max << 2) + 2] + (A_1[(b_max << 2) + 1] * 0.0 +
      A_1[b_max << 2] * 0.0);
  }

  t3qdd_1 = (fivalfa05qdd_0[0] * twow[0] + fivalfa05qdd_0[1] * twow[1]) +
    fivalfa05qdd_0[2] * twow[2];

  /*  t3 = t3qddc + t3qdd1*qdd(1)+t3qdd2*qdd(2)+t3qdd3*qdd(3)+t3qdd4*qdd(4)+t3qdd5*qdd(5); */
  /* '<S1>:1:535' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_2[(b_max << 2) + 2] + (A_2[(b_max << 2) + 1] * 0.0 +
      A_2[b_max << 2] * 0.0);
  }

  t4qddc = (fivMc05qddc[0] * twow[0] + fivMc05qddc[1] * twow[1]) + fivMc05qddc[2]
    * twow[2];

  /* '<S1>:1:536' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_2[(b_max << 2) + 2] + (A_2[(b_max << 2) + 1] * 0.0 +
      A_2[b_max << 2] * 0.0);
  }

  t4qdd = (fivalfa05qdd_1[0] * twow[0] + fivalfa05qdd_1[1] * twow[1]) +
    fivalfa05qdd_1[2] * twow[2];

  /* t4qdd2 = tau4qdd2'*(A34(1:3,1:3)'*k(:,1)); */
  /* '<S1>:1:538' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_2[(b_max << 2) + 2] + (A_2[(b_max << 2) + 1] * 0.0 +
      A_2[b_max << 2] * 0.0);
  }

  t4qdd_0 = (fora04qdd_0[0] * twow[0] + fora04qdd_0[1] * twow[1]) + fora04qdd_0
    [2] * twow[2];

  /* '<S1>:1:539' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_2[(b_max << 2) + 2] + (A_2[(b_max << 2) + 1] * 0.0 +
      A_2[b_max << 2] * 0.0);
  }

  t4qdd_1 = (forw_0[0] * twow[0] + forw_0[1] * twow[1]) + forw_0[2] * twow[2];

  /* '<S1>:1:540' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_2[(b_max << 2) + 2] + (A_2[(b_max << 2) + 1] * 0.0 +
      A_2[b_max << 2] * 0.0);
  }

  t4qdd_2 = (fora04qdd[0] * twow[0] + fora04qdd[1] * twow[1]) + fora04qdd[2] *
    twow[2];

  /*  t4 = t4qddc + t4qdd1*qdd(1)+t4qdd2*qdd(2)+t4qdd3*qdd(3)+t4qdd4*qdd(4)+t4qdd5*qdd(5);t3qddc = tau3qddc'*(A23(1:3,1:3)'*k(:,1)); */
  /* '<S1>:1:543' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_3[(b_max << 2) + 2] + (A_3[(b_max << 2) + 1] * 0.0 +
      A_3[b_max << 2] * 0.0);
  }

  t5qddc = (fivM5c[0] * twow[0] + fivM5c[1] * twow[1]) + t5qdd_1 * twow[2];

  /* '<S1>:1:544' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_3[(b_max << 2) + 2] + (A_3[(b_max << 2) + 1] * 0.0 +
      A_3[b_max << 2] * 0.0);
  }

  t5qdd = (fivMc05qdd[0] * twow[0] + fivMc05qdd[1] * twow[1]) + (fivMc05qdd[2] +
    tmp_1[2]) * twow[2];

  /* t5qdd2 = tau5qdd2'*(A45(1:3,1:3)'*k(:,1)); */
  /* '<S1>:1:546' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_3[(b_max << 2) + 2] + (A_3[(b_max << 2) + 1] * 0.0 +
      A_3[b_max << 2] * 0.0);
  }

  t5qdd_0 = (fivMc05qdd_0[0] * twow[0] + fivMc05qdd_0[1] * twow[1]) +
    (fivMc05qdd_0[2] + tmp_4[2]) * twow[2];

  /* '<S1>:1:547' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_3[(b_max << 2) + 2] + (A_3[(b_max << 2) + 1] * 0.0 +
      A_3[b_max << 2] * 0.0);
  }

  t5qdd_1 = (fivMc05qdd_1[0] * twow[0] + fivMc05qdd_1[1] * twow[1]) +
    (fivMc05qdd_1[2] + tmp_3[2]) * twow[2];

  /* '<S1>:1:548' */
  for (b_max = 0; b_max < 3; b_max++) {
    twow[b_max] = A_3[(b_max << 2) + 2] + (A_3[(b_max << 2) + 1] * 0.0 +
      A_3[b_max << 2] * 0.0);
  }

  /*  t5 = t5qddc + t5qdd1*qdd(1)+t5qdd2*qdd(2)+t5qdd3*qdd(3)+t5qdd4*qdd(4)+t5qdd5*qdd(5);t3qddc = tau3qddc'*(A23(1:3,1:3)'*k(:,1)); */
  /* eliminate the qdd2 terms since offset doesn't move in normal motion */
  /* t2qdd1  t2qdd3 t2qdd4 t2qdd5; */
  /* '<S1>:1:552' */
  A[0] = spdslope;
  A[4] = d_engspd;
  A[8] = torq;
  A[12] = torqindex;
  A[1] = spdindex;
  A[5] = t3qdd;
  A[9] = t3qdd_0;
  A[13] = t3qdd_1;
  A[2] = t4qdd;
  A[6] = t4qdd_0;
  A[10] = t4qdd_1;
  A[14] = t4qdd_2;
  A[3] = t5qdd;
  A[7] = t5qdd_0;
  A[11] = t5qdd_1;
  A[15] = (fivMc05qdd_2[0] * twow[0] + fivMc05qdd_2[1] * twow[1]) +
    (fivMc05qdd_2[2] + tmp_5[2]) * twow[2];

  /* qddcmat = [t1qddc t2qddc t3qddc t4qddc t5qddc]';       */
  /* '<S1>:1:558' */
  rtb_flow[0] = torq_reduce;
  rtb_flow[1] = d_torq;
  rtb_flow[2] = t4qddc;
  rtb_flow[3] = t5qddc;

  /*  tau = (qddmat*qdd'+qddcmat)'; */
  /* '<S1>:1:561' */
  rtb_qddout[0] = rtb_TmpSignalConversionAtSFunct[0] - torq_reduce;
  rtb_qddout[1] = rtb_TmpSignalConversionAtSFunct[1] - d_torq;
  rtb_qddout[2] = rtb_TmpSignalConversionAtSFunct[2] - t4qddc;
  rtb_qddout[3] = rtb_TmpSignalConversionAtSFunct[3] - t5qddc;
  MatlabExcavatorMode_mldivide_ly(A, rtb_qddout);

  /* qdd5 = [qdd4(1) 0 qdd4(2) qdd4(3) qdd4(4)]; */
  /* '<S1>:1:564' */
  memset(&I05c[0], 0, 9U * sizeof(real_T));

  /* '<S1>:1:565' */
  twow[0] = 0.0;
  twow[1] = 0.0;
  twow[2] = 0.0;

  /* '<S1>:1:566' */
  thrw[0] = 0.0;
  thrw[1] = 0.0;
  thrw[2] = 0.0;

  /* qdd3 = zeros(3,1); */
  /* '<S1>:1:569' */
  qddmatnew[0] = 0.0;
  qddmatnew[1] = 0.0;
  qddmatnew[2] = 0.0;
  qddmatnew[3] = 0.0;

  /* '<S1>:1:570' */
  qddcmatnew[0] = 0.0;
  qddcmatnew[1] = 0.0;

  /* '<S1>:1:571' */
  taunew[0] = 0.0;
  taunew[1] = 0.0;

  /* qdd2 = zeros(2,1); */
  /* '<S1>:1:574' */
  torq_reduce = 0.0;

  /* '<S1>:1:575' */
  spdslope = 0.0;

  /* '<S1>:1:576' */
  d_engspd = 0.0;

  /* count = 0; */
  /* '<S1>:1:579' */
  b_max = 0;

  /* '<S1>:1:580' */
  if (rtb_qddout[0] < 0.0) {
    t5qddc = -1.0;
  } else if (rtb_qddout[0] > 0.0) {
    t5qddc = 1.0;
  } else if (rtb_qddout[0] == 0.0) {
    t5qddc = 0.0;
  } else {
    t5qddc = rtb_qddout[0];
  }

  if (limitq[0] == t5qddc) {
    /* '<S1>:1:581' */
    /* '<S1>:1:582' */
    b_max = 1;
  }

  /* '<S1>:1:580' */
  if (rtb_qddout[1] < 0.0) {
    t5qddc = -1.0;
  } else if (rtb_qddout[1] > 0.0) {
    t5qddc = 1.0;
  } else if (rtb_qddout[1] == 0.0) {
    t5qddc = 0.0;
  } else {
    t5qddc = rtb_qddout[1];
  }

  if (limitq[1] == t5qddc) {
    /* '<S1>:1:581' */
    /* '<S1>:1:582' */
    b_max = 2;
  }

  /* '<S1>:1:580' */
  if (rtb_qddout[2] < 0.0) {
    t5qddc = -1.0;
  } else if (rtb_qddout[2] > 0.0) {
    t5qddc = 1.0;
  } else if (rtb_qddout[2] == 0.0) {
    t5qddc = 0.0;
  } else {
    t5qddc = rtb_qddout[2];
  }

  if (limitq[2] == t5qddc) {
    /* '<S1>:1:581' */
    /* '<S1>:1:582' */
    b_max = 3;
  }

  /* '<S1>:1:580' */
  if (rtb_qddout[3] < 0.0) {
    t5qddc = -1.0;
  } else if (rtb_qddout[3] > 0.0) {
    t5qddc = 1.0;
  } else if (rtb_qddout[3] == 0.0) {
    t5qddc = 0.0;
  } else {
    t5qddc = rtb_qddout[3];
  }

  if (limitq[3] == t5qddc) {
    /* '<S1>:1:581' */
    /* '<S1>:1:582' */
    b_max = 4;
  }

  /* '<S1>:1:580' */
  if (b_max > 0) {
    /* '<S1>:1:586' */
    /* '<S1>:1:587' */
    torq = 0.0;

    /* '<S1>:1:588' */
    for (i = 0; i < 4; i++) {
      /* '<S1>:1:588' */
      /* '<S1>:1:589' */
      torqindex = 0.0;
      if (1 + i != b_max) {
        /* '<S1>:1:590' */
        /* '<S1>:1:591' */
        torq++;

        /* '<S1>:1:592' */
        if (1 != b_max) {
          /* '<S1>:1:593' */
          /* '<S1>:1:594' */
          torqindex = 1.0;

          /* '<S1>:1:595' */
          I05c[(int32_T)torq - 1] = A[i];
        }

        /* '<S1>:1:592' */
        if (2 != b_max) {
          /* '<S1>:1:593' */
          /* '<S1>:1:594' */
          torqindex++;

          /* '<S1>:1:595' */
          I05c[((int32_T)torq + 3 * ((int32_T)torqindex - 1)) - 1] = A[i + 4];
        }

        /* '<S1>:1:592' */
        if (3 != b_max) {
          /* '<S1>:1:593' */
          /* '<S1>:1:594' */
          torqindex++;

          /* '<S1>:1:595' */
          I05c[((int32_T)torq + 3 * ((int32_T)torqindex - 1)) - 1] = A[i + 8];
        }

        /* '<S1>:1:592' */
        if (4 != b_max) {
          /* '<S1>:1:593' */
          /* '<S1>:1:594' */
          torqindex++;

          /* '<S1>:1:595' */
          I05c[((int32_T)torq + 3 * ((int32_T)torqindex - 1)) - 1] = A[i + 12];
        }

        /* '<S1>:1:592' */
        /* '<S1>:1:598' */
        twow[(int32_T)torq - 1] = rtb_flow[i];

        /* '<S1>:1:599' */
        thrw[(int32_T)torq - 1] = rtb_TmpSignalConversionAtSFunct[i];
      }

      /* '<S1>:1:588' */
    }

    /* '<S1>:1:602' */
    thrw_1[0] = thrw[0] - twow[0];
    thrw_1[1] = thrw[1] - twow[1];
    thrw_1[2] = thrw[2] - twow[2];
    MatlabExcavatorModel_mldivide(I05c, thrw_1, twow);

    /* '<S1>:1:604' */
    torq = 0.0;

    /* '<S1>:1:605' */
    if (1 != b_max) {
      /* '<S1>:1:606' */
      /* '<S1>:1:607' */
      torq = 1.0;

      /* '<S1>:1:608' */
      rtb_qddout[0] = twow[0];
    } else {
      /* '<S1>:1:610' */
      rtb_qddout[0] = 0.0;
    }

    /* '<S1>:1:605' */
    if (2 != b_max) {
      /* '<S1>:1:606' */
      /* '<S1>:1:607' */
      torq++;

      /* '<S1>:1:608' */
      rtb_qddout[1] = twow[(int32_T)torq - 1];
    } else {
      /* '<S1>:1:610' */
      rtb_qddout[1] = 0.0;
    }

    /* '<S1>:1:605' */
    if (3 != b_max) {
      /* '<S1>:1:606' */
      /* '<S1>:1:607' */
      torq++;

      /* '<S1>:1:608' */
      rtb_qddout[2] = twow[(int32_T)torq - 1];
    } else {
      /* '<S1>:1:610' */
      rtb_qddout[2] = 0.0;
    }

    /* '<S1>:1:605' */
    if (4 != b_max) {
      /* '<S1>:1:606' */
      /* '<S1>:1:607' */
      torq++;

      /* '<S1>:1:608' */
      rtb_qddout[3] = twow[(int32_T)torq - 1];
    } else {
      /* '<S1>:1:610' */
      rtb_qddout[3] = 0.0;
    }

    /* '<S1>:1:605' */
    /* '<S1>:1:614' */
    max = 0;

    /* '<S1>:1:615' */
    if (rtb_qddout[0] < 0.0) {
      t5qddc = -1.0;
    } else if (rtb_qddout[0] > 0.0) {
      t5qddc = 1.0;
    } else if (rtb_qddout[0] == 0.0) {
      t5qddc = 0.0;
    } else {
      t5qddc = rtb_qddout[0];
    }

    if ((limitq[0] == t5qddc) && (1 != b_max)) {
      /* '<S1>:1:616' */
      /* '<S1>:1:617' */
      max = 1;
    }

    /* '<S1>:1:615' */
    if (rtb_qddout[1] < 0.0) {
      t5qddc = -1.0;
    } else if (rtb_qddout[1] > 0.0) {
      t5qddc = 1.0;
    } else if (rtb_qddout[1] == 0.0) {
      t5qddc = 0.0;
    } else {
      t5qddc = rtb_qddout[1];
    }

    if ((limitq[1] == t5qddc) && (2 != b_max)) {
      /* '<S1>:1:616' */
      /* '<S1>:1:617' */
      max = 2;
    }

    /* '<S1>:1:615' */
    if (rtb_qddout[2] < 0.0) {
      t5qddc = -1.0;
    } else if (rtb_qddout[2] > 0.0) {
      t5qddc = 1.0;
    } else if (rtb_qddout[2] == 0.0) {
      t5qddc = 0.0;
    } else {
      t5qddc = rtb_qddout[2];
    }

    if ((limitq[2] == t5qddc) && (3 != b_max)) {
      /* '<S1>:1:616' */
      /* '<S1>:1:617' */
      max = 3;
    }

    /* '<S1>:1:615' */
    if (rtb_qddout[3] < 0.0) {
      t5qddc = -1.0;
    } else if (rtb_qddout[3] > 0.0) {
      t5qddc = 1.0;
    } else if (rtb_qddout[3] == 0.0) {
      t5qddc = 0.0;
    } else {
      t5qddc = rtb_qddout[3];
    }

    if ((limitq[3] == t5qddc) && (4 != b_max)) {
      /* '<S1>:1:616' */
      /* '<S1>:1:617' */
      max = 4;
    }

    /* '<S1>:1:615' */
    if (max > 0) {
      /* '<S1>:1:620' */
      /* '<S1>:1:621' */
      torq = 0.0;

      /* '<S1>:1:622' */
      for (i = 0; i < 4; i++) {
        /* '<S1>:1:622' */
        /* '<S1>:1:623' */
        torqindex = 0.0;
        if ((1 + i != max) && (1 + i != b_max)) {
          /* '<S1>:1:624' */
          /* '<S1>:1:625' */
          torq++;

          /* '<S1>:1:626' */
          if ((1 != max) && (1 != b_max)) {
            /* '<S1>:1:627' */
            /* '<S1>:1:628' */
            torqindex = 1.0;

            /* '<S1>:1:629' */
            qddmatnew[(int32_T)torq - 1] = A[i];
          }

          /* '<S1>:1:626' */
          if ((2 != max) && (2 != b_max)) {
            /* '<S1>:1:627' */
            /* '<S1>:1:628' */
            torqindex++;

            /* '<S1>:1:629' */
            qddmatnew[((int32_T)torq + (((int32_T)torqindex - 1) << 1)) - 1] =
              A[i + 4];
          }

          /* '<S1>:1:626' */
          if ((3 != max) && (3 != b_max)) {
            /* '<S1>:1:627' */
            /* '<S1>:1:628' */
            torqindex++;

            /* '<S1>:1:629' */
            qddmatnew[((int32_T)torq + (((int32_T)torqindex - 1) << 1)) - 1] =
              A[i + 8];
          }

          /* '<S1>:1:626' */
          if ((4 != max) && (4 != b_max)) {
            /* '<S1>:1:627' */
            /* '<S1>:1:628' */
            torqindex++;

            /* '<S1>:1:629' */
            qddmatnew[((int32_T)torq + (((int32_T)torqindex - 1) << 1)) - 1] =
              A[i + 12];
          }

          /* '<S1>:1:626' */
          /* '<S1>:1:632' */
          qddcmatnew[(int32_T)torq - 1] = rtb_flow[i];

          /* '<S1>:1:633' */
          taunew[(int32_T)torq - 1] = rtb_TmpSignalConversionAtSFunct[i];
        }

        /* '<S1>:1:622' */
      }

      /* '<S1>:1:636' */
      taunew_0[0] = taunew[0] - qddcmatnew[0];
      taunew_0[1] = taunew[1] - qddcmatnew[1];
      MatlabExcavatorModel_mldivide_l(qddmatnew, taunew_0, qddcmatnew);

      /* '<S1>:1:638' */
      torq = 0.0;

      /* '<S1>:1:639' */
      if ((1 != max) && (1 != b_max)) {
        /* '<S1>:1:640' */
        /* '<S1>:1:641' */
        torq = 1.0;

        /* '<S1>:1:642' */
        rtb_qddout[0] = qddcmatnew[0];
      } else {
        /* '<S1>:1:644' */
        rtb_qddout[0] = 0.0;
      }

      /* '<S1>:1:639' */
      if ((2 != max) && (2 != b_max)) {
        /* '<S1>:1:640' */
        /* '<S1>:1:641' */
        torq++;

        /* '<S1>:1:642' */
        rtb_qddout[1] = qddcmatnew[(int32_T)torq - 1];
      } else {
        /* '<S1>:1:644' */
        rtb_qddout[1] = 0.0;
      }

      /* '<S1>:1:639' */
      if ((3 != max) && (3 != b_max)) {
        /* '<S1>:1:640' */
        /* '<S1>:1:641' */
        torq++;

        /* '<S1>:1:642' */
        rtb_qddout[2] = qddcmatnew[(int32_T)torq - 1];
      } else {
        /* '<S1>:1:644' */
        rtb_qddout[2] = 0.0;
      }

      /* '<S1>:1:639' */
      if ((4 != max) && (4 != b_max)) {
        /* '<S1>:1:640' */
        /* '<S1>:1:641' */
        torq++;

        /* '<S1>:1:642' */
        rtb_qddout[3] = qddcmatnew[(int32_T)torq - 1];
      } else {
        /* '<S1>:1:644' */
        rtb_qddout[3] = 0.0;
      }

      /* '<S1>:1:639' */
      /* '<S1>:1:648' */
      max_0 = 0;

      /* '<S1>:1:649' */
      if (rtb_qddout[0] < 0.0) {
        t5qddc = -1.0;
      } else if (rtb_qddout[0] > 0.0) {
        t5qddc = 1.0;
      } else if (rtb_qddout[0] == 0.0) {
        t5qddc = 0.0;
      } else {
        t5qddc = rtb_qddout[0];
      }

      if ((limitq[0] == t5qddc) && (1 != b_max) && (1 != max)) {
        /* '<S1>:1:650' */
        /* '<S1>:1:651' */
        max_0 = 1;
      }

      /* '<S1>:1:649' */
      if (rtb_qddout[1] < 0.0) {
        t5qddc = -1.0;
      } else if (rtb_qddout[1] > 0.0) {
        t5qddc = 1.0;
      } else if (rtb_qddout[1] == 0.0) {
        t5qddc = 0.0;
      } else {
        t5qddc = rtb_qddout[1];
      }

      if ((limitq[1] == t5qddc) && (2 != b_max) && (2 != max)) {
        /* '<S1>:1:650' */
        /* '<S1>:1:651' */
        max_0 = 2;
      }

      /* '<S1>:1:649' */
      if (rtb_qddout[2] < 0.0) {
        t5qddc = -1.0;
      } else if (rtb_qddout[2] > 0.0) {
        t5qddc = 1.0;
      } else if (rtb_qddout[2] == 0.0) {
        t5qddc = 0.0;
      } else {
        t5qddc = rtb_qddout[2];
      }

      if ((limitq[2] == t5qddc) && (3 != b_max) && (3 != max)) {
        /* '<S1>:1:650' */
        /* '<S1>:1:651' */
        max_0 = 3;
      }

      /* '<S1>:1:649' */
      if (rtb_qddout[3] < 0.0) {
        t5qddc = -1.0;
      } else if (rtb_qddout[3] > 0.0) {
        t5qddc = 1.0;
      } else if (rtb_qddout[3] == 0.0) {
        t5qddc = 0.0;
      } else {
        t5qddc = rtb_qddout[3];
      }

      if ((limitq[3] == t5qddc) && (4 != b_max) && (4 != max)) {
        /* '<S1>:1:650' */
        /* '<S1>:1:651' */
        max_0 = 4;
      }

      /* '<S1>:1:649' */
      if (max_0 > 0) {
        /* '<S1>:1:654' */
        /* '<S1>:1:655' */
        /* '<S1>:1:656' */
        for (i = 0; i < 4; i++) {
          /* '<S1>:1:656' */
          /* '<S1>:1:657' */
          if ((1 + i != max_0) && (1 + i != max) && (1 + i != b_max)) {
            /* '<S1>:1:658' */
            /* '<S1>:1:659' */
            /* '<S1>:1:660' */
            if ((1 != max_0) && (1 != max) && (1 != b_max)) {
              /* '<S1>:1:661' */
              /* '<S1>:1:662' */
              /* '<S1>:1:663' */
              torq_reduce = A[i];
            }

            /* '<S1>:1:660' */
            if ((2 != max_0) && (2 != max) && (2 != b_max)) {
              /* '<S1>:1:661' */
              /* '<S1>:1:662' */
              /* '<S1>:1:663' */
              torq_reduce = A[i + 4];
            }

            /* '<S1>:1:660' */
            if ((3 != max_0) && (3 != max) && (3 != b_max)) {
              /* '<S1>:1:661' */
              /* '<S1>:1:662' */
              /* '<S1>:1:663' */
              torq_reduce = A[i + 8];
            }

            /* '<S1>:1:660' */
            if ((4 != max_0) && (4 != max) && (4 != b_max)) {
              /* '<S1>:1:661' */
              /* '<S1>:1:662' */
              /* '<S1>:1:663' */
              torq_reduce = A[i + 12];
            }

            /* '<S1>:1:660' */
            /* '<S1>:1:666' */
            spdslope = rtb_flow[i];

            /* '<S1>:1:667' */
            d_engspd = rtb_TmpSignalConversionAtSFunct[i];
          }

          /* '<S1>:1:656' */
        }

        /* '<S1>:1:670' */
        torq_reduce = (d_engspd - spdslope) / torq_reduce;

        /* '<S1>:1:672' */
        /* '<S1>:1:673' */
        if ((1 != max_0) && (1 != max) && (1 != b_max)) {
          /* '<S1>:1:674' */
          /* '<S1>:1:675' */
          /* '<S1>:1:676' */
          rtb_qddout[0] = torq_reduce;
        } else {
          /* '<S1>:1:678' */
          rtb_qddout[0] = 0.0;
        }

        /* '<S1>:1:673' */
        if ((2 != max_0) && (2 != max) && (2 != b_max)) {
          /* '<S1>:1:674' */
          /* '<S1>:1:675' */
          /* '<S1>:1:676' */
          rtb_qddout[1] = torq_reduce;
        } else {
          /* '<S1>:1:678' */
          rtb_qddout[1] = 0.0;
        }

        /* '<S1>:1:673' */
        if ((3 != max_0) && (3 != max) && (3 != b_max)) {
          /* '<S1>:1:674' */
          /* '<S1>:1:675' */
          /* '<S1>:1:676' */
          rtb_qddout[2] = torq_reduce;
        } else {
          /* '<S1>:1:678' */
          rtb_qddout[2] = 0.0;
        }

        /* '<S1>:1:673' */
        if ((4 != max_0) && (4 != max) && (4 != b_max)) {
          /* '<S1>:1:674' */
          /* '<S1>:1:675' */
          /* '<S1>:1:676' */
          rtb_qddout[3] = torq_reduce;
        } else {
          /* '<S1>:1:678' */
          rtb_qddout[3] = 0.0;
        }

        /* '<S1>:1:673' */
        /* '<S1>:1:682' */
        for (i = 0; i < 4; i++) {
          /* '<S1>:1:682' */
          if (rtb_qddout[i] < 0.0) {
            t5qddc = -1.0;
          } else if (rtb_qddout[i] > 0.0) {
            t5qddc = 1.0;
          } else if (rtb_qddout[i] == 0.0) {
            t5qddc = 0.0;
          } else {
            t5qddc = rtb_qddout[i];
          }

          if ((limitq[i] == t5qddc) && (1 + i != b_max) && (1 + i != max) && (1
               + i != max_0)) {
            /* '<S1>:1:683' */
            /* '<S1>:1:684' */
            rtb_qddout[0] = 0.0;
            rtb_qddout[1] = 0.0;
            rtb_qddout[2] = 0.0;
            rtb_qddout[3] = 0.0;
          }

          /* '<S1>:1:682' */
        }
      }
    }
  }

  /* End of MATLAB Function: '<Root>/Arm Dynamics' */

  /* Gain: '<Root>/Transform4' */
  /* '<S1>:1:692' */
  /* MATLAB Function 'Command to Flow/Torque limiter/Limit torque at given engine speed': '<S8>:1' */
  /*  this function limits the pump output evenly for all 4 pumps when the */
  /*  maximum allowable torque is exceeded */
  /*  phead is head side pressure in psi */
  /*  prod is rod side pressure in psi */
  /*  q is flow in in^3/sec */
  /*  maxpumpflow is the max displacement of the pumps being driven at pumpspeed */
  /*  maxtorq is the max torque rating of the diesel engine in inch lbs */
  /*  maxflow_per_rev is the full displacement of the pump in in^3/rev */
  /* '<S8>:1:13' */
  /* '<S8>:1:14' */
  /* '<S8>:1:15' */
  /*  maxhp = 49; */
  /* '<S8>:1:18' */
  /*  calculate needed torq for current demand */
  /* '<S8>:1:24' */
  /* '<S8>:1:25' */
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
  /* '<S8>:1:50' */
  /*  % calculate limited hp (to check that it's < max hp) */
  /*  totalhp = 0; */
  /*  for k = 1:4 */
  /*      totalhp = totalhp + (phead(k)-prod(k))*flowout(k)/12/550; */
  /*  end */
  /*  calculate limited torque (to check that it's < max torque) */
  /* '<S8>:1:62' */
  /* '<S8>:1:63' */
  /* '<S8>:1:69' */
  t5qddc = MatlabExcavatorModel_P.Transform4_Gain * MatlabExcavatorModel_Y.Flow
    [0];

  /* Rounding: '<Root>/Rounding Function' */
  if (t5qddc < 0.0) {
    t5qddc = ceil(t5qddc);
  } else {
    t5qddc = floor(t5qddc);
  }

  /* Sum: '<Root>/Sum' incorporates:
   *  Inport: '<Root>/Flow1'
   *  Rounding: '<Root>/Rounding Function'
   */
  rtb_Sum[0] = MatlabExcavatorModel_U.Flow1 + t5qddc;

  /* Gain: '<Root>/Transform4' */
  t5qddc = MatlabExcavatorModel_P.Transform4_Gain * MatlabExcavatorModel_Y.Flow
    [1];

  /* Rounding: '<Root>/Rounding Function' */
  if (t5qddc < 0.0) {
    t5qddc = ceil(t5qddc);
  } else {
    t5qddc = floor(t5qddc);
  }

  /* Sum: '<Root>/Sum' incorporates:
   *  Inport: '<Root>/Flow2'
   *  Rounding: '<Root>/Rounding Function'
   */
  rtb_Sum[1] = MatlabExcavatorModel_U.Flow2 + t5qddc;

  /* Gain: '<Root>/Transform4' */
  t5qddc = MatlabExcavatorModel_P.Transform4_Gain * MatlabExcavatorModel_Y.Flow
    [2];

  /* Rounding: '<Root>/Rounding Function' */
  if (t5qddc < 0.0) {
    t5qddc = ceil(t5qddc);
  } else {
    t5qddc = floor(t5qddc);
  }

  /* Sum: '<Root>/Sum' incorporates:
   *  Inport: '<Root>/Flow3'
   *  Rounding: '<Root>/Rounding Function'
   */
  rtb_Sum[2] = MatlabExcavatorModel_U.Flow3 + t5qddc;

  /* Gain: '<Root>/Transform4' */
  t5qddc = MatlabExcavatorModel_P.Transform4_Gain * MatlabExcavatorModel_Y.Flow
    [3];

  /* Rounding: '<Root>/Rounding Function' */
  if (t5qddc < 0.0) {
    t5qddc = ceil(t5qddc);
  } else {
    t5qddc = floor(t5qddc);
  }

  /* Sum: '<Root>/Sum' incorporates:
   *  Inport: '<Root>/Flow4'
   *  Rounding: '<Root>/Rounding Function'
   */
  rtb_Sum[3] = MatlabExcavatorModel_U.Flow4 + t5qddc;

  /* Update for DiscreteIntegrator: '<S4>/Integrator (qd to q)' */
  MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[0] +=
    MatlabExcavatorModel_P.Integratorqdtoq_gainval *
    MatlabExcavatorModel_B.qdot[0];
  MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[1] +=
    MatlabExcavatorModel_P.Integratorqdtoq_gainval *
    MatlabExcavatorModel_B.qdot[1];
  MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[2] +=
    MatlabExcavatorModel_P.Integratorqdtoq_gainval *
    MatlabExcavatorModel_B.qdot[2];
  MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[3] +=
    MatlabExcavatorModel_P.Integratorqdtoq_gainval *
    MatlabExcavatorModel_B.qdot[3];

  /* Update for Delay: '<S4>/Integer Delay' */
  MatlabExcavatorModel_DW.IntegerDelay_DSTATE[0] =
    MatlabExcavatorModel_B.limiter_o3[0];
  MatlabExcavatorModel_DW.IntegerDelay_DSTATE[1] =
    MatlabExcavatorModel_B.limiter_o3[1];
  MatlabExcavatorModel_DW.IntegerDelay_DSTATE[2] =
    MatlabExcavatorModel_B.limiter_o3[2];
  MatlabExcavatorModel_DW.IntegerDelay_DSTATE[3] =
    MatlabExcavatorModel_B.limiter_o3[3];

  /* Update for DiscreteIntegrator: '<S4>/Integrator (qdd to qd)' */
  MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0] +=
    MatlabExcavatorModel_P.Integratorqddtoqd_gainval * rtb_qddout[0];
  MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1] +=
    MatlabExcavatorModel_P.Integratorqddtoqd_gainval * rtb_qddout[1];
  MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2] +=
    MatlabExcavatorModel_P.Integratorqddtoqd_gainval * rtb_qddout[2];
  MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3] +=
    MatlabExcavatorModel_P.Integratorqddtoqd_gainval * rtb_qddout[3];
  if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0] >=
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[0]) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0] =
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[0];
  } else {
    if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0] <=
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[0]) {
      MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0] =
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[0];
    }
  }

  if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1] >=
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[1]) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1] =
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[1];
  } else {
    if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1] <=
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[1]) {
      MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1] =
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[1];
    }
  }

  if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2] >=
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[2]) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2] =
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[2];
  } else {
    if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2] <=
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[2]) {
      MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2] =
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[2];
    }
  }

  if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3] >=
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[3]) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3] =
      MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[3];
  } else {
    if (MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3] <=
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[3]) {
      MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3] =
        MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[3];
    }
  }

  if (rtb_Transform1[0] > 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[0] = 1;
  } else if (rtb_Transform1[0] < 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[0] = -1;
  } else if (rtb_Transform1[0] == 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[0] = 0;
  } else {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[0] = 2;
  }

  if (rtb_Transform1[1] > 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[1] = 1;
  } else if (rtb_Transform1[1] < 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[1] = -1;
  } else if (rtb_Transform1[1] == 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[1] = 0;
  } else {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[1] = 2;
  }

  if (rtb_Transform1[2] > 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[2] = 1;
  } else if (rtb_Transform1[2] < 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[2] = -1;
  } else if (rtb_Transform1[2] == 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[2] = 0;
  } else {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[2] = 2;
  }

  if (rtb_Transform1[3] > 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[3] = 1;
  } else if (rtb_Transform1[3] < 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[3] = -1;
  } else if (rtb_Transform1[3] == 0.0) {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[3] = 0;
  } else {
    MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[3] = 2;
  }

  /* End of Update for DiscreteIntegrator: '<S4>/Integrator (qdd to qd)' */

  /* Update for UnitDelay: '<Root>/Unit Delay1' */
  MatlabExcavatorModel_DW.UnitDelay1_DSTATE[0] = rtb_Sum[0];
  MatlabExcavatorModel_DW.UnitDelay1_DSTATE[1] = rtb_Sum[1];
  MatlabExcavatorModel_DW.UnitDelay1_DSTATE[2] = rtb_Sum[2];
  MatlabExcavatorModel_DW.UnitDelay1_DSTATE[3] = rtb_Sum[3];

  /* Update for Delay: '<S7>/Integer Delay3' */
  MatlabExcavatorModel_DW.IntegerDelay3_DSTATE = rtb_totalfuel;

  /* Update for Delay: '<S7>/Integer Delay1' */
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[0] =
    MatlabExcavatorModel_B.SFunction_o1_i[0];
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[1] =
    MatlabExcavatorModel_B.SFunction_o1_i[1];
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[2] =
    MatlabExcavatorModel_B.SFunction_o1_i[2];
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[3] =
    MatlabExcavatorModel_B.SFunction_o1_i[3];

  /* Update for Delay: '<S7>/Integer Delay2' */
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[0] =
    MatlabExcavatorModel_B.SFunction_o2_h[0];
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[1] =
    MatlabExcavatorModel_B.SFunction_o2_h[1];
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[2] =
    MatlabExcavatorModel_B.SFunction_o2_h[2];
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[3] =
    MatlabExcavatorModel_B.SFunction_o2_h[3];

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
  MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[1] = rtInf;
  MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[2] = rtInf;
  MatlabExcavatorModel_P.Integratorqddtoqd_UpperSat[3] = rtInf;
  MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[1] = rtMinusInf;
  MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[2] = rtMinusInf;
  MatlabExcavatorModel_P.Integratorqddtoqd_LowerSat[3] = rtMinusInf;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[854] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[911] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[968] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1025] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1082] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1138] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1139] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1195] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1196] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1252] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1253] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1309] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1310] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1365] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1366] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1367] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1422] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1423] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1424] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1479] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1480] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1481] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1536] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1537] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1538] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1592] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1593] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1594] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1595] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1649] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1650] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1651] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1652] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1653] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1706] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1707] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1708] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1709] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1710] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1711] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1762] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1763] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1764] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1765] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1766] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1767] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1768] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1769] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1819] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1820] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1821] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1822] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1823] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1824] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1825] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1826] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1827] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1876] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1877] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1878] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1879] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1880] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1881] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1882] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1883] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1884] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1933] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1934] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1935] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1936] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1937] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1938] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1939] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1940] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1941] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1942] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1990] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1991] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1992] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1993] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1994] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1995] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1996] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1997] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1998] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[1999] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2000] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2046] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2047] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2048] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2049] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2050] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2051] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2052] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2053] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2054] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2055] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2056] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2057] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2103] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2104] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2105] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2106] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2107] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2108] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2109] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2110] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2111] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2112] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2113] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2114] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2115] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2160] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2161] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2162] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2163] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2164] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2165] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2166] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2167] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2168] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2169] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2170] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2171] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2172] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2217] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2218] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2219] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2220] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2221] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2222] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2223] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2224] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2225] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2226] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2227] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2228] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2229] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2230] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2273] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2274] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2275] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2276] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2277] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2278] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2279] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2280] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2281] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2282] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2283] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2284] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2285] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2286] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2287] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2330] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2331] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2332] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2333] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2334] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2335] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2336] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2337] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2338] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2339] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2340] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2341] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2342] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2343] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2344] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2387] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2388] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2389] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2390] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2391] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2392] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2393] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2394] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2395] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2396] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2397] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2398] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2399] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2400] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2401] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2402] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2403] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2443] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2444] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2445] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2446] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2447] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2448] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2449] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2450] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2451] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2452] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2453] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2454] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2455] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2456] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2457] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2458] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2459] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2460] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2461] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2500] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2501] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2502] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2503] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2504] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2505] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2506] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2507] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2508] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2509] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2510] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2511] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2512] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2513] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2514] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2515] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2516] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2517] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2518] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2519] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2557] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2558] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2559] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2560] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2561] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2562] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2563] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2564] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2565] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2566] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2567] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2568] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2569] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2570] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2571] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2572] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2573] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2574] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2575] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2576] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2614] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2615] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2616] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2617] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2618] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2619] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2620] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2621] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2622] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2623] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2624] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2625] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2626] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2627] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2628] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2629] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2630] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2631] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2632] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2633] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2634] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2635] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2670] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2671] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2672] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2673] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2674] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2675] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2676] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2677] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2678] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2679] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2680] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2681] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2682] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2683] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2684] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2685] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2686] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2687] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2688] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2689] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2690] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2691] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2692] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2693] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2715] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2716] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2717] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2718] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2719] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2720] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2721] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2722] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2723] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2724] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2725] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2726] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2727] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2728] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2729] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2730] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2731] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2732] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2733] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2734] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2735] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2736] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2737] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2738] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2739] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2740] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2741] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2742] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2743] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2744] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2745] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2746] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2747] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2748] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2749] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2750] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2751] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2760] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2761] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2762] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2771] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2772] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2773] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2774] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2775] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2776] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2777] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2778] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2779] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2780] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2781] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2782] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2783] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2784] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2785] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2786] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2787] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2788] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2789] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2790] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2791] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2792] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2793] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2794] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2795] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2796] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2797] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2798] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2799] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2800] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2801] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2802] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2803] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2804] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2805] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2806] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2807] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2808] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2809] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2810] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2811] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2812] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2813] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2814] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2815] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2816] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2817] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2818] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2819] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2820] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2827] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2828] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2829] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2830] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2831] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2832] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2833] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2834] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2835] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2836] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2837] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2838] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2839] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2840] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2841] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2842] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2843] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2844] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2845] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2846] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2847] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2848] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2849] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2850] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2851] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2852] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2853] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2854] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2855] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2856] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2857] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2858] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2859] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2860] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2861] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2862] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2863] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2864] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2865] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2866] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2867] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2868] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2869] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2870] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2871] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2872] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2873] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2874] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2875] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2876] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2877] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2879] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2880] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2881] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2882] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2883] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2884] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2885] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2886] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2887] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2888] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2889] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2890] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2891] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2892] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2893] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2894] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2895] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2896] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2897] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2898] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2899] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2900] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2901] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2902] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2903] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2904] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2905] = rtNaN;
  MatlabExcavatorModel_P.fuelratematrixkghr_Value[2906] = rtNaN;

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

  MatlabExcavatorModel_M->Sizes.numSFcns = (7);

  /* register each child */
  {
    (void) memset((void *)
                  &MatlabExcavatorModel_M->NonInlinedSFcns.childSFunctions[0], 0,
                  7*sizeof(SimStruct));
    MatlabExcavatorModel_M->childSfunctions =
      (&MatlabExcavatorModel_M->NonInlinedSFcns.childSFunctionPtrs[0]);

    {
      int_T i;
      for (i = 0; i < 7; i++) {
        MatlabExcavatorModel_M->childSfunctions[i] =
          (&MatlabExcavatorModel_M->NonInlinedSFcns.childSFunctions[i]);
      }
    }

    /* Level2 S-Function Block: MatlabExcavatorModel/<S4>/limiter (limiterg_435_brake) */
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
        _ssSetNumInputPorts(rts, 4);
        ssSetPortInfoForInputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn0.inputPortInfo[0]);

        /* port 0 */
        {
          ssSetInputPortRequiredContiguous(rts, 0, 1);
          ssSetInputPortSignal(rts, 0, MatlabExcavatorModel_B.q);
          _ssSetInputPortNumDimensions(rts, 0, 1);
          ssSetInputPortWidth(rts, 0, 4);
        }

        /* port 1 */
        {
          ssSetInputPortRequiredContiguous(rts, 1, 1);
          ssSetInputPortSignal(rts, 1, MatlabExcavatorModel_B.qdot);
          _ssSetInputPortNumDimensions(rts, 1, 1);
          ssSetInputPortWidth(rts, 1, 4);
        }

        /* port 2 */
        {
          ssSetInputPortRequiredContiguous(rts, 2, 1);
          ssSetInputPortSignal(rts, 2, MatlabExcavatorModel_P.Constant_Value);
          _ssSetInputPortNumDimensions(rts, 2, 1);
          ssSetInputPortWidth(rts, 2, 4);
        }

        /* port 3 */
        {
          ssSetInputPortRequiredContiguous(rts, 3, 1);
          ssSetInputPortSignal(rts, 3, MatlabExcavatorModel_B.UnitDelay1);
          _ssSetInputPortNumDimensions(rts, 3, 1);
          ssSetInputPortWidth(rts, 3, 4);
        }
      }

      /* outputs */
      {
        ssSetPortInfoForOutputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn0.outputPortInfo[0]);
        _ssSetNumOutputPorts(rts, 3);

        /* port 0 */
        {
          _ssSetOutputPortNumDimensions(rts, 0, 1);
          ssSetOutputPortWidth(rts, 0, 4);
          ssSetOutputPortSignal(rts, 0, ((real_T *) &MatlabExcavatorModel_Y.Q[0]));
        }

        /* port 1 */
        {
          _ssSetOutputPortNumDimensions(rts, 1, 1);
          ssSetOutputPortWidth(rts, 1, 4);
          ssSetOutputPortSignal(rts, 1, ((real_T *) &MatlabExcavatorModel_Y.Qd[0]));
        }

        /* port 2 */
        {
          _ssSetOutputPortNumDimensions(rts, 2, 1);
          ssSetOutputPortWidth(rts, 2, 4);
          ssSetOutputPortSignal(rts, 2, ((real_T *)
            MatlabExcavatorModel_B.limiter_o3));
        }
      }

      /* states */
      ssSetDiscStates(rts, (real_T *) &MatlabExcavatorModel_DW.limiter_DSTATE[0]);

      /* path info */
      ssSetModelName(rts, "limiter");
      ssSetPath(rts, "MatlabExcavatorModel/Integration/limiter");
      ssSetRTModel(rts,MatlabExcavatorModel_M);
      ssSetParentSS(rts, (NULL));
      ssSetRootSS(rts, rts);
      ssSetVersion(rts, SIMSTRUCT_VERSION_LEVEL2);

      /* parameters */
      {
        mxArray **sfcnParams = (mxArray **)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn0.params;
        ssSetSFcnParamsCount(rts, 1);
        ssSetSFcnParamsPtr(rts, &sfcnParams[0]);
        ssSetSFcnParam(rts, 0, (mxArray*)MatlabExcavatorModel_P.limiter_P1_Size);
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
        ssSetDWorkWidth(rts, 0, 4);
        ssSetDWorkDataType(rts, 0,SS_DOUBLE);
        ssSetDWorkComplexSignal(rts, 0, 0);
        ssSetDWorkUsedAsDState(rts, 0, 1);
        ssSetDWork(rts, 0, &MatlabExcavatorModel_DW.limiter_DSTATE[0]);
      }

      /* registration */
      limiterg_435_brake(rts);
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
      _ssSetOutputPortConnected(rts, 1, 1);
      _ssSetOutputPortConnected(rts, 2, 1);
      _ssSetOutputPortBeingMerged(rts, 0, 0);
      _ssSetOutputPortBeingMerged(rts, 1, 0);
      _ssSetOutputPortBeingMerged(rts, 2, 0);

      /* Update the BufferDstPort flags for each input port */
      ssSetInputPortBufferDstPort(rts, 0, -1);
      ssSetInputPortBufferDstPort(rts, 1, -1);
      ssSetInputPortBufferDstPort(rts, 2, -1);
      ssSetInputPortBufferDstPort(rts, 3, -1);
    }

    /* Level2 S-Function Block: MatlabExcavatorModel/<S6>/S-Function (j2c_435) */
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
        _ssSetNumInputPorts(rts, 1);
        ssSetPortInfoForInputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn1.inputPortInfo[0]);

        /* port 0 */
        {
          ssSetInputPortRequiredContiguous(rts, 0, 1);
          ssSetInputPortSignal(rts, 0,
                               MatlabExcavatorModel_B.TmpSignalConversionAtSFunctionI);
          _ssSetInputPortNumDimensions(rts, 0, 1);
          ssSetInputPortWidth(rts, 0, 8);
        }
      }

      /* outputs */
      {
        ssSetPortInfoForOutputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn1.outputPortInfo[0]);
        _ssSetNumOutputPorts(rts, 2);

        /* port 0 */
        {
          _ssSetOutputPortNumDimensions(rts, 0, 1);
          ssSetOutputPortWidth(rts, 0, 8);
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
      }

      /* path info */
      ssSetModelName(rts, "S-Function");
      ssSetPath(rts, "MatlabExcavatorModel/j2c/S-Function");
      ssSetRTModel(rts,MatlabExcavatorModel_M);
      ssSetParentSS(rts, (NULL));
      ssSetRootSS(rts, rts);
      ssSetVersion(rts, SIMSTRUCT_VERSION_LEVEL2);

      /* parameters */
      {
        mxArray **sfcnParams = (mxArray **)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn1.params;
        ssSetSFcnParamsCount(rts, 1);
        ssSetSFcnParamsPtr(rts, &sfcnParams[0]);
        ssSetSFcnParam(rts, 0, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P1_Size);
      }

      /* registration */
      j2c_435(rts);
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
      _ssSetOutputPortConnected(rts, 0, 1);
      _ssSetOutputPortConnected(rts, 1, 1);
      _ssSetOutputPortBeingMerged(rts, 0, 0);
      _ssSetOutputPortBeingMerged(rts, 1, 0);

      /* Update the BufferDstPort flags for each input port */
      ssSetInputPortBufferDstPort(rts, 0, -1);
    }

    /* Level2 S-Function Block: MatlabExcavatorModel/<S3>/S-Function (cylmodel_435_ryder) */
    {
      SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[2];

      /* timing info */
      time_T *sfcnPeriod =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn2.sfcnPeriod;
      time_T *sfcnOffset =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn2.sfcnOffset;
      int_T *sfcnTsMap = MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn2.sfcnTsMap;
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
                         [2]);
      }

      ssSetRTWSfcnInfo(rts, MatlabExcavatorModel_M->sfcnInfo);

      /* Allocate memory of model methods 2 */
      {
        ssSetModelMethods2(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods2[2]);
      }

      /* Allocate memory of model methods 3 */
      {
        ssSetModelMethods3(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods3[2]);
      }

      /* Allocate memory for states auxilliary information */
      {
        ssSetStatesInfo2(rts,
                         &MatlabExcavatorModel_M->NonInlinedSFcns.statesInfo2[2]);
      }

      /* inputs */
      {
        _ssSetNumInputPorts(rts, 3);
        ssSetPortInfoForInputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn2.inputPortInfo[0]);

        /* port 0 */
        {
          ssSetInputPortRequiredContiguous(rts, 0, 1);
          ssSetInputPortSignal(rts, 0, &MatlabExcavatorModel_B.SFunction_o1[0]);
          _ssSetInputPortNumDimensions(rts, 0, 1);
          ssSetInputPortWidth(rts, 0, 4);
        }

        /* port 1 */
        {
          ssSetInputPortRequiredContiguous(rts, 1, 1);
          ssSetInputPortSignal(rts, 1, &MatlabExcavatorModel_B.SFunction_o1[4]);
          _ssSetInputPortNumDimensions(rts, 1, 1);
          ssSetInputPortWidth(rts, 1, 4);
        }

        /* port 2 */
        {
          ssSetInputPortRequiredContiguous(rts, 2, 1);
          ssSetInputPortSignal(rts, 2, &MatlabExcavatorModel_Y.Flow[0]);
          _ssSetInputPortNumDimensions(rts, 2, 1);
          ssSetInputPortWidth(rts, 2, 4);
        }
      }

      /* outputs */
      {
        ssSetPortInfoForOutputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn2.outputPortInfo[0]);
        _ssSetNumOutputPorts(rts, 5);

        /* port 0 */
        {
          _ssSetOutputPortNumDimensions(rts, 0, 1);
          ssSetOutputPortWidth(rts, 0, 4);
          ssSetOutputPortSignal(rts, 0, ((real_T *)
            MatlabExcavatorModel_B.SFunction_o1_i));
        }

        /* port 1 */
        {
          _ssSetOutputPortNumDimensions(rts, 1, 1);
          ssSetOutputPortWidth(rts, 1, 4);
          ssSetOutputPortSignal(rts, 1, ((real_T *)
            MatlabExcavatorModel_B.SFunction_o2_h));
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
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn2.params;
        ssSetSFcnParamsCount(rts, 27);
        ssSetSFcnParamsPtr(rts, &sfcnParams[0]);
        ssSetSFcnParam(rts, 0, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P1_Size_e);
        ssSetSFcnParam(rts, 1, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P2_Size);
        ssSetSFcnParam(rts, 2, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P3_Size);
        ssSetSFcnParam(rts, 3, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P4_Size);
        ssSetSFcnParam(rts, 4, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P5_Size);
        ssSetSFcnParam(rts, 5, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P6_Size);
        ssSetSFcnParam(rts, 6, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P7_Size);
        ssSetSFcnParam(rts, 7, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P8_Size);
        ssSetSFcnParam(rts, 8, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P9_Size);
        ssSetSFcnParam(rts, 9, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P10_Size);
        ssSetSFcnParam(rts, 10, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P11_Size);
        ssSetSFcnParam(rts, 11, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P12_Size);
        ssSetSFcnParam(rts, 12, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P13_Size);
        ssSetSFcnParam(rts, 13, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P14_Size);
        ssSetSFcnParam(rts, 14, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P15_Size);
        ssSetSFcnParam(rts, 15, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P16_Size);
        ssSetSFcnParam(rts, 16, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P17_Size);
        ssSetSFcnParam(rts, 17, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P18_Size);
        ssSetSFcnParam(rts, 18, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P19_Size);
        ssSetSFcnParam(rts, 19, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P20_Size);
        ssSetSFcnParam(rts, 20, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P21_Size);
        ssSetSFcnParam(rts, 21, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P22_Size);
        ssSetSFcnParam(rts, 22, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P23_Size);
        ssSetSFcnParam(rts, 23, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P24_Size);
        ssSetSFcnParam(rts, 24, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P25_Size);
        ssSetSFcnParam(rts, 25, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P26_Size);
        ssSetSFcnParam(rts, 26, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P27_Size);
      }

      /* work vectors */
      {
        struct _ssDWorkRecord *dWorkRecord = (struct _ssDWorkRecord *)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn2.dWork;
        struct _ssDWorkAuxRecord *dWorkAuxRecord = (struct _ssDWorkAuxRecord *)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn2.dWorkAux;
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

    /* Level2 S-Function Block: MatlabExcavatorModel/<S5>/S-Function (p2torq1_435_brake) */
    {
      SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[3];

      /* timing info */
      time_T *sfcnPeriod =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn3.sfcnPeriod;
      time_T *sfcnOffset =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn3.sfcnOffset;
      int_T *sfcnTsMap = MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn3.sfcnTsMap;
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
                         [3]);
      }

      ssSetRTWSfcnInfo(rts, MatlabExcavatorModel_M->sfcnInfo);

      /* Allocate memory of model methods 2 */
      {
        ssSetModelMethods2(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods2[3]);
      }

      /* Allocate memory of model methods 3 */
      {
        ssSetModelMethods3(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods3[3]);
      }

      /* Allocate memory for states auxilliary information */
      {
        ssSetStatesInfo2(rts,
                         &MatlabExcavatorModel_M->NonInlinedSFcns.statesInfo2[3]);
      }

      /* inputs */
      {
        _ssSetNumInputPorts(rts, 4);
        ssSetPortInfoForInputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn3.inputPortInfo[0]);

        /* port 0 */
        {
          ssSetInputPortRequiredContiguous(rts, 0, 1);
          ssSetInputPortSignal(rts, 0, &MatlabExcavatorModel_B.SFunction_o1_i[0]);
          _ssSetInputPortNumDimensions(rts, 0, 1);
          ssSetInputPortWidth(rts, 0, 1);
        }

        /* port 1 */
        {
          ssSetInputPortRequiredContiguous(rts, 1, 1);
          ssSetInputPortSignal(rts, 1, &MatlabExcavatorModel_B.SFunction_o2_h[0]);
          _ssSetInputPortNumDimensions(rts, 1, 1);
          ssSetInputPortWidth(rts, 1, 1);
        }

        /* port 2 */
        {
          ssSetInputPortRequiredContiguous(rts, 2, 1);
          ssSetInputPortSignal(rts, 2, &MatlabExcavatorModel_B.SFunction_o1[0]);
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
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn3.outputPortInfo[0]);
        _ssSetNumOutputPorts(rts, 1);

        /* port 0 */
        {
          _ssSetOutputPortNumDimensions(rts, 0, 1);
          ssSetOutputPortWidth(rts, 0, 1);
          ssSetOutputPortSignal(rts, 0, ((real_T *)
            &MatlabExcavatorModel_B.SFunction));
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
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn3.params;
        ssSetSFcnParamsCount(rts, 4);
        ssSetSFcnParamsPtr(rts, &sfcnParams[0]);
        ssSetSFcnParam(rts, 0, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P1_Size_c);
        ssSetSFcnParam(rts, 1, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P2_Size_n);
        ssSetSFcnParam(rts, 2, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P3_Size_j);
        ssSetSFcnParam(rts, 3, (mxArray*)
                       MatlabExcavatorModel_P.SFunction_P4_Size_g);
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

    /* Level2 S-Function Block: MatlabExcavatorModel/<S5>/S-Function1 (p2torq2_435) */
    {
      SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[4];

      /* timing info */
      time_T *sfcnPeriod =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn4.sfcnPeriod;
      time_T *sfcnOffset =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn4.sfcnOffset;
      int_T *sfcnTsMap = MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn4.sfcnTsMap;
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
                         [4]);
      }

      ssSetRTWSfcnInfo(rts, MatlabExcavatorModel_M->sfcnInfo);

      /* Allocate memory of model methods 2 */
      {
        ssSetModelMethods2(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods2[4]);
      }

      /* Allocate memory of model methods 3 */
      {
        ssSetModelMethods3(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods3[4]);
      }

      /* Allocate memory for states auxilliary information */
      {
        ssSetStatesInfo2(rts,
                         &MatlabExcavatorModel_M->NonInlinedSFcns.statesInfo2[4]);
      }

      /* inputs */
      {
        _ssSetNumInputPorts(rts, 4);
        ssSetPortInfoForInputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn4.inputPortInfo[0]);

        /* port 0 */
        {
          ssSetInputPortRequiredContiguous(rts, 0, 1);
          ssSetInputPortSignal(rts, 0, &MatlabExcavatorModel_B.SFunction_o1_i[1]);
          _ssSetInputPortNumDimensions(rts, 0, 1);
          ssSetInputPortWidth(rts, 0, 1);
        }

        /* port 1 */
        {
          ssSetInputPortRequiredContiguous(rts, 1, 1);
          ssSetInputPortSignal(rts, 1, &MatlabExcavatorModel_B.SFunction_o2_h[1]);
          _ssSetInputPortNumDimensions(rts, 1, 1);
          ssSetInputPortWidth(rts, 1, 1);
        }

        /* port 2 */
        {
          ssSetInputPortRequiredContiguous(rts, 2, 1);
          ssSetInputPortSignal(rts, 2, &MatlabExcavatorModel_B.SFunction_o1[1]);
          _ssSetInputPortNumDimensions(rts, 2, 1);
          ssSetInputPortWidth(rts, 2, 1);
        }

        /* port 3 */
        {
          ssSetInputPortRequiredContiguous(rts, 3, 1);
          ssSetInputPortSignal(rts, 3, &MatlabExcavatorModel_B.SFunction_o2[1]);
          _ssSetInputPortNumDimensions(rts, 3, 1);
          ssSetInputPortWidth(rts, 3, 1);
        }
      }

      /* outputs */
      {
        ssSetPortInfoForOutputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn4.outputPortInfo[0]);
        _ssSetNumOutputPorts(rts, 1);

        /* port 0 */
        {
          _ssSetOutputPortNumDimensions(rts, 0, 1);
          ssSetOutputPortWidth(rts, 0, 1);
          ssSetOutputPortSignal(rts, 0, ((real_T *)
            &MatlabExcavatorModel_B.SFunction1));
        }
      }

      /* path info */
      ssSetModelName(rts, "S-Function1");
      ssSetPath(rts,
                "MatlabExcavatorModel/Pressure to Torque Calculation/S-Function1");
      ssSetRTModel(rts,MatlabExcavatorModel_M);
      ssSetParentSS(rts, (NULL));
      ssSetRootSS(rts, rts);
      ssSetVersion(rts, SIMSTRUCT_VERSION_LEVEL2);

      /* parameters */
      {
        mxArray **sfcnParams = (mxArray **)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn4.params;
        ssSetSFcnParamsCount(rts, 3);
        ssSetSFcnParamsPtr(rts, &sfcnParams[0]);
        ssSetSFcnParam(rts, 0, (mxArray*)
                       MatlabExcavatorModel_P.SFunction1_P1_Size);
        ssSetSFcnParam(rts, 1, (mxArray*)
                       MatlabExcavatorModel_P.SFunction1_P2_Size);
        ssSetSFcnParam(rts, 2, (mxArray*)
                       MatlabExcavatorModel_P.SFunction1_P3_Size);
      }

      /* registration */
      p2torq2_435(rts);
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

    /* Level2 S-Function Block: MatlabExcavatorModel/<S5>/S-Function2 (p2torq3_435) */
    {
      SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[5];

      /* timing info */
      time_T *sfcnPeriod =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn5.sfcnPeriod;
      time_T *sfcnOffset =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn5.sfcnOffset;
      int_T *sfcnTsMap = MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn5.sfcnTsMap;
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
                         [5]);
      }

      ssSetRTWSfcnInfo(rts, MatlabExcavatorModel_M->sfcnInfo);

      /* Allocate memory of model methods 2 */
      {
        ssSetModelMethods2(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods2[5]);
      }

      /* Allocate memory of model methods 3 */
      {
        ssSetModelMethods3(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods3[5]);
      }

      /* Allocate memory for states auxilliary information */
      {
        ssSetStatesInfo2(rts,
                         &MatlabExcavatorModel_M->NonInlinedSFcns.statesInfo2[5]);
      }

      /* inputs */
      {
        _ssSetNumInputPorts(rts, 4);
        ssSetPortInfoForInputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn5.inputPortInfo[0]);

        /* port 0 */
        {
          ssSetInputPortRequiredContiguous(rts, 0, 1);
          ssSetInputPortSignal(rts, 0, &MatlabExcavatorModel_B.SFunction_o1_i[2]);
          _ssSetInputPortNumDimensions(rts, 0, 1);
          ssSetInputPortWidth(rts, 0, 1);
        }

        /* port 1 */
        {
          ssSetInputPortRequiredContiguous(rts, 1, 1);
          ssSetInputPortSignal(rts, 1, &MatlabExcavatorModel_B.SFunction_o2_h[2]);
          _ssSetInputPortNumDimensions(rts, 1, 1);
          ssSetInputPortWidth(rts, 1, 1);
        }

        /* port 2 */
        {
          ssSetInputPortRequiredContiguous(rts, 2, 1);
          ssSetInputPortSignal(rts, 2, &MatlabExcavatorModel_B.SFunction_o1[2]);
          _ssSetInputPortNumDimensions(rts, 2, 1);
          ssSetInputPortWidth(rts, 2, 1);
        }

        /* port 3 */
        {
          ssSetInputPortRequiredContiguous(rts, 3, 1);
          ssSetInputPortSignal(rts, 3, &MatlabExcavatorModel_B.SFunction_o2[2]);
          _ssSetInputPortNumDimensions(rts, 3, 1);
          ssSetInputPortWidth(rts, 3, 1);
        }
      }

      /* outputs */
      {
        ssSetPortInfoForOutputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn5.outputPortInfo[0]);
        _ssSetNumOutputPorts(rts, 1);

        /* port 0 */
        {
          _ssSetOutputPortNumDimensions(rts, 0, 1);
          ssSetOutputPortWidth(rts, 0, 1);
          ssSetOutputPortSignal(rts, 0, ((real_T *)
            &MatlabExcavatorModel_B.SFunction2));
        }
      }

      /* path info */
      ssSetModelName(rts, "S-Function2");
      ssSetPath(rts,
                "MatlabExcavatorModel/Pressure to Torque Calculation/S-Function2");
      ssSetRTModel(rts,MatlabExcavatorModel_M);
      ssSetParentSS(rts, (NULL));
      ssSetRootSS(rts, rts);
      ssSetVersion(rts, SIMSTRUCT_VERSION_LEVEL2);

      /* parameters */
      {
        mxArray **sfcnParams = (mxArray **)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn5.params;
        ssSetSFcnParamsCount(rts, 3);
        ssSetSFcnParamsPtr(rts, &sfcnParams[0]);
        ssSetSFcnParam(rts, 0, (mxArray*)
                       MatlabExcavatorModel_P.SFunction2_P1_Size);
        ssSetSFcnParam(rts, 1, (mxArray*)
                       MatlabExcavatorModel_P.SFunction2_P2_Size);
        ssSetSFcnParam(rts, 2, (mxArray*)
                       MatlabExcavatorModel_P.SFunction2_P3_Size);
      }

      /* registration */
      p2torq3_435(rts);
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

    /* Level2 S-Function Block: MatlabExcavatorModel/<S5>/S-Function3 (p2torq4_435) */
    {
      SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[6];

      /* timing info */
      time_T *sfcnPeriod =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn6.sfcnPeriod;
      time_T *sfcnOffset =
        MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn6.sfcnOffset;
      int_T *sfcnTsMap = MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn6.sfcnTsMap;
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
                         [6]);
      }

      ssSetRTWSfcnInfo(rts, MatlabExcavatorModel_M->sfcnInfo);

      /* Allocate memory of model methods 2 */
      {
        ssSetModelMethods2(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods2[6]);
      }

      /* Allocate memory of model methods 3 */
      {
        ssSetModelMethods3(rts,
                           &MatlabExcavatorModel_M->NonInlinedSFcns.methods3[6]);
      }

      /* Allocate memory for states auxilliary information */
      {
        ssSetStatesInfo2(rts,
                         &MatlabExcavatorModel_M->NonInlinedSFcns.statesInfo2[6]);
      }

      /* inputs */
      {
        _ssSetNumInputPorts(rts, 4);
        ssSetPortInfoForInputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn6.inputPortInfo[0]);

        /* port 0 */
        {
          ssSetInputPortRequiredContiguous(rts, 0, 1);
          ssSetInputPortSignal(rts, 0, &MatlabExcavatorModel_B.SFunction_o1_i[3]);
          _ssSetInputPortNumDimensions(rts, 0, 1);
          ssSetInputPortWidth(rts, 0, 1);
        }

        /* port 1 */
        {
          ssSetInputPortRequiredContiguous(rts, 1, 1);
          ssSetInputPortSignal(rts, 1, &MatlabExcavatorModel_B.SFunction_o2_h[3]);
          _ssSetInputPortNumDimensions(rts, 1, 1);
          ssSetInputPortWidth(rts, 1, 1);
        }

        /* port 2 */
        {
          ssSetInputPortRequiredContiguous(rts, 2, 1);
          ssSetInputPortSignal(rts, 2, &MatlabExcavatorModel_B.SFunction_o1[3]);
          _ssSetInputPortNumDimensions(rts, 2, 1);
          ssSetInputPortWidth(rts, 2, 1);
        }

        /* port 3 */
        {
          ssSetInputPortRequiredContiguous(rts, 3, 1);
          ssSetInputPortSignal(rts, 3, &MatlabExcavatorModel_B.SFunction_o2[3]);
          _ssSetInputPortNumDimensions(rts, 3, 1);
          ssSetInputPortWidth(rts, 3, 1);
        }
      }

      /* outputs */
      {
        ssSetPortInfoForOutputs(rts,
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn6.outputPortInfo[0]);
        _ssSetNumOutputPorts(rts, 1);

        /* port 0 */
        {
          _ssSetOutputPortNumDimensions(rts, 0, 1);
          ssSetOutputPortWidth(rts, 0, 1);
          ssSetOutputPortSignal(rts, 0, ((real_T *)
            &MatlabExcavatorModel_B.SFunction3));
        }
      }

      /* states */
      ssSetDiscStates(rts, (real_T *) &MatlabExcavatorModel_DW.SFunction3_DSTATE);

      /* path info */
      ssSetModelName(rts, "S-Function3");
      ssSetPath(rts,
                "MatlabExcavatorModel/Pressure to Torque Calculation/S-Function3");
      ssSetRTModel(rts,MatlabExcavatorModel_M);
      ssSetParentSS(rts, (NULL));
      ssSetRootSS(rts, rts);
      ssSetVersion(rts, SIMSTRUCT_VERSION_LEVEL2);

      /* parameters */
      {
        mxArray **sfcnParams = (mxArray **)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn6.params;
        ssSetSFcnParamsCount(rts, 3);
        ssSetSFcnParamsPtr(rts, &sfcnParams[0]);
        ssSetSFcnParam(rts, 0, (mxArray*)
                       MatlabExcavatorModel_P.SFunction3_P1_Size);
        ssSetSFcnParam(rts, 1, (mxArray*)
                       MatlabExcavatorModel_P.SFunction3_P2_Size);
        ssSetSFcnParam(rts, 2, (mxArray*)
                       MatlabExcavatorModel_P.SFunction3_P3_Size);
      }

      /* work vectors */
      {
        struct _ssDWorkRecord *dWorkRecord = (struct _ssDWorkRecord *)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn6.dWork;
        struct _ssDWorkAuxRecord *dWorkAuxRecord = (struct _ssDWorkAuxRecord *)
          &MatlabExcavatorModel_M->NonInlinedSFcns.Sfcn6.dWorkAux;
        ssSetSFcnDWork(rts, dWorkRecord);
        ssSetSFcnDWorkAux(rts, dWorkAuxRecord);
        _ssSetNumDWork(rts, 1);

        /* DSTATE */
        ssSetDWorkWidth(rts, 0, 1);
        ssSetDWorkDataType(rts, 0,SS_DOUBLE);
        ssSetDWorkComplexSignal(rts, 0, 0);
        ssSetDWorkUsedAsDState(rts, 0, 1);
        ssSetDWork(rts, 0, &MatlabExcavatorModel_DW.SFunction3_DSTATE);
      }

      /* registration */
      p2torq4_435(rts);
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

  /* Level2 S-Function Block: '<S4>/limiter' (limiterg_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[0];
    sfcnStart(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* Level2 S-Function Block: '<S5>/S-Function' (p2torq1_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[3];
    sfcnStart(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* Level2 S-Function Block: '<S5>/S-Function1' (p2torq2_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[4];
    sfcnStart(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* Level2 S-Function Block: '<S5>/S-Function2' (p2torq3_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[5];
    sfcnStart(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* Level2 S-Function Block: '<S5>/S-Function3' (p2torq4_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[6];
    sfcnStart(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* InitializeConditions for DiscreteIntegrator: '<S4>/Integrator (qd to q)' */
  MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[0] =
    MatlabExcavatorModel_P.Integratorqdtoq_IC[0];
  MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[1] =
    MatlabExcavatorModel_P.Integratorqdtoq_IC[1];
  MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[2] =
    MatlabExcavatorModel_P.Integratorqdtoq_IC[2];
  MatlabExcavatorModel_DW.Integratorqdtoq_DSTATE[3] =
    MatlabExcavatorModel_P.Integratorqdtoq_IC[3];

  /* InitializeConditions for Delay: '<S4>/Integer Delay' */
  MatlabExcavatorModel_DW.IntegerDelay_DSTATE[0] =
    MatlabExcavatorModel_P.IntegerDelay_InitialCondition;
  MatlabExcavatorModel_DW.IntegerDelay_DSTATE[1] =
    MatlabExcavatorModel_P.IntegerDelay_InitialCondition;
  MatlabExcavatorModel_DW.IntegerDelay_DSTATE[2] =
    MatlabExcavatorModel_P.IntegerDelay_InitialCondition;
  MatlabExcavatorModel_DW.IntegerDelay_DSTATE[3] =
    MatlabExcavatorModel_P.IntegerDelay_InitialCondition;

  /* InitializeConditions for DiscreteIntegrator: '<S4>/Integrator (qdd to qd)' */
  MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[0] =
    MatlabExcavatorModel_P.Integratorqddtoqd_IC[0];
  MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[1] =
    MatlabExcavatorModel_P.Integratorqddtoqd_IC[1];
  MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[2] =
    MatlabExcavatorModel_P.Integratorqddtoqd_IC[2];
  MatlabExcavatorModel_DW.Integratorqddtoqd_DSTATE[3] =
    MatlabExcavatorModel_P.Integratorqddtoqd_IC[3];
  MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[0] = 2;
  MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[1] = 2;
  MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[2] = 2;
  MatlabExcavatorModel_DW.Integratorqddtoqd_PrevResetStat[3] = 2;

  /* InitializeConditions for UnitDelay: '<Root>/Unit Delay1' */
  MatlabExcavatorModel_DW.UnitDelay1_DSTATE[0] =
    MatlabExcavatorModel_P.UnitDelay1_InitialCondition;
  MatlabExcavatorModel_DW.UnitDelay1_DSTATE[1] =
    MatlabExcavatorModel_P.UnitDelay1_InitialCondition;
  MatlabExcavatorModel_DW.UnitDelay1_DSTATE[2] =
    MatlabExcavatorModel_P.UnitDelay1_InitialCondition;
  MatlabExcavatorModel_DW.UnitDelay1_DSTATE[3] =
    MatlabExcavatorModel_P.UnitDelay1_InitialCondition;

  /* Level2 S-Function Block: '<S4>/limiter' (limiterg_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[0];
    sfcnInitializeConditions(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* InitializeConditions for Delay: '<S7>/Integer Delay3' */
  MatlabExcavatorModel_DW.IntegerDelay3_DSTATE =
    MatlabExcavatorModel_P.IntegerDelay3_InitialCondition;

  /* InitializeConditions for Delay: '<S7>/Integer Delay1' */
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[0] =
    MatlabExcavatorModel_P.IntegerDelay1_InitialCondition[0];
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[1] =
    MatlabExcavatorModel_P.IntegerDelay1_InitialCondition[1];
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[2] =
    MatlabExcavatorModel_P.IntegerDelay1_InitialCondition[2];
  MatlabExcavatorModel_DW.IntegerDelay1_DSTATE[3] =
    MatlabExcavatorModel_P.IntegerDelay1_InitialCondition[3];

  /* InitializeConditions for Delay: '<S7>/Integer Delay2' */
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[0] =
    MatlabExcavatorModel_P.IntegerDelay2_InitialCondition[0];
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[1] =
    MatlabExcavatorModel_P.IntegerDelay2_InitialCondition[1];
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[2] =
    MatlabExcavatorModel_P.IntegerDelay2_InitialCondition[2];
  MatlabExcavatorModel_DW.IntegerDelay2_DSTATE[3] =
    MatlabExcavatorModel_P.IntegerDelay2_InitialCondition[3];

  /* Level2 S-Function Block: '<S3>/S-Function' (cylmodel_435_ryder) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[2];
    sfcnInitializeConditions(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* Level2 S-Function Block: '<S5>/S-Function' (p2torq1_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[3];
    sfcnInitializeConditions(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* Level2 S-Function Block: '<S5>/S-Function1' (p2torq2_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[4];
    sfcnInitializeConditions(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* Level2 S-Function Block: '<S5>/S-Function2' (p2torq3_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[5];
    sfcnInitializeConditions(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }

  /* Level2 S-Function Block: '<S5>/S-Function3' (p2torq4_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[6];
    sfcnInitializeConditions(rts);
    if (ssGetErrorStatus(rts) != (NULL))
      return;
  }
}

/* Model terminate function */
void MatlabExcavatorModel_terminate(void)
{
  /* Level2 S-Function Block: '<S4>/limiter' (limiterg_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[0];
    sfcnTerminate(rts);
  }

  /* Level2 S-Function Block: '<S6>/S-Function' (j2c_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[1];
    sfcnTerminate(rts);
  }

  /* Level2 S-Function Block: '<S3>/S-Function' (cylmodel_435_ryder) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[2];
    sfcnTerminate(rts);
  }

  /* Level2 S-Function Block: '<S5>/S-Function' (p2torq1_435_brake) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[3];
    sfcnTerminate(rts);
  }

  /* Level2 S-Function Block: '<S5>/S-Function1' (p2torq2_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[4];
    sfcnTerminate(rts);
  }

  /* Level2 S-Function Block: '<S5>/S-Function2' (p2torq3_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[5];
    sfcnTerminate(rts);
  }

  /* Level2 S-Function Block: '<S5>/S-Function3' (p2torq4_435) */
  {
    SimStruct *rts = MatlabExcavatorModel_M->childSfunctions[6];
    sfcnTerminate(rts);
  }
}

/*
 * File trailer for generated code.
 *
 * [EOF]
 */
