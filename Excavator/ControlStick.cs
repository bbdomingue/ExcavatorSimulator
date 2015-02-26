using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

using SamSeifert.DoubleBuffer;

namespace Excavator
{
    public partial class ControlStick : UserControl
    {

        internal const int l_LR = 1;
        internal const int l_FB = -2;
        internal const int r_LR = 3;
        internal const int r_FB = -4;

        internal float getValForStick(int a, bool filtered)
        {
            float res = 0;
            switch (Math.Abs(a))
            {
                case 1: res = (filtered ? this._StickBackFloat0F : this._StickBackFloat0); break;
                case 2: res = (filtered ? this._StickBackFloat1F : this._StickBackFloat1); break;
                case 3: res = (filtered ? this._StickBackFloat2F : this._StickBackFloat2); break;
                case 4: res = (filtered ? this._StickBackFloat3F : this._StickBackFloat3); break;
            }

            const float thresh = 0.05f;
            const float mult = 1 + thresh;

            res = Math.Sign(a) * Math.Sign(res) * Math.Max((Math.Abs(res) * mult - thresh), 0);
            return Math.Max(-1, Math.Min(1, res));
        }

        private DoubleBufferedPanel doubleBufferedPanel1 = new DoubleBufferedPanel();

        private static ControlStick __ControlStick = null;
        public static ControlStick _ControlStick
        {
            get
            {
                if (ControlStick.__ControlStick == null) ControlStick.__ControlStick = new ControlStick();
                return ControlStick.__ControlStick;
            }
        }

        private ControlStick()
        {
            InitializeComponent();

            this.panel1.Controls.Add(this.doubleBufferedPanel1);
            this.doubleBufferedPanel1.Dock = DockStyle.Fill;
            this.doubleBufferedPanel1.Paint += new PaintEventHandler(this.doubleBufferedPanel1_Paint);
            this.updateThreadStatus();

        }

        private void comboBoxComPort_SelectedIndexChanged(object sender, EventArgs e)
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
            this._SticksBoolCalibrateNext = true;
        }

        private void buttonPhantomStatus_Click(object sender, EventArgs e)
        {
            if (this._BoolActive)
            {
                ControlStick._BoolKeepRunning = false;
            }
            else
            {
                this.comboBoxComPort.Enabled = false;

                this.port = this.comboBoxComPort.SelectedItem.ToString();

                if (this._BoolActive) return;

                (new Thread(new ThreadStart(this.tsm))).Start();
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
            }
        }

        private float fpsPU = 0;
        private float fpsPUF = 0;
        
        public void updateControlGUI() // Called at 30 fps
        {
            this.doubleBufferedPanel1.Invalidate();
        }

        public void updateFPS(float elapsed) // Called Once Every 0.2 seconds
        {
            const float alpha = 0.05f;

/*            if (this._WPFControlPhantom != null)
            {
                this.fpsPR = this._WPFControlPhantom.get_IntFrameCount() / elapsed;
                if (this.fpsPR > this.fpsPRF * 1.1 || this.fpsPR < this.fpsPRF * 0.9) this.fpsPRF = this.fpsPR;
                else this.fpsPRF = this.fpsPRF * (1 - alpha) + this.fpsPR * alpha;
                this.labelPR.Text = this.fpsPRF.ToString("00");
            }
            else this.labelPR.Text = "-";*/

            this.fpsPU = this._SticksIntUpdateCount / elapsed;
            if (this.fpsPU > this.fpsPUF * 1.1 || this.fpsPU < this.fpsPUF * 0.9) this.fpsPUF = this.fpsPU;
            else this.fpsPUF = this.fpsPUF * (1 - alpha) + this.fpsPU * alpha;
            this.labelPU.Text = this.fpsPUF.ToString("00");

            this._SticksIntUpdateCount = 0;
        }




















        internal static void Stop()
        {
            ControlStick._BoolKeepRunning = false;
        }

        internal bool _BoolActive = false;
        internal static bool _BoolKeepRunning = false;

        private String port;

