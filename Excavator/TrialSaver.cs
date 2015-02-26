using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Excavator
{
    public class TrialSaver
    {
        public static volatile float _TimeLeft = 0;

        private FileStream _FileStream1 = null;
        private FileStream _FileStream2 = null;
        private int _IntMilisToEnd;

        private static TrialSaver _TrialSaver = null;
        private static readonly object _TrialSaverLock = new object();

        private static String BaseFileName = null;
        private static int BaseTrialNumber = 0;

        private const int TrailLengthMinutes = 5;

        private int _TimeMilisStart = int.MaxValue;

        public static bool FromFile(String FileName)
        {
            BaseFileName = FileName;
            BaseTrialNumber = 0;

            Boolean b;
            var ret = new TrialSaver(out b);

            if (b)
            {
                lock (TrialSaver._TrialSaverLock)
                {
                    TrialSaver._TrialSaver = ret;
                }
            }

            return b;
        }

        private TrialSaver(out bool safe)
        {
            var fn = TrialSaver.direct();

            Directory.CreateDirectory(fn);

            this._FileStream1 = File.Create(Path.Combine(fn, "main.dat"));
            this._FileStream2 = File.Create(Path.Combine(fn, "main.bin"));

            this._IntMilisToEnd = 1000 * 60 * TrailLengthMinutes;

            safe = true;

            if (this._FileStream1 == null) safe = false;
            else if (!this._FileStream1.CanWrite) safe = false;

            if (this._FileStream2 == null) safe = false;
            else if (!this._FileStream2.CanWrite) safe = false;

            if (!safe) this.StopP();
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public unsafe struct File1_DataType
        {
            [FieldOffset(a0)] public fixed byte data[dl];      const int a0 = 0;
            [FieldOffset(a0)] public int TimeMilis;            const int a1 = a0 + sizeof(int); // 1
            [FieldOffset(a1)] public fixed float Q[4];         const int a2 = a1 + sizeof(float) * 4; // 2, 3, 4, 5
            [FieldOffset(a2)] public fixed float QD[4];        const int a3 = a2 + sizeof(float) * 4; // 6, 7, 8, 9
            [FieldOffset(a3)] public float BucketSoil;         const int a4 = a3 + sizeof(float); // 10
            [FieldOffset(a4)] public float BinSoilRight;       const int a5 = a4 + sizeof(float); // 11
            [FieldOffset(a5)] public float BinSoilLeft;        const int a6 = a5 + sizeof(float); // 12
            [FieldOffset(a6)] public float BinSoilNone;        const int a7 = a6 + sizeof(float); // 13
            [FieldOffset(a7)] public fixed float JoyStick[4];  const int a8 = a7 + sizeof(float) * 4; // 14, 15, 16, 17
            [FieldOffset(a8)] public fixed float Fuel[2];      const int a9 = a8 + sizeof(float) * 2; // 18, 19
            public const int dl = a9;
        };

        // Dump Time - Int Miliseconds
        // Dump Size - FLOAT
        // Dump X - FLOAT
        // Dump Y - FLOAT
        // Dump Z - FLOAT
        // In Bin - Int
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public unsafe struct File2_DataType
        {
            [FieldOffset(a0)] public fixed byte data[dl];   const int a0 = 0;
            [FieldOffset(a0)] public int TimeMilis;         const int a1 = a0 + sizeof(int);
            [FieldOffset(a1)] public float Size;            const int a2 = a1 + sizeof(float);
            [FieldOffset(a2)] public float dX;              const int a3 = a2 + sizeof(float);
            [FieldOffset(a3)] public float dY;              const int a4 = a3 + sizeof(float);
            [FieldOffset(a4)] public float dZ;              const int a5 = a4 + sizeof(float);
            [FieldOffset(a5)] public int inBin;             const int a6 = a5 + sizeof(float);
            public const int dl = a6;

            public const int inBinTrench = 0;
            public const int inBinLeft = 1;
            public const int inBinRight = 2;
            public const int inBinMiss = 3;
        }

        private static string direct()
        {
            return Path.Combine(new String[]{BaseFileName,
                "Trial_" + BaseTrialNumber.ToString()});
        }

        public static unsafe bool update1(File1_DataType dat)
        {
            lock (TrialSaver._TrialSaverLock)
            {
                if (TrialSaver._TrialSaver != null)
                {
                    TrialSaver._TrialSaver._TimeMilisStart = Math.Min(TrialSaver._TrialSaver._TimeMilisStart, dat.TimeMilis);
                    dat.TimeMilis -= TrialSaver._TrialSaver._TimeMilisStart;

                    for (int i = 0; i < File1_DataType.dl; i++) TrialSaver._TrialSaver._FileStream1.WriteByte(dat.data[i]);

                    TrialSaver._TimeLeft = (3 - BaseTrialNumber) * TrailLengthMinutes * 60 +
                        0.001f * (TrialSaver._TrialSaver._IntMilisToEnd - dat.TimeMilis);

                    if (dat.TimeMilis > TrialSaver._TrialSaver._IntMilisToEnd)
                    {
                        bool b;

                        TrialSaver._TrialSaver.StopP();

                        switch (BaseTrialNumber)
                        {
                            case 0:
                                Directory.Delete(TrialSaver.direct(), true);
                                BaseTrialNumber++;
                                Trial.ResetSoilModelS();
                                TrialSaver._TrialSaver = new TrialSaver(out b);
                                return true;
                            case 1:
                                BaseTrialNumber++;
                                Trial.ResetSoilModelS();
                                TrialSaver._TrialSaver = new TrialSaver(out b);
                                return true;
                            case 2:
                                BaseTrialNumber++;
                                Trial.ResetSoilModelS();
                                TrialSaver._TrialSaver = new TrialSaver(out b);
                                return true;
                        }
                    }
                }
                else
                {
                    TrialSaver._TimeLeft = -1;
                }
            }

            return false;
        }

        public static unsafe void update2(File2_DataType dat)
        {
            lock (TrialSaver._TrialSaverLock)
            {
                if (TrialSaver._TrialSaver != null)
                {
                    TrialSaver._TrialSaver._TimeMilisStart = Math.Min(TrialSaver._TrialSaver._TimeMilisStart, dat.TimeMilis);
                    dat.TimeMilis -= TrialSaver._TrialSaver._TimeMilisStart;
                    for (int i = 0; i < File2_DataType.dl; i++) TrialSaver._TrialSaver._FileStream2.WriteByte(dat.data[i]);
                }
            }
        }

        public static void Stop()
        {
            lock (TrialSaver._TrialSaverLock)
            {
                if (TrialSaver._TrialSaver != null)
                {
                    TrialSaver._TrialSaver.StopP();
                }
            }
        }

        private void StopP()
        {
            if (this._FileStream1 != null)
            {
                this._FileStream1.Close();
                this._FileStream1.Dispose();
                this._FileStream1 = null;
            }
            if (this._FileStream2 != null)
            {
                this._FileStream2.Close();
                this._FileStream2.Dispose();
                this._FileStream2 = null;
            }
            TrialSaver._TrialSaver = null;
        }
    }
}
