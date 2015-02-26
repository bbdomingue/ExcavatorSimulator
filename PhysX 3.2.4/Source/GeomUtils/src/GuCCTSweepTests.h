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


#ifndef PX_PHYSICS_GEOMUTILS_PX_CCTSWEEPTESTS
#define PX_PHYSICS_GEOMUTILS_PX_CCTSWEEPTESTS

#include "GuSweepTests.h"

namespace physx
{

// PT: specialized version where oneOverDir is available
// PT: why did we change the initial epsilon value?
#define LOCAL_EPSILON_RAY_BOX PX_EPS_F32
//#define LOCAL_EPSILON_RAY_BOX 0.0001f

#define USE_SAT_VERSION_CCT			// PT: the SAT-based sweep for this function, almost exclusively used by the CCT
#define USE_SAT_VERSION				// PT: uses SAT-based sweep test instead of feature-based.

#define PRECOMPUTE_FAT_BOX			// PT: TODO: clean this up
#define PRECOMPUTE_FAT_BOX_MORE


namespace Gu
{

	PX_PHYSX_COMMON_API bool sweepCCTCapsule(const PxBoxGeometry& boxGeom,					const PxTransform& pose, const Gu::Capsule& lss, const PxVec3& unitDir, const PxReal distance, PxSweepHit& sweepHit, PxSceneQueryFlags hintFlags, const PxReal inflation);
	
	PX_PHYSX_COMMON_API bool sweepCCTBox(const PxCapsuleGeometry& capsuleGeom,			const PxTransform& pose, const Gu::Box& box, const PxVec3& unitDir, const PxReal distance, PxSweepHit& sweepHit, PxSceneQueryFlags hintFlags, const PxReal inflation);
	PX_PHYSX_COMMON_API bool sweepCCTBox(const PxBoxGeometry& boxGeom,					const PxTransform& pose, const Gu::Box& box, const PxVec3& unitDir, const PxReal distance, PxSweepHit& sweepHit, PxSceneQueryFlags hintFlags, const PxReal inflation);
	PX_PHYSX_COMMON_API bool sweepCCTBox(const PxTriangleMeshGeometry& triMeshGeom,	const PxTransform& pose, const Gu::Box& box, const PxVec3& unitDir, const PxReal distance, PxSweepHit& sweepHit, PxSceneQueryFlags hintFlags, const PxReal inflation);
	PX_PHYSX_COMMON_API bool sweepCCTBox(const PxHeightFieldGeometry& heightFieldGeom,	const PxTransform& pose, const Gu::Box& box, const PxVec3& unitDir, const PxReal distance, PxSweepHit& sweepHit, PxSceneQueryFlags hintFlags, const PxReal inflation);

	PX_PHYSX_COMMON_API const SweepCapsuleFunc* GetSweepCCTCapsuleMap();
	extern const SweepCapsuleFunc gSweepCCTCapsuleMap[7];

	PX_PHYSX_COMMON_API const SweepBoxFunc* GetSweepCCTBoxMap();
	extern const SweepBoxFunc gSweepCCTBoxMap[7];


	// For sweep vs. triangle list: PxGeometryQuery::sweep()
	PX_PHYSX_COMMON_API bool SweepCCTBoxTriangles(PxU32 nb_tris, const PxTriangle* triangles,
							const PxBoxGeometry& boxGeom, const PxTransform& boxPose, const PxVec3& unitDir, const PxReal distance, 
							PxVec3& _hit, PxVec3& _normal, float& _d, PxU32& _index, const PxU32* cachedIndex, const PxReal inflation);

}  // namespace Gu

}
#endif