        private void tsm()
        {
            Thread.CurrentThread.Name = "Arduino Sticks Thread";

            ControlStick._BoolKeepRunning = true;
            this._BoolActive = true;
            FormBase.addThread();

            this.Invoke((MethodInvoker)delegate { this.updateThreadStatus(); });

            Console.WriteLine("Start Joysticks Arduino");

            try
            {
                SerialPort port = new SerialPort(this.port, 57600);

                try
                {
                    port.Open();

                    const int bufL = ControlStick._IntAnalogChannels * 2;

                    Byte[] buf = new Byte[bufL];

                    Int16[] dat = new Int16[ControlStick._IntAnalogChannels];

                    Int16 test = 0;
                    int countbad = 0;

                    while (FormBase._BoolThreadAlive && ControlStick._BoolKeepRunning)
                    {
                        if (test == Int16.MaxValue)
                        {
                            if (port.BytesToRead >= bufL)
                            {
                                port.Read(buf, 0, bufL);

                                for (int i = 0; i < ControlStick._IntAnalogChannels; i++)
                                    dat[i] = BitConverter.ToInt16(buf, i * 2);

                                this.UpdateStick(dat);

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
                    Console.WriteLine("Exc: " + exc.ToString());
                }
            }
            catch (Exception exc) // Error on Open or Close
            {
                Console.WriteLine("Exc: " + exc.ToString());
            }

            Console.WriteLine("Stop Joysticks Arduino");

            this._BoolActive = false;

            this.Invoke((MethodInvoker)delegate { this.updateThreadStatus(); });

            FormBase.subThread();
        }












        const int _IntAnalogChannels = 4;

        internal volatile int _SticksIntUpdateCount = 0;
        internal volatile bool _SticksBoolCalibrateNext = false;
        private volatile bool _SticksBoolCalibrated = true;

        private void UpdateStick(Int16[] dat)
        {
            this._SticksIntUpdateCount++;

            if (this._SticksBoolCalibrateNext)
            {
                this._SticksBoolCalibrateNext = false;

                if (this._SticksBoolCalibrated)
                {
                    this._SticksBoolCalibrated = false;

                    this._StickBackFloat0 = 0;
                    this._StickBackFloat1 = 0;
                    this._StickBackFloat2 = 0;
                    this._StickBackFloat3 = 0;

                    this._StickBackFloat0F = 0;
                    this._StickBackFloat1F = 0;
                    this._StickBackFloat2F = 0;
                    this._StickBackFloat3F = 0;

                    for (int i = 0; i < ControlStick._IntAnalogChannels; i++)
                    {
                        this._StickBackIntMax[i] = int.MinValue;
                        this._StickBackIntMin[i] = int.MaxValue;
                    }
                }
                else
                {
                    this._SticksBoolCalibrated = true;
                    for (int i = 0; i < ControlStick._IntAnalogChannels; i++)
                    {
                        this._StickBackFloatCenter[i] = (this._StickBackIntMax[i] + this._StickBackIntMin[i]) / 2.0f;
                        this._StickBackFloatRange[i] = (this._StickBackIntMax[i] - this._StickBackIntMin[i]) / 2.0f;
                        this._StickBackFloatRange[i] = Math.Max(this._StickBackFloatRange[i], 1);

                        Console.WriteLine(
                            "C: " + this._StickBackFloatCenter[i].ToString("0.0") +
                            " R: " + this._StickBackFloatRange[i].ToString("0.0"));
                    }
                }
            }
            else if (this._SticksBoolCalibrated)
            {
                this._StickBackFloat0 = (dat[0] - this._StickBackFloatCenter[0]) / this._StickBackFloatRange[0];
                this._StickBackFloat1 = (dat[1] - this._StickBackFloatCenter[1]) / this._StickBackFloatRange[1];
                this._StickBackFloat2 = (dat[2] - this._StickBackFloatCenter[2]) / this._StickBackFloatRange[2];
                this._StickBackFloat3 = (dat[3] - this._StickBackFloatCenter[3]) / this._StickBackFloatRange[3];

                const float alpha = 0.95f;
                const float alpham1 = 1 - alpha;

                this._StickBackFloat0F = alpha * this._StickBackFloat0F + alpham1 * this._StickBackFloat0;
                this._StickBackFloat1F = alpha * this._StickBackFloat1F + alpham1 * this._StickBackFloat1;
                this._StickBackFloat2F = alpha * this._StickBackFloat2F + alpham1 * this._StickBackFloat2;
                this._StickBackFloat3F = alpha * this._StickBackFloat3F + alpham1 * this._StickBackFloat3;
            }
            else
            {
                for (int i = 0; i < ControlStick._IntAnalogChannels; i++)
                {
                    this._StickBackIntMax[i] = Math.Max(this._StickBackIntMax[i], dat[i]);
                    this._StickBackIntMin[i] = Math.Min(this._StickBackIntMin[i], dat[i]);
                }
            }
        }

        private int[] _StickBackIntMin = new int[ControlStick._IntAnalogChannels];
        private int[] _StickBackIntMax = new int[ControlStick._IntAnalogChannels];
        private float[] _StickBackFloatCenter = new float[ControlStick._IntAnalogChannels] { 436, 443, 437, 438 };
        private float[] _StickBackFloatRange = new float[ControlStick._IntAnalogChannels] { 351, 350, 350, 351 };

        /*
            C: 436.0 R: 351.0
            C: 443.0 R: 350.0
            C: 437.0 R: 350.0
            C: 438.5 R: 351.5
         */

        private volatile float _StickBackFloat0 = 0;
        internal volatile float _StickBackFloat0F = 0;
        private volatile float _StickBackFloat1 = 0;
        internal volatile float _StickBackFloat1F = 0;
        private volatile float _StickBackFloat2 = 0;
        internal volatile float _StickBackFloat2F = 0;
        private volatile float _StickBackFloat3 = 0;
        internal volatile float _StickBackFloat3F = 0;

        public void doubleBufferedPanel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.LightGray);
            this.drawStick(true, e.Graphics);
            this.drawStick(false, e.Graphics);
        }

        private void drawStick(Boolean b, Graphics g)
        {
            int LR = b ? ControlStick.r_LR : ControlStick.l_LR;
            int FB = b ? ControlStick.r_FB : ControlStick.l_FB;
           
            const int sq = 95;
            const int d = 14;


            int centy = (sq - d) / 2;
            int centx = centy + (b ? sq : 0); 

            Rectangle r = new Rectangle(centx, centy, d, d);

            g.FillEllipse(new SolidBrush(Color.Black), r);

            int range = (sq / 2 - d);
            
            r.X = (int)(centx + range * this.getValForStick(LR, true));
            r.Y = (int)(centy + range * this.getValForStick(FB, true));

            g.FillEllipse(new SolidBrush(Color.Purple), r);

            r.X = (int)(centx + range * this.getValForStick(LR, false));
            r.Y = (int)(centy + range * this.getValForStick(FB, false));

            g.FillEllipse(new SolidBrush(Color.Green), r);
        }


    }
}
