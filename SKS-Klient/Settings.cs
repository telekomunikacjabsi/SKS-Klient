using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace SKS_Klient
{
    public class Settings
    {
        private string serversListFilePath = "servers.txt";
        private string settingsFilePath = "settings.xml";
        public List<Server> Servers { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string IPFilter { get; set; }
        public string PasswordHash { get; private set; }
        public bool Startup { get; set; }

        public void LoadServers()
        {
            if (!File.Exists(serversListFilePath))
                throw new FileNotFoundException();
            Servers = GetServerList(File.ReadAllLines(serversListFilePath));
            if (Servers == null || Servers.Count == 0)
                throw new Exception();
        }

        public List<Server> GetServerList(string[] data)
        {
            List<Server> servers = new List<Server>();
            foreach (string item in data)
            {
                try
                {
                    servers.Add(new Server(item));
                }
                catch { }
            }
            return servers;
        }

        public void LoadSettings()
        {
            if (!File.Exists(settingsFilePath))
                throw new FileNotFoundException();
            using (XmlReader reader = new XmlTextReader(settingsFilePath))
            {
                string currentElement = String.Empty;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                        currentElement = reader.Name;
                    else if (reader.NodeType == XmlNodeType.Text)
                    {
                        if (currentElement == "Name")
                            Name = reader.Value;
                        else if (currentElement == "GroupName")
                            GroupName = reader.Value;
                        else if (currentElement == "Password")
                            PasswordHash = reader.Value;
                        else if (currentElement == "AllowedIPsRegex")
                            IPFilter = reader.Value;
                        else if (currentElement == "Startup")
                            Startup = reader.Value == "true";
                    }
                }
            }
        }

        public void Save(byte[] password)
        {
            Name = Name.Trim();
            if (Name.Length == 0)
                throw new ArgumentException("Nazwa komputera musi zawierać co najmniej 1 znak");

            if (Name.Length > 32)
                throw new ArgumentException("Nazwa komputera musi być krótsza niż 32 znaki");

            GroupName = GroupName.Trim();
            if (GroupName.Length == 0)
                throw new ArgumentException("Nazwa grupy musi zawierać co najmniej 1 znak");

            if (GroupName.Length > 32)
                throw new ArgumentException("Nazwa grupy musi być krótsza niż 32 znaki");

            if (password.Length < 5)
                throw new ArgumentException("Hasło musi zawierać co najmniej 5 znaków");

            IPFilter = IPFilter.Trim();
            if (IPFilter.Length == 0) // pominięcie tego pola spowoduje akcpetację wszystkich adresów IP
                IPFilter = ".*";
            if (!RegexValidator.IsValidRegex(IPFilter))
                throw new ArgumentException("Akceptowane adresy IP muszą mieć postać wyrażenia regularnego");

            if (Servers == null || Servers.Count == 0)
                throw new ArgumentException("Brak podanych serwerów");

            PasswordHash = CalculateSHA256(password, Encoding.UTF8.GetBytes(GroupName));

            if (File.Exists(settingsFilePath))
                File.Delete(settingsFilePath);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            using (XmlWriter writer = XmlWriter.Create("settings.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");
                writer.WriteElementString("Name", Name);
                writer.WriteElementString("GroupName", GroupName);
                writer.WriteElementString("Password", PasswordHash);
                writer.WriteElementString("AllowedIPsRegex", IPFilter);
                writer.WriteElementString("Startup", Startup.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            File.WriteAllLines(serversListFilePath, GetServerStrings(Servers));
        }

        public string[] GetServerStrings(List<Server> servers)
        {
            string[] items = new string[servers.Count];
            for (int i = 0; i < servers.Count; i++)
            {
                items[i] = servers[i].Hostname + ":" + servers[i].Port;
            }
            return items;
        }

        private string CalculateSHA256(byte[] text, byte[] salt)
        {
            SHA256Managed crypt = new SHA256Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(text.Concat(salt).ToArray(), 0, text.Length + salt.Length);
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}