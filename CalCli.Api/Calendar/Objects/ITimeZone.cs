using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalCli.API
{
    public interface ITimeZone : ISerializeToICAL
    {
        ICalDAVCalendar Calendar { get; set; }
    }
}
