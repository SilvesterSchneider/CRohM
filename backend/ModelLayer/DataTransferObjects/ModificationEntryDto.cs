using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class ModificationEntryDto
    {
        public long Id { get; set; }
        public MODEL_TYPE DataModelType { get; set; }
        public long DataModelId { get; set; }
        public MODIFICATION ModificationType { get; set; }
        public string UserName { get; set; }
        public DateTime DateTime { get; set; }
    }
}
