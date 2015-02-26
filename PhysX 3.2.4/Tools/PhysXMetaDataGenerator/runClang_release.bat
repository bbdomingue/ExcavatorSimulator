
@echo off

rem Find SDK_ROOT by searching backwards from cwd for SDKs
set SDK_ROOT=%~p0
:sdkloop
if exist "%SDK_ROOT%\Source" goto :havesdkpath
set SDK_ROOT=%SDK_ROOT%..\
goto :sdkloop

:havesdkpath

echo %SDK_ROOT%
xcopy /Y /I /C /R *.h "%SDK_ROOT%\Include\"

set INCLUDES=-I"./../../Include/foundation"
set INCLUDES=%INCLUDES% -I"./../../Include/foundation/windows"
set INCLUDES=%INCLUDES% -I"./../../Include/physxprofilesdk"
set INCLUDES=%INCLUDES% -I"./../../Include/physxvisualdebuggersdk"
set INCLUDES=%INCLUDES% -I"./../../pxtask/include"
set INCLUDES=%INCLUDES% -I"./../../Include/common"		
set INCLUDES=%INCLUDES% -I"./../../Include/geometry"		
set INCLUDES=%INCLUDES% -I"./../../Include/pvd"		
set INCLUDES=%INCLUDES% -I"./../../Include/particles"		
set INCLUDES=%INCLUDES% -I"./../../Include/cloth"		
set INCLUDES=%INCLUDES% -I"./../../Include/physxvisualdebuggersdk"
set INCLUDES=%INCLUDES% -I"./../../Include/gpu"
set INCLUDES=%INCLUDES% -I"./../../Include"
set INCLUDES=%INCLUDES% -I"./../../Source/PhysXCommon/src"		
set INCLUDES=%INCLUDES% -I"./../../Source/GeomUtils/headers"		
set INCLUDES=%INCLUDES% -I"./../../Source/GeomUtils/src"		
set INCLUDES=%INCLUDES% -I"./../../Source/GeomUtils/Opcode"		
set INCLUDES=%INCLUDES% -I"./../../Source/PhysX/src"
set INCLUDES=%INCLUDES% -I"./../../Source/PhysX/src/buffering"
set INCLUDES=%INCLUDES% -I"./../../Source/PhysX/src/particles"
set INCLUDES=%INCLUDES% -I"./../../Source/PhysX/src/cloth"		
set INCLUDES=%INCLUDES% -I"./../../Source/SimulationController/src"		
set INCLUDES=%INCLUDES% -I"./../../Source/SimulationController/src/framework"	
set INCLUDES=%INCLUDES% -I"./../../Source/SimulationController/include"		
set INCLUDES=%INCLUDES% -I"./../../Source/PhysXCooking/include"		
set INCLUDES=%INCLUDES% -I"./../../Source/SceneQuery"		
set INCLUDES=%INCLUDES% -I"./../../Source/PvdRuntime/src"		
set INCLUDES=%INCLUDES% -I"./../../Source/PhysXMetaData/core/include"	
set INCLUDES=%INCLUDES% -I"./../../Source/PhysXGpu/include"

set CMD="%SDK_ROOT%\Tools\PhysXMetaDataGenerator\clang.exe" -cc1 -x c++-header -fms-extensions -std=c++0x %INCLUDES% -w -DNDEBUG -DWIN32 -DPX_VC -nobuiltininc "%SDK_ROOT%/Include/PxPhysicsWithExtensionsAPI.h" -boilerplate-file "%SDK_ROOT%/Tools/Validation/boilerplate.txt" -o "%SDK_ROOT%/Source/PhysXMetaData"


echo %CMD% >>"%SDK_ROOT%\Tools\PhysXMetaDataGenerator\clangCommandLine.txt"
%CMD%



