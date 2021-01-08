using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ModelLayer.Helper;
using ModelLayer.Models.Base;

namespace ModelLayer.Models
{
    /// <summary>
    /// RAM: 20%
    /// </summary>
    public class User : IdentityUser<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LastLoginDate { get; set; } = DateTime.Now;
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

        public bool IsSuperAdmin { get; set; } = false;

        public bool IsDeleted { get; set; } = false;
    }

}
