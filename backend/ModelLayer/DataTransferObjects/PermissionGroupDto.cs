using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ModelLayer.Helper;

namespace ModelLayer.DataTransferObjects
{
    public class PermissionDto
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }

        public UserRight Grant { get; set; }
    }

    public class PermissionGroupDto
    {

        public long id { get; set; }
        public string Name { get; set; }

        public List<PermissionDto> Permissions { get; set;} = PredefinedPermissionList();

        private static List<PermissionDto> PredefinedPermissionList()
        {
            List<PermissionDto> list = new List<PermissionDto>();
            list.Add(new PermissionDto(){ IsEnabled = false, Grant = UserRight.USER_CREATE, Name = "User create" }); 
            list.Add(new PermissionDto(){ IsEnabled = false, Grant = UserRight.USER_DELETE, Name = "User delete" });
            return list;
        }
    }

    public class PermissionGroupCreateDto {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }
}

