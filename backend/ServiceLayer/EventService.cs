using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IEventService : IEventRepository
    {
        Task<bool> SendInvitationMailsAsync(List<long> contacts, List<long> orgaIds, string mailContent);
    }

    public class EventService : EventRepository, IEventService
    {
        private IMailService mailService;
        private IContactRepository contactRepo;
        private IOrganizationRepository orgaRepo;

        public EventService(CrmContext context,
            IEventContactRepository eventContactRepo,
            IEventOrganizationRepository eventOrgaRepo,
            IContactRepository contactRepo,
            IMailService mailService,
            IOrganizationRepository orgaRepo) : base(context, eventContactRepo, eventOrgaRepo, contactRepo, orgaRepo)
        {
            this.mailService = mailService;
            this.contactRepo = contactRepo;
            this.orgaRepo = orgaRepo;
        }

        public async Task<bool> SendInvitationMailsAsync(List<long> contacts, List<long> orgaIds, string mailContent)
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
            foreach (long orgaId in orgaIds)
            {
                Organization orga = await orgaRepo.GetByIdAsync(orgaId);
                if (orga != null)
                {
                    mailService.CreateAndSendInvitationMail(orga.Contact.Mail, orga.Name, mailContent);
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
