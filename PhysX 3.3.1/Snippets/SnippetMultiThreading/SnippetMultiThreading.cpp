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
// This snippet shows how to coordinate threads performing asynchronous
// work during the scene simulation. After simulate() is called, user threads 
// are started that perform ray-casts against the scene. The call to 
// fetchResults() is delayed until all ray-casts have completed.
// ****************************************************************************

#include <ctype.h>
#include "PxPhysicsAPI.h"
#include "PsThread.h"
#include "PsAtomic.h"
#include "PsSync.h"

using namespace physx;
using namespace shdfnd; // internal platform abstractions for atomics, threads and synchronization

PxDefaultAllocator		 gAllocator;
PxDefaultErrorCallback	 gErrorCallback;

PxFoundation*			 gFoundation = NULL;
PxPhysics*				 gPhysics	= NULL;

PxDefaultCpuDispatcher*	 gDispatcher = NULL;
PxScene*				 gScene		= NULL;

PxMaterial*				 gMaterial	= NULL;

PxVisualDebuggerConnection*
						gConnection	= NULL;

class RaycastThread;

const PxU32				 gNumThreads = 1;
RaycastThread*			 gThreads[gNumThreads];
shdfnd::Sync*			 gWorkDoneSync;

const PxI32				 gRayCount = 1024;
volatile PxI32			 gRaysAvailable;
volatile PxI32			 gRaysCompleted;

class RaycastThread : public Thread
{
public:

	PxVec3 randVec3() const
	{
		return (PxVec3(float(rand())/RAND_MAX,
					   float(rand())/RAND_MAX, 
					   float(rand())/RAND_MAX)*2.0f - PxVec3(1.0f)).getNormalized();
	}

	virtual void execute()
	{
		// perform random ray-casts against the scene until stop
		for(;;)
		{
			mWorkReadySync.wait();
			mWorkReadySync.reset();

			if (quitIsSignalled())
				break;

			while (atomicDecrement(&gRaysAvailable) >= 0)
			{
				PxVec3 dir = randVec3();

				PxRaycastBuffer buf;
				gScene->raycast(PxVec3(0.0f), dir.getNormalized(), 1000.0f, buf, PxHitFlag::eDEFAULT);

				// if this is the last ray-cast then release the main thread
				if (atomicIncrement(&gRaysCompleted) == gRayCount)
					gWorkDoneSync->set();
			}
		}

		// shutdown thread
		quit();
	}

	Sync mWorkReadySync;

};

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

void createPhysicsAndScene()
{
	gFoundation = PxCreateFoundation(PX_PHYSICS_VERSION, gAllocator, gErrorCallback);
	PxProfileZoneManager* profileZoneManager = &PxProfileZoneManager::createProfileZoneManager(gFoundation);
	gPhysics = PxCreatePhysics(PX_PHYSICS_VERSION, *gFoundation, PxTolerancesScale(),true,profileZoneManager);

	if(gPhysics->getPvdConnectionManager())
	{
		gConnection = PxVisualDebuggerExt::createConnection(gPhysics->getPvdConnectionManager(), PVD_HOST, 5425, 10);
	}

	gMaterial = gPhysics->createMaterial(0.5f, 0.5f, 0.6f);

	PxSceneDesc sceneDesc(gPhysics->getTolerancesScale());
	sceneDesc.gravity = PxVec3(0.0f, -9.81f, 0.0f);
	
	PxU32 numWorkers = PxMax(PxI32(shdfnd::Thread::getNbPhysicalCores())-1, 0);
	gDispatcher = PxDefaultCpuDispatcherCreate(numWorkers);
	sceneDesc.cpuDispatcher	= gDispatcher;
	sceneDesc.filterShader	= PxDefaultSimulationFilterShader;
	
	gScene = gPhysics->createScene(sceneDesc);
	
	PxRigidStatic* groundPlane = PxCreatePlane(*gPhysics, PxPlane(0,1,0,0), *gMaterial);
	gScene->addActor(*groundPlane);

	for(PxU32 i=0;i<5;i++)
		createStack(PxTransform(PxVec3(0,0,i*10.0f)), 10, 2.0f);
}

void createRaycastThreads()
{
	gWorkDoneSync = (Sync*)PX_ALLOC(sizeof(Sync), "sync");
	PX_PLACEMENT_NEW(gWorkDoneSync, Sync);

	// start ray-cast threads
	for (PxU32 i=0; i < gNumThreads; ++i)
	{
		gThreads[i] = PX_NEW(RaycastThread);
#ifdef PX_WIIU
		PxU32 affinityMask = (i % 2) ? 4 : 1;
		gThreads[i]->setAffinityMask(affinityMask); 
#endif
		gThreads[i]->start();
	}
}

void initPhysics()
{
	createPhysicsAndScene();
	createRaycastThreads();
}

void stepPhysics()
{
	// start simulation
	gScene->simulate(1.0f/60.0f);

	// start ray-cast threads
	gRaysAvailable = gRayCount;
	gRaysCompleted = 0;

	for (PxU32 i=0; i < gNumThreads; ++i)
		gThreads[i]->mWorkReadySync.set();

	// wait for ray-cast threads to finish
	gWorkDoneSync->wait();
	gWorkDoneSync->reset();

	// fetch simulation results
	gScene->fetchResults(true);
}
	
void cleanupPhysics()
{
	// signal threads to quit
	for (PxU32 i=0; i < gNumThreads; ++i)
	{
		gThreads[i]->signalQuit();
		gThreads[i]->mWorkReadySync.set();
	}

	// clean up threads
	for (PxU32 i=0; i < gNumThreads; ++i)
	{
		gThreads[i]->waitForQuit();
		PX_DELETE(gThreads[i]);
	}

	gWorkDoneSync->~Sync();
	PX_FREE(gWorkDoneSync);

	gScene->release();

	gDispatcher->release();
	PxProfileZoneManager* profileZoneManager = gPhysics->getProfileZoneManager();
	if(gConnection != NULL)
		gConnection->release();
	gPhysics->release();
	profileZoneManager->release();
	gFoundation->release();
	
	printf("SnippetMultiThreading done.\n");
}

#if defined(PX_XBOXONE) || defined(PX_WINMODERN)
int main(Platform::Array<Platform::String^>^)
#else
int main(int, char**)
#endif
{
	initPhysics();

	for(PxU32 i=0; i<100; ++i)
		stepPhysics();

	cleanupPhysics();

	return 0;
}

