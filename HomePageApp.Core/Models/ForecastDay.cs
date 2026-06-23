using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Core.Models
{
    public class ForecastDay
    {
        public DateOnly Date { get; set;  }
        public string? ConditionIconUrl { get; set;  }
        public string? ConditionText { get; set; }
        public double? AverageTempF { get; set; }
        public double? MinTempF { get; set; }
        public double? MaxTempF { get; set; }
        public int? ChanceOfRain { get; set; }
    }
}
