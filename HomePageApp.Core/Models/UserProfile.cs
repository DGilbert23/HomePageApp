using System;
using System.Collections.Generic;
using System.Text;

namespace HomePageApp.Core.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public Guid IdentityUserId { get; set; }
    }
}
