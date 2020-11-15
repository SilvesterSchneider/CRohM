using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ServiceLayer
{
    public interface IContactCheckDateService : IContactRepository
    {
        Task DelecteContactsWithoutApproval();
    }


    public class ContactCheckDateService : ContactRepository, IContactCheckDateService
    {
        public ContactCheckDateService(CrmContext context) : base(context)
        {
        }

        public async Task DelecteContactsWithoutApproval()
        {
            List<Contact> contacts = await GetAllUnapprovedContactsWithAllIncludesAsync();
            foreach (Contact c in contacts){
                if (c.CreationDate.AddDays(30) < DateTime.Now){
                    await DeleteAsync(c);
                }
            }
        }
    }
}
