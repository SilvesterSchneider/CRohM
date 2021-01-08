using ModelLayer.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class ContactPossibilitiesDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string Mail { get; set; }
        public List<ContactPossibilitiesEntryDto> ContactEntries { get; set; }
    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class ContactPossibilitiesCreateDto
    {

        [NumberValidator]
        public string PhoneNumber { get; set; }

        [NumberValidator]
        public string Fax { get; set; }

        [MailValidator]
        public string Mail { get; set; }

        public List<ContactPossibilitiesEntryCreateDto> ContactEntries { get; set; }
    }
}
