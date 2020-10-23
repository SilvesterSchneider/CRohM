using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ModelLayer.Models
{
    public class Contact : BaseEntity
    {
        public enum GenderTypes
        {
            [EnumMember(Value = "Männlich")]
            MALE,

            [EnumMember(Value = "Weiblich")]
            FEMALE,

            [EnumMember(Value = "Divers")]
            DIVERS
        }

        public GenderTypes Gender { get; set; } = GenderTypes.MALE;
        public string ContactPartner { get; set; } = string.Empty;
        public string PreName { get; set; } = string.Empty;
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
            sb.Append("Geschlecht: " + Enum.GetName(typeof(GenderTypes), Gender));
            sb.AppendLine();
            if (!string.IsNullOrEmpty(ContactPartner))
            {
                sb.Append("Ansprechpartner: " + ContactPartner);
                sb.AppendLine();
            }
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
