using ModelLayer.Models.Base;

namespace ModelLayer.Models
{
    public class Contact : BaseEntity
    {
        public string PreName { get; set; }
        public Address Address { get; set; }
        public ContactPossibilities ContactPossibilities { get; set; }
    }
}