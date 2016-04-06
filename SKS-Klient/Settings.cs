using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SKS_Klient
{
    class Settings
    {
        string serversFileName = "servers.txt";
        string settingsFileName = "settings.xml";
        public string[] Servers { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string IPFilter { get; set; }
        public bool Startup { get; set; }

        public void LoadServers()
        {
            if (!File.Exists(serversFileName))
                throw new FileNotFoundException();
            Servers = File.ReadAllLines(serversFileName);
            if (Servers == null || Servers.Length == 0)
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
                        else if (currentElement == "AllowedIPsRegex")
                            IPFilter = reader.Value;
                        else if (currentElement == "Startup")
                            Startup = reader.Value == "true";
                    }
                }
            }
        }

        public void Save(string password)
        {
            if (Servers == null || Servers.Length == 0)
                throw new ArgumentException("Brak podanych serwerów");

            /*if (File.Exists(settingsFileName))
                File.Delete(settingsFileName);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            using (XmlWriter writer = XmlWriter.Create("settings.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");
                writer.WriteElementString("Port", "5000");
                writer.WriteElementString("DomainsListPath", "domains.txt");
                writer.WriteElementString("ProcessesListPath", "processes.txt");
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }*/
        }
    }
}