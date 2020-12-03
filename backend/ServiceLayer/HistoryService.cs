using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IHistoryService : IHistoryRepository
    {
        Task<HistoryElement> GetHistoryByEventAsync(long eventId);

        Task CreateOrUpdateForContactAsync(long eventId, Contact contact, bool arrived);
    }

    public class HistoryService : HistoryRepository, IHistoryService
    {
        public HistoryService(CrmContext context) : base(context)
        {
        }

        public async Task<HistoryElement> GetHistoryByEventAsync(long eventId)
        {
            return await Entities.FirstOrDefaultAsync(history => history.EventId == eventId);
        }

        public async Task CreateOrUpdateForContactAsync(long eventId, Contact contact, bool arrived)
        {
            var history = await Entities
                .Include(history => history.contact)
                .FirstOrDefaultAsync(history => history.EventId == eventId &&
                                                history.contact.Id == contact.Id &&
                                                history.Arrived != null);
            if (history != null)
            {
                history.Date = DateTime.UtcNow;
                history.Arrived = arrived;
            }
            else
            {
                await CreateAsync(new HistoryElement()
                {
                    contact = contact,
                    Date = DateTime.UtcNow,
                    Type = HistoryElementType.VISIT,
                    EventId = eventId,
                    Arrived = arrived
                });
            }
        }

        public async Task CreateOrUpdateForOrganizationAsync(long eventId, Organization organization, bool arrived)
        {
            var history = await Entities
                .Include(history => history.organization)
                .FirstOrDefaultAsync(history => history.EventId == eventId &&
                                                history.contact.Id == organization.Id &&
                                                history.Arrived != null);
            if (history != null)
            {
                history.Date = DateTime.UtcNow;
                history.Arrived = arrived;
            }
            else
            {
                await CreateAsync(new HistoryElement()
                {
                    organization = organization,
                    Date = DateTime.UtcNow,
                    Type = HistoryElementType.VISIT,
                    EventId = eventId
                });
            }
        }
    }
}
