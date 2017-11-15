using System;
using System.Windows.Forms;
using CalCli.API;
using System.Threading.Tasks;
using CalDav.Outlook;

namespace OutlookCalDAVSyncronizer.UI
{
    public partial class ConfigWindow : Form
    {
        Synchronizer synchronizer;

        public ConfigWindow()
        {
            InitializeComponent();
            SetBindings();
            synchronizer = Synchronizer.Get();

            if(Config.Instance.IsSync)
            {
                UpdateCalendarsList();
            }
        }

        private void SetBindings()
        {
            passwordTextBox.DataBindings.Add("Text", Config.Instance, "Passw");
            usernameTextBox.DataBindings.Add("Text", Config.Instance, "Username");
            urlCombo.DataBindings.Add("Text", Config.Instance, "Url");
            syncTimeTextBox.DataBindings.Add("Text", Config.Instance, "SyncTimeSeconds");
        }

        #region Events
        private void Main_Load(object sender, EventArgs e)
        {
            //ApplyConfig();
            toolStripStatusLabel.Text = "";
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                toolStripStatusLabel.Text = "Connecting...";
                //UpdateConfig();

                await ConnectAndUpdateCalendarsList();
            }
        }

        private async void btnSync_Click(object sender, EventArgs e)
        {
            Config.Instance.Calendar = cmbCallendars.Text;
            Config.Instance.IsSync = true;
            await Config.Serialize();
        }

        #endregion

        private async Task ConnectAndUpdateCalendarsList()
        {
            synchronizer.Autostart = false;
            synchronizer.Connect();
            UpdateCalendarsList();
        }

        bool ValidateData()
        {
            int syncMin = 0;

            toolStripStatusLabel.Text = 
                string.IsNullOrWhiteSpace(urlCombo.Text) ? "Please, enter the calendar URL" :
                string.IsNullOrWhiteSpace(passwordTextBox.Text) ? "Please, enter the password" :
                string.IsNullOrWhiteSpace(usernameTextBox.Text) ? "Please, enter the user name" :
                (!int.TryParse(syncTimeTextBox.Text, out syncMin) || syncMin <= 0) ? "The syncronization time must be a valid integer" : "";

            bool isValid = string.IsNullOrWhiteSpace(toolStripStatusLabel.Text);
            
            return isValid;
        }

        private void UpdateConfig()
        {
            Config.Instance.Url = urlCombo.Text;
            Config.Instance.Username = usernameTextBox.Text;
            Config.Instance.Passw = passwordTextBox.Text;
            Config.Instance.SyncTimeSeconds = int.Parse(syncTimeTextBox.Text);
        }

        private void ApplyConfig()
        {
            urlCombo.Text = Config.Instance.Url ?? "";
            usernameTextBox.Text = Config.Instance.Username ?? "";
            passwordTextBox.Text = Config.Instance.Passw ?? "";
            cmbCallendars.Text = Config.Instance.Calendar ?? "";
            syncTimeTextBox.Text = Config.Instance.SyncTimeSeconds.ToString() ?? "";
        }

        private void UpdateCalendarsList()
        {
            if (synchronizer.Calendars != null)
            {
                cmbCallendars.Items.Clear();
                foreach (ICalendar calendar in synchronizer.Calendars)
                {
                    cmbCallendars.Items.Add(calendar.Name);
                }

                if (cmbCallendars.Items.Count > 0)
                {
                    cmbCallendars.Text =
                        string.IsNullOrWhiteSpace(Config.Instance.Calendar) ?
                        (string)cmbCallendars.Items[0] : Config.Instance.Calendar;
                }

                btnSync.Enabled = true;
                cmbCallendars.Enabled = true;
                toolStripStatusLabel.Text = "Connected";
            }
            else
            {
                toolStripStatusLabel.Text = "Cannot load a calendars list for this account";
            }
        }

        
    }
}
