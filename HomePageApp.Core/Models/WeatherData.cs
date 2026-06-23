using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Core.Models
{
    public class WeatherData
    {
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public double? CurrentTempF { get; set; }
        public double? TodayMinTempF { get; set; }
        public double? TodayMaxTempF { get; set; }
        public string ConditionText { get; set; } = string.Empty;
        public string ConditionIconUrl { get; set; } = string.Empty;
        public List<Hour> TodayHours { get; set; } = new();
        public List<ForecastDay> ForecastFuture { get; set; } = new();
    }
}
