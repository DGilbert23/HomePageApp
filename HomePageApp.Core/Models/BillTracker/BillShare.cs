using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Core.Models.BillTracker
{
    public class BillShare
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int ShareWithId { get; set; }
    }
}
