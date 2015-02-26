bore = zeros(1,4);
roddiam = zeros(1,4);
borerad = zeros(1,4);
rodrad = zeros(1,4);
area_head = zeros(1,4);
area_rod = zeros(1,4);

bore(2) = 3.75; %in
bore(3) = 3.25;
bore(4) = 3.0;

roddiam(2) = 2;   %in
roddiam(3) = 2;
roddiam(4) = 1.75;

borerad(2) = bore(2)/2; %in
borerad(3) = bore(3)/2;
borerad(4) = bore(4)/2;

rodrad(2) = roddiam(2)/2;   %in
rodrad(3) = roddiam(3)/2;
rodrad(4) = roddiam(4)/2;

area_head(2) = borerad(2)*borerad(2)*pi;
area_head(3) = borerad(3)*borerad(3)*pi;
area_head(4) = borerad(4)*borerad(4)*pi;

area_rod(2) = area_head(2) - rodrad(2)*rodrad(2)*pi;
area_rod(3) = area_head(3) - rodrad(3)*rodrad(3)*pi;
area_rod(4) = area_head(4) - rodrad(4)*rodrad(4)*pi;