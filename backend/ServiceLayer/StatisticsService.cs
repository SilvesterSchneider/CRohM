using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModelLayer.DataTransferObjects.StatisticsDto;

namespace ServiceLayer
{
    public interface IStatisticsService
    {
        Task<List<VerticalGroupedBarDto>> GetInvitedAndParticipatedRelationOfEvents();
        Task<List<VerticalGroupedBarDto>> GetAllCreatedObjects();
    }

    public class StatisticsService : IStatisticsService
    {
        private IEventService eventService;
        private IModificationEntryService modService;

        public StatisticsService(IEventService eventService, IModificationEntryService modService)
        {
            this.eventService = eventService;
            this.modService = modService;
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
                if (ev.Contacts != null && ev.Contacts.Count > 0)
                {
                    VerticalGroupedBarDto dto = new VerticalGroupedBarDto();
                    dto.Name = ev.Date.ToString("yyyy-MM-dd");
                    dto.Series = new List<VerticalGroupedBarDataSet>();
                    dto.Series.Add(new VerticalGroupedBarDataSet() { Name = StatisticsDto.SERIES_INVITED_CONTACTS, Value = ev.Contacts.Count });
                    dto.Series.Add(new VerticalGroupedBarDataSet() { Name = StatisticsDto.SERIES_PARTICIPATED_CONTACS, Value = ev.Participated.FindAll(a => a.HasParticipated).Count });
                    list.Add(dto);
                }
            }
            return list;
        }
    }
}
