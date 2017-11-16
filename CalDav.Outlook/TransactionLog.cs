using CalCli.API;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace CalDav.Outlook
{
    public class TransactionLog : ITransactionLog
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TransactionLog));

        const string itemsQueueTableName = "ItemsQueue";

        Thread workerThread;
        ConcurrentQueue<IDataItem> itemsQueue;

        public TransactionLog(
            ICalendar remoteCal,
            ICalendar localCal,
            IDataProvider dataProvider,
            int updatingInterval)
        {
            itemsQueue = new ConcurrentQueue<IDataItem>();
            workerThread = new Thread(DoWork);

            RemoteCalendar = remoteCal;
            LocalCalendar = localCal;
            DbProvider = dataProvider;
            UpdatingInterval = updatingInterval;
        }

        public TransactionLog(
            ICalendar remoteCal,
            ICalendar localCal,
            IDataProvider dataProvider)
            : this(remoteCal, localCal, dataProvider, 10)
        {
        }

        ICalendar remoteCalendar;
        ICalendar RemoteCalendar {
            get {
                return remoteCalendar;
            }

            set {
                if (value == null)
                    throw new ArgumentNullException("RemoteCalendar object can`t be null");

                remoteCalendar = value;
            }
        }

        ICalendar localCalendar;
        ICalendar LocalCalendar {
            get {
                return localCalendar;
            }

            set {
                if (value == null)
                    throw new ArgumentNullException("LocalCalendar object can`t be null");

                localCalendar = value;
            }
        }

        IDataProvider dbProvider;
        public IDataProvider DbProvider {
            get {
                return dbProvider;
            }

            private set {
                if (value == null)
                    throw new ArgumentNullException("DBProvider object can`t be null");

                dbProvider = value;
            }
        }

        int updatingInterval;
        public int UpdatingInterval {
            get {
                return updatingInterval;
            }

            private set {
                if (value < 10)
                    throw new ArgumentOutOfRangeException("Updating interval cannot be less than 10 seconds");

                updatingInterval = value;
            }
        }

        public void Add(IEvent item, Action action)
        {
            try {
                if (item != null && !itemsQueue.Select(dbItem => dbItem.Event).Contains(item)) {
                    IDataItem dbItem = new DBItem(item, action);

                    itemsQueue.Enqueue(dbItem);
                    DbProvider.Add(dbItem, itemsQueueTableName);
                }
            }
            catch (Exception ex) {
                log.Error(ex.Message);
            }
        }

        public void Start()
        {
            if (!workerThread.IsAlive) {
                DbProvider.CreateIfNotExist(new string[] { itemsQueueTableName });
                DbProvider.Load(itemsQueueTableName);

                workerThread.Start();
            }
        }

        private void DoWork()
        {
            log.Info("Start the worker thread for a local items handling.");

            while (true) {
                Thread.Sleep(UpdatingInterval * 1000);

                HandleQueues();
            }
        }

        private void HandleQueues()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            IDataItem item;

            while (!itemsQueue.IsEmpty) {
                if (!itemsQueue.TryDequeue(out item))
                    continue;

                try {
                    switch (item.EventAction) {
                        case Action.RemoteAdd:
                            log.Info(string.Format("Saving item '{0}' to the remote calendar", item.Event));

                            RemoteCalendar.Save(item.Event);
                            DbProvider.Remove(item, itemsQueueTableName);
                            break;
                        case Action.RemoteDelete:
                            log.Info(string.Format("Deleting item '{0}' from the remote calendar", item.Event));

                            if (!RepeatedDeleteFromRemote(item.Event))
                                LocalCalendar.Save(item.Event);

                            DbProvider.Remove(item, itemsQueueTableName);
                            break;
                        case Action.LocalAdd:
                            log.Info(string.Format("Saving item '{0}' to the local calendar", item.Event));

                            LocalCalendar.Save(item.Event);
                            break;
                        case Action.LocalDelete:
                            log.Info(string.Format("Deleting item '{0}' from the local calendar", item.Event));

                            LocalCalendar.Delete(item.Event);
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
                    log.Error(string.Format("Item was returned to the queue - {0}", item.Event));
                }
            }
        }

        private bool RepeatedDeleteFromRemote(IEvent item)
        {
            int maxRepeats = 3;

            for (int i = 0; i < maxRepeats; i++) {
                try {
                    RemoteCalendar.Delete(item);

                    return true;
                }
                catch (System.Exception ex) {
                    Thread.Sleep((i + 1) * 2000);

                    log.Error(ex.Message);
                }
            }

            log.Error(
                string.Format("Deleting of item '{0}' was rejected. It will be returned to the Outlook Calendar",
                item));

            return false;
        }
    }
}
