using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Excavator
{
    public partial class FormPickTrial : Form
    {
        public FormPickTrial()
        {
            InitializeComponent();
        }

        private void buttonTrialReset_Click(object sender, EventArgs e)
        {
            string s = (this.checkBoxSave.Enabled && this.checkBoxSave.Checked) ? this.saveFileDialog1.FileName : null;
            int i = (int) this.nudMinutes.Value;

            if (false) ;
            else if (this.rb_E_Flow_Keyboard.Checked)
                new TE_FlowKeyboard(FormBase._FormBase, s, i);
            else if (this.rb_E_Flow_Sticks.Checked)
                new TE_FlowSticks(FormBase._FormBase, s, i);
            else if (this.rb_E_CylindricalVelocity_Keyboard.Checked)
                new TE_VelocityCylinderKeyboard(FormBase._FormBase, s, i);
            else if (this.rb_E_CylindricalVelocity_Sticks.Checked)
                new TE_VelocityCylinderSticks(FormBase._FormBase, s, i);
            else new TrialPillars(FormBase._FormBase);

            this.Close();
        }




        private void checkBoxSave_Click(object sender, EventArgs e)
        {
            if (this.checkBoxSave.Checked) this.checkBoxSave.Checked = false;
            else this.saveFileDialog1.ShowDialog();
            this.checkBoxSave_CheckedChanged(sender, e);
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.checkBoxSave.Checked = true;
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton r = sender as RadioButton;
            if (r.Checked)
            {
                this.checkBoxSave.Enabled =
                    r == this.rb_E_CylindricalVelocity_Keyboard ||
                    r == this.rb_E_CylindricalVelocity_Sticks ||
                    r == this.rb_E_Flow_Keyboard ||
                    r == this.rb_E_Flow_Sticks;

                this.checkBoxSave_CheckedChanged(sender, e);
            }

        }

        private void checkBoxSave_CheckedChanged(object sender, EventArgs e)
        {
            this.nudMinutes.Enabled = this.checkBoxSave.Checked && this.checkBoxSave.Enabled;
        }
    }
}
