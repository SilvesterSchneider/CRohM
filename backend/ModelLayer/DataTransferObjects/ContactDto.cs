using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ModelLayer.Models;
using static ModelLayer.Models.Contact;

namespace ModelLayer.DataTransferObjects
{
    public class ContactDto
    {
        public long Id { get; set; }
        public GenderTypes Gender { get; set; }
        public string ContactPartner { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreName { get; set; }
        public AddressDto Address { get; set; }
        public ContactPossibilitiesDto ContactPossibilities { get; set; }
        public List<OrganizationDto> Organizations { get; set; } = new List<OrganizationDto>();
        public List<EventDto> Events { get; set; } = new List<EventDto>();
        public List<HistoryElementDto> History { get; set; } = new List<HistoryElementDto>();
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }

    public class ContactCreateDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public GenderTypes Gender { get; set; }
        public string ContactPartner { get; set; }

        [Required]
        public string PreName { get; set; }

        public AddressCreateDto Address { get; set; }
        public ContactPossibilitiesCreateDto ContactPossibilities { get; set; }
    }
}
