using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoryLayer
{
    public interface IContactHistoryRepository : IBaseRepository<HistoryElement>
    {
        /// <summary>
        /// Get the contact history for a contact
        /// </summary>
        /// <param name="id"> contact id </param>
        /// <returns></returns>
        Task<List<HistoryElement>> GetContactHistoryByContactAsync(long id);
    }

    public class ContactHistoryRepository : BaseRepository<HistoryElement>, IContactHistoryRepository
    {
        public ContactHistoryRepository(CrmContext context) : base(context)
        {
        }

        public async Task<List<HistoryElement>> GetContactHistoryByContactAsync(long id)
        {
            return await Entities
                .Where(entity => entity.contact.Id == id)
                .ToListAsync();
        }
    }
}
