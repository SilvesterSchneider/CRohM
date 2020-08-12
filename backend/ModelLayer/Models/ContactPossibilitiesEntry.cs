using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class ContactPossibilitiesEntry : BaseEntity
    {
        public string ContactEntryName { get; set; } = "";
        public string ContactEntryValue { get; set; } = "";
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Name der Kontaktmöglichkeit: " + ContactEntryName);
            sb.AppendLine();
            sb.Append("Kontaktmöglichkeit: " + ContactEntryValue);
            return sb.ToString();
        }
    }
}
