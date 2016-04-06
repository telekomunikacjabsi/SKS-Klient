using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKS_Klient
{
    public partial class MainForm : Form
    {
        private Settings settings;

        public MainForm()
        {
            InitializeComponent();
            settings = new Settings();
            bool serversLoaded = false;
            bool settingsLoaded = false;
            try
            {
                settings.LoadServers();
                serversLoaded = true;
            }
            catch
            { }
            try
            {
                settings.LoadSettings();
                settingsLoaded = true;
            }
            catch
            { }
            if (serversLoaded && serversLoaded) // jeśli wszystkie ustawienia są znane
                StartWork();
            else
            {
                if (settingsLoaded)
                {
                    clientNameTextBox.Text = settings.Name;
                    groupNameTextBox.Text = settings.GroupName;
                    ipFilterTextBox.Text = settings.IPFilter;
                }
                if (serversLoaded)
                    serversListTextBox.Lines = settings.Servers;
            }
        }

        private void HideWindow()
        {
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        }

        private void StartWork()
        {
            HideWindow();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (passTextBox.Text != confirmPassTextBox.Text)
            {
                MessageBox.Show("Podane hasła są różne!", "Błąd zapisu ustawień");
                return;
            }
            settings.Name = clientNameTextBox.Text;
            settings.GroupName = groupNameTextBox.Text;
            settings.IPFilter = ipFilterTextBox.Text;
            settings.Startup = startupCheckBox.Checked;
            settings.Servers = serversListTextBox.Lines;
            try
            {
                settings.Save(passTextBox.Text);
                StartWork();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd zapisu ustawień");
            }
        }
    }
}
