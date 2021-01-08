using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    /// <summary>
    /// RAM: 90%
    /// </summary>
    public class Event : BaseEntity
    {
        //der tag des events
        public DateTime Date { get; set; } = DateTime.Now;
        //die genaue uhrzeit des eventstarts
        public DateTime Starttime { get; set; } = DateTime.Now;

        //die genaue uhrzeit des eventendes
        public DateTime Endtime { get; set; } = DateTime.Now.AddHours(1);

        public String Location { get; set; }
        //die kontakte die diesem event zugeordnet sind
        public List<EventContact> Contacts { get; set; } = new List<EventContact>();
        public List<EventOrganization> Organizations { get; set; } = new List<EventOrganization>();
        //die informationen darüber welche personen daran bereits teilgenommen haben oder nicht
        public List<Participated> Participated { get; set; } = new List<Participated>();
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}

