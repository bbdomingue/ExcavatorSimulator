// This code contains NVIDIA Confidential Information and is disclosed to you
// under a form of NVIDIA software license agreement provided separately to you.
//
// Notice
// NVIDIA Corporation and its licensors retain all intellectual property and
// proprietary rights in and to this software and related documentation and
// any modifications thereto. Any use, reproduction, disclosure, or
// distribution of this software and related documentation without an express
// license agreement from NVIDIA Corporation is strictly prohibited.
//
// ALL NVIDIA DESIGN SPECIFICATIONS, CODE ARE PROVIDED "AS IS.". NVIDIA MAKES
// NO WARRANTIES, EXPRESSED, IMPLIED, STATUTORY, OR OTHERWISE WITH RESPECT TO
// THE MATERIALS, AND EXPRESSLY DISCLAIMS ALL IMPLIED WARRANTIES OF NONINFRINGEMENT,
// MERCHANTABILITY, AND FITNESS FOR A PARTICULAR PURPOSE.
//
// Information and code furnished is believed to be accurate and reliable.
// However, NVIDIA Corporation assumes no responsibility for the consequences of use of such
// information or for any infringement of patents or other rights of third parties that may
// result from its use. No license is granted by implication or otherwise under any patent
// or patent rights of NVIDIA Corporation. Details are subject to change without notice.
// This code supersedes and replaces all information previously supplied.
// NVIDIA Corporation products are not authorized for use as critical
// components in life support devices or systems without express written approval of
// NVIDIA Corporation.
//
// Copyright (c) 2008-2013 NVIDIA Corporation. All rights reserved.
// Copyright (c) 2004-2008 AGEIA Technologies, Inc. All rights reserved.
// Copyright (c) 2001-2004 NovodeX AG. All rights reserved.  


#include "ExtD6Joint.h"
#include "ExtConstraintHelper.h"
#include "CmRenderOutput.h"
#include "CmConeLimitHelper.h"
#include "PxTolerancesScale.h"

#ifdef PX_PS3
#include "PS3/ExtD6JointSpu.h"
#endif

#include "common/PxSerialFramework.h"

using namespace physx;
using namespace Ext;

PxD6Joint* physx::PxD6JointCreate(PxPhysics& physics,
								  PxRigidActor* actor0, const PxTransform& localFrame0,
								  PxRigidActor* actor1, const PxTransform& localFrame1)
{
	PX_CHECK_AND_RETURN_NULL(localFrame0.isSane(), "PxD6JointCreate: local frame 0 is not a valid transform"); 
	PX_CHECK_AND_RETURN_NULL(localFrame1.isSane(), "PxD6JointCreate: local frame 1 is not a valid transform"); 
	PX_CHECK_AND_RETURN_NULL(actor0 != actor1, "PxD6JointCreate: actors must be different");
	PX_CHECK_AND_RETURN_NULL((actor0 && actor0->is<PxRigidBody>()) || (actor1 && actor1->is<PxRigidBody>()), "PxD6JointCreate: at least one actor must be dynamic");

	D6Joint* j = PX_NEW(D6Joint)(physics.getTolerancesScale(), actor0, localFrame0, actor1, localFrame1);

	if(j->attach(physics, actor0, actor1))
		return j;

	PX_DELETE(j);
	return NULL;
}



D6Joint::D6Joint(const PxTolerancesScale& scale,
				 PxRigidActor* actor0, const PxTransform& localFrame0, 
				 PxRigidActor* actor1, const PxTransform& localFrame1)
:	D6JointT(PxJointConcreteType::eD6, PxBaseFlag::eOWNS_MEMORY | PxBaseFlag::eIS_RELEASABLE)	
,	mRecomputeMotion(true)
,	mRecomputeLimits(true)
{
	D6JointData* data = reinterpret_cast<D6JointData*>(PX_ALLOC(sizeof(D6JointData), PX_DEBUG_EXP("D6JointData")));
	mData = data;

	initCommonData(*data,actor0, localFrame0, actor1, localFrame1);
	for(PxU32 i=0;i<6;i++)
		data->motion[i] = PxD6Motion::eLOCKED;

	data->twistLimit = PxJointAngularLimitPair(-PxPi/2, PxPi/2);
	data->swingLimit = PxJointLimitCone(PxPi/2, PxPi/2);
	data->linearLimit = PxJointLinearLimit(scale, PX_MAX_F32);
	data->linearMinDist = 1e-6f*scale.length;

	for(PxU32 i=0;i<PxD6Drive::eCOUNT;i++)
		data->drive[i] = PxD6JointDrive();

	data->drivePosition = PxTransform(PxIdentity);
	data->driveLinearVelocity = PxVec3(0);
	data->driveAngularVelocity = PxVec3(0);

	data->projectionLinearTolerance = 1e10;
	data->projectionAngularTolerance = PxPi;
}

