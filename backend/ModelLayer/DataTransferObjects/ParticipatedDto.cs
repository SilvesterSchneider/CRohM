using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class ParticipatedDto
    {
        public long Id { get; set; }
        public long ContactId { get; set; }
        public bool HasParticipated { get; set; }
        public bool WasInvited { get; set; }
    }
}
