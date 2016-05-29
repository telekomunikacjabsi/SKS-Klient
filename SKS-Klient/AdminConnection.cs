using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;

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
                        listener.Stop();
                        SendMessage(CommandSet.Auth, "SUCCESS", settings.Name);
                        Debug.WriteLine("Połączono administratora");
                        return;
                    }
                    else
                        SendMessage(CommandSet.Auth, "FAIL");
                }
                else
                    Close();
            }
        }

        public void SendMessage(Command command, byte[] bytes)
        {
            string bytesLenght = bytes.Length.ToString();
            byte[] header = Encoding.UTF8.GetBytes(bytesLenght + ";");
            stream.Write(header, 0, header.Length);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
