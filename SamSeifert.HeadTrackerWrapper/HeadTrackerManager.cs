using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Linq;
using System.Text;

using HeadTrackerWrapper;

using OpenTK;

namespace SamSeifert.HeadTracker
{
    public delegate void HeadTrackEventHandler();
    public delegate void HeadTrackDataEventHandler(HeadTrackerManager.HeadTrackerData data);

    public static class HeadTrackerManager
    {
        public static event HeadTrackEventHandler ThreadStarted;
        public static event HeadTrackEventHandler TrackingStarted;
        public static event HeadTrackEventHandler TrackingEnded;
        public static event HeadTrackEventHandler ThreadEnded;
        public static event HeadTrackDataEventHandler GotHeadData;


        // Data comes of sensor in milimeters.
        // Data need to be milimeters
        public static bool _BoolShowViewOnStart = true;

        public static void Stop()
        {
            HeadTrackerManager._BoolKeepRunning = false;
        }

        public static void Start()
        {
            if (HeadTrackerManager._BoolActive) return;

            HeadTrackerManager._BoolKeepRunning = true;
            HeadTrackerManager._BoolActive = true;

            (new Thread(new ThreadStart(HeadTrackerManager.tsm))).Start();
        }

        public static void End()
        {
            if (HeadTrackerManager._HeadTrackerClass != null)
                HeadTrackerManager._HeadTrackerClass.end();
        }












        internal static float[] _DataReturn = new float[0];
        internal static uint[] _ImageParams = new uint[4];
        internal static Bitmap _Bitmap = null;
        internal static HeadTrackerClass _HeadTrackerClass = null;

        private static bool _BoolActive = false;
        internal static bool _BoolKeepRunning = false;

        public static bool IsTracking
        {
            get
            {
                return HeadTrackerManager._BoolActive;
            }
        }

        private unsafe static void tsm()
        {
            Thread.CurrentThread.Name = "Head Tracker Thread";

            if (HeadTrackerManager.ThreadStarted != null) HeadTrackerManager.ThreadStarted();

            Console.WriteLine("Start Head Tracking Thread");

            if (HeadTrackerManager._HeadTrackerClass == null)
            {
                HeadTrackerManager._HeadTrackerClass = new HeadTrackerClass();

                int w = 0;
                int h = 0;

                Console.WriteLine("Starting head tracker - Attempt");

                if (HeadTrackerManager._HeadTrackerClass.start(&w, &h) == 0)
                {
                    Console.WriteLine("Starting head tracker- Failed");
                    HeadTrackerManager._HeadTrackerClass.end();
                    HeadTrackerManager._HeadTrackerClass = null;
                }
                else
                {
                    Console.WriteLine("Starting head tracker - Success");

                    HeadTrackerManager._ImageParams[0] = (uint)w;
                    HeadTrackerManager._ImageParams[1] = (uint)h;

                    HeadTrackerManager._Bitmap = new Bitmap(w, h, PixelFormat.Format32bppRgb);

                    HeadTrackerManager._ImageParams[3] = (uint)Image.GetPixelFormatSize(HeadTrackerManager._Bitmap.PixelFormat);

                    BitmapData data = HeadTrackerManager._Bitmap.LockBits(
                        new Rectangle(Point.Empty, HeadTrackerManager._Bitmap.Size),
                        ImageLockMode.ReadWrite, HeadTrackerManager._Bitmap.PixelFormat);

                    HeadTrackerManager._ImageParams[2] = (uint)data.Stride;

                    HeadTrackerManager._Bitmap.UnlockBits(data);
                }
            }

            if (HeadTrackerManager._HeadTrackerClass != null)
            {
                if (HeadTrackerManager.TrackingStarted != null) HeadTrackerManager.TrackingStarted();

                if (HeadTrackerManager._BoolShowViewOnStart) new HeadTrackerForm().ShowDialog();

                while (HeadTrackerManager._BoolKeepRunning)
                {
                    Thread.Sleep(7);

                    int count = HeadTrackerManager._DataReturn.Length;
                    int update = 0;

                    fixed (float* data = HeadTrackerManager._DataReturn)
                    {
                        fixed (uint* parms = HeadTrackerManager._ImageParams)
                        {
                            update = HeadTrackerManager._HeadTrackerClass.update(data, &count);
                        }
                    }

                    HeadTrackerManager.update(update, count);
                }

                if (HeadTrackerManager.TrackingEnded != null) HeadTrackerManager.TrackingEnded();
            }

            Console.WriteLine("End Head Tracking");

            HeadTrackerManager._BoolActive = false;

            if (HeadTrackerManager.ThreadEnded != null) HeadTrackerManager.ThreadEnded();
        }

        public struct HeadTrackerData
        {
            public Vector3 Sensor1;
            public Vector3 Sensor2;
            public Vector3 Sensor3;
            public Vector3 Head;
            public Vector3 Eyes;
        }

        private static HeadTrackerData dat = new HeadTrackerData();

        public static Vector3 update(int update, int count)
        {
            if (update != 1) ;
            else if (count * 3 > HeadTrackerManager._DataReturn.Length)
            {
                HeadTrackerManager._DataReturn = new float[count * 3];
                Console.WriteLine("POINTS:" + count);
            }
            else if (count == 3)
            {

                dat.Sensor1 = new Vector3(_DataReturn[0], _DataReturn[1], _DataReturn[2]);
                dat.Sensor2 = new Vector3(_DataReturn[3], _DataReturn[4], _DataReturn[5]);
                dat.Sensor3 = new Vector3(_DataReturn[6], _DataReturn[7], _DataReturn[8]);

                Vector3 d1;
                Vector3 d2;

                Vector3.Subtract(ref dat.Sensor2, ref dat.Sensor1, out d1);
                Vector3.Subtract(ref dat.Sensor3, ref dat.Sensor1, out d2);

                Vector3.Cross(ref d1, ref d2, out dat.Head);

                dat.Head.Normalize();
                dat.Head = Vector3.Multiply(dat.Head, -75.0f);
                dat.Head.X += (dat.Sensor1.X * 2 + dat.Sensor2.X + dat.Sensor3.X) / 4;
                dat.Head.Y += (dat.Sensor1.Y * 2 + dat.Sensor2.Y + dat.Sensor3.Y) / 4;
                dat.Head.Z += (dat.Sensor1.Z * 2 + dat.Sensor2.Z + dat.Sensor3.Z) / 4;

                Vector3.Subtract(ref dat.Sensor2, ref dat.Sensor3, out dat.Eyes);
                dat.Eyes.Normalize();

                if (HeadTrackerManager.GotHeadData != null) HeadTrackerManager.GotHeadData(dat);

            }
            return dat.Sensor1;
        }
    }
}
