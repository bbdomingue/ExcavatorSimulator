using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using SamSeifert.GLE;

namespace Excavator
{
    public partial class TrialMarkElton : Trial
    {
        const int HOST_SEND_PHANTOM_INP = 42101; // Data leaves this port on here to
        const int XPC_PORT_PHANTOM_INP = 26451;  // get to this port on target

        const int HOST_SEND_TIME_INP = 42102; // Data leaves from this port on here to
        const int XPC_PORT_TIME_INP = 25505;  // get to this port on target

        const int HOST_READ_SIM_DATA = 25521; // Comes from 25541 on Target
//        const int HOST_READ_PHANTOM_FORCES = 23232;


        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public unsafe struct PhantomDataSentToExcavator
	    {
            [FieldOffset(0)]
            public fixed byte data[56];

            [FieldOffset(0)]
            public fixed double pos[3]; // 24 Bytes (8 Each)

            [FieldOffset(24)]
            public double theta;  // 8 Bytes

            [FieldOffset(32)]
            public fixed double vel[3];  // 24 Bytes (8 Each)
            //double time;
	    };

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public unsafe struct GraphicsDataRecievedFromExcavator
        {		
            [FieldOffset(0)]
            public fixed byte data[141];

            [FieldOffset(0)]
            public fixed double th[4]; // 32 Bytes (8 Each)

            [FieldOffset(32)]
            public fixed double t[4]; // 32 Bytes (8 Each)

            [FieldOffset(64)]
            public UInt32 time; // 4 Bytes

            [FieldOffset(68)]
            public UInt32 Load; // 4 Bytes

            [FieldOffset(72)]
            public fixed UInt16 depth[16]; // 32 Bytes (2 Each)

            [FieldOffset(104)]
            public fixed double pile[4]; // 32 Bytes (8 Each)

            [FieldOffset(136)]
            public UInt32 vol; // 4 Bytes

            [FieldOffset(140)]
            public bool trench; // 1 Byte
        };

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public unsafe struct TimeDataSentToExcavator
        {
            [FieldOffset(0)]
            public fixed byte data[4];

            [FieldOffset(0)]
            public UInt32 time;  // 8 Bytes
        };












        private bool _SetupTheNums = false;

        public TrialMarkElton() : base()
        {
            InitializeComponent();
        }

