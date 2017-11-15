using CalCli.API;
using System.Collections.Generic;

namespace CalDav {
	public class CalendarCollection : List<CalDAVCalendar>, ISerializeToICAL {
		public CalendarCollection() { }
		public CalendarCollection(IEnumerable<CalDAVCalendar> calendars) : base(calendars) { }
		public void Deserialize(System.IO.TextReader rdr, ISerializer serializer) {
			string name, value;
			var parameters = new XNameValueCollection();
			while (rdr.Property(out name, out value, parameters) && !string.IsNullOrEmpty(name)) {
				switch (name.ToUpper()) {
					case "BEGIN":
						if (value == "VCALENDAR") {
							var e = serializer.GetService<CalDAVCalendar>();
							e.Deserialize(rdr, serializer);
							this.Add(e);
						}
						break;
				}
			}
		}

		public void Serialize(System.IO.TextWriter wrtr) {
			foreach (var cal in this)
				cal.Serialize(wrtr);
		}
	}
}
