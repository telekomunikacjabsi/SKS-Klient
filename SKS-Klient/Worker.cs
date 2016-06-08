using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SKS_Klient
{
    public class Worker
    {
        Settings settings;
        Thread mainThread;
        ServerConnection serverConnection;
        AdminConnection adminConnection;

        public Worker(Settings settings)
        {
            this.settings = settings;
            mainThread = new Thread(DoWork);
            mainThread.Start();
        }

        private void DoWork()
        {
            while (true)
            {
                try
                {
                    serverConnection = new ServerConnection(settings);
                    adminConnection = new AdminConnection(settings);
                    serverConnection.Connect(adminConnection.Port);
                    adminConnection.ListenAndConnect();
                    serverConnection.Disconnect(); // po uzyskaniu połączenia z administratorem zrywamy połączenie z serwerem
                    while (true) // pętla obsługi komunikatów od admina
                    {
                        try
                        {
                            // obsługa komunikatów od administratora
                            adminConnection.ReceiveMessage();
                            if (adminConnection.Command == CommandSet.Disconnect)
                            {
                                adminConnection.Close();
                                break; // wznawia procedurę nawiązywania połączeń od nowa
                            }
                            else if (adminConnection.Command == CommandSet.Screenshot)
                                SendScreenshot();
                            else if (adminConnection.Command == CommandSet.Message)
                                DisplayMessage();
                            else if (adminConnection.Command == CommandSet.Processes)
                                SendProcessesList();
                            else
                            {
                                adminConnection.Close();
                                break;
                            }
                        }
                        catch (IOException) // zerwanie połączenia z adminem
                        {
                            Debug.WriteLine("Rozłączono administratora");
                            adminConnection.Close();
                            break;
                        }
                    }
                }
                catch (IOException)
                {
                    if (serverConnection != null)
                        serverConnection.Close();
                    if (adminConnection != null)
                        adminConnection.Close();
                    serverConnection = null;
                    adminConnection = null;
                }
            }
        }

        private void SendProcessesList()
        {
            Process[] processes = Process.GetProcesses();
            List<string> processNames = new List<string>();
            foreach (var process in processes)
            {
                string name = process.MainWindowTitle.Trim();
                if (!String.IsNullOrEmpty(name))
                    processNames.Add(name);
            }
            adminConnection.SendMessage(CommandSet.Processes, processNames.ToArray());
        }

        private void DisplayMessage()
        {
            string message = adminConnection[0].Trim();
            MessageBox.Show(new Form { TopMost = true }, message, "Wiadomość od prowadzącego", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SendScreenshot()
        {
            adminConnection.SendMessage(CommandSet.Screenshot, ScreenshotProvider.GetScreenshot());
        }

        public void StopWork()
        {
            Debug.WriteLine("Zatrzymano pracę klienta");
            if (serverConnection != null && serverConnection.Connected)
            {
                serverConnection.SendMessage(CommandSet.Disconnect);
                serverConnection.Close();
            }
            if (mainThread != null && mainThread.IsAlive)
                mainThread.Abort();
        }
    }
}
