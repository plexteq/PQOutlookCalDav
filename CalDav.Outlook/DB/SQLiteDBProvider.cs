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
            string.Format("Data Source={0}; Version=3; FailIfMissing=True; Foreign Keys = True;", DBPath);
                

        IRemoteCalendar calendar;

        public SQLiteDBProvider(IRemoteCalendar calendar)
        {
            this.calendar = calendar;
        }

        public void CreateIfNotExist(string[] tables)
        {
            try
            {
                if (!System.IO.File.Exists(DBPath))
                {
                    SQLiteConnection.CreateFile(DBPath);

                    foreach (string str in tables)
                    {
                        CreateTable(str);
                    }
                }
            }
            catch (SQLiteException e)
            {
                log.Error(e.Message);
            }
        }

        public void Add(IEvent item, string tableName)
        {
            int result = -1;
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO {0} VALUES (@Title, @Start, @End, @UID)", tableName);

                    cmd.Parameters.AddWithValue("@Title", item.Summary);
                    cmd.Parameters.AddWithValue("@Start", item.Start);
                    cmd.Parameters.AddWithValue("@End", item.End);
                    cmd.Parameters.AddWithValue("@UID", item.UID);

                    result = ExecuteCommand(cmd);
                }
            }
            catch (SQLiteException e)
            {
                log.Error(e.Message);
            }
        }

        public ConcurrentQueue<IEvent> Load(string table)
        {
            ConcurrentQueue<IEvent> events = new ConcurrentQueue<IEvent>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string commandText = string.Format("SELECT * FROM {0}", table);

                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        using (SQLiteDataReader sqliteReader = command.ExecuteReader())
                        {
                            while (sqliteReader.Read())
                            {
                                IEvent ev = calendar.createEvent();

                                ev.Summary = sqliteReader.GetString(0);
                                ev.Start = sqliteReader.GetDateTime(1);
                                ev.End = sqliteReader.GetDateTime(2);
                                ev.UID = sqliteReader.GetString(3);

                                events.Enqueue(ev);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (SQLiteException e)
            {
                log.Error(e.Message);
            }

            return events;
        }

        public void Remove(IEvent item,string tableName)
        {
            int result = -1;
            try
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = string.Format("DELETE FROM {0} WHERE UID = @UID", tableName);

                    command.Parameters.AddWithValue("@UID", item.UID);

                    result = ExecuteCommand(command);
                }
            }
            catch (SQLiteException e)
            {
                log.Error(e.Message);
            }
        }

        private int ExecuteCommand(SQLiteCommand command)
        {
            int result = -1;

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                command.Connection = connection;

                try
                {
                    result = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    log.Error(e.Message);
                }

                connection.Close();
            }

            return result;
        }
        private void CreateTable(string tableName)
        {
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText =
                    string.Format("CREATE TABLE {0} (Title varchar(50), Start datetime, End datetime, UID varchar(50))",
                    tableName);
                ExecuteCommand(command);
            }
        }
    }
}
