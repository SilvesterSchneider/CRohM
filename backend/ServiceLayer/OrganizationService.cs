using ModelLayer;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer
{
    public interface IOrganizationService : IOrganizationRepository
    {

    }

    public class OrganizationService : OrganizationRepository, IOrganizationService
    {
        public OrganizationService(CrmContext context) : base(context)
        {
        }
    }
}
