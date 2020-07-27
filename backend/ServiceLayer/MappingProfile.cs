using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ModelLayer;
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
                    expression => expression.MapFrom((organization, dto) =>
                    {
                        if (organization.OrganizationContacts.Any())
                        {
                            return organization.OrganizationContacts
                                .Select(contact => contact.Contact)
                                .ToList();
                        }
                        else
                        {
                            return new List<Contact>();
                        }
                    }));

            CreateMap<OrganizationDto, Organization>(); //TODO: check-> do we need this?

            CreateMap<OrganizationCreateDto, Organization>();
            CreateMap<ContactPossibilitiesDto, ContactPossibilities>().ReverseMap();
            CreateMap<ContactPossibilities, ContactPossibilitiesDto>();
            CreateMap<ContactPossibilitiesCreateDto, ContactPossibilities>().ReverseMap();

            CreateMap<Contact, ContactDto>()
                .ForMember(dto => dto.Organizations,
                    expression => expression.MapFrom((organization, dto) =>
                    {
                        if (organization.OrganizationContacts.Any())
                        {
                            return organization.OrganizationContacts
                                .Select(contact => contact.Organization)
                                .ToList();
                        }
                        else
                        {
                            return new List<Organization>();
                        }
                    }))
                .ForMember(dto => dto.Events,
                    expression => expression.MapFrom((events, dto) =>
                    {
                        if (events.Events.Any())
                        {
                            return events.Events
                                .Select(eventToGet => eventToGet.Event)
                                .ToList();
                        }
                        else
                        {
                            return new List<Event>();
                        }
                    }));

            CreateMap<ContactDto, Contact>();
            CreateMap<ContactCreateDto, Contact>();
            CreateMap<ContactPossibilitiesEntryCreateDto, ContactPossibilitiesEntry>().ReverseMap();
            CreateMap<ContactPossibilitiesEntryDto, ContactPossibilitiesEntry>().ReverseMap();
            CreateMap<Event, EventDto>()
                .ForMember(dto => dto.Contacts,
                    expression => expression.MapFrom((contact, dto) =>
                    {
                        if (contact.Contacts.Any())
                        {
                            return contact.Contacts
                                .Select(contactToGet => contactToGet.Contact)
                                .ToList();
                        }
                        else
                        {
                            return new List<Contact>();
                        }
                    }));
            CreateMap<EventDto, Event>();
            CreateMap<EventCreateDto, Event>();
            CreateMap<Participated, ParticipatedDto>().ReverseMap();
            CreateMap<HistoryElement, HistoryElementDto>().ReverseMap();
            CreateMap<HistoryElementCreateDto, HistoryElement>();
            CreateMap<ModificationEntry, ModificationEntryDto>();
            CreateMap<PermissionGroupDto, PermissionGroup>().ReverseMap();
            CreateMap<PermissionGroupCreateDto, PermissionGroup>();
            CreateMap<TagOrganization, TagDto>().ReverseMap();
        }
    }
}
