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
        private string domainsListChecksum;
        private string processesListChecksum;

        public ListManager()
        {
            if (File.Exists(domainsListPath))
            {
                disallowedDomains = File.ReadAllLines(domainsListPath, Encoding.UTF8).Where(value => RegexValidator.IsValidRegex(value)).ToArray(); // wybieramy tylko reguły które są poprawnymi wyrażeniami regularnymi
                domainsListChecksum = CalculateMD5Hash(disallowedDomains);
            }
            else
            {
                disallowedDomains = new string[] { String.Empty };
                domainsListChecksum = "0";
            }
            if (File.Exists(processesListPath))
            {
                disallowedProcesses = File.ReadAllLines(processesListPath, Encoding.UTF8).Where(value => RegexValidator.IsValidRegex(value)).ToArray();
                processesListChecksum = CalculateMD5Hash(disallowedProcesses);
            }
            else
            {
                disallowedProcesses = new string[] { String.Empty };
                processesListChecksum = "0";
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

        private string CalculateMD5Hash(string[] array)
        {
            string input = StringArrayToString(array);
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private string StringArrayToString(string[] array)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append('.');
            }
            return builder.ToString();
        }

        public string GetListHash(ListID listID)
        {
            if (listID == ListID.Domains)
                return domainsListChecksum;
            else if (listID == ListID.Processes)
                return processesListChecksum;
            return "0";
        }

        public bool VerifyList(ListID listID, string checksum)
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
