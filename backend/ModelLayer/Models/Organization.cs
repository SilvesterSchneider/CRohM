using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class Organization : BaseEntity
    {
        public Address Address { get; set; }
        public ContactPossibilities Contact { get; set; }
        public List<Contact> Employees { get; set; }
    }
}
