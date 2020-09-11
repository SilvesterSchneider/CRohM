using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class OrganizationDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public AddressDto Address { get; set; }

        public ContactPossibilitiesDto Contact { get; set; }

        public List<ContactDto> Employees { get; set; } = new List<ContactDto>();

        public List<TagDto> Tags { get; set; } = new List<TagDto>();

        public List<HistoryElementDto> History { get; set; } = new List<HistoryElementDto>();
    }

    public class OrganizationCreateDto
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string Description { get; set; }

        public AddressCreateDto Address { get; set; }

        public ContactPossibilitiesCreateDto Contact { get; set; }

        public List<ContactCreateDto> Employees { get; set; }
    }
}
