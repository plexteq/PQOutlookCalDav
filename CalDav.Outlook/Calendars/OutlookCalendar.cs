using CalCli.API;
using log4net;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CalDav.Outlook
{
    public class OutlookCalendar : ICalendar, ILoggable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OutlookCalendar));
        ConcurrentDictionary<IEvent, AppointmentItem> itemsMap;
        MAPIFolder localCalendar;
        Application addIn;
        ITransactionLog transactionLog;
        Items _items;

        public OutlookCalendar(Application addIn, string name)
        {
            this.addIn = addIn;
            Name = name;
            itemsMap = new ConcurrentDictionary<IEvent, AppointmentItem>();
        }

        public string FullName {
            get {
                return Name;
            }
        }

        public string Name { get; set; }

        public MAPIFolder LocalCalendar {
            get {
                if (localCalendar == null) {
                    MAPIFolder defaultCalendar = addIn.Session.GetDefaultFolder(OlDefaultFolders.olFolderCalendar);

                    try {
                        foreach (MAPIFolder calendar in defaultCalendar.Folders) {
                            string name = calendar.Name;

                            if (calendar.Name.Contains(Name)) {
                                localCalendar = calendar;

                                _items = localCalendar.Items;

                                _items.ItemAdd += new ItemsEvents_ItemAddEventHandler(Items_ItemAdd);
                                _items.ItemChange += new ItemsEvents_ItemChangeEventHandler(Items_ItemChange);
                                
                                foreach (AppointmentItem appt in _items) {
                                    appt.BeforeDelete += new ItemEvents_10_BeforeDeleteEventHandler(AppItem_BeforeDelete);
                                }

                                break;
                            }
                        }
                    }
                    catch (System.Exception ex) {
                        log.Error(ex.Message);
                    }
                }

                return localCalendar;
            }
        }

        public ITransactionLog TransactionLog {
            get {
                return transactionLog;
            }

            set {
                transactionLog = value;
            }
        }

        public void Delete(IEvent e)
        {
            if (itemsMap.ContainsKey(e)) {
                try {
                    AppointmentItem appt;

                    itemsMap.TryRemove(e, out appt);

                    appt.Delete();
                }
                catch (System.Exception ex) {
                    log.Error(ex.Message);
                }
            }
        }

        public ICollection<IEvent> GetEvents(DateTime? from = null, DateTime? to = null)
        {
            if (from == null && to == null) {
                return itemsMap.Keys;
            }

            ICollection<IEvent> list = new List<IEvent>();

            foreach (IEvent e in itemsMap.Keys) {
                if (e.Start >= from && e.End <= to) {
                    list.Add(e);
                }
            }

            return list;
        }

        public void Save(IEvent e)
        {
            if (!itemsMap.ContainsKey(e)) {
                AppointmentItem appointment = _items.Add(OlItemType.olAppointmentItem);
                appointment = appointment.FromEvent(e);
                
                itemsMap[e] = appointment;
            }

            itemsMap[e].Save();
        }

        public void Update()
        {
            if (!Exists()) {
                Create();
            }

            foreach (AppointmentItem appt in LocalCalendar.Items) {
                itemsMap[appt.ConvertToEvent()] = appt;
            }
        }

        private bool Exists()
        {
            return LocalCalendar != null;
        }

        private void Create()
        {
            MAPIFolder defaultCalendar = addIn.Session.GetDefaultFolder(OlDefaultFolders.olFolderCalendar);

            try {
                MAPIFolder localCalendar = defaultCalendar.Folders.Add(Name,
                    OlDefaultFolders.olFolderCalendar);

                this.localCalendar = localCalendar;
            }
            catch (System.Exception ex) {
                log.Error(ex.Message);
            }
        }

        #region EventsHandlers

        private void Items_ItemAdd(object Item)
        {
            try {
                AppointmentItem appItem = Item as AppointmentItem;

                IEvent localEvent = null;
                foreach (IEvent e in itemsMap.Keys) {
                    if (e.IsEqual(appItem)) {
                        localEvent = e;
                        break;
                    }
                }

                if (localEvent == null) {
                    appItem.BeforeDelete += new ItemEvents_10_BeforeDeleteEventHandler(AppItem_BeforeDelete);
                    localEvent = appItem.ConvertToEvent();
                    itemsMap[localEvent] = appItem;
                }

                TransactionLog.Add(localEvent, Action.RemoteAdd);
            }
            catch (System.Exception ex) {
                log.Error(ex.Message);
            }
        }

        private void Items_ItemChange(object Item)
        {
            AppointmentItem appItem = Item as AppointmentItem;

            foreach (IEvent e in itemsMap.Keys) {
                try {
                    AppointmentItem storedAppItem = itemsMap[e];

                    if (storedAppItem == null) {
                        throw new KeyNotFoundException(string.Format("{0} cannot be found in the items map", e));
                    }

                    if (storedAppItem.EntryID.Equals(appItem.EntryID)) {
                        IEvent newEvent = appItem.ConvertToEvent();
                        newEvent.UID = e.UID;

                        TransactionLog.Add(newEvent, Action.RemoteAdd);

                        itemsMap.TryRemove(e, out appItem);
                        itemsMap[newEvent] = appItem;
                        break;
                    }
                }
                catch (System.Exception ex) {
                    log.Error(ex.Message);
                }
            }
        }

        private void AppItem_BeforeDelete(object Item, ref bool Cancel)
        {
            AppointmentItem appItem = Item as AppointmentItem;

            foreach (IEvent e in itemsMap.Keys) {
                try {
                    if (e.IsEqual(appItem)) {
                        TransactionLog.Add(e, Action.RemoteDelete);
                        break;
                    }
                }
                catch (System.Exception ex) {
                    log.Error(ex.Message);
                }
            }
        }

        #endregion
    }
}
