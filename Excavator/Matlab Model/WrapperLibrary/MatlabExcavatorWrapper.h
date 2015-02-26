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
			float * Qd,
			float * Flow,
			// Back
			float * Fuel, 
			float * CylinderPositions,
			float * CylinderVelocities,
			float * JointTorques);
		
		void SamTerminateClass();
	};
}
