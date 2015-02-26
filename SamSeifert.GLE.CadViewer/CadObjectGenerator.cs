using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.ComplexFileParser;
using OpenTK;

namespace SamSeifert.GLE.CadViewer
{
    public static class CadObjectGenerator
    {
        /// <summary>
        /// No Color Support Yet
        /// </summary>
        /// <param name="FileText"></param>
        /// <param name="ObjectName"></param>
        /// <returns></returns>
        public static CadObject fromVRMLUnknownSource(String FileText, String ObjectName = "")
        {
            BracketFile bf = BracketFile.parseText(FileText);
            var points = new List<Vector3>();

            foreach (BracketFile f in bf._Children)
            {
                if (f.text.Equals("Coordinate3"))
                {
                    foreach (BracketFile f2 in f._Children)
                    {
                        if (f2.text.Equals("point"))
                        {
                            foreach (BracketFile f3 in f2._Children)
                            {
                                var ls = f3.text.Split(',');
                                foreach (var lsi in ls)
                                {
                                    var vxs = lsi.Trim().Split(' ');

                                    if (vxs.Length == 3)
                                    {
                                        double a, b, c;
                                        Double.TryParse(vxs[0], out a);
                                        Double.TryParse(vxs[1], out b);
                                        Double.TryParse(vxs[2], out c);

                                        points.Add(new Vector3((float)a, (float)b, (float)c));
                                    }
                                    else
                                    {
                                        MessageBox.Show("Vertex Series Length Not 3");
                                        return null;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var list = new List<Vector3>();

            foreach (BracketFile f in bf._Children)
            {
                if (f.text.Equals("IndexedFaceSet"))
                {
                    foreach (BracketFile f2 in f._Children)
                    {
                        if (f2.text.Equals("coordIndex"))
                        {
                            foreach (BracketFile f3 in f2._Children)
                            {
                                var ls = f3.text.Split(',');

                                if (ls.Length % 4 == 0)
                                {
                                    for (int i = 0; i < ls.Length; i += 4)
                                    {
                                        int a, b, c;
                                        int.TryParse(ls[i + 0], out a);
                                        int.TryParse(ls[i + 1], out b);
                                        int.TryParse(ls[i + 2], out c);

                                        int min = Math.Min(Math.Min(a, b), c);
                                        int max = Math.Max(Math.Max(a, b), c);

                                        if (min >= 0 && max < points.Count)
                                        {
                                            int[] iii = new int[] { a, b, c };

                                            foreach (int ii in iii)
                                            {
                                                list.Add(points[ii]);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Corner Number Out Of Range");
                                            return null;
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Corner Series Length Not 4");
                                    return null;
                                }
                            }
                        }
                    }
                }
            }

            var norms = new Vector3[] { };
            var verts = list.ToArray();

            if (verts.Length % 3 != 0)
            {
                MessageBox.Show("Vertex List Not Divisible By 3");
                return null;
            }

            var co = new CadObject();
            co._Name = ObjectName;
            co.setup(verts, norms);

            return co;
        }























        public static void TrianglesFromXAML(
            out Vector3[] vss,
            out int[] iss,
            String FileText,
            float xScale = 1.0f,
            float yScale = 1.0f,
            float zScale = 1.0f,
            float xOff = 0.0f,
            float yOff = 0.0f,
            float zOff = 0.0f)
        {
            var f = TagFile.parseText(FileText);
            String match0 = "ModelVisual3D.Children";
            String match1 = "GeometryModel3D";
            String match6 = "MeshGeometry3D";

            var match0L = f.getMatches(ref match0);
            foreach (var f0 in match0L)
            {
                var match1L = f0.getMatches(ref match1);
                foreach (var f1 in match1L)
                {
                    var match6L = f1.getMatches(ref match6);
                    if (match6L.Count == 1)
                    {
                        String ves, ins;

                        const String m2 = "Positions";
                        const String m3 = "TriangleIndices";

                        if (match6L[0]._Params.TryGetValue(m2, out ves) &&
                            match6L[0]._Params.TryGetValue(m3, out ins))
                        {
                            vss = CadObjectGenerator.XAMLparseStringD(ves).ToArray();

                            for (int j = 0; j < vss.Length; j++)
                            {
                                vss[j].X = vss[j].X * xScale + xOff;
                                vss[j].Y = vss[j].Y * yScale + yOff;
                                vss[j].Z = vss[j].Z * zScale + zOff;
                            }

                            var i = CadObjectGenerator.XAMLparseStringI(ins).ToArray();

                            int count = 0;
                            foreach (var trn in i) count += trn.Length;

                            iss = new int[count];

                            count = 0;

                            foreach (var trn in i)
                                foreach (var id in trn)
                                    iss[count++] = id;

                            return;
                        }


                    }
                }
            }

            vss = null;
            iss = null;
        }


        public static CadObject fromXAML(
            String FileText, 
            String ObjectName = "",
            float xScale = 1.0f,
            float yScale = 1.0f,
            float zScale = 1.0f,
            float xOff = 0.0f,
            float yOff = 0.0f,
            float zOff = 0.0f,
            bool useAmbient = true,
            bool useDiffuse = true,
            bool useSpecular = true,
            bool useEmission = true
            )
        {
            var f = TagFile.parseText(FileText);

            String match0 = "ModelVisual3D.Children";
            String match1 = "GeometryModel3D";
            String match2 = "DiffuseMaterial";
            String match3 = "SpecularMaterial";
            String match4 = "AmbientMaterial";
            String match5 = "EmissionMaterial";
            String matchX = "SolidColorBrush";
            String match6 = "MeshGeometry3D";

            const String SCOLOR = "Color";
            const String SOPAC = "Opacity";


            var ret = new List<CadObject>();

            var match0L = f.getMatches(ref match0);
            foreach (var f0 in match0L)
            {
                var list = new List<XAMLD>();

                var match1L = f0.getMatches(ref match1);
                foreach (var f1 in match1L)
                {
                    var match2L = f1.getMatches(ref match2, ref matchX);
                    var match3L = f1.getMatches(ref match3, ref matchX);
                    var match4L = f1.getMatches(ref match4, ref matchX);
                    var match5L = f1.getMatches(ref match5, ref matchX);
                    var match6L = f1.getMatches(ref match6);

                    String output;
                    Color diffuse = Color.Black;
                    Color specular = Color.Black;
                    Color ambient = Color.Black;
                    Color emission = Color.Black;

                    if (match2L.Count == 1 && useDiffuse)
                    {
                        var dict = match2L[0]._Params;
                        if (dict.TryGetValue(SCOLOR, out output))
                        {
                            Int32 iColorInt = Convert.ToInt32(output.Substring(1), 16);
                            diffuse = System.Drawing.Color.FromArgb(iColorInt);
                        }
                        if (dict.TryGetValue(SOPAC, out output))
                        {
                            Double a;
                            if (Double.TryParse(output, out a))
                            {
                                Byte alpha = (Byte)Math.Min(255, Math.Max(0, (a * 255)));
                                diffuse = Color.FromArgb(alpha, diffuse);
                            }
                        }
                    }
                    if (match3L.Count == 1 && useSpecular)
                    {
                        var dict = match3L[0]._Params;
                        if (dict.TryGetValue(SCOLOR, out output))
                        {
                            Int32 iColorInt = Convert.ToInt32(output.Substring(1), 16);
                            specular = System.Drawing.Color.FromArgb(iColorInt);
                        }
                        if (dict.TryGetValue(SOPAC, out output))
                        {
                            Double a;
                            if (Double.TryParse(output, out a))
                            {
                                Byte alpha = (Byte)Math.Min(255, Math.Max(0, (a * 255)));
                                specular = Color.FromArgb(alpha, specular);
                            }
                        }
                    }
                    if (match4L.Count == 1 && useAmbient)
                    {
                        var dict = match4L[0]._Params;
                        if (dict.TryGetValue(SCOLOR, out output))
                        {
                            Int32 iColorInt = Convert.ToInt32(output.Substring(1), 16);
                            ambient = System.Drawing.Color.FromArgb(iColorInt);
                        }
                        if (dict.TryGetValue(SOPAC, out output))
                        {
                            Double a;
                            if (Double.TryParse(output, out a))
                            {
                                Byte alpha = (Byte)Math.Min(255, Math.Max(0, (a * 255)));
                                ambient = Color.FromArgb(alpha, ambient);
                            }
                        }
                    }
                    if (match5L.Count == 1 && useEmission)
                    {
                        var dict = match5L[0]._Params;
                        if (dict.TryGetValue(SCOLOR, out output))
                        {
                            Int32 iColorInt = Convert.ToInt32(output.Substring(1), 16);
                            emission = System.Drawing.Color.FromArgb(iColorInt);
                        }
                        if (dict.TryGetValue(SOPAC, out output))
                        {
                            Double a;
                            if (Double.TryParse(output, out a))
                            {
                                Byte alpha = (Byte)Math.Min(255, Math.Max(0, (a * 255)));
                                emission = Color.FromArgb(alpha, emission);
                            }
                        }
                    }
                    if (match6L.Count == 1)
                    {
                        var temp = new XAMLD();
                        temp._Data = match6L[0]._Params;
                        temp._Ambient = ambient;
                        temp._Diffuse = diffuse;
                        temp._Emission = emission;
                        temp._Specular = specular;
                        temp._Covered = false;
                        list.Add(temp);
                    }
                }

                int count = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    var first = list[i];

                    if (!first._Covered)
                    {
                        first._Covered = true;

                        var verts = new List<Vector3>();
                        var norms = new List<Vector3>();

                        CadObjectGenerator.XAMLparseDict(ref first._Data, ref verts, ref norms);

                        for (int j = i + 1; j < list.Count; j++)
                        {
                            var second = list[j];

                            if (!second._Covered)
                            {
                                if (second._Diffuse.Equals(first._Diffuse) && second._Ambient.Equals(first._Ambient))
                                {
                                    if (second._Specular.Equals(first._Specular) && second._Emission.Equals(first._Emission))
                                    {
                                        CadObjectGenerator.XAMLparseDict(ref second._Data, ref verts, ref norms);
                                        second._Covered = true;
                                    }
                                }
                            }
                        }

                        var verts2array = verts.ToArray();
                        var norms2array = norms.ToArray();

                        for (int j = 0; j < verts.Count; j++)
                        {
                            verts2array[j].X *= xScale;
                            verts2array[j].Y *= yScale;
                            verts2array[j].Z *= zScale;
                            verts2array[j].X += xOff;
                            verts2array[j].Y += yOff;
                            verts2array[j].Z += zOff;
                        }

                        var co = new CadObject();
                        co.setup(verts2array, norms2array);
                        co._Diffuse[0] = first._Diffuse.R / 255.0f;
                        co._Diffuse[1] = first._Diffuse.G / 255.0f;
                        co._Diffuse[2] = first._Diffuse.B / 255.0f;
                        co._Diffuse[3] = first._Diffuse.A / 255.0f;
                        co._Specular[0] = first._Specular.R / 255.0f;
                        co._Specular[1] = first._Specular.G / 255.0f;
                        co._Specular[2] = first._Specular.B / 255.0f;
                        co._Specular[3] = first._Specular.A / 255.0f;
                        co._Ambient[0] = first._Ambient.R / 255.0f;
                        co._Ambient[1] = first._Ambient.G / 255.0f;
                        co._Ambient[2] = first._Ambient.B / 255.0f;
                        co._Ambient[3] = first._Ambient.A / 255.0f;
                        co._Emission[0] = first._Emission.R / 255.0f;
                        co._Emission[1] = first._Emission.G / 255.0f;
                        co._Emission[2] = first._Emission.B / 255.0f;
                        co._Emission[3] = first._Emission.A / 255.0f;

                        if (++count > 0) co._Name = ObjectName;
                        else co._Name = ObjectName + " (" + count + ")";
                        ret.Add(co);
                    }
                }
            }

            if (ret.Count == 0) return null;
            else if (ret.Count == 1) return ret[0];
            else
            {
                bool isgl4 = true;
                bool isgl3 = true;

                foreach (CadObject itr in ret)
                {
                    if (itr.type != CadObject.GLType.GL4) isgl4 = false;
                    if (itr.type != CadObject.GLType.GL3) isgl3 = false;
                }

                CadObject ret2 = new CadObject();

                if (isgl4) ret2._GLType = CadObject.GLType.GL4;
                else if (isgl3) ret2._GLType = CadObject.GLType.GL3;
                else ret2._GLType = CadObject.GLType.UNK;

                ret2._Name = ObjectName;
                ret2._CadObjects = ret.ToArray();

                Console.WriteLine(ret2._CadObjects.Length.ToString("00") + " CAD Objects in: " + ObjectName);
                return ret2;
            }
        }

        private static void XAMLparseDict(ref Dictionary<String, String> dict, ref List<Vector3> verts, ref List<Vector3> norms)
        {
            String nos, ves, ins;

            const String m1 = "Normals";
            const String m2 = "Positions";
            const String m3 = "TriangleIndices";

            if (dict.TryGetValue(m1, out nos) &&
                dict.TryGetValue(m2, out ves) &&
                dict.TryGetValue(m3, out ins))
            {
                var v = CadObjectGenerator.XAMLparseStringD(ves);
                var n = CadObjectGenerator.XAMLparseStringD(nos);

                if (v.Count == n.Count)
                {
                    var i = CadObjectGenerator.XAMLparseStringI(ins);

                    foreach (var trn in i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            verts.Add(v[trn[j]]);
                            norms.Add(n[trn[j]]);
                        }
                    }
                }
            }
        }

        static List<Vector3> XAMLparseStringD(String input)
        {
            var verts = input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var points = new List<Vector3>();

            foreach (var vline in verts)
            {
                var vxs = vline.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (vxs.Length == 3)
                {
                    double a, b, c;
                    Double.TryParse(vxs[0], out a);
                    Double.TryParse(vxs[1], out b);
                    Double.TryParse(vxs[2], out c);
                    points.Add(new Vector3((float)a, (float)b, (float)c));
                }
            }

            return points;
        }

        static List<int[]> XAMLparseStringI(String input)
        {
            var verts = input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var points = new List<int[]>();

            foreach (var vline in verts)
            {
                var vxs = vline.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (vxs.Length == 3)
                {
                    int a, b, c;
                    int.TryParse(vxs[0], out a);
                    int.TryParse(vxs[1], out b);
                    int.TryParse(vxs[2], out c);
                    points.Add(new int[] { a, b, c });
                }
            }

            return points;
        }

        private class XAMLD
        {
            public Dictionary<String, String> _Data;
            public Color _Ambient;
            public Color _Diffuse;
            public Color _Specular;
            public Color _Emission;
            public Boolean _Covered;
        }
















        private static Vector3[] parseFile(float yMult, String[] ss)
        {
            var list = new List<Vector3>();

            foreach (String line in ss)
            {
                String[] vxs = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (vxs.Length == 3)
                {
                    double a, b, c;
                    Double.TryParse(vxs[0], out a);
                    Double.TryParse(vxs[1], out b);
                    Double.TryParse(vxs[2], out c);

                    list.Add(new Vector3(
                        (float)a,
                        (float)b * yMult,
                        (float)c));
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// If possible, use the other overloaded method.  With string array
        /// </summary>
        /// <param name="FileText"></param>
        /// <param name="ObjectName"></param>
        /// <returns></returns>
        public static CadObject fromVertexList(String FileText, String ObjectName = "")
        {
            return CadObjectGenerator.fromVertexList(
                FileText.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries),
                ObjectName);
        }

        public static CadObject fromVertexList(String[] FileText, String ObjectName = "")
        {
            var verts = new Vector3[] { };

            try
            {
                verts = parseFile(1.0f, FileText);
            }
            catch
            {
                MessageBox.Show("Read Vertex File Error");
                return null;
            }

            if (verts.Length % 3 != 0)
            {
                MessageBox.Show("Vertex List Not Divisible By 3");
                return null;
            }

            var norms = new Vector3[] { };

            var co = new CadObject();
            co._Name = ObjectName;
            co.setup(verts, norms);

            return co;
        }


        /// <summary>
        /// If possible use the other overloaded method. With string array
        /// </summary>
        /// <param name="FileTextVertexs"></param>
        /// <param name="FileTextNormals"></param>
        /// <param name="ObjectName"></param>
        /// <returns></returns>
        public static CadObject fromVertexAndNormalList(
            String FileTextVertexs,
            String FileTextNormals,
            String ObjectName = "")
        {
            return CadObjectGenerator.fromVertexAndNormalList(
                FileTextVertexs.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries),
                FileTextNormals.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries),
                ObjectName);
        }

        public static CadObject fromVertexAndNormalList(
            String[] FileTextVertexs,
            String[] FileTextNormals,
            String ObjectName = "")
        {
            var verts = new Vector3[] { };
            var norms = new Vector3[] { };

            try
            {
                verts = parseFile(1.0f, FileTextVertexs);
            }
            catch
            {
                MessageBox.Show("Read Vertex File Error");
                return null;
            }

            try
            {
                norms = parseFile(1.0f, FileTextNormals);
            }
            catch
            {
                MessageBox.Show("Read Norms File Error");
                return null;
            }

            if (verts.Length != norms.Length)
            {
                MessageBox.Show("Unequal File Lengths");
                return null;
            }

            if (verts.Length % 3 != 0)
            {
                MessageBox.Show("Vertex List Not Divisible By 3");
                return null;
            }

            var co = new CadObject();
            co._Name = ObjectName;
            co.setup(verts, norms);

            return co;
        }





























        internal static CadObject fromVRML_Pro_E(string t, string name)
        {
            String s1 = "children";
            String s2 = "Shape";

            String s11 = "geometry IndexedFaceSet";
            String s12 = "coord USE FaceC normal USE FaceN ccw TRUE convex TRUE normalPerVertex TRUE solid FALSE coordIndex";

            var f0 = SamSeifert.ComplexFileParser.BracketFile.parseText(t);

            var match1 = f0.getMatches(ref s1, ref s2);

            BracketFile lastmatch = null;


            foreach (var it1 in match1)
            {
                var match2 = it1.getMatches(ref s11, ref s12);

                if (match2.Count > 0)
                {
                    lastmatch = it1;
                }
                else
                {
                    it1.display(false);
                }
            }

            Console.WriteLine("GOOD");
            Console.WriteLine("GOOD");
            Console.WriteLine("GOOD");
            Console.WriteLine("GOOD");
            Console.WriteLine("GOOD");

            lastmatch.display(true);

            return null;
        }

        public static CadObject gen(CadObject[] cadObject)
        {
            if (cadObject.Length == 0) return null;
            else if (cadObject.Length == 1) return cadObject[0];
            else
            {
                CadObject co = new CadObject();
                co._CadObjects = cadObject;
                return co;
            }
        }

























        public static CadObject CreateSphere(
            Vector3 offset,
            float radius,
            int color = int.MaxValue,
            int resolution = 6,
            String name = "Sphere"
            )
        {
            Vector3 p0, p1, p2, p3;

            int lx, ly;
            int x2, y2, x, y;

            Vector3[][] grid = getGridSphere(radius, out lx, out ly, resolution);

            List<Vector3> vs = new List<Vector3>();
            List<Vector3> ns = new List<Vector3>();

            for (y = 0, y2 = 1; y < ly - 1; y++, y2++)
            {
                for (x = 0; x < lx; x++)
                {
                    x2 = (x + 1) % lx;

                    p0 = grid[x][y];
                    p1 = grid[x][y2];
                    p2 = grid[x2][y2];
                    p3 = grid[x2][y];

                    vs.Add(Vector3.Add(p0, offset)); ns.Add(p0);
                    vs.Add(Vector3.Add(p2, offset)); ns.Add(p2);
                    vs.Add(Vector3.Add(p3, offset)); ns.Add(p3);

                    vs.Add(Vector3.Add(p2, offset)); ns.Add(p2);
                    vs.Add(Vector3.Add(p0, offset)); ns.Add(p0);
                    vs.Add(Vector3.Add(p1, offset)); ns.Add(p1);
                }
            }

            CadObject co = new CadObject(vs.ToArray(), ns.ToArray(), name);

            co.setColor(Color.FromArgb(color));

            return co;

        }

        private static double toRadiansD(double inp) { return inp * Math.PI / 180; }

        private static Vector3[][] getGridSphere(double radius, out int thetaCount, out int phiCount, int resolution)
        {
            thetaCount = resolution * 2;
            int thetaInc = 360 / thetaCount;
            int theta;

            phiCount = resolution;
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



        public static CadObject CreateFace(
            Vector3 v1,
            Vector3 v2,
            Vector3 v3,
            Vector3 v4,
            Vector3 n,
            String name = "Plane")
        {
            List<Vector3> vs = new List<Vector3>();
            List<Vector3> ns = new List<Vector3>();

            vs.Add(v1); ns.Add(n);
            vs.Add(v2); ns.Add(n);
            vs.Add(v3); ns.Add(n);

            vs.Add(v1); ns.Add(n);
            vs.Add(v3); ns.Add(n);
            vs.Add(v4); ns.Add(n);

            return new CadObject(vs.ToArray(), ns.ToArray(), name);
        }
    }
}
