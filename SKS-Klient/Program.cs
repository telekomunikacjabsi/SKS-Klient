using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKS_Klient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Settings settings = new Settings();
            Worker worker;
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
                worker = new Worker(settings);
            else
            {
                Application.Run(new MainForm(settings, settingsLoaded, serversLoaded));
            }
        }
    }
}
