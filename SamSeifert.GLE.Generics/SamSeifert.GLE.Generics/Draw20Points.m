%% Start With 4 Sided Pyramid
clc;
close all;
clear all;
cnv = pi / 180;

triangs = [];

a = 1;
b = a / sqrt(3);
h = sqrt (a^2 - b^2);
z = b - h;
p1 = [0, 0, b];
ang = 120 * cnv; p2 = [b * cos(ang), b * sin(ang), z];
ang = 240 * cnv; p3 = [b * cos(ang), b * sin(ang), z];
ang = 360 * cnv; p4 = [b * cos(ang), b * sin(ang), z];
        

triang.v1 = p1;
triang.v2 = p3;
triang.v3 = p2;    
triangs = [triangs; triang];

triang.v1 = p1;
triang.v2 = p4;
triang.v3 = p3;    
triangs = [triangs; triang];

triang.v1 = p1;
triang.v2 = p2;
triang.v3 = p4;    
triangs = [triangs; triang];

triang.v1 = p2;
triang.v2 = p3;
triang.v3 = p4;    
triangs = [triangs; triang];   

%% Start With 20 Sided Die
clc;
close all;
clear all;
cnv = pi / 180;

r = 0.5 / sin(36 * cnv);
r_sq = r * r;
vx = sqrt(1 - r_sq);
syms vy;
sol = solve(vx + (vy / 2) - sqrt(r_sq + (vy / 2)^2));
vy = double(sol);

triangs = [];

pnt3 = [0, 0, 0];
pnt2 = [0, 0, 0];
pnt1 = [0, 0, 0];

for i = 1:12
    ang = i * 36 * cnv;
    x = r * cos(ang);
    y = r * sin(ang);
    z = vy / 2;
    if (mod(i,2) == 0); z = -z; end;
    
    pnt3 = pnt2;
    pnt2 = pnt1;
    pnt1 = [x, y, z];
    pnt1 = pnt1 / norm(pnt1);
    
    if (i > 2)
        triang = struct();
        triang.v1 = pnt1;

        if (mod(i,2) == 0);
            triang.v2 = pnt2;
            triang.v3 = pnt3;
        else;
            triang.v2 = pnt3;
            triang.v3 = pnt2;
        end;
        
        triangs = [triangs; triang];
    end
end

for i = 1:7
    ang = (i * 72 + 36) * cnv;
    x = r * cos(ang);
    y = r * sin(ang);
    z = vy / 2;
    
    pnt2 = pnt1;
    pnt1 = [x, y, z];
    pnt1 = pnt1 / norm(pnt1);
    
    if (i > 2)
        triang = struct();
        triang.v1 = pnt2;
        triang.v2 = pnt1;
        triang.v3 = [0, 0, 1];
        triangs = [triangs; triang];
    end
end

for i = 1:7
    ang = (i * 72) * cnv;
    x = r * cos(ang);
    y = r * sin(ang);
    z = vy / 2;
    
    pnt2 = pnt1;
    pnt1 = [x, y, -z];
    pnt1 = pnt1 / norm(pnt1);
    
    if (i > 2)
        triang = struct();
        triang.v1 = pnt1;
        triang.v2 = pnt2;
        triang.v3 = [0, 0, -1];
        triangs = [triangs; triang];
    end
end%
% hold on;

% points = [points; ];
% points = [points; 0, 0, -(vx + vy / 2)];

%scatter3(points(:, 1), points(:, 2), points(:, 3));

%% Duplicate

ntriangs(size(triangs, 1) * 4, 1) = struct('v1', [0, 0, 0],'v2', [0, 0, 0],'v3', [0, 0, 0]);

for i = 1:size(triangs, 1)
    triang = triangs(i , 1);
    p1 = triang.v1;
    p2 = triang.v2;
    p3 = triang.v3;
    p4 = p3 + p1;
    p5 = p2 + p1;
    p6 = p2 + p3;
    p1 = p1 / norm(p1);
    p2 = p2 / norm(p2);
    p3 = p3 / norm(p3);
    p4 = p4 / norm(p4);
    p5 = p5 / norm(p5);
    p6 = p6 / norm(p6);
    
    dex = (i - 1) * 4;  
    ntriangs(dex + 1).v1 = p1;
    ntriangs(dex + 1).v2 = p5;
    ntriangs(dex + 1).v3 = p4;    
    
    ntriangs(dex + 2).v1 = p5;
    ntriangs(dex + 2).v2 = p2;
    ntriangs(dex + 2).v3 = p6;    
    
    ntriangs(dex + 3).v1 = p4;
    ntriangs(dex + 3).v2 = p5;
    ntriangs(dex + 3).v3 = p6;    
    
    ntriangs(dex + 4).v1 = p4;
    ntriangs(dex + 4).v2 = p6;
    ntriangs(dex + 4).v3 = p3;    
end

triangs = ntriangs;
clear ntriangs;

% Plot

figure();
hold on;
title(['Sides: ' int2str(size(triangs, 1))]);

for i = 1:size(triangs, 1)
    triang = triangs(i , 1);
    dex = 1; X = [triang.v1(dex); triang.v2(dex); triang.v3(dex); triang.v1(dex);];
    dex = 2; Y = [triang.v1(dex); triang.v2(dex); triang.v3(dex); triang.v1(dex);];
    dex = 3; Z = [triang.v1(dex); triang.v2(dex); triang.v3(dex); triang.v1(dex);];
    plot3(X, Y, Z);
end
axis equal;
axis [-1 1 -1 1 -1 1 -1 1];

%% Print
clc
disp('_Sphere = new Triangle[] {');
i = 1;
while (1)
    triang = triangs(i , 1);
    disp('          new Triangle(');
    disp(['            fm(' num2str(triang.v1(1)) ',' num2str(triang.v1(2)) ',' num2str(triang.v1(3)) '),' ]);
    disp(['            fm(' num2str(triang.v2(1)) ',' num2str(triang.v2(2)) ',' num2str(triang.v2(3)) '),' ]);
    i = i + 1;
    if (i > size(triangs, 1))
        disp(['            fm(' num2str(triang.v3(1)) ',' num2str(triang.v3(2)) ',' num2str(triang.v3(3)) ')) };' ]);
        break;
    else
        disp(['            fm(' num2str(triang.v3(1)) ',' num2str(triang.v3(2)) ',' num2str(triang.v3(3)) ')),' ]);        
    end    
end

