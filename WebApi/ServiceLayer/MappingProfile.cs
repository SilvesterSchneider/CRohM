using AutoMapper;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;

namespace ServiceLayer
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Address, AddressDto>().ReverseMap();
        }
    }
}