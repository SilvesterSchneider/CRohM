using System.ComponentModel.DataAnnotations;
using ModelLayer.Models;

namespace ModelLayer.DataTransferObjects
{
    public class ContactDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreName { get; set; }
        public Address Address { get; set; }
        public ContactPossibilities ContactPossibilities { get; set; }
    }

    public class ContactCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }


        // TODO
    }
}