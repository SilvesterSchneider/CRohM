using ModelLayer.Models.Base;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public class Contact : BaseEntity
    {
        public string PreName { get; set; } = "";
        public Address Address { get; set; } = new Address();
        public ContactPossibilities ContactPossibilities { get; set; } = new ContactPossibilities();
        public List<EventContact> Events { get; set; } = new List<EventContact>();
        public List<OrganizationContact> OrganizationContacts { get; set; } = new List<OrganizationContact>();
        public List<HistoryElement> History { get; set; } = new List<HistoryElement>();
    }
}
