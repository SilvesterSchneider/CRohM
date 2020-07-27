using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class UserDeletionCheckDate : BaseEntity
    {
        public DateTime DateOfLastCheck { get; set; }
    }
}
