using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class TagContact : BaseEntity
    {
        public Contact Contact { get; set; }
    }
}
