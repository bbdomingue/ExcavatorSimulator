% ========== HAPTIC BACKHOE CONTROLLER STARTUP ROUTINE =========
% before building the Haptic Backhoe controller program in Simulink

Ts = 0.001;
timeconst = (1/.075);     % pump time constant (Hz) 1/.075

%from LoadConversion
m_2_in = 1/0.0254;
N_2_lb = 1/4.448221615;
N_m_2_lb_in = 8.85075;
N_per_m_2_lb_per_in = 0.0254/4.448221615;
Pa_2_psi = 1/6894.757293;

%1 bar = 14.5037738 pounds per square inch
bar2psi = 14.5037738;
%1 pound per square inch  6 894.75729 pascals
 psi2pa = 6894.75729;
%1 newton meter  0.737562149 foot pound
 Nm2ftlb = 0.737562149;
%1 inches  0.0254 meters
 in2m = 0.0254;
%1 newton  0.224808943 pounds force
 N2lb = 0.224808943;
%1 cubic meter  61 023.7441 cubic inches
 m32in3 = 1/((in2m)*(in2m)*(in2m));
%1 cc  0.0610237441 cubic inches
cc_2_in3 = 0.0610237441;

%OFFSET%
VO_A = (3e-4 * m32in3); %[m^3] volume of piston-side line
VO_B = (3e-4 * m32in3); % [m^3] volume of rod-side line
KLi_O = (6e-13 * m32in3 * psi2pa); % [m^3/s/Pa] Coefficient of internal leakage
%BOOM%
linevol_head2 = (20e-4 * m32in3); %[m^3] volume of piston-side line
linevol_rod2 = (30e-4 * m32in3); %[m^3] volume of rod-side line
leak2 = (6e-13 * m32in3 * psi2pa); %[m^3/s/Pa] Coefficient of internal leakage
%ARM%
linevol_head3 = (15e-4 * m32in3); % [m^3] volume of piston-side line
linevol_rod3 = (20e-4 * m32in3); % [m^3] volume of rod-side line
leak3 = (6e-13 * m32in3 * psi2pa); % [m^3/s/Pa] Coefficient of internal leakage
%BUCKET%
linevol_head4 = 30e-4 * m32in3; % [m^3] volume of piston-side line
linevol_rod4 = 35e-4 * m32in3; % [m^3] volume of rod-side line
leak4 = 6e-13 * m32in3 * psi2pa; % [m^3/s/Pa] Coefficient of internal leakage
%SLEW%
linevol_head1 = (15e-4 * m32in3); % [m^3] volume of piston-side line
linevol_rod1 = (15e-4 * m32in3); % [m^3] volume of rod-side line
leak1 = (5e-13 * m32in3 * psi2pa); % [m^3/s/Pa] Coefficient of internal leakage
KLi_ext_S = (9e-13 * m32in3 * psi2pa); % [m^3/s/Pa] Coefficient of external leakage

% from purdue
% %fcswing = 87.5*Nm2ftlb;%600*Nm2ftlb;  % [N*m*s] coeff of viscous friction
% fcswing = 87.5;
% fcboom = 1.53E4 * N2lb; %[N] coeff of coulombic friction
% fcarm = 1.45E3* N2lb;
% fcbucket = 1e2 * N2lb;
% 
% %fvswing = 1.72*Nm2ftlb;%600*Nm2ftlb;  % [N*m*s] coeff of viscous friction
% fvswing = 1.72;
% fvboom = 1.26E3;% [kg/s] coeff of viscous friction
% fvarm = 1.93E4;
% fvbucket = 2.88E4;

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

reliefp = 193*bar2psi; %4500 %psi
tankp = 30*bar2psi;%150;        % 30 from purdue tank pressure in psi
bulkmodulus = 150000;       %psi 150000

gear_ratio = 91/17;     % ratio of swing gears - orbital to planetary
Vm = 820e-6;    % [m^3/rev] swing motor displacement (from purdue)
motor_disp_per_rev = Vm*m32in3;     %swing motor displacement per revolution in in^3; %m3_2_in3;

enginespeed = 2500;     % speed of diesel engine in rpm

maxflow_per_rev = 18*cc_2_in3;  % maximum pump displacement in in^3 (18cc/rev from purdue)
pumpspeed = [enginespeed 3442 3442 3442]' ./ 60;  % speed of the pumps in rpm -> rps
maxpflow = maxflow_per_rev*pumpspeed;           % maximum pump flow at the given pump speed

phantom_deadband = 35;      % deadband for phantom velocity control (radius of sphere)
wallforce = .25;            % initial force for phantom velocity control
phantomspringk = .02;       % spring constant for phantom vel ctrl

torqmax = 170*N_m_2_lb_in;  % maximum engine torque (in lb)

bobcat_param
cylinfo

LoadSoil
fuelmap

speedscale = standengspeed;
torqscale = newstandardtorq;
fuelmatrix = standardfuelmap;

maxforce = 5;       % maximum phantom force

prodinit = [tankp tankp tankp tankp];   % initial headside pressures
pheadinit = [tankp tankp tankp tankp];  % initial rodside pressures

qinit = [0 40 -90 0] *pi/180;       % initial joint angles in rad
qdotinit = [0 0 0 0];               % initial joint velocities in rad/s

%throwaway = 20;
throwaway = 10*50;
%load C:\work\excavator\purdue\rundata\excavator_data2
%load C:\work\excavator\purdue\rundata\excavator_data_120309
%load C:\work\excavator\purdue\rundata\swing_armout\data
%load C:\work\excavator\purdue\rundata\open_loop\boom_armin\data
%load C:\work\excavator\purdue\rundata\open_loop\boom_armout\data
%load C:\work\excavator\purdue\rundata\open_loop\bucket\data
load data


prodinit = [pressure.data(throwaway,1),cylpress.data(throwaway,1),cylpress.data(throwaway,3),cylpress.data(throwaway,5)]*bar2psi;
pheadinit = [pressure.data(throwaway,2),cylpress.data(throwaway,2),cylpress.data(throwaway,4),cylpress.data(throwaway,6)]*bar2psi;
len = length(joystick.data);
len = len - round(len/20);
joymat = joystick.data(throwaway:len,[1:4,7]);
joymat(:,5) = joymat(:,5)-joymat(1,5)+.02;

%swashmat = swash.data(throwaway:len,1:5);
swashmat = swashref.data(throwaway:len,1:5);

swashmat(:,5) = swashmat(:,5)-swashmat(1,5)+.02;

cylposinit = position.data(throwaway,1:4);
cylposinit(1) = cylposinit(1)*pi/180;
cylposinit(2) = cylposinit(2)/100*stroke2+Ymin2;
cylposinit(4) = cylposinit(4)/100*stroke4+Ymin4;
cylposinit(3) = cylposinit(3)/100*stroke3+Ymin3;

[qinit, qdotinit] = c2j_bobcat(cylposinit, [0 0 0 0]);
qdotinit = [0 0.05 0.02 0];
qinit = [0    0.4503   -0.9568   -1.1514];
[pos, vel] = j2t_bobcat(qinit, qdotinit)
disp(['**********  Variables loaded!  *************']);

posf = pos + [-50 100 0 .5];
%posf = pos + [-30 50 0 .5];

% make_rtw;


%% Added By Sam

%PistonDamper = - [1; 1; 1; 1];

PC_Kp = [1; 1; 1; 1] * 0.99;
PC_Ki = [0; 0; 0; 0];
PC_Kd = [0; 0; 0; 0];

















































