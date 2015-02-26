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


namespace Excavator
{
    public partial class FormBase : Form
    {
        private GLControl3D _GLControl3D;

        internal static FormBase _FormBase;

        private static volatile int _IntThreadCount = 0;
        internal static volatile bool _BoolThreadAlive = true;

        public FormBase()
        {
            this.InitializeComponent();
            FormBase._FormBase = this;

            new TrialJointControlKeyboard(this);
        }













        internal void loadTrial(Trial t)
        {
            this.panelRight.Controls.Remove(Trial._Trial);
            this.panelRight.Controls.Add(t);
            this.panelRight.Controls.SetChildIndex(t, 0);

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
            this._GLControl3D.setControl(this._FormFullScreen);
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
                    this._GLControl3D.setControl(this.panelInnerGL);
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
            FormBase f = FormBase._FormBase;
            f.Invoke((MethodInvoker)delegate
            {
                FormBase._IntThreadCount++;
                f.backgroundThreadsChanged();
            });
        }

        internal static void subThread()
        {
            FormBase f = FormBase._FormBase;
            f.Invoke((MethodInvoker) delegate
            {
                FormBase._IntThreadCount--;
                if (FormBase._IntThreadCount == 0 && !FormBase._BoolThreadAlive) f.Close();
                else f.backgroundThreadsChanged();
            });
        }

        private void backgroundThreadsChanged()
        {
        }

