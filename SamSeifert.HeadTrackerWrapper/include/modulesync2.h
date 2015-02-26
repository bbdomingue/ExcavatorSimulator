
//======================================================================================================-----
//== Copyright NaturalPoint
//======================================================================================================-----

#ifndef __CAMERALIBRARY__MODULESYNCHRONIZER2_H__
#define __CAMERALIBRARY__MODULESYNCHRONIZER2_H__

//== INCLUDES ===========================================================================================----

#include "modulesync.h"
#include "helpers.h"

//== GLOBAL DEFINITIONS AND SETTINGS ====================================================================----

namespace CameraLibrary
{
    struct sCameraHistory
    {
        int Serial;
        CameraLibrary::eVideoMode Mode;
        int FrameID;
        double TimeStamp;
        int CameraID;
    };

    enum eQueInfoFlags
    {
        FlagEmpty = 0,
        FlagSyncTelemetry = 1,
        FlagCameraFrames  = 2
    };
    struct sSyncQueueInfo
    {
        bool Valid;
        int  FrameID;
        int  ExpectedFrames;
        int  ReceivedExpectedFrames;
        int  QueueEntry;
        int  Flags;
    };

    const int kSyncCameraHistory = 200;

    class CLAPI cModuleSync2 : public cModuleSync
    {
    public:
        cModuleSync2();
        ~cModuleSync2();

        bool         PostFrame(Camera *Camera, Frame *Frame);

        void         FlushFrames();

#if !defined(WIN64)
        unsigned char mImplementationSpace[59000];
#else
        unsigned char mImplementationSpace[74520];
#endif
    };
}

#endif
