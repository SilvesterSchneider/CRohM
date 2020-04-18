using System.Linq;
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

            CreateMap<Organization, OrganizationDto>()
                .ForMember(dto => dto.Employees,
                    expression => expression.MapFrom((organization, dto) => organization.OrganizationContacts
                    .Select(contact => contact.Contact)
                    .ToList()));

            CreateMap<OrganizationDto, Organization>(); //TODO: check-> do we need this?

            CreateMap<OrganizationCreateDto, Organization>();
            CreateMap<ContactPossibilitiesDto, ContactPossibilities>();
            CreateMap<ContactPossibilities, ContactPossibilitiesDto>();
            CreateMap<ContactPossibilitiesCreateDto, ContactPossibilities>();

            CreateMap<Contact, ContactDto>()
                .ForMember(dto => dto.Organizations,
                    expression => expression.MapFrom((organization, dto) => organization.OrganizationContacts
                        .Select(contact => contact.Organization)
                        .ToList()));
            CreateMap<ContactDto, Contact>();

            CreateMap<ContactCreateDto, Contact>();
        }
    }
}