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

// suppress LNK4221
#include "PxPreprocessor.h"
PX_DUMMY_SYMBOL

#if PX_SUPPORT_VISUAL_DEBUGGER

#include "ExtPulleyJointPVD.h"
#include "ExtPulleyJoint.h"
#include "ExtVisualDebugger.h"

#include "PvdMetaDataDefineProperties.h"
#include "PvdMetaDataPropertyVisitor.h"
#include "PvdTypeNames.h"
#include "PvdDataStream.h"

#include "CustomPulleyJoint.h"

namespace physx
{
namespace Ext
{
	using namespace Pvd;
	template<typename TObjType, typename TOperator>
	static inline void visitUserPvdInstanceProperties( TOperator inOperator )
	{
		PxClassInfoTraits<TObjType>().Info.visitInstanceProperties( makePvdPropertyFilter( inOperator ), 0 );	
	}

	template<typename TObjType>
	static void registerUserProperties( PvdDataStream& inStream )
	{
		inStream.createClass<TObjType>();
		PvdPropertyDefinitionHelper& theHelper( inStream.getPropertyDefinitionHelper() );
		PvdClassInfoDefine theDefinitionObj( theHelper, getPvdNamespacedNameForType<TObjType>() );
		visitUserPvdInstanceProperties<TObjType>( theDefinitionObj );
	}

	template<typename TObjType, typename TOperator>
	static inline void visitUserPvdProperties( TOperator inOperator )
	{
		PvdPropertyFilter<TOperator> theFilter( makePvdPropertyFilter( inOperator ) );
		PxU32 thePropCount = PxClassInfoTraits<TObjType>().Info.visitBaseProperties( theFilter );
		PxClassInfoTraits<TObjType>().Info.visitInstanceProperties( theFilter, thePropCount );
	}


	template<typename TObjType, typename TValueStructType>
	static void registerUserPropertiesAndValueStruct( PvdDataStream& inStream )
	{
		inStream.createClass<TObjType>();
		inStream.deriveClass<PxJoint,TObjType>();
		PvdPropertyDefinitionHelper& theHelper( inStream.getPropertyDefinitionHelper() );
		{
			PvdClassInfoDefine theDefinitionObj( theHelper, getPvdNamespacedNameForType<TObjType>() );
			visitUserPvdInstanceProperties<TObjType>( theDefinitionObj );
		}
		{
			PvdClassInfoValueStructDefine theDefinitionObj( theHelper );
			visitUserPvdProperties<TObjType>( theDefinitionObj );
			theHelper.addPropertyMessage<TObjType,TValueStructType>();
		}
	}

	void sendUserClassDescriptions(physx::debugger::comm::PvdDataStream& inStream)
	{
		registerUserPropertiesAndValueStruct<CustomPulleyJoint,CustomPulleyJointGeneratedValues>( inStream);
	}

}
}

#else

#include "CmPhysXCommon.h"
#include "PxVisualDebuggerExt.h"

namespace physx { namespace debugger {
	class PvdDataStream;
}}

#endif // PX_SUPPORT_VISUAL_DEBUGGER