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

        const string AddQueueTableName = "AddQueue";
        const string RemoveQueueTableName = "RemoveQueue";

        Thread workerThread;
        ConcurrentQueue<IEvent> addedQueue;
        ConcurrentQueue<IEvent> removedQueue;

        IDataProvider dbProvider;

        public OfflineCalendar(ICalendar RemoteCalendar, IDataProvider dbProvider)
        {
            addedQueue = new ConcurrentQueue<IEvent>();
            removedQueue = new ConcurrentQueue<IEvent>();

            this.RemoteCalendar = RemoteCalendar;

            DbProvider = dbProvider;

            UpdatingInterval = 10;

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

        public string FullName
        {
            get
            {
                if (RemoteCalendar != null)
                    return RemoteCalendar.FullName;

                return null;
            }
        }

        public IDataProvider DbProvider
        {
            get
            {
                return dbProvider;
            }

            private set
            {
                dbProvider = value;
            }
        }

        public void Save(IEvent e)
        {
            if (e != null && !addedQueue.Contains(e))
            {
                addedQueue.Enqueue(e);
                DbProvider.Add(e, AddQueueTableName);
            }
        }

        public void Delete(IEvent e)
        {
            if (e != null && !removedQueue.Contains(e))
            {
                removedQueue.Enqueue(e);
                DbProvider.Add(e, RemoveQueueTableName);
            }
        }

        private delegate void HandleQueueItem(IEvent e);

        private void ProcessQueue(ConcurrentQueue<IEvent> queue, HandleQueueItem handleItem, string tableName)
        {
            IEvent e;

            while (!queue.IsEmpty)
            {
                if (queue.TryDequeue(out e))
                {
                    try
                    {
                        handleItem(e);
                        DbProvider.Remove(e,tableName);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                    }
                }
            }
        }

        private void Load()
        {
            addedQueue = DbProvider.Load(AddQueueTableName);
            removedQueue = DbProvider.Load(RemoveQueueTableName);
        }

        public void DoWork()
        {
            while (true)
            {
                Thread.Sleep(UpdatingInterval * 60 * 1000);

                HandleQueues();
            }
        }

        private void HandleQueues()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                ProcessQueue(addedQueue, new HandleQueueItem(RemoteCalendar.Save), AddQueueTableName);

                ProcessQueue(removedQueue, new HandleQueueItem(RemoteCalendar.Delete), RemoveQueueTableName);
            }
        }

        public void Update()
        {
            if (!workerThread.IsAlive)
            {
                DbProvider.CreateIfNotExist(new string[] { AddQueueTableName, RemoveQueueTableName });
                Load();
                HandleQueues();

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
