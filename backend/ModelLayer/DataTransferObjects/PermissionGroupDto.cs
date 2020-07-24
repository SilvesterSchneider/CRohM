using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ModelLayer.Helper;
using ModelLayer.Models;
using Microsoft.AspNetCore.Identity;

namespace ModelLayer.DataTransferObjects
{
    public class PermissionGroupDto
    {

        public long id { get; set; }
        public string Name { get; set; }

        public List<Permission> Permissions { get; set; } = new List<Permission>();
    }

    public class PermissionGroupCreateDto {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public List<Permission> Permissions { get; set; } = new List<Permission>();
    }
}

