using CalCli.API;
using System;
using System.Collections.Concurrent;

namespace CalDav.Outlook
{

    public interface IDataProvider
    {
        void CreateIfNotExist(string[] tables);

        void Add(IDataItem item, string tableName);

        void Remove(IDataItem item, string tableName);

        ConcurrentQueue<IDataItem> Load(string table);
    }
}