PxD6Motion::Enum D6Joint::getMotion(PxD6Axis::Enum index) const
{	
	return data().motion[index];	
}

void D6Joint::setMotion(PxD6Axis::Enum index, PxD6Motion::Enum t)
{	
	data().motion[index] = t; 
	mRecomputeMotion = true; 
	markDirty(); 
}


PxReal D6Joint::getTwist() const
{
	PxQuat q = getRelativeTransform().q, swing, twist;
	Ps::separateSwingTwist(q, swing, twist);
	PxReal angle = twist.getAngle();
	return angle<=PxPi ? angle : angle - 2*PxPi;
}

PxReal D6Joint::getSwingYAngle()	const
{
	PxQuat q = getRelativeTransform().q, swing, twist;
	Ps::separateSwingTwist(q, swing, twist);
	PxReal tqY = Ps::tanHalf(swing.y, swing.w);
	PxReal angle = 4*PxAtan(tqY);
	return angle<=PxPi ? angle : angle - 2*PxPi;
}

PxReal D6Joint::getSwingZAngle()	const
{
	PxQuat q = getRelativeTransform().q, swing, twist;
	Ps::separateSwingTwist(q, swing, twist);
	PxReal tqZ = Ps::tanHalf(swing.z, swing.w);
	PxReal angle = 4*PxAtan(tqZ);
	return angle<=PxPi ? angle : angle - 2*PxPi;
}


PxD6JointDrive D6Joint::getDrive(PxD6Drive::Enum index) const
{	
	return data().drive[index];	
}

void D6Joint::setDrive(PxD6Drive::Enum index, const PxD6JointDrive& d)
{	
	PX_CHECK_AND_RETURN(d.isValid(), "PxD6Joint::setDrive: drive is invalid"); 

	data().drive[index] = d; 
	mRecomputeMotion = true; 
	markDirty(); 
}

PxJointLinearLimit D6Joint::getLinearLimit() const
{	

	return data().linearLimit;	
}

void D6Joint::setLinearLimit(const PxJointLinearLimit& l)
{	
	PX_CHECK_AND_RETURN(l.isValid(), "PxD6Joint::setLinearLimit: limit invalid");
	data().linearLimit = l; 
	mRecomputeLimits = true; 
	markDirty(); 
}

PxJointAngularLimitPair D6Joint::getTwistLimit() const
{	
	return data().twistLimit;	
}

void D6Joint::setTwistLimit(const PxJointAngularLimitPair& l)
{	
	PX_CHECK_AND_RETURN(l.isValid(), "PxD6Joint::setTwistLimit: limit invalid");
	PX_CHECK_AND_RETURN(l.lower>-PxTwoPi && l.upper<PxTwoPi , "PxD6Joint::twist limit must be strictly -2*PI and 2*PI");
	PX_CHECK_AND_RETURN(l.upper - l.lower < PxTwoPi, "PxD6Joint::twist limit range must be strictly less than 2*PI");

	data().twistLimit = l; 
	mRecomputeLimits = true; 
	markDirty(); 
}

PxJointLimitCone D6Joint::getSwingLimit() const
{	
	return data().swingLimit;	
}

void D6Joint::setSwingLimit(const PxJointLimitCone& l)
{	
	PX_CHECK_AND_RETURN(l.isValid(), "PxD6Joint::setSwingLimit: limit invalid");

	data().swingLimit = l; 
	mRecomputeLimits = true; 
	markDirty(); 
}

PxTransform D6Joint::getDrivePosition() const
{	
	return data().drivePosition;	
}

void D6Joint::setDrivePosition(const PxTransform& pose)
{	
	PX_CHECK_AND_RETURN(pose.isSane(), "PxD6Joint::setDrivePosition: pose invalid");
	data().drivePosition = pose.getNormalized(); 
	markDirty(); 
}

void D6Joint::getDriveVelocity(PxVec3& linear, PxVec3& angular)	const
{	
	linear = data().driveLinearVelocity;
	angular = data().driveAngularVelocity; 
}

void D6Joint::setDriveVelocity(const PxVec3& linear,
									 const PxVec3& angular)
{	
	PX_CHECK_AND_RETURN(linear.isFinite() && angular.isFinite(), "PxD6Joint::setDriveVelocity: velocity invalid");
	data().driveLinearVelocity = linear; 
	data().driveAngularVelocity = angular; 
	markDirty();
}

