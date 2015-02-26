
//======================================================================================================-----
//== NaturalPoint 2008
//======================================================================================================-----

#ifndef __CAMERALIBRARY__MODULESYNCHRONIZER_H__
#define __CAMERALIBRARY__MODULESYNCHRONIZER_H__


//== INCLUDES ===========================================================================================----

#include <stdio.h>
#include <vector>
#include "cameralibraryglobals.h"
#include "cameramodulebase.h"
#include "frame.h"
#include "lock.h"
#include "synchronizer.h"
#include "framegroup.h"
#include "healthmonitor.h"
#include "helpers.h"

//== GLOBAL DEFINITIONS AND SETTINGS ====================================================================----

namespace CameraLibrary
{
    class Camera;
    class cModuleSyncListener;
    class cModuleSync;


    class CLAPI cModuleSync : public cCameraModule, public cFrameGroupListener, public HealthMonitor
    {
    public:
        cModuleSync();
        ~cModuleSync();

        enum RunningModes
        {
            Hardware=0,
            Software,
            RunningModeCount
        };

        enum eTimeStampCalculation
        {
            SystemClock=0,  //== Default
            FrameIDBased
        };

        enum eOptimization
        {
            ForceTimelyDelivery,
            FavorTimelyDevliery,
            ForceCompleteDelivery,
            eOptimizationCount
        };

        struct sSyncDebug
        {
            int FrameID;
            double TimeStamp;
            double SortValue;
            int CameraID;
        };

        FrameGroup * GetFrameGroup();

        float        FrameDeliveryRate();

        virtual void AttachListener(cModuleSyncListener *Listener);
        virtual void RemoveListener(cModuleSyncListener *Listener);

        virtual int      CameraCount();
        virtual Camera * GetCamera(int Index);

        void          SetOptimization(eOptimization OptimizationMode);
        eOptimization Optimization();

        void         SetAllowIncompleteGroups(bool Value);
        bool         AllowIncompleteGroups();

        //==========================================================================---

        virtual void FrameRasterize(Camera *Camera, Frame *Frame, Bitmap *FrameBuffer);
        virtual bool PostFrame     (Camera *Camera, Frame *Frame);
        virtual void FrameQueueOverflow(Camera *Camera);

        virtual void OrderCameras(CameraLibrary::Camera **Order, int Count);
        virtual void AddCamera(Camera *camera, int UserData=0);
        virtual void RemoveCamera(Camera *camera);
        virtual void RemoveAllCameras();
        virtual void UpdateCameraUserData(Camera *camera, int UserData);


        FrameGroup::Modes     LastFrameGroupMode();
        double                LastFrameGroupSpread();

        void                  SetTimeStampMode(eTimeStampCalculation Mode);
        eTimeStampCalculation TimeStampMode();
        void                  SetFrameRate(float FrameRate); //== Needed when using FrameIDBased ==

        void                  ResetTimeStamp();
        void                  FrameGroupReleased(FrameGroup *Group);

#if !defined(WIN64)
        unsigned char mImplementationSpace[51832];
#else
        unsigned char mImplementationSpace[67352];
#endif
    };

    class CLAPI cModuleSyncListener
    {
    public:
        cModuleSyncListener()  {};
        ~cModuleSyncListener() {};

        virtual void FrameGroupAvailable() {};
    };
}

#endif
