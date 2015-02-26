#pragma once
class Unmanaged
{
public:
	Unmanaged(void);
	~Unmanaged(void);

	void SamInitWrap(void);

	float SamUpdateWrap(float * Q, float * QD, float * SDF, float * SDM, float * SWF, float BucketMass);

	void SamTerminateWrap(void);

	void setControlModeWrap(int a);
	void setJointTorquesWrap(float t1, float t2, float t3, float t4);
	void setJointPositionsWrap(float t1, float t2, float t3, float t4);
};

