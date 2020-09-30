using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class Participated : BaseEntity
    {
        public MODEL_TYPE ModelType { get; set; }
        public long ObjectId { get; set; }
        public bool HasParticipated { get; set; }
        public bool WasInvited { get; set; }
    }
}

