
a1 = 31.5423;
a1b = 5.60224; 
a2 = 104;
a3 = 54;
a4 = 34.6133;%34.6593;
d1 = -1.68;
q2 = (-pi/2 + acos(d1/a1));

r01 = a1;
r12 = a2;
r23 = a3;
r34 = a4;

r1A = 14.8486;
r1B = 54.4949;
r1C = 69.6644;
r2B = 54.4949;
r2C = 53.1519;
r2D = 14.8011;
r2E = 13.5656;
r3D = 66.3633;
r3E = 49.6927;
r3F = 7.7154;
r3G = 8.65592;
rEF = 40.8296;
rFH = 10.9881;
rFI = 0.576462;
r3I = r3F; 
rGH = 12.3942;%12.42;
r4G = 35.9042;%35.9193;

ro1 = 6.76942;
roA = 15.6884;
to1A = acos((ro1*ro1+r1A*r1A-roA*roA)/(2*ro1*r1A));
%r0A = 15.6884; 
t01o = .57;%acos(3.5893/ro1)

 
t01A = (t01o + to1A); %acos((r01*r01+r1A*r1A-r0A*r0A)/(2*r01*r1A));

t12C = acos((r12*r12+r2C*r2C-r1C*r1C)/(2*r12*r2C));
t23D = acos((r23*r23+r3D*r3D-r2D*r2D)/(2*r23*r3D));
tB12 = acos((r1B*r1B+r12*r12-r2B*r2B)/(2*r1B*r12));
tD23 = acos((r2D*r2D+r23*r23-r3D*r3D)/(2*r2D*r23));
tG34 = acos((r3G*r3G+r34*r34-r4G*r4G)/(2*r3G*r34));
t23E = acos((r23*r23+r3E*r3E-r2E*r2E)/(2*r23*r3E));
%pi = 3.1415926535897931;
alfa = 16.6878 * pi/180;

t1min = -400*pi/180;%-90*pi/180;
t1max = 400*pi/180;%90*pi/180;
Ymin2 = 40.5807;%42.1822;%      
Ymax2 = 67.3780;%58.8380;%44.625;
stroke2 = Ymax2 - Ymin2; %26.72
t2max = 66*pi/180;
t2min = -63*pi/180;
Ymin3 = 39.8290;%27.25;
Ymax3 = 65.8375;%44.625;
stroke3 = Ymax3 - Ymin3;     %26.0
t3max = -22*pi/180;
t3min = -145*pi/180; %-2.188768083034109
 
%dmax =(a2 + a3*cos(t3max)); %dmax 85.197418048124192
dmax = sqrt(a3*sin(t3max)*a3*sin(t3max) + (a2+a3*cos(t3max))*(a2+a3*cos(t3max)));
dmin = sqrt((a2 + a3*cos(t3min))*(a2 + a3*cos(t3min)) + a3*sin(t3min)*a3*sin(t3min)); %(a2 + a3*cos(t3min)) ;%40.606568032136707
 
t312min = atan((a3 * (-sin(t3max)))/(a2 + a3*cos(t3max))) ;
t312max = 27.5*pi/180;   %.5460 %0.88914881238211452
Ymin4 = 30.2084;    %23.375;
Ymax4 = 49.9174;%49.9009;    %49.8727;%37.125;
stroke4 = Ymax4 - Ymin4;     %20.63
t4max = 33*pi/180;  %0.13725870294037668
t4min = -154*pi/180; %-2.2578906226452915
%rmin = 59.2258; %(a1 + a2*cos(t2max)+ a3*cos(t2max + t3min));%20
%t2min_rmin_t3min =0; %-0.39451279633262937
%t3min_rmin_t2min =-150*pi/180; %-1.2103510689803758

% zmax_rmin_t2min =(a2*sin(t2min) + a3*sin(t2min+t3min));
% zmin_rmin_t3min =(a2*sin(t2min) + a3*sin(t2min+t3max)); 

zlowmin =(a2*sin(t2min) + a3*sin(t2min+t3min));
zlowmax =(a2*sin(t2min) + a3*sin(t2min+t3max)); 
zhighmin =(a2*sin(t2max) + a3*sin(t2max+t3min));
zhighmax =(a2*sin(t2max) + a3*sin(t2max+t3max));
rhighpt = a2*cos(t2max) + a3*cos(t2max+t3max);
rlowpt = a2*cos(t2min) + a3*cos(t2min+t3max);

