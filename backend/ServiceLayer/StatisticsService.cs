using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ModelLayer.DataTransferObjects.StatisticsDto;

namespace ServiceLayer
{
    public interface IStatisticsService
    {
        Task<List<VerticalGroupedBarDto>> GetInvitedAndParticipatedRelationOfEvents();
        Task<List<VerticalGroupedBarDto>> GetAllCreatedObjects();
        Task<List<VerticalGroupedBarDto>> GetAllTags();
    }

    public class StatisticsService : IStatisticsService
    {
        private IEventService eventService;
        private IModificationEntryService modService;
        private IContactService contactService;
        private IOrganizationService organizationService;

        public StatisticsService(IEventService eventService, IModificationEntryService modService, IContactService contactService, IOrganizationService organizationService)
        {
            this.eventService = eventService;
            this.modService = modService;
            this.organizationService = organizationService;
            this.contactService = contactService;
        }

        public async Task<List<VerticalGroupedBarDto>> GetAllCreatedObjects()
        {
            List<VerticalGroupedBarDto> list = new List<VerticalGroupedBarDto>();
            List<ModificationEntry> listContacts = await modService.GetModificationEntriesForCreationByModelType(MODEL_TYPE.CONTACT);
            List<ModificationEntry> listOrgas = await modService.GetModificationEntriesForCreationByModelType(MODEL_TYPE.ORGANIZATION);
            List<ModificationEntry> listEvents = await modService.GetModificationEntriesForCreationByModelType(MODEL_TYPE.EVENT);
            CreateDtoObjectsForCreationlist(listContacts, list, StatisticsDto.SERIES_CREATED_CONTACTS);
            CreateDtoObjectsForCreationlist(listOrgas, list, StatisticsDto.SERIES_CREATED_ORGANIZATIONS);
            CreateDtoObjectsForCreationlist(listEvents, list, StatisticsDto.SERIES_CREATED_EVENTS);
            return list;
        }

        private void CreateDtoObjectsForCreationlist(List<ModificationEntry> listEntries, List<VerticalGroupedBarDto> list, string modelName)
        {
            foreach (ModificationEntry entry in listEntries)
            {
                String dateToFind = entry.DateTime.ToString("yyyy-MM-dd");
                VerticalGroupedBarDto dto = list.FirstOrDefault(x => x.Name.Equals(dateToFind));
                if (dto == null)
                {
                    dto = new VerticalGroupedBarDto() { Name = dateToFind, Series = new List<VerticalGroupedBarDataSet>() };
                    list.Add(dto);
                }
                VerticalGroupedBarDataSet data = dto.Series.FirstOrDefault(x => x.Name.Equals(modelName));
                if (data == null)
                {
                    data = new VerticalGroupedBarDataSet() { Name = modelName, Value = 0 };
                    dto.Series.Add(data);
                }
                data.Value += 1;
            }
        }

        public async Task<List<VerticalGroupedBarDto>> GetInvitedAndParticipatedRelationOfEvents()
        {
            List<VerticalGroupedBarDto> list = new List<VerticalGroupedBarDto>();
            List<Event> events = await eventService.GetAllEventsWithAllIncludesAsync();
            foreach (Event ev in events)
            {
                int count = 0;
                if (ev.Contacts != null && ev.Contacts.Count > 0)
                {
                    count = ev.Contacts.Count;
                }
                if (ev.Organizations != null && ev.Organizations.Count > 0)
                {
                    count += ev.Organizations.Count;
                }
                if (count > 0)
                { 
                    VerticalGroupedBarDto dto = new VerticalGroupedBarDto();
                    dto.Name = ev.Date.ToString("yyyy-MM-dd");
                    dto.Series = new List<VerticalGroupedBarDataSet>();
                    dto.Series.Add(new VerticalGroupedBarDataSet() { Name = StatisticsDto.SERIES_INVITED_CONTACTS, Value = count });
                    dto.Series.Add(new VerticalGroupedBarDataSet() { Name = StatisticsDto.SERIES_PARTICIPATED_CONTACS, Value = ev.Participated.FindAll(a => a.EventStatus == ParticipatedStatus.PARTICIPATED).Count });
                    list.Add(dto);
                }
            }
            return list;
        }

        public async Task<List<VerticalGroupedBarDto>> GetAllTags()
        {
            List<VerticalGroupedBarDto> list = new List<VerticalGroupedBarDto>();
            foreach (Contact contact in await contactService.GetAllContactsWithAllIncludesAsync())
            {
                foreach (Tag tag in contact.Tags)
                {
                    await CheckTagInternally(tag, list, MODEL_TYPE.CONTACT);
                }
            }
            foreach (Organization organization in await organizationService.GetAllOrganizationsWithIncludesAsync())
            {
                foreach (Tag tag in organization.Tags)
                {
                    await CheckTagInternally(tag, list, MODEL_TYPE.ORGANIZATION);
                }
            }
            foreach (Event EventCheck in await eventService.GetAllEventsWithAllIncludesAsync())
            {
                foreach (Tag tag in EventCheck.Tags)
                {
                    await CheckTagInternally(tag, list, MODEL_TYPE.EVENT);
                }
            }
            return list;
        }

        private async Task CheckTagInternally(Tag tag, List<VerticalGroupedBarDto> list, MODEL_TYPE model)
        {
            await Task.Run(() =>
            {
                VerticalGroupedBarDto vDto = list.FirstOrDefault(a => a.Name.Equals(tag.Name));
                if (vDto == null)
                {
                    vDto = new VerticalGroupedBarDto() { Name = tag.Name };
                    list.Add(vDto);
                }
                string enumName = EnumHelper.GetEnumMemberValue(model);
                VerticalGroupedBarDataSet vEntry = vDto.Series.FirstOrDefault(a => a.Name.Equals(enumName));
                if (vEntry == null)
                {
                    vEntry = new VerticalGroupedBarDataSet() { Name = enumName, Value = 0 };
                    vDto.Series.Add(vEntry);
                }
                vEntry.Value++;
            });
        }
    }
}
