using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class ParticipatedDto
    {
        public long Id { get; set; }
        public MODEL_TYPE ModelType { get; set; }
        public long ObjectId { get; set; }
        public ParticipatedStatus EventStatus { get; set; } = ParticipatedStatus.NOT_INVITED;
        public bool HasParticipated { get; set; }
    }
}
