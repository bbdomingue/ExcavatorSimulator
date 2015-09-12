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
            if (false) { }
            else if (this.rb_E_Flow_Keyboard.Checked)
                new TE_FlowKeyboard();
            else if (this.rb_E_Flow_Sticks.Checked)
                new TE_FlowSticks();
            else if (this.rb_E_CylindricalVelocity_Keyboard.Checked)
                new TE_VelocityCylinderKeyboard();
            else if (this.rb_E_CylindricalVelocity_Sticks.Checked)
                new TE_VelocityCylinderSticks();
            else if (this.rb_E_CylindricalVelocityFlipped_Sticks.Checked) ;
                new TE_VelocityCylinderSticksBeau();

            this.Close();
        }

    }
}
