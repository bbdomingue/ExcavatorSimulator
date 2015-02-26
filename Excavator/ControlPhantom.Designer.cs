namespace Excavator
{
    partial class ControlPhantom
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHostPhantom = new System.Windows.Forms.Integration.ElementHost();
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.buttonPhantomStatus = new System.Windows.Forms.Button();
            this.labelPR = new System.Windows.Forms.Label();
            this.buttonPhantomCalibrate = new System.Windows.Forms.Button();
            this.labelPhantomGLX = new System.Windows.Forms.Label();
            this.labelPhantomFPSX = new System.Windows.Forms.Label();
            this.labelPU = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // elementHostPhantom
            // 
            this.elementHostPhantom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.elementHostPhantom.Location = new System.Drawing.Point(10, 10);
            this.elementHostPhantom.Margin = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.elementHostPhantom.Name = "elementHostPhantom";
            this.elementHostPhantom.Size = new System.Drawing.Size(190, 190);
            this.elementHostPhantom.TabIndex = 79;
            this.elementHostPhantom.Text = "elementHost1";
            this.elementHostPhantom.Child = null;
            // 
            // comboBoxComPort
            // 
            this.comboBoxComPort.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxComPort.ForeColor = System.Drawing.Color.Black;
            this.comboBoxComPort.FormattingEnabled = true;
            this.comboBoxComPort.Location = new System.Drawing.Point(10, 205);
            this.comboBoxComPort.Margin = new System.Windows.Forms.Padding(10, 5, 10, 0);
            this.comboBoxComPort.Name = "comboBoxComPort";
            this.comboBoxComPort.Size = new System.Drawing.Size(190, 26);
            this.comboBoxComPort.TabIndex = 72;
            this.comboBoxComPort.Text = "Select a Port";
            this.comboBoxComPort.DropDown += new System.EventHandler(this.comboBoxComPort_DropDown);
            this.comboBoxComPort.SelectedIndexChanged += new System.EventHandler(this.comboBoxComPort_SelectedValueChanged);
            this.comboBoxComPort.SelectedValueChanged += new System.EventHandler(this.comboBoxComPort_SelectedValueChanged);
            // 
            // buttonPhantomStatus
            // 
            this.buttonPhantomStatus.Enabled = false;
            this.buttonPhantomStatus.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPhantomStatus.ForeColor = System.Drawing.Color.Black;
            this.buttonPhantomStatus.Location = new System.Drawing.Point(10, 236);
            this.buttonPhantomStatus.Margin = new System.Windows.Forms.Padding(10, 5, 0, 0);
            this.buttonPhantomStatus.Name = "buttonPhantomStatus";
            this.buttonPhantomStatus.Size = new System.Drawing.Size(90, 26);
            this.buttonPhantomStatus.TabIndex = 73;
            this.buttonPhantomStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonPhantomStatus.UseVisualStyleBackColor = true;
            this.buttonPhantomStatus.Click += new System.EventHandler(this.ledPhantomStatus_Click);
            this.buttonPhantomStatus.Paint += new System.Windows.Forms.PaintEventHandler(this.buttonPhantomStatus_Paint);
            // 
            // labelPhantomGL
            // 
            this.labelPR.AutoSize = true;
            this.labelPR.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPR.ForeColor = System.Drawing.Color.Black;
            this.labelPR.Location = new System.Drawing.Point(152, 290);
            this.labelPR.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.labelPR.Name = "labelPhantomGL";
            this.labelPR.Size = new System.Drawing.Size(15, 18);
            this.labelPR.TabIndex = 78;
            this.labelPR.Text = "0";
            this.labelPR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonPhantomCalibrate
            // 
            this.buttonPhantomCalibrate.Enabled = false;
            this.buttonPhantomCalibrate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPhantomCalibrate.ForeColor = System.Drawing.Color.Black;
            this.buttonPhantomCalibrate.Location = new System.Drawing.Point(110, 236);
            this.buttonPhantomCalibrate.Margin = new System.Windows.Forms.Padding(10, 5, 10, 0);
            this.buttonPhantomCalibrate.Name = "buttonPhantomCalibrate";
            this.buttonPhantomCalibrate.Size = new System.Drawing.Size(90, 26);
            this.buttonPhantomCalibrate.TabIndex = 74;
            this.buttonPhantomCalibrate.Text = "Calibrate";
            this.buttonPhantomCalibrate.UseVisualStyleBackColor = true;
            this.buttonPhantomCalibrate.Click += new System.EventHandler(this.buttonPhantomCalibrate_Click);
            // 
            // labelPhantomGLX
            // 
            this.labelPhantomGLX.AutoSize = true;
            this.labelPhantomGLX.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPhantomGLX.ForeColor = System.Drawing.Color.Black;
            this.labelPhantomGLX.Location = new System.Drawing.Point(7, 290);
            this.labelPhantomGLX.Margin = new System.Windows.Forms.Padding(10, 5, 0, 0);
            this.labelPhantomGLX.Name = "labelPhantomGLX";
            this.labelPhantomGLX.Size = new System.Drawing.Size(140, 18);
            this.labelPhantomGLX.TabIndex = 77;
            this.labelPhantomGLX.Text = "Phantom Renders PS:";
            // 
            // labelPhantomFPSX
            // 
            this.labelPhantomFPSX.AutoSize = true;
            this.labelPhantomFPSX.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPhantomFPSX.ForeColor = System.Drawing.Color.Black;
            this.labelPhantomFPSX.Location = new System.Drawing.Point(7, 267);
            this.labelPhantomFPSX.Margin = new System.Windows.Forms.Padding(10, 5, 0, 0);
            this.labelPhantomFPSX.Name = "labelPhantomFPSX";
            this.labelPhantomFPSX.Size = new System.Drawing.Size(140, 18);
            this.labelPhantomFPSX.TabIndex = 75;
            this.labelPhantomFPSX.Text = "Phantom Updates PS:";
            // 
            // labelPhantomFPS
            // 
            this.labelPU.AutoSize = true;
            this.labelPU.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPU.ForeColor = System.Drawing.Color.Black;
            this.labelPU.Location = new System.Drawing.Point(152, 267);
            this.labelPU.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.labelPU.Name = "labelPhantomFPS";
            this.labelPU.Size = new System.Drawing.Size(36, 18);
            this.labelPU.TabIndex = 76;
            this.labelPU.Text = "0000";
            this.labelPU.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ControlPhantom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.elementHostPhantom);
            this.Controls.Add(this.comboBoxComPort);
            this.Controls.Add(this.buttonPhantomStatus);
            this.Controls.Add(this.labelPR);
            this.Controls.Add(this.buttonPhantomCalibrate);
            this.Controls.Add(this.labelPhantomGLX);
            this.Controls.Add(this.labelPhantomFPSX);
            this.Controls.Add(this.labelPU);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ControlPhantom";
            this.Size = new System.Drawing.Size(210, 320);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHostPhantom;
        private System.Windows.Forms.ComboBox comboBoxComPort;
        private System.Windows.Forms.Button buttonPhantomStatus;
        private System.Windows.Forms.Label labelPR;
        private System.Windows.Forms.Button buttonPhantomCalibrate;
        private System.Windows.Forms.Label labelPhantomGLX;
        private System.Windows.Forms.Label labelPhantomFPSX;
        private System.Windows.Forms.Label labelPU;
    }
}
