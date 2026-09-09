namespace HomePageApp.Core.Models.CalendarApi
{
    public class CalendarData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string HtmlLink { get; set; }
        public bool IsAllDay { get; set; }
    }
}