void D6Joint::setProjectionAngularTolerance(PxReal tolerance)
{	
	PX_CHECK_AND_RETURN(PxIsFinite(tolerance) && tolerance >=0 && tolerance <= PxPi, "PxD6Joint::setProjectionAngularTolerance: tolerance invalid");
	data().projectionAngularTolerance = tolerance;	
	markDirty();
}

PxReal D6Joint::getProjectionAngularTolerance()	const
{	
	return data().projectionAngularTolerance; 
}

void D6Joint::setProjectionLinearTolerance(PxReal tolerance)
{	
	PX_CHECK_AND_RETURN(PxIsFinite(tolerance) && tolerance >=0, "PxD6Joint::setProjectionLinearTolerance: invalid parameter");
	data().projectionLinearTolerance = tolerance;	
	markDirty(); 
}

PxReal D6Joint::getProjectionLinearTolerance() const	
{	
	return data().projectionLinearTolerance;		
}



void* D6Joint::prepareData()
{
	D6JointData& d = data();

	if(mRecomputeLimits)
	{
		d.thSwingY = PxTan(d.swingLimit.yAngle/2);
		d.thSwingZ = PxTan(d.swingLimit.zAngle/2);
		d.thSwingPad = PxTan(d.swingLimit.contactDistance/2);

		d.tqSwingY = PxTan(d.swingLimit.yAngle/4);
		d.tqSwingZ = PxTan(d.swingLimit.zAngle/4);
		d.tqSwingPad = PxTan(d.swingLimit.contactDistance/4);

		d.tqTwistLow  = PxTan(d.twistLimit.lower/4);
		d.tqTwistHigh = PxTan(d.twistLimit.upper/4);
		d.tqTwistPad = PxTan(d.twistLimit.contactDistance/4);
		mRecomputeLimits = false;
	}

	if(mRecomputeMotion)
	{
		d.driving = 0;
		d.limited = 0;
		d.locked = 0;

		for(PxU32 i=0;i<PxD6Axis::eCOUNT;i++)
		{
			if(d.motion[i] == PxD6Motion::eLIMITED)
				d.limited |= 1<<i;
			else if(d.motion[i] == PxD6Motion::eLOCKED)
				d.locked |= 1<<i;
		}

		// a linear direction isn't driven if it's locked
		if(active(PxD6Drive::eX) && d.motion[PxD6Axis::eX]!=PxD6Motion::eLOCKED) d.driving |= 1<< PxD6Drive::eX;
		if(active(PxD6Drive::eY) && d.motion[PxD6Axis::eY]!=PxD6Motion::eLOCKED) d.driving |= 1<< PxD6Drive::eY;
		if(active(PxD6Drive::eZ) && d.motion[PxD6Axis::eZ]!=PxD6Motion::eLOCKED) d.driving |= 1<< PxD6Drive::eZ;

		// SLERP drive requires all angular dofs unlocked, and inhibits swing/twist

		bool swing1Locked = d.motion[PxD6Axis::eSWING1] == PxD6Motion::eLOCKED;
		bool swing2Locked = d.motion[PxD6Axis::eSWING2] == PxD6Motion::eLOCKED;
		bool twistLocked  = d.motion[PxD6Axis::eTWIST]  == PxD6Motion::eLOCKED;

		if(active(PxD6Drive::eSLERP) && !swing1Locked && !swing2Locked && !twistLocked)
			d.driving |= 1<<PxD6Drive::eSLERP;
		else
		{
			if(active(PxD6Drive::eTWIST) && !twistLocked) 
				d.driving |= 1<<PxD6Drive::eTWIST;
			if(active(PxD6Drive::eSWING) && (!swing1Locked || !swing2Locked)) 
				d.driving |= 1<< PxD6Drive::eSWING;
		}

		mRecomputeMotion = false;
	}

	this->D6JointT::prepareData();

	return mData;
}

// Notes:
/*

This used to be in the linear drive model:

	if(motion[PxD6Axis::eX+i] == PxD6Motion::eLIMITED)
	{
		if(data.driveLinearVelocity[i] < 0.0f && cB2cA.p[i] < -mLimits[PxD6Limit::eLINEAR].mValue ||
			data.driveLinearVelocity[i] > 0.0f && cB2cA.p[i] > mLimits[PxD6Limit::eLINEAR].mValue)
			continue;
	}

it doesn't seem like a good idea though, because it turns off drive altogether, despite the fact that positional
drive might pull us back in towards the limit. Might be better to make the drive unilateral so it can only pull
us in from the limit

This used to be in angular locked:

	// Angular locked
	//TODO fix this properly. 	
	if(PxAbs(cB2cA.q.x) < 0.0001f) cB2cA.q.x = 0;
	if(PxAbs(cB2cA.q.y) < 0.0001f) cB2cA.q.y = 0;
	if(PxAbs(cB2cA.q.z) < 0.0001f) cB2cA.q.z = 0;
	if(PxAbs(cB2cA.q.w) < 0.0001f) cB2cA.q.w = 0;
	

*/

