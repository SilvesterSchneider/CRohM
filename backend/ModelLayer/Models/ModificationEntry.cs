using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public enum MODEL_TYPE
    {
        CONTACT,
        ORGANIZATION,
        EVENT
    }

    public enum MODIFICATION
    {
        CREATED,
        MODIFIED
    }

    public class ModificationEntry : BaseEntity
    {
        public MODEL_TYPE DataModelType { get; set; }
        public long DataModelId { get; set; }
        public MODIFICATION ModificationType { get; set; }
        public string UserName { get; set; }
        public DateTime DateTime { get; set; }
    }
}
