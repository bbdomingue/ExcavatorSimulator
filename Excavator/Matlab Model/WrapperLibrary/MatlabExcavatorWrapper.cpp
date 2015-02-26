// This is the main DLL file.

#include "stdafx.h"
#include "SamHeader.h"

#include "MatlabExcavatorWrapper.h"

void MatlabExcavatorWrapper::MatlabSimulation::SamInitClass()
{
	SamInit();
}

float MatlabExcavatorWrapper::MatlabSimulation::SamUpdateClass(
	float * Q,
	float * Qd,
	float * Flow,
	// Back
	float * Fuel, 
	float * CylinderPositions,
	float * CylinderVelocities,
	float * JointTorques)
{
	return SamUpdate(
		Q,
		Qd,
		Flow,
		// Back
		Fuel, 
		CylinderPositions, 
		CylinderVelocities, 
		JointTorques);
}

void MatlabExcavatorWrapper::MatlabSimulation::SamTerminateClass()
{
	SamTerminate();
}