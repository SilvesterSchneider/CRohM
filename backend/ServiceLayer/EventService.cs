using Microsoft.AspNetCore.Identity;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IEventService : IEventRepository
    {
        Task<bool> SendInvitationMailsAsync(Event oldOne, List<long> contactIds, string mailContent);
    }

    public class EventService : EventRepository, IEventService
    {
        private IMailService mailService;
        public EventService(CrmContext context, IEventContactRepository eventContactRepo, IContactRepository contactRepo, IMailService mailService) : base(context, eventContactRepo, contactRepo)
        {
            this.mailService = mailService;
        }

        public async Task<bool> SendInvitationMailsAsync(Event oldOne, List<long> contactIds, string mailContent)
        {
            foreach (long id in contactIds)
            {
                Participated part = oldOne.Participated.FirstOrDefault(a => a.ContactId == id);
                if (part == null)
                {
                    oldOne.Participated.Add(new Participated() { ContactId = id, HasParticipated = false, WasInvited = true });
                }
                else if (!part.WasInvited)
                {
                    part.WasInvited = true;
                }
            }
            await UpdateAsync(oldOne);
            foreach (Participated part in oldOne.Participated)
            {
                if (part.WasInvited)
                {
                    Contact contact = oldOne.Contacts.FirstOrDefault(a => a.ContactId == part.ContactId).Contact;
                    mailService.CreateAndSendInvitationMail(contact.ContactPossibilities.Mail, oldOne.Description, oldOne.Date.ToString(), oldOne.Time.ToString(), contact.Name, mailContent, contact.Gender);
                }
            }
            return true; 
        }
    }
}
