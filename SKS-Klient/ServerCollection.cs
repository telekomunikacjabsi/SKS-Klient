using System.Collections;
using System.Collections.Generic;

namespace SKS_Klient
{
    public class ServerCollection : IEnumerable<Server>
    {
        private List<Server> servers;
        public int Count
        {
            get
            {
                return servers.Count;
            }
         }


        public ServerCollection(string[] data)
        {
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

        public string[] GetStrings()
        {
            string[] items = new string[servers.Count];
            for (int i = 0; i < servers.Count; i++)
            {
                items[i] = servers[i].Hostname + ":" + servers[i].Port;
            }
            return items;
        }

        public IEnumerator<Server> GetEnumerator()
        {
            return servers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return servers.GetEnumerator();
        }
    }
}
