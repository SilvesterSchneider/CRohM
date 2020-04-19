using System;
using System.ComponentModel.DataAnnotations.Schema;
using ModelLayer.Models.Base;

namespace ModelLayer.Models
{
    public class OrganizationContact : BaseEntity
    {
        #region obsolet

        [NotMapped]
        [Obsolete("Don't use this", true)]
        public new long Id { get; set; }

        [NotMapped]
        [Obsolete("Don't use this", true)]
        public new string Name { get; set; }

        [NotMapped]
        [Obsolete("Don't use this", true)]
        public new string Description { get; set; }

        #endregion obsolet

        public long OrganizationId { get; set; }

        public Organization Organization { get; set; }
        public long ContactId { get; set; }
        public Contact Contact { get; set; }
    }
}