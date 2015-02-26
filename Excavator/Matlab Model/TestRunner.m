clear all;
LoadVars
mexall;
clc;
SimEndTime = 10;
SimEndTime1000 = SimEndTime * 1000;
Sim_Time = ((0:(SimEndTime1000 - 1))/1000)';
Q = [Sim_Time, zeros(SimEndTime1000,4)];
Qd = [Sim_Time, zeros(SimEndTime1000,4)];
Flow1 = [zeros(3000,1); zeros(4000,1) + 1; zeros(3000,1)];
Flow = [Sim_Time, Flow1, zeros(SimEndTime1000,3)];

sim('Tester.slx');

Sim_Time_Out = ((0:(SimEndTime1000))/1000)';

plotter = torques;
figure();
hold on;
plot (Sim_Time_Out, plotter(:, 1), 'r');
% plot (Sim_Time_Out, plotter(:, 2), 'g');
% plot (Sim_Time_Out, plotter(:, 3), 'b');
% plot (Sim_Time_Out, plotter(:, 4), 'y');
