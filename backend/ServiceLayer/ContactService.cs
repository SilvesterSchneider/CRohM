using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace ServiceLayer
{
    public interface IContactService : IContactRepository
    {
        Task AddHistoryElement(long id, HistoryElement historyElement);

        /// <summary>
        /// Get all information of a contact in a text form for later pdf creation usage.
        /// </summary>
        /// <param name="id">the contact id</param>
        /// <returns>the text</returns>
        Task<string> GetContactInformationAsTextAsync(long id);
    }

    public class ContactService : ContactRepository, IContactService
    {
        public ContactService(CrmContext context) : base(context)
        {
        }

        public async Task AddHistoryElement(long id, HistoryElement historyElement)
        {
            Contact contact = await Entities
                .Include(x => x.Address)
                .Include(x => x.ContactPossibilities)
                .Include(x => x.Events)
                .ThenInclude(y => y.Event)
                .Include(x => x.OrganizationContacts)
                .Include(x => x.History)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (contact != null)
            {
                contact.History.Add(historyElement);
                await UpdateAsync(contact);
            }
        }

        public async Task<string> GetContactInformationAsTextAsync(long id)
        {
            Contact contact = await GetByIdAsync(id);
            if (contact == null)
            {
                return string.Empty;
            }
            return await Task.FromResult(contact.ToString());
        }
    }
}
