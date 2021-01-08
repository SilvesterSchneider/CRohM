using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System.Linq;

namespace RepositoryLayer
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public interface IContactPossibilitiesEntryRepository : IBaseRepository<ContactPossibilitiesEntry>
    {
        /// <summary>
        /// gesamte anzahl an entities.
        /// </summary>
        /// <returns></returns>
        int GetTotalAmountOfEntities();
    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class ContactPossibilitiesEntryRepository : BaseRepository<ContactPossibilitiesEntry>, IContactPossibilitiesEntryRepository
    {
        public ContactPossibilitiesEntryRepository(CrmContext crm) : base(crm)
        {

        }

        public int GetTotalAmountOfEntities()
        {
            return Entities.ToList().Count;
        }
    }
}
