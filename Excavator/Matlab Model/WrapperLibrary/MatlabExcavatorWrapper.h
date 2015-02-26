// WrapperLibrary.h

#pragma once

using namespace System;

namespace MatlabExcavatorWrapper
{
	public ref class MatlabSimulation
	{
		public:
		MatlabSimulation() {};

		void SamInitClass();
		float SamUpdateClass(
			float * Q,
			float * QD, 
			float * Fuel, 
			float * RandBack, 
			float * CylinderPositions,
			float * CylinderVelocities,
			float BucketMass, 
			float * SDF, 
			float * SDM, 
			float * SWF);
		
		void SamTerminateClass();

		void setJointFlowsClass(float * flows);
	};
}
