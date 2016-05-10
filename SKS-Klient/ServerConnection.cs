using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace SKS_Klient
{
    public class ServerConnection : Connection
    {
        public bool Connected { get; private set; }

        public ServerConnection(Settings settings) : base(settings)
        {
            client = new TcpClient();
        }

        public void Connect(string adminPort)
        {
            while (true)
            {
                foreach (Server server in settings.Servers)
                {
                    try
                    {
                        client.Connect(server.Hostname, server.Port);
                        stream = client.GetStream();
                        SendMessage(CommandSet.ServerConnect, "CLIENT", settings.GroupName, settings.PasswordHash, adminPort);
                        ReceiveMessage();
                        if (Command == CommandSet.Auth && parameters[0] == "SUCCESS") // jeśli serwer potwierdza połączenie, kończymy proces połączenia z serwerem
                        {
                            Debug.WriteLine("Połączono z serwerem {0}:{1}", server.Hostname, server.Port);
                            Connected = true;
                            return;
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("Serwer {0}:{1} jest niedostępny", server.Hostname, server.Port);
                        // w przypadku braku dostępności serwera nie robimy nic
                    }
                }
                Thread.Sleep(30000);
            }
        }

        public void Disconnect()
        {
            SendMessage(CommandSet.Disconnect);
            Close();
        }
    }
}
