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
#include "CmRenderOutput.h"
#include "CmConeLimitHelper.h"
#include "PxTolerancesScale.h"
#include "CustomPulleyJoint.h"
#include "PxMetaDataObjects.h"
#include "PxSnippetExtensionMetaDataObjects.h"
#include "PxMetaData.h"
#include "ExtPulleyJointExtension.h"

using namespace physx;
using namespace Ext;
using namespace Cm;

static void getMetaData_PulleyJoint(PxOutputStream& stream)
{
	PX_DEF_BIN_METADATA_CLASS(stream, PulleyJointData)
	PX_DEF_BIN_METADATA_BASE_CLASS(stream, PulleyJointData, JointData)

	PX_DEF_BIN_METADATA_ITEMS(stream, PulleyJointData,  PxVec3, pulley, 0, 2)
	PX_DEF_BIN_METADATA_ITEM(stream, PulleyJointData,	PxReal,	distance,	0)
	PX_DEF_BIN_METADATA_ITEM(stream, PulleyJointData,	PxReal,	tolerance,	0)
	PX_DEF_BIN_METADATA_ITEM(stream, PulleyJointData,	PxReal,	stiffness,	0)
	PX_DEF_BIN_METADATA_ITEM(stream, PulleyJointData,	PxReal,	damping,	0)
	PX_DEF_BIN_METADATA_ITEM(stream, PulleyJointData,	PxReal,	ratio,	0)
}

void PulleyJoint::getBinaryMetaData(PxOutputStream& stream)
{
	getMetaData_PulleyJoint(stream);

	PX_DEF_BIN_METADATA_VCLASS(stream, PulleyJoint)
	PX_DEF_BIN_METADATA_BASE_CLASS(stream, PulleyJoint, PxJoint)
	PX_DEF_BIN_METADATA_BASE_CLASS(stream, PulleyJoint, PxConstraintConnector)

	PX_DEF_BIN_METADATA_ITEM(stream, PulleyJoint,	char,			mName,			PxMetaDataFlag::ePTR)
#ifdef PX_DEF_BIN_METADATA_ITEMS_FIXED_SIZE
	PX_DEF_BIN_METADATA_ITEMS_FIXED_SIZE(stream, PulleyJoint,	    PxTransform,	mLocalPose,		0)
#else
	PX_DEF_BIN_METADATA_ITEMS(stream, PulleyJoint,	PxTransform,	mLocalPose,		0, 2)
#endif
	PX_DEF_BIN_METADATA_ITEM(stream, PulleyJoint,	PxConstraint,	mPxConstraint,	PxMetaDataFlag::ePTR)
	PX_DEF_BIN_METADATA_ITEM(stream, PulleyJoint,	JointData,		mData,			PxMetaDataFlag::ePTR)

	//------ Extra-data ------

	PX_DEF_BIN_METADATA_EXTRA_ITEM(stream, PulleyJoint, PulleyJointData, mData, PX_SERIAL_ALIGN)
	PX_DEF_BIN_METADATA_EXTRA_NAME(stream,	PulleyJoint, mName, 0)
}

void registerPulleyJointBinaryExtension(PxSerializationRegistry& sr)
{
	sr.registerSerializer(PulleyJointConcreteType::eUSER_PULLEY_JOINT,  PX_NEW_SERIALIZER_ADAPTER(PulleyJoint));	
	sr.registerBinaryMetaDataCallback(PulleyJoint::getBinaryMetaData);
}

void unregisterPulleyJointBinaryExtension(PxSerializationRegistry& sr)
{
	PX_DELETE_SERIALIZER_ADAPTER(sr.unregisterSerializer(PulleyJointConcreteType::eUSER_PULLEY_JOINT));
}
