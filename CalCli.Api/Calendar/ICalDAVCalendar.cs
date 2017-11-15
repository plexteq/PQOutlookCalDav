using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalCli.API
{
    public interface ICalDAVCalendar : ISerializeToICAL
    {
        string Version { get; set; }
        string ProdID { get; set; }
        ICollection<IEvent> Events { get; set; }
        ICollection<IToDo> ToDos { get; set; }
        ICollection<ITimeZone> TimeZones { get; set; }
        ICollection<IJournalEntry> JournalEntries { get; set; }
        ICollection<IFreeBusy> FreeBusy { get; set; }
        ICollection<NameValuePairWithParameters> Properties { get; set; }

        IQueryable<ICalendarObject> Items { get; }

        void AddItem(ICalendarObject obj);

        string Scale { get; set; }
    }
}
