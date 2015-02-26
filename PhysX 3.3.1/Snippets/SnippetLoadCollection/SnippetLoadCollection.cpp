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

// ****************************************************************************
// This snippet illustrates loading repx or binary serialized collections and instantiating the objects in a scene.
//
// It only compiles and runs on authoring platforms (windows, osx and linux). 
// The snippet supports connecting to PVD in order to display the scene.
//
// It is a simple command-line tool supporting the following options::
// SnippetLoadCollection [--pvdhost=<ip address> ] [--pvdport=<ip port> ] [--pvdtimeout=<time ms> ] [--binary|repx] [--generateExampleFile=<filename> ] <filename>...		   
//
// --pvdhost=<ip address>              Defines ip address of PVD, default is 127.0.0.1
// --pvdport=<ip port>                 Defines ip port of PVD, default is 5425
// --pvdtimeout=<time ms>              Defines time out of PVD, default is 10
// --binary|repx		               Defines serialization type for input and example generation, default is binary
// --generateExampleFile=<filename>	   Generates an example file
//   <filename>...	                   Input files
//
// Multiple collection files can be specified. The snippet is currently restricted to load a list of collections which obeys
// the following rule: The first collection needs to be complete. All following collections - if any - may only maintain
// dependencies to objects in the first collection.
//
// ****************************************************************************

#include "PxPhysicsAPI.h"
#include "PsString.h"

#define MAX_INPUT_FILES   16

using namespace physx;

PxDefaultAllocator		    gAllocator;
PxDefaultErrorCallback	    gErrorCallback;

PxFoundation*			    gFoundation = NULL;
PxPhysics*				    gPhysics	= NULL;
PxCooking*				    gCooking	= NULL;
PxSerializationRegistry*    gSerializationRegistry = NULL;

PxDefaultCpuDispatcher*	    gDispatcher = NULL;
PxScene*				    gScene		= NULL;

PxVisualDebuggerConnection*	gConnection	= NULL;

PxU8*					    gMemBlocks[MAX_INPUT_FILES];
PxU32                       gNbMemBlocks = 0;
PxCollection*               gSharedCollection = NULL;

struct CmdLineParameters
{
	const char*		pvdhost;		
	PxU32		    pvdport;			
	PxU32		    pvdtimeout;	
	const char*     inputFiles[MAX_INPUT_FILES];
	const char*     exampleFile;
	PxU32           nbFiles;
	bool            isBinary;

	CmdLineParameters()	: 
	      pvdhost("127.0.0.1")
		, pvdport(5425)
		, pvdtimeout(10)
		, exampleFile(NULL)
		, nbFiles(0)
		, isBinary(true)
	{}
} gParameters;

static bool match(const char* opt, const char* ref)
{
	return !physx::string::strnicmp(opt, ref, strlen(ref));
}

static void printHelpMsg()
{
	printf("SnippetLoadCollection usage:\n"
		"SnippetLoadCollection "
		"[--pvdhost=<ip address> ] "
		"[--pvdport=<ip port> ]"
		"[--pvdtimeout=<time ms> ] "
		"[--binary|repx] "		
		"[--generateExampleFile=<filename> ]"		
		"<filename>...\n\n"		   
		"Load binary or repx serialized collections and instatiate the objects in a PhysX scene.\n");

	printf("--pvdhost=<ip address> \n");
	printf("  Defines ip address of PVD, default is 127.0.0.1 \n");

	printf("--pvdport=<ip port> \n");
	printf("  Defines ip port of PVD, default is 5425\n");

	printf("--pvdtimeout=<time ms> \n");
	printf("  Defines timeout of PVD, default is 10\n");

	printf("--binary|repx\n");
	printf("  Defines serialization type for input and example generation, default is binary\n");
		
	printf("--generateExampleFile=<filename>\n");
	printf("  Generates an example file\n");	

	printf("<filename>...\n");
	printf("  Input files, if a collection contains shared objects, it needs to be provided with the first file. \n\n");

}

static bool parseCommandLine(CmdLineParameters& result, int argc, const char *const*argv)
{
	if( argc <= 1 )
	{
		printHelpMsg();	
		return false;
	}

	for(int i = 1; i < argc; ++i)
	{
		if(argv[i][0] != '-' || argv[i][1] != '-')
		{
			if (result.nbFiles < MAX_INPUT_FILES)
			{
				result.inputFiles[result.nbFiles++] = argv[i];
			}
			else
				printf( "[WARNING] more input files are specified than supported (maximum %d). Ignoring the file  %s\n", MAX_INPUT_FILES, argv[i] );				
		}
		else if(match(argv[i], "--binary"))
		{
			result.isBinary = true;
		}
		else if(match(argv[i], "--repx"))
		{
			result.isBinary = false;
		}
		else if(match(argv[i], "--pvdhost="))
		{
			const char* hostStr = argv[i] + strlen("--pvdhost=");
			if(hostStr)
				result.pvdhost = hostStr;
		}
		else if(match(argv[i], "--pvdport="))
		{			
			const char* portStr = argv[i] + strlen("--pvdport=");		
			if (portStr)
				result.pvdport = atoi(portStr);	
		}
		else if(match(argv[i], "--pvdtimeout="))
		{			
			const char* timeoutStr = argv[i] + strlen("--pvdtimeout=");		
			if (timeoutStr)
				result.pvdtimeout = atoi(timeoutStr);	
		}
		else if(match(argv[i], "--generateExampleFile="))
		{
			const char* exampleFile = argv[i] + strlen("--generateExampleFile=");
			if(exampleFile)
				result.exampleFile = exampleFile;
		}
		else
		{
			printf( "[ERROR] Unknown command line parameter \"%s\"\n", argv[i] );
			printHelpMsg();
			return false;
		}
	}
	
	if(result.nbFiles == 0 &&  result.exampleFile == NULL)
	{
		printf( "[ERROR] parameter missing.\n" );
		printHelpMsg();
		return false;				   
	}

	return true;
}


