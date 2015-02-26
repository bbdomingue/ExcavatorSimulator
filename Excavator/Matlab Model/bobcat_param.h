#include <math.h>

#ifndef __BOBCAT_PARAM_HEADER_FILE_DEFINITION
#define __BOBCAT_PARAM_HEADER_FILE_DEFINITION

#define a1  31.5423
#define a1b  5.60224
#define a2  104.0
#define a3  54.0
#define a4  34.6133//34.6593
#define d1  (-1.68)

#define q2 (-pi/2 + acos(d1/a1))

#define r01  a1
#define r12  a2
#define r23  a3
#define r34  a4

#define r1A  14.8486
#define r1B  54.4949
#define r1C  69.6644
#define r2B  54.4949
#define r2C  53.1519
#define r2D  14.8011
#define r2E  13.5656
#define r3D  66.3633
#define r3E  49.6927
#define r3F  7.7154
#define r3G  8.65592
#define rEF  40.8296
#define rFH  10.9881
#define rFI  0.576462
#define r3I  r3F 
#define rGH  12.3942 //12.42
#define r4G  35.9042 //35.9193

#define ro1  6.76942
#define roA  15.6884
#define to1A  acos((ro1*ro1+r1A*r1A-roA*roA)/(2*ro1*r1A))
//r0A  15.6884 
#define t01o  .57//acos(3.5893/ro1)

 
#define t01A  (t01o + to1A) //acos((r01*r01+r1A*r1A-r0A*r0A)/(2*r01*r1A))

#define t12C  acos((r12*r12+r2C*r2C-r1C*r1C)/(2*r12*r2C))
#define t23D  acos((r23*r23+r3D*r3D-r2D*r2D)/(2*r23*r3D))
#define tB12  acos((r1B*r1B+r12*r12-r2B*r2B)/(2*r1B*r12))
#define tD23  acos((r2D*r2D+r23*r23-r3D*r3D)/(2*r2D*r23))
#define tG34  acos((r3G*r3G+r34*r34-r4G*r4G)/(2*r3G*r34))
#define t23E  acos((r23*r23+r3E*r3E-r2E*r2E)/(2*r23*r3E))
#define pi  3.1415926535897931
#define alfa (16.6878 * pi/180)

#define t1min  (-400*pi/180)
#define t1max  (400*pi/180)
#define Ymin2  40.5807//42.1822//      
#define Ymax2  67.3780//58.8380//44.625
#define stroke2 (Ymax2 - Ymin2) //26.72
#define t2max   (66*pi/180)
#define t2min   (-63*pi/180)
#define Ymin3   39.8290//27.25
#define Ymax3   65.8375//44.625
#define stroke3 (Ymax3 - Ymin3)     //26.0
#define t3max   (-22*pi/180)
#define t3min   (-145*pi/180) //-2.188768083034109
 
//dmax (a2 + a3*cos(t3max)) //dmax 85.197418048124192
#define dmax  sqrt(a3*sin(t3max)*a3*sin(t3max) + (a2+a3*cos(t3max))*(a2+a3*cos(t3max)))
#define dmin  sqrt((a2 + a3*cos(t3min))*(a2 + a3*cos(t3min)) + a3*sin(t3min)*a3*sin(t3min)) //(a2 + a3*cos(t3min)) //40.606568032136707
 
#define t312min  atan((a3 * (-sin(t3max)))/(a2 + a3*cos(t3max))) 
#define t312max  (27.5*pi/180)   //.5460 //0.88914881238211452
#define Ymin4  30.2084    //23.375
#define Ymax4  49.9174//49.9009
#define stroke4  (Ymax4 - Ymin4)     //20.63
#define t4max  (33*pi/180)  //0.13725870294037668
#define t4min  (-154*pi/180) //-2.2578906226452915
//rmin  59.2258 //(a1 + a2*cos(t2max)+ a3*cos(t2max + t3min))//20
//t2min_rmin_t3min =0 //-0.39451279633262937
//t3min_rmin_t2min -150*pi/180 //-1.2103510689803758

// zmax_rmin_t2min (a2*sin(t2min) + a3*sin(t2min+t3min))
// zmin_rmin_t3min (a2*sin(t2min) + a3*sin(t2min+t3max)) 

#define zlowmin (a2*sin(t2min) + a3*sin(t2min+t3min))
#define zlowmax (a2*sin(t2min) + a3*sin(t2min+t3max)) 
#define zhighmin (a2*sin(t2max) + a3*sin(t2max+t3min))
#define zhighmax (a2*sin(t2max) + a3*sin(t2max+t3max))
#define rhighpt  (a2*cos(t2max) + a3*cos(t2max+t3max))
#define rlowpt  (a2*cos(t2min) + a3*cos(t2min+t3max))

