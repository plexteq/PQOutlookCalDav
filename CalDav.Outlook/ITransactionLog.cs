using CalCli.API;

namespace CalDav.Outlook
{
    public interface ITransactionLog
    {
        IDataProvider DbProvider { get; }
        int UpdatingInterval { get; }

        void Add(IEvent item, Action action);
        void Start();
    }
}