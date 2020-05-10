using ModelLayer.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(AllowEmptyStrings = false)]
        public string ContactEntryName { get; set; }

        [MailAndPhoneValidator]
        public string ContactEntryValue { get; set; }
    }
}
