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
                    expression => expression.MapFrom((organization, organizationDto) =>
                    {
                        if (organization.OrganizationContacts.Any())
                        {
                            return organization.OrganizationContacts
                                .Select(organizationContact => organizationContact.Contact)
                                .ToList();
                        }
                        else
                        {
                            return new List<Contact>();
                        }
                    }));

            CreateMap<OrganizationDto, Organization>()
                .ForMember(dto => dto.OrganizationContacts,
                    expression => expression.MapFrom((organizationDto, organization) =>
                    {
                        if (organizationDto.Employees.Any())
                        {
                            return organizationDto.Employees
                                .Select(innerContact => new OrganizationContact() { ContactId= innerContact.Id, OrganizationId=organizationDto.Id })
                                .ToList();
                        }
                        else
                        {
                            return new List<OrganizationContact>();
                        }
                    }));

            CreateMap<OrganizationCreateDto, Organization>();
            CreateMap<ContactPossibilitiesDto, ContactPossibilities>().ReverseMap();
            CreateMap<ContactPossibilities, ContactPossibilitiesDto>();
            CreateMap<ContactPossibilitiesCreateDto, ContactPossibilities>().ReverseMap();

            CreateMap<Contact, ContactDto>()
                .ForMember(dto => dto.Organizations,
                    expression => expression.MapFrom((contact, contactDto) =>
                    {
                        if (contact.OrganizationContacts.Any())
                        {
                            return contact.OrganizationContacts
                                .Select(organizationContact => organizationContact.Organization)
                                .ToList();
                        }
                        else
                        {
                            return new List<Organization>();
                        }
                    }))
                .ForMember(dto => dto.Events,
                    expression => expression.MapFrom((contact, contactDto) =>
                    {
                        if (contact.Events.Any())
                        {
                            return contact.Events
                                .Select(innerEvent => innerEvent.Event)
                                .ToList();
                        }
                        else
                        {
                            return new List<Event>();
                        }
                    }));
            CreateMap<ContactDto, Contact>()
                .ForMember(dto => dto.OrganizationContacts,
                    expression => expression.MapFrom((contactDto, contact) =>
                    {
                        if (contactDto.Organizations.Any())
                        {
                            return contactDto.Organizations
                                .Select(innerOrganization => new OrganizationContact() { ContactId = contactDto.Id, OrganizationId = innerOrganization.Id })
                                .ToList();
                        }
                        else
                        {
                            return new List<OrganizationContact>();
                        }
                    }))
                .ForMember(dto => dto.Events,
                    expression => expression.MapFrom((contactDto, contact) =>
                    {
                        if (contactDto.Events.Any())
                        {
                            return contactDto.Events
                                .Select(innerEvent => new EventContact() { ContactId = contactDto.Id, EventId = innerEvent.Id })
                                .ToList();
                        }
                        else
                        {
                            return new List<EventContact>();
                        }
                    }));
            CreateMap<ContactCreateDto, Contact>();
            CreateMap<ContactPossibilitiesEntryCreateDto, ContactPossibilitiesEntry>().ReverseMap();
            CreateMap<ContactPossibilitiesEntryDto, ContactPossibilitiesEntry>().ReverseMap();
            CreateMap<Event, EventDto>()
                .ForMember(dto => dto.Contacts,
                    expression => expression.MapFrom((modelEvent, eventDto) =>
                    {
                        if (modelEvent.Contacts.Any())
                        {
                            return modelEvent.Contacts
                                .Select(eventContact => eventContact.Contact)
                                .ToList();
                        }
                        else
                        {
                            return new List<Contact>();
                        }
                    }))
                .ForMember(dto => dto.Organizations,
                    expression => expression.MapFrom((modelEvent, eventDto) =>
                    {
                        if (modelEvent.Organizations.Any())
                        {
                            return modelEvent.Organizations
                                .Select(eventOrga => eventOrga.Organization)
                                .ToList();
                        }
                        else
                        {
                            return new List<Organization>();
                        }
                    }));
            CreateMap<EventDto, Event>()
                .ForMember(dto => dto.Contacts,
                    expression => expression.MapFrom((eventDto, modelEvent) =>
                    {
                        if (eventDto.Contacts.Any())
                        {
                            return eventDto.Contacts
                                .Select(innerContact => new EventContact() { ContactId = innerContact.Id, EventId = eventDto.Id })
                                .ToList();
                        } else
                        {
                            return new List<EventContact>();
                        }                        
                    }))
                .ForMember(dto => dto.Organizations,
                    expression => expression.MapFrom((eventDto, modelEvent) =>
                    {
                        if (eventDto.Organizations.Any())
                        {
                            return eventDto.Organizations
                                .Select(innerOrga => new EventOrganization() { OrganizationId = innerOrga.Id, EventId = eventDto.Id })
                                .ToList();
                        }
                        else
                        {
                            return new List<EventOrganization>();
                        }
                    }));
            CreateMap<EventCreateDto, Event>();
            CreateMap<Participated, ParticipatedDto>().ReverseMap();
            CreateMap<HistoryElement, HistoryElementDto>().ReverseMap();
            CreateMap<HistoryElementCreateDto, HistoryElement>();
            CreateMap<ModificationEntry, ModificationEntryDto>();
            CreateMap<Tag, TagDto>().ReverseMap();
        }
    }
}
