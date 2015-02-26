using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SamSeifert.GLE.CadViewer
{
    public partial class FormSaveAs : Form
    {
        private CadObject _CadObject;

        private FormSaveAs()
        {
            InitializeComponent();
        }

        public FormSaveAs(CadObject cad) : this()
        {
            this._CadObject = cad;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox1.Text = this.saveFileDialog1.FileName;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button0_Click(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                var save = new List<String>();

                foreach (var ln in this._CadObject.vertex)
                {
                    save.Add(ln.X + "," + ln.Y + "," + ln.Z);
                }

                File.WriteAllLines(this.textBox1.Text, save, Encoding.UTF8);

                this.Close();
            }
            else if (this.radioButton2.Checked)
            {
            }
            else if (this.radioButton3.Checked)
            {
            }
            else if (this.radioButton4.Checked)
            {
                this.XAMLSaveAs();

                this.Close();

            }
            else if (this.radioButtonSam.Checked)
            {
                FormSaveAs.SamSave(this.textBox1.Text, this._CadObject);
                this.Close();
            }
        }

        public static void SamSave(String path, CadObject co)
        {
            TextWriter tw = new StreamWriter(path);

            if (co._CadObjects.Length == 0)
            {
                Single[] x;
                x = co._Ambient; tw.WriteLine(x[0] + "," + x[1] + "," + x[2] + "," + x[3]);
                x = co._Diffuse; tw.WriteLine(x[0] + "," + x[1] + "," + x[2] + "," + x[3]);
                x = co._Emission; tw.WriteLine(x[0] + "," + x[1] + "," + x[2] + "," + x[3]);
                x = co._Specular; tw.WriteLine(x[0] + "," + x[1] + "," + x[2] + "," + x[3]);
                x = co._Shininess; tw.WriteLine(x[0]);
                tw.WriteLine(co.indices.Length); foreach (var i in co.indices) tw.WriteLine(i);

                tw.WriteLine((co.vertex.Length * 2));
                for (int i = 0; i < co.vertex.Length; i++)
                {
                    tw.WriteLine(FormSaveAs.forVec(co.vertex[i]));
                    tw.WriteLine(FormSaveAs.forVec(co.normal[i]));
                }
            }
            else
            {
                tw.WriteLine("CHILD");
                for (int i = 0; i < co._CadObjects.Length; i++)
                {
                    CadObject c = co._CadObjects[i];

                    ;

                    String p = "";
                    bool first = true;
                    foreach (String s in path.Split(new char[] { '.' }))
                    { 
                        p += s;
                        if (first) p += "_" + i + "." ;
                        first = false;
                    }
                    tw.WriteLine(p);
                    FormSaveAs.SamSave(p, c);
                }
            }

            // close the stream
            tw.Close();
        }
        public static String forVec(OpenTK.Vector3 v)
        {
            const string mat = "0.0000";
            return v.X.ToString(mat) + "," + v.Y.ToString(mat) + "," + v.Z.ToString(mat);
        }








        /*
                private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
                {
                    StreamWriter f = File.CreateText(this.saveFileDialog1.FileName);

                    f.WriteLine("solid thing");
            
                    for (int i = 0; i < this.verts.Length; )
                    {
                        if (this.d2)
                        {
                            f.Write("facet normal ");
                            f.Write(((this.norms[i][0] + this.norms[i][1] + this.norms[i][2]) / 3).ToString() + " ");
                            f.Write(((this.norms[i][0] + this.norms[i][1] + this.norms[i][2]) / 3).ToString() + " ");
                            f.WriteLine(((this.norms[i][0] + this.norms[i][1] + this.norms[i][2]) / 3).ToString());
                        }
                        else f.WriteLine("facet normal 0 0 0");
                        {
                            f.WriteLine("outer loop");
                            {
                                f.Write("vertex ");
                                f.Write(this.verts[i][0].ToString() + " ");
                                f.Write(this.verts[i][1].ToString() + " ");
                                f.WriteLine(this.verts[i][2].ToString());
                                i++;
                                f.Write("vertex ");
                                f.Write(this.verts[i][0].ToString() + " ");
                                f.Write(this.verts[i][1].ToString() + " ");
                                f.WriteLine(this.verts[i][2].ToString());
                                i++;
                                f.Write("vertex ");
                                f.Write(this.verts[i][0].ToString() + " ");
                                f.Write(this.verts[i][1].ToString() + " ");
                                f.WriteLine(this.verts[i][2].ToString());
                                i++;
                            }
                            f.WriteLine("endloop");
                        }
                        f.WriteLine("endfacet");
                    }                

                    f.WriteLine("endsolid thing");
                    f.Flush();
                    f.Close();
                }

                */












        private void XAMLSaveAs()
        {
            var save = new List<String>();

            const String match0 = "ModelVisual3D.Children";

            save.Add("<" + match0 + ">");

            if (this._CadObject._CadObjects.Length == 0)
                save.AddRange(FormSaveAs.getDataForObject(this._CadObject));
            else foreach (CadObject co in this._CadObject._CadObjects)
                 save.AddRange(FormSaveAs.getDataForObject(co));

            File.WriteAllLines(this.textBox1.Text, save, Encoding.UTF8);

            save.Add("</" + match0 + ">");
        }

        private static List<String> getDataForObject(CadObject co)
        {
            var save = new List<String>();

            if (co.type != CadObject.GLType.GL4) return save;

            const String match1 = "GeometryModel3D";
            const String match2 = "DiffuseMaterial";
            const String match3 = "SpecularMaterial";
            const String match4 = "AmbientMaterial";
            const String match5 = "EmissionMaterial";
            const String match6 = "MeshGeometry3D";

            save.Add("<" + match1 + ">");

            save.Add("<" + match2 + ">");
            save.Add(FormSaveAs.XAMLstring4Color(co._Diffuse));
            save.Add("</" + match2 + ">");

            save.Add("<" + match3 + ">");
            save.Add(FormSaveAs.XAMLstring4Color(co._Specular));
            save.Add("</" + match3 + ">");

            save.Add("<" + match4 + ">");
            save.Add(FormSaveAs.XAMLstring4Color(co._Ambient));
            save.Add("</" + match4 + ">");

            save.Add("<" + match5 + ">");
            save.Add(FormSaveAs.XAMLstring4Color(co._Emission));
            save.Add("</" + match5 + ">");

            save.Add("<" + match6 + " " + FormSaveAs.XAMLconsolidateData(co) + "/>");

            save.Add("</" + match1 + ">");

            return save;
        }

        private static String XAMLstring4Color(float[] args)
        {
            Byte R = (Byte)(255 * Math.Max(0, Math.Min(255, args[0])));
            Byte G = (Byte)(255 * Math.Max(0, Math.Min(255, args[1])));
            Byte B = (Byte)(255 * Math.Max(0, Math.Min(255, args[2])));
            float alpha = args[3];


            String matchX = "SolidColorBrush";

            const String SCOLOR = "Color";
            const String SOPAC = "Opacity";

            return "<" + matchX + " " +
                SCOLOR + "=\"#" + R.ToString("X2") + G.ToString("X2") + B.ToString("X2") + "\" " +
                SOPAC + "=\"" + alpha.ToString("0.0000") + "\"/>";
        }

        private static String XAMLconsolidateData(CadObject co)
        {
            const String m1 = "Positions";
            const String m2 = "Normals";
            const String m3 = "TriangleIndices";
            var m1a = new List<String>();
            var m2a = new List<String>();
            var m3a = new List<String>();

            foreach (var vec in co.vertex)
                m1a.Add(vec.X + "," + vec.Y + "," + vec.Z + " ");

            foreach (var vec in co.normal)
                m2a.Add(vec.X + "," + vec.Y + "," + vec.Z + " ");

            for (int i = 0; i < co.indices.Length; i++)
                m3a.Add(co.indices[i] + (((i + 1) % 3 == 0) ? " " : ","));

            return
                m1 + "=\"" + String.Join("", m1a) + "\" " +
                m2 + "=\"" + String.Join("", m2a) + "\" " +
                m3 + "=\"" + String.Join("", m3a) + "\" ";
        }


    }
}
