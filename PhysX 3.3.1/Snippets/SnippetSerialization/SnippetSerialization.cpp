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
// This snippet illustrates the use of binary serialization
//
// It creates a chain of boxes and serializes them in two collections: one of 
// shared objects, and one of actors and joints which has references into the 
// collection of shared objects
//
// Then it tears down physics, recreates it, reloads the objects, and simulates
//
// ****************************************************************************

#include "PxPhysicsAPI.h"
#include "PsThread.h"

using namespace physx;

PxDefaultAllocator		gAllocator;
PxDefaultErrorCallback	gErrorCallback;

PxFoundation*			gFoundation = NULL;
PxPhysics*				gPhysics	= NULL;

PxDefaultCpuDispatcher*	gDispatcher = NULL;
PxScene*				gScene		= NULL;

PxVisualDebuggerConnection*		
						gConnection	= NULL;

PxU8*					gSharedBlock;
PxU8*					gActorBlock;

void createCollections(PxOutputStream& sharedStream, PxOutputStream& actorStream, PxSerializationRegistry& sr)
{
	PxMaterial* material = gPhysics->createMaterial(0.5f, 0.5f, 0.6f);

	PxReal halfLength = 2.0f, height = 25.0f;
	PxVec3 offset(halfLength, 0, 0);
	PxRigidActor* prevActor = PxCreateStatic(*gPhysics, PxTransform(PxVec3(0,height,0)), PxSphereGeometry(halfLength), *material, PxTransform(offset));

	PxShape* shape = gPhysics->createShape(PxBoxGeometry(halfLength, 1.0f, 1.0f), *material);
	for(PxU32 i=1; i<8;i++)
	{
		PxTransform tm(PxVec3(PxReal(i*2)* halfLength, height, 0));	
		PxRigidDynamic* dynamic = gPhysics->createRigidDynamic(tm);
		dynamic->attachShape(*shape);
		PxRigidBodyExt::updateMassAndInertia(*dynamic, 10.0f);

		PxSphericalJointCreate(*gPhysics, prevActor, PxTransform(offset), dynamic, PxTransform(-offset));
		prevActor = dynamic;
	}
		
	PxCollection* sharedCollection = PxCreateCollection();			// collection for all the shared objects
	PxCollection* actorCollection = PxCreateCollection();			// collection for all the nonshared objects

	sharedCollection->add(*shape);
	PxSerialization::complete(*sharedCollection, sr);								       // chases the pointer from shape to material, and adds it
	PxSerialization::createSerialObjectIds(*sharedCollection, PxSerialObjectId(77));		// arbitrary choice of base for references to shared objects
	PxSerialization::serializeCollectionToBinary(sharedStream, *sharedCollection, sr);

	actorCollection->add(*prevActor);
	PxSerialization::complete(*actorCollection, sr, sharedCollection, true);		// chases all pointers and recursively adds actors and joints
	PxSerialization::serializeCollectionToBinary(actorStream, *actorCollection, sr, sharedCollection);				

	actorCollection->release();
	sharedCollection->release();	
}

void* createAlignedBlock(PxInputData& data, PxU8*& baseAddr)
{
	PxU32 length = data.getLength();
	baseAddr = (PxU8*)malloc(length+PX_SERIAL_FILE_ALIGN-1);
	void* alignedBlock = (void*)((size_t(baseAddr)+PX_SERIAL_FILE_ALIGN-1)&~(PX_SERIAL_FILE_ALIGN-1));
	data.read(alignedBlock, length);
	return alignedBlock;
}

void instanceCollection(PxInputData& sharedData, PxInputData& actorData, PxSerializationRegistry& sr)
{
	void* alignedBlock = createAlignedBlock(sharedData, gSharedBlock);
	PxCollection* sharedCollection = PxSerialization::createCollectionFromBinary(alignedBlock, sr);

	alignedBlock = createAlignedBlock(actorData, gActorBlock);
	
	PxCollection* actorCollection = PxSerialization::createCollectionFromBinary(alignedBlock, sr, sharedCollection);
	gScene->addCollection(*actorCollection);
	sharedCollection->release();
	actorCollection->release();	
}

