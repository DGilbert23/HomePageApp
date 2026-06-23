using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HomePageApp.Infrastructure.Services.Google.GoogleCalendar
{
    public record CalendarResponse(
        List<Item> Items
        );

    public class Creator
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("self")]
        public bool? Self { get; set; }
    }

    public class DefaultReminder
    {
        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("minutes")]
        public int? Minutes { get; set; }
    }

    public class End
    {
        [JsonPropertyName("dateTime")]
        public DateTime? DateTime { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("etag")]
        public string Etag { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("htmlLink")]
        public string HtmlLink { get; set; }

        [JsonPropertyName("created")]
        public DateTime? Created { get; set; }

        [JsonPropertyName("updated")]
        public DateTime? Updated { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("creator")]
        public Creator Creator { get; set; }

        [JsonPropertyName("organizer")]
        public Organizer Organizer { get; set; }

        [JsonPropertyName("start")]
        public Start Start { get; set; }

        [JsonPropertyName("end")]
        public End End { get; set; }

        [JsonPropertyName("transparency")]
        public string Transparency { get; set; }

        [JsonPropertyName("iCalUID")]
        public string ICalUID { get; set; }

        [JsonPropertyName("sequence")]
        public int? Sequence { get; set; }

        [JsonPropertyName("reminders")]
        public Reminders Reminders { get; set; }

        [JsonPropertyName("eventType")]
        public string EventType { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class Organizer
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("self")]
        public bool? Self { get; set; }
    }

    public class Override
    {
        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("minutes")]
        public int? Minutes { get; set; }
    }

    public class Reminders
    {
        [JsonPropertyName("useDefault")]
        public bool? UseDefault { get; set; }

        [JsonPropertyName("overrides")]
        public List<Override> Overrides { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("etag")]
        public string Etag { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("updated")]
        public DateTime? Updated { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("accessRole")]
        public string AccessRole { get; set; }

        [JsonPropertyName("defaultReminders")]
        public List<DefaultReminder> DefaultReminders { get; set; }

        [JsonPropertyName("nextSyncToken")]
        public string NextSyncToken { get; set; }

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }
    }

    public class Start
    {
        [JsonPropertyName("dateTime")]
        public DateTime? DateTime { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }
    }
}
