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

#ifndef SNIPPET_VEHICLE_PROFILER_H
#define SNIPPET_VEHICLE_PROFILER_H

#ifdef PX_PROFILE

#include "PxPhysicsAPI.h"
#include "physxprofilesdk/PxProfileZone.h"
#include "PsTime.h"

using namespace physx;

class VehicleProfiler
{
public:

	VehicleProfiler()
		: mStartTimes(NULL),
		  mAccumulatedTimes(NULL),
		  mProfileEventNames(NULL),
		  mNumProfileEventNames(0),
		  mEventNameProvider(NULL, 0),
		  mProfiler(NULL)
	{
	}

	~VehicleProfiler()
	{
	}

	enum
	{
		eMAX_NAME_LENGTH = 64
	};

	static VehicleProfiler* allocate(const char profileNames[][eMAX_NAME_LENGTH], const PxU32 numProfileZones, PxProfileZoneManager* profileZoneManager, PxAllocatorCallback& allocator)
	{
		const PxU32 byteSize = sizeof(VehicleProfiler) + sizeof(PxU64)*numProfileZones + sizeof(PxU64)*numProfileZones + sizeof(PxProfileEventName)*numProfileZones;
		PxU8* buffer = (PxU8*)allocator.allocate(byteSize, NULL, NULL, 0);

		VehicleProfiler* vehProfiler = (VehicleProfiler*)buffer;
		new(vehProfiler) VehicleProfiler();
		buffer += sizeof(VehicleProfiler);

		vehProfiler->mStartTimes = (PxU64*)buffer;
		buffer += sizeof(PxU64)*numProfileZones;
		PxMemZero(vehProfiler->mStartTimes, sizeof(PxU64)*numProfileZones);

		vehProfiler->mAccumulatedTimes = (PxU64*)buffer;
		buffer += sizeof(PxU64)*numProfileZones;
		PxMemZero(vehProfiler->mAccumulatedTimes, sizeof(PxU64)*numProfileZones);

		vehProfiler->mProfileEventNames = (PxProfileEventName*)buffer;
		buffer += sizeof(PxProfileEventName)*numProfileZones;
		for(PxU16 i = 0; i < numProfileZones; i++)
		{
			vehProfiler->mProfileEventNames[i] = PxProfileEventName(profileNames[i], PxProfileEventId(i, true));
		}
		vehProfiler->mNumProfileEventNames = numProfileZones;

		vehProfiler->mEventNameProvider = EventNameProvider(vehProfiler->mProfileEventNames, numProfileZones);

		vehProfiler->mProfiler = &PxProfileZone::createProfileZone(&allocator, "VehicleProfiler", vehProfiler->mEventNameProvider);

		profileZoneManager->addProfileZone(*vehProfiler->mProfiler);

		return vehProfiler;
	}

	void free(PxAllocatorCallback& allocator)
	{
		mProfiler->release();
		allocator.deallocate(this);
	}

	void start(const PxU16 zone)
	{
		mStartTimes[zone] = mTimer.getCurrentCounterValue();
		mProfiler->startEvent(zone, 0);
	}
	void stop(const PxU16 zone)
	{
		mAccumulatedTimes[zone] += mTimer.getCurrentCounterValue() - mStartTimes[zone];
		mProfiler->stopEvent(zone, 0);
	}

	PxF32 getTotalTimeMs(const PxU16 zone) const
	{
		return mTimer.getCounterFrequency().toTensOfNanos(mAccumulatedTimes[zone])/(100.0f * 1000.0f);
	}

private:

	PxU64* mStartTimes;
	PxU64* mAccumulatedTimes;
	shdfnd::Time mTimer;

	PxProfileEventName* mProfileEventNames;
	PxU32 mNumProfileEventNames;

	class EventNameProvider : public physx::PxProfileNameProvider
	{
	public:

		EventNameProvider(PxProfileEventName* profileEventNames, const PxU32 numProfileEventNames)
			: mProfileEventNames(profileEventNames),
			  mNumProfileEventNames(numProfileEventNames)
		{
		}

		virtual PxProfileNames getProfileNames() const {return PxProfileNames(mNumProfileEventNames, mProfileEventNames);}

	private:

		PxProfileEventName* mProfileEventNames;
		PxU32 mNumProfileEventNames;
	};
	EventNameProvider mEventNameProvider;

	PxProfileZone* mProfiler;	


};

#endif // PX_PROFILE

#endif //SNIPPET_VEHICLE_PROFILER_H
