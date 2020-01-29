using ModelLayer;
using RepositoryLayer;

namespace ServiceLayer
{
    public interface IAddressService : IAddressRepository
    {
    }

    public class AddressService : AddressRepository, IAddressService
    {
        public AddressService(CrmContext ctx) : base(ctx)
        {
        }
    }
}