using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoryLayer
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public interface IHistoryRepository : IBaseRepository<HistoryElement>
    {
        /// <summary>
        /// Get the history for a contact
        /// </summary>
        /// <param name="id"> contact id </param>
        /// <returns></returns>
        Task<List<HistoryElement>> GetHistoryByContactAsync(long id);

        /// <summary>
        /// Get the  history for an organisation (with pagination)
        /// </summary>
        /// <param name="id"> organization id </param>
        /// <param name="pageStart">starting position for pagination</param>
        /// <param name="pageSize">Number of elements requested by pagination</param>
        /// <returns></returns>
        Task<List<HistoryElement>> GetHistoryByOrganisationAsync(long id, int pageStart, int pageSize);

        /// <summary>
        /// Get Count of history by organisation
        /// </summary>
        /// <param name="id">organisation id</param>
        /// <returns></returns>
        Task<int> GetHistoryByOrganisationCountAsync(long id);

    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class HistoryRepository : BaseRepository<HistoryElement>, IHistoryRepository
    {
        public HistoryRepository(CrmContext context) : base(context)
        {
        }

        public async Task<List<HistoryElement>> GetHistoryByContactAsync(long id)
        {
            return await Entities
                .Where(entity => entity.contact.Id == id)
                .ToListAsync();
        }

        public async Task<List<HistoryElement>> GetHistoryByOrganisationAsync(long id, int pageStart, int pageSize)
        {
            return await Entities
                .Where(entity => entity.organization.Id == id)
                .OrderByDescending(x => x.Date)
                .Skip(pageStart)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetHistoryByOrganisationCountAsync(long id)
        {
            return await Entities
                .Where(entity => entity.organization.Id == id)
                .CountAsync();
        }

    }
}
