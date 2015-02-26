%clear all
%close all
%clc

matfromfile = csvread('fuelmap_artificial_pts.csv',6,0);
%matfromfile = csvread('c:\work\excavator\purdue\fuelmap.csv',6,0);

engspeed = matfromfile(:,3);
torque = matfromfile(:,4);
fuelrate = matfromfile(:,13);
throttle = matfromfile(:,2);

engspeeddiv = 4;        % # of divisions between each engine speed in the data
divisions = 50;         % # of divisions betweeen max and min torq

engspeeddiff = engspeed(2)-engspeed(1);     % engine speed step size

% create vector of engine speeds 
n = 1;
while (engspeed(n) < engspeed(n+1))
    for m = 1:engspeeddiv
        standengspeed(engspeeddiv*(n-1)+ m) = engspeed(n)+(engspeed(n+1)-engspeed(n))/engspeeddiv*(m-1);
    end
    n = n+1;
end
standengspeed(engspeeddiv*(n-1)+1) = engspeed(n);
numengspeeds = n;

% calculate the number of different throttle openings
numthrottle = 1;
for n = 1:length(throttle)-1
    if ( throttle(n) ~= throttle(n+1) )
        numthrottle = numthrottle + 1;
    end
end

fuelratemat = zeros(numthrottle,numengspeeds);
engspeedmat = zeros(numthrottle,numengspeeds);
torquemat = zeros(numthrottle,numengspeeds);

% read in data from purdue of each engine speed and throttle setting
for n = 1:numengspeeds
    for m = 0:numthrottle-1
        fuelratemat(m+1,n) = fuelrate(m*numengspeeds+n);
        engspeedmat(m+1,n) = engspeed(m*numengspeeds+n);
        torquemat(m+1,n) = torque(m*numengspeeds+n);        
    end
end

maxtorq = max(torquemat);
maxmaxtorq = max(maxtorq);
mintorq = min(torquemat);
minmintorq = min(mintorq);

torqdiv = (maxmaxtorq-minmintorq)/divisions;        % torque step size

% create torque scale vector
standardtorq = zeros(divisions,1);
for n = 0:divisions
    standardtorq(n+1) = minmintorq + n*torqdiv;
end

newmaxtorq = 169;
stretch = newmaxtorq/maxmaxtorq;
newstandardtorq = standardtorq*stretch;

% %straight interpolation
% standardfuelmap = zeros(length(engspeedmat),divisions+1);
% 
% for m = 0:divisions
%     for n = 1:length(engspeedmat)
%         p = 1;
%         if (min(torquemat(:,n)) > standardtorq(m+1)) %less than min
%             standardfuelmap(n,m+1) = NaN;
%         else
%             if (max(torquemat(:,n)) < standardtorq(m+1)) %more than max
%                 standardfuelmap(n,m+1) = NaN;
%             else            
%             while ( (torquemat(p,n) <= standardtorq(m+1)) )% && (p < length(fuelratemat)))
%                 p = p+1;
%                 if (p > 4)
%                     p = 4;
%                    break;
%                 end
%             end
%             standardfuelmap(n,m+1) = fuelratemat(p-1,n) + ...
%                 (fuelratemat(p,n)-fuelratemat(p-1,n))/(torquemat(p,n)-torquemat(p-1,n)) *...
%                 (standardtorq(m+1)-torquemat(p-1,n));            
%             end
%         end
%     end
% end
% 
% surf(engspeed(1:13),standardtorq',standardfuelmap')
% title('fuel consumption')
% xlabel('engine speed')
% ylabel('torque')

standardfuelmap = zeros(length(standengspeed),divisions+1)*NaN;
%size(standardfuelmap)

