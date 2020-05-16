using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class ContactPossibilitiesEntry : BaseEntity
    {
        public string ContactEntryName { get; set; }

        public string ContactEntryValue { get; set; }
    }
}
