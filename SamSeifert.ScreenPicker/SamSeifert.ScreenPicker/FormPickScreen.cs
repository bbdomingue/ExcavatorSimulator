using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;

namespace SamSeifert.ScreenPicker
{
    public partial class FormPickScreen : Form
    {
        public static bool SetupFormGood = false;

        private static Point _FormPoint = Point.Empty;
        private static Size _FormSize = Size.Empty;

        private static DisplayDevice _DisplayDevice = null;

        public static void SetupForm(Form f)
        {
            f.StartPosition = FormStartPosition.Manual;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Location = FormPickScreen._FormPoint;
            f.Size = FormPickScreen._FormSize;

            f.FormClosed += delegate(object sender, FormClosedEventArgs e)
            {
                FormPickScreen._DisplayDevice.RestoreResolution();
            };  

            f.ShowInTaskbar = false;

            f.Show();
        }







        private int _ScreensIndex = 0;

        private DisplayDevice[] _Screens;
        private Panel[] _Panels;

        const int _IntScale = 10;
        static Color _ColorSelected = Color.Green;
        static Color _ColorSelectedNot = Color.White;

        public FormPickScreen()
        {
            FormPickScreen.SetupFormGood = false;
            InitializeComponent();
        }



        private void FormFullScreenPick_Load(object sender, EventArgs e)
        {
            var list = DisplayDevice.AvailableDisplays;

            int count = list.Count;

            this._Screens = new DisplayDevice[count];
            this._Panels = new Panel[count];

            for (int c = 0; c < count; c++)
            {
                DisplayDevice d = list[c];
                Panel p = new Panel();

                p.Size = new Size(d.Width / FormPickScreen._IntScale, d.Height / FormPickScreen._IntScale);
                p.BackColor = FormPickScreen._ColorSelectedNot;               
                p.BorderStyle = BorderStyle.FixedSingle;

                p.Click += new EventHandler(this.screenClick);
                this.panel1.Controls.Add(p);

                this._Screens[c] = d;
                this._Panels[c] = p;
            }

            this.panel1_Resize(sender, e);
        }



        private void panel1_Resize(object sender, EventArgs e)
        {
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;

            foreach (DisplayDevice d in this._Screens)
            {
                maxX = Math.Max(maxX, d.Bounds.X + d.Width);
                minX = Math.Min(minX, d.Bounds.X);
                maxY = Math.Max(maxY, d.Bounds.Y + d.Height);
                minY = Math.Min(minY, d.Bounds.Y);
            }


            int left = (this.panel1.Width / 2) - ((minX + maxX) / (2 * FormPickScreen._IntScale));
            int top = (this.panel1.Height / 2) - ((minY + maxY) / (2 * FormPickScreen._IntScale));

            int count = Math.Min(this._Panels.Length, this._Screens.Length);

            for (int c = 0; c < count; c++)
            {
                DisplayDevice d = this._Screens[c];
                Panel p = this._Panels[c];

                p.Left = left + (d.Bounds.X / FormPickScreen._IntScale);
                p.Top = top + (d.Bounds.Y / FormPickScreen._IntScale);
            }
        }




        private void screenClick(object sender, EventArgs e)
        {
            int count = Math.Min(this._Panels.Length, this._Screens.Length);

            for (int c = 0; c < count; c++)
            {
                DisplayDevice d = this._Screens[c];
                Panel p = this._Panels[c];

                if (p == sender)
                {

                    p.BackColor = FormPickScreen._ColorSelected;

                    var list2 = d.AvailableResolutions;
                    var list3 = new List<Resolution>();

                    int count2 = list2.Count;

                    for (int c2 = 0; c2 < count2; c2++)
                    {
                        Resolution r2 = new Resolution(list2[c2]);
                        bool add = true;

                        if (d.BitsPerPixel == r2._DisplayResolution.BitsPerPixel)
                        {
                            foreach (Resolution r3 in list3)
                            {
                                if (r2.Equals(r3))
                                {
                                    add = false;
                                    break;
                                }
                            }
                            if (add) list3.Add(r2);
                        }
                    }

                    list3.Sort();

                    this._ScreensIndex = c;

                    this.comboBox1.Items.Clear();
                    this.comboBox1.Items.AddRange(list3.ToArray());

                    int index = 0;

                    foreach (Resolution r in list3)
                    {
                        if (r._DisplayResolution.Height == d.Height &&
                            r._DisplayResolution.Width == d.Width &&
                            r._DisplayResolution.RefreshRate == d.RefreshRate) break;

                        index++;
                    }

                    if (this.comboBox1.Items.Count > 0)
                    {
                        this.buttonGo.Enabled = true;
                        this.comboBox1.Enabled = true;
                        this.comboBox1.SelectedIndex = index;
                        this.comboBox1.Focus();
                    }
                    else
                    {
                        this.buttonGo.Enabled = false;
                        this.comboBox1.Enabled = false;
                    }
                }
                else
                {
                    p.BackColor = FormPickScreen._ColorSelectedNot;
                }
            }
        }




        private class Resolution : IComparable<Resolution>
        {
            public DisplayResolution _DisplayResolution;

            public Resolution(DisplayResolution d) { this._DisplayResolution = d; }

            public override string ToString()
            {
                return this._DisplayResolution.RefreshRate + " Hz @ " +
                    this._DisplayResolution.Width + " x " + this._DisplayResolution.Height;
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;

                Resolution r = obj as Resolution;
                if ((System.Object) r == null)  return false;

                return
                    r._DisplayResolution.Width == this._DisplayResolution.Width &&
                    r._DisplayResolution.Height == this._DisplayResolution.Height &&
                    r._DisplayResolution.RefreshRate == this._DisplayResolution.RefreshRate;
            }

            private int area() { return this._DisplayResolution.Width * this._DisplayResolution.Height; }

            public int CompareTo(Resolution other)
            {
                int c;
                
                c = (int)(other._DisplayResolution.RefreshRate - this._DisplayResolution.RefreshRate);
                if (c != 0) return c;

                return other.area() - this.area();
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }




        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        public static int _BottomPix = 0;

        private void buttonGo_Click(object sender, EventArgs e)
        {
            Resolution r = this.comboBox1.SelectedItem as Resolution;

            if (r != null)
            {                
                FormPickScreen._DisplayDevice = this._Screens[this._ScreensIndex];
                FormPickScreen._DisplayDevice.ChangeResolution(r._DisplayResolution);

                const int inc = 0; // Doesnt work at 0 for some unknown reason

                FormPickScreen._FormPoint = new Point(
                    FormPickScreen._DisplayDevice.Bounds.Location.X + inc,
                    FormPickScreen._DisplayDevice.Bounds.Location.Y);

                FormPickScreen._FormSize = new Size(
                    r._DisplayResolution.Width - inc * 2,
                    r._DisplayResolution.Height - FormPickScreen._BottomPix);

                FormPickScreen.SetupFormGood = true;

                this.Close();
            }
        }


        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.buttonGo_Click(sender, e);
            }
        }
    }
}
