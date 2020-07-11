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
        public List<ContactPossibilitiesEntry> ContactEntries { get; set; } = new List<ContactPossibilitiesEntry>();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Telefonnummer: " + PhoneNumber);
            sb.AppendLine();
            sb.Append("E-Mail: " + Mail);
            sb.AppendLine();
            sb.Append("Fax: " + Fax);
            if (ContactEntries.Count > 0)
            {
                sb.AppendLine();
                sb.Append("Weitere Kontaktmöglichkeiten: ");
                foreach (ContactPossibilitiesEntry entry in ContactEntries)
                {
                    sb.AppendLine();
                    sb.Append(entry.ToString());
                }                
            }
            return sb.ToString();
        }
    }
}
