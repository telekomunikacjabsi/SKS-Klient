using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SKS_Klient
{
    public class Worker
    {
        ListManager listManager;
        Settings settings;
        Thread mainThread;
        ServerConnection serverConnection;
        AdminConnection adminConnection;
        Watcher watcher;

        public Worker(Settings settings)
        {
            this.settings = settings;
            listManager = new ListManager();
            watcher = new Watcher(listManager);
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
                    VerifyList(ListID.Domains, "domains");
                    VerifyList(ListID.Processes, "processes");
                    adminConnection.ListenAndConnect();
                    serverConnection.Disconnect(); // po uzyskaniu połączenia z administratorem zrywamy połączenie z serwerem
                    while (true) // pętla obsługi komunikatów od admina
                    {
                        watcher.Start();
                        try
                        {
                            // obsługa komunikatów od administratora
                            adminConnection.ReceiveMessage();
                            if (adminConnection.Command == CommandSet.Disconnect)
                            {
                                watcher.Stop();
                                adminConnection.Close();
                                break; // wznawia procedurę nawiązywania połączeń od nowa
                            }
                            else if (adminConnection.Command == CommandSet.Screenshot)
                                SendScreenshot();
                            else if (adminConnection.Command == CommandSet.Message)
                                DisplayMessage();
                            else
                            {
                                watcher.Stop();
                                adminConnection.Close();
                                break;
                            }
                        }
                        catch (IOException) // zerwanie połączenia z adminem
                        {
                            Debug.WriteLine("Rozłączono administratora");
                            adminConnection.Close();
                            watcher.Stop();
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

        private void DisplayMessage()
        {
            string message = adminConnection[0].Trim();
            MessageBox.Show(new Form { TopMost = true }, message, "Wiadomość od prowadzącego", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SendScreenshot()
        {
            adminConnection.SendMessage(CommandSet.Screenshot, ScreenshotProvider.GetScreenshot());
        }

        private void VerifyList(ListID listID, string s)
        {
            serverConnection.SendMessage(CommandSet.VerifyList, ((int)listID).ToString(), listManager.GetListHash(listID));
            serverConnection.ReceiveMessage();
            if (serverConnection.Command == CommandSet.List)
                listManager.SetListFromString(listID, serverConnection[1]);
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
