using ModelLayer.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class ContactPossibilitiesEntryDto
    {
        public long Id { get; set; }
        public string ContactEntryName { get; set; }
        public string ContactEntryValue { get; set; }
    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class ContactPossibilitiesEntryCreateDto
    {
        [Required(AllowEmptyStrings = false)]
        public string ContactEntryName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ContactEntryValue { get; set; }
    }
}
