#include "SamHeader.h"
#include "MatlabExcavatorModel.h"

// Timing.stepSize0 = Big Step Size

void SamInit(void)
{
	MatlabExcavatorModel_initialize();
}

float SamUpdate(
	float * Q,
	float * QD, 
	float * Fuel, 
	float * RandBack, 
	float * CylinderPositions,
	float * CylinderVelocities,
	float BucketMass, 
	float * SDF, 
	float * SDM, 
	float * SWF)
{	
	MatlabExcavatorModel_U.Soil_Dig_Force[0] = SDF[0];
	MatlabExcavatorModel_U.Soil_Dig_Force[1] = SDF[1];
	MatlabExcavatorModel_U.Soil_Dig_Force[2] = SDF[2];

	MatlabExcavatorModel_U.Soil_Dig_Moment[0] = SDM[0];
	MatlabExcavatorModel_U.Soil_Dig_Moment[1] = SDM[1];
	MatlabExcavatorModel_U.Soil_Dig_Moment[2] = SDM[2];

	MatlabExcavatorModel_U.Soil_Wrist_Force[0] = SWF[0];
	MatlabExcavatorModel_U.Soil_Wrist_Force[1] = SWF[1];
	MatlabExcavatorModel_U.Soil_Wrist_Force[2] = SWF[2];

	MatlabExcavatorModel_U.Soil_Bucket_Mass = BucketMass;

//	MatlabExcavatorModel_B.q[0] = Q[0]; // Settting Q to the rotate value!;

	MatlabExcavatorModel_step();

	Q[0] = MatlabExcavatorModel_Y.Q[0];
	Q[1] = MatlabExcavatorModel_Y.Q[1];
	Q[2] = MatlabExcavatorModel_Y.Q[2];
	Q[3] = MatlabExcavatorModel_Y.Q[3];

	QD[0] = MatlabExcavatorModel_Y.Qd[0];
	QD[1] = MatlabExcavatorModel_Y.Qd[1];
	QD[2] = MatlabExcavatorModel_Y.Qd[2];
	QD[3] = MatlabExcavatorModel_Y.Qd[3];

	RandBack[0] = MatlabExcavatorModel_Y.BreakOn[0];
	RandBack[1] = MatlabExcavatorModel_Y.Flow[0];

	Fuel[0] = MatlabExcavatorModel_Y.FuelInstant;
	Fuel[1] = MatlabExcavatorModel_Y.FuelTotal;

	CylinderPositions[0] = MatlabExcavatorModel_Y.CylinderPosition[0];
	CylinderPositions[1] = MatlabExcavatorModel_Y.CylinderPosition[1];
	CylinderPositions[2] = MatlabExcavatorModel_Y.CylinderPosition[2];
	CylinderPositions[3] = MatlabExcavatorModel_Y.CylinderPosition[3];

	CylinderVelocities[0] = MatlabExcavatorModel_Y.CylinderVelocity[0];
	CylinderVelocities[1] = MatlabExcavatorModel_Y.CylinderVelocity[1];
	CylinderVelocities[2] = MatlabExcavatorModel_Y.CylinderVelocity[2];
	CylinderVelocities[3] = MatlabExcavatorModel_Y.CylinderVelocity[3];

	return MatlabExcavatorModel_Y.Time;
}

void SamTerminate(void)
{
	MatlabExcavatorModel_terminate();
}

void setJointFlows(float * flows)
{
	MatlabExcavatorModel_U.Flow1 = flows[0];
	MatlabExcavatorModel_U.Flow2 = flows[1];
	MatlabExcavatorModel_U.Flow3 = flows[2];
	MatlabExcavatorModel_U.Flow4 = flows[3];
}

