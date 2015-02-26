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
	return SamUpdate(
		Q,
		QD,
		Fuel, 
		RandBack,
		CylinderPositions, 
		CylinderVelocities, 
		BucketMass, 
		SDF, 
		SDM, 
		SWF);
}

void MatlabExcavatorWrapper::MatlabSimulation::SamTerminateClass()
{
	SamTerminate();
}

void MatlabExcavatorWrapper::MatlabSimulation::setJointFlowsClass(float * flows)
{
	setJointFlows(flows);
}