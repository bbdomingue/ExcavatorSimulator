using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Vector3 = OpenTK.Vector3;
using Matrix4 = OpenTK.Matrix4;

namespace Excavator
{
    public partial class ControlPhantom : UserControl
    {
        public static volatile float f1 = 0, f2 = 0, f3 = 0, f4 = 0;

        private WPFControlPhantom _WPFControlPhantom;

        public ControlPhantom()
        {
            InitializeComponent();

            this.updateThreadStatus();

            this._IntsEncoderOffset[0] = Properties.Settings.Default.calib0;
            this._IntsEncoderOffset[1] = Properties.Settings.Default.calib1;
            this._IntsEncoderOffset[2] = Properties.Settings.Default.calib2;
            this._IntsEncoderOffset[3] = Properties.Settings.Default.calib3;

            var ABM = Matrix4.CreateTranslation(AB) * Matrix4.CreateRotationZ(StaticMethods.toRadiansF(0));
            var BCM = Matrix4.CreateTranslation(BC) * Matrix4.CreateRotationX(StaticMethods.toRadiansF(90));
            var CDM = Matrix4.CreateTranslation(CD) * Matrix4.CreateRotationX(StaticMethods.toRadiansF(90));
            var A = CDM * (BCM * ABM);
            this._Vector3Subtractor = new Vector3(A.M41, A.M42, A.M43);
        }

        private void comboBoxComPort_SelectedValueChanged(object sender, EventArgs e)
        {
            this.buttonPhantomStatus.Enabled = this.comboBoxComPort.SelectedItem != null;
        }


        private void comboBoxComPort_DropDown(object sender, EventArgs e)
        {
            this.comboBoxComPort.Items.Clear();

            foreach (String s in SerialPort.GetPortNames())
            {
                this.comboBoxComPort.Items.Add(s);
            }

            if (this.comboBoxComPort.Items.Count == 0) Console.WriteLine("No items");
        }

        private void buttonPhantomCalibrate_Click(object sender, EventArgs e)
        {
            this._PhantomBoolCalibrateNext = true;
        }



        private void ledPhantomStatus_Click(object sender, EventArgs e)
        {
            if (this._BoolActive)
            {
                this._BoolKeepRunning = false;
            }
            else
            {
                this.comboBoxComPort.Enabled = false;

                this.port  = this.comboBoxComPort.SelectedItem.ToString();

                if (this._BoolActive) return;

                (new Thread(new ThreadStart(this.tsm))).Start();

                this._WPFControlPhantom = new WPFControlPhantom();
                this.elementHostPhantom.Child = this._WPFControlPhantom;
            }
        }

        private void buttonPhantomStatus_Paint(object sender, PaintEventArgs e)
        {
            Color c = this._BoolActive ? Color.Green : Color.Red;

            int border = 6;
            int dim = this.buttonPhantomStatus.Height - border * 2;
            Rectangle r = new Rectangle(border, border, dim, dim);

            e.Graphics.FillEllipse(new SolidBrush(c), r);
        }

        private void updateThreadStatus()
        {
            this.buttonPhantomStatus.Invalidate();

            if (this._BoolActive)
            {
                this.buttonPhantomCalibrate.Enabled = true;
                this.buttonPhantomStatus.Text = "      Cancel";
            }
            else
            {
                this.comboBoxComPort.Enabled = true;
                this.buttonPhantomCalibrate.Enabled = false;
                this.buttonPhantomStatus.Text = "      Start";
                this.elementHostPhantom.Child = null;

                if (this._WPFControlPhantom != null)
                {
                    this._WPFControlPhantom.Deconstruct();
                    this._WPFControlPhantom = null;
                }
            }
        }

        private float fpsPR = 0;
        private float fpsPU = 0;
        private float fpsPRF = 0;
        private float fpsPUF = 0;
        public void updateControlGUI() // Called every tenth update of GL Sim
        {
            if (this._WPFControlPhantom != null)
            {
                this._WPFControlPhantom.angles[0] = this._PhantomState.Angle0;
                this._WPFControlPhantom.angles[1] = this._PhantomState.Angle1;
                this._WPFControlPhantom.angles[2] = this._PhantomState.Angle2;
                this._WPFControlPhantom.angles[3] = this._PhantomState.Angle3;
            }
        }

