using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace ModelLayer.Models
{
    public class Organization : BaseEntity
    {
        public Address Address { get; set; } = new Address();
        public ContactPossibilities Contact { get; set; } = new ContactPossibilities();
        public List<OrganizationContact> OrganizationContacts { get; set; } = new List<OrganizationContact>();
    }
}
