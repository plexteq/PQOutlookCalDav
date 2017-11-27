using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalCli.API;

namespace CalDav.Outlook
{
    public class DBItem : IDataItem
    {
        Tuple<IEvent, Action> item;

        public DBItem(IEvent ev, Action act)
        {
            item = new Tuple<IEvent, Action>(ev, act);
        }

        public IEvent Event {
            get {
                return item.Item1;
            }
        }

        public Action EventAction {
            get {
                return item.Item2;
            }
        }

        public override string ToString()
        {
            return string.Format("DBItem '{0} - {1}'",
                   Event, Enum.GetName(typeof(Action), EventAction));
        }
    }
}
