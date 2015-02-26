using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Excavator
{
    internal static class GLSphere
    {
        static int list = 0;

        internal static void drawSphere(float x, float y, float z, float radius)
        {
            if (list == 0)
            {
                list = GL.GenLists(1);

                GL.NewList(list, ListMode.Compile);

                createSphere(1.0f);

                GL.EndList();
            }

            GL.Translate(x, y, z);
            GL.Scale(radius, radius, radius);
            GL.CallList(list);
            GL.Scale(1 / radius, 1 / radius, 1 / radius);
            GL.Translate(-x, -y, -z);
        }

        private static void createSphere(float radius)
        {
            Vector3 p0, p1, p2, p3;

            int lx, ly;
            int x2, y2, x, y;

            Vector3[][] grid = getGrid(radius, out lx, out ly);

            GL.Begin(BeginMode.Triangles);
            {
                for (y = 0, y2 = 1; y < ly - 1; y++, y2++)
                {
                    for (x = 0; x < lx; x++)
                    {
                        x2 = (x + 1) % lx;

                        p0 = grid[x][y];
                        p1 = grid[x][y2];
                        p2 = grid[x2][y2];
                        p3 = grid[x2][y];

                        GL.Normal3(p0);
                        GL.Vertex3(p0);
                        GL.Normal3(p2);
                        GL.Vertex3(p2);
                        GL.Normal3(p3);
                        GL.Vertex3(p3);


                        GL.Normal3(p2); GL.Vertex3(p2);
                        GL.Normal3(p0); GL.Vertex3(p0);
                        GL.Normal3(p1); GL.Vertex3(p1);
                    }
                }
            }
            GL.End();
        }

        private static double toRadiansD(double inp) { return inp * Math.PI / 180; }

        private static Vector3[][] getGrid(double radius, out int thetaCount, out int phiCount)
        {
            thetaCount = 12;
            int thetaInc = 360 / thetaCount;
            int theta;

            phiCount = 6;
            float phiInc = 180.0f / phiCount;
            float phi;

            phiCount++;

            Vector3[][] grid = new Vector3[thetaCount][];

            int x, y;
            for (x = 0, theta = 0; x < thetaCount; x++, theta += thetaInc)
            {
                grid[x] = new Vector3[phiCount];

                for (y = 0, phi = 0; y < phiCount; y++, phi += phiInc)
                {
                    double vY = Math.Cos(toRadiansD(phi));
                    double planarDistance = Math.Sin(toRadiansD(phi));

                    double vZ = Math.Cos(toRadiansD(theta));
                    double vX = Math.Sin(toRadiansD(theta));


                    grid[x][y] = new Vector3(
                        (float)(vX * radius * planarDistance),
                        (float)(vY * radius),
                        (float)(vZ * radius * planarDistance));

                }
            }

            return grid;
        }
    }
}
