using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Excavator
{
    public delegate void CabRotaterEventHandler();

    public class CabRotater
    {
        public static volatile float CabDegrees = -100;

        private static readonly float CabAmplitude = StaticMethods.toRadiansF(65);

        private static bool _BoolReset = false;
        private static int _DateTime = 0;

        private static object LockOb = new object();
        private static float _Cab_Q = 0;
        private static float _Cab_Qd = 0;
        private static float _Cab_Kp = 0;
        private static float _Cab_Kd = 0;

        /// <summary>
        /// Thread Save!
        /// Use Radians FOOL
        /// </summary>
        /// <param name="Q"></param>
        /// <param name="Qd"></param>
        public static void setAngleValues(float Q, float Qd)
        {
            lock (CabRotater.LockOb)
            {
                CabRotater._Cab_Q = Q;
                CabRotater._Cab_Qd = Qd;
            }
        }

        public static void ResetFor30Seconds()
        {
            lock (CabRotater.LockOb)
            {
                CabRotater._DateTime = Environment.TickCount;
                CabRotater._BoolReset = true;
            }
        }




        /// <summary>
        /// Thread Save!
        /// </summary>
        /// <param name="Kp"></param>
        /// <param name="Kd"></param>
        public static void setGainValues(float Kp, float Kd)
        {
            Console.WriteLine("Cab Kp: " + Kp + ", Cab Kd: " + Kd);
            lock (CabRotater.LockOb)
            {
                CabRotater._Cab_Kp = Kp;
                CabRotater._Cab_Kd = Kd;
            }
        }

        const int HOST_SEND_CAB_INP = 42101; // Data leaves this port on here to
        const int XPC_PORT_CAB_INP = 26451;  // get to this port on target

        const int HOST_RECIEVE_CAB_INP = 25521;

        private static volatile bool _BoolAlive = false;
        private static volatile int _IntCount = 0;

        public static event CabRotaterEventHandler RotateStarted;
        public static event CabRotaterEventHandler RotateEnded;

        // Main Thread
        public static void Kill()
        {
            CabRotater._BoolAlive = false;
        }

        // Main Thread
        public static void Start()
        {
            if (!CabRotater._BoolAlive && (CabRotater._IntCount == 0))
            {
                CabRotater._IntCount++;
                CabRotater._BoolAlive = true;

                (new Thread(new ThreadStart(CabRotater.tsm))).Start();
                (new Thread(new ThreadStart(CabRotater.tsr))).Start();
            }
        }


        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public unsafe struct CabDataSentToExcavator
        {
            [FieldOffset(0)] public fixed byte data[40];
            [FieldOffset(0)] public double Cab_Q;
            [FieldOffset(8)] public double Cab_Qd;
            [FieldOffset(16)] public double Kill;
            [FieldOffset(24)] public double Kp;
            [FieldOffset(32)] public double Kd;
        };

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public unsafe struct CabDataFromExcavator
        {
            [FieldOffset(0)]
            public fixed byte data[8];
            [FieldOffset(0)]
            public double Cab_Q;
        };


        private static unsafe void tsm()
        {
            Thread.CurrentThread.Name = "Send Cab";

            if (CabRotater.RotateStarted != null) CabRotater.RotateStarted();
            FormBase.addThread();

            Console.WriteLine("Start Send Cab");

            int size = sizeof(CabDataSentToExcavator);
            var dataStruct = new CabDataSentToExcavator()
            {
                Kill = 2,
                Kp = 0,
                Kd = 0
            };

            Byte[] dataBytes = new byte[size];

            int i;

            try
            {
                UdpClient _UdpClientSend = new UdpClient(HOST_SEND_CAB_INP);
                _UdpClientSend.Connect(IPAddress.Parse(XPC.XPC_IP), XPC_PORT_CAB_INP);

                while (FormBase._BoolThreadAlive && CabRotater._BoolAlive)
                {
                    lock (CabRotater.LockOb)
                    {
                        if (CabRotater._BoolReset)
                        {
                            dataStruct.Cab_Q = 0;
                            dataStruct.Cab_Qd = 0;
                            dataStruct.Kp = 6;
                            dataStruct.Kd = 0;

                            CabRotater._BoolReset = Environment.TickCount - CabRotater._DateTime < 20000;
                        }
                        else
                        {
                            dataStruct.Cab_Q = CabRotater._Cab_Q;
                            dataStruct.Cab_Qd = CabRotater._Cab_Qd;
                            dataStruct.Kp = CabRotater._Cab_Kp;
                            dataStruct.Kd = CabRotater._Cab_Kd;
                        }
                    }

                    dataStruct.Cab_Q = Math.Max(-CabAmplitude, Math.Min(CabAmplitude, dataStruct.Cab_Q));

                    for (i = 0; i < size; i++) dataBytes[i] = dataStruct.data[i];

                    _UdpClientSend.Send(dataBytes, size);

                    Thread.Sleep(1);
                }

                dataStruct.Kill = 0;
                for (i = 0; i < size; i++) dataBytes[i] = dataStruct.data[i];
                _UdpClientSend.Send(dataBytes, size);

                _UdpClientSend.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            Console.WriteLine("Stop Send Cab");

            if (CabRotater.RotateEnded != null) CabRotater.RotateEnded();
            FormBase.subThread();

            CabRotater._IntCount--;
            CabRotater._BoolAlive = false;
        }

        private static unsafe void tsr()
        {
            Thread.CurrentThread.Name = "Start Read Excavator Data";

            FormBase.addThread();

            Socket mListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint _IPEndPoint = new IPEndPoint(IPAddress.Parse(XPC.HOST_IP), HOST_RECIEVE_CAB_INP);
            mListener.Bind(_IPEndPoint);
            mListener.ReceiveTimeout = 250000;

            int size = sizeof(CabDataFromExcavator);

            var dataStruct = new CabDataFromExcavator();

            Byte[] dataBytes = new Byte[size];

            int remainder = 0;
            int i = 0;

            try
            {
                while (FormBase._BoolThreadAlive && CabRotater._BoolAlive)
                {
                    if (mListener.Available >= size)
                    {
                        mListener.Receive(dataBytes, 0, size, SocketFlags.None);
                        for (i = 0; i < size; i++) dataStruct.data[i] = dataBytes[i];
                        CabRotater.CabDegrees = StaticMethods.toDegreesF(dataStruct.Cab_Q);
                        remainder = mListener.Available % size;
                        if (remainder != 0) Console.WriteLine("Cabs Remain: " + remainder);
                    }
                    else Thread.Sleep(0);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            mListener.Dispose();

            Console.WriteLine("Stop Read Excavator Data");

            FormBase.subThread();
        }
    }
}
