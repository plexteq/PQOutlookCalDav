using CalDav.Outlook;

namespace OutlookCalDAVSyncronizer
{
    public partial class ThisAddIn
    {
        ISynchronizer syncronizer;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            syncronizer = Synchronizer.Get(Globals.ThisAddIn.Application);

            if (Config.Instance.IsSync)
            {
                syncronizer.Autostart = true;
                syncronizer.Start();
            }                                     
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
        }


        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
