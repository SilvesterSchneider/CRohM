using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;

namespace RepositoryLayer
{
    public interface IEducationalOpportunityRepository : IBaseRepository<EducationalOpportunity>
    {
        Task<List<EducationalOpportunity>> GetByEctsAsync(float ects);
    }

    public class EducationalOpportunityRepository : BaseRepository<EducationalOpportunity>, IEducationalOpportunityRepository
    {
        public EducationalOpportunityRepository(CrmContext context) : base(context)
        {
        }

        public async Task<List<EducationalOpportunity>> GetByEctsAsync(float ects)
        {
            return await Entities
                .Where(x => Math.Abs(x.Ects - ects) < 0.1)
                .ToListAsync();
        }
    }
}