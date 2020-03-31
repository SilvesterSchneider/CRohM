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
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Mail { get; set; }
    }
}
