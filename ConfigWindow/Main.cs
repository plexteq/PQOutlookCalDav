using System;
using System.Linq;
using System.Windows.Forms;
//using CalCli.UI;
using CalCli.Connections;
using System.IO;
using CalCli.API;
using CalCli;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using CalCli.Util;
using CalDav;
using System.Threading.Tasks;

namespace CalCli.UI
{
    public partial class ConfigWindow : Form
    {
        static private ConfigWindow instance;

        public ConfigWindow()
        {
            InitializeComponent();
        }
        
        #region Events
        private void Main_Load(object sender, EventArgs e)
        {
            ApplyConfig();
        }

        private void usernameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            UpdateConfig();
            
            UpdateCalendarsList();
            await Config.Serialize();
        }

        private async void btnAutoConfig_Click(object sender, EventArgs e)
        {
            UpdateCalendarsList();
            await Config.Serialize();
        }

        private async void btnSync_Click(object sender, EventArgs e)
        {

            Config.Instance.Calendar = cmbCallendars.Text;
            Config.Instance.IsSync = true;
            await Config.Serialize();
        }

        #endregion

        private void UpdateConfig()
        {
            Config.Instance.Url = urlCombo.Text;
            Config.Instance.Username = usernameTextBox.Text;
            Config.Instance.Passw = passwordTextBox.Text;
        }

        private void UpdateCalendarsList()
        {
            try
            {
                Calendars = Server.GetCalendars();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Could not login.");
                return;
            }
            cmbCallendars.Items.Clear();
            foreach (ICalendar calendar in Calendars)
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
        }
        
        private void ApplyConfig()
        {
            urlCombo.Text = Config.Instance.Url ?? "";
            usernameTextBox.Text = Config.Instance.Username ?? "";
            passwordTextBox.Text = Config.Instance.Passw ?? "";
            cmbCallendars.Text = Config.Instance.Calendar ?? "";
        }

        private CalendarTypes calendarTypeFromText(string text)
        {
            if (text == "Google")
                return CalendarTypes.Google;
            if (text == "iCloud")
                return CalendarTypes.iCloud;
            if (text == "Yahoo")
                return CalendarTypes.Yahoo;
            if (text == "Outlook")
                return CalendarTypes.Outlook;
            return CalendarTypes.Google;
        }

        private void refreshGoogleToken()
        {
            //GoogleOAuthForm form = new GoogleOAuthForm();
            //form.ShowDialog();
            //connection = new GoogleConnection(form.Result.Token);
            //StreamWriter sw = new StreamWriter("token");
            //sw.WriteLine(form.Result.Token);
            //sw.Close();
        }        

    }
}
