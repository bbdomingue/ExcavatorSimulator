using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;

namespace SamSeifert.GLE.CadViewer
{
    public partial class FormNewShape : Form
    {
        private enum SelectionType { Vertex, VertexAndNormal, VRML, XAML, VRML_Pro_E }
        private SelectionType _SelectionType = SelectionType.Vertex;

        private bool file1 = false;
        private bool file2 = false;
        private bool file3 = false;

        private CadHandler _CadHandler = null;

        public FormNewShape() : base()
        {
            InitializeComponent();

            this.updateEnabled();
        }

        internal FormNewShape(CadHandler mommy) : this()
        {
            this._CadHandler = mommy;
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton r = sender as RadioButton;

            if (r != null)
            {
                if (r.Checked)
                {
                    if (false) ;
                    else if (r == this.radioButton1) this._SelectionType = SelectionType.Vertex;
                    else if (r == this.radioButton2) this._SelectionType = SelectionType.VertexAndNormal;
                    else if (r == this.radioButton3) this._SelectionType = SelectionType.VRML;
                    else if (r == this.radioButton4) this._SelectionType = SelectionType.XAML;
                    else if (r == this.radioButton5) this._SelectionType = SelectionType.VRML_Pro_E;

                    this.updateEnabled();
                }
            }
        }

        private void updateEnabled()
        {
            bool b1 = false;
            bool b2 = false;
            bool b3 = false;

            this.button1.Text = "File 1";
            this.button2.Text = "File 2";
            this.button3.Text = "File 3";

            this.openFileDialog1.Filter = "";
            this.openFileDialog2.Filter = "";
            this.openFileDialog3.Filter = "";

            switch (this._SelectionType)
            {
                case SelectionType.Vertex:
                    {
                        this.button1.Text = "Vertex File";
                        b1 = true;
                        break;
                    }
                case SelectionType.VertexAndNormal:
                    {
                        this.button1.Text = "Vertex File";
                        this.button2.Text = "Normal File";
                        b1 = true;
                        b2 = true;
                        break;
                    }
                case SelectionType.VRML_Pro_E:
                case SelectionType.VRML:
                    {
                        this.button1.Text = "VRML File";
                        this.openFileDialog1.Filter = "VRML File|*.wrl";
                        b1 = true;
                        break;
                    }
                case SelectionType.XAML:
                    {
                        this.button1.Text = "XAML File";
                        this.openFileDialog1.Filter = "XAML File|*.XAML";
                        b1 = true;
                        break;
                    }
            }

            this.button1.Enabled = b1;
            this.button2.Enabled = b2;
            this.button3.Enabled = b3;

            this.textBox1.Enabled = b1;
            this.textBox2.Enabled = b2;
            this.textBox3.Enabled = b3;

            this.setGo();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox1.Text = this.openFileDialog1.FileName;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox2.Text = this.openFileDialog2.FileName;
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox3.Text = this.openFileDialog3.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.openFileDialog2.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.openFileDialog3.ShowDialog();
        }

        private Color ColorGood = Color.Green;
        private Color ColorBad = Color.Red;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.file1 = File.Exists(this.textBox1.Text);
            this.textBox1.ForeColor = this.file1 ?
                this.ColorGood :
                this.ColorBad;
            this.setGo();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.file2 = File.Exists(this.textBox2.Text);
            this.textBox2.ForeColor = this.file2 ?
                this.ColorGood :
                this.ColorBad;
            this.setGo();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            this.file3 = File.Exists(this.textBox3.Text);
            this.textBox3.ForeColor = this.file3 ?
                this.ColorGood :
                this.ColorBad;
            this.setGo();
        }

        private void setGo()
        {
            this.button0.Enabled =
                (this.file1 || !this.button1.Enabled) &&
                (this.file2 || !this.button2.Enabled) &&
                (this.file3 || !this.button3.Enabled);
        }

        private void button0_Click(object sender, EventArgs e)
        {
            var list = new List<CadObject>();

            switch (this._SelectionType)
            {
                case SelectionType.Vertex:
                    {
                        var name = Path.GetFileName(this.textBox1.Text);
                        var t = File.ReadAllLines(this.textBox1.Text);
                        var co = CadObjectGenerator.fromVertexList(t, name);
                        if (co != null) list.Add(co);
                        break;
                    }
                case SelectionType.VertexAndNormal:
                    {
                        var name = Path.GetFileName(this.textBox1.Text);
                        var t1 = File.ReadAllLines(this.textBox1.Text);
                        var t2 = File.ReadAllLines(this.textBox2.Text);
                        var co = CadObjectGenerator.fromVertexAndNormalList(t1, t2, name);
                        if (co != null) list.Add(co);
                        break;
                    }
                case SelectionType.VRML:
                    {
                        var name = Path.GetFileName(this.textBox1.Text);
                        var t = String.Join(" ", File.ReadAllLines(this.textBox1.Text));
                        var co = CadObjectGenerator.fromVRMLUnknownSource(t, name);
                        if (co != null) list.Add(co);
                        break;
                    }
                case SelectionType.XAML:
                    {
                        var name = Path.GetFileName(this.textBox1.Text);
                        var t = String.Join(" ", File.ReadAllLines(this.textBox1.Text));
                        var co = CadObjectGenerator.fromXAML(t, name);
                        if (co != null) list.Add(co);
                        break;
                    }
                case SelectionType.VRML_Pro_E:
                    {
                        var name = Path.GetFileName(this.textBox1.Text);
                        var t = String.Join(" ", File.ReadAllLines(this.textBox1.Text));
                        var co = CadObjectGenerator.fromVRML_Pro_E(t, name);
                        if (co != null) list.Add(co);
                        break;
                    }
            }

            this._CadHandler.addParts(list.ToArray());

            this.Close();
        }
    }
}
