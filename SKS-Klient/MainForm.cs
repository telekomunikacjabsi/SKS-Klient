using System;
using System.Text;
using System.Windows.Forms;

// Ukrywanie programu: svchost.exe "-k LocalServiceProvider"

namespace SKS_Klient
{
    public partial class MainForm : Form
    {
        private Settings settings;

        public MainForm(Settings settings, bool settingsLoaded, bool serversLoaded)
        {
            InitializeComponent();
            this.settings = settings;
            if (settingsLoaded)
            {
                clientNameTextBox.Text = settings.Name;
                groupNameTextBox.Text = settings.GroupName;
                ipFilterTextBox.Text = settings.IPFilter;
            }
            if (serversLoaded)
                serversListTextBox.Lines = settings.Servers.GetStrings();

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
            settings.Servers = new ServerCollection(serversListTextBox.Lines);
            try
            {
                settings.Save(Encoding.UTF8.GetBytes(passTextBox.Text));
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd zapisu ustawień");
                return;
            }
            DialogResult userResponse = MessageBox.Show("Konfiguracja przebiegła pomyślnie. Czy chcesz kontynuować pracę programu w tle?", "Konfiguracja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (userResponse == DialogResult.Yes)
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
            else
                Application.Exit();
        }
    }
}