% ztop =a2*sin(t2max) + a3*sin(t2max+t3max);
% zbottom =a2*sin(t2min) + a3*sin(t2min+t3min);
% rtop =a1 + a2*cos(t2max) + a3*cos(t2max+t3max);
% rbottom =a1 + a2*cos(t2min) + a3*cos(t2min+t3min);

bore2 = 3.75; %in
bore3 = 3.25;
bore4 = 3.0;

roddiam2 = 2;   %in
roddiam3 = 2;
roddiam4 = 1.75;

borerad2 = bore2/2; %in
borerad3 = bore3/2;
borerad4 = bore4/2;

rodrad2 = roddiam2/2;   %in
rodrad3 = roddiam3/2;
rodrad4 = roddiam4/2;

area_head2 = borerad2*borerad2*pi;
area_head3 = borerad3*borerad3*pi;
area_head4 = borerad4*borerad4*pi;

area_rod2 = area_head2 - rodrad2*rodrad2*pi;
area_rod3 = area_head3 - rodrad3*rodrad3*pi;
area_rod4 = area_head4 - rodrad4*rodrad4*pi;

% gear_ratio = 91/17;
% Vm = 820e-6;    % [m^3/rev] swing motor displacement
% %1 cubic meter  61 023.7441 cubic inches
% m3_2_in3 = 61023.7441;
% motor_disp_per_rev = Vm*m3_2_in3;
% 
% %1 cc  0.0610237441 cubic inches
% cc_2_in3 = 0.0610237441;
% maxflow_per_rev = 18*cc_2_in3; %18cc/rev
% motorspeed = 4500/60; %rpm -> rps
% maxpumpflow = maxflow_per_rev*motorspeed;


% %1 pound per square inch  6 894.75729 pascals
% psi_2_pa = 6894.75729;
% %1 newton meter  0.737562149 foot pound
% Nm_2_ftlb = 0.737562149;
% %1 inches  0.0254 meters
% in_2_m = 0.0254;
% %1 newton  0.224808943 pounds force
% N_2_lb = 0.224808943;
% 
% m3_2_in3 = (in_2_m)^(-3);

% %OFFSET%
% VO_A = 3e-4 * m3_2_in3; %[m^3] volume of piston-side line
% VO_B = 3e-4 * m3_2_in3; % [m^3] volume of rod-side line
% KLi_O = 6e-13 * m3_2_in3 * psi_2_pa;% [m^3/s/Pa] Coefficient of internal leakage
% %BOOM%
% linevol_head2 = 20e-4 * m3_2_in3; %[m^3] volume of piston-side line
% linevol_rod2 = 30e-4 * m3_2_in3; %[m^3] volume of rod-side line
% leak2 = 6e-13 * m3_2_in3 * psi_2_pa; %[m^3/s/Pa] Coefficient of internal leakage
% %ARM%
% linevol_head3 = 15e-4 * m3_2_in3; % [m^3] volume of piston-side line
% linevol_rod3 = 20e-4 * m3_2_in3; % [m^3] volume of rod-side line
% leak3 = 6e-13 * m3_2_in3 * psi_2_pa; % [m^3/s/Pa] Coefficient of internal leakage
% %BUCKET%
% linevol_head4 = 30e-4 * m3_2_in3; % [m^3] volume of piston-side line
% linevol_rod4 = 35e-4 * m3_2_in3; % [m^3] volume of rod-side line
% leak4 = 6e-13 * m3_2_in3 * psi_2_pa; % [m^3/s/Pa] Coefficient of internal leakage
% %SLEW%
% linevol_head1 = 15e-4 * m3_2_in3; % [m^3] volume of piston-side line
% linevol_rod1 = 15e-4 * m3_2_in3; % [m^3] volume of rod-side line
% leak1 = 5e-13 * m3_2_in3; % [m^3/s/Pa] Coefficient of internal leakage
% KLi_ext_S = 9e-13 * m3_2_in3 * psi_2_pa; % [m^3/s/Pa] Coefficient of external leakage

% %1 liters per hour = 0.01695104 (cubic inches) per sec
% lph_2_in3ps = 0.01695104;
% orificek = 5000*0.01695104/1e3*sqrt(psi_2_pa);