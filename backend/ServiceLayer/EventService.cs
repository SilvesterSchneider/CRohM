using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IEventService : IEventRepository
    {
        Task<bool> SendInvitationMailsAsync(List<long> contacts, string mailContent);
    }

    public class EventService : EventRepository, IEventService
    {
        private IMailService mailService;
        private IContactRepository contactRepo;

        public EventService(CrmContext context, IEventContactRepository eventContactRepo, IContactRepository contactRepo, IMailService mailService) : base(context, eventContactRepo, contactRepo)
        {
            this.mailService = mailService;
            this.contactRepo = contactRepo;
        }

        public async Task<bool> SendInvitationMailsAsync(List<long> contacts, string mailContent)
        {
            bool ok = true;
            foreach (long contactId in contacts)
            {
                Contact contact = await contactRepo.GetByIdAsync(contactId);
                if (contact != null)
                {
                    mailService.CreateAndSendInvitationMail(contact.ContactPossibilities.Mail, contact.PreName, contact.Name, mailContent, contact.Gender);
                }
                else
                {
                    ok = false;
                }                
            }
            return ok; 
        }
    }
}
