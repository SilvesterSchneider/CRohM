using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class UserPermissionGroup : BaseEntity
    {
        public long UserId { get; set; }

        public User User { get; set; }

        public long PermissionGroupId { get; set; }

        public PermissionGroup PermissionGroup { get; set; }
    }
}
