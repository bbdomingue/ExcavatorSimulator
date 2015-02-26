using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.GLE.CadViewer;

namespace TreeConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        string foundfile = null;

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            Console.WriteLine("Found File");
            this.foundfile = this.openFileDialog1.FileName;
            this.glControl1.Invalidate();
        }


        private bool notload = true;
        private void glControl1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Loaded");

            this.notload = false;
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (notload) return;

            this.glControl1.SwapBuffers();

            if (this.foundfile == null) return;

            string[] lines = File.ReadAllLines(this.foundfile);

            this.foundfile = null;

            Console.WriteLine("Picked File: " + lines.Length + " lines");

            float[] currentColor = new float[] { };

            var verts = new List<String>();
            var norms = new List<String>();

            var objects = new List<CadObject>();

            foreach (String line in lines)
            {
                int first = line.IndexOf("(") + 1;
                int second = line.IndexOf(")");

                if (line.Contains("glColor3f"))
                {
                    if (verts.Count == norms.Count)
                    {
                        if (verts.Count + norms.Count > 0)
                        {
                            CadObject co = CadObjectGenerator.fromVertexAndNormalList(
                                verts.ToArray(),
                                norms.ToArray());

                            Console.WriteLine("Created Cad File:" + co);

                            float[] amb = new float[4];
                            float[] dif = new float[4];
                            float[] zero = new float[4] { 0, 0, 0, 0 };
                            float[] shine = new float[1] { 0 };

                            for (int i = 0; i < 3; i++)
                            {
                                amb[i] = currentColor[i] * 0.2f;
                                dif[i] = currentColor[i] * 0.8f;
                            }

                            co.setColor(amb, dif, zero, zero, shine);

                            objects.Add(co);
                        }
                    }
                    else Console.WriteLine("Unequal Lines");

                    norms.Clear();
                    verts.Clear();

                    var data = line.Substring(first, second - first);
                    var nums = data.Split(',');

                    currentColor = new float[] { 0, 0, 0, 1 };

                    for (int i = 0; i < Math.Min(nums.Length, 3); i++)
                        currentColor[i] = (float)Double.Parse(nums[i]);
                }
                else if (line.Contains("glNormal3f"))
                {
                    norms.Add(line.Substring(first, second - first));
                }
                else if (line.Contains("glVertex3f"))
                {
                    verts.Add(line.Substring(first, second - first));
                }

            }

            if (verts.Count == norms.Count)
            {
                if (verts.Count + norms.Count > 0)
                {
                    CadObject co = CadObjectGenerator.fromVertexAndNormalList(
                        verts.ToArray(),
                        norms.ToArray());

                    float[] amb = new float[4];
                    float[] dif = new float[4];
                    float[] zero = new float[4] { 0, 0, 0, 0 };
                    float[] shine = new float[1] { 0 };

                    for (int i = 0; i < 3; i++)
                    {
                        amb[i] = currentColor[i] * 0.2f;
                        dif[i] = currentColor[i] * 0.8f;
                    }

                    co.setColor(amb, dif, zero, zero, shine);

                    objects.Add(co);
                }
            }
            else Console.WriteLine("Unequal Lines");

            verts.Clear();
            norms.Clear();

            CadObject co2 = CadObjectGenerator.gen(objects.ToArray());

            new FormSaveAs(co2).ShowDialog();
        }

    }
}
