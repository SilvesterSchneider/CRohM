using ModelLayer;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer
{
    public interface IContactPossibilitiesService : IContactPossibilitiesRepository
    {

    }

    public class ContactPossibilitiesService : ContactPossibilitiesRepository, IContactPossibilitiesService
    {
        public ContactPossibilitiesService(CrmContext context) : base(context) { }
    }
}
