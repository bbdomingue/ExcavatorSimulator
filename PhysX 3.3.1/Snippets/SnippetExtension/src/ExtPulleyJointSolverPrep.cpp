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

#include "ExtPulleyJoint.h"
#include "ExtConstraintHelper.h"
#include "PxTolerancesScale.h"

namespace physx
{
namespace Ext
{
	PxU32 PulleyJointSolverPrep(Px1DConstraint* constraints,
		PxVec3& body0WorldOffset,
		PxU32 maxConstraints,
		PxConstraintInvMassScale&,
		const void* constantBlock,
		const PxTransform& bA2w,
		const PxTransform& bB2w)
	{		
		PX_UNUSED(maxConstraints);

		const PulleyJointData& data = *reinterpret_cast<const PulleyJointData*>(constantBlock);

		PxTransform cA2w = bA2w.transform(data.c2b[0]);
		PxTransform cB2w = bB2w.transform(data.c2b[1]);

		body0WorldOffset = cB2w.p - bA2w.p;

		PxVec3 directionA = data.pulley[0] - cA2w.p;
		PxReal distanceA = directionA.normalize();

		PxVec3 directionB = data.pulley[1] - cB2w.p;
		PxReal distanceB = directionB.normalize();
		directionB *= data.ratio;

		PxReal totalDistance = distanceA + distanceB;

		// compute bias:
		PxReal bias = (data.distance - totalDistance); 

		Px1DConstraint *c = constraints;
		// constraint is breakable, so we need to output forces
		c->flags = Px1DConstraintFlag::eOUTPUT_FORCE;

		if (bias < 0.0f) 
		{
			c->maxImpulse = PX_MAX_F32;
			c->minImpulse = 0;
			c->geometricError = bias + data.tolerance; 
		}
		else if(bias > 0.0f)
		{
			c->maxImpulse = 0;
			c->minImpulse = -PX_MAX_F32;
			c->geometricError = bias - data.tolerance; 
		}

		c->linear0 = directionA;		c->angular0 = (cA2w.p - bA2w.p).cross(c->linear0);		
		c->linear1 = -directionB;		c->angular1 = (cB2w.p - bB2w.p).cross(c->linear1);		

		if (data.stiffness)
		{
			c->flags |= Px1DConstraintFlag::eSPRING;
			c->mods.spring.stiffness = data.stiffness;
			c->mods.spring.damping  = data.damping;
		}

		return 1;
	}


}//namespace

}

