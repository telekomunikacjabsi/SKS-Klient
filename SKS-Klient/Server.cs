using System;
using System.Text.RegularExpressions;

namespace SKS_Klient
{
    public class Server
    {
        public string Hostname { get; private set; }
        public int Port { get; private set; }

        public Server(string data)
        {
            data = data.Trim().Replace(" ", "");
            string[] arguments = Regex.Split(data, ":");
            if (arguments.Length != 2)
                throw new ArgumentException("Wrong hostname");
            int port;
            if (!Int32.TryParse(arguments[1], out port) || port < 1 || port > 65535)
                throw new ArgumentException("Wrong port");
            Port = port;
            Hostname = arguments[0];
            UriHostNameType hostnameType = Uri.CheckHostName(Hostname);
            if (hostnameType == UriHostNameType.Unknown) // jeśli podana wartość nie jest nazwą domenową albo poprawnym adresem IP
                throw new ArgumentException("Wrong hostname");
            else if (hostnameType == UriHostNameType.IPv4 || hostnameType == UriHostNameType.IPv6)
            {
                string IPv4Regex = "^([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\\.([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\\.([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\\.([01]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])$";
                string IPv6Regex = "^((?:[0-9A-Fa-f]{1,4}))((?::[0-9A-Fa-f]{1,4}))*::((?:[0-9A-Fa-f]{1,4}))((?::[0-9A-Fa-f]{1,4}))*|((?:[0-9A-Fa-f]{1,4}))((?::[0-9A-Fa-f]{1,4})){7}$";
                if (!Regex.IsMatch(Hostname, IPv4Regex) && !Regex.IsMatch(Hostname, IPv6Regex))
                    throw new ArgumentException("Wrong hostname");
            }
        }
    }
}
