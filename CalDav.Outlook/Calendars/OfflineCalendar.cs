using CalCli.API;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace CalDav.Outlook
{
    public class OfflineCalendar : ICalendar
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OfflineCalendar));

        const string itemsQueueTableName = "ItemsQueue";

        Thread workerThread;
        ConcurrentQueue<IDataItem> itemsQueue;

        IDataProvider dbProvider;

        public OfflineCalendar(ICalendar RemoteCalendar, IDataProvider dbProvider)
        {
            itemsQueue = new ConcurrentQueue<IDataItem>();

            this.RemoteCalendar = RemoteCalendar;

            DbProvider = dbProvider;

            UpdatingInterval = 10;//interval in seconds

            workerThread = new Thread(DoWork);
        }

        ~OfflineCalendar()
        {
            workerThread.Abort();
        }

        ICalendar RemoteCalendar { get; set; }

        public int UpdatingInterval { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string FullName {
            get {
                if (RemoteCalendar != null)
                    return RemoteCalendar.FullName;

                return null;
            }
        }

        public IDataProvider DbProvider {
            get {
                return dbProvider;
            }

            private set {
                dbProvider = value;
            }
        }

        public void Save(IEvent e)
        {
            AddToQueue(e, Action.RemoteAdd);
        }

        public void Delete(IEvent e)
        {
            AddToQueue(e, Action.RemoteDelete);
        }

        private void AddToQueue(IEvent e, Action action)
        {
            try {
                if (e != null && !itemsQueue.Select(item => item.Event).Contains(e)) {
                    IDataItem item = new DBItem(e, action);

                    itemsQueue.Enqueue(item);
                    DbProvider.Add(item, itemsQueueTableName);
                }
            }
            catch (Exception ex) {
                log.Error(ex.Message);
            }
        }

        private delegate void HandleQueueItem(IEvent e);


        private void Load()
        {
            itemsQueue = DbProvider.Load(itemsQueueTableName);
        }

        public void DoWork()
        {
            log.Info("Start the worker thread for a local items handling.");

            while (true) {
                Thread.Sleep(UpdatingInterval * 1000);

                HandleQueues();
            }
        }

        private void HandleQueues()
        {
            if (NetworkInterface.GetIsNetworkAvailable()) {
                IDataItem item;

                while (!itemsQueue.IsEmpty) {
                    if (itemsQueue.TryDequeue(out item)) {
                        try {
                            switch (item.EventAction) {
                                case Action.RemoteAdd:
                                    RemoteCalendar.Save(item.Event);
                                    DbProvider.Remove(item, itemsQueueTableName);
                                    break;
                                case Action.RemoteDelete:
                                    RemoteCalendar.Delete(item.Event);
                                    DbProvider.Remove(item, itemsQueueTableName);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(
                                        "Tuple<IEvent, Action>.Item2",
                                        Enum.GetName(typeof(Action), item.EventAction),
                                        "Unknown action for the event");
                            }

                        }
                        catch (Exception ex) {
                            itemsQueue.Enqueue(item);
                            log.Error(ex.Message);
                            log.Error(string.Format("Item was returned to the queue - {0}", item.Event.Summary));
                        }
                    }
                }
            }
        }

        public void Update()
        {
            if (!workerThread.IsAlive) {
                DbProvider.CreateIfNotExist(new string[] { itemsQueueTableName });
                Load();

                log.Info("Handling items from a DB...");
                HandleQueues();
                log.Info("Items handled successful");

                workerThread.Start();
            }
        }

        #region Unsupported

        public ICollection<IEvent> GetEvents(DateTime? from = default(DateTime?), DateTime? to = default(DateTime?))
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
