using CalCli.API;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalDav.Outlook
{
    public static class Extensions
    {
        public static bool IsEqual(this IEvent oneItem, AppointmentItem otherItem)
        {
            return string.Equals(oneItem.Summary, otherItem.Subject) &&
                                //string.Equals(oneItem.Description, otherItem.Body) &&
                                oneItem.Start.Equals(otherItem.Start) &&
                                oneItem.End.Equals(otherItem.End);
        }

        public static IEvent ConvertToEvent(this AppointmentItem appointment)
        {
            IEvent item = new Event()
            {
                Start = appointment.Start,
                End = appointment.End,
                Description = appointment.Body ?? "",
                IsAllDay = appointment.AllDayEvent,
                Summary = appointment.Subject ?? "",
                Location = appointment.Location,
                //Organizer = appointment.GetOrganizer().ConvertToContact(),
                Class = appointment.Sensitivity.ConvertToClass(),
                Categories = appointment.CategoriesToList()
                //Recurrences = new List<IRecurrence>();
                //Properties = new List<NameValuePairWithParameters>();
            };

            appointment.AttachmentsToList(item.Attachments);
            appointment.AttendiesToList(item.Attendees);
            //appointment.AlarmsToList(item.Alarms);

            return item;
        }

        public static void AlarmsToList(this AppointmentItem appointment, ICollection<IAlarm> alarms)
        {
            if (appointment.ReminderSet)
            {
                IAlarm alarm = new Alarm();
                alarm.Trigger = new Trigger();

                alarm.Action = AlarmActions.DISPLAY;
                alarm.Trigger.Related = Relateds.Start;
                alarm.Trigger.DateTime = appointment.Start.AddMinutes(0 - appointment.Start.Minute - appointment.ReminderMinutesBeforeStart);
            }
        }

        public static ICollection<string> CategoriesToList(this AppointmentItem appointment)
        {
            if (!string.IsNullOrWhiteSpace(appointment.Categories))
            {
                string[] categories = appointment.Categories.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return categories.ToList();
            }

            return null;
        }

        public static void AttachmentsToList(this AppointmentItem appointment, ICollection<Uri> attachments)
        {
            if (appointment.Attachments.Count > 0)
            {
                foreach (Attachment att in appointment.Attachments)
                {
                    attachments.Add(new Uri(att.PathName));
                }
            }
        }

        public static void AttendiesToList(this AppointmentItem appointment, ICollection<IContact> attendies)
        {
            if (appointment.Recipients.Count > 0)
            {
                foreach (Recipient recp in appointment.Recipients)
                {
                    attendies.Add(new Contact()
                    {
                        Name = recp.Name,
                        Email = recp.Address
                    });
                }
            }
        }

        public static Classes ConvertToClass(this OlSensitivity sensitivity)
        {
            return sensitivity == OlSensitivity.olConfidential ? Classes.CONFIDENTIAL :
                sensitivity == OlSensitivity.olPrivate ? Classes.PRIVATE : Classes.PUBLIC; ;
        }

        public static OlSensitivity ConvertToSensitivity(this Classes? cls)
        {
            if (!cls.HasValue)
                return OlSensitivity.olNormal;

            return cls.Value == Classes.CONFIDENTIAL ? OlSensitivity.olConfidential :
                cls.Value == Classes.PRIVATE ? OlSensitivity.olPrivate : OlSensitivity.olNormal;
        }

        public static IContact ConvertToContact(this AddressEntry addressEntry)
        {
            IContact contact = new Contact()
            {
                Name = addressEntry.Name,
                Email = addressEntry.Address
            };

            return contact;
        }
        public static void FromContact(this AddressEntry addressEntry, IContact contact)
        {
            if (contact != null)
            {
                addressEntry.Name = contact.Name;
                addressEntry.Address = contact.Email;
            }
        }

        public static AppointmentItem FromEvent(this AppointmentItem appointment, IEvent item)
        {
            if (item.Start.HasValue)
                appointment.Start = item.Start.Value;
            if (item.End.HasValue)
                appointment.End = item.End.Value;
            if (item.Categories != null && item.Categories.Count > 0)
                appointment.Categories =
                    item.Categories.Aggregate((current, next) => current + " " + next);
            else appointment.Categories = "";

            appointment.Body = item.Description ?? "";
            appointment.AllDayEvent = item.IsAllDay;
            appointment.Subject = item.Summary ?? "";
            appointment.Location = item.Location;
            
            //appointment.GetOrganizer().FromContact(item.Organizer);
            appointment.Sensitivity = item.Class.ConvertToSensitivity();

            foreach (Uri uri in item.Attachments)
            {
                appointment.Attachments.Add(uri);
            }

            foreach (IContact contact in item.Attendees)
            {
                if (!string.IsNullOrWhiteSpace(contact.Name))
                    appointment.Recipients.Add(contact.Name);
            }

            //    appointment.AttendiesToList(item.Attendees);

            return appointment;
        }
    }
}
