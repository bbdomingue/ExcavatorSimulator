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


#define PVD_HOST "127.0.0.1"

// ****************************************************************************
// This snippet demonstrates how to get PxProfileZone event data from the
// internal profiler and use them.  
//
// The profiler caters for multiple profile zones (e.g. PhysX and APEX) and multiple clients
// (e.g. the application and PVD.)
//
// There is a single app-wide ProfileZoneManager, which is created before any profile zones.
// Clients register zone handlers with the manager, which receive events when zones are
// added and removed. This is the opportunity for a client to register interest in 
// a particular zone (in this case PhysX.) 
//
// Every so often the profile buffer for a zone gets flushed (either when it is full, or
// manually) and clients receive the buffer for parsing.
//
// PhysX events are collected and printed on screen each simulation step.
// ****************************************************************************


#include "PxPhysicsAPI.h"
#include "PsTime.h"
#include <ctype.h>

using namespace physx;

PxDefaultAllocator		gAllocator;
PxDefaultErrorCallback	gErrorCallback;

PxFoundation*			gFoundation = NULL;
PxPhysics*				gPhysics	= NULL;

PxDefaultCpuDispatcher*	gDispatcher = NULL;
PxScene*				gScene		= NULL;

PxMaterial*				gMaterial	= NULL;

PxVisualDebuggerConnection*
						gConnection	= NULL;

PxProfileZoneManager*   gProfileZoneManager = NULL;
PxProfileZone*			gPhysXProfileZone = NULL;


// This is the main class that handles stop and start events and collects them for later printing
class SnippetProfileEventHandler: public PxProfileEventHandler
{
public:

	static const PxU32		COLLECTION_SIZE = 1024;
	static const PxU32		NUM_COLLECTIONS = 16;

	// help structure to hold the event data. Times are in profiler ticks, we convert when we print
	struct ProfileEvent
	{
		PxU16 id;
		PxU64 startTime;	
		PxU64 stopTime;

		static const PxU64 INVALID_TIME = PxU64(-1);
	};

	struct EventCollection
	{
		PxU32				threadId;
		ProfileEvent		events[COLLECTION_SIZE];
		PxU32				numEvents;
	};

public:
	SnippetProfileEventHandler()
	{ 
		clear(); 
	}

	virtual void onStartEvent( const PxProfileEventId& inId, PxU32 threadId, PxU64 contextId, PxU8 cpuId, PxU8 threadPriority, PxU64 timestamp )
	{
		PX_UNUSED(contextId);
		PX_UNUSED(cpuId);
		PX_UNUSED(threadPriority);

		EventCollection* threadCollection = findCollection(threadId);

		// add a new collection for this thread Id if it's not already there
		if(!threadCollection)
		{
			threadCollection = &mThreadCollections[mNumThreads++];
			threadCollection->threadId = threadId;
			PX_ASSERT(mNumThreads < NUM_COLLECTIONS);
		}

		// add the event record
		ProfileEvent ev = { inId, timestamp, ProfileEvent::INVALID_TIME };
		threadCollection->events[threadCollection->numEvents++] = ev;

		PX_ASSERT(threadCollection->numEvents < COLLECTION_SIZE);
	}

	virtual void onStopEvent( const PxProfileEventId& inId, PxU32 threadId, PxU64 contextId, PxU8 cpuId, PxU8 threadPriority, PxU64 timestamp )
	{
		PX_UNUSED(contextId);
		PX_UNUSED(cpuId);
		PX_UNUSED(threadPriority);

		EventCollection* threadCollection = findCollection(threadId);
		PX_ASSERT(threadCollection != NULL);

		// an event (e.g. narrow phase batch) can occur several times per thread per frame, so
		// we take the earliest event with a matching id that does not yet have a stop time
		for (PxU32 i = 0; i < threadCollection->numEvents; i++)
		{			
			ProfileEvent& ev = threadCollection->events[i];
			if(ev.id == inId.mEventId && ev.stopTime == ProfileEvent::INVALID_TIME)
			{
				PX_ASSERT(timestamp > ev.startTime);
				ev.stopTime = timestamp;
				break;
			}
		}
	}
	
