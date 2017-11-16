using CalCli.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalDav.Outlook
{
    public enum Action
    {
        RemoteAdd, RemoteDelete, LocalAdd, LocalDelete
    };

    public interface IDataItem
    {
        IEvent Event { get; }
        Action EventAction { get; }
    }
}
