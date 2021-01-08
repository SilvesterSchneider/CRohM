using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public enum ParticipatedStatus
    {
        NOT_INVITED,
        INVITED,
        AGREED,
        CANCELLED
    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class Participated : BaseEntity
    {
        public MODEL_TYPE ModelType { get; set; }
        public long ObjectId { get; set; }
        public ParticipatedStatus EventStatus { get; set; } = ParticipatedStatus.NOT_INVITED;
        public bool HasParticipated { get; set; }
    }
}

