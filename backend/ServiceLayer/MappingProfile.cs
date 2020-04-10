using AutoMapper;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;

namespace ServiceLayer
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Address, AddressDto>();
            CreateMap<AddressDto, Address>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<AddressCreateDto, Address>();

            CreateMap<EducationalOpportunity, EducationalOpportunityDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserCreateDto, User>();
            CreateMap<Organization, OrganizationDto>();
            CreateMap<OrganizationDto, Organization>();
            CreateMap<OrganizationCreateDto, Organization>();
            CreateMap<ContactPossibilitiesDto, ContactPossibilities>();
            CreateMap<ContactPossibilities, ContactPossibilitiesDto>();
            CreateMap<ContactPossibilitiesCreateDto, ContactPossibilities>();
            CreateMap<Contact, ContactDto>();
            CreateMap<ContactDto, Contact>();
            CreateMap<ContactCreateDto, Contact>();
        }
    }
}