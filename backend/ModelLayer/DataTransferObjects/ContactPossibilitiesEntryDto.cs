using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class ContactPossibilitiesEntryDto
    {
        public long Id { get; set; }
        public string ContactEntryName { get; set; }
        public string ContactEntryValue { get; set; }
    }

    public class ContactPossibilitiesEntryCreateDto
    {
        public string ContactEntryName { get; set; }
        public string ContactEntryValue { get; set; }
    }
}
