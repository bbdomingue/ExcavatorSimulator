% ========== HAPTIC BACKHOE CONTROLLER STARTUP ROUTINE =========
% before building the Haptic Backhoe controller program in Simulink
Ts = 0.001;

%1 bar = 14.5037738 pounds per square inch
 bar2psi = 14.5037738;
%1 newton  0.224808943 pounds force
 N2lb = 0.224808943;
%1 inches  0.0254 meters
 in2m = 0.0254;
%1 cubic meter  61 023.7441 cubic inches
 m32in3 = 1/((in2m)*(in2m)*(in2m));
%1 pound per square inch  6 894.75729 pascals
 psi2pa = 6894.75729;
%1 cc  0.0610237441 cubic inches
cc_2_in3 = 0.0610237441;
%from LoadConversion
N_m_2_lb_in = 8.85075;

enginespeed = 2500;     % speed of diesel engine in rpm
maxflow_per_rev = 18*cc_2_in3;  % maximum pump displacement in in^3 (18cc/rev from purdue)
pumpspeed = [enginespeed 3442 3442 3442]' ./ 60;  % speed of the pumps in rpm -> rps
maxpflow = maxflow_per_rev*pumpspeed;           % maximum pump flow at the given pump speed
torqmax = 170*N_m_2_lb_in;  % maximum engine torque (in lb)

tankp = 30*bar2psi;%150;        % 30 from purdue tank pressure in psi

prodinit = [tankp tankp tankp tankp];   % initial headside pressures
pheadinit = [tankp tankp tankp tankp];  % initial rodside pressures

%fcswing = 87.5*Nm2ftlb;%600*Nm2ftlb;  % [N*m*s] coeff of viscous friction
fcswing = 87.5;
fcboom = 1.53E4 * N2lb; %1.26E4 * N2lb; %[N] coeff of coulombic friction
fcarm = 1.45E3* N2lb;
fcbucket = 1e2 * N2lb;

%fvswing = 1.72*Nm2ftlb;%600*Nm2ftlb;  % [N*m*s] coeff of viscous friction
fvswing = 1.72;
fvboom = 1.26E3;%2.25E3;% [kg/s] coeff of viscous friction
fvarm = 1.93E4;
fvbucket = 2.88E4;

bulkmodulus = 150000;       %psi 150000
reliefp = 193*bar2psi; %4500 %psi

%BOOM%
linevol_head2 = (20e-4 * m32in3); %[m^3] volume of piston-side line
linevol_rod2 = (30e-4 * m32in3); %[m^3] volume of rod-side line
%ARM%
linevol_head3 = (15e-4 * m32in3); % [m^3] volume of piston-side line
linevol_rod3 = (20e-4 * m32in3); % [m^3] volume of rod-side line
%BUCKET%
linevol_head4 = 30e-4 * m32in3; % [m^3] volume of piston-side line
linevol_rod4 = 35e-4 * m32in3; % [m^3] volume of rod-side line
%SLEW%
linevol_head1 = (15e-4 * m32in3); % [m^3] volume of piston-side line
linevol_rod1 = (15e-4 * m32in3); % [m^3] volume of rod-side line

leak1 = (5e-13 * m32in3 * psi2pa); % [m^3/s/Pa] Coefficient of internal leakage
leak2 = (6e-13 * m32in3 * psi2pa); %[m^3/s/Pa] Coefficient of internal leakage
leak3 = (6e-13 * m32in3 * psi2pa); % [m^3/s/Pa] Coefficient of internal leakage
leak4 = 6e-13 * m32in3 * psi2pa; % [m^3/s/Pa] Coefficient of internal leakage

KLi_ext_S = (9e-13 * m32in3 * psi2pa); % [m^3/s/Pa] Coefficient of external leakage

LoadFuel
fuelmatrix = standardfuelmap;
torqscale = newstandardtorq;
speedscale = standengspeed;

return;

[qinit, qdotinit] = c2j_bobcat(cylposinit, [0 0 0 0]);








































