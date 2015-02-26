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
	for (int i = 0; i < 4; i++)
	{
		MatlabExcavatorModel_U.Q[i] = Q[i];
		MatlabExcavatorModel_U.Qd[i] = Qd[i];
		MatlabExcavatorModel_U.Flow[i] = Flow[i];
	}

	MatlabExcavatorModel_step();

	Fuel[0] = MatlabExcavatorModel_Y.FuelInstant;
	Fuel[1] = MatlabExcavatorModel_Y.FuelTotal;

	for (int i = 0; i < 4; i++)
	{
		CylinderPositions[i] = MatlabExcavatorModel_Y.CylinderPosition[i];
		CylinderVelocities[i] = MatlabExcavatorModel_Y.CylinderVelocity[i];
		JointTorques[i] = MatlabExcavatorModel_Y.JointTorques[i];
	}

	return MatlabExcavatorModel_Y.Time;
}

void SamTerminate(void)
{
	MatlabExcavatorModel_terminate();
}
