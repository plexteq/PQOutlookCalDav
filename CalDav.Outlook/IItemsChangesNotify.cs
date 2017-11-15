using CalCli.API;

namespace CalDav.Outlook
{
    public delegate bool ItemAdded(IEvent item);
    public delegate bool ItemChanged(IEvent item);
    public delegate bool ItemBeforeDelete(IEvent item);

    public interface IItemsChangesNotify
    {
        ItemAdded ItemAddedHandler { get;  set; }
        ItemChanged ItemChangedHandler { get; set; }
        ItemBeforeDelete ItemBeforeDeleteHandler { get; set; }
    }
}
