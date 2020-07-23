using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System.Linq;

namespace RepositoryLayer
{
    public interface IContactPossibilitiesEntryRepository : IBaseRepository<ContactPossibilitiesEntry>
    {
        /// <summary>
        /// gesamte anzahl an entities.
        /// </summary>
        /// <returns></returns>
        int GetTotalAmountOfEntities();
    }

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
