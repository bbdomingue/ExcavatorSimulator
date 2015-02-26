#pragma once

#ifdef SAM_DEFINE
	#define SFNC __declspec( dllimport )
#else
	#define SFNC __declspec( dllexport )
#endif // SAM_DEFINE
	
#ifdef __cplusplus
extern "C" {  // only need to export C interface if used by C++ source code
#endif

SFNC void SamInit();
	
SFNC float SamUpdate(
	float * CylinderPositions,
	float * CylinderVelocities,
	float * Flow,
	// Back
	float * Fuel, 
	float * CabTorque,
	float * JointForces
	);

SFNC void SamTerminate(void);

SFNC void setJointFlows(float * flows);

#ifdef __cplusplus
}
#endif