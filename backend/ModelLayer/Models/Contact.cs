using ModelLayer.Models.Base;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public class Contact : BaseEntity
    {
        public string PreName { get; set; }
        public Address Address { get; set; }
        public ContactPossibilities ContactPossibilities { get; set; }
        public List<OrganizationContact> OrganizationContacts { get; set; } = new List<OrganizationContact>();
    }
}