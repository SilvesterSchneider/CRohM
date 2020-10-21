using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IHistoryService : IHistoryRepository
    {
    }

    public class HistoryService : HistoryRepository, IHistoryService
    {

        public HistoryService(CrmContext context) : base(context)
        {
            
        }

    }
}
