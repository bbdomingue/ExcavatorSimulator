using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;

using SamSeifert.GlobalEvents;
using SamSeifert.GLE;
using SamSeifert.GLE.CadViewer;
using SamSeifert.HeadTracker;

using SamSeifert.ScreenPicker;

namespace Excavator
{
    public partial class FormBase : Form
    {
        private static FormBase _Instance = null;
        internal static FormBase Instance
        {
            get
            {
                if (_Instance == null) _Instance = new FormBase();
                return _Instance;
            }
        }

        private GL_Handler _GL_Handler = new GL_Handler();

        private static volatile int _IntThreadCount = 0;
        internal static volatile bool _BoolThreadAlive = true;

        private CadHandler cadHandler1;

        private FormBase()
        {
            this.InitializeComponent();

            if (this.DesignMode) return;

            FormPickScreen._BottomPix = Bobcat._BoolLaptop ? 0 : 1;

            HeadTrackerManager.ThreadStarted += new HeadTrackEventHandler(FormBase.addThread);
            HeadTrackerManager.ThreadEnded += new HeadTrackEventHandler(FormBase.subThread);
            HeadTrackerManager.TrackingStarted += new HeadTrackEventHandler(this.HeadTrackStart);
            HeadTrackerManager.TrackingEnded += new HeadTrackEventHandler(this.HeadTrackEnd);
            HeadTrackerManager.GotHeadData += new HeadTrackDataEventHandler(FormBase.GotHeadData);

            CabRotater.RotateStarted += new CabRotaterEventHandler(this.CabRotater_RotateStarted);
            CabRotater.RotateEnded += new CabRotaterEventHandler(this.CabRotater_RotateEnded);

            this.textBoxSavePath.Text = Properties.Settings.Default.SavePath;
        }













        internal void RefreshTrial(Trial t)
        {
            while (this.panelRightTrial.Controls.Count > 0) this.panelRightTrial.Controls.RemoveAt(0);

            if (t == null) return;

            this.panelRightTrial.Controls.Add(t);

            t.BackColor = Color.White;
            t.Dock = DockStyle.Fill;

            this.labelTrial.Text = t.getName();
        }



        internal bool _BoolFullScreen = false;
        private Form _FormFullScreen;
        internal void setFullScreenForm(Form f)
        {
            this._BoolFullScreen = true;
            this._FormFullScreen = f;
            this.buttonFullScreen.Enabled = false;
            this._GL_Handler.setControl(this._FormFullScreen);
            this.checkBoxAspectRatio.Enabled = false;
        }

        private void GlobalKeyDown(object sender, EventArgs e)
        {
            if (GlobalEventHandler.isKeyPressed(Keys.Escape))
            {
                if (this._FormFullScreen != null)
                {
                    this._BoolFullScreen = false;
                    this.buttonFullScreen.Enabled = true;
                    this._GL_Handler.setControl(this.panelInnerGL);
                    this._FormFullScreen.Close();

                    this._FormFullScreen = null;
                    this.checkBoxAspectRatio.Enabled = true;
                }
            }
        }

        private bool GlobalMouseDragBool = false;
        private void GlobalMouseClick(object sender, EventArgs e)
        {
            Point p = this.panelInnerGL.PointToClient(Cursor.Position);
            this.GlobalMouseDragBool = this.panelInnerGL.DisplayRectangle.Contains(p);
        }

        private void GlobalMouseDrag(object sender, MouseEventArgs e)
        {
            if (this.GlobalMouseDragBool)
            {
                NumericUpDown nud;
                const decimal scale = 0.25m;

                nud = this.numericUpDownHeadYaw;
                nud.Value = Math.Min(nud.Maximum, Math.Max(nud.Minimum, nud.Value + e.X * scale));

                nud = this.numericUpDownHeadPitch;
                nud.Value = Math.Min(nud.Maximum, Math.Max(nud.Minimum, nud.Value + e.Y * scale));
            }
        }

        internal static void addThread()
        {
            FormBase f = FormBase.Instance;
            if (f.InvokeRequired) f.Invoke((MethodInvoker)delegate { FormBase.addThread(); });
            else FormBase._IntThreadCount++;
        }

