using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class ModificationEntryDto
    {
        public long Id { get; set; }
        public DATA_TYPE DataType { get; set; }
        public MODEL_TYPE DataModelType { get; set; }
        public long DataModelId { get; set; }
        public MODIFICATION ModificationType { get; set; }
        public UserDto User { get; set; }
        public DateTime DateTime { get; set; }
        public string OldValue { get; set; }
        public string ActualValue { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int ExtensionIndex { get; set; } = -1;
        public string PropertyName { get; set; }
    }
}
