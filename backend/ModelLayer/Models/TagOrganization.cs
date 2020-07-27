using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class TagOrganization : BaseEntity
    {
        public Organization Organization { get; set; }
    }
}
