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
            Trial._Trial.Deconstruct();

            if (false) ;
            else if (this.rb_D_CCP.Checked) new TrialMarkElton(FormBase._FormBase);
            else new TrialJointControlKeyboard(FormBase._FormBase);


            this.Close();
        }
    }
}
