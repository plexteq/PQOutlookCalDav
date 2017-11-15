using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalCli.API
{
    public interface IJournalEntry : ICalendarObject
    {
        Classes? Class { get; set; }
        IContact Organizer { get; set; }
        Statuses? Status { get; set; }
        ICollection<string> Categories { get; set; }
        string Description { get; set; }
        ICollection<NameValuePairWithParameters> Properties { get; set; }
    }
}
