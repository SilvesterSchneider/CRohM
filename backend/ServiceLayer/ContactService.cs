using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System.Threading.Tasks;

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
        
        Task SendDisclosure(long id);
    }

    public class ContactService : ContactRepository, IContactService
    {
        private readonly IMailProvider mailProvider;
        public ContactService(CrmContext context, IMailProvider mailProvider) : base(context)
        {
            this.mailProvider = mailProvider;
        }

        public async Task SendDisclosure(long id)
        {
            Contact contact = await GetByIdAsync(id);
            string body = "<h3> Auskunft über gespeicherte Daten </h3> " +
                          "<p> Sehr geehrte/r Herr/Frau " + contact.Name + ",</p>" +
                          "<p Sie hatten um eine Auskunft über die zur Ihrer Person in unserem Customer Relationship Management System(CRMS) gespeicherten " +
                          "Daten gebeten. Im angehängten PDF - Dokument erhalten Sie die entsprechende Auskunft gem. Art. 15 EU - DSGVO.</p>" +
                          "<p></p>" +
                          "<p>Technische Hochschule Nürnberg</p>";
            mailProvider.CreateAndSendMail(contact.ContactPossibilities.Mail, "Auskunft über gespeicherte Daten", body,
                PdfGenerator.GenerateNewContactPdf(contact), "disclosure.pdf");
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
