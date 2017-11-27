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

        public ITransactionLog TransactionLog {
            get {
                return Synchronizer.Get().TransactionLog;
            }
        }
                
        public ICalendar OutlookCalendar
        {
            get
            {
                return Synchronizer.Get().OutlookCalendar;
            }
        }
        #endregion

        public void Initialize()
        {
            if (RemoteCalendar == null ||  OutlookCalendar == null)
            {
                log.Error("Calendars cannot be null");

                return;
            }

            OutlookCalendar.Update();

            UpdateEventsUID();

            Syncronize();

            //SetLocalEventsHandlers();
        }

        public void Syncronize()
        {
            if (RemoteCalendar == null ||  OutlookCalendar == null)
            {
                log.Error("Calendars cannot be null");

                return;
            }
            
            ICollection<IEvent> remoteEvents = RemoteCalendar.GetEvents(start, end);

            List<IEvent> newRemoteEvents = new List<IEvent>(remoteEvents);
            List<IEvent> removedLocalEvents = new List<IEvent>();

            ICollection<IEvent> localEvents = OutlookCalendar.GetEvents();

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
            newRemoteEvents.ForEach(item => TransactionLog.Add(item, Action.LocalAdd));//  OutlookCalendar.Save(item));

            /* Remove deleted on the remote server events from local calendar */
            removedLocalEvents.ForEach(item => TransactionLog.Add(item, Action.LocalDelete));// OutlookCalendar.Delete(item));

            //SetLocalEventsHandlers();
        }

        private void UpdateEventsUID()
        {
            List<IEvent> list = RemoteCalendar.GetEvents(start, end) as List<IEvent>;

            ICollection<IEvent> localEvents = OutlookCalendar.GetEvents(start, end);

            foreach (IEvent ev in localEvents)
            {
                if (string.IsNullOrWhiteSpace(ev.UID))
                {
                    IEvent item = list.Find(i => i.Equals(ev));

                    ev.UID = item?.UID;
                }
            }
        }
    }
}
