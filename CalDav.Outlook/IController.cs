using CalCli.API;

namespace CalDav.Outlook
{
    public interface IController
    {
        ICalendar RemoteCalendar { get; }
        ICalendar OfflineCalendar { get;  }
                                
        void Initialize();

        void Syncronize();
        
    }
}
