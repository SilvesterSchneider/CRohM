using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class ContactPossibilities : BaseEntity
    {
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string Mail { get; set; }
    }
}
