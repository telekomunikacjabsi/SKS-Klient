using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SKS_Klient
{
    public class Worker
    {
        ListManager listManager;
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
            Debug.WriteLine(settings.PasswordHash);
            listManager = new ListManager();
            while (true)
            {
                try
                {
                    serverConnection = new ServerConnection(settings);
                    VerifyList(ListID.Domains);
                    VerifyList(ListID.Processes);
                    adminConnection = new AdminConnection(settings);
                    serverConnection.SendMessage(CommandSet.Port, adminConnection.Port);
                    adminConnection.Listen();
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
                                break;
                            }
                        }
                        catch (IOException) // zerwanie połączenia z adminem
                        {
                            adminConnection.Close();
                            break;
                        }
                    }
                }
                catch (IOException) // jeśli dojdzie do nagłego rozłączenia to próbujemy od nowa
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

        private void VerifyList(ListID listID)
        {
            serverConnection.SendMessage(CommandSet.VerifyList, ((int)listID).ToString(), listManager.GetListHash(listID));
            serverConnection.ReceiveMessage();
            if (serverConnection.Command == CommandSet.List)
            {
                listManager.SetListFromString(listID, serverConnection[1]);
                Console.WriteLine(serverConnection[1]);
            }
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
