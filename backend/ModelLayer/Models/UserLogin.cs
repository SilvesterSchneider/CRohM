using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    public class UserLogin : BaseEntity
    {
        public long UserId { get; set; }
        public DateTime DateTimeOfLastLogin { get; set; }
    }
}
