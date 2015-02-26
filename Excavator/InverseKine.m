clc;
clear all;

syms a2;
syms a3;

syms A;
syms B;

Rz = @(thetaZ) [cos(thetaZ),  -sin(thetaZ),  0;
               sin(thetaZ),  cos(thetaZ),  0;
               0,             0,            1];
           
p1 = Rz(B) * ([a3; 0; 0]);
pBucket = Rz(A) * ([a2; 0; 0] + p1)
pArm = Rz(A)* ([a2; 0; 0])

diffA = diff(pBucket, A)
diffB = diff(pBucket, B)
