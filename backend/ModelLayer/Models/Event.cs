using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class Event : BaseEntity
    {
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public float Duration { get; set; }
        public List<EventContact> Contacts { get; set; } = new List<EventContact>();
        public List<Participated> Participated { get; set; } = new List<Participated>();
    }
}
