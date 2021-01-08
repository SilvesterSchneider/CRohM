using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Models
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class UserDeletionCheckDate : BaseEntity
    {
        public DateTime DateOfNextCheck { get; set; }
    }
}
