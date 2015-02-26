using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;

namespace SamSeifert.GLE.CadViewer
{
    internal class StaticMethods
    {
        private struct SorterStruct
        {
            internal int v1, v2, v3, n1, n2, n3;
        }

        internal static bool ConsolidateData(
            Vector3[] verts, Vector3[] norms,
            out uint count, out Vector3[] vout, out Vector3[] nout, out uint[] iout)
        {
            if (verts.Length != norms.Length)
            {
                vout = null;
                nout = null;
                iout = null;
                count = 0;
                return false;
            }
            else
            {
                var m1a = new List<Vector3>();
                var m2a = new List<Vector3>();
                var m3a = new List<uint>();

                var dict = new Dictionary<SorterStruct, uint>();
                uint dex;
                count = 0;
                var sort = new SorterStruct();

                for (int i = 0; i < verts.Length; i++)
                {
                    var v = verts[i];
                    var n = norms[i];

                    const int multiplier = 10000; // Minimum, groups similar numbers together

                    sort.v1 = (int)(v.X * multiplier);
                    sort.v2 = (int)(v.Y * multiplier);
                    sort.v3 = (int)(v.Z * multiplier);

                    sort.n1 = (int)(n.X * multiplier);
                    sort.n2 = (int)(n.Y * multiplier);
                    sort.n3 = (int)(n.Z * multiplier);

                    if (!dict.TryGetValue(sort, out dex))
                    {
                        dex = count;
                        m1a.Add(new Vector3(v.X, v.Y, v.Z));
                        m2a.Add(new Vector3(n.X, n.Y, n.Z));
                        dict.Add(sort, dex);
                        count++;
                    }

                    m3a.Add(dex);
                }

                vout = m1a.ToArray();
                nout = m2a.ToArray();
                iout = m3a.ToArray();

                return true;
            }
        }

        internal static bool ExpandData(
            Vector3[] verts, Vector3[] norms, uint[] indices,
            out uint count, out Vector3[] vout, out Vector3[] nout)
        {
            if (verts.Length != norms.Length)
            {
                vout = null;
                nout = null;
                count = 0;
                return false;
            }
            else
            {
                var voutL = new List<Vector3>();
                var noutL = new List<Vector3>();

                count = 0;

                foreach (int dex in indices)
                {
                    voutL.Add(verts[dex]);
                    noutL.Add(norms[dex]);
                    count++;
                }

                vout = voutL.ToArray();
                nout = noutL.ToArray();
                return true;
            }
        }
    }
}
