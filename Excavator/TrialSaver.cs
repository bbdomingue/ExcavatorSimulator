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
        private bool _BoolGood = true;
        private FileStream _FileStream1 = null;
        private FileStream _FileStream2 = null;
        private int _IntMilisToEnd;

        public static TrialSaver FromFile(String FileName, int minutes)
        {
            if (FileName == null) return null;
            else
            {
                var ret = new TrialSaver(FileName, minutes);
                if (ret._BoolGood) return ret;
                else return null;
            }
        }

        private TrialSaver(String FileName, int minutes)
        {
            this._FileStream1 = File.Create(FileName);
            this._FileStream2 = File.Create(FileName.Replace(".dat", ".bin"));

            this._IntMilisToEnd = 1000 * 60 * minutes;

            if (this._FileStream1 == null) this._BoolGood = false;
            else if (!this._FileStream1.CanWrite)  this._BoolGood = false;

            if (this._FileStream2 == null) this._BoolGood = false;
            else if (!this._FileStream2.CanWrite)  this._BoolGood = false;

            if (!this._BoolGood) this.Stop();
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
            [FieldOffset(a8)] public fixed float Fuel[2];      const int a9 = a8 + sizeof(float) * 4; // 18, 19, 20, 21
            public const int dl = a9 + sizeof(float); // 22
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

        public unsafe bool update1_ReturnContinue(File1_DataType dat)
        {
            if (this._FileStream1 == null) return true;

            for (int i = 0; i < File1_DataType.dl; i++) this._FileStream1.WriteByte(dat.data[i]);

            if (dat.TimeMilis > this._IntMilisToEnd)
            {
                this.Stop();
                return true;
            }
            else return false;
        }

        public unsafe void update2(File2_DataType dat)
        {
            if (this._FileStream2 == null) return;

            for (int i = 0; i < File2_DataType.dl; i++) this._FileStream2.WriteByte(dat.data[i]);
        }

        public void Stop()
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
        }
    }
}
