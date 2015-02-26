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

#include <stdio.h>
#include "ExtPulleyJoint.h"
#include "ExtConstraintHelper.h"
#include "CmRenderOutput.h"
#include "CmConeLimitHelper.h"
#include "PxTolerancesScale.h"
#include "CustomPulleyJoint.h"
#include "PxRepXSerializer.h"
#include "PxSerialization.h"
#include "PxMetaDataObjects.h"
#include "PxSnippetExtensionMetaDataObjects.h"
#include "ExtPulleyJointExtension.h"
#include "PxJointRepXSerializer.h"
#include "SnRepXSerializerImpl.h"

using namespace physx;
using namespace Ext;
using namespace Sn;
using namespace Cm;

namespace physx { 
	static inline CustomPulleyJoint* createJoint(PxPhysics& physics, 
					PxRigidActor* actor0, const PxTransform& localFrame0, 
					PxRigidActor* actor1, const PxTransform& localFrame1)
	{
		return CustomPulleyJointCreate( physics, actor0, localFrame0, actor1, localFrame1 );
	}
	
	template<>
	PxRepXObject PxJointRepXSerializer<CustomPulleyJoint>::fileToObject( XmlReader& inReader, XmlMemoryAllocator& inAllocator, PxRepXInstantiationArgs& inArgs, PxCollection* inCollection )
	{
		PxRigidActor* actor0 = NULL;
		PxRigidActor* actor1 = NULL;
		PxTransform localPose0 = PxTransform(PxIdentity);
		PxTransform localPose1 = PxTransform(PxIdentity);
		bool ok = true;
		if ( inReader.gotoChild( "Actors" ) )
		{
			ok = readReference<PxRigidActor>( inReader, *inCollection, "actor0", actor0 );
			ok &= readReference<PxRigidActor>( inReader, *inCollection, "actor1", actor1 );
			inReader.leaveChild();
		}
		CustomPulleyJoint* theJoint = !ok ? NULL : createJoint( inArgs.physics, actor0, localPose0, actor1, localPose1 );
		
		if ( theJoint )
			this->fileToObjectImpl( theJoint, inReader, inAllocator, inArgs, inCollection );
		return createRepXObject( theJoint );
	}

	template<>
	void PxJointRepXSerializer<CustomPulleyJoint>::objectToFileImpl(const CustomPulleyJoint* inObj, PxCollection* collection, XmlWriter& writer , MemoryBuffer& buffer, PxRepXInstantiationArgs& /*inArgs*/)
	{
		writeAllProperties( inObj, writer, buffer, *collection );
	}
}

void registerPulleyJointRepXExtension(PxSerializationRegistry& sr)
{
	sr.registerRepXSerializer(PulleyJointConcreteType::eUSER_PULLEY_JOINT, PX_NEW_REPX_SERIALIZER(PxJointRepXSerializer<CustomPulleyJoint>));
}
	
void unregisterPulleyJointRepXExtension(physx::PxSerializationRegistry& sr)
{
	PX_DELETE_REPX_SERIALIZER(sr.unregisterRepXSerializer(PulleyJointConcreteType::eUSER_PULLEY_JOINT));
}
