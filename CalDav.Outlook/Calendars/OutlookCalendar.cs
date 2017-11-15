using CalCli.API;
using log4net;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CalDav.Outlook
{
    public class OutlookCalendar : ICalendar, IItemsChangesNotify
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OutlookCalendar));
        ConcurrentDictionary<IEvent, AppointmentItem> itemsMap;
        MAPIFolder localCalendar;
        Application addIn;

        ItemAdded itemAddedHandler;
        ItemChanged itemChangedHandler;
        ItemBeforeDelete itemBeforeDeleteHandler;

        public OutlookCalendar(Application addIn, string name)
        {
            this.addIn = addIn;
            Name = name;
            itemsMap = new ConcurrentDictionary<IEvent, AppointmentItem>();
        }

        public string FullName
        {
            get
            {
                return Name;
            }
        }

        public string Name { get; set; }

        public MAPIFolder LocalCalendar
        {
            get
            {
                if (localCalendar == null)
                {
                    MAPIFolder defaultCalendar = addIn.Session.GetDefaultFolder(OlDefaultFolders.olFolderCalendar);

                    try
                    {
                        foreach (MAPIFolder calendar in defaultCalendar.Folders)
                        {
                            if (calendar.Name.Contains(Name))
                            {
                                localCalendar = calendar;

                                break;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        log.Error(ex.Message);
                    }
                }

                return localCalendar;
            }
        }
                
        public ItemAdded ItemAddedHandler
        {
            get
            {
                return itemAddedHandler;
            }

            set
            {
                itemAddedHandler = value;

                LocalCalendar.Items.ItemAdd += Items_ItemAdd;
            }
        }

        public ItemChanged ItemChangedHandler
        {
            get
            {
                return itemChangedHandler;
            }

            set
            {
                itemChangedHandler = value;

                LocalCalendar.Items.ItemChange += Items_ItemChange;
            }
        }

        public ItemBeforeDelete ItemBeforeDeleteHandler
        {
            get
            {
                return itemBeforeDeleteHandler;
            }

            set
            {
                itemBeforeDeleteHandler = value;

                foreach (AppointmentItem appt in localCalendar.Items)
                {
                    appt.BeforeDelete += AppItem_BeforeDelete;
                }
            }
        }

        public void Delete(IEvent e)
        {
            if(itemsMap.ContainsKey(e))
            {
                AppointmentItem appt;

                itemsMap.TryRemove(e, out appt);

                appt.Delete();
            }            
        }

        public ICollection<IEvent> GetEvents(DateTime? from = null, DateTime? to = null)
        {
            if (from == null && to == null)
            {
                return itemsMap.Keys;
            }

            ICollection<IEvent> list = new List<IEvent>();

            foreach(IEvent e in itemsMap.Keys)
            {
                if(e.Start >= from && e.End <= to)
                {
                    list.Add(e);
                }
            }

            return list;
        }

        public void Save(IEvent e)
        {
            if (!itemsMap.ContainsKey(e))
            {
                AppointmentItem appointment = LocalCalendar.Items.Add(OlItemType.olAppointmentItem);
                appointment = appointment.FromEvent(e);

                appointment.BeforeDelete += AppItem_BeforeDelete;
                itemsMap[e] = appointment;
            }

            itemsMap[e].Save();
        }

        public void Update()
        {
            if (!Exists())
            {
                Create();
            }

            foreach (AppointmentItem appt in LocalCalendar.Items)
            {
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

            try
            {
                MAPIFolder localCalendar = defaultCalendar.Folders.Add(Name,
                    OlDefaultFolders.olFolderCalendar);

                this.localCalendar = localCalendar;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        #region EventsHandlers

        private void Items_ItemAdd(object Item)
        {
            AppointmentItem appItem = (Item as AppointmentItem);
            appItem.BeforeDelete += AppItem_BeforeDelete;

            IEvent localEvent = null;
            foreach (IEvent e in itemsMap.Keys)
            {
                if (e.IsEqual(appItem))
                {
                    localEvent = e;
                    break;
                }
            }

            if(localEvent == null)
            {
                localEvent = appItem.ConvertToEvent();
                itemsMap[localEvent] = appItem;
            }

            ItemAddedHandler(localEvent);
        }

        private void Items_ItemChange(object Item)
        {
            AppointmentItem appItem = Item as AppointmentItem;

            foreach(IEvent e in itemsMap.Keys)
            {
                if (itemsMap[e].EntryID.Equals(appItem.EntryID))
                {
                    IEvent newEvent = appItem.ConvertToEvent();
                    newEvent.UID = e.UID;

                    ItemChangedHandler(newEvent);

                    itemsMap.TryRemove(e, out appItem);
                    itemsMap[newEvent] = appItem;
                    break;
                }
            }            
        }

        private void AppItem_BeforeDelete(object Item, ref bool Cancel)
        {
            AppointmentItem appItem = Item as AppointmentItem;
            
            foreach (IEvent e in itemsMap.Keys)
            {
                if(e.IsEqual(appItem))
                {
                    Cancel = !ItemBeforeDeleteHandler(e);

                    if (!Cancel) itemsMap.TryRemove(e, out appItem);
                    break;
                }
            }
        }

        #endregion
    }
}