        internal static void subThread()
        {
            FormBase f = FormBase.Instance;
            if (f.InvokeRequired) f.Invoke((MethodInvoker)delegate { FormBase.subThread(); });
            else
            {
                FormBase._IntThreadCount--;
                if (FormBase._IntThreadCount == 0 && !FormBase._BoolThreadAlive) FormBase.Instance.Close();
            }
        }

        private void FormBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormBase._IntThreadCount > 0) e.Cancel = true;
            else
            {
                Properties.Settings.Default.Save();
                Trial.DeconstructS();
            }

            HeadTrackerManager.Stop();
            ControlStick.Stop();
            TrialSaver.Stop();

            FormBase._BoolThreadAlive = false;
            FormBase.Instance.Enabled = false;
        }

        private void FormBase_FormClosed(object sender, FormClosedEventArgs e)
        {
            HeadTrackerManager.Stop();
        }

        private void checkBoxShowSide_CheckedChanged(object sender, EventArgs e)
        {
            this._GL_Handler._BoolShowSide = this.checkBoxShowSide.Checked;
        }

        private void buttonFullScreen_Click(object sender, EventArgs e)
        {
            new FormPickScreen().ShowDialog();

            if (FormPickScreen.SetupFormGood)
            {
                this._FormFullScreen = new Form();
                FormPickScreen.SetupForm(this._FormFullScreen);
                FormBase.Instance.setFullScreenForm(this._FormFullScreen);
            }
        }





























        public static volatile float floatRandom;

        private void Application_Idle(object sender, EventArgs e)
        {
            if (FormBase._BoolThreadAlive)
            {
                if (this._GL_Handler.IsIdle)
                {
                    this._GL_Handler.Invalidate();
                }
            }

            if (Trial.UpdateMainThreadS()) // TRUE EVERY 30 MS
            {
                var ba = Bobcat.ActualAngles;
                this.labelAngleCab.Text = ba.cab.ToString("0.0");
                this.labelAngleSwing.Text = ba.swi.ToString("0.0");
                this.labelAngleBoom.Text = ba.boo.ToString("0.0");
                this.labelAngleArm.Text = ba.arm.ToString("0.0");
                this.labelAngleBucket.Text = ba.buc.ToString("0.0");
                this.labelFPS.Text = Trial.FPS.ToString("0");
                
                this.labelCabHardware.Text = CabRotater.CabDegrees.ToString();
                this.labelTimeLeft.Text = TrialSaver._TimeLeft.ToString("00.0");

                this.labelRandom.Text = floatRandom.ToString();
            }
        }






























        private GLControl _GLControlTemp;

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.panelPinkTV.Height = (int)(this.panelPinkTV.Width * Bobcat._TV_Height_Inches / Bobcat._TV_Width_Inches);
            this.updatePinkTV2();

            GraphicsMode g = GraphicsMode.Default;
            Console.WriteLine("Defaults - Depth: " + g.Depth + ", Stencil: " + g.Stencil);
            g = new GraphicsMode(g.ColorFormat, 32, 0, g.Samples, g.ColorFormat, 1, true);
            this._GLControlTemp = new GLControl(g);
            this._GLControlTemp.Load += new EventHandler(this._GLControlTemp_Load);
            this.panelOuterGL.Controls.Add(this._GLControlTemp);
        }

        private void _GLControlTemp_Load(object sender, EventArgs e)
        {
            this.radioButtonEnvStereo.Enabled = this._GLControlTemp.GraphicsMode.Stereo;
            Console.WriteLine("3D Enabled: " + this.radioButtonEnvStereo.Enabled);
            this.panelOuterGL.Controls.Remove(this._GLControlTemp);
            this._GLControlTemp.Dispose();
            this._GLControlTemp = null;

            this.cadHandler1 = new CadHandler();
            this.cadHandler1.Dock = DockStyle.Fill;
            this.tabPageOpenGL.Controls.Add(this.cadHandler1);

            new TE_FlowKeyboard();

            this.panelOuterGL_Resize(sender, e);

            GlobalEventHandler.KeyDown += new EventHandler(this.GlobalKeyDown);
            GlobalEventHandler.LMouseDrag += new MouseEventHandler(this.GlobalMouseDrag);
            GlobalEventHandler.LMouseDown += new EventHandler(this.GlobalMouseClick);

            this.numericUpDownSkyBoxDim_ValueChanged(sender, e);

            this.numericUpDownHeadPitch_ValueChanged(sender, e);
            this.numericUpDownHeadYaw_ValueChanged(sender, e);
            this.numericUpDownOutsideDistance_ValueChanged(sender, e);

//            this.radioButtonEnv_CheckedChanged(this.radioButtonEnvNorm, e);
//            this.radioButtonEnv_CheckedChanged(this.radioButtonEnvCat, e);
            this.radioButtonEnv_CheckedChanged(this.radioButtonEnvOutside, e);

            this.numericUpDownHT_X.Value = Config.HTX;
            this.numericUpDownHT_Y.Value = Config.HTY;
            this.numericUpDownHT_Z.Value = Config.HTZ;
            this.numericUpDownHT_O.Value = Config.HTO;

            this.numericUpDownHT_X_ValueChanged(sender, e);
            this.numericUpDownHT_Y_ValueChanged(sender, e);
            this.numericUpDownHT_Z_ValueChanged(sender, e);
            this.numericUpDownHT_O_ValueChanged(sender, e);
            this.numericUpDownHead_ValueChanged(sender, e);

            this.checkBoxMute.Checked = Config.Mute;
            this.checkBoxMute_CheckedChanged(sender, e);

            this.checkBoxPhysX.Checked = Config.DrawPhysX;
            this.checkBoxPhysX_CheckedChanged(sender, e);

            this.numericUpDownEyeSeperation.Value = Config.EyeDist;
            this.numericUpDownEyeSeperation_ValueChanged(sender, e);

            this.nudCabK_ValueChanged(sender, e);

            this._GL_Handler.setControl(this.panelInnerGL);

            this.loadMedia();

            Application.Idle += new EventHandler(this.Application_Idle);

            this.Enabled = true;
        }
















        private void numericUpDownHeadPitch_ValueChanged(object sender, EventArgs e)
        {
            this._GL_Handler._FloatHeadPitch = (float)this.numericUpDownHeadPitch.Value;
        }

        private void numericUpDownHeadYaw_ValueChanged(object sender, EventArgs e)
        {
            decimal value = this.numericUpDownHeadYaw.Value;
            if (Math.Abs(value) == 180.1m)
            {
                value = 179.9m * (value > 0 ? -1 : 1);
                this.numericUpDownHeadYaw.Value = value;
            }

            this._GL_Handler._FloatHeadYaw = (float)this.numericUpDownHeadYaw.Value;
        }

        private void numericUpDownOutsideDistance_ValueChanged(object sender, EventArgs e)
        {
            this._GL_Handler._FloatOutsideDistance = (float)this.numericUpDownOutsideDistance.Value;
        }

        private void numericUpDownHead_ValueChanged(object sender, EventArgs e)
        {
            if (!this.checkBoxHeadTrack.Checked)
            {
                Vector3 v = new Vector3(
                    (float)this.numericUpDownHeadX.Value,
                    (float)this.numericUpDownHeadY.Value,
                    (float)this.numericUpDownHeadZ.Value);

                FormBase._HeadLocationCurrent.EyeM = v;

                v.X += _HeadEyeDistance2;

                FormBase._HeadLocationCurrent.EyeR = v;

                v.X -= _HeadEyeDistance2 * 2;

                FormBase._HeadLocationCurrent.EyeL = v;

                this.updatePinkTV2();
            }
        }

        private void updatePinkTV2()
        {
            Vector3 v = FormBase._HeadLocationCurrent.EyeM;

            this.panelPinkTV2.Left = (int)
                ((v.X * this.panelPinkTV.Width / Bobcat._TV_Width_Inches) + ((this.panelPinkTV.Width - this.panelPinkTV2.Width) / 2));

            this.panelPinkTV2.Top = (int)
                ((-v.Y * this.panelPinkTV.Height / Bobcat._TV_Height_Inches) + ((this.panelPinkTV.Height - this.panelPinkTV2.Height) / 2));
        }

        


















        private void radioButtonEnv_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton r = sender as RadioButton;

            if (r != null && r.Checked)
            {
                bool bNorm = r == this.radioButtonEnvNorm;
                bool bOutside = r == this.radioButtonEnvOutside;
                bool bShadowBuffer = r == this.radioButtonEnvShadowBuffer;
                bool bShadowBufferGraphics = r == this.radioButtonEnvShadowBufferGraphics;
                bool bStereo = r == this.radioButtonEnvStereo;
                bool bVertSync = r == this.radioButtonEnvVertSync;
                bool bLaces = r == this.radioButtonEnvLaces;
                bool bRift = r == this.radioButtonEnvRift;
                bool bCat = r == this.radioButtonEnvCat;
                bool bCatStereo = r == this.radioButtonEnvCatStereo;

                this.numericUpDownOutsideDistance.Enabled = bOutside;
                FormBase._HeadTrackerDraw = bOutside;

                this.numericUpDownEyeSeperation.Enabled =
                    !bOutside && !bNorm && !bCat && !bShadowBuffer && !bShadowBufferGraphics;

                this.numericUpDownHeadPitch.Enabled =
                    !bCat && !bCatStereo && !bShadowBuffer && !bShadowBufferGraphics;

                this.numericUpDownHeadYaw.Enabled =
                    !bCat && !bCatStereo && !bShadowBuffer && !bShadowBufferGraphics;

                this.numericUpDownHeadX.Enabled = !bShadowBuffer;
                this.numericUpDownHeadY.Enabled = !bShadowBuffer;
                this.numericUpDownHeadZ.Enabled = !bShadowBuffer;

                if (bNorm) this._GL_Handler._DrawType = GL_Handler.DrawType.Normal;

                else if (bOutside) this._GL_Handler._DrawType = GL_Handler.DrawType.Outside;
                else if (bShadowBuffer) this._GL_Handler._DrawType = GL_Handler.DrawType.ShadowBuffer;
                else if (bShadowBufferGraphics) this._GL_Handler._DrawType = GL_Handler.DrawType.ShadowBufferGraphics;

//                else if (bStereo) this._GL_Handler._DrawType = GL_Handler.DrawType.Stereo;
                else if (bVertSync) this._GL_Handler._DrawType = GL_Handler.DrawType.CatFlipFlop;
                else if (bLaces) this._GL_Handler._DrawType = GL_Handler.DrawType.CatLaces;

                else if (bRift) this._GL_Handler._DrawType = GL_Handler.DrawType.Rift;
                else if (bCat) this._GL_Handler._DrawType = GL_Handler.DrawType.Cat;
                else if (bCatStereo) this._GL_Handler._DrawType = GL_Handler.DrawType.CatSplitVertical;

            }
        }

        private void panelOuterGL_Resize(object sender, EventArgs e)
        {
            this.timerResize.Stop();
            this.timerResize.Start();
        }

        private void checkBoxAspectRatio_CheckedChanged(object sender, EventArgs e)
        {
            this.timerResize_Tick(sender, e);
        }

        private void timerResize_Tick(object sender, EventArgs e)
        {
            if (this.checkBoxAspectRatio.Checked)
            {
                Size A = new Size(9, 16);
                Size B = this.panelOuterGL.Size;

                Rectangle _Rectangle = new Rectangle(0, 0, 10, 10);

                if (A.Width * A.Height == 0) { }
                else if (A.Width * B.Height > B.Width * A.Height)
                {
                    _Rectangle.Width = B.Width;
                    _Rectangle.Height = (B.Width * A.Height) / A.Width;
                    _Rectangle.Y = (B.Height - _Rectangle.Height) / 2;
                }
                else
                {
                    // image taller than screen
                    _Rectangle.Width = (B.Height * A.Width) / A.Height;
                    _Rectangle.Height = B.Height;
                    _Rectangle.X = (B.Width - _Rectangle.Width) / 2;
                }

                this.panelInnerGL.Left = _Rectangle.X;
                this.panelInnerGL.Top = _Rectangle.Y;
                this.panelInnerGL.Size = _Rectangle.Size;
            }
            else
            {
                this.panelInnerGL.Left = 0;
                this.panelInnerGL.Top = 0;
                this.panelInnerGL.Size = this.panelOuterGL.Size;
            }
            this.timerResize.Stop();
        }






































        private void loadMedia()
        {
            int i = 0;
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "skybox.xml", SearchOption.AllDirectories);
            foreach (var file in files) this.comboBoxSkyBox.Items.Add(new SkyBox(file, ++i));
            if (this.comboBoxSkyBox.Items.Count > 0) this.comboBoxSkyBox.SelectedIndex = 0;

            this._GL_Handler._SkyBox = this.comboBoxSkyBox.SelectedItem as SkyBox;
        }

        private void comboBoxSkyBox_SelectedValueChanged(object sender, EventArgs e)
        {
            foreach (var item in this.comboBoxSkyBox.Items)
            {
                if (item == this.comboBoxSkyBox.SelectedItem)
                {
                    this._GL_Handler._SkyBox = item as SkyBox;
                }
                else
                {
                    var sb = item as SkyBox;
                    if (sb != null) sb.GLDelete();
                }
            }
        }

        internal static float _FloatSkyBoxDim = 1;
        private void numericUpDownSkyBoxDim_ValueChanged(object sender, EventArgs e)
        {
            FormBase._FloatSkyBoxDim = (float) this.numericUpDownSkyBoxDim.Value;

            if (this._GL_Handler != null)
            {
                this._GL_Handler.SkyBoxDimChanged();
            }
        }














        internal void removeCadItem(CadObject item)
        {
            this.cadHandler1.checkedListBox1.Items.Remove(item);
        }

        internal void removeCadItems(CadObject[] list)
        {
            foreach (CadObject co in list) this.removeCadItem(co);
        }

        internal void addCadItem(CadObject item)
        {
            int count = this.cadHandler1.checkedListBox1.Items.Count;
            this.cadHandler1.checkedListBox1.Items.Add(item);
            this.cadHandler1.checkedListBox1.SetItemChecked(count, item._Display);
        }

        internal void addCadItems(CadObject[] list)
        {
            foreach (CadObject co in list) this.addCadItem(co);
        }



























        private void buttonTrialNew_Click(object sender, EventArgs e)
        {
            Trial.SetupNone();
            this.RefreshTrial(null);
            new FormPickTrial().ShowDialog();
        }

        private void buttonTrialReset_Click(object sender, EventArgs e)
        {
        }

        private void checkBoxTrialPause_CheckedChanged(object sender, EventArgs e)
        {
        }














































        private bool dragging = false;
        private System.Drawing.Size mouseOffset;

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseOffset = new System.Drawing.Size(e.Location);
            dragging = true;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                System.Drawing.Point newLocationOffset = e.Location - mouseOffset;

                if (!this.checkBoxHeadTrack.Checked)
                {
                    StaticMethods.setNudValue(this.numericUpDownHeadX, - (decimal) (((this.panelPinkTV.Width / 2) -
                        (this.panelPinkTV2.Left + newLocationOffset.X + this.panelPinkTV2.Width / 2)) * (Bobcat._TV_Width_Inches / this.panelPinkTV.Width)));

                    StaticMethods.setNudValue(this.numericUpDownHeadY, (decimal) (((this.panelPinkTV.Height / 2) -
                        (this.panelPinkTV2.Top + newLocationOffset.Y + this.panelPinkTV2.Height / 2)) * (Bobcat._TV_Height_Inches / this.panelPinkTV.Height)));
                }
            }
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            this.numericUpDownHeadX.Value = 0;
            this.numericUpDownHeadY.Value = 0;
        }






























        public static bool _HeadTrackerDraw = false;
        public static volatile float _HeadTrackerX;
        public static volatile float _HeadTrackerY;
        public static volatile float _HeadTrackerZ;
        public static volatile float _HeadTrackerO_sin;
        public static volatile float _HeadTrackerO_cos;

        public static volatile float _HeadEyeDistance2 = 0;

        private void checkBoxHeadTrack_Click(object sender, EventArgs e)
        {
            if (this.checkBoxHeadTrack.Checked)
            {
                HeadTrackerManager.Stop();
            }
            else
            {
                HeadTrackerManager.Start();
            }
        }

        private void HeadTrackStart() // Called By Head Tracking Thread
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.checkBoxHeadTrack.Checked = true;
            });
        }

        private void HeadTrackEnd() // Called By Head Tracking Thread
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.checkBoxHeadTrack.Checked = false;
            });
        }

        private void checkBoxHeadTrack_CheckedChanged(object sender, EventArgs e)
        {
            bool b = this.checkBoxHeadTrack.Enabled;
            this.numericUpDownHT_X.Enabled = b;
            this.numericUpDownHT_Y.Enabled = b;
            this.numericUpDownHT_Z.Enabled = b;
            this.numericUpDownHT_O.Enabled = b;
        }

        private void numericUpDownEyeSeperation_ValueChanged(object sender, EventArgs e)
        {
            FormBase._HeadEyeDistance2 = (float) this.numericUpDownEyeSeperation.Value;
            if (!this.checkBoxHeadTrack.Checked) this.numericUpDownHead_ValueChanged(sender, e);
        }

        private void numericUpDownHT_X_ValueChanged(object sender, EventArgs e)
        {
            FormBase._HeadTrackerX = (float)this.numericUpDownHT_X.Value;
            this.setHeadTrackerLocation();
        }

        private void numericUpDownHT_Y_ValueChanged(object sender, EventArgs e)
        {
            FormBase._HeadTrackerY = (float)this.numericUpDownHT_Y.Value;
            this.setHeadTrackerLocation();
        }

        private void numericUpDownHT_Z_ValueChanged(object sender, EventArgs e)
        {
            FormBase._HeadTrackerZ = (float)this.numericUpDownHT_Z.Value;
            this.setHeadTrackerLocation();
        }

        private void numericUpDownHT_O_ValueChanged(object sender, EventArgs e)
        {
            double rads = StaticMethods.toRadiansD((double)this.numericUpDownHT_O.Value);
            FormBase._HeadTrackerO_sin = (float)Math.Sin(rads);
            FormBase._HeadTrackerO_cos = (float)Math.Cos(rads);
            this.setHeadTrackerLocation();
        }

        private void setHeadTrackerLocation()
        {
            Bobcat._SensorLocationTV = FormBase.convertToTVFromHeadTracker(0, 0, 0);
            Bobcat._SensorDirectionTV = FormBase.convertToTVFromHeadTracker(0, 0, 100);
        }

        private static void GotHeadData(HeadTrackerManager.HeadTrackerData dat) // Called By Head Tracking Thread
        {
            FormBase._HeadLocationOther.EyeM = FormBase.convertToTVFromHeadTracker(dat.Head);

            Vector3 eye = Vector3.Multiply(dat.Eyes, FormBase._HeadEyeDistance2 * 25.4f);

            FormBase._HeadLocationOther.EyeR = FormBase.convertToTVFromHeadTracker(Vector3.Add(dat.Head, eye));
            FormBase._HeadLocationOther.EyeL = FormBase.convertToTVFromHeadTracker(Vector3.Subtract(dat.Head, eye));

            FormBase._HeadLocationOther.Reflect1 = FormBase.convertToTVFromHeadTracker(dat.Sensor1);
            FormBase._HeadLocationOther.Reflect2 = FormBase.convertToTVFromHeadTracker(dat.Sensor2);
            FormBase._HeadLocationOther.Reflect3 = FormBase.convertToTVFromHeadTracker(dat.Sensor3);

            FormBase._HeadLocationOther.EyeR.Z = Math.Max(2, FormBase._HeadLocationOther.EyeR.Z);
            FormBase._HeadLocationOther.EyeL.Z = Math.Max(2, FormBase._HeadLocationOther.EyeL.Z);
            FormBase._HeadLocationOther.EyeM.Z = Math.Max(2, FormBase._HeadLocationOther.EyeM.Z);

            HeadLocationData temp = FormBase._HeadLocationCurrent;
            FormBase._HeadLocationCurrent = FormBase._HeadLocationOther;
            FormBase._HeadLocationOther = temp;

            FormBase.Instance.BeginInvoke((MethodInvoker)delegate
            {
                FormBase.Instance.updatePinkTV2();
            });
        }

        public static volatile HeadLocationData _HeadLocationCurrent = new HeadLocationData();
        private static volatile HeadLocationData _HeadLocationOther = new HeadLocationData();
        public class HeadLocationData
        {
            public Vector3 EyeM = new Vector3();
            public Vector3 EyeR = new Vector3();
            public Vector3 EyeL = new Vector3();

            public Vector3 Reflect1 = new Vector3();
            public Vector3 Reflect2 = new Vector3();
            public Vector3 Reflect3 = new Vector3();
        }

        private static Vector3 convertToTVFromHeadTracker(Vector3 v) { return FormBase.convertToTVFromHeadTracker(v.X, v.Y, v.Z); }
        private static Vector3 convertToTVFromHeadTracker(float x, float y, float z)
        {
            const float fdev = 1 / 25.4f;

            x *= fdev;
            y *= fdev;
            z *= fdev;

            return new Vector3(
                FormBase._HeadTrackerX + x,
                FormBase._HeadTrackerY + y * FormBase._HeadTrackerO_cos + z * FormBase._HeadTrackerO_sin,
                FormBase._HeadTrackerZ + z * FormBase._HeadTrackerO_cos - y * FormBase._HeadTrackerO_sin);
        }


        private void checkBoxMute_CheckedChanged(object sender, EventArgs e)
        {
            ExcavatorSound.Mute = this.checkBoxMute.Checked;
        }























        private void checkBoxCabRotater_Click(object sender, EventArgs e)
        {
            if (this.checkBoxCabRotater.Checked) CabRotater.Kill();
            else CabRotater.Start();
        }

        private void CabRotater_RotateStarted() // Called By Head Tracking Thread
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.checkBoxCabRotater.Checked = true;
            });
        }

        private void CabRotater_RotateEnded() // Called By Head Tracking Thread
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.checkBoxCabRotater.Checked = false;
            });
        }


        public static bool _BoolDrawPhysX { get; private set; }
        private void checkBoxPhysX_CheckedChanged(object sender, EventArgs e)
        {
            FormBase._BoolDrawPhysX = this.checkBoxPhysX.Checked;
        }



















        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Trial.f1 = (float)this.numericUpDown1.Value;

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Trial.f2 = (float)this.numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Trial.f3 = (float)this.numericUpDown3.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Trial.f4 = (float)this.numericUpDown4.Value;
        }

        private void buttonResetSoil_Click(object sender, EventArgs e)
        {
            Trial.ResetSoilModelS();

        }

        private void nudCabK_ValueChanged(object sender, EventArgs e)
        {
            CabRotater.setGainValues(
                (float)this.nudCabKp.Value,
                (float)this.nudCabKd.Value);
        }

        private void textBoxSavePath_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(this.textBoxSavePath.Text))
            {
                this.textBoxSavePath.ForeColor = Color.Black;
                Properties.Settings.Default.SavePath = this.textBoxSavePath.Text;
            }
            else this.textBoxSavePath.ForeColor = Color.Red;

            this.nudSessionSubject_ValueChanged(sender, e);
        }

        private void buttonSavePath_Click(object sender, EventArgs e)
        {
            switch(this.fbdSavePath.ShowDialog())
            {
                case System.Windows.Forms.DialogResult.OK:
                    this.textBoxSavePath.Text = this.fbdSavePath.SelectedPath;
                    break;
            }

        }

        private string getSavePath()
        {
            return Path.Combine(new String[]{this.textBoxSavePath.Text,
                "Subject_" + ((int)(this.nudSubject.Value)).ToString("00"),  
                "Session_" + ((int)(this.nudSession.Value)).ToString()});
        }

        private void nudSessionSubject_ValueChanged(object sender, EventArgs e)
        {
            if ((this.nudSubject.Value > 0) && (this.nudSession.Value > 0) && !Directory.Exists(this.getSavePath()))
            {
                this.buttonBeginSave.Enabled = true;

                int subj = (int)(this.nudSubject.Value);
                int sess = (int)(this.nudSession.Value);

                this.numericUpDownEyeSeperation.Value = (((subj + sess) % 2 == 0) || sess == 3) ? Config.EyeDist : 0;
                this.numericUpDownEyeSeperation_ValueChanged(sender, e);
            }
            else
            {
                this.buttonBeginSave.Enabled = false;
            }
        }

        private void buttonBeginSave_Click(object sender, EventArgs e)
        {
            if (TrialSaver.FromFile(this.getSavePath()))
            {
                this.buttonBeginSave.Enabled = false;
                Console.WriteLine("Starting Save");
            }
            else
            {
                Console.WriteLine("Save File Error");
            }

        }
    }
}
