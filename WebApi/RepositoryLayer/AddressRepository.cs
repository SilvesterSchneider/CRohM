using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;

namespace RepositoryLayer
{
    public interface IAddressRepository : IBaseRepository<Address>
    {
    }

    public class AddressRepository : BaseRepository<Address>, IAddressRepository
    {
        public AddressRepository(CrmContext context) : base(context)
        {
        }
    }
}