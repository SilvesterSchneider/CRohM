
using System.Linq;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;

namespace RepositoryLayer
{
    class AddressRepository:BaseRepository<Address>
    {
        public AddressRepository(CrmContext context) : base(context)
        {
        }

        public Address SpecialFunction()
        {
            return Entities.FirstOrDefault();
        }
    }
}
