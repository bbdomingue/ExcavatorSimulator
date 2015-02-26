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


#ifndef CUSTOM_PULLEYJOINTCONSTRAINT_H
#define CUSTOM_PULLEYJOINTCONSTRAINT_H

#include "ExtJoint.h"
#include "ExtPulleyJointPVD.h"
#include "CustomPulleyJoint.h"
#include "PxSnippetExtensionMetaDataObjects.h"

namespace physx
{
struct CustomPulleyJointGeneratedValues;
namespace Ext
{
	struct PulleyJointData : public JointData
	{
		PxVec3					pulley[2];
		PxReal					distance; 
		PxReal					tolerance;
		PxReal					stiffness;
		PxReal					damping;
		PxReal					ratio;
	};

	typedef Joint<CustomPulleyJoint, CustomPulleyJointGeneratedValues> PulleyJointT;
	class PulleyJoint  : public PulleyJointT
	{
	public:
//serializable

		/**
		\brief Deserialization constructor
		*/
		PulleyJoint(PxBaseFlags baseFlags) : PulleyJointT(baseFlags) {}
		
		/**
	    \brief Exports object's extra data to stream, used for serialization
	    */
		virtual		void			exportExtraData(PxSerializationContext& context);

		/**
		\brief Reads object's extra data from address, used for serialization
		*/
		virtual     void             importExtraData(PxDeserializationContext& context);

		/**
		\brief Replace object's pointer with correct address		
		*/
		virtual     void             resolveReferences(PxDeserializationContext& context);

		/**
		\brief Creates a PullyJoint instance at the address location containing the PulleyJoint data..
		
		\context Deserialization context to support importing extra data and resolving references.
		\param address Address of the PullyJoint data in memory. Needs to be incremented by sizeof(PulleyJoint).
		\return Instance of the PullyJoint.
		*/
		static		PulleyJoint*	createObject(PxU8*& address, PxDeserializationContext& context);

		/**
		\brief Put class meta data in stream, used for serialization
		*/
		static		void			getBinaryMetaData(PxOutputStream& stream);
//~serializable

		PulleyJoint(PxPhysics& physics, 
			PxRigidActor* actor0, const PxTransform& localFrame0,
			PxRigidActor* actor1, const PxTransform& localFrame1);

		~PulleyJoint()
		{
			if(getBaseFlags()&PxBaseFlag::eOWNS_MEMORY)
				PX_FREE(mData);
		}

	bool	attach(PxPhysics &physics, PxRigidActor* actor0, PxRigidActor* actor1);

	void	setDistance(PxReal totalDistance);
	PxReal	getDistance() const;
	
	void	setRatio(PxReal ratio);
	PxReal	getRatio() const;

	void	setStiffness(PxReal stiffness);
	PxReal	getStiffness() const;

	void	setDamping(PxReal damping);
	PxReal	getDamping() const;

	void	setTolerance(PxReal tolerance);
	PxReal	getTolerance() const;

	private:
		PxConstraint* getConstraint() const { return mPxConstraint; }
		static PxConstraintShaderTable sShaders;

		PX_FORCE_INLINE PulleyJointData& data() const				
		{	
			return *static_cast<PulleyJointData*>(mData);
		}

	protected:
		PxTransform getCom(PxU32 index) const
		{
			PxRigidActor* a[2];
			mPxConstraint->getActors(a[0],a[1]);
			return getCom(a[index]);
		}

		PxTransform getCom(PxRigidActor* actor) const
		{
			if (!actor)
				return PxTransform(PxIdentity);
			else if (actor->getType() == PxActorType::eRIGID_DYNAMIC || actor->getType() == PxActorType::eARTICULATION_LINK)
				return static_cast<PxRigidBody*>(actor)->getCMassLocalPose();
			else
			{
				PX_ASSERT(actor->getType() == PxActorType::eRIGID_STATIC);
				return static_cast<PxRigidStatic*>(actor)->getGlobalPose().getInverse();
			}
		}
	};
}

namespace Ext
{
	extern "C"  PxU32 PulleyJointSolverPrep(Px1DConstraint* constraints,
		PxVec3& body0WorldOffset,
		PxU32 maxConstraints,
		PxConstraintInvMassScale&,
		const void* constantBlock,							  
		const PxTransform& bA2w,
		const PxTransform& bB2w);
}
}

#endif