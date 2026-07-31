using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HomePageApp.Core.Contracts.Accounts
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = "";
        [Required]
        public string NewPassword { get; set; } = "";
        [Required]
        public string ConfirmPassword { get; set; } = "";
    }
}