void initPhysics(bool interactive)
{
	PX_UNUSED(interactive);

	gFoundation = PxCreateFoundation(PX_PHYSICS_VERSION, gAllocator, gErrorCallback);
	PxProfileZoneManager* profileZoneManager = &PxProfileZoneManager::createProfileZoneManager(gFoundation);	
	gPhysics = PxCreatePhysics(PX_PHYSICS_VERSION, *gFoundation, PxTolerancesScale(), true, profileZoneManager);
	PxInitExtensions(*gPhysics);
	PxSerializationRegistry* sr = PxSerialization::createSerializationRegistry(*gPhysics);

	PxDefaultMemoryOutputStream sharedStream, actorStream;
	createCollections(sharedStream, actorStream, *sr);
	sr->release();
	PxCloseExtensions();
	gPhysics->release();
	// Now we are left with just foundation and the memory streams, we set up physics again,
	// create a scene, and reload our collections	
	gPhysics = PxCreatePhysics(PX_PHYSICS_VERSION, *gFoundation, PxTolerancesScale(), true, profileZoneManager);
	PxInitExtensions(*gPhysics);
	sr = PxSerialization::createSerializationRegistry(*gPhysics);
	if(gPhysics->getPvdConnectionManager())
	{
		gPhysics->getVisualDebugger()->setVisualizeConstraints(true);
		gPhysics->getVisualDebugger()->setVisualDebuggerFlag(PxVisualDebuggerFlag::eTRANSMIT_CONTACTS, true);
		gPhysics->getVisualDebugger()->setVisualDebuggerFlag(PxVisualDebuggerFlag::eTRANSMIT_SCENEQUERIES, true);
		gConnection = PxVisualDebuggerExt::createConnection(gPhysics->getPvdConnectionManager(), PVD_HOST, 5425, 10);
	}

	PxU32 numWorkers = PxMax(PxI32(shdfnd::Thread::getNbPhysicalCores())-1, 0);
	gDispatcher = PxDefaultCpuDispatcherCreate(numWorkers);
	PxSceneDesc sceneDesc(gPhysics->getTolerancesScale());
	sceneDesc.gravity = PxVec3(0, -9.81f, 0);
	sceneDesc.cpuDispatcher = gDispatcher;
	sceneDesc.filterShader	= PxDefaultSimulationFilterShader;
	gScene = gPhysics->createScene(sceneDesc);

	PxDefaultMemoryInputData sharedData(sharedStream.getData(), sharedStream.getSize()),
		                     actorData(actorStream.getData(), actorStream.getSize());
	instanceCollection(sharedData, actorData, *sr);

	PxMaterial* material;
	gPhysics->getMaterials(&material,1);
	PxRigidStatic* groundPlane = PxCreatePlane(*gPhysics, PxPlane(0,1,0,0), *material);
	gScene->addActor(*groundPlane);
	sr->release();
}

void stepPhysics(bool interactive)
{
	PX_UNUSED(interactive);
	gScene->simulate(1.0f/60.0f);
	gScene->fetchResults(true);
}
	
void cleanupPhysics(bool interactive)
{
	PX_UNUSED(interactive);
	gScene->release();
	gDispatcher->release();
	PxCloseExtensions();
	PxProfileZoneManager* profileZoneManager = gPhysics->getProfileZoneManager();
	if(gConnection != NULL)
		gConnection->release();
	gPhysics->release();	// releases all objects	
	profileZoneManager->release();

	free(gSharedBlock);		// now that the objects have been released, it's safe to release the space they occupy
	free(gActorBlock);

	gFoundation->release();
	
	printf("SnippetSerialization done.\n");
}

#if defined(PX_XBOXONE) || defined(PX_WINMODERN)
int main(Platform::Array<Platform::String^>^)
#else
int main(int, char**)
#endif
{
#ifdef RENDER_SNIPPET
	extern void renderLoop();
	renderLoop();
#else
	static const PxU32 frameCount = 250;
	initPhysics(false);
	for(PxU32 i=0; i<frameCount; i++)
		stepPhysics(false);
	cleanupPhysics(false);
#endif
}
