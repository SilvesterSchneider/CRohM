using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System.Linq;

namespace RepositoryLayer
{
    public interface IContactPossibilitiesEntryRepository : IBaseRepository<ContactPossibilitiesEntry>
    {
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
