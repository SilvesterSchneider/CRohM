using ModelLayer.Models.Base;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class Contact : BaseEntity
    {
        public string PreName { get; set; }
        public Address Address { get; set; }
        public ContactPossibilities ContactPossibilities { get; set; }
        public List<EventContact> Events { get; set; } = new List<EventContact>();
        public List<OrganizationContact> OrganizationContacts { get; set; } = new List<OrganizationContact>();
        public List<HistoryElement> History { get; set; } = new List<HistoryElement>();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(PreName + " " + Name);
            sb.AppendLine();
            sb.Append("Wohnt an folgender Anschrift: ");
            sb.AppendLine();
            sb.Append(Address.ToString());
            sb.AppendLine();
            sb.Append("und verfügt über folgende Kontaktmöglichkeiten: ");
            sb.AppendLine();
            sb.Append(ContactPossibilities.ToString());
            return sb.ToString();
        }
    }
}