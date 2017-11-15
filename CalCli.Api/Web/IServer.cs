using System;

namespace CalCli.API
{
    public interface IServer
    {
        Uri Url { get; set; }
        IConnection Connection { get; set; }
        IRemoteCalendar[] GetCalendars();
        void CreateCalendar(IRemoteCalendar calendar);
    }
}
