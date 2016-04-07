using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SKS_Klient
{
    public class Settings
    {
        private string serversFileName = "servers.txt";
        private string settingsFileName = "settings.xml";
        public ServerCollection Servers { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string IPFilter { get; set; }
        public string PasswordHash { get; private set; }
        public bool Startup { get; set; }

        public void LoadServers()
        {
            if (!File.Exists(serversFileName))
                throw new FileNotFoundException();
            Servers = new ServerCollection(File.ReadAllLines(serversFileName));
            if (Servers == null || Servers.Count == 0)
                throw new Exception();
        }

        public void LoadSettings()
        {
            if (!File.Exists(settingsFileName))
                throw new FileNotFoundException();
            using (XmlReader reader = new XmlTextReader(settingsFileName))
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
                throw new ArgumentException("Nazwa komputera musi zawierać conajmniej 1 znak");

            GroupName = GroupName.Trim();
            if (Name.Length == 0)
                throw new ArgumentException("Nazwa grupy musi zawierać conajmniej 1 znak");

            if (password.Length < 5)
                throw new ArgumentException("Hasło musi zawierać conajmniej 5 znaków");

            IPFilter = IPFilter.Trim();
            if (IPFilter.Length == 0) // pominięcie tego pola spowoduje akcpetację wszystkich adresów IP
                IPFilter = ".*";
            if (!RegexValidator.IsValidRegex(IPFilter))
                throw new ArgumentException("Akceptowane adresy IP muszą mieć postać wyrażenia regularnego");

            if (Servers == null || Servers.Count == 0)
                throw new ArgumentException("Brak podanych serwerów");

            PasswordHash = CalculateSHA256(password, Encoding.UTF8.GetBytes(GroupName));

            if (File.Exists(settingsFileName))
                File.Delete(settingsFileName);
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

            File.WriteAllLines(serversFileName, Servers.GetStrings());
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