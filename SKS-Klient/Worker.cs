using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKS_Klient
{
    class Worker
    {
        Settings settings;
        Thread mainThread;
        TcpClient serverConnection;
        NetworkStream stream;

        public Worker(Settings settings)
        {
            this.settings = settings;
            mainThread = new Thread(DoWork);
            mainThread.Start();
        }

        private void DoWork()
        {
            serverConnection = ConnectWithServer();
            stream = serverConnection.GetStream();
            WriteMessage("CONNECT", "CLIENT", settings.GroupName, settings.PasswordHash);
            // dalsza obsługa klienta
        }

        private TcpClient ConnectWithServer()
        {
            while (true)
            {
                foreach (Server server in settings.Servers)
                {
                    try
                    {
                        return new TcpClient(server.Hostname, server.Port);
                    }
                    catch
                    {
                        // w przypadku braku dostępności serwera nie robimy nic
                    }
                }
                Thread.Sleep(30000);
            }
        }

        public void StopWork()
        {
            if (serverConnection != null && serverConnection.Connected)
            {
                WriteMessage("DISCONNECT"); // POPRAWIC WYSYLANIE KOMUNIKATU DISCONNECT
                stream.Close();
                serverConnection.Close();
            }
            if (mainThread != null && mainThread.IsAlive)
                mainThread.Abort();
        }

        private string[] ReceiveMessage()
        {
            int i;
            byte[] bytes = new byte[256];
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                string msg = Encoding.ASCII.GetString(bytes, 0, i);
                string[] args = Regex.Split(msg, ";"); // automatyczny podział komunikatu na argumenty
                for (int j = 0; j < args.Length; j++)
                    args[j] = args[j].Replace("&sem", ";");
                return args;
            }
            return new string[] { String.Empty };
        }

        private void WriteMessage(params string[] message)
        {
            for (int i = 0; i < message.Length; i++)
                message[i] = message[i].Replace(";", "&sem"); // usuwa średnik z wiadomości ze względu na ich użycie przy podziale komunikatów
            _WriteMessage(String.Join(";", message));
        }

        private void _WriteMessage(string message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
