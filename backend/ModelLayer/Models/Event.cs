using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class Event : BaseEntity
    {
        //der tag des events
        public DateTime Date { get; set; } = DateTime.Now;
        //die genaue uhrzeit des events
        public DateTime Start { get; set; } = DateTime.Now;
        // Ende des Events
        public DateTime End { get; set; } = DateTime.Now;
        public DateTime Time { get; set; } = DateTime.Now;
        /// <summary>
        /// die dauer des events in x.y stunden
        /// </summary>
        public float Duration { get; set; } = 0f;

        public String Location { get; set; }
        //die kontakte die diesem event zugeordnet sind
        public List<EventContact> Contacts { get; set; } = new List<EventContact>();
        public List<EventOrganization> Organizations { get; set; } = new List<EventOrganization>();
        //die informationen darüber welche personen daran bereits teilgenommen haben oder nicht
        public List<Participated> Participated { get; set; } = new List<Participated>();
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
