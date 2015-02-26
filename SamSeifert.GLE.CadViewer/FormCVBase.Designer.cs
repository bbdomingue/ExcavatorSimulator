namespace SamSeifert.GLE.CadViewer
{
    partial class FormCVBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelFPS = new System.Windows.Forms.Label();
            this.labelFPSX = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.glControl1 = new OpenTK.GLControl();
            this.cadHandler1 = new SamSeifert.GLE.CadViewer.CadHandler();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelFPS);
            this.panel1.Controls.Add(this.labelFPSX);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(10, 646);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(481, 146);
            this.panel1.TabIndex = 1;
            // 
            // labelFPS
            // 
            this.labelFPS.AutoSize = true;
            this.labelFPS.Location = new System.Drawing.Point(56, 3);
            this.labelFPS.Name = "labelFPS";
            this.labelFPS.Size = new System.Drawing.Size(35, 13);
            this.labelFPS.TabIndex = 4;
            this.labelFPS.Text = "label2";
            // 
            // labelFPSX
            // 
            this.labelFPSX.AutoSize = true;
            this.labelFPSX.Location = new System.Drawing.Point(3, 3);
            this.labelFPSX.Name = "labelFPSX";
            this.labelFPSX.Size = new System.Drawing.Size(47, 13);
            this.labelFPSX.TabIndex = 3;
            this.labelFPSX.Text = "GL FPS:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.glControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(10, 10);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 0, 10, 10);
            this.panel2.Size = new System.Drawing.Size(481, 636);
            this.panel2.TabIndex = 2;
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl1.Location = new System.Drawing.Point(0, 0);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(471, 626);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = true;
            this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
            this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
            this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
            // 
            // cadHandler1
            // 
            this.cadHandler1.Dock = System.Windows.Forms.DockStyle.Right;
            this.cadHandler1.Location = new System.Drawing.Point(491, 10);
            this.cadHandler1.MaximumSize = new System.Drawing.Size(266, 0);
            this.cadHandler1.MinimumSize = new System.Drawing.Size(266, 266);
            this.cadHandler1.Name = "cadHandler1";
            this.cadHandler1.Size = new System.Drawing.Size(266, 782);
            this.cadHandler1.TabIndex = 0;
            // 
            // FormBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 802);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cadHandler1);
            this.Name = "FormBase";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBase_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CadHandler cadHandler1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.Label labelFPS;
        private System.Windows.Forms.Label labelFPSX;



    }
}