namespace
{
	PxReal tanHalfFromSin(PxReal sin)
	{
		return Ps::tanHalf(sin, 1-sin*sin);
	}
}

namespace
{

PxQuat truncateSwing(const PxQuat& in, const PxVec3& twistAxis, PxReal shat, PxReal chat, bool& angularTrunc)
{
	using namespace joint;
	// q.w should be positive, but there can be precision issues here
	PxQuat q = in.w>=0 ? in : -in;

	PxReal tw = twistAxis.dot(q.getImaginaryPart());
	PxQuat twist = PxQuat(PxIdentity);
	if(PxAbs(tw) > 1e-6f)
	{
		PxVec3 tv = twistAxis*PxSqrt(1-tw*tw);
		twist = PxQuat(tv.x, tv.y, tv.z, tw);
	}
	PxQuat swing = q * twist.getConjugate();

	swing = truncateAngular(swing,shat,chat,angularTrunc);
	return angularTrunc ? swing * twist : in;
}

void D6JointProject(const void* constantBlock,
					PxTransform& bodyAToWorld,
					PxTransform& bodyBToWorld,
					bool projectToA)
{
	using namespace joint;
	const D6JointData &data = *reinterpret_cast<const D6JointData*>(constantBlock);

	PxTransform cA2w, cB2w, cB2cA, projected;
	computeDerived(data, bodyAToWorld, bodyBToWorld, cA2w, cB2w, cB2cA);

	PxVec3 v(data.locked & 1 ? cB2cA.p.x : 0,
		     data.locked & 2 ? cB2cA.p.y : 0,
			 data.locked & 4 ? cB2cA.p.z : 0);

	bool linearTrunc, angularTrunc = false;
	projected.p = truncateLinear(v, data.projectionLinearTolerance, linearTrunc) + (cB2cA.p-v);

	PxU32 angularLocked = data.locked>>3;

	PxReal chat = PxCos(data.projectionAngularTolerance/2), shat = PxSin(data.projectionAngularTolerance/2);
	switch(angularLocked)
	{
	case 0: projected.q = cB2cA.q; break;
	case 1: projected.q = cB2cA.q; break;	// TODO
	case 2:	projected.q = cB2cA.q; break;   // TODO
	case 3: projected.q = truncateSwing(cB2cA.q, PxVec3(0,0,1.f), shat, chat, angularTrunc); break;
	case 4: projected.q = cB2cA.q; break;   // TODO
	case 5: projected.q = truncateSwing(cB2cA.q, PxVec3(0,1.f,0), shat, chat, angularTrunc); break; 
	case 6: projected.q = truncateSwing(cB2cA.q, PxVec3(0,0,1.f), shat, chat, angularTrunc); break;
	case 7: projected.q = truncateAngular(cB2cA.q, shat, chat, angularTrunc); break;
	}

	if(linearTrunc || angularTrunc)
		projectTransforms(bodyAToWorld, bodyBToWorld, cA2w, cB2w, projected, data, projectToA);

}


void D6JointVisualize(PxConstraintVisualizer &viz,
 				      const void* constantBlock,
					  const PxTransform& body0Transform,
					  const PxTransform& body1Transform,
					  PxU32 /*flags*/)
{
	using namespace joint;

	const PxU32 SWING1_FLAG = 1<<PxD6Axis::eSWING1, 
			    SWING2_FLAG = 1<<PxD6Axis::eSWING2, 
				TWIST_FLAG  = 1<<PxD6Axis::eTWIST;

	const PxU32 ANGULAR_MASK = SWING1_FLAG | SWING2_FLAG | TWIST_FLAG;
	const PxU32 LINEAR_MASK = 1<<PxD6Axis::eX | 1<<PxD6Axis::eY | 1<<PxD6Axis::eZ;

	PX_UNUSED(ANGULAR_MASK);
	PX_UNUSED(LINEAR_MASK);

	const D6JointData & data = *reinterpret_cast<const D6JointData*>(constantBlock);

	PxTransform cA2w = body0Transform * data.c2b[0];
	PxTransform cB2w = body1Transform * data.c2b[1];

	viz.visualizeJointFrames(cA2w, cB2w);

	if(cA2w.q.dot(cB2w.q)<0)
		cB2w.q = -cB2w.q;

	PxTransform cB2cA = cA2w.transformInv(cB2w);	

	PxQuat swing, twist;
	Ps::separateSwingTwist(cB2cA.q,swing,twist);

	PxMat33 cA2w_m(cA2w.q), cB2w_m(cB2w.q);
	PxVec3 bX = cB2w_m[0], aY = cA2w_m[1], aZ = cA2w_m[2];

	if(data.limited&TWIST_FLAG)
	{
		PxReal tqPhi = Ps::tanHalf(twist.x, twist.w);		// always support (-pi, +pi)
		viz.visualizeAngularLimit(cB2w, data.twistLimit.lower, data.twistLimit.upper, 
			PxAbs(tqPhi) > data.tqTwistHigh + data.tqSwingPad);
	}

	bool swing1Limited = (data.limited & SWING1_FLAG)!=0, swing2Limited = (data.limited & SWING2_FLAG)!=0;

	if(swing1Limited && swing2Limited)
	{
		PxVec3 tanQSwing = PxVec3(0, Ps::tanHalf(swing.z,swing.w), -Ps::tanHalf(swing.y,swing.w));
		Cm::ConeLimitHelper coneHelper(data.tqSwingZ, data.tqSwingY, data.tqSwingPad);
		viz.visualizeLimitCone(cB2w, data.tqSwingZ, data.tqSwingY, 
			!coneHelper.contains(tanQSwing));
	}
	else if(swing1Limited ^ swing2Limited)
	{
		PxTransform yToX = PxTransform(PxVec3(0), PxQuat(-PxPi/2, PxVec3(0,0,1.f)));
		PxTransform zToX = PxTransform(PxVec3(0), PxQuat(PxPi/2, PxVec3(0,1.f,0)));

		if(swing1Limited)
		{
			if(data.locked & SWING2_FLAG)
				viz.visualizeAngularLimit(cB2w * yToX, -data.swingLimit.yAngle, data.swingLimit.yAngle, 
					PxAbs(Ps::tanHalf(swing.y, swing.w)) > data.tqSwingY - data.tqSwingPad);
			else
				viz.visualizeDoubleCone(cB2w * zToX, data.swingLimit.yAngle, 
					PxAbs(tanHalfFromSin(aZ.dot(bX)))> data.thSwingY - data.thSwingPad);
		}
		else 
		{
			if(data.locked & SWING1_FLAG)
				viz.visualizeAngularLimit(cB2w * zToX, -data.swingLimit.zAngle, data.swingLimit.zAngle,
					PxAbs(Ps::tanHalf(swing.z, swing.w)) > data.tqSwingZ - data.tqSwingPad);
			else
				viz.visualizeDoubleCone(cB2w * yToX, data.swingLimit.zAngle,  
					PxAbs(tanHalfFromSin(aY.dot(bX)))> data.thSwingZ - data.thSwingPad);
		}
	}
}
}

