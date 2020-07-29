using Microsoft.AspNetCore.Identity;
using ModelLayer.Models;
using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;

namespace ModelLayer
{ 
	public class PermissionGroup : BaseEntity
	{
		public List<Permission> Permissions { get; set; } = new List<Permission>();
        public List<UserPermissionGroup> User { get; set; }
	}
}
