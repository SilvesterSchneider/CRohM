using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class EventContact : BaseEntity
    {
        public long EventId { get; set; }
        public Event Event { get; set; }
        public long ContactId { get; set; }
        public Contact Contact { get; set; }
    }
}
