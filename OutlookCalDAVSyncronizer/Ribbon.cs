using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Tools.Ribbon;
using CalCli.API;
using System.Windows.Forms;
using System.Threading.Tasks;
using CalDav;

namespace OutlookCalDAVSyncronizer
{
    public partial class Ribbon
    {
        //IController outlookController;
        //ICalendar[] calendars;
        //ICalendar currentCalendar;
        //IServer server;

        UI.ConfigWindow configWindow;

        private async void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {            
            configWindow = new UI.ConfigWindow();
        }

        //async Task LoadEvents()
        //{
        //    outlookController.BidirectionalSyncronize();            
        //}

        private async void btnConnect_Click(object sender, RibbonControlEventArgs e)
        {
            DialogResult dialogResult = configWindow.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                CalDav.Outlook.Synchronizer.Get().Start();
            }
        }
        
    }
}