	virtual void onEventValue( const PxProfileEventId& , PxU32 , PxU64 , PxI64  ) {}
	virtual void onCUDAProfileBuffer( PxU64 , PxF32 , const PxU8* , PxU32 , PxU32  ) {}

	void clear()
	{
		mNumThreads = 0;
		mCrossThreadCollection.numEvents = 0;
		for (PxU32 i = 0; i < NUM_COLLECTIONS; i++)
			mThreadCollections[i].numEvents = 0;
	}

	// lets print the stored events on screen
	void printEvents()
	{
		printf("------------------------------ frame start ------------------------------------- \n");

		printf("---------------------- Cross-thread events ---------------------------- \n");
		printCollection(mCrossThreadCollection);

		for (PxU32 j = 0; j < mNumThreads; j++)
		{
			printf("\n---------------------- Thread id %d events ---------------------------- \n", mThreadCollections[j].threadId);
			printCollection(mThreadCollections[j]);
		}

		printf("------------------------------ frame end ----------------------------------- \n");
		clear();
	}
	
protected:
	EventCollection* findCollection(PxU32 threadId)
	{
		if(threadId == PxProfileEventSender::CrossThreadId)
			return &mCrossThreadCollection;

		for (PxU32 i = 0; i < mNumThreads; i++)
		{
			if(mThreadCollections[i].threadId == threadId)
				return &mThreadCollections[i];
		}
		return NULL;
	}

	void printCollection(const EventCollection &collection)
	{
		for (PxU32 i = 0; i < collection.numEvents; i++)
		{
			const ProfileEvent& ev = collection.events[i];
			const char* name = findEventName(ev.id);
			PxU64 duration = shdfnd::Time::getBootCounterFrequency().toTensOfNanos( ev.stopTime - ev.startTime );
			float timeInMs = duration/100000.0f;
			printf("%-40s: %5.5f ms\n", name, timeInMs);
		}
	}


	const char* findEventName(PxU16 eventId)
	{
		const PxProfileNames& names = gPhysXProfileZone->getProfileNames();
		for (PxU32 i = 0; i < names.mEventCount; i++)
		{
			if(names.mEvents[i].mEventId.mEventId == eventId)
				return names.mEvents[i].mName;
		}
		return "<unknown event>";
	}

	EventCollection		mCrossThreadCollection;
	EventCollection		mThreadCollections[NUM_COLLECTIONS];
	PxU32				mNumThreads;

} snippetProfileEventHandler;

// our profile zone client, which just forwards events to our handler object
class SnippetProfileZoneClient : public PxProfileZoneClient
{
public:
	virtual void handleEventAdded( const PxProfileEventName& ) {}
	virtual void handleClientRemoved() {}

	virtual void handleBufferFlush( const PxU8* inData, PxU32 inLength )
	{
		PxProfileEventHandler::parseEventBuffer(inData, inLength, snippetProfileEventHandler, false);
	}

} snippetProfileZoneClient;

// our zone handler where we watch for the zone we're interested in, and hook our client on to it
class SnippetProfileZoneHandler: public PxProfileZoneHandler
{
public:

	// check for the PhysXSDK profile zone
	virtual void onZoneAdded( PxProfileZone& inSDK )
	{
		if(strstr(inSDK.getName(),"PhysXSDK") != 0)
		{
			gPhysXProfileZone = &inSDK;
			inSDK.addClient(snippetProfileZoneClient);
		}
	}

	virtual void onZoneRemoved( PxProfileZone&  )	{}

} snippetProfileZoneHandler;


PxReal stackZ = 10.0f;

PxRigidDynamic* createDynamic(const PxTransform& t, const PxGeometry& geometry, const PxVec3& velocity=PxVec3(0))
{
	PxRigidDynamic* dynamic = PxCreateDynamic(*gPhysics, t, geometry, *gMaterial, 10.0f);
	dynamic->setAngularDamping(0.5f);
	dynamic->setLinearVelocity(velocity);
	gScene->addActor(*dynamic);
	return dynamic;
}

