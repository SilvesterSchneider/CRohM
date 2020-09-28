using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public enum HistoryElementType
    {
        MAIL = 0,
        PHONE_CALL = 1,
        NOTE = 2,
        VISIT = 3
    }

    public class HistoryElement : BaseEntity
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public HistoryElementType Type { get; set; } = HistoryElementType.MAIL;
        public string Comment { get; set; } = "";
    }
}
