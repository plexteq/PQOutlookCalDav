using CalCli;
using CalCli.API;
using log4net;
using System;
using System.Linq;
using System.Threading;

namespace CalDav.Outlook
{
    public class Synchronizer : ISynchronizer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Synchronizer));

        public static Synchronizer Get()
        {
            return instance;
        }

        public static Synchronizer Get(Microsoft.Office.Interop.Outlook.Application addIn)
        {
            if (instance == null)
            {
                log4net.GlobalContext.Properties["LogFileName"] = Config.Directory + "outlookCalDAVsync.log"; //log file path
                log4net.Config.XmlConfigurator.Configure();

                instance = new Synchronizer();
                instance.Controller = new OutlookController(addIn);
            }

            return instance;
        }

        public void Connect()
        {
            if (string.IsNullOrWhiteSpace(config.Username) || string.IsNullOrWhiteSpace(config.Passw)
                || string.IsNullOrWhiteSpace(config.Url))
            {
                log.Error("Username, password and a calendar URL cannot be NULL or empty");
                return;
            }

            Connection = new BasicConnection(config.Username, config.Passw);
            try
            {
                Server = new CalDav.Client.Server(config.Url, Connection, config.Username, config.Passw);
                Calendars = Server.GetCalendars();
                log.Info("Successfully connected to the CalDAV server");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        public void Sync()
        {           
            Controller.Syncronize();
        }

        public void Prepare()
        {
            Controller.Initialize();
        }

        public void Start()
        {
            if (syncronizeThread != null)
            {
                syncronizeThread.Abort();
            }

            syncronizeThread = new Thread(ForegroundSync);
            syncronizeThread.Start();
        }

        private static void ForegroundSync()
        {            
            ISynchronizer syncronizer = Synchronizer.Get();

            if (syncronizer.Autostart)
            {
                syncronizer.Connect();
            }

            syncronizer.Prepare();

            while (true)
            {
                int sleepTime = 1000 * Config.Instance.SyncTimeSeconds;

                syncronizer.Sync();
                Thread.Sleep(sleepTime);
            }
        }

        private Synchronizer() { }

        #region Fields
        static Synchronizer instance;

        Thread syncronizeThread;

        Config config = Config.Instance;

        IController outlookController;

        IConnection connection;

        ICalendar offlineCalendar;
        
        IRemoteCalendar currentCalendar;

        IRemoteCalendar[] calendars;

        IServer server;

        bool autoStart;
        #endregion

        #region Properties
        public IConnection Connection
        {
            get
            {
                return connection;
            }
            private set
            {
                connection = value;
            }
        }
        public IRemoteCalendar[] Calendars
        {
            get
            {
                return calendars;
            }

            private set
            {
                calendars = value;
            }
        }

        public IServer Server
        {
            get
            {
                return server;
            }

            private set
            {
                server = value;
            }
        }

        public IRemoteCalendar CurrentCalendar
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(config.Calendar))
                {
                    IRemoteCalendar current = Calendars.Where(cal => string.Equals(cal.Name, config.Calendar)).FirstOrDefault();

                    if(current != null  && currentCalendar != current)
                    {
                        OfflineCalendar = null; 
                    }

                    currentCalendar = current;
                }

                return currentCalendar;
            }
        }

        public bool Autostart
        {
            get
            {
                return autoStart;
            }

            set
            {
                autoStart = value;
            }
        }

        public IController Controller
        {
            get
            {
                return outlookController;
            }

            private set
            {
                outlookController = value;
            }
        }

        public ICalendar OfflineCalendar
        {
            get
            {
                if(offlineCalendar == null)
                {
                    offlineCalendar = new OfflineCalendar(CurrentCalendar, new SQLiteDBProvider(CurrentCalendar));
                }

                return offlineCalendar;
            }

            set
            {
                offlineCalendar = value;
            }
        }
        
        #endregion
    }
}
