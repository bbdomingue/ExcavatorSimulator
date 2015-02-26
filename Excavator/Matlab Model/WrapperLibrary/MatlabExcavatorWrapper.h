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
			float * CylinderPositions,
			float * CylinderVelocities,
			float * Flow,
			// Back
			float * Fuel,
			float * CabTorque,
			float * JointForces
			);
		
		void SamTerminateClass();
	};
}
