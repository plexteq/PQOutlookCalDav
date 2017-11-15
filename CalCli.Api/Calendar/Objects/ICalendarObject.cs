using System;

namespace CalCli.API
{
    public interface ICalendarObject : ISerializeToICAL
    {
        string UID { get; set; }
        int? Sequence { get; set; }
        DateTime? LastModified { get; set; }
        ICalDAVCalendar Calendar { get; set; }
    }
}
