using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace ModelLayer.Models
{
    public class Organization : BaseEntity
    {
        public Address Address { get; set; }
        public ContactPossibilities Contact { get; set; }
        public List<OrganizationContact> OrganizationContacts { get; set; }
    }
}