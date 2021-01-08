using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public interface IHistoryService : IHistoryRepository
    {
    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class HistoryService : HistoryRepository, IHistoryService
    {

        public HistoryService(CrmContext context) : base(context)
        {
            
        }

    }
}