for m = 0:divisions
    for n = 1:length(engspeedmat)
        p = 1;
        if (min(torquemat(:,n)) > standardtorq(m+1)) %less than min
            %standardfuelmap(2*n-1,m+1) = NaN;
        else
            if (max(torquemat(:,n)) < standardtorq(m+1)) %more than max
                %standardfuelmap(2*n-1,m+1) = NaN;
            else            
                while (torquemat(p,n) <= standardtorq(m+1))
                    p = p+1;
                    if (p > numthrottle)
                        p = numthrottle;
                       break;
                    end
                end
                standardfuelmap(engspeeddiv*(n-1)+1,m+1) = fuelratemat(p-1,n) + ...
                    (fuelratemat(p,n)-fuelratemat(p-1,n))/(torquemat(p,n)-torquemat(p-1,n)) *...
                    (standardtorq(m+1)-torquemat(p-1,n));  
                if (n > 1)
                    x = 2;
                    for x = 2:engspeeddiv
                        if (isnan(standardfuelmap(engspeeddiv*(n-2)+1,m+1)) )
                            %standardfuelmap(engspeeddiv*(n-2)+x,m+1) = NaN;
                        else
                            standardfuelmap(engspeeddiv*(n-2)+x,m+1) = ...
                                (engspeeddiv - (x-1))/engspeeddiv * standardfuelmap(engspeeddiv*(n-2)+1,m+1) +...
                                (x-1)/engspeeddiv * standardfuelmap(engspeeddiv*(n-1)+1,m+1);
                        end
                    end
                end
            end
        end            
    end
end

% find out the first entry to have 'NaN' in each column
colh = ones(1,numengspeeds)*length(standardtorq);
for n = 1:numengspeeds
    for m = 1:length(standardtorq)
        if ( isnan(standardfuelmap(engspeeddiv*(n-1)+1,m)) )
            colh(n) = m;
            break;
        end
    end
end

% interpolate on the lefthand side
for n = 1:numengspeeds-1
    slope = (colh(n+1)-colh(n))*torqdiv/engspeeddiff;
    for m = engspeeddiv-1:-1:1
        linept = slope*(m*engspeeddiff/engspeeddiv);
        if (slope > 0 )
            boxpt = torqdiv;
            p = 0;
            while (boxpt < linept)   
                standardfuelmap(engspeeddiv*(n-1)+1+m,colh(n)+p) = standardfuelmap(engspeeddiv*(n-1)+1,colh(n)-1)+...
                    (standardfuelmap(engspeeddiv*(n-1)+1,colh(n)-1)-standardfuelmap(engspeeddiv*(n-1)+1,colh(n)-2))*(p+1);

                slopeh = (standardfuelmap(engspeeddiv*(n-1)+1+m+2,colh(n)+p) - standardfuelmap(engspeeddiv*(n-1)+1+m+1,colh(n)+p))/(engspeeddiff/engspeeddiv);
                slopev = (standardfuelmap(engspeeddiv*(n-1)+1+m,colh(n)+p-2) - standardfuelmap(engspeeddiv*(n-1)+1+m,colh(n)+p-1))/(torqdiv);
                standardfuelmap(engspeeddiv*(n-1)+1+m,colh(n)+p) = standardfuelmap(engspeeddiv*(n-1)+2,colh(n)-1)+...
                    (slopev*(-torqdiv) + slopeh*(-engspeeddiff/engspeeddiv))/2;

                standardfuelmap(engspeeddiv*(n-1)+1+m,colh(n)+p) = ...
                    (standardfuelmap(engspeeddiv*(n-1)+1+m,colh(n)+p-1)+slopev*(-torqdiv) + ...
                    standardfuelmap(engspeeddiv*(n-1)+1+m+1,colh(n)+p)+slopeh*(-engspeeddiff/engspeeddiv))/2;

                boxpt = boxpt + torqdiv;
                p = p+1;
            end
        end
    end
end

