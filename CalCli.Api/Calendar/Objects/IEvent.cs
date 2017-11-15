using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalCli.API
{
    public interface IEvent : ICalendarObject, IEquatable<IEvent>, IEqualityComparer<IEvent>
    {
        ICollection<IContact> Attendees { get; set; }
        ICollection<IAlarm> Alarms { get; set; }
        ICollection<string> Categories { get; set; }
        ICollection<Uri> Attachments { get; set; }
        Classes? Class { get; set; }
        DateTime? Created { get; set; }
        string Description { get; set; }
        bool IsAllDay { get; set; }
        DateTime? Start { get; set; }
        DateTime? End { get; set; }
        string Location { get; set; }
        int? Priority { get; set; }
        Statuses? Status { get; set; }
        string Summary { get; set; }
        string Transparency { get; set; }
        Uri Url { get; set; }
        IContact Organizer { get; set; }
        ICollection<IRecurrence> Recurrences { get; set; }
    }
}
