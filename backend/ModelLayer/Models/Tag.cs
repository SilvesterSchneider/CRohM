using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class Tag : BaseEntity
    {
        public Contact Contact { get; set; }
        public Organization Organization { get; set; }
        public Event Event { get; set; }
    }
}
