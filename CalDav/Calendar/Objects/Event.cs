﻿using System;
using System.Collections.Generic;
using System.Linq;
using CalCli.API;

namespace CalDav
{
    public class Event : IEvent
    {
        private DateTime DTSTAMP = DateTime.UtcNow;

        public Event()
        {
            Attendees = new List<IContact>();
            Alarms = new List<IAlarm>();
            Categories = new List<string>();
            Recurrences = new List<IRecurrence>();
            Properties = new List<NameValuePairWithParameters>();
            Attachments = new List<Uri>();
        }

        public virtual ICalDAVCalendar Calendar { get; set; }
        public virtual ICollection<IContact> Attendees { get; set; }
        public virtual ICollection<IAlarm> Alarms { get; set; }
        public virtual ICollection<string> Categories { get; set; }
        public virtual ICollection<Uri> Attachments { get; set; }
        public virtual Classes? Class { get; set; }
        public virtual DateTime? Created { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsAllDay { get; set; }
        public virtual DateTime? LastModified { get; set; }
        public virtual DateTime? Start { get; set; }
        public virtual DateTime? End { get; set; }
        public virtual string Location { get; set; }
        public virtual int? Priority { get; set; }
        public virtual Statuses? Status { get; set; }
        public virtual int? Sequence { get; set; }
        public virtual string Summary { get; set; }
        public virtual string Transparency { get; set; }
        public virtual string UID { get; set; }
        public virtual Uri Url { get; set; }
        public virtual IContact Organizer { get; set; }
        public virtual ICollection<IRecurrence> Recurrences { get; set; }

        public ICollection<NameValuePairWithParameters> Properties { get; set; }

        ICollection<IAlarm> IEvent.Alarms
        {
            get
            {
                List<IAlarm> result = new List<IAlarm>();
                foreach (Alarm alarm in Alarms)
                {
                    result.Add(alarm);
                }
                return result;
            }

            set
            {
                Alarms.Clear();
                foreach (IAlarm alarm in value)
                {
                    Alarms.Add((Alarm)alarm);
                }
            }
        }

        public void Deserialize(System.IO.TextReader rdr, ISerializer serializer)
        {
            string name, value;
            var parameters = new XNameValueCollection();
            while (rdr.Property(out name, out value, parameters) && !string.IsNullOrEmpty(name))
            {
                switch (name.ToUpper())
                {
                    case "BEGIN":
                        switch (value)
                        {
                            case "VALARM":
                                var a = serializer.GetService<Alarm>();
                                a.Deserialize(rdr, serializer);
                                Alarms.Add(a);
                                break;
                        }
                        break;
                    case "ATTENDEE":
                        var contact = new Contact();
                        contact.Deserialize(value, parameters);
                        Attendees.Add(contact);
                        break;
                    case "CATEGORIES":
                        Categories = value.SplitEscaped().ToList();
                        break;
                    case "CLASS": Class = value.ToEnum<Classes>(); break;
                    case "CREATED": Created = value.ToDateTime(); break;
                    case "DESCRIPTION": Description = value; break;
                    case "DTEND": End = value.ToDateTime(); break;
                    case "DTSTAMP": DTSTAMP = value.ToDateTime().GetValueOrDefault(); break;
                    case "DTSTART": Start = value.ToDateTime(); break;
                    case "LAST-MODIFIED": LastModified = value.ToDateTime(); break;
                    case "LOCATION": Location = value; break;
                    case "ORGANIZER":
                        Organizer = serializer.GetService<Contact>();
                        Organizer.Deserialize(value, parameters);
                        break;
                    case "PRIORITY": Priority = value.ToInt(); break;
                    case "SEQUENCE": Sequence = value.ToInt(); break;
                    case "STATUS": Status = value.ToEnum<Statuses>(); break;
                    case "SUMMARY": Summary = value; break;
                    case "TRANSP": Transparency = value; break;
                    case "UID": UID = value; break;
                    case "URL": Url = value.ToUri(); break;
                    case "ATTACH":
                        var attach = value.ToUri();
                        if (attach != null)
                            Attachments.Add(attach);
                        break;
                    case "RRULE":
                        var rule = serializer.GetService<Recurrence>();
                        rule.Deserialize(null, parameters);
                        Recurrences.Add(rule);
                        break;
                    case "END": return;
                    default:
                        Properties.Add(new NameValuePairWithParameters(name, value, parameters));
                        break;
                }
            }

            IsAllDay = Start == End;
        }

        public void Serialize(System.IO.TextWriter wrtr)
        {
            if (End != null && Start != null && End < Start)
                End = Start;

            wrtr.BeginBlock("VEVENT");
            wrtr.Property("UID", UID);
            if (Attendees != null)
                foreach (var attendee in Attendees)
                    wrtr.Property("ATTENDEE", attendee);
            if (Categories != null && Categories.Count > 0)
                wrtr.Property("CATEGORIES", Categories);
            wrtr.Property("CLASS", Class);
            wrtr.Property("CREATED", Created);
            wrtr.Property("DESCRIPTION", Description);
            wrtr.Property("DTEND", IsAllDay ? (End ?? Start.Value).Date : End);
            wrtr.Property("DTSTAMP", DTSTAMP);
            wrtr.Property("DTSTART", IsAllDay ? (Start ?? End.Value).Date : Start);
            wrtr.Property("LAST-MODIFIED", LastModified);
            wrtr.Property("LOCATION", Location);
            wrtr.Property("ORGANIZER", Organizer);
            wrtr.Property("PRIORITY", Priority);
            wrtr.Property("SEQUENCE", Sequence);
            wrtr.Property("STATUS", Status);
            wrtr.Property("SUMMARY", Summary);
            wrtr.Property("TRANSP", Transparency);
            wrtr.Property("URL", Url);

            if (Properties != null)
                foreach (var prop in Properties)
                    wrtr.Property(prop.Name, prop.Value, parameters: prop.Parameters);

            if (Alarms != null)
                foreach (var alarm in Alarms)
                    ((Alarm)alarm).Serialize(wrtr);
            wrtr.EndBlock("VEVENT");
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Event)) return false;

            Event otherItem = obj as Event;

            return string.Equals(Summary, otherItem.Summary) &&
                                //string.Equals(Description, otherItem.Description) &&
                                Start.Equals(otherItem.Start) &&
                                End.Equals(otherItem.End);
        }

        /* For IEnumerable method 'Contains' */
        public bool Equals(IEvent other)
        {
            return Equals(other as object);
        }

        /* For LINQ method 'Except' */
        public bool Equals(IEvent x, IEvent y)
        {
            return x.Equals(y);
        }

        /* For LINQ method 'Except' */
        public int GetHashCode(IEvent obj)
        {
            int hashSummary = Summary == null ? 0 : Summary.GetHashCode();
            int hashDescription = Description == null ? 0 : Description.GetHashCode();
            int hashStart = Start.HasValue ? 0 : Start.GetHashCode();
            int hashEnd = End.HasValue ? 0 : End.GetHashCode();

            return hashSummary ^ hashDescription ^ hashStart ^ hashEnd;
        }
    }
}
