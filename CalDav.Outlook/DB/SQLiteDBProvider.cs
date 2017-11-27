using System;
using CalCli.API;
using System.Data.SQLite;
using System.Collections.Concurrent;
using log4net;

namespace CalDav.Outlook
{
    public class SQLiteDBProvider : IDataProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SQLiteDBProvider));
        private static string DBPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                                        "/AppData/Local/Microsoft/Outlook/calendar_synchronizer.db";

        private static string ConnectionString =
            string.Format("Data Source={0}; Version=3; FailIfMissing=False; Foreign Keys = True;", DBPath);


        IRemoteCalendar calendar;

        public SQLiteDBProvider(IRemoteCalendar calendar)
        {
            this.calendar = calendar;
        }

        public void CreateIfNotExist(string[] tables)
        {
            try {
                if (!System.IO.File.Exists(DBPath)) {
                    SQLiteConnection.CreateFile(DBPath);

                    foreach (string str in tables) {
                        CreateTable(str);
                        //CreateIndex(str, string.Format("{0}_idx_uid_title", str).ToLower());
                    }

                    log.Info(string.Format("DB {0} created", DBPath));
                }
                else {
                    log.Info(string.Format("DB {0} exists", DBPath));
                }
            }
            catch (Exception e) {
                log.Error(e.Message);
            }
        }

        public void Add(IDataItem item, string tableName)
        {
            int result = -1;
            try {
                using (SQLiteCommand cmd = new SQLiteCommand()) {
                    cmd.CommandText = string.Format("INSERT INTO {0} VALUES (@Title, @UID, @Action, @Event)", tableName);

                    cmd.Parameters.AddWithValue("@Title", item.Event.Summary);
                    cmd.Parameters.AddWithValue("@UID", item.Event.UID);
                    cmd.Parameters.AddWithValue("@Action", item.EventAction);

                    byte[] blob = Utility.Serialize(item.Event);

                    if (blob != null)
                        cmd.Parameters.AddWithValue("@Event", Utility.Serialize(item.Event));
                    else
                        throw new ArgumentNullException(
                            "Can`t serialize an event to the byte array. Writing to the DB was rejected.");
                    
                    result = ExecuteCommand(cmd);

                    log.Info(string.Format("Item {0} added to the DB", item.ToString()));
                }
            }
            catch (Exception e) {
                log.Error(e.Message);
            }
        }

        public void Remove(IDataItem item, string tableName)
        {
            int result = -1;
            try {
                using (SQLiteCommand command = new SQLiteCommand()) {

                    command.CommandText = string.Format(
                        "DELETE FROM {0} WHERE Title = @Title AND Action = @Action", tableName);

                    command.Parameters.AddWithValue("@Title", item.Event.Summary);
                    //command.Parameters.AddWithValue("@UID", item.Event.UID);
                    command.Parameters.AddWithValue("@Action", item.EventAction);


                    result = ExecuteCommand(command);
                }
            }
            catch (Exception e) {
                log.Error(e.Message);
            }
        }

        public ConcurrentQueue<IDataItem> Load(string table)
        {
            ConcurrentQueue<IDataItem> events = new ConcurrentQueue<IDataItem>();
            try {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString)) {
                    connection.Open();

                    string commandText = string.Format("SELECT * FROM {0}", table);

                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection)) {
                        using (SQLiteDataReader sqliteReader = command.ExecuteReader()) {
                            while (sqliteReader.Read()) {
                                byte[] blob = (byte[])sqliteReader.GetValue(3);
                                //SQLiteBlob eventBlob = sqliteReader.GetBlob(3, true);

                                //byte[] blob = new byte[eventBlob.GetCount()];
                                //eventBlob.Read(blob, eventBlob.GetCount(), 0);

                                IEvent ev = (IEvent)Utility.Deserialize(blob);
                                events.Enqueue(new DBItem(ev, (Action)sqliteReader.GetInt32(2)));
                            }
                        }
                    }

                    connection.Close();

                    log.Info(string.Format("Successfull loaded {0} items from a DB", events.Count));
                }
            }
            catch (Exception e) {
                log.Error(e.Message);
            }

            return events;
        }

        private int ExecuteCommand(SQLiteCommand command)
        {
            int result = -1;

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString)) {
                connection.Open();

                command.Connection = connection;

                try {
                    result = command.ExecuteNonQuery();
                }
                catch (Exception e) {
                    log.Error(e.Message);
                }

                connection.Close();
            }

            return result;
        }
        private void CreateTable(string tableName)
        {
            using (SQLiteCommand command = new SQLiteCommand()) {
                command.CommandText =
                    string.Format(
                        "CREATE TABLE {0} (Title VARCHAR(50), UID VARCHAR(50), Action INT, Event BLOB NOT NULL)",
                        tableName);

                ExecuteCommand(command);
            }
        }
        private void CreateIndex(string tableName, string indexName)
        {
            using (SQLiteCommand command = new SQLiteCommand()) {
                command.CommandText =
                    string.Format("CREATE INDEX {0} ON {1} (UID, Title)",
                    indexName, tableName);
                ExecuteCommand(command);
            }
        }
    }
}