void createStack(const PxTransform& t, PxU32 size, PxReal halfExtent)
{
	PxShape* shape = gPhysics->createShape(PxBoxGeometry(halfExtent, halfExtent, halfExtent), *gMaterial);
	for(PxU32 i=0; i<size;i++)
	{
		for(PxU32 j=0;j<size-i;j++)
		{
			PxTransform localTm(PxVec3(PxReal(j*2) - PxReal(size-i), PxReal(i*2+1), 0) * halfExtent);
			PxRigidDynamic* body = gPhysics->createRigidDynamic(t.transform(localTm));
			body->attachShape(*shape);
			PxRigidBodyExt::updateMassAndInertia(*body, 10.0f);
			gScene->addActor(*body);
		}
	}
	shape->release();
}

void initPhysics(bool interactive)
{
	PX_UNUSED(interactive);
	gFoundation = PxCreateFoundation(PX_PHYSICS_VERSION, gAllocator, gErrorCallback);

	gProfileZoneManager = &PxProfileZoneManager::createProfileZoneManager(gFoundation);

	// when PhysX starts up, it will add a create profile zone. So we hook the
	// handler in before creating PxPhysics, so that it receives the zone creation event.
	// When we receive the event, we attach our client to the zone.

	gProfileZoneManager->addProfileZoneHandler(snippetProfileZoneHandler);
	gPhysics = PxCreatePhysics(PX_PHYSICS_VERSION, *gFoundation, PxTolerancesScale(),true,gProfileZoneManager);

	if(gPhysics->getPvdConnectionManager())
	{
		gPhysics->getVisualDebugger()->setVisualizeConstraints(true);
		gPhysics->getVisualDebugger()->setVisualDebuggerFlag(PxVisualDebuggerFlag::eTRANSMIT_CONTACTS, true);
		gPhysics->getVisualDebugger()->setVisualDebuggerFlag(PxVisualDebuggerFlag::eTRANSMIT_SCENEQUERIES, true);
		gConnection = PxVisualDebuggerExt::createConnection(gPhysics->getPvdConnectionManager(), PVD_HOST, 5425, 10);
	}

	PxSceneDesc sceneDesc(gPhysics->getTolerancesScale());
	sceneDesc.gravity = PxVec3(0.0f, -9.81f, 0.0f);
	gDispatcher = PxDefaultCpuDispatcherCreate(2);
	sceneDesc.cpuDispatcher	= gDispatcher;
	sceneDesc.filterShader	= PxDefaultSimulationFilterShader;
	gScene = gPhysics->createScene(sceneDesc);

	gMaterial = gPhysics->createMaterial(0.5f, 0.5f, 0.6f);

	PxRigidStatic* groundPlane = PxCreatePlane(*gPhysics, PxPlane(0,1,0,0), *gMaterial);
	gScene->addActor(*groundPlane);

	for(PxU32 i=0;i<5;i++)
		createStack(PxTransform(PxVec3(0,0,stackZ-=10.0f)), 10, 2.0f);

	if(!interactive)
		createDynamic(PxTransform(PxVec3(0,40,100)), PxSphereGeometry(10), PxVec3(0,-50,-100));
}

void stepPhysics(bool interactive)
{
	PX_UNUSED(interactive);

	gScene->simulate(1.0f/60.0f);

	// Custom events recorded before fetchResults will appear in the same frame buffer as physx events.
	// Events recorded after fetchResults will appear in the next frame buffer.
	gScene->fetchResults(true);

	// flush the profile zone events so we can parse them and get the recorded data
	if(gPhysXProfileZone)
	{
		gPhysXProfileZone->flushProfileEvents();
		// print the flushed events on screen
		snippetProfileEventHandler.printEvents();
	}
}

void cleanupPhysics(bool interactive)
{
	PX_UNUSED(interactive);
	gScene->release();
	gDispatcher->release();
	if(gConnection != NULL)
		gConnection->release();
	gPhysics->release();
	gProfileZoneManager->release();
	gFoundation->release();

	printf("SnippetProfileZone done.\n");
}

#if defined(PX_XBOXONE) || defined(PX_WINMODERN)
int main(Platform::Array<Platform::String^>^)
#else
int main(int, char**)
#endif
{
	static const PxU32 frameCount = 10;
	initPhysics(false);
	for(PxU32 i=0; i<frameCount; i++)
		stepPhysics(false);
	cleanupPhysics(false);
}
