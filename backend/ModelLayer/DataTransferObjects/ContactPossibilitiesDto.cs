using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class ContactPossibilitiesDto
    {
        public string PhoneNumber { get; set; }
        public string Faxnumber { get; set; }
        public string Email { get; set; }
    }

    public class ContactPossibilitiesCreateDto
    {
        public string PhoneNumber { get; set; }
        public string Faxnumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }
    }
}
