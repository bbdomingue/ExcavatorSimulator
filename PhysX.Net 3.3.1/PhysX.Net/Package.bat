
cd F:\Development\PhysX.Net\PhysX.Net
F:

cd Package
rd Working /S /Q
md Working
cd Working
md "PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6"
cd "PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6"
md "Assemblies"
md "Assemblies\x86"
md "Assemblies\x86\Debug"
md "Assemblies\x86\Release"
md "Assemblies\x64"
md "Assemblies\x64\Debug"
md "Assemblies\x64\Release"
cd ..\..\..

# Samples
md "Package\Working\PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Samples"
robocopy Samples "Package\Working\PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Samples" /E /XF *.suo *.vssscc *.cachefile *.user *.vspscc *.pdb /XD obj Release
md "Package\Working\PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Assemblies"
robocopy Assemblies "Package\Working\PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Assemblies" /E

msbuild PhysX.Net.sln /p:Configuration=Debug;Platform=x86
msbuild PhysX.Net.sln /p:Configuration=Release;Platform=x86
msbuild PhysX.Net.sln /p:Configuration=Debug;Platform=x64
msbuild PhysX.Net.sln /p:Configuration=Release;Platform=x64

xcopy "PhysX.Net\bin\x86\Debug\StillDesign.PhysX.Net.dll" "Package\Working\PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Assemblies\x86\Debug"
xcopy "PhysX.Net\bin\x86\Debug\StillDesign.PhysX.Net.pdb" "Package\Working\PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Assemblies\x86\Debug"
xcopy "PhysX.Net\bin\x86\Release\StillDesign.PhysX.Net.dll" "Package\Working\PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Assemblies\x86\Release"

xcopy "PhysX.Net\bin\x64\Debug\StillDesign.PhysX.Net.dll" "Package\Working\PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Assemblies\x64\Debug"
xcopy "PhysX.Net\bin\x64\Debug\StillDesign.PhysX.Net.pdb" "Package\Working\PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Assemblies\x64\Debug"
xcopy "PhysX.Net\bin\x64\Release\StillDesign.PhysX.Net.dll" "Package\Working\PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Assemblies\x64\Release"

cd "Package\Working"

attrib * -R /S /D
..\..\Tools\StripSourceControl "PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Samples\PhysX.Net Samples.sln"
..\..\Tools\StripSourceControl "PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Samples\CCD Skeleton\CCD Skeleton.csproj"
..\..\Tools\StripSourceControl "PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Samples\Engine\Engine.csproj"
..\..\Tools\StripSourceControl "PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Samples\Fluid Blob\Fluid Blob.csproj"
..\..\Tools\StripSourceControl "PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Samples\Main Sample\Main Sample.csproj"
..\..\Tools\StripSourceControl "PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6\Samples\Tearing Cloth\Tearing Cloth.csproj"
..\..\Tools\7-Zip\7z.exe a "PhysX.Net 0.12.1 for NVIDIA PhysX 2.8.4.6.zip" .

cd ..\..
