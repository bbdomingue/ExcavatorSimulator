#include "SamHeader.h"
#include "MatlabExcavatorModel.h"

// Timing.stepSize0 = Big Step Size

void SamInit(void)
{
	MatlabExcavatorModel_initialize();
}

float SamUpdate(
	float * Q,
	float * Qd,
	float * Flow,
	// Back
	float * Fuel, 
	float * CylinderPositions,
	float * CylinderVelocities,
	float * JointTorques)
{	
	MatlabExcavatorModel_U.Flow[0] = Flow[0];
	MatlabExcavatorModel_U.Flow[1] = Flow[1];
	MatlabExcavatorModel_U.Flow[2] = Flow[2];
	MatlabExcavatorModel_U.Flow[3] = Flow[3];

	MatlabExcavatorModel_U.Q[0] = Q[0];
	MatlabExcavatorModel_U.Q[1] = Q[1];
	MatlabExcavatorModel_U.Q[2] = Q[2];
	MatlabExcavatorModel_U.Q[3] = Q[3];

	MatlabExcavatorModel_U.Qd[0] = Qd[0];
	MatlabExcavatorModel_U.Qd[1] = Qd[1];
	MatlabExcavatorModel_U.Qd[2] = Qd[2];
	MatlabExcavatorModel_U.Qd[3] = Qd[3];

	MatlabExcavatorModel_step();

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

	JointTorques[0] = MatlabExcavatorModel_Y.JointTorques[0];
	JointTorques[1] = MatlabExcavatorModel_Y.JointTorques[1];
	JointTorques[2] = MatlabExcavatorModel_Y.JointTorques[2];
	JointTorques[3] = MatlabExcavatorModel_Y.JointTorques[3];

	return MatlabExcavatorModel_Y.Time;
}

void SamTerminate(void)
{
	MatlabExcavatorModel_terminate();
}
