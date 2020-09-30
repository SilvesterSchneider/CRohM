using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class EventOrganization : BaseEntity
    {
        public long EventId { get; set; }
        public Event Event { get; set; }
        public long OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