bool D6Joint::attach(PxPhysics &physics, PxRigidActor* actor0, PxRigidActor* actor1)
{
	mPxConstraint = physics.createConstraint(actor0, actor1, *this, sShaders, sizeof(D6JointData));
	return mPxConstraint!=NULL;
}

void D6Joint::exportExtraData(PxSerializationContext& stream)
{
	if(mData)
	{
		stream.alignData(PX_SERIAL_ALIGN);
		stream.writeData(mData, sizeof(D6JointData));
	}
	stream.writeName(mName);
}

void D6Joint::importExtraData(PxDeserializationContext& context)
{
	if(mData)
		mData = context.readExtraData<D6JointData, PX_SERIAL_ALIGN>();

	context.readName(mName);
}

void D6Joint::resolveReferences(PxDeserializationContext& context)
{
	setPxConstraint(resolveConstraintPtr(context, getPxConstraint(), getConnector(), sShaders));	
}

D6Joint* D6Joint::createObject(PxU8*& address, PxDeserializationContext& context)
{
	D6Joint* obj = new (address) D6Joint(PxBaseFlag::eIS_RELEASABLE);
	address += sizeof(D6Joint);	
	obj->importExtraData(context);
	obj->resolveReferences(context);
	return obj;
}

#ifdef PX_PS3
PxConstraintShaderTable Ext::D6Joint::sShaders = { Ext::D6JointSolverPrep, ExtD6JointSpu, EXTD6JOINTSPU_SIZE, D6JointProject, D6JointVisualize };
#else
PxConstraintShaderTable Ext::D6Joint::sShaders = { Ext::D6JointSolverPrep, 0, 0, D6JointProject, D6JointVisualize };
#endif

//~PX_SERIALIZATION
