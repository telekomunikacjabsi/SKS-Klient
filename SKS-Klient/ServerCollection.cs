using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SKS_Klient
{
    public class ServerCollection : IEnumerator
    {
        private List<Server> servers;
        private int pointer;
        public int Count
        {
            get
            {
                return servers.Count;
            }
         }
        public object Current
        {
            get
            {
                return servers[pointer];
            }
        }

        public ServerCollection(string[] data)
        {
            pointer = 0;
            servers = new List<Server>();
            foreach (string item in data)
            {
                try
                {
                    servers.Add(new Server(item));
                }
                catch { }
            }
        }

        public bool MoveNext()
        {
            pointer++;
            return pointer < servers.Count;
        }

        public void Reset()
        {
            pointer = 0;
        }

        public string[] GetStrings()
        {
            string[] items = new string[servers.Count];
            for (int i = 0; i < servers.Count; i++)
            {
                items[i] = servers[i].Hostname + ":" + servers[i].Port;
            }
            return items;
        }

        private class Server
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
                    throw new ArgumentException("Wrong hostname");
                Port = port;
                Hostname = arguments[0];
                if (Uri.CheckHostName(Hostname) == UriHostNameType.Unknown) // jeśli podana wartość nie jest nazwą domenową albo poprawnym adresem IP
                    throw new ArgumentException("Wrong hostname");
            }
        }
    }
}
