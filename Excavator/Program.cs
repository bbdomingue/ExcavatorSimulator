using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Excavator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
//            SamSeifert.GLE.CadViewer.CadHandler._BoolAllowDelete = false;
//            SamSeifert.GLE.CadViewer.CadHandler._BoolAllowNew = false;
//            SamSeifert.GLE.CadViewer.CadHandler._BoolAllowSaveAs = false;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormBase());
        }
    }
}
