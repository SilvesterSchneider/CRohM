using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static ModelLayer.DataTransferObjects.StatisticsDto;

namespace ServiceLayer
{
    public interface IStatisticsService
    {
        Task<List<VerticalGroupedBarDto>> GetInvitedAndParticipatedRelationOfEvents();
    }

    public class StatisticsService : IStatisticsService
    {
        private IEventService eventService;

        public StatisticsService(IEventService eventService)
        {
            this.eventService = eventService;
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
