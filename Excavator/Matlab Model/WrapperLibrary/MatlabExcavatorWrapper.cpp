// This is the main DLL file.

#include "stdafx.h"
#include "SamHeader.h"

#include "MatlabExcavatorWrapper.h"

void MatlabExcavatorWrapper::MatlabSimulation::SamInitClass()
{
	SamInit();
}

float MatlabExcavatorWrapper::MatlabSimulation::SamUpdateClass(
	float * CylinderPositions,
	float * CylinderVelocities,
	float * Flow,
	// Back
	float * Fuel,
	float * CabTorque,
	float * JointForces
	)
{
	return SamUpdate(
		CylinderPositions,
		CylinderVelocities,
		Flow,
		// Back
		Fuel,
		CabTorque,
		JointForces		
		);
}

void MatlabExcavatorWrapper::MatlabSimulation::SamTerminateClass()
{
	SamTerminate();
}