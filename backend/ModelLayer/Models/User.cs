using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ModelLayer.Helper;
using ModelLayer.Models.Base;

namespace ModelLayer.Models
{
    public class User : IdentityUser<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LastLoginDate { get; set; }
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

        public List<PermissionGroup> Permission { get; set; } = new List<PermissionGroup>();
    }

}
