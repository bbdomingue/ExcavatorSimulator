using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SamSeifert.GLE.CadViewer
{
    public partial class CadHandler : UserControl
    {
        public static bool _BoolAllowDelete = true;
        public static bool _BoolAllowNew = true;
        public static bool _BoolAllowSaveAs = true;

        private readonly NumericUpDown[] _NumericUpDowns;
        private bool _BoolManualChangeValues = false;

        public CadHandler()
        {
            InitializeComponent();

            this._NumericUpDowns = new NumericUpDown[]
            {
                this.numericUpDownMat11,
                this.numericUpDownMat21,
                this.numericUpDownMat31,
                this.numericUpDownMat41,
                this.numericUpDownMat12,
                this.numericUpDownMat22,
                this.numericUpDownMat32,
                this.numericUpDownMat42,
                this.numericUpDownMat13,
                this.numericUpDownMat23,
                this.numericUpDownMat33,
                this.numericUpDownMat43,
                this.numericUpDownMat14,
                this.numericUpDownMat24,
                this.numericUpDownMat34,
                this.numericUpDownMat44,
                this.numericUpDownMat15
            };

            this.buttonAddPart.Enabled = CadHandler._BoolAllowNew;
            this.buttonDeletePart.Enabled = CadHandler._BoolAllowDelete;
            this.buttonSaveAs.Enabled = CadHandler._BoolAllowSaveAs;
        }

        public void GLDelete()
        {
            foreach (object o in this.checkedListBox1.Items)
            {
                var co = o as CadObject;
                if (co != null) co.GLDelete();
            }
        }

        public void Setup()
        {
            this.checkedListBox1.SetItemCheckState(0, CheckState.Checked);
            for (int i = 0; i < CadHandler.LIGHT_COUNT; i++) this.setLight(i);
        }

        private void buttonAddPart_Click(object sender, EventArgs e)
        {
            new FormNewShape(this).ShowDialog();
        }

        private void buttonDeletePart_Click(object sender, EventArgs e)
        {
            int dex = this.checkedListBox1.SelectedIndex;
            bool dexG = dex < LIGHT_COUNT;

            object o = this.checkedListBox1.SelectedItem;

            if (!dexG)
            {
                if (o != null)
                {
                    var co = o as CadObject;
                    if (co != null) co.GLDelete();
                    this.checkedListBox1.Items.Remove(o);
                }
            }

        }

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            var o = this.checkedListBox1.SelectedItem;
            var co = o as CadObject;
            if (co != null) new FormSaveAs(co).ShowDialog();
        }

        public void addParts(CadObject[] list)
        {
            var cb = this.checkedListBox1;
            int cc = cb.Items.Count;
            cb.Items.AddRange(list);
            for (int i = 0; i < list.Length; i++) cb.SetItemCheckState(cc + i, CheckState.Checked);
        }


















        private const float DIFFUSE = 1.0f;
        private const float SPECULAR = 1.0f;
        private const float AMBIENT = 1.0f;

        public const int LIGHT_COUNT = 4;
        public float[][] Position = new float[][]{
            new float[]{0, 500, 500, 1},
            new float[]{0, 0, 0, 1},
            new float[]{0, 0, 0, 1},
            new float[]{0, 0, 0, 1}
        };

        public float[][] Diffuse = new float[][]{
            new float[]{DIFFUSE, DIFFUSE, DIFFUSE, 1},
            new float[]{0, 0, 0, 1},
            new float[]{0, 0, 0, 1},
            new float[]{0, 0, 0, 1}
        };

        public float[][] Specular = new float[][]{
            new float[]{SPECULAR, SPECULAR, SPECULAR, 1},
            new float[]{0, 0, 0, 1},
            new float[]{0, 0, 0, 1},
            new float[]{0, 0, 0, 1}
        };

        public float[][] Ambients = new float[][]{
            new float[]{AMBIENT, AMBIENT, AMBIENT, 1},
            new float[]{0, 0, 0, 1},
            new float[]{0, 0, 0, 1},
            new float[]{0, 0, 0, 1}
        };

        


        private void checkedListBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            this._BoolManualChangeValues = true;
            var ns = this._NumericUpDowns;
            int dex = this.checkedListBox1.SelectedIndex;
            bool dexG = dex < LIGHT_COUNT;

            bool isNull = this.checkedListBox1.SelectedItem == null;
            bool isCad = !isNull && !dexG;

            this.buttonDeletePart.Enabled = isCad && CadHandler._BoolAllowDelete;
            this.buttonSaveAs.Enabled =     isCad && CadHandler._BoolAllowSaveAs;

            foreach (NumericUpDown n in ns) n.Enabled = !isNull;

            ns[8].Enabled = isCad;
            ns[9].Enabled = isCad;
            ns[10].Enabled = isCad;
            ns[11].Enabled = isCad;
            ns[16].Enabled = isCad;



            if (dexG)
            {
                if (dex > -1)
                {
                    ns[0].Value = (decimal)this.Ambients[dex][0];
                    ns[1].Value = (decimal)this.Ambients[dex][1];
                    ns[2].Value = (decimal)this.Ambients[dex][2];
                    ns[3].Value = (decimal)this.Ambients[dex][3];
                    ns[4].Value = (decimal)this.Diffuse[dex][0];
                    ns[5].Value = (decimal)this.Diffuse[dex][1];
                    ns[6].Value = (decimal)this.Diffuse[dex][2];
                    ns[7].Value = (decimal)this.Diffuse[dex][3];
                    ns[8].Value = 0;
                    ns[9].Value = 0;
                    ns[10].Value = 0;
                    ns[11].Value = 0;
                    ns[12].Value = (decimal)this.Specular[dex][0];
                    ns[13].Value = (decimal)this.Specular[dex][1];
                    ns[14].Value = (decimal)this.Specular[dex][2];
                    ns[15].Value = (decimal)this.Specular[dex][3];
                    ns[16].Value = 0;
                }
            }
            else
            {
                var co = this.checkedListBox1.SelectedItem as CadObject;
                if (co != null)
                {
                    ns[0].Value = (decimal)co._Ambient[0];
                    ns[1].Value = (decimal)co._Ambient[1];
                    ns[2].Value = (decimal)co._Ambient[2];
                    ns[3].Value = (decimal)co._Ambient[3];
                    ns[4].Value = (decimal)co._Diffuse[0];
                    ns[5].Value = (decimal)co._Diffuse[1];
                    ns[6].Value = (decimal)co._Diffuse[2];
                    ns[7].Value = (decimal)co._Diffuse[3];
                    ns[8].Value = (decimal)co._Emission[0];
                    ns[9].Value = (decimal)co._Emission[1];
                    ns[10].Value = (decimal)co._Emission[2];
                    ns[11].Value = (decimal)co._Emission[3];
                    ns[12].Value = (decimal)co._Specular[0];
                    ns[13].Value = (decimal)co._Specular[1];
                    ns[14].Value = (decimal)co._Specular[2];
                    ns[15].Value = (decimal)co._Specular[3];
                    ns[16].Value = (decimal)co._Shininess[0];
                }
            }

            this._BoolManualChangeValues = false;
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int dex = e.Index;
            bool dexG = dex < LIGHT_COUNT;

            if (dexG)
            {
                if (false) ;
                else if (dex == 0)
                {
                    if (e.NewValue == CheckState.Checked) GL.Enable(EnableCap.Light0);
                    else GL.Disable(EnableCap.Light0);
                }
                else if (dex == 1)
                {
                    if (e.NewValue == CheckState.Checked) GL.Enable(EnableCap.Light1);
                    else GL.Disable(EnableCap.Light1);
                }
                else if (dex == 2)
                {
                    if (e.NewValue == CheckState.Checked) GL.Enable(EnableCap.Light2);
                    else GL.Disable(EnableCap.Light2);
                }
                else if (dex == 3)
                {
                    if (e.NewValue == CheckState.Checked) GL.Enable(EnableCap.Light3);
                    else GL.Disable(EnableCap.Light3);
                }
            }
            else
            {
                var co = this.checkedListBox1.Items[e.Index] as CadObject;
                if (co != null)
                {
                    co._Display = e.NewValue == CheckState.Checked;
                }
            }
        }


        private void numericUpDownMat_ValueChanged(object sender, EventArgs e)
        {
            if (this._BoolManualChangeValues) return;

            var ns = this._NumericUpDowns;
            int dex = this.checkedListBox1.SelectedIndex;
            bool dexG = dex < LIGHT_COUNT;

            if (dexG)
            {
                if (dex > -1)
                {
                    this.Ambients[dex][0] = (float)ns[0].Value;
                    this.Ambients[dex][1] = (float)ns[1].Value;
                    this.Ambients[dex][2] = (float)ns[2].Value;
                    this.Ambients[dex][3] = (float)ns[3].Value;
                    this.Diffuse[dex][0] = (float)ns[4].Value;
                    this.Diffuse[dex][1] = (float)ns[5].Value;
                    this.Diffuse[dex][2] = (float)ns[6].Value;
                    this.Diffuse[dex][3] = (float)ns[7].Value;
                    this.Specular[dex][0] = (float)ns[12].Value;
                    this.Specular[dex][1] = (float)ns[13].Value;
                    this.Specular[dex][2] = (float)ns[14].Value;
                    this.Specular[dex][3] = (float)ns[15].Value;

                    this.setLight(dex);
                }
            }
            else
            {
                var co = this.checkedListBox1.SelectedItem as CadObject;
                if (co != null)
                {
                    co._Ambient[0] = (float)ns[0].Value;
                    co._Ambient[1] = (float)ns[1].Value;
                    co._Ambient[2] = (float)ns[2].Value;
                    co._Ambient[3] = (float)ns[3].Value;
                    co._Diffuse[0] = (float)ns[4].Value;
                    co._Diffuse[1] = (float)ns[5].Value;
                    co._Diffuse[2] = (float)ns[6].Value;
                    co._Diffuse[3] = (float)ns[7].Value;
                    co._Emission[0] = (float)ns[8].Value;
                    co._Emission[1] = (float)ns[9].Value;
                    co._Emission[2] = (float)ns[10].Value;
                    co._Emission[3] = (float)ns[11].Value;
                    co._Specular[0] = (float)ns[12].Value;
                    co._Specular[1] = (float)ns[13].Value;
                    co._Specular[2] = (float)ns[14].Value;
                    co._Specular[3] = (float)ns[15].Value;
                    co._Shininess[0] = (float)ns[16].Value;
                }
            }
        }

        public void setLight(int i)
        {
            LightName ln;

            if (i == 0) ln = LightName.Light0;
            else if (i == 1) ln = LightName.Light1;
            else if (i == 2) ln = LightName.Light2;
            else if (i == 3) ln = LightName.Light3;
            else return;

            GL.Light(ln, LightParameter.Ambient, this.Ambients[i]);
            GL.Light(ln, LightParameter.Diffuse, this.Diffuse[i]);
            GL.Light(ln, LightParameter.Specular, this.Specular[i]);

            GL.Light(ln, LightParameter.Position, this.Position[i]);
        }
    }
}
