using ModelLayer.Models.Base;
using System.Collections.Generic;
using System.Text;

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
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Name: " + PreName + " " + Name);
            sb.AppendLine();
            sb.Append("Anschrift: ");
            sb.AppendLine();
            sb.Append(Address.ToString());
            sb.AppendLine();
            sb.Append("Kontaktmöglichkeiten: ");
            sb.AppendLine();
            sb.Append(ContactPossibilities.ToString());
            return sb.ToString();
        }
    }
}
