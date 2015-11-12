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

        private void buttonTrialReset_Click(object sender, EventArgs e)  //If the New Trial button from FormBase is clicked..
        {
            if (false) { }
            else if (this.rb_E_Flow_Keyboard.Checked)                   //I'm not sure exactly where these are coming from. seems like consise way
                new TE_FlowKeyboard();                                  //to determine which radio button is 'checked'. But where are instances created
            else if (this.rb_E_Flow_Sticks.Checked)
                new TE_FlowSticks();
            else if (this.rb_E_CylindricalVelocity_Keyboard.Checked)
                new TE_VelocityCylinderKeyboard();
            else if (this.rb_E_CylindricalVelocity_Sticks.Checked)
                new TE_VelocityCylinderSticks();
            else if (this.rb_E_CylindricalVelocityFlipped_Sticks.Checked)
                new TE_VelocityCylinderSticksBeau();

            this.Close();
        }

    }
}
