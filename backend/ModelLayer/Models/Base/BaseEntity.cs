using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models.Base
{
    public abstract class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}