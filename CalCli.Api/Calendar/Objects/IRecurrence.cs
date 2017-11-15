using System;

namespace CalCli.API
{
    public interface IRecurrence : IHasParameters
    {
        Frequencies? Frequency { get; set; }
        int? Count { get; set; }
        int? Interval { get; set; }
        DateTime? Until { get; set; }
        int? ByMonth { get; set; }
        string[] ByDay { get; set; }
        int? ByMonthDay { get; set; }
    }
}
