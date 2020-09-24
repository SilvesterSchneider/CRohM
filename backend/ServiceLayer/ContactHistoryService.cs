using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IContactHistoryService : IContactHistoryRepository
    {
    }

    public class ContactHistoryService : ContactHistoryRepository, IContactHistoryService
    {

        public ContactHistoryService(CrmContext context) : base(context)
        {
            
        }

    }
}
