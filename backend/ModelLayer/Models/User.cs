using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace ModelLayer.Models
{
    public class User : IdentityUser<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool hasPasswordChanged { get; set; } = false;

        public bool UserLockEnabled { get
            {
                if (LockoutEnd.HasValue && LockoutEnd.Value.LocalDateTime > DateTime.Today)
                {
                    return true;
                } 
                else
                {
                    return false;
                }
            } 
        }
    }
}