% interpolate on the righthand side
for n = 1:numengspeeds-1
    slope = (colh(n+1)-colh(n))*torqdiv/engspeeddiff;
    for m = 1:engspeeddiv-1
        linept = slope*(m*engspeeddiff/engspeeddiv);
        if (slope < 0 )
            boxpt = standardtorq(colh(n+1))-standardtorq(colh(n));
            p = 0;
            while (boxpt < linept)   

                slopeh = (-standardfuelmap(engspeeddiv*(n-1)+1+m-2,colh(n+1)+p) + standardfuelmap(engspeeddiv*(n-1)+1+m-1,colh(n+1)+p))/(engspeeddiff/engspeeddiv);
                slopev = (-standardfuelmap(engspeeddiv*(n-1)+1+m,colh(n+1)+p-2) + standardfuelmap(engspeeddiv*(n-1)+1+m,colh(n+1)+p-1))/(torqdiv);

                standardfuelmap(engspeeddiv*(n-1)+1+m,colh(n+1)+p) = ...
                    (standardfuelmap(engspeeddiv*(n-1)+1+m,colh(n+1)+p-1)+slopev*(torqdiv) + ...
                    standardfuelmap(engspeeddiv*(n-1)+1+m-1,colh(n+1)+p)+slopeh*(engspeeddiff/engspeeddiv))/2;
                
                boxpt = boxpt + torqdiv;
                p = p+1;
            end
        end
    end
end

% %standardfuelmap(size(standardfuelmap)) = 1;
% surf(standengspeed,standardtorq',standardfuelmap')
% title('fuel consumption')
% xlabel('engine speed')
% ylabel('torque')
% axis([engspeed(1) engspeed(numengspeeds) minmintorq maxmaxtorq])
% 
% %stretched graph
% figure
% surf(standengspeed,newstandardtorq',standardfuelmap')
% title('fuel consumption')
% xlabel('engine speed')
% ylabel('torque')
% axis([engspeed(1) engspeed(numengspeeds) min(newstandardtorq) max(newstandardtorq)])


% engspeedpt = 2633;
% torqpt = 75;
% 
% d_engspd = standengspeed(2)-standengspeed(1);
% d_torq = newstandardtorq(2)-newstandardtorq(1);
% 
% if (engspeedpt < standengspeed(1))
%     spdindex = 1;
% else
%     if (engspeedpt > standengspeed(length(standengspeed)))
%         spdindex = length(standengspeed);
%     else
%         spdindex = floor( (engspeedpt-standengspeed(1))/d_engspd ) + 1;
%     end
% end
% 
% if (torqpt < newstandardtorq(1))
%     torqindex = 1;
% else
%     if (torqpt > newstandardtorq(length(newstandardtorq)))
%         torqindex = length(newstandardtorq);
%     else
%         torqindex = floor( (torqpt-newstandardtorq(1))/d_torq ) + 1;
%     end
% end
% 
% % [newstandardtorq(torqindex), torqpt, newstandardtorq(torqindex+1)]
% % [standengspeed(spdindex), engspeedpt, standengspeed(spdindex+1)]
% 
% if ( isnan(standardfuelmap(spdindex+1,torqindex)) )
%     spdslope = (standardfuelmap(spdindex,torqindex) - standardfuelmap(spdindex-1,torqindex))/d_engspd;
% else
%     spdslope = (standardfuelmap(spdindex+1,torqindex) - standardfuelmap(spdindex,torqindex))/d_engspd;
% end
% 
% if ( isnan(standardfuelmap(spdindex,torqindex+1)) )
%     torqslope = (standardfuelmap(spdindex,torqindex)- standardfuelmap(spdindex,torqindex-1))/d_torq;
% else
%     torqslope = (standardfuelmap(spdindex,torqindex+1)- standardfuelmap(spdindex,torqindex))/d_torq;
% end
% 
% spdincrease = spdslope * (engspeedpt - standengspeed(spdindex));
% torqincrease = torqslope * (torqpt - newstandardtorq(torqindex));
% 
% fueluse = standardfuelmap(spdindex,torqindex) + spdincrease + torqincrease;
% 
% [standardfuelmap(spdindex,torqindex+1), standardfuelmap(spdindex+1,torqindex+1);
%     standardfuelmap(spdindex,torqindex),standardfuelmap(spdindex+1,torqindex)]


% find out the first entry to have 'NaN' in each column
maxincol = ones(1,(numengspeeds-1)*engspeeddiv+1)*length(standardtorq);   %initialize to max
for n = 1:length(maxincol)
    for m = 1:length(standardtorq)
        if ( isnan(standardfuelmap(n,m)) )
            maxincol(n) = m - 1;
            break;
        end
    end
end

