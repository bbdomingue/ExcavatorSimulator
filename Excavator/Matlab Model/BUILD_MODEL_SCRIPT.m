clear all;
clc;

mexall;

LoadVars;

rtwbuild('MatlabExcavatorModel')
%open_system('MatlabExcavatorModel')

disp('**********  Build Successful!  *************');


clear all;

fpath = 'MatlabExcavatorModel_ert_rtw/msvc/MatlabExcavatorModel.vcxproj';
fid = fopen(fpath,'rt');

textOut = cell(1,1);
outCount = 1;

GUID = '';

typeCount = 0;
typePreprocess = '';
typeInclude = '';

lineInp = fgetl(fid);
while ischar(lineInp)
    lineOut = strrep(lineInp, '>Application<', '>DynamicLibrary<');

    if max(size(strfind(lineOut, '"..\ert_main.cpp"'))) ~= 0
        textOut{outCount, 1} = '    <ClCompile Include="..\..\WrapperLibrary\SamFunc.cpp" />';
        outCount = outCount + 1;
    end

    if max(size(strfind(lineOut, '"..\MatlabExcavatorModel.h"'))) ~= 0
        textOut{outCount, 1} = '    <ClInclude Include="..\..\WrapperLibrary\SamHeader.h" />';
        outCount = outCount + 1;
    end

    if max(size(strfind(lineOut, 'ProjectGuid'))) ~= 0
        lineOut = '    <ProjectGuid>{45E34D1E-F1C1-48C4-A4AF-D78860EC03FA}</ProjectGuid>';        
    end

    if max(size(strfind(lineOut, '<ItemDefinitionGroup'))) ~= 0
        typeCount = typeCount + 1;
    elseif max(size(strfind(lineOut, 'AdditionalIncludeDirectories'))) ~= 0
        if typeCount == 2
            typeInclude = lineOut;
        end
    elseif max(size(strfind(lineOut, '<PreprocessorDefinitions>'))) ~= 0
        if typeCount == 2
            typePreprocess = lineOut;
        elseif typeCount == 4
            textOut{outCount, 1} = strrep(typePreprocess, '_DEBUG', 'NDEBUG');
            outCount = outCount + 1;
            lineOut = typeInclude;
        end        
    end    
    
    

    textOut{outCount, 1} = lineOut;
    outCount = outCount + 1;

    if max(size(strfind(lineOut, '<LinkIncremental>'))) ~= 0
        textOut{outCount, 1} = '    <OutDir>$(SolutionDir)..\..\..\..\Build\Excavator\$(Configuration)\</OutDir>';
        outCount = outCount + 1;
        textOut{outCount, 1} = '    <IntDir>$(SolutionDir)..\..\..\..\Build\Excavator\$(Configuration)T\</IntDir>';
        outCount = outCount + 1;
    end     

     

    lineInp = fgetl(fid);
end

fclose(fid);

fid = fopen(fpath,'w');
for i = 1:length(textOut)
    line = textOut{i};
    line = strrep(line, '\', '\\');
    line = strrep(line, '%', '%%');
    fprintf(fid, line);    
    fprintf(fid, '\n');
end
fclose(fid);

disp('**********  MatlabExcavatorProject Mod Successful!  *************');
disp('DONE');

clear all;
