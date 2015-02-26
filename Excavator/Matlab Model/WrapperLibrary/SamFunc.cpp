#include "SamHeader.h"
#include "MatlabExcavatorModel.h"

// Timing.stepSize0 = Big Step Size

void SamInit(void)
{
	MatlabExcavatorModel_initialize();
}

float SamUpdate(
	float * CylinderPositions,
	float * CylinderVelocities,
	float * Flow,
	// Back
	float * Fuel,
	float * CabTorque,
	float * JointForces
	)
{
	for (int i = 0; i < 4; i++)
	{
		MatlabExcavatorModel_U.CylinderPosition[i] = CylinderPositions[i];
		MatlabExcavatorModel_U.CylinderVelocity[i] = CylinderVelocities[i];
		MatlabExcavatorModel_U.Flow[i] = Flow[i];
	}

	MatlabExcavatorModel_step();

	Fuel[0] = MatlabExcavatorModel_Y.FuelInstant;
	Fuel[1] = MatlabExcavatorModel_Y.FuelTotal;
	CabTorque[0] = MatlabExcavatorModel_Y.CabTorque;

	for (int i = 0; i < 4; i++)
	{
		JointForces[i] = MatlabExcavatorModel_Y.JointForce[i];
	}

	return MatlabExcavatorModel_Y.Time;
}

void SamTerminate(void)
{
	MatlabExcavatorModel_terminate();
}
