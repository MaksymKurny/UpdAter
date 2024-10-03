using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdAter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool hiddenMode = args.Contains("--hidden");
            MainForm mainForm = new MainForm(args);
            if (hiddenMode)
            {
                mainForm.WindowState = FormWindowState.Minimized;
                mainForm.ShowInTaskbar = false;
                mainForm.Visible = false;
                Application.Run();
            }
            else
            {
                Application.Run(mainForm);
            }
        }
    }
}
