using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    public class Organization : BaseEntity
    {
        public Address Address { get; set; }
        public ContactPossibilities Contact { get; set; }
        public List<OrganizationContact> OrganizationContacts { get; set; } = new List<OrganizationContact>();
        public List<TagOrganization> Tags { get; set; } = new List<TagOrganization>();
    }
}
