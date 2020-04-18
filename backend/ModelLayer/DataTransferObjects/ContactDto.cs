using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ModelLayer.Models;

namespace ModelLayer.DataTransferObjects
{
    public class ContactDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreName { get; set; }
        public AddressDto Address { get; set; }
        public ContactPossibilitiesDto ContactPossibilities { get; set; }
        public List<OrganizationDto> Organizations { get; set; }
    }

    public class ContactCreateDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string PreName { get; set; }

        public AddressCreateDto Address { get; set; }
        public ContactPossibilitiesCreateDto ContactPossibilities { get; set; }
    }
}