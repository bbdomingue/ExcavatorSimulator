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
        public static volatile float _CAB_Q_RETURN_VALID_IF_BELOW_1000 = 1001;

        private static object LockOb = new object();
        private static float _Cab_Q = 0;
        private static float _Cab_Qd = 0;
        private static float _Cab_Break = 0;
        private static float _Cab_Flow = 0;

        public static void setValues(float Q, float Qd, float brk, float flow)
        {
            lock (CabRotater.LockOb)
            {
                CabRotater._Cab_Q = Q;
                CabRotater._Cab_Qd = Qd;
                CabRotater._Cab_Break = brk;
                CabRotater._Cab_Flow = flow;
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
            [FieldOffset(0)] public fixed byte data[32];
            [FieldOffset(0)] public double Cab_Q;
            [FieldOffset(8)] public double Cab_Qd;
            [FieldOffset(16)] public double Cab_Flow;
            [FieldOffset(24)] public double Cab_Break;
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
            var dataStruct = new CabDataSentToExcavator();
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
                        dataStruct.Cab_Q = CabRotater._Cab_Q;
                        dataStruct.Cab_Qd = CabRotater._Cab_Qd;
                        dataStruct.Cab_Break = CabRotater._Cab_Break;
                        dataStruct.Cab_Flow = CabRotater._Cab_Flow;
                    }

                    for (i = 0; i < size; i++) dataBytes[i] = dataStruct.data[i];

                    _UdpClientSend.Send(dataBytes, size);

                    Thread.Sleep(1);
                }
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
            Thread.CurrentThread.Name = "Send Read Excavator Data";

            FormBase.addThread();

            Console.WriteLine("Start Read Excavator Data");

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
                        CabRotater._CAB_Q_RETURN_VALID_IF_BELOW_1000 = (float)dataStruct.Cab_Q;

                        if (dataStruct.Cab_Q != 0) Console.WriteLine("G:" + dataStruct.Cab_Q);

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

            CabRotater._CAB_Q_RETURN_VALID_IF_BELOW_1000 = 1001;

            mListener.Dispose();

            Console.WriteLine("Stop Read Excavator Data");

            FormBase.subThread();
        }



    }
}