bool instanceCollection(PxInputData& sharedData, PxSerializationRegistry& sr, PxU32 index)
{
	PxCollection* collection;

	if(gParameters.isBinary)
	{
		PxU32 length = sharedData.getLength();
		gMemBlocks[index] = (PxU8*)malloc(length+PX_SERIAL_FILE_ALIGN-1);
		gNbMemBlocks++;
		void* alignedBlock = (void*)((size_t(gMemBlocks[index])+PX_SERIAL_FILE_ALIGN-1)&~(PX_SERIAL_FILE_ALIGN-1));
		sharedData.read(alignedBlock, length);
		collection = PxSerialization::createCollectionFromBinary(alignedBlock, sr, gSharedCollection);
	}
	else
	{
		collection = PxSerialization::createCollectionFromXml(sharedData, *gCooking, sr, gSharedCollection);		
	}

	if(!collection)
		return false;

	gScene->addCollection(*collection);	

	if(index == 0)
		gSharedCollection = collection;
	else
		collection->release();	

	return true;
}

void initPhysics()
{
	gFoundation = PxCreateFoundation(PX_PHYSICS_VERSION, gAllocator, gErrorCallback);

	gPhysics = PxCreatePhysics(PX_PHYSICS_VERSION, *gFoundation, PxTolerancesScale(), true);
	PxInitExtensions(*gPhysics);

	if(gPhysics->getPvdConnectionManager())
	{
		gPhysics->getVisualDebugger()->setVisualizeConstraints(true);
		gPhysics->getVisualDebugger()->setVisualDebuggerFlag(PxVisualDebuggerFlag::eTRANSMIT_CONTACTS, true);
		gPhysics->getVisualDebugger()->setVisualDebuggerFlag(PxVisualDebuggerFlag::eTRANSMIT_SCENEQUERIES, true);
		gConnection = PxVisualDebuggerExt::createConnection(gPhysics->getPvdConnectionManager(), gParameters.pvdhost, gParameters.pvdport, gParameters.pvdtimeout);
	}

	PxSceneDesc sceneDesc(gPhysics->getTolerancesScale());
	sceneDesc.gravity = PxVec3(0, -9.81f, 0);	
	gDispatcher = PxDefaultCpuDispatcherCreate(1);
	sceneDesc.cpuDispatcher	= gDispatcher;
	sceneDesc.filterShader	= PxDefaultSimulationFilterShader;
	gScene = gPhysics->createScene(sceneDesc);

	gCooking = PxCreateCooking(PX_PHYSICS_VERSION, *gFoundation, PxCookingParams(PxTolerancesScale()));	
	
	gSerializationRegistry = PxSerialization::createSerializationRegistry(*gPhysics);

}

void cleanupPhysics()
{	
	gSerializationRegistry->release();
	gScene->release();
	gDispatcher->release();
	PxCloseExtensions();

	if(gConnection != NULL)
		gConnection->release();

	if(gSharedCollection)
		gSharedCollection->release();

	gPhysics->release();	// releases of all objects	
	gCooking->release();

	for(PxU32 i=0; i<gNbMemBlocks; i++)
		free(gMemBlocks[i]); // now that the objects have been released, it's safe to release the space they occupy

	gFoundation->release();

	printf("SnippetCollection done.\n");
}

static void generateExampleFiles()
{
	PxCollection* collection = PxCreateCollection();
	PX_ASSERT( collection );
	
	PxMaterial *material = gPhysics->createMaterial(0.5f, 0.5f, 0.6f);
	PX_ASSERT( material );
	PxShape* shape = gPhysics->createShape(PxBoxGeometry(2.f, 2.f, 2.f), *material);
	PxRigidStatic* theStatic = PxCreateStatic(*gPhysics, PxTransform(PxIdentity), *shape);
	
	collection->add(*material);
	collection->add(*shape);
	collection->add(*theStatic);

	PxDefaultFileOutputStream exampleFile(gParameters.exampleFile);
	bool bret;
	if(gParameters.isBinary)
	   bret = PxSerialization::serializeCollectionToBinary(exampleFile, *collection, *gSerializationRegistry);
	else
	   bret = PxSerialization::serializeCollectionToXml(exampleFile, *collection, *gSerializationRegistry);	

	if(bret)
		printf( "Generated %s file!\n", gParameters.exampleFile);
	else
		printf( "[ERROR] Generate %s file failure!\n", gParameters.exampleFile);

	collection->release();
}

int main(int argc, const char *const* argv)
{
	if(!parseCommandLine(gParameters, argc, argv))
	   return 1;

	initPhysics();

	if(gParameters.exampleFile)
		generateExampleFiles();
	
	for(PxU32 i=0; i<gParameters.nbFiles; i++)
	{
		const char*	filename = gParameters.inputFiles[i];
		PxDefaultFileInputData inputStream(filename); 
		if(!instanceCollection(inputStream, *gSerializationRegistry, i))
		{
			printf( "[ERROR] deserialize file failure! filename: %s\n", filename);
			break;
		}
	}	

	gScene->simulate(1.0f/60.0f);

	gScene->fetchResults(true);

	cleanupPhysics();	

	return 0;
}

