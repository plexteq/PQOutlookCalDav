using CalCli.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalDav {
	public class CalDAVCalendar : ICalDAVCalendar {
		public CalDAVCalendar() {
			Events = new List<IEvent>();
			TimeZones = new List<ITimeZone>();
			ToDos = new List<IToDo>();
			JournalEntries = new List<IJournalEntry>();
			FreeBusy = new List<IFreeBusy>();
			Properties = new List<NameValuePairWithParameters>();
		}

		public virtual string Version { get; set; }
		public virtual string ProdID { get; set; }
		public virtual ICollection<IEvent> Events { get; set; }
		public virtual ICollection<IToDo> ToDos { get; set; }
		public virtual ICollection<ITimeZone> TimeZones { get; set; }
		public virtual ICollection<IJournalEntry> JournalEntries { get; set; }
		public virtual ICollection<IFreeBusy> FreeBusy { get; set; }
		public ICollection<NameValuePairWithParameters> Properties { get; set; }

		public virtual IQueryable<ICalendarObject> Items {
			get {
				return Events.OfType<ICalendarObject>()
					.Union(ToDos).Union(JournalEntries).Union(FreeBusy)
					.AsQueryable();
			}
		}

		public virtual void AddItem(ICalendarObject obj) {
			if (obj == null) return;
			else if (obj is Event) Events.Add((Event)obj);
			else if (obj is ToDo) ToDos.Add((ToDo)obj);
			else if (obj is JournalEntry) JournalEntries.Add((JournalEntry)obj);
			else if (obj is FreeBusy) FreeBusy.Add((FreeBusy)obj);
			else throw new InvalidCastException();
		}

		public string Scale { get; set; }
               
        public virtual void Deserialize(System.IO.TextReader rdr, ISerializer serializer = null) {
			if (serializer == null) serializer = new Serializer();
			string name, value;
			var parameters = new XNameValueCollection();
			while (rdr.Property(out name, out value, parameters) && !string.IsNullOrEmpty(name)) {
				switch (name.ToUpper()) {
					case "BEGIN":
						switch (value) {
							case "VEVENT":
								var e = serializer.GetService<Event>();
								e.Calendar = this;
								Events.Add(e);
								e.Deserialize(rdr, serializer);
								break;
							case "VTIMEZONE":
								var tz = serializer.GetService<TimeZone>();
								tz.Calendar = this;
								TimeZones.Add(tz);
								tz.Deserialize(rdr, serializer);
								break;
							case "VTODO":
								var td = serializer.GetService<ToDo>();
								td.Calendar = this;
								ToDos.Add(td);
								td.Deserialize(rdr, serializer);
								break;
							case "VFREEBUSY":
								var fb = serializer.GetService<FreeBusy>();
								fb.Calendar = this;
								FreeBusy.Add(fb);
								fb.Deserialize(rdr, serializer);
								break;
							case "VJOURNAL":
								var jn = serializer.GetService<JournalEntry>();
								jn.Calendar = this;
								JournalEntries.Add(jn);
								jn.Deserialize(rdr, serializer);
								break;
						}
						break;
					case "CALSCALE": Scale = value; break;
					case "VERSION": Version = value; break;
					case "PRODID": ProdID = value; break;
					case "END":
						if (value == "VCALENDAR")
							return;
						break;
					default:
						Properties.Add(new NameValuePairWithParameters(name, value, parameters));
						break;
				}
			}
		}

		public virtual void Serialize(System.IO.TextWriter wrtr) {
			wrtr.BeginBlock("VCALENDAR");
			wrtr.Property("VERSION", Version ?? "2.0");
			wrtr.Property("PRODID", Common.PRODID);
			wrtr.Property("CALSCALE", Scale);

			if (Properties != null)
				foreach (var prop in Properties)
					wrtr.Property(prop.Name, prop.Value, parameters: prop.Parameters);

			foreach (var tz in TimeZones) {
				tz.Calendar = this as ICalDAVCalendar;
				tz.Serialize(wrtr);
			}
			foreach (var e in Events) {
				e.Calendar = this as ICalDAVCalendar;
				e.Serialize(wrtr);
			}
			foreach (var td in ToDos) {
				td.Calendar = this as ICalDAVCalendar;
				td.Serialize(wrtr);
			}
			foreach (var fb in FreeBusy) {
				fb.Calendar = this as ICalDAVCalendar;
				fb.Serialize(wrtr);
			}
			foreach (var jn in JournalEntries) {
				jn.Calendar = this as ICalDAVCalendar;
				jn.Serialize(wrtr);
			}
			wrtr.EndBlock("VCALENDAR");
		}
	}
}
