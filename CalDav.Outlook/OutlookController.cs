using System;
using System.Collections.Generic;
using CalCli.API;
using Microsoft.Office.Interop.Outlook;
using System.Threading;
using System.Net.NetworkInformation;
using log4net;

namespace CalDav.Outlook
{
    public class OutlookController : IController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OutlookController));
        #region Fields

        DateTime start;
        DateTime end;
        Application addIn;
        ICalendar outlookCalendar;

        #endregion

        public OutlookController(Application addIn)
        {
            this.addIn = addIn;
            start = new DateTime(2016, 1, 1);
            end = new DateTime(2018, 1, 1);
        }

        #region Properties

        public ICalendar RemoteCalendar
        {
            get
            {
                return Synchronizer.Get().CurrentCalendar;
            }
        }
        public ICalendar OfflineCalendar
        {
            get
            {
                return Synchronizer.Get().OfflineCalendar;
            }
        }

        public ICalendar OutlookCalendar
        {
            get
            {
                if (outlookCalendar == null)
                {
                    outlookCalendar = new OutlookCalendar(addIn, RemoteCalendar.FullName);
                }

                return outlookCalendar;
            }
        }
        #endregion

        public void Initialize()
        {
            if (RemoteCalendar == null || OfflineCalendar == null || OutlookCalendar == null)
            {
                log.Error("Calendars cannot be null");

                return;
            }

            OutlookCalendar.Update();
            OfflineCalendar.Update();

            UpdateEventsUID();

            Syncronize();

            //SetLocalEventsHandlers();
        }

        public void Syncronize()
        {
            if (RemoteCalendar == null || OfflineCalendar == null || OutlookCalendar == null)
            {
                log.Error("Calendars cannot be null");

                return;
            }


            ICollection<IEvent> remoteEvents = RemoteCalendar.GetEvents(start, end);

            List<IEvent> newRemoteEvents = new List<IEvent>(remoteEvents);
            List<IEvent> removedLocalEvents = new List<IEvent>();

            ICollection<IEvent> localEvents = outlookCalendar.GetEvents();

            /* Get new events for both sides calendars */
            foreach (IEvent item in localEvents)
            {
                newRemoteEvents.Remove(item);

                if (!remoteEvents.Contains(item))
                {
                    removedLocalEvents.Add(item);
                }
            }

            /* Save new remote events to the local calendar */
            newRemoteEvents.ForEach(item => OutlookCalendar.Save(item));

            /* Remove deleted on the remote server events from local calendar */
            removedLocalEvents.ForEach(item => OutlookCalendar.Delete(item));

            SetLocalEventsHandlers();
        }

        private void UpdateEventsUID()
        {
            List<IEvent> list = RemoteCalendar.GetEvents(start, end) as List<IEvent>;

            ICollection<IEvent> localEvents = outlookCalendar.GetEvents(start, end);

            foreach (IEvent ev in localEvents)
            {
                if (string.IsNullOrWhiteSpace(ev.UID))
                {
                    IEvent item = list.Find(i => i.Equals(ev));

                    ev.UID = item?.UID;
                }
            }
        }

        private void SetLocalEventsHandlers()
        {
            if (OutlookCalendar is IItemsChangesNotify)
            {
                IItemsChangesNotify notifier = OutlookCalendar as IItemsChangesNotify;

                notifier.ItemAddedHandler = AddToRemote;
                notifier.ItemChangedHandler = AddToRemote;
                notifier.ItemBeforeDeleteHandler = DeleteFromRemote;
            }
        }

        private bool RepeatedDeleteFromRemote(IEvent item)
        {
            int maxRepeats = 3;

            for (int i = 0; i < maxRepeats; i++)
            {
                try
                {
                    RemoteCalendar.Delete(item);

                    return true;
                }
                catch (System.Exception ex)
                {
                    Thread.Sleep((i + 1) * 2000);

                    log.Error(ex.Message);
                }
            }

            return false;
        }

        private bool DeleteFromRemote(IEvent item)
        {
            //if (NetworkInterface.GetIsNetworkAvailable())
            //    return RepeatedDeleteFromRemote(item);
            //else
                OfflineCalendar.Delete(item);

            return true;
        }

        private bool AddToRemote(IEvent item)
        {
            try
            {
                //if (NetworkInterface.GetIsNetworkAvailable())
                //    RemoteCalendar.Save(item);
                //else
                    OfflineCalendar.Save(item);

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
