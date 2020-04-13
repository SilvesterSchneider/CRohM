using ModelLayer.Models.Base;

namespace ModelLayer.Models
{
    public class Address : BaseEntity
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
    }
}