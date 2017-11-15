using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalCli.API
{
    public interface IFreeBusy : ICalendarObject
    {
        Uri Url { get; set; }
        DateTime? Start { get; set; }
        DateTime? End { get; set; }
        IContact Organizer { get; set; }
        ICollection<DateTimeRange> Details { get; set; }
        ICollection<NameValuePairWithParameters> Properties { get; set; }
    }
}
