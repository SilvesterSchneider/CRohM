using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DataTransferObjects
{
    public class AddressDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int StreetNumber { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
    }

    public class AddressCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Street { get; set; }

        [Required]
        public int StreetNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Zipcode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Country { get; set; }
    }
}