// ztop =a2*sin(t2max) + a3*sin(t2max+t3max)
// zbottom =a2*sin(t2min) + a3*sin(t2min+t3min)
// rtop =a1 + a2*cos(t2max) + a3*cos(t2max+t3max)
// rbottom =a1 + a2*cos(t2min) + a3*cos(t2min+t3min)

#define bore2  3.75 //in
#define bore3  3.25
#define bore4  3.0

#define roddiam2  2   //in
#define roddiam3  2
#define roddiam4  1.75

#define borerad2  (bore2/2) //in
#define borerad3  (bore3/2)
#define borerad4  (bore4/2)

#define rodrad2  (roddiam2/2)   //in
#define rodrad3  (roddiam3/2)
#define rodrad4  (roddiam4/2)

#define area_head2  (borerad2*borerad2*pi) //in^2
#define area_head3  (borerad3*borerad3*pi)
#define area_head4  (borerad4*borerad4*pi)

#define area_rod2  (area_head2 - rodrad2*rodrad2*pi) //in^2
#define area_rod3  (area_head3 - rodrad3*rodrad3*pi)
#define area_rod4  (area_head4 - rodrad4*rodrad4*pi)

#define gear_ratio  (91/17)
#define Vm  820e-6    // [m^3/rev] swing motor displacement
//1 cubic meter  61 023.7441 cubic inches
#define m3_2_in3  61023.7441
#define motor_disp_per_rev  (Vm*m3_2_in3)

//1 cc  0.0610237441 cubic inches
#define cc_2_in3  0.0610237441
#define maxflow_per_rev  (18*cc_2_in3) //18cc/rev
#define motorspeed  (3442/60) //rpm -> rps
#define maxpumpflow  (maxflow_per_rev*motorspeed)


//1 pound per square inch  6 894.75729 pascals
#define psi_2_pa  6894.75729
//1 newton meter  0.737562149 foot pound
#define Nm_2_ftlb  0.737562149
//1 inches  0.0254 meters
#define in_2_m  0.0254
//1 newton  0.224808943 pounds force
#define N_2_lb  0.224808943

//#define m3_2_in3  1/((in_2_m)*(in_2_m)*(in_2_m))

// //OFFSET//
// #define VO_A  (3e-4 * m3_2_in3) //[m^3] volume of piston-side line
// #define VO_B  (3e-4 * m3_2_in3) // [m^3] volume of rod-side line
// #define KLi_O  (6e-13 * m3_2_in3 * psi_2_pa)// [m^3/s/Pa] Coefficient of internal leakage
// //BOOM//
// #define linevol_head2  (20e-4 * m3_2_in3) //[m^3] volume of piston-side line
// #define linevol_rod2  (30e-4 * m3_2_in3) //[m^3] volume of rod-side line
// #define leak2  (6e-13 * m3_2_in3 * psi_2_pa) //[m^3/s/Pa] Coefficient of internal leakage
// //ARM//
// #define linevol_head3  (15e-4 * m3_2_in3) // [m^3] volume of piston-side line
// #define linevol_rod3  (20e-4 * m3_2_in3) // [m^3] volume of rod-side line
// #define leak3  (6e-13 * m3_2_in3 * psi_2_pa) // [m^3/s/Pa] Coefficient of internal leakage
// //BUCKET//
// #define linevol_head4  30e-4 * m3_2_in3 // [m^3] volume of piston-side line
// #define linevol_rod4  35e-4 * m3_2_in3 // [m^3] volume of rod-side line
// #define leak4  6e-13 * m3_2_in3 * psi_2_pa // [m^3/s/Pa] Coefficient of internal leakage
// //SLEW//
// #define linevol_head1  (15e-4 * m3_2_in3) // [m^3] volume of piston-side line
// #define linevol_rod1  (15e-4 * m3_2_in3) // [m^3] volume of rod-side line
// #define leak1  (5e-13 * m3_2_in3) // [m^3/s/Pa] Coefficient of internal leakage
// #define KLi_ext_S  (9e-13 * m3_2_in3 * psi_2_pa) // [m^3/s/Pa] Coefficient of external leakage

//1 liters per hour  0.01695104 (cubic inches) per sec
// #define lph_2_in3ps  0.01695104
// #define orificek  (5000*0.01695104/1e3*sqrt(psi_2_pa))

#endif // __BOBCAT_PARAM_HEADER_FILE_DEFINITION
