using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;

namespace RepositoryLayer
{
    public interface IAddressRepository : IBaseRepository<Address>
    {
        Task<List<Address>> GetByZipcodeAsync(string zipcode);
    }

    public class AddressRepository : BaseRepository<Address>, IAddressRepository
    {
        public AddressRepository(CrmContext context) : base(context)
        {
        }

        public async Task<List<Address>> GetByZipcodeAsync(string zipcode)
        {
            return await Entities
                .Where(addresses => addresses.Zipcode == zipcode)
                .ToListAsync();
        }
    }
}