using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalCli.API
{
    public interface ICalendar
    {
        string Name { get; set; }
        string FullName { get; }
        void Save(IEvent e);
        //void Save(IToDo t);
        void Delete(IEvent e);
    
        void Update();
              
        ICollection<IEvent> GetEvents(DateTime? from = null, DateTime? to = null);
    }
}
