using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace SceneNavi
{
    static class Program
    {
        public static string AppNameVer = Application.ProductName + " " + VersionManagement.CreateVersionString(Application.ProductVersion);

        public static StatusMessageHandler Status = new StatusMessageHandler();
        public static bool IsHinting = false;

        /* Mutex & general app restart stuff from http://stackoverflow.com/a/9056664 */

        [STAThread]
        static void Main()
        {
            Mutex runOnce = null;

            if (Configuration.IsRestarting)
            {
                Configuration.IsRestarting = false;
                Thread.Sleep(3000);
            }
#if !DEBUG
            try
            {
#endif
            runOnce = new Mutex(true, "SOME_MUTEX_NAME");

            if (runOnce.WaitOne(TimeSpan.Zero))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
#if !DEBUG
            }
            catch (Exception ex)
            {
                MessageBox.Show("Critical error occured: " + ex.GetType().FullName + " - " + ex.Message + "\nTarget site: " + ex.TargetSite, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                if (null != runOnce)
                    runOnce.Close();
            }
#endif
        }
    }
}
