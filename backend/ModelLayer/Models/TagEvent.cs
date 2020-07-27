using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class TagEvent : BaseEntity
    {
        public Event Event { get; set; }
    }
}
