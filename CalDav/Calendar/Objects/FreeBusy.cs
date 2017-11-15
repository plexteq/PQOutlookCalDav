﻿using CalCli.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalDav {
	public class FreeBusy : IFreeBusy
    {
		public FreeBusy() {
			DTSTAMP = DateTime.UtcNow;
			Details = new List<DateTimeRange>();
			Properties = new List<NameValuePairWithParameters>();
		}
		DateTime? DTSTAMP;
		public virtual string UID { get; set; }
		public virtual Uri Url { get; set; }
		public virtual int? Sequence { get; set; }
		public virtual DateTime? LastModified { get; set; }
		public virtual DateTime? Start { get; set; }
		public virtual DateTime? End { get; set; }
		public virtual IContact Organizer { get; set; }
		public ICalDAVCalendar Calendar { get; set; }
		public virtual ICollection<DateTimeRange> Details { get; set; }
		public ICollection<NameValuePairWithParameters> Properties { get; set; }

		public void Deserialize(System.IO.TextReader rdr, ISerializer serializer) {
			string name, value;
			var parameters = new XNameValueCollection();
			while (rdr.Property(out name, out value, parameters) && !string.IsNullOrEmpty(name)) {
				switch (name.ToUpper()) {
					case "UID": UID = value; break;
					case "ORGANIZER":
						Organizer = new Contact();
						Organizer.Deserialize(value, parameters);
						break;
					case "SEQUENCE": Sequence = value.ToInt(); break;
					case "LAST-MODIFIED": LastModified = value.ToDateTime(); break;
					case "DTSTART": LastModified = value.ToDateTime(); break;
					case "DTEND": LastModified = value.ToDateTime(); break;
					case "DTSTAMP": DTSTAMP = value.ToDateTime(); break;
					case "FREEBUSY":
						var parts = value.Split('/');
						Details.Add(new DateTimeRange {
							From = parts.FirstOrDefault().ToDateTime(),
							To = parts.ElementAtOrDefault(1).ToDateTime()
						});
						break;
					case "END": return;
					default:
						Properties.Add(new NameValuePairWithParameters(name, value, parameters));
						break;
				}
			}
		}

		public void Serialize(System.IO.TextWriter wrtr) {
			wrtr.BeginBlock("VFREEBUSY");
			wrtr.Property("ORGANIZER", Organizer);
			wrtr.Property("UID", UID);
			wrtr.Property("SEQUENCE", Sequence);
			wrtr.Property("LAST-MODIFIED", LastModified);
			wrtr.Property("DTSTART", Start);
			wrtr.Property("DTEND", End);
			wrtr.Property("Url", Url);
			foreach (var detail in Details) {
				wrtr.Property("FREEBUSY",
					(detail.From == null ? null : Common.FormatDate(detail.From.Value)) + "/"
					+ (detail.To == null ? null : Common.FormatDate(detail.To.Value))
					);

			}
			wrtr.EndBlock("VFREEBUSY");
		}
	}
}
