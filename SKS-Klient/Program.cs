using System;
using System.Windows.Forms;

namespace SKS_Klient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        private static Worker worker;

        [STAThread]
        static void Main()
        {
            Settings settings = new Settings();
            bool serversLoaded = false;
            bool settingsLoaded = false;
            try
            {
                settings.LoadServers();
                serversLoaded = true;
            }
            catch
            { }
            try
            {
                settings.LoadSettings();
                settingsLoaded = true;
            }
            catch
            { }
            if (settingsLoaded && serversLoaded) // jeśli wszystkie ustawienia są znane
            {
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
                worker = new Worker(settings);
            }
            else
            {
                Application.Run(new MainForm(settings, settingsLoaded, serversLoaded));
            }
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            worker.StopWork();
        }
    }
}
