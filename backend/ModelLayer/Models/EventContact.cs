using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class EventContact : BaseEntity
    {
        public long EventId { get; set; }
        public Event Event { get; set; }
        public long ContactId { get; set; }
        public Contact Contact { get; set; }
    }
}
