using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class Event : BaseEntity
    {
        //der tag des events
        public DateTime Date { get; set; }
        //die genaue uhrzeit des events
        public DateTime Time { get; set; }
        //die dauer des events in x.y stunden
        public float Duration { get; set; }
        //die kontakte die diesem event zugeordnet sind
        public List<EventContact> Contacts { get; set; } = new List<EventContact>();
        //die informationen darüber welche personen daran bereits teilgenommen haben oder nicht
        public List<Participated> Participated { get; set; } = new List<Participated>();
    }
}