        public void updateFPS(float elapsed) // Called Once Every 0.2 seconds
        {
            const float alpha = 0.05f;

            if (this._WPFControlPhantom != null)
            {
                this.fpsPR = this._WPFControlPhantom.get_IntFrameCount() / elapsed;
                if (this.fpsPR > this.fpsPRF * 1.1 || this.fpsPR < this.fpsPRF * 0.9) this.fpsPRF = this.fpsPR;
                else this.fpsPRF = this.fpsPRF * (1 - alpha) + this.fpsPR * alpha;
                this.labelPR.Text = this.fpsPRF.ToString("00");
            }
            else this.labelPR.Text = "-";

            this.fpsPU = this._PhantomIntUpdateCount / elapsed;
            if (this.fpsPU > this.fpsPUF * 1.1 || this.fpsPU < this.fpsPUF * 0.9) this.fpsPUF = this.fpsPU;
            else this.fpsPUF = this.fpsPUF * (1 - alpha) + this.fpsPU * alpha;
            this.labelPU.Text = this.fpsPUF.ToString("00");

            this._PhantomIntUpdateCount = 0;
        }



















        internal const float arm0LengthInches = 5.5f;
        internal const float arm1LengthInches = 5.5f;
        internal const float arm2LengthInches = 5.625f;
        internal const float arm3LengthInches = 5.5f;


        internal bool _BoolActive = false;
        internal bool _BoolKeepRunning = false;

        private String port;

        private void tsm()
        {
            this._BoolKeepRunning = true;
            this._BoolActive = true;
            FormBase.addThread();

            this.Invoke((MethodInvoker)delegate { this.updateThreadStatus(); });

            Console.WriteLine("Start Phantom Arduino");

            this._UpdatePhantomStopWatch.Start();

            try
            {
                SerialPort port = new SerialPort(this.port, 115200);

                try
                {
                    port.Open();

                    const int bufL = 8;

                    Byte[] buf = new Byte[bufL];

                    Int16 i0, i1, i2, i3;
                    Int16 test = 0;
                    int countbad = 0;

                    while (FormBase._BoolThreadAlive && this._BoolKeepRunning)
                    {
                        if (test == Int16.MaxValue)
                        {
                            if (port.BytesToRead >= bufL)
                            {
                                port.Read(buf, 0, bufL);

                                i0 = BitConverter.ToInt16(buf, 0);
                                i1 = BitConverter.ToInt16(buf, 2);
                                i2 = BitConverter.ToInt16(buf, 4);
                                i3 = BitConverter.ToInt16(buf, 6);

                                this.UpdatePhantom(i0, i1, i2, i3);

                                test = 0;
                                countbad = 0;
                            }
                        }
                        else if (countbad > bufL)
                        {
                            if (port.BytesToRead > 0)
                            {
                                port.Read(buf, 0, 1);
                                countbad = 0;
                            }
                        }
                        else
                        {
                            if (port.BytesToRead > 1)
                            {
                                port.Read(buf, 0, 2);
                                test = BitConverter.ToInt16(buf, 0);
                                countbad++;
                            }
                        }
                    }

                    port.Close();
                }
                catch (Exception exc) // Error In Loop
                {
                }
            }
            catch (Exception exc) // Error on Open or Close
            {
            }

            Console.WriteLine("Stop Phantom Arduino");

            this._BoolActive = false;

            this.Invoke((MethodInvoker)delegate { this.updateThreadStatus(); });

            FormBase.subThread();
        }













        private const int _IntEncoders = 4;
        private volatile Int32[] _IntsEncoderOffset = new Int32[ControlPhantom._IntEncoders];

        internal volatile int _PhantomIntUpdateCount = 0;
        internal volatile bool _PhantomBoolCalibrateNext = false;

//        private const float unitGain = 2.54f * 3;

        internal Vector3 AB = new Vector3(0, 0, ControlPhantom.arm0LengthInches);
        internal Vector3 BC = new Vector3(0, 0, ControlPhantom.arm1LengthInches);
        internal Vector3 CD = new Vector3(0, 0, ControlPhantom.arm2LengthInches);

