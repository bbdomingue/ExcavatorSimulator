// This is the main DLL file.

#include "HeadTrackerWrapper.h"

#include "cameralibrary.h"
#include "modulevector.h"
#include "modulevectorprocessing.h"
#include "coremath.h"

#include <iostream>

using namespace CameraLibrary; 


Camera * camera;
Core::DistortionModel lensDistortion;
cModuleVector *vec;
cModuleVectorProcessing *vecprocessor;

int HeadTrackerWrapper::HeadTrackerClass::start(int * camw, int * camh)
{
	//== For OptiTrack Ethernet cameras, it's important to enable development mode if you
	//== want to stop execution for an extended time while debugging without disconnecting
	//== the Ethernet devices.  Lets do that now:

    CameraLibrary_EnableDevelopment();

	//== Initialize connected cameras ==========----
	
    CameraManager::X().WaitForInitialization();

    //== Get a connected camera ================----

    camera = CameraManager::X().GetCamera();

    //== If no device connected, pop a message box and exit ==--

    if (camera==0)
	{
		end();
		return 0;
	}

    int cameraWidth  = camera->Width();
    int cameraHeight = camera->Height();


	* camw = cameraWidth;
	* camh = cameraHeight;

    //== Create a texture to push the rasterized camera image ====----

    //== We're using textures because it's an easy & cpu light 
    //== way to utilize the 3D hardware to display camera
    //== imagery at high frame rates

    //== Set Video Mode ==--

    //== We set the camera to Segment Mode here.  This mode is support by all of our products.
    //== Depending on what device you have connected you might want to consider a different
    //== video mode to achieve the best possible tracking quality.  All devices that support a
    //== mode that will achieve a better quality output with a mode other than Segment Mode are
    //== listed here along with what mode you should use if you're looking for the best head
    //== tracking:
    //==
    //==     V100:R1/R2    Precision Mode
    //==     TrackIR 5     Bit-Packed Precision Mode
    //==     V120          Precision Mode
    //==     TBar          Precision Mode
    //==     S250e         Precision Mode
    //==
    //== If you have questions about a new device that might be conspicuously missing here or
    //== have any questions about head tracking, email support or participate in our forums.

//	camera->SetVideoType(SegmentMode);
	camera->SetVideoType(BitPackedPrecisionMode);
    
    //== Start camera output ==--

    camera->Start();

    //== Turn on some overlay text so it's clear things are     ===---
    //== working even if there is nothing in the camera's view. ===---

    camera->SetTextOverlay(false);

    vec = cModuleVector::Create(); //new cModuleVector();
    vecprocessor = new cModuleVectorProcessing();


    camera->GetDistortionModel(lensDistortion);

    //== Plug distortion into vector module ==--

    cVectorSettings vectorSettings;
    vectorSettings = *vec->Settings();

    vectorSettings.Arrangement = cVectorSettings::VectorClip;
    vectorSettings.Enabled     = true;
    
    cVectorProcessingSettings vectorProcessorSettings;

    vectorProcessorSettings = *vecprocessor->Settings();

    vectorProcessorSettings.Arrangement = cVectorSettings::VectorClip;
    vectorProcessorSettings.ShowPivotPoint = false;
    vectorProcessorSettings.ShowProcessed  = false;

    vecprocessor->SetSettings(vectorProcessorSettings);

    //== Plug in focal length in (mm) by converting it from pixels -> mm

    vectorSettings.ImagerFocalLength =  (lensDistortion.HorizontalFocalLength/((float) camera->PhysicalPixelWidth()))*camera->ImagerWidth();

    vectorSettings.ImagerHeight = camera->ImagerHeight();
    vectorSettings.ImagerWidth  = camera->ImagerWidth();

    vectorSettings.PrincipalX   = camera->PhysicalPixelWidth()/2;
    vectorSettings.PrincipalY   = camera->PhysicalPixelHeight()/2;

    vectorSettings.PixelWidth   = camera->PhysicalPixelWidth();
    vectorSettings.PixelHeight  = camera->PhysicalPixelHeight();

    vec->SetSettings(vectorSettings);

	return 1;
}














//0 Break
//1 Good
//2 No Frame Yet

int spec_update(float * ret, int * count, // Data Points
	unsigned int * ImParams, void * Buffer, bool rasterize) // The Image
{

	Frame *frame = camera->GetFrame();

	if(frame)
	{
		if (rasterize) frame->Rasterize(ImParams[0], ImParams[1], ImParams[2], ImParams[3], Buffer);

		vec->BeginFrame();

		float firstarray[] = {5, 10, 15};

		float * const arng = new float[frame->ObjectCount()];
		delete [] arng;

		for(int i=0; i < frame->ObjectCount(); i++)
		{
			cObject *obj = frame->Object(i);

			float x = obj->X();
			float y = obj->Y();

			Core::Undistort2DPoint(lensDistortion,x,y);

			vec->PushMarkerData(x, y, obj->Area(), obj->Width(), obj->Height());
		}
		vec->Calculate();
		vecprocessor->PushData(vec);
            


		for(int j = 0; (j < vecprocessor->MarkerCount()) && (j < (*count)); j++)
		{
			float x,y,z;
			vecprocessor->GetResult(j,x,y,z);
			ret[j * 3 + 0] = x;
			ret[j * 3 + 1] = y;
			ret[j * 3 + 2] = z;
		}

		*count =  vecprocessor->MarkerCount();

		frame->Release();
		return 1;
	}
	else return 2;
}

int HeadTrackerWrapper::HeadTrackerClass::update(float * ret, int * count)
{
	return spec_update(ret, count, 0, 0, false);
}

int HeadTrackerWrapper::HeadTrackerClass::update(
	float * ret, int * count, // Data Points
	unsigned int * ImParams, void * Buffer) // The Image
{
	return spec_update(ret, count, ImParams, Buffer, true);
}











void HeadTrackerWrapper::HeadTrackerClass::end()
{
	if (camera != NULL) camera->Release();

    //== Shutdown Camera Library ==--

    CameraManager::X().Shutdown();
}