        public TrialMarkElton(FormBase fb)
            : base(fb)
        {
            InitializeComponent();

            this.numericUpDown1.Value = (decimal)Properties.Settings.Default.f1;
            this.numericUpDown2.Value = (decimal)Properties.Settings.Default.f2;
            this.numericUpDown3.Value = (decimal)Properties.Settings.Default.f3;
            this.numericUpDown4.Value = (decimal)Properties.Settings.Default.f4;
            this._SetupTheNums = true;
            this.numericUpDownPhantomScale_ValueChanged(null, EventArgs.Empty);

            new Thread(new ThreadStart(this.SendPhantomData)).Start();
            new Thread(new ThreadStart(this.SendTimeData)).Start();
            new Thread(new ThreadStart(this.ReadSimData)).Start();


            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    Console.WriteLine(localIP);
                }
            }
        }

            



        public override bool hasGhost()
        {
            return true;
        }

        public override void Deconstruct()
        {
            base.Deconstruct();
            this.controlPhantom1._BoolKeepRunning = false;
        }

        public override void GLDelete()
        {
            base.GLDelete();
            if (this._BoolSetupGL4)
            {
                this._BoolSetupGL4 = false;
                GL.DeleteBuffers(1, ref this._IntIndicesBufferID);
                GL.DeleteBuffers(1, ref this._IntInterleaveBufferID);
            }
        }


        private volatile int _FPS_IntSendTime = 0;
        private volatile int _FPS_IntSendPhantom = 0;
        private volatile int _FPS_IntRecieveGraphics = 0;

        private float _FPS_ST = 0;
        private float _FPS_STF = 0;
        private float _FPS_SP = 0;
        private float _FPS_SPF = 0;
        private float _FPS_RG = 0;
        private float _FPS_RGF = 0;


        public override void updateSim()
        {
            base.updateSim();

            if (this._BoolSetupGL4) this.updateGL4();
            else this.setupGL4();
        }

        public override void Gui_Label_Tick(float accumulator)
        {
            base.Gui_Label_Tick(accumulator);

            this.controlPhantom1.updateFPS(accumulator);

            const float alpha = 0.05f;

            this._FPS_ST = this._FPS_IntSendTime / accumulator;
            if (this._FPS_ST > this._FPS_STF * 1.1 || this._FPS_ST < this._FPS_STF * 0.9) this._FPS_STF = this._FPS_ST;
            else this._FPS_STF = this._FPS_STF * (1 - alpha) + this._FPS_ST * alpha;
            this.labelST.Text = this._FPS_STF.ToString("00");
            this._FPS_IntSendTime = 0;

            this._FPS_SP = this._FPS_IntSendPhantom / accumulator;
            if (this._FPS_SP > this._FPS_SPF * 1.1 || this._FPS_SP < this._FPS_SPF * 0.9) this._FPS_SPF = this._FPS_SP;
            else this._FPS_SPF = this._FPS_SPF * (1 - alpha) + this._FPS_SP * alpha;
            this.labelSP.Text = this._FPS_SPF.ToString("00");
            this._FPS_IntSendPhantom = 0;

            this._FPS_RG = this._FPS_IntRecieveGraphics / accumulator;
            if (this._FPS_RG > this._FPS_RGF * 1.1 || this._FPS_RG < this._FPS_RGF * 0.9) this._FPS_RGF = this._FPS_RG;
            else this._FPS_RGF = this._FPS_RGF * (1 - alpha) + this._FPS_RG * alpha;
            this.labelRG.Text = this._FPS_RGF.ToString("00");
            this._FPS_IntRecieveGraphics = 0;
        }

        public override void Gui_Draw_Tick()
        {
            base.Gui_Draw_Tick();

            this.controlPhantom1.updateControlGUI();
        }













        private unsafe void SendPhantomData()
        {
            FormBase.addThread();

            Console.WriteLine("Start Mark Elton Send Phantom");

            int size = sizeof(PhantomDataSentToExcavator);
            var dataStruct = new PhantomDataSentToExcavator();
            Byte[] dataBytes = new byte[size];

            int i;

            try
            {
                UdpClient _UdpClient = new UdpClient(HOST_SEND_PHANTOM_INP);
                _UdpClient.Connect(IPAddress.Parse(XPC.XPC_IP), XPC_PORT_PHANTOM_INP);

                while (FormBase._BoolThreadAlive && this._BoolAlive)
                {
                    this._FPS_IntSendPhantom++;

                    dataStruct = this.controlPhantom1.getPhantomData();

                    for (i = 0; i < size; i++) dataBytes[i] = dataStruct.data[i];

                    _UdpClient.Send(dataBytes, size);

                    Thread.Sleep(10);
                }
                _UdpClient.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            Console.WriteLine("Stop Mark Elton Send Phantom");

            FormBase.subThread();
        }

        private unsafe void SendTimeData()
        {
            FormBase.addThread();

            Console.WriteLine("Start Mark Elton Send Time");

            int size = sizeof(TimeDataSentToExcavator);
            var dataStruct = new TimeDataSentToExcavator();
            Byte[] dataBytes = new byte[size];

            int i;
         
            Stopwatch sendTimer = new Stopwatch();
            sendTimer.Start();

            try
            {
                UdpClient _UdpClient = new UdpClient(HOST_SEND_TIME_INP);
                _UdpClient.Connect(IPAddress.Parse(XPC.XPC_IP), XPC_PORT_TIME_INP);

                while (FormBase._BoolThreadAlive && this._BoolAlive)
                {
                    this._FPS_IntSendTime++;

                    dataStruct.time = (UInt32)sendTimer.ElapsedMilliseconds;

                    for (i = 0; i < size; i++) dataBytes[i] = dataStruct.data[i];

                    _UdpClient.Send(dataBytes, size);

                    Thread.Sleep(1);
                }
                _UdpClient.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            Console.WriteLine("Stop Mark Elton Send Time");

            FormBase.subThread();
        }

        private unsafe void ReadSimData()
        {
            FormBase.addThread();

            Console.WriteLine("Start Mark Elton Read Data");

            Socket mListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint _IPEndPoint = new IPEndPoint(IPAddress.Any, HOST_READ_SIM_DATA);
            mListener.Bind(_IPEndPoint);
            mListener.ReceiveTimeout = 250000;

            int size = sizeof(GraphicsDataRecievedFromExcavator);

            GraphicsDataRecievedFromExcavator dataStruct = new GraphicsDataRecievedFromExcavator();

            Byte[] dataBytes = new Byte[size];

            int remainder = 0;
            int i = 0;

            try
            {
                while (FormBase._BoolThreadAlive && this._BoolAlive)
                {
                    if (mListener.Available >= size)
                    {
                        this._FPS_IntRecieveGraphics++;

                        mListener.Receive(dataBytes, 0, size, SocketFlags.None);
                        for (i = 0; i < size; i++) dataStruct.data[i] = dataBytes[i];

                        this.ActualAngles.swi = 0;
                        this.ActualAngles.cab = (float)dataStruct.th[0];
                        this.ActualAngles.boo = (float)dataStruct.th[1];
                        this.ActualAngles.arm = (float)dataStruct.th[2];
                        this.ActualAngles.buc = (float)dataStruct.th[3];

                        this.GhostAngles.swi = 0;
                        this.GhostAngles.cab = (float)dataStruct.t[0];
                        this.GhostAngles.boo = (float)dataStruct.t[1];
                        this.GhostAngles.arm = (float)dataStruct.t[2];
                        this.GhostAngles.buc = (float)dataStruct.t[3];

                        this._DataStorage.setDepthLoad(dataStruct);

                        remainder = mListener.Available % size;
                        if (remainder != 0) Console.WriteLine("Graphics Remain: " + remainder);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            mListener.Dispose();

            Console.WriteLine("Stop Mark Elton Read Data");

            FormBase.subThread();
        }


























































        private DataStorage _DataStorage = new DataStorage();
        private class  DataStorage
        {
            const int datal = 256;

            public int[] Depth = new int[datal];
	        public double Vload, Vpile, pilex, piley, pilez, pileamt;
            public UInt64 volume;

	        public DataStorage() 
	        {
		        for(int k = 0 ; k < datal ; k++ )
		        {
			        Depth[k] = 50;
		        }	
	        }
	        
            public int getDepth(int n) { return Depth[n]; }

            public unsafe void setDepthLoad(GraphicsDataRecievedFromExcavator msg)
            { 
		        uint based = (32*( msg.time % 8 ));
		
		        for (int k = 0 ; k < 16 ; k++ )	 // unpack
		        {
			        // First variable
			        int temp = ( msg.depth[k] % 256 );
			        Depth[based + 2*k] = (temp);
			
			        // Second variable
			        temp = (msg.depth[k] - temp);
                    Depth[based + 2 * k + 1] = (temp / 256);
		        }	
		
		        // unpack Vload and Vpile
		        uint temp32 = msg.Load % 65536;
		        Vload = temp32;
                Vpile = ((msg.Load - temp32) / 65536);
		        pilex = msg.pile[0];
                piley = msg.pile[1];
                pilez = msg.pile[2];
                pileamt = msg.pile[3];
		        volume = msg.vol;
	        }
        };
















        const int _IntDiscreteSectionsMatlab = 256;				// number of nodes
        const int GroundLevel = -20;			// inches
        const int MaxDepth = 150;				// inches
        const int trenchWidth = 24;					// inches
        const float TrenchStart = -31.5423f;    // inches
        const float TrenchEnd = (250-TrenchStart); // inches	
        const int NumStatesK_c = 3;
        const int NumStatesFilter = 8;
        const int NumStatesLoad = 2;	
        const int Max_V_Load = 7200;
        const int sidelen = 60;				//length of the sides of the box

        const float YGround = GroundLevel + Bobcat._SamCatLiftHeight + Bobcat._V3RotationOffsetY;				// inches (defined in SoilParam)
        const float YTrenchBottom = YGround - MaxDepth; // inches (defined in SoilParam)
        const float XhalfL = - trenchWidth / 2;
        const float XhalfR = trenchWidth / 2;
        const float ZStartTrench = TrenchStart;				// inches
        const float ZEndTrench = TrenchEnd;					// inches
        const float dX = (ZEndTrench - ZStartTrench) / 257;			// X resolution

        const int _IntDiscreteSectionsRender = _IntDiscreteSectionsMatlab + 2;
        const int _IntTopDrawPoints = _IntDiscreteSectionsRender * 2;

        Vector3[] idata = new Vector3[_IntTopDrawPoints * 2];

        public const float XTextL = Trial._IntTextureDensity * (0.5f + (XhalfL / (2 * Trial._FloatGroundPlaneDim)));
        public const float XTextR = Trial._IntTextureDensity * (0.5f - (XhalfL / (2 * Trial._FloatGroundPlaneDim)));
        public const float ZTextBehind = Trial._IntTextureDensity * (0.5f - (ZStartTrench / (2 * Trial._FloatGroundPlaneDim)));
        public const float ZTextFront = Trial._IntTextureDensity * (0.5f - (ZEndTrench / (2 * Trial._FloatGroundPlaneDim)));

        public override void drawObjectsInShadow(bool ShadowBufferDraw)
        {
            if (this._BoolSetupGL4) this.drawGL4();
                
//                display_piles(xpilearray, ypilearray, vpilearray, pilecount, zpile, falltime);// draw piles

//SAM            display_box(boxx, boxy, boxheight, soilheightL, soilheightR);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Trial._IntTextureGrass);

            GL.Begin(BeginMode.Quads);
            {
                GL.TexCoord2(TrialMarkElton.XTextL, Trial._IntTextureDensity);
                GL.Vertex3(XhalfL, YGround, Trial._FloatGroundPlaneDim);
                GL.TexCoord2(TrialMarkElton.XTextL, 0);
                GL.Vertex3(XhalfL, YGround, -Trial._FloatGroundPlaneDim);
                GL.TexCoord2(0, 0);
                GL.Vertex3(-Trial._FloatGroundPlaneDim, YGround, -Trial._FloatGroundPlaneDim);
                GL.TexCoord2(0, Trial._IntTextureDensity);
                GL.Vertex3(-Trial._FloatGroundPlaneDim, YGround, Trial._FloatGroundPlaneDim);

                GL.TexCoord2(Trial._IntTextureDensity, Trial._IntTextureDensity);
                GL.Vertex3(Trial._FloatGroundPlaneDim, YGround, Trial._FloatGroundPlaneDim);
                GL.TexCoord2(Trial._IntTextureDensity, 0);
                GL.Vertex3(Trial._FloatGroundPlaneDim, YGround, -Trial._FloatGroundPlaneDim);
                GL.TexCoord2(TrialMarkElton.XTextR, 0);
                GL.Vertex3(XhalfR, YGround, -Trial._FloatGroundPlaneDim);
                GL.TexCoord2(TrialMarkElton.XTextR, Trial._IntTextureDensity);
                GL.Vertex3(XhalfR, YGround, Trial._FloatGroundPlaneDim);

                GL.TexCoord2(TrialMarkElton.XTextL, TrialMarkElton.ZTextBehind);
                GL.Vertex3(XhalfL, YGround, -ZStartTrench);
                GL.TexCoord2(TrialMarkElton.XTextL, Trial._IntTextureDensity);
                GL.Vertex3(XhalfL, YGround, Trial._FloatGroundPlaneDim);
                GL.TexCoord2(TrialMarkElton.XTextR, Trial._IntTextureDensity);
                GL.Vertex3(XhalfR, YGround, Trial._FloatGroundPlaneDim);
                GL.TexCoord2(TrialMarkElton.XTextR, TrialMarkElton.ZTextBehind);
                GL.Vertex3(XhalfR, YGround, -ZStartTrench);

                GL.TexCoord2(TrialMarkElton.XTextL, 0);
                GL.Vertex3(XhalfL, YGround, -Trial._FloatGroundPlaneDim);
                GL.TexCoord2(TrialMarkElton.XTextL, TrialMarkElton.ZTextFront);
                GL.Vertex3(XhalfL, YGround, -ZEndTrench);
                GL.TexCoord2(TrialMarkElton.XTextR, TrialMarkElton.ZTextFront);
                GL.Vertex3(XhalfR, YGround, -ZEndTrench);
                GL.TexCoord2(TrialMarkElton.XTextR, 0);
                GL.Vertex3(XhalfR, YGround, -Trial._FloatGroundPlaneDim);
            }
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture7);

/*            else
            {
                GL.Begin(BeginMode.Quads);
                {
                    GL.Vertex3(XhalfL, YGround, Trial._FloatGroundPlaneDim);
                    GL.Vertex3(XhalfL, YGround, -Trial._FloatGroundPlaneDim);
                    GL.Vertex3(-Trial._FloatGroundPlaneDim, YGround, -Trial._FloatGroundPlaneDim);
                    GL.Vertex3(-Trial._FloatGroundPlaneDim, YGround, Trial._FloatGroundPlaneDim);

                    GL.Vertex3(Trial._FloatGroundPlaneDim, YGround, Trial._FloatGroundPlaneDim);
                    GL.Vertex3(Trial._FloatGroundPlaneDim, YGround, -Trial._FloatGroundPlaneDim);
                    GL.Vertex3(XhalfR, YGround, -Trial._FloatGroundPlaneDim);
                    GL.Vertex3(XhalfR, YGround, Trial._FloatGroundPlaneDim);

                    GL.Vertex3(XhalfL, YGround, -ZStartTrench);
                    GL.Vertex3(XhalfL, YGround, Trial._FloatGroundPlaneDim);
                    GL.Vertex3(XhalfR, YGround, Trial._FloatGroundPlaneDim);
                    GL.Vertex3(XhalfR, YGround, -ZStartTrench);
                }
                GL.End();
            }*/

        }



        private bool _BoolSetupGL4 = false;
        private int _IntInterleaveBufferID;
        private int _IntIndicesBufferID;
        private int _IntElementCount = 0;

        private void setupGL4()
        {
            DataStorage data = this._DataStorage;

            idata[(0) * 2] = new Vector3(XhalfL, YGround, -(ZStartTrench));
            idata[(1) * 2] = new Vector3(XhalfR, YGround, -(ZStartTrench));
            idata[(0) * 2 + 1] = Vector3.UnitY;
            idata[(1) * 2 + 1] = Vector3.UnitY;

            float Ym = 0;
            float Y0 = YGround;
            float Yp = YGround - data.getDepth(0) + 50;
            float Z0;

            for (int k = 0; k < _IntDiscreteSectionsMatlab; k++)
            {
                Ym = Y0;
                Y0 = Yp;
                Yp = YGround - ((k == _IntDiscreteSectionsMatlab - 1) ? 0 : data.getDepth(k + 1) - 50);

                Z0 = -(ZStartTrench + (k + 1) * dX);

                Vector3 norm = new Vector3(0, 2 * dX, Ym - Yp);
                norm.NormalizeFast();

                idata[(2 * k + 2) * 2] = new Vector3(XhalfL, Y0, Z0);
                idata[(2 * k + 3) * 2] = new Vector3(XhalfR, Y0, Z0);
                idata[(2 * k + 2) * 2 + 1] = norm;
                idata[(2 * k + 3) * 2 + 1] = norm;
            }

            idata[(_IntTopDrawPoints - 2) * 2] = new Vector3(XhalfL, YGround, -(ZEndTrench));
            idata[(_IntTopDrawPoints - 1) * 2] = new Vector3(XhalfR, YGround, -(ZEndTrench));
            idata[(_IntTopDrawPoints - 2) * 2 + 1] = Vector3.UnitY;
            idata[(_IntTopDrawPoints - 1) * 2 + 1] = Vector3.UnitY;
          
            int[] triangs = new int[(_IntDiscreteSectionsRender - 1) * 6];

            for (int i = 0; i < _IntDiscreteSectionsRender - 1; i++)
            {
                int i6 = i * 6;
                int i2 = i * 2;

                triangs[i6 + 0] = i2;
                triangs[i6 + 1] = i2 + 1;
                triangs[i6 + 2] = i2 + 2;
                triangs[i6 + 3] = i2 + 2;
                triangs[i6 + 4] = i2 + 1;
                triangs[i6 + 5] = i2 + 3;
            }

            int bufferSize;

            bufferSize = idata.Length * Vector3.SizeInBytes;
            GL.GenBuffers(1, out this._IntInterleaveBufferID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._IntInterleaveBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bufferSize), idata, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            bufferSize = triangs.Length * sizeof(uint);
            GL.GenBuffers(1, out this._IntIndicesBufferID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._IntIndicesBufferID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferSize), triangs, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            this._IntElementCount = triangs.Length;

            this._BoolSetupGL4 = true;
        }

        private void updateGL4()
        {
            DataStorage data = this._DataStorage;

            idata[(0) * 2] = new Vector3(XhalfL, YGround, -(ZStartTrench));
            idata[(1) * 2] = new Vector3(XhalfR, YGround, -(ZStartTrench));
            idata[(0) * 2 + 1] = Vector3.UnitY;
            idata[(1) * 2 + 1] = Vector3.UnitY;

            float Ym = 0;
            float Y0 = YGround;
            float Yp = YGround - data.getDepth(0) + 50;
            float Z0;

            for (int k = 0; k < _IntDiscreteSectionsMatlab; k++)
            {
                Ym = Y0;
                Y0 = Yp;
                Yp = YGround - ((k == _IntDiscreteSectionsMatlab - 1) ? 0 : data.getDepth(k + 1) - 50);

                Z0 = -(ZStartTrench + (k + 1) * dX);

                Vector3 norm = new Vector3(0, 2 * dX, Ym - Yp);
                norm.NormalizeFast();

                idata[(2 * k + 2) * 2].Y = Y0;
                idata[(2 * k + 3) * 2].Y = Y0;
                idata[(2 * k + 2) * 2 + 1] = norm;
                idata[(2 * k + 3) * 2 + 1] = norm;
            }

            idata[(_IntTopDrawPoints - 2) * 2] = new Vector3(XhalfL, YGround, -(ZEndTrench));
            idata[(_IntTopDrawPoints - 1) * 2] = new Vector3(XhalfR, YGround, -(ZEndTrench));
            idata[(_IntTopDrawPoints - 2) * 2 + 1] = Vector3.UnitY;
            idata[(_IntTopDrawPoints - 1) * 2 + 1] = Vector3.UnitY;

            int bufferSize = idata.Length * Vector3.SizeInBytes;
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._IntInterleaveBufferID);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(bufferSize), idata);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        private void drawGL4()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._IntInterleaveBufferID);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes * 2, IntPtr.Zero);
            GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes * 2, Vector3.SizeInBytes);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IntIndicesBufferID);
            GL.DrawElements(BeginMode.Triangles, _IntElementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        private void numericUpDownPhantomScale_ValueChanged(object sender, EventArgs e)
        {
            if (this._SetupTheNums)
            {
                ControlPhantom.f1 = (float)this.numericUpDown1.Value;
                Properties.Settings.Default.f1 = ControlPhantom.f1;

                ControlPhantom.f2 = (float)this.numericUpDown2.Value;
                Properties.Settings.Default.f2 = ControlPhantom.f2;

                ControlPhantom.f3 = (float)this.numericUpDown3.Value;
                Properties.Settings.Default.f3 = ControlPhantom.f3;

                ControlPhantom.f4 = (float)this.numericUpDown4.Value;
                Properties.Settings.Default.f4 = ControlPhantom.f4;

                Properties.Settings.Default.Save();
            }
        }





                // left side of trench
/*                GL.Begin(BeginMode.Triangles);
                {
                    GL.Normal3(0, -1, 0);
                    GL.Vertex3(X1, halfWidth, Zground);
                    GL.Normal3(0, -1, 0);
                    GL.Vertex3(X1, halfWidth, Z1);
                    GL.Normal3(0, -1, 0);
                    GL.Vertex3(X2, halfWidth, Z2);

                    GL.Normal3(0, -1, 0);
                    GL.Vertex3(X2, halfWidth, Z2);
                    GL.Normal3(0, -1, 0);
                    GL.Vertex3(X2, halfWidth, Zground);
                    GL.Normal3(0, -1, 0);
                    GL.Vertex3(X1, halfWidth, Zground);

                    // right side of trench
                    GL.Normal3(0, -1, 0);
                    GL.Vertex3(X2, -halfWidth, Z2);
                    GL.Normal3(0, -1, 0);
                    GL.Vertex3(X1, -halfWidth, Z1);
                    GL.Normal3(0, -1, 0);
                    GL.Vertex3(X1, -halfWidth, Zground);

                    GL.Normal3(0, 1, 0);
                    GL.Vertex3(X1, -halfWidth, Zground);	//changed
                    GL.Normal3(0, 1, 0);
                    GL.Vertex3(X2, -halfWidth, Zground);
                    GL.Normal3(0, 1, 0);
                    GL.Vertex3(X2, -halfWidth, Z2);
                }
                GL.End();*/
//            }
	
	        ////////////////////////////////////////////////////////////////////////////////
	        //				Display pile of dirt									  //
	        ////////////////////////////////////////////////////////////////////////////////
        /*	
	        temp = (3*data->getVpile())/4;
	        d_pile = 40;

	        // take cube root using Newton-Raphson method 
	        // should converge in less than 10 steps
	        //d_pile = 40;
	        for( k = 0; k < 40; k++ )
	        {
		        d_pile = d_pile - (pow(d_pile,3) - temp)/(3*pow(d_pile,2));
	        }
	
	        // red clay / dirt
	        glColor3f(0.5, 0.4, 0.4);

	        Zground = Zground;
	        */
        /*
	        for( k = 0; k < 4; k++ )
	        {

	        glTranslatef(X_pile,Y_pile,0);
	        glRotatef(22.5*k,0,0,1);
	        glTranslatef(-X_pile,-Y_pile,0);

	        GL.Begin(BeginMode.Triangles);
		        GL.Normal3(1, 0, 1);
		        GL.Vertex3(X_pile + d_pile, Y_pile + d_pile, Zground);
		        GL.Normal3(1, 0, 1);
		        GL.Vertex3(X_pile + d_pile, Y_pile - d_pile, Zground);
		        GL.Normal3(1, 0, 1);
		        GL.Vertex3(X_pile, Y_pile, Zground + d_pile);
	        GL.End(); 

	        GL.Begin(BeginMode.Triangles);
		        GL.Normal3(-1, 0, 1);
		        GL.Vertex3(X_pile - d_pile, Y_pile + d_pile, Zground);
		        GL.Normal3(-1, 0, 1);
		        GL.Vertex3(X_pile - d_pile, Y_pile - d_pile, Zground);
		        GL.Normal3(-1, 0, 1);
		        GL.Vertex3(X_pile, Y_pile, Zground + d_pile);
	        GL.End(); 

	        GL.Begin(BeginMode.Triangles);
		        GL.Normal3(0, 1, 1);
		        GL.Vertex3(X_pile + d_pile, Y_pile + d_pile, Zground);
		        GL.Normal3(0, 1, 1);
		        GL.Vertex3(X_pile - d_pile, Y_pile + d_pile, Zground);
		        GL.Normal3(0, 1, 1);
		        GL.Vertex3(X_pile, Y_pile, Zground + d_pile);
	        GL.End(); 

	        GL.Begin(BeginMode.Triangles);
		        GL.Normal3(0, -1, 1);
		        GL.Vertex3(X_pile + d_pile, Y_pile - d_pile, Zground);
		        GL.Normal3(0, -1, 1);
		        GL.Vertex3(X_pile - d_pile, Y_pile - d_pile, Zground);
		        GL.Normal3(0, -1, 1);
		        GL.Vertex3(X_pile, Y_pile, Zground + d_pile);
	        GL.End();
	        }
	        
        }*/


   }
}




