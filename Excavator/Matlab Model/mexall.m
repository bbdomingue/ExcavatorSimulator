% function [] = mexall()
%
% Will mex all the c s-function in the current directory
function [] = mexall()

dirname = cd;
files=extractfiles(dirname,'.c');

disp('mex s-functions')
for k=1:length(files)
   eval(['mex ' files{k} '.c'])
   disp(['     ' files{k} '.c']) 
end

function files=extractfiles(fileLocation, ext)
  dirstr=dir(fileLocation);
  files=[];
  len=length(ext);
  for i=1:length(dirstr)
    if (~dirstr(i).isdir)
      if strcmp(dirstr(i).name(end+1-len:end), ext)
	files = [files {dirstr(i).name(1:end-len)}];
      end
    end
  end