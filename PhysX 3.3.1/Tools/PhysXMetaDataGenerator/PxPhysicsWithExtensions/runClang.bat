@echo off

call get_extension_settings.bat

set CMD= %EXE% %COMMON_FLAGS% %WINDOWS_DEFINES% %INCLUDES% %SRCPATH% -o %TARGETDIR% 
echo %CMD% >>clangCommandLine_windows.txt
%CMD%

set TARGETDIR=


