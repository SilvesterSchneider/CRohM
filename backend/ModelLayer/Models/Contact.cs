using ModelLayer.Models.Base;

namespace ModelLayer.Models
{
    public class Contact : BaseEntity
    {
        public string PreName { get; set; }
        public string PhoneNumber { get; set; }
        public string Mail { get; set; }
    }
}