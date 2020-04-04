﻿using System.ComponentModel.DataAnnotations;
using ModelLayer.Models;

namespace ModelLayer.DataTransferObjects
{
    public class ContactDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreName { get; set; }
        public AddressDto Address { get; set; }
        public ContactPossibilitiesDto ContactPossibilities { get; set; }
    }

    public class ContactCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string PreName { get; set; }
        [Required]
        public AddressCreateDto Address { get; set; }
        [Required]
        public ContactPossibilitiesCreateDto ContactPossibilities { get; set; }
    }
}