        private void FormBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormBase._IntThreadCount > 0) e.Cancel = true;
            else
            {
                Properties.Settings.Default.Save();
                Trial._Trial.Deconstruct();
            }
            FormBase._BoolThreadAlive = false;
            FormBase._FormBase.Enabled = false;
        }

        private void checkBoxShowSide_CheckedChanged(object sender, EventArgs e)
        {
            if (this._GLControl3D == null) return;
            this._GLControl3D._BoolShowSide = this.checkBoxShowSide.Checked;
        }

        private void buttonFullScreen_Click(object sender, EventArgs e)
        {
            FormPickScreen f = new FormPickScreen();
            f.ShowDialog();
        }





























        private void Application_Idle(object sender, EventArgs e)
        {
            if (FormBase._BoolThreadAlive)
            {
                if (this._GLControl3D.IsIdle)
                {
                    this._GLControl3D.Invalidate();
                }
            }
        }






























        private GLControl _GLControlTemp;

        private void MainForm_Load(object sender, EventArgs e)
        {            
            GraphicsMode g = GraphicsMode.Default;
            Console.WriteLine("Defaults - Depth: " + g.Depth + ", Stencil: " + g.Stencil);
            g = new GraphicsMode(g.ColorFormat, 32, 0, g.Samples, g.ColorFormat, 1, true);
            this._GLControlTemp = new GLControl(g);
            this._GLControlTemp.Load += new EventHandler(this._GLControlTemp_Load);
            this.panelOuterGL.Controls.Add(this._GLControlTemp);
        }

        private void _GLControlTemp_Load(object sender, EventArgs e)
        {
            bool b3d = this._GLControlTemp.GraphicsMode.Stereo;
            Console.WriteLine("3D Enabled: " + b3d);
            this.panelOuterGL.Controls.Remove(this._GLControlTemp);
            this._GLControlTemp.Dispose();
            this._GLControlTemp = null;

            this.radioButtonEnvStereo.Enabled = b3d;

            this.panelOuterGL_Resize(sender, e);

            this._GLControl3D = new GLControl3D(this.panelInnerGL);

            GlobalEventHandler.KeyDown += new EventHandler(this.GlobalKeyDown);
            GlobalEventHandler.LMouseDrag += new MouseEventHandler(this.GlobalMouseDrag);
            GlobalEventHandler.LMouseDown += new EventHandler(this.GlobalMouseClick);

            this.numericUpDownHead_ValueChanged(sender, e);

            this.numericUpDownSkyBoxDim_ValueChanged(sender, e);

            this.backgroundThreadsChanged();
            
            this.loadMedia();

            Application.Idle += new EventHandler(this.Application_Idle);

            this.Enabled = true;
        }















        private void Head_ValueChanged(object sender, EventArgs e)
        {
            if (this._GLControl3D == null) return;
            
            decimal value = this.numericUpDownHeadYaw.Value;
            if (Math.Abs(value) == 180.1m)
            {
                value = 179.9m * (value > 0 ? -1 : 1);
                this.numericUpDownHeadYaw.Value = value;
            }

            this._GLControl3D.setHeadYawAndPitch(
                (float)this.numericUpDownHeadYaw.Value,
                (float)this.numericUpDownHeadPitch.Value);
        }

        private void numericUpDownHead_ValueChanged(object sender, EventArgs e)
        {
            float sx = (float)this.numericUpDownHeadX.Value;
            float sy = (float)this.numericUpDownHeadY.Value;
            float sz = (float)this.numericUpDownHeadZ.Value;

            float ed = (float)this.numericUpDownEyeSeperation.Value;

            Vector3 vr = new Vector3();
            Vector3 vl = new Vector3();
            Vector3 vm = new Vector3();

            for (int i = 0; i < 3; i++)
            {
                float relX = sx - Bobcat._TV_V3_Lift.X;
                float relY = sy - Bobcat._TV_V3_Lift.Y;
                float relZ = sz - Bobcat._TV_V3_Lift.Z;

                switch (i)
                {
                    case 0: relX += ed; break;
                    case 1: relX -= ed; break;
                }

                relX /= Bobcat._TV_Width_Inches / 2;

                float k = (float)Math.Sqrt(relY * relY + relZ * relZ);
                float th1 = (float)Math.Atan2(relY, relZ);
                float th2 = StaticMethods.toRadiansF(Bobcat._TV_Float_Rotate);
                float th3 = StaticMethods._PIF / 2 - th1 - th2;
                float fsy = (float)Math.Cos(th3);
                float fsd = (float)Math.Sin(th3);

                fsy *= k;
                fsd *= k;

                relY = -fsy * 2 / Bobcat._TV_Height_Inches;

                fsd /= Bobcat._TV_Height_Inches;

                switch (i)
                {
                    case 0:
                        {
                            vr = new Vector3(relX, relY, fsd);
                            break;
                        }
                    case 1:
                        {
                            vl = new Vector3(relX, relY, fsd);
                            break;
                        }
                    case 2:
                        {
                            this.panel2.Left = (int)((this.panel1.Width * (relX + 1) - this.panel2.Width) / 2);
                            this.panel2.Top = (int)((this.panel1.Height * (relY + 1) - this.panel2.Height) / 2);

                            vm = new Vector3(relX, relY, fsd);
                            break;
                        }
                }
            }

            this._GLControl3D.setViewEyePositionInCab(
                ed,
                (float) this.numericUpDownFocalDistance.Value,
                new Vector3(sx, sy, sz),
                vr,
                vl,
                vm);
        }











        private void numericUpDownCabRotation_ValueChanged(object sender, EventArgs e)
        {
            decimal value = this.numericUpDownCabRotation.Value;
            if (Math.Abs(value) == 180.1m)
            {
                value = 179.9m * (value > 0 ? -1 : 1);
                this.numericUpDownCabRotation.Value = value;
            }
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
                bool bSplitVert = r == this.radioButtonEnvSplitVert;
                bool bLaces = r == this.radioButtonEnvLaces;
                bool bRift = r == this.radioButtonEnvRift;
                bool bCat = r == this.radioButtonEnvCat;
                bool bCatStereo = r == this.radioButtonEnvCatStereo;
                              
                this.numericUpDownEyeSeperation.Enabled =
                    !bOutside && !bNorm && !bCat && !bShadowBuffer && !bShadowBufferGraphics;
                this.numericUpDownFocalDistance.Enabled = this.numericUpDownEyeSeperation.Enabled && !bCatStereo;

                this.numericUpDownHeadPitch.Enabled =
                    !bCat && !bCatStereo && !bShadowBuffer && !bShadowBufferGraphics;

                this.numericUpDownHeadYaw.Enabled =
                    !bCat && !bCatStereo && !bShadowBuffer && !bShadowBufferGraphics;

                this.numericUpDownHeadX.Enabled = !bShadowBuffer;
                this.numericUpDownHeadY.Enabled = !bShadowBuffer;
                this.numericUpDownHeadZ.Enabled = !bShadowBuffer;

                if (bNorm) this._GLControl3D._DrawType = GLControl3D.DrawType.Normal;

                else if (bOutside) this._GLControl3D._DrawType = GLControl3D.DrawType.Outside;
                else if (bShadowBuffer) this._GLControl3D._DrawType = GLControl3D.DrawType.ShadowBuffer;
                else if (bShadowBufferGraphics) this._GLControl3D._DrawType = GLControl3D.DrawType.ShadowBufferGraphics;

                else if (bStereo) this._GLControl3D._DrawType = GLControl3D.DrawType.Stereo;
                else if (bVertSync) this._GLControl3D._DrawType = GLControl3D.DrawType.VerticalSync;
                else if (bSplitVert) this._GLControl3D._DrawType = GLControl3D.DrawType.SplitVertical;
                else if (bLaces) this._GLControl3D._DrawType = GLControl3D.DrawType.Laces;

                else if (bRift) this._GLControl3D._DrawType = GLControl3D.DrawType.Rift;
                else if (bCat) this._GLControl3D._DrawType = GLControl3D.DrawType.Cat;
                else if (bCatStereo) this._GLControl3D._DrawType = GLControl3D.DrawType.CatStereo;

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

                if (A.Width * A.Height == 0) ;
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

            if (this._GLControl3D != null)
                this._GLControl3D._SkyBox = this.comboBoxSkyBox.SelectedItem as SkyBox;
        }

        private void comboBoxSkyBox_SelectedValueChanged(object sender, EventArgs e)
        {
            foreach (var item in this.comboBoxSkyBox.Items)
            {
                if (item == this.comboBoxSkyBox.SelectedItem)
                {
                    this._GLControl3D._SkyBox = item as SkyBox;
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

            if (this._GLControl3D != null)
            {
                this._GLControl3D.SkyBoxDimChanged();
            }
        }















        internal void resetGL()
        {
            foreach (object o in this.cadHandler1.checkedListBox1.Items)
            {
                CadObject co = o as CadObject;
                if (co != null)
                {
                    co.GLDelete();
                }
            }

            SkyBox sb = this.comboBoxSkyBox.SelectedItem as SkyBox;
            if (sb != null) sb.GLDelete();
        }

        internal void addCadItems(CadObject[] list)
        {
            foreach (CadObject co in list)
            {
                int count = this.cadHandler1.checkedListBox1.Items.Count;
                this.cadHandler1.checkedListBox1.Items.Add(co);
                this.cadHandler1.checkedListBox1.SetItemChecked(count, co._Display);
            }
        }



























        private void buttonTrialNew_Click(object sender, EventArgs e)
        {
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
                this.panel2.Left += newLocationOffset.X;
                this.panel2.Top += newLocationOffset.Y;
            }
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            this.panel2.Left = (this.panel1.Width - this.panel2.Width) / 2;
            this.panel2.Top = (this.panel1.Height - this.panel2.Height) / 2;
        }
















        internal static float f1 = 0;
        internal static float f2 = 0;
        internal static float f3 = 0;
        internal static float f4 = 0;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            FormBase.f1 = (float)(this.numericUpDown1.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            FormBase.f2 = (float)(this.numericUpDown2.Value);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            FormBase.f3 = (float)(this.numericUpDown3.Value);
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            FormBase.f4 = (float)(this.numericUpDown4.Value);
        }


    }
}
