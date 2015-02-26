namespace Excavator
{
    partial class FormPickTrial
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
            this.label1 = new System.Windows.Forms.Label();
            this.rb_S_P = new System.Windows.Forms.RadioButton();
            this.rb_B_JCK = new System.Windows.Forms.RadioButton();
            this.rb_D_CCP = new System.Windows.Forms.RadioButton();
            this.buttonTrialReset = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 18);
            this.label1.TabIndex = 76;
            this.label1.Text = "Basic";
            // 
            // rb_S_P
            // 
            this.rb_S_P.AutoSize = true;
            this.rb_S_P.Enabled = false;
            this.rb_S_P.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.rb_S_P.Location = new System.Drawing.Point(14, 137);
            this.rb_S_P.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rb_S_P.Name = "rb_S_P";
            this.rb_S_P.Size = new System.Drawing.Size(64, 22);
            this.rb_S_P.TabIndex = 75;
            this.rb_S_P.Text = "Pillars";
            this.rb_S_P.UseVisualStyleBackColor = true;
            // 
            // rb_B_JCK
            // 
            this.rb_B_JCK.AutoSize = true;
            this.rb_B_JCK.Checked = true;
            this.rb_B_JCK.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.rb_B_JCK.Location = new System.Drawing.Point(14, 35);
            this.rb_B_JCK.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rb_B_JCK.Name = "rb_B_JCK";
            this.rb_B_JCK.Size = new System.Drawing.Size(167, 22);
            this.rb_B_JCK.TabIndex = 73;
            this.rb_B_JCK.TabStop = true;
            this.rb_B_JCK.Text = "Joint Control Keyboard";
            this.rb_B_JCK.UseVisualStyleBackColor = true;
            // 
            // rb_D_JCK
            // 
            this.rb_D_CCP.AutoSize = true;
            this.rb_D_CCP.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.rb_D_CCP.Location = new System.Drawing.Point(14, 86);
            this.rb_D_CCP.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rb_D_CCP.Name = "rb_D_JCK";
            this.rb_D_CCP.Size = new System.Drawing.Size(211, 22);
            this.rb_D_CCP.TabIndex = 74;
            this.rb_D_CCP.Text = "Coordinated Control Phantom";
            this.rb_D_CCP.UseVisualStyleBackColor = true;
            // 
            // buttonTrialReset
            // 
            this.buttonTrialReset.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.buttonTrialReset.Location = new System.Drawing.Point(183, 223);
            this.buttonTrialReset.Name = "buttonTrialReset";
            this.buttonTrialReset.Size = new System.Drawing.Size(89, 27);
            this.buttonTrialReset.TabIndex = 80;
            this.buttonTrialReset.Text = "Done";
            this.buttonTrialReset.UseVisualStyleBackColor = true;
            this.buttonTrialReset.Click += new System.EventHandler(this.buttonTrialReset_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 65);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 18);
            this.label2.TabIndex = 81;
            this.label2.Text = "Dig";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 116);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 18);
            this.label3.TabIndex = 82;
            this.label3.Text = "Special";
            // 
            // FormPickTrial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonTrialReset);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rb_S_P);
            this.Controls.Add(this.rb_B_JCK);
            this.Controls.Add(this.rb_D_CCP);
            this.Name = "FormPickTrial";
            this.ShowIcon = false;
            this.Text = "Pick One";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rb_S_P;
        private System.Windows.Forms.RadioButton rb_B_JCK;
        private System.Windows.Forms.RadioButton rb_D_CCP;
        private System.Windows.Forms.Button buttonTrialReset;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;

    }
}