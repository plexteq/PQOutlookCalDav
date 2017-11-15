namespace CalCli.API
{
    public interface IRemoteCalendar : ICalendar
    {       
        string Description { get; set; }
        IToDo createToDo();
        ITrigger createTrigger();
        IAlarm createAlarm();
        IEvent createEvent();
        
        string GetSyncToken();
        string GetSyncChanges(string syncToken);
    }
}
