function [pos, vel] = j2t_bobcat(theta, omega)
    
    bobcat_param
    
    t1 = theta(1);
    t2 = theta(2);
    t3 = theta(3);
    t4 = theta(4);
    
    w1 = omega(1);
    w2 = omega(2);
    w3 = omega(3);
    w4 = omega(4);

	%/************************************************************
	%			Trig/Link Length Calculations
	%************************************************************/ 
    c1 = cos(t1);
	c2 = cos(t2);
	c23 = cos(t2 + t3);
	c234 = cos(t2 + t3 + t4);
	s1 = sin(t1);
	s2 = sin(t2);
	s23 = sin(t2 + t3);
	s234 = sin(t2 + t3 + t4);
    
	%/************************************************************
	%			Position Calculations
	%************************************************************/
    p02 = [a1*c1-d1*s1 a1*s1+d1*c1 0];
    p03 = p02 + [a2*c2*c1 a2*c2*s1 a2*s2];
    p04 = p03 + [a3*(c23)*c1 a3*(c23)*s1 a3*(s23)];
    p0tip = p03 + [a4*(c234)*c1 a4*(c234)*s1 a4*(s234)];

    x = a1*c1-d1*s1 + a2*c2*c1 + a3*(c23)*c1; 
    y = a1*s1+d1*c1 + a2*c2*s1 + a3*(c23)*s1;
    z = a2*s2 + a3*(s23);
    
%     x = p04(1);
%     y = p04(2);
%     z = p04(3);
    phi = t4;
    
    pos = [x y z phi];
    
% 	/************************************************************
% 				Velocity Calculations
% 	*************************************************************
% 			V = J(th)*omega
% 	************************************************************/

% J11 = -t1-atan(d1/(a1+a2*c2+a3*c23+a4*c234));
% J12 = a4*s234;
% J13 = a4*s234-a2*s2;
% J14 = a4*s234-a3*s23;
% J21 = (a1^2+2*a1*a2*c2+2*a1*a3*c23+2*a1*a4*c234+a2^2*c2^2+2*a2*c2*a3*c23+2*a2*c2*a4*c234+a3^2*c23^2+2*a3*c23*a4*c234+a4^2*c234^2+d1^2)^(1/2);
% J22 = 0;
% J23 = 0;
% J24 = 0;
% J31 = 0;
% J32 = -(a1^2+2*a1*a2*c2+2*a1*a3*c23+2*a1*a4*c234+a2^2*c2^2+2*a2*c2*a3*c23+2*a2*c2*a4*c234+a3^2*c23^2+2*a3*c23*a4*c234+a4^2*c234^2+d1^2)^(1/2)+(a1^2+d1^2)^(1/2);
% J33 = -(a1^2+2*a1*a2*c2+2*a1*a3*c23+2*a1*a4*c234+a2^2*c2^2+2*a2*c2*a3*c23+2*a2*c2*a4*c234+a3^2*c23^2+2*a3*c23*a4*c234+a4^2*c234^2+d1^2)^(1/2)+(a1^2+2*a1*a2*c2+a2^2*c2^2+d1^2)^(1/2);
% J34 = -(a1^2+2*a1*a2*c2+2*a1*a3*c23+2*a1*a4*c234+a2^2*c2^2+2*a2*c2*a3*c23+2*a2*c2*a4*c234+a3^2*c23^2+2*a3*c23*a4*c234+a4^2*c234^2+d1^2)^(1/2)+(a1^2+2*a1*a2*c2+2*a1*a3*c23+a2^2*c2^2+2*a2*c2*a3*c23+a3^2*c23^2+d1^2)^(1/2);
% J41 = 0;
% J42 = 1;
% J43 = 1;
% J44 = 1;

J11 = -t1-atan(d1/(a1+a2*c2+a3*c23+a4*c234));
J12 = a4*s234;
J13 = a4*s234-a2*s2;
J14 = a4*s234-a3*s23;
J21 = sqrt((a1*a1+2*a1*a2*c2+2*a1*a3*c23+2*a1*a4*c234+a2*a2*c2*c2+2*a2*c2*a3*c23+2*a2*c2*a4*c234+a3*a3*c23*c23+2*a3*c23*a4*c234+a4*a4*c234*c234+d1*d1));
J22 = 0;
J23 = 0;
J24 = 0;
J31 = 0;
J32 = -sqrt(a1*a1+2*a1*a2*c2+2*a1*a3*c23+2*a1*a4*c234+a2*a2*c2*c2+2*a2*c2*a3*c23+2*a2*c2*a4*c234+a3*a3*c23*c23+2*a3*c23*a4*c234+a4*a4*c234*c234+d1*d1)+sqrt(a1*a1+d1*d1);
J33 = -sqrt(a1*a1+2*a1*a2*c2+2*a1*a3*c23+2*a1*a4*c234+a2*a2*c2*c2+2*a2*c2*a3*c23+2*a2*c2*a4*c234+a3*a3*c23*c23+2*a3*c23*a4*c234+a4*a4*c234*c234+d1*d1)+sqrt(a1*a1+2*a1*a2*c2+a2*a2*c2*c2+d1*d1);
J34 = -sqrt(a1*a1+2*a1*a2*c2+2*a1*a3*c23+2*a1*a4*c234+a2*a2*c2*c2+2*a2*c2*a3*c23+2*a2*c2*a4*c234+a3*a3*c23*c23+2*a3*c23*a4*c234+a4*a4*c234*c234+d1*d1)+sqrt(a1*a1+2*a1*a2*c2+2*a1*a3*c23+a2*a2*c2*c2+2*a2*c2*a3*c23+a3*a3*c23*c23+d1*d1);
J41 = 0;
J42 = 1;
J43 = 1;
J44 = 1;


%   old jacobian
%     J11 = -t1-atan(d1/(a1+a2*c2));
%     J12 = a2*s2+a4*s234;
%     J13 = a4*s234;
%     J14 = a2*s2+a4*s234-a3*s23;
%     J21 = (a1*a1+2*a1*a2*c2+a2*a2*c2*c2+d1*d1)^(1/2)+a4*c234;
%     J22 = 0;
%     J23 = 0;
%     J24 = 0;
%     J31 = 0;
%     J32 = -(a1*a1+2*a1*a2*c2+a2*a2*c2*c2+d1*d1)^(1/2)-a4*c234+(a1*a1+d1*d1)^(1/2);
%     J33 = -a4*c234;
%     J34 = -(a1*a1+2*a1*a2*c2+a2*a2*c2*c2+d1*d1)^(1/2)-a4*c234+(a1*a1+2*a1*a2*c2+2*a1*a3*c23+a2*a2*c2*c2+2*a2*c2*a3*c23+a3*a3*c23*c23+d1*d1)^(1/2);
%     J41 = 0;
%     J42 = 1;
%     J43 = 1;
%     J44 = 1;
    
    
    vr =     J11*w1 + J12*w2 + J13*w3 + J14*w4;
    vtheta = J21*w1 + J22*w2 + J23*w3 + J24*w4;
    vz =     J31*w1 + J32*w2 + J33*w3 + J34*w4;
    vphi =   J41*w1 + J42*w2 + J43*w3 + J44*w4;
    
    vx = vr*cos(t1) - vtheta*cos(t1-pi/2);
    vy = vr*sin(t1) - vtheta*sin(t1-pi/2);

    vel = [vx vy vz vphi];
    