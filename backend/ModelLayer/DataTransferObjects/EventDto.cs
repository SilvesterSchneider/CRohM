using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class EventDto
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string Name { get; set; }
        public float Duration { get; set; }
        public List<ContactDto> Contacts { get; set; } = new List<ContactDto>();
        public List<OrganizationDto> Organizations { get; set; } = new List<OrganizationDto>();
        public List<ParticipatedDto> Participated { get; set; } = new List<ParticipatedDto>();
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }

    public class EventCreateDto
    {
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string Name { get; set; }
        public float Duration { get; set; }
        public List<int> Contacts { get; set; } = new List<int>();
        public List<int> Organizations { get; set; } = new List<int>();
    }
}
