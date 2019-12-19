
using ModelLayer.DataTransferObjects.Base;

namespace ModelLayer.DataTransferObjects
{
    class AddressDto: BaseEntityDto
    {
        public string City { get; set; }
        public string Street { get; set; }
        public int StreetNumber { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
    }
}
