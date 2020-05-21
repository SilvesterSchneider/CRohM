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

        public List<PermissionGroup> Permission { get; set; } = new List<PermissionGroup>();
    }

}