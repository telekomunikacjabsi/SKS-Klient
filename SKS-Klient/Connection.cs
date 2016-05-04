using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace SKS_Klient
{
    public abstract class Connection
    {
        public Command Command { get; private set; }
        protected TcpClient client;
        protected NetworkStream stream;
        protected string[] parameters;
        protected Settings settings;

        public Connection(Settings settings)
        {
            this.settings = settings;
        }

        public void ReceiveMessage()
        {
            Command = new Command(String.Empty);
            int i;
            byte[] bytes = new byte[256];
            parameters = null;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                string msg = Encoding.UTF8.GetString(bytes, 0, i);
                string[] args = Regex.Split(msg, ";"); // automatyczny podział komunikatu na argumenty
                if (args.Length > 1)
                {
                    parameters = new string[args.Length - 1];
                    for (int j = 1; j < args.Length; j++)
                        parameters[j - 1] = args[j].Trim();
                }
                if (args.Length > 0)
                {
                    if (parameters != null && parameters.Length > 0)
                        Command = new Command(args[0].Trim(), parameters.Length);
                    else
                        Command = new Command(args[0].Trim());
                }
                return;
            }
        }

        public void SendMessage(Command command, params string[] parameters)
        {
            if (command.ParametersCount > 0 && command.ParametersCount != parameters.Length) // zabezpiecza przed wysłaniem niekompatybilnego komunikatu
                throw new ArgumentOutOfRangeException("Command", "Command has less or more parameters than required");
            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = parameters[i].Trim();
            byte[] bytes = Encoding.UTF8.GetBytes(String.Join(";", command.Text, String.Join(";", parameters)));
            stream.Write(bytes, 0, bytes.Length);
        }

        public void Close()
        {
            stream.Close();
            client.Close();
        }

        public string this[int index]
        {
            get
            {
                return parameters[index];
            }
        }
    }
}
