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
            this.rb_B_Flow_Keyboard = new System.Windows.Forms.RadioButton();
            this.rb_XPC = new System.Windows.Forms.RadioButton();
            this.buttonTrialReset = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rb_E_Flow_Keyboard = new System.Windows.Forms.RadioButton();
            this.rb_E_Flow_Sticks = new System.Windows.Forms.RadioButton();
            this.checkBoxSave = new System.Windows.Forms.CheckBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.nudMinutes = new System.Windows.Forms.NumericUpDown();
            this.rb_B_CylindricalVelocity_Keyboard = new System.Windows.Forms.RadioButton();
            this.rb_E_CylindricalVelocity_Keyboard = new System.Windows.Forms.RadioButton();
            this.rb_E_CylindricalVelocity_Sticks = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinutes)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 18);
            this.label1.TabIndex = 76;
            this.label1.Text = "Basic";
            // 
            // rb_B_Flow_Keyboard
            // 
            this.rb_B_Flow_Keyboard.AutoSize = true;
            this.rb_B_Flow_Keyboard.Checked = true;
            this.rb_B_Flow_Keyboard.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.rb_B_Flow_Keyboard.Location = new System.Drawing.Point(14, 35);
            this.rb_B_Flow_Keyboard.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rb_B_Flow_Keyboard.Name = "rb_B_Flow_Keyboard";
            this.rb_B_Flow_Keyboard.Size = new System.Drawing.Size(126, 22);
            this.rb_B_Flow_Keyboard.TabIndex = 73;
            this.rb_B_Flow_Keyboard.TabStop = true;
            this.rb_B_Flow_Keyboard.Text = "Flow - Keyboard";
            this.rb_B_Flow_Keyboard.UseVisualStyleBackColor = true;
            this.rb_B_Flow_Keyboard.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rb_XPC
            // 
            this.rb_XPC.AutoSize = true;
            this.rb_XPC.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.rb_XPC.Location = new System.Drawing.Point(14, 121);
            this.rb_XPC.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rb_XPC.Name = "rb_XPC";
            this.rb_XPC.Size = new System.Drawing.Size(133, 22);
            this.rb_XPC.TabIndex = 74;
            this.rb_XPC.Text = "Target Simulation";
            this.rb_XPC.UseVisualStyleBackColor = true;
            this.rb_XPC.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // buttonTrialReset
            // 
            this.buttonTrialReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTrialReset.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.buttonTrialReset.Location = new System.Drawing.Point(470, 274);
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
            this.label2.Location = new System.Drawing.Point(11, 100);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 18);
            this.label2.TabIndex = 81;
            this.label2.Text = "Elton (Matlab Target)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 161);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 18);
            this.label3.TabIndex = 83;
            this.label3.Text = "Seifert (Matlab Embed)";
            // 
            // rb_E_Flow_Keyboard
            // 
            this.rb_E_Flow_Keyboard.AutoSize = true;
            this.rb_E_Flow_Keyboard.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.rb_E_Flow_Keyboard.Location = new System.Drawing.Point(14, 185);
            this.rb_E_Flow_Keyboard.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rb_E_Flow_Keyboard.Name = "rb_E_Flow_Keyboard";
            this.rb_E_Flow_Keyboard.Size = new System.Drawing.Size(126, 22);
            this.rb_E_Flow_Keyboard.TabIndex = 82;
            this.rb_E_Flow_Keyboard.Text = "Flow - Keyboard";
            this.rb_E_Flow_Keyboard.UseVisualStyleBackColor = true;
            this.rb_E_Flow_Keyboard.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rb_E_Flow_Sticks
            // 
            this.rb_E_Flow_Sticks.AutoSize = true;
            this.rb_E_Flow_Sticks.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.rb_E_Flow_Sticks.Location = new System.Drawing.Point(14, 213);
            this.rb_E_Flow_Sticks.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rb_E_Flow_Sticks.Name = "rb_E_Flow_Sticks";
            this.rb_E_Flow_Sticks.Size = new System.Drawing.Size(101, 22);
            this.rb_E_Flow_Sticks.TabIndex = 86;
            this.rb_E_Flow_Sticks.Text = "Flow - Sticks";
            this.rb_E_Flow_Sticks.UseVisualStyleBackColor = true;
            this.rb_E_Flow_Sticks.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // checkBoxSave
            // 
            this.checkBoxSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxSave.AutoCheck = false;
            this.checkBoxSave.AutoSize = true;
            this.checkBoxSave.Enabled = false;
            this.checkBoxSave.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxSave.ForeColor = System.Drawing.Color.Black;
            this.checkBoxSave.Location = new System.Drawing.Point(169, 160);
            this.checkBoxSave.Name = "checkBoxSave";
            this.checkBoxSave.Size = new System.Drawing.Size(60, 22);
            this.checkBoxSave.TabIndex = 87;
            this.checkBoxSave.Text = "Save:";
            this.checkBoxSave.UseVisualStyleBackColor = true;
            this.checkBoxSave.CheckedChanged += new System.EventHandler(this.checkBoxSave_CheckedChanged);
            this.checkBoxSave.Click += new System.EventHandler(this.checkBoxSave_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Data File|*.dat";
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // nudMinutes
            // 
            this.nudMinutes.Enabled = false;
            this.nudMinutes.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.nudMinutes.Location = new System.Drawing.Point(235, 159);
            this.nudMinutes.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMinutes.Name = "nudMinutes";
            this.nudMinutes.Size = new System.Drawing.Size(58, 26);
            this.nudMinutes.TabIndex = 88;
            this.nudMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudMinutes.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // rb_B_CylindricalVelocity_Keyboard
            // 
            this.rb_B_CylindricalVelocity_Keyboard.AutoSize = true;
            this.rb_B_CylindricalVelocity_Keyboard.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.rb_B_CylindricalVelocity_Keyboard.Location = new System.Drawing.Point(14, 63);
            this.rb_B_CylindricalVelocity_Keyboard.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rb_B_CylindricalVelocity_Keyboard.Name = "rb_B_CylindricalVelocity_Keyboard";
            this.rb_B_CylindricalVelocity_Keyboard.Size = new System.Drawing.Size(214, 22);
            this.rb_B_CylindricalVelocity_Keyboard.TabIndex = 89;
            this.rb_B_CylindricalVelocity_Keyboard.Text = "Cylindrical Velocity - Keyboard";
            this.rb_B_CylindricalVelocity_Keyboard.UseVisualStyleBackColor = true;
            this.rb_B_CylindricalVelocity_Keyboard.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rb_E_CylindricalVelocity_Keyboard
            // 
            this.rb_E_CylindricalVelocity_Keyboard.AutoSize = true;
            this.rb_E_CylindricalVelocity_Keyboard.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.rb_E_CylindricalVelocity_Keyboard.Location = new System.Drawing.Point(150, 185);
            this.rb_E_CylindricalVelocity_Keyboard.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rb_E_CylindricalVelocity_Keyboard.Name = "rb_E_CylindricalVelocity_Keyboard";
            this.rb_E_CylindricalVelocity_Keyboard.Size = new System.Drawing.Size(214, 22);
            this.rb_E_CylindricalVelocity_Keyboard.TabIndex = 90;
            this.rb_E_CylindricalVelocity_Keyboard.Text = "Cylindrical Velocity - Keyboard";
            this.rb_E_CylindricalVelocity_Keyboard.UseVisualStyleBackColor = true;
            this.rb_E_CylindricalVelocity_Keyboard.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rb_E_CylindricalVelocity_Sticks
            // 
            this.rb_E_CylindricalVelocity_Sticks.AutoSize = true;
            this.rb_E_CylindricalVelocity_Sticks.Font = new System.Drawing.Font("Calibri", 11.25F);
            this.rb_E_CylindricalVelocity_Sticks.Location = new System.Drawing.Point(150, 213);
            this.rb_E_CylindricalVelocity_Sticks.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rb_E_CylindricalVelocity_Sticks.Name = "rb_E_CylindricalVelocity_Sticks";
            this.rb_E_CylindricalVelocity_Sticks.Size = new System.Drawing.Size(183, 22);
            this.rb_E_CylindricalVelocity_Sticks.TabIndex = 91;
            this.rb_E_CylindricalVelocity_Sticks.Text = "Cylindrical Velocity - Stick";
            this.rb_E_CylindricalVelocity_Sticks.UseVisualStyleBackColor = true;
            this.rb_E_CylindricalVelocity_Sticks.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // FormPickTrial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 313);
            this.Controls.Add(this.rb_E_CylindricalVelocity_Sticks);
            this.Controls.Add(this.rb_E_CylindricalVelocity_Keyboard);
            this.Controls.Add(this.rb_B_CylindricalVelocity_Keyboard);
            this.Controls.Add(this.nudMinutes);
            this.Controls.Add(this.checkBoxSave);
            this.Controls.Add(this.rb_E_Flow_Sticks);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rb_E_Flow_Keyboard);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonTrialReset);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rb_B_Flow_Keyboard);
            this.Controls.Add(this.rb_XPC);
            this.Name = "FormPickTrial";
            this.ShowIcon = false;
            this.Text = "Pick One";
            ((System.ComponentModel.ISupportInitialize)(this.nudMinutes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rb_B_Flow_Keyboard;
        private System.Windows.Forms.RadioButton rb_XPC;
        private System.Windows.Forms.Button buttonTrialReset;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rb_E_Flow_Keyboard;
        private System.Windows.Forms.RadioButton rb_E_Flow_Sticks;
        private System.Windows.Forms.CheckBox checkBoxSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.NumericUpDown nudMinutes;
        private System.Windows.Forms.RadioButton rb_B_CylindricalVelocity_Keyboard;
        private System.Windows.Forms.RadioButton rb_E_CylindricalVelocity_Keyboard;
        private System.Windows.Forms.RadioButton rb_E_CylindricalVelocity_Sticks;

    }
}