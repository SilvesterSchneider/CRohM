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

        Task<bool> ApproveContact(long id);
    }

    public class ContactService : ContactRepository, IContactService
    {
        private readonly IMailService mailProvider;

        public ContactService(CrmContext context, IMailService mailProvider) : base(context)
        {
            this.mailProvider = mailProvider;
        }

        public async Task SendDisclosure(long id)
        {
            Contact contact = await GetByIdAsync(id);
            string gender = " geehrter Herr ";
            if (contact.Gender == Contact.GenderTypes.FEMALE)
            {
                gender = " geehrte Frau ";
            }
            else if (contact.Gender == Contact.GenderTypes.DIVERS)
            {
                gender = " geehrt ";
            }

            string body = "<h3> Auskunft über gespeicherte Daten </h3> " +
                          "<p> Sehr" + gender + contact.Name + ",</p>" +
                          "<p> Sie hatten um eine Auskunft über die zur Ihrer Person in unserem Customer Relationship Management System(CRMS) gespeicherten Daten gebeten. Im angehängten PDF - Dokument erhalten Sie die entsprechende Auskunft gem. Art. 15 EU - DSGVO.</p>" +
                          "<p></p>" +
                          "<p>Technische Hochschule Nürnberg</p>";
            string pdfBody = await GetContactInformationAsTextAsync(contact.Id);
            pdfBody = pdfBody + "";
            mailProvider.CreateAndSendMail(contact.ContactPossibilities.Mail, "Auskunft über gespeicherte Daten", body,
                PdfGenerator.GenerateNewContactPdf(contact, pdfBody), "disclosure.pdf");
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

        public async Task<bool> ApproveContact(long id) {
            Contact contact = await GetByIdAsync(id);
            if (contact != null) {
                contact.isApproved = true;
                return await UpdateAsync(contact, id);
            }
            return false;

        }
    }
}
