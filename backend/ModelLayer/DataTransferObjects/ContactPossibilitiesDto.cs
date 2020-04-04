using ModelLayer.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class ContactPossibilitiesDto
    {
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string Mail { get; set; }
    }

    public class ContactPossibilitiesCreateDto
    {
        [NumberValidator]
        public string PhoneNumber { get; set; }

        [NumberValidator]
        public string Fax { get; set; }

        [MailValidator]
        public string Mail { get; set; }
    }
}