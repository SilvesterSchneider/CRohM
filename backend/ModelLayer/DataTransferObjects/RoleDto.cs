using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class RoleDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<string> Claims { get; set; }
    }
}
