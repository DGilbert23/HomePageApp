using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Core.Models
{
    public class Hour
    {
        public TimeOnly Time { get; set; }
        public string? ConditionIconUrl { get; set; }
        public double? TempF { get; set; }
        public int? ChanceOfRain { get; set; }
    }
}