        private PhantomState _PhantomState = new PhantomState();
        private PhantomState _PhantomStateTemp = new PhantomState();

        private struct PhantomState
        {
            public Vector3 P;
            public Vector3 V;
            public float Angle0;
            public float Angle1;
            public float Angle2;
            public float Angle3;
        }

        private Vector3 _Vector3Subtractor = new Vector3();
        internal unsafe TrialMarkElton.PhantomDataSentToExcavator getPhantomData()
        {
            var p = new TrialMarkElton.PhantomDataSentToExcavator();

            p.vel[0] = this._PhantomState.V.X;
            p.vel[1] = this._PhantomState.V.Z;
            p.vel[2] = -this._PhantomState.V.Y;

            p.pos[0] = this._PhantomState.P.X;
            p.pos[1] = this._PhantomState.P.Z;
            p.pos[2] = -this._PhantomState.P.Y;

            p.theta = StaticMethods.toRadiansD(this._PhantomState.Angle3);

            return p;

        }

        private Stopwatch _UpdatePhantomStopWatch = new Stopwatch();
        private void UpdatePhantom(Int16 i0, Int16 i1, Int16 i2, Int16 i3)
        {
            float elapsed = (float) this._UpdatePhantomStopWatch.Elapsed.TotalSeconds;
            this._UpdatePhantomStopWatch.Restart();
            this._PhantomIntUpdateCount++;

            if (this._PhantomBoolCalibrateNext)
            {
                this._PhantomBoolCalibrateNext = false;
                this._IntsEncoderOffset[0] = -i0;
                this._IntsEncoderOffset[1] = -i1;
                this._IntsEncoderOffset[2] = -i2;
                this._IntsEncoderOffset[3] = -i3;

                Properties.Settings.Default.calib0 = this._IntsEncoderOffset[0];
                Properties.Settings.Default.calib1 = this._IntsEncoderOffset[1];
                Properties.Settings.Default.calib2 = this._IntsEncoderOffset[2];
                Properties.Settings.Default.calib3 = this._IntsEncoderOffset[3];
            }

            this._PhantomStateTemp = new PhantomState();

            this._PhantomStateTemp.Angle0 = (i3 + this._IntsEncoderOffset[3]) / 100.0f;
            this._PhantomStateTemp.Angle1 = (i1 + this._IntsEncoderOffset[1]) / 100.0f;
            this._PhantomStateTemp.Angle2 = (i2 + this._IntsEncoderOffset[2]) / 100.0f;
            this._PhantomStateTemp.Angle3 = (i0 + this._IntsEncoderOffset[0]) / -5.0f;

            this._PhantomStateTemp.Angle2 = 90 - this._PhantomStateTemp.Angle2 + this._PhantomStateTemp.Angle1;
            this._PhantomStateTemp.Angle1 = 90 - this._PhantomStateTemp.Angle1;

            var ABM = Matrix4.CreateTranslation(AB) * Matrix4.CreateRotationZ(StaticMethods.toRadiansF(this._PhantomStateTemp.Angle0));
            var BCM = Matrix4.CreateTranslation(BC) * Matrix4.CreateRotationX(StaticMethods.toRadiansF(this._PhantomStateTemp.Angle1));
            var CDM = Matrix4.CreateTranslation(CD) * Matrix4.CreateRotationX(StaticMethods.toRadiansF(this._PhantomStateTemp.Angle2));

            var A = CDM * (BCM * ABM);

            this._PhantomStateTemp.P = new Vector3(A.M41, A.M42, A.M43) - this._Vector3Subtractor;
            this._PhantomStateTemp.P *= ControlPhantom.f4;
            this._PhantomStateTemp.P.X -= ControlPhantom.f1;
            this._PhantomStateTemp.P.Y -= ControlPhantom.f2;
            this._PhantomStateTemp.P.Z -= ControlPhantom.f3;
            this._PhantomStateTemp.V = (this._PhantomStateTemp.P - this._PhantomState.P) / elapsed;
            this._PhantomState = this._PhantomStateTemp;
        }


    }
}
