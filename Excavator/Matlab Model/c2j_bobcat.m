function [q, qdot] = c2j_bobcat(cylpos, cylv)

    bobcat_param
    
    t1 = cylpos(1);
    y2 = cylpos(2);
    y3 = cylpos(3);
    y4 = cylpos(4);
    
    w1 = cylv(1);
    v2 = cylv(2);
    v3 = cylv(3);
    v4 = cylv(4);
    %(r1A*r1A + r1B*r1B - y2*y2)/(2*r1A*r1B)
    tA1B = acos((r1A*r1A + r1B*r1B - y2*y2)/(2*r1A*r1B));
    %tA1B = pi - t01A - t2 - tB12;
    %tA1B = tB12 + t2 + pi - t01A;
    %if (y2 > 59.1809) tA1B = -tA1B;
	t2 = -pi + t01A + tA1B - tB12;
	C2 = y2/(r1A*r1B*sin(tA1B));
	w2 = C2*v2;
	%T2 = f2/C2;
    
%     tA1B = acos((r1A*r1A + r1B*r1B - y2*y2)/(2*r1A*r1B));
% 	t2 = pi - t01A - tA1B - tB12;
% 	C2 = -y2/(r1A*r1B*sin(tA1B));
% 	w2 = C2*v2;
% 	%T2 = f2/C2;

	%/************************************************************
	%						axis #3
	%************************************************************/
	tC2D = acos((r2C*r2C + r2D*r2D - y3*y3)/(2*r2C*r2D));
	t3 = pi - t12C - tC2D - tD23;
	C3 = -y3/(r2C*r2D*sin(tC2D));
	w3 = C3*v3;
	%T3 = f3/C3;

	%/************************************************************
	%						axis #4
	%************************************************************/
    tEFH = acos((rEF*rEF + rFH*rFH - y4*y4)/(2*rEF*rFH));
	rC = rFH*cos(alfa+tEFH);
	rS = rFH*sin(alfa+tEFH);
	r3Hx = r3I + rC;
	r3Hy = rFI + rS;
	t23H = atan2(r3Hy,r3Hx);
	r3H_2 = r3Hx*r3Hx + r3Hy*r3Hy;
	r3H = sqrt(r3H_2);
	cH3G = (r3H*r3H + r3G*r3G - rGH*rGH)/(2*r3H*r3G);
	tH3G = acos(cH3G);
	t4 = pi - (t23H + tH3G + tG34);

	wEFH = y4/(rEF*rFH*sin(tEFH));
	w23H = (r3I*rC + rFI*rS +rFH*rFH)/r3H_2*wEFH;
	wH3G = (rFI*rC - r3I*rS)*(r3G*cH3G - r3H)/(r3H_2*r3G*sin(tH3G))*wEFH;
	C4 = -w23H - wH3G;
    w4 = C4*v4;
    
% 	tEFH = acos((rEF*rEF + rFH*rFH - y4*y4)/(2*rEF*rFH));
%     tEFH = tEFH + alfa;
%     %alphaEFH = alpha + tEFH;
% 	%rC = rFH*cos(alpha+tEFH);
%     %rC = alpha + tEFH;
%     rC = rFH * cos(tEFH);
% 	%rS = rFH*sin(alpha+tEFH);
% 	%rS = alpha + tEFH;    %don't ask me why this command has to be split in two
%     rS = rFH*sin(tEFH);   %the compiler doesn't like it as one line
% 	r3Hx = r3I + rC;
% 	r3Hy = rFI + rS;
% 	t23H = atan2(r3Hy,r3Hx);
% 	r3H_2 = r3Hx*r3Hx + r3Hy*r3Hy;
% 	r3H = sqrt(r3H_2);
% 	cH3G = (r3H*r3H + r3G*r3G - rGH*rGH)/(2*r3H*r3G);
% 	tH3G = acos(cH3G);
%     %t4 = pi - (t23H + tH3G + tG34);
%     t4 = pi - t23H;
%     t4 = t4 - tH3G - tG34;
% 
% 	wEFH = y4/(rEF*rFH*sin(tEFH));
% 	w23H = (r3I*rC + rFI*rS +rFH*rFH)/r3H_2*wEFH;
% 	wH3G = (rFI*rC - r3I*rS)*(r3G*cH3G - r3H)/(r3H_2*r3G*sin(tH3G))*wEFH;
% 	C4 = -w23H - wH3G;
%      
%     %t4 = 0;
% 	w4 = C4*v4;
% 	%T4 = f4/C4;

    q = [t1 t2 t3 t4];
    qdot = [w1 w2 w3 w4];