using ModelLayer.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class PermissionDto
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public UserRight UserRight { get; set; }
    }
}
