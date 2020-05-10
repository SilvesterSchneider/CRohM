using ModelLayer;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer
{
    public interface IContactService : IContactRepository
    {

    }

    public class ContactService : ContactRepository, IContactService
    {
        public ContactService(CrmContext context, IAddressRepository addressRepository, IContactPossibilitiesRepository contactPossibilitiesRepository) : base(context, addressRepository, contactPossibilitiesRepository)
        {
        }
    }
}
