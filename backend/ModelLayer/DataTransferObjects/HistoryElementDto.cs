using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class HistoryElementDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public HistoryElementType Type { get; set; }
        public string Comment { get; set; }
        public long? EventId { get; set; }
        public bool? Arrived { get; set; }
        public HistoryState? State { get; set; }
    }

    public class HistoryElementCreateDto
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public DateTime Date { get; set; }

        [Required(AllowEmptyStrings = false)]
        public HistoryElementType Type { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Comment { get; set; }
    }
}
