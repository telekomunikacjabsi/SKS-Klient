using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SKS_Klient
{
    public class ListManager
    {
        private const string domainsListPath = "domains.txt";
        private const string processesListPath = "processes.txt";
        private string[] disallowedDomains;
        private string[] disallowedProcesses;
        private byte[] domainsListChecksum;
        private byte[] processesListChecksum;

        public ListManager()
        {
            if (File.Exists(domainsListPath))
            {
                disallowedDomains = File.ReadAllLines(domainsListPath, Encoding.UTF8).Where(value => RegexValidator.IsValidRegex(value)).ToArray(); // wybieramy tylko reguły które są poprawnymi wyrażeniami regularnymi
                domainsListChecksum = CalculateMD5(disallowedDomains);
            }
            else
            {
                disallowedDomains = new string[] { String.Empty };
                domainsListChecksum = new byte[] { 0 }; // inicjalizujemy hash jako "pusty"
            }
            if (File.Exists(processesListPath))
            {
                disallowedProcesses = File.ReadAllLines(processesListPath, Encoding.UTF8).Where(value => RegexValidator.IsValidRegex(value)).ToArray();
                processesListChecksum = CalculateMD5(disallowedProcesses);
            }
            else
            {
                disallowedProcesses = new string[] { String.Empty };
                processesListChecksum = new byte[] { 0 };
            }
        }

        public bool IsForbiddenAction(ListID listID, string s)
        {
            string[] list = null;
            if (listID == ListID.Domains)
                list = disallowedDomains;
            else if (listID == ListID.Processes)
                list = disallowedProcesses;
            if (list == null || list.Length == 0)
                return false;
            for (int i = 0; i < list.Length; i++)
            {
                if (Regex.IsMatch(list[i], s))
                    return true;
            }
            return false;
        }

        private byte[] CalculateMD5(string[] lines)
        {
            string sum = String.Join(String.Empty, lines);
            var checkSum = MD5.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(sum.ToString());
            return checkSum.ComputeHash(bytes);
        }

        public string GetListHash(ListID listID)
        {
            if (listID == ListID.Domains)
                return Encoding.UTF8.GetString(domainsListChecksum);
            else if (listID == ListID.Processes)
                return Encoding.UTF8.GetString(processesListChecksum);
            return "0";
        }

        public bool VerifyList(int listID, string checksum)
        {
            return VerifyList((ListID)listID, Encoding.UTF8.GetBytes(checksum));
        }

        public bool VerifyList(ListID listID, byte[] checksum)
        {
            if (listID == ListID.Domains)
                return checksum == domainsListChecksum;
            else if (listID == ListID.Processes)
                return checksum == processesListChecksum;
            return false;
        }

        public void SetListFromString(ListID listID, string listString)
        {
            listString = listString.Trim();
            if (listID == ListID.Domains)
            {
                disallowedDomains = listString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                File.WriteAllText(domainsListPath, listString, Encoding.UTF8);
            }
            else if (listID == ListID.Processes)
            {
                disallowedProcesses = listString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                File.WriteAllText(processesListPath, listString, Encoding.UTF8);
            }
        }

        public int GetListID(string s)
        {
            int id;
            bool success = Int32.TryParse(s, out id);
            if (success)
            {
                if (Enum.IsDefined(typeof(ListID), id))
                    return id;
                else return -1;
            }
            else
                return -1;
        }
    }

    public enum ListID
    { Domains, Processes };
}
