using CalCli.API;
using System.Collections.Concurrent;

namespace CalDav.Outlook
{
    public interface IDataProvider
    {
        void CreateIfNotExist(string[] tables);

        void Add(IEvent item, string tableName);

        void Remove(IEvent item, string tableName);

        ConcurrentQueue<IEvent> Load(string table);
    }
}
