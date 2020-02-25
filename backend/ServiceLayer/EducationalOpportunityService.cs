using System;
using System.Collections.Generic;
using System.Text;
using ModelLayer;
using RepositoryLayer;

namespace ServiceLayer
{
    public interface IEducationalOpportunityService : IEducationalOpportunityRepository
    {
    }

    public class EducationalOpportunityService : EducationalOpportunityRepository, IEducationalOpportunityService
    {
        public EducationalOpportunityService(CrmContext context) : base(context)
        {
        }
    }
}