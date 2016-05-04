using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace SKS_Klient
{
    public class AdminConnection : Connection
    {
        public string Port { get; }
        TcpListener listener;

        public AdminConnection(Settings settings) : base(settings)
        {
            listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            Port = ((IPEndPoint)listener.LocalEndpoint).Port.ToString();
        }

        public void Listen()
        {
            Debug.WriteLine("Oczekiwanie na admina na porcie " + Port);
            while (true)
            {
                client = listener.AcceptTcpClient();
                stream = client.GetStream();
                ReceiveMessage();
                if (Command == CommandSet.AdminConnect)
                {
                    string passwordHash = parameters[0];
                    if (passwordHash  == settings.PasswordHash)
                    {
                        SendMessage(CommandSet.Auth, "SUCCESS");
                        Debug.WriteLine("Połączono admina");
                        return;
                    }
                    SendMessage(CommandSet.Auth, "FAIL");
                }
                Close();
            }
        }
    }
}
