using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;

namespace SKS_Klient
{
    public class AdminConnection : Connection
    {
        public string Port { get; } // port na którym do klienta może podłączyć się klient
        TcpListener listener;

        public AdminConnection(Settings settings) : base(settings)
        {
            listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            Port = ((IPEndPoint)listener.LocalEndpoint).Port.ToString();
        }

        public void ListenAndConnect()
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
                        SendMessage(CommandSet.Auth, "SUCCESS", settings.Name);
                        Debug.WriteLine("Połączono administratora");
                        return;
                    }
                    SendMessage(CommandSet.Auth, "FAIL");
                }
                Close();
            }
        }

        public void SendMessage(Command command, byte[] bytes)
        {
            byte[] commandBytes = Encoding.UTF8.GetBytes(command.Text + ";");
            bytes = commandBytes.Concat(bytes).ToArray<byte>();
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
