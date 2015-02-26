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

#ifndef GU_INTERSECTION_TRIANGLE_BOX_H
#define GU_INTERSECTION_TRIANGLE_BOX_H

#include "PxPhysXCommonConfig.h"
#include "CmPhysXCommon.h"
#include "GuBox.h"

namespace physx
{
namespace Gu
{
	class Box;
	struct OBBTriangleTest
	{
		PxMat33								mRModelToBox;		//!< Rotation from model space to obb space
		PxMat33								mRBoxToModel;		//!< Rotation from obb space to model space
		PxVec3								mTModelToBox;		//!< Translation from model space to obb space
		PxVec3								mTBoxToModel;		//!< Translation from obb space to model space
		PxVec3								mBoxExtents;

		OBBTriangleTest(const Gu::Box& box);
		Ps::IntBool obbTriTest(const PxVec3& vert0, const PxVec3& vert1, const PxVec3& vert2) const;

	};
	PX_PHYSX_COMMON_API Ps::IntBool intersectTriangleBox(const PxVec3& boxCenter, const PxVec3& extents, const PxVec3& tp0, const PxVec3& tp1, const PxVec3& tp2);

} // namespace Gu
}

#endif