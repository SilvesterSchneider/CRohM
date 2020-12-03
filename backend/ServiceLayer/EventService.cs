using System;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApi.Helper;

namespace ServiceLayer
{
    public interface IEventService : IEventRepository
    {
        Task<bool> SendInvitationMailsAsync(long eventId, List<long> contacts, List<long> orgaIds, string mailContent);

        Task HandleInvitationResponseForContactAsync(long id, long contactId, bool participate, string passCode);

        Task HandleInvitationResponseForOrganizationAsync(long id, long organizationId, bool participate, string passCode);
    }

    public class EventService : EventRepository, IEventService
    {
        private IMailService mailService;
        private IContactRepository contactRepo;
        private IOrganizationRepository orgaRepo;
        private readonly IHistoryService _historyService;
        private readonly IOptions<AppSettings> _configuration;
        private readonly ILogger _logger;

        public EventService(CrmContext context,
            IEventContactRepository eventContactRepo,
            IEventOrganizationRepository eventOrgaRepo,
            IContactRepository contactRepo,
            ILoggerFactory logger,
            IMailService mailService,
            IOrganizationRepository orgaRepo, IHistoryService historyService, IOptions<AppSettings> configuration) : base(context, eventContactRepo, eventOrgaRepo, contactRepo, orgaRepo)
        {
            this.mailService = mailService;
            this.contactRepo = contactRepo;
            this.orgaRepo = orgaRepo;
            _historyService = historyService;
            _configuration = configuration;
            _logger = logger.CreateLogger(nameof(EventService));
        }

        public async Task<bool> SendInvitationMailsAsync(long eventId, List<long> contacts, List<long> orgaIds,
            string mailContent)
        {
            bool ok = true;
            //get invitation code
            var appSettings = _configuration.Value;
            foreach (long contactId in contacts)
            {
                Contact contact = await contactRepo.GetByIdAsync(contactId);
                if (contact != null)
                {
                    mailService.CreateAndSendInvitationMail(contact.ContactPossibilities.Mail,
                        contact.PreName,
                        contact.Name,
                        mailContent,
                        contact.Gender,
                        appSettings.InvitationCode,
                        eventId,
                        contactId);
                    await _historyService.CreateAsync(new HistoryElement()
                    {
                        contact = contact,
                        Date = DateTime.UtcNow,
                        Name = "invitation response",
                        Type = HistoryElementType.VISIT,
                        State = HistoryState.INVITE,
                        EventId = eventId
                    });
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
                    mailService.CreateAndSendInvitationMail(orga.Contact.Mail,
                        orga.Name,
                        mailContent,
                        appSettings.InvitationCode,
                        eventId,
                        orgaId);
                    await _historyService.CreateAsync(new HistoryElement()
                    {
                        organization = orga,
                        Date = DateTime.UtcNow,
                        Name = "invitation response",
                        Type = HistoryElementType.VISIT,
                        State = HistoryState.INVITE,
                        EventId = eventId
                    });
                }
                else
                {
                    ok = false;
                }
            }
            return ok;
        }

        public async Task HandleInvitationResponseForOrganizationAsync(long id, long organizationId, bool participate, string passCode)
        {
            if (!passCode.Equals(_configuration.Value.InvitationCode))
            {
                _logger.LogWarning("wrong validation code", passCode);
                throw new InvalidOperationException("invalid operation");
            }

            Event eve = await GetByIdAsync(id);
            var organization = await orgaRepo.GetByIdAsync(organizationId);

            if (eve != null && organization != null)
            {
                var historyState = participate ? HistoryState.ACCEPT : HistoryState.CANCEL;

                HistoryElement history = await _historyService.GetHistoryByEventAsync(eve.Id);
                if (history != null)
                {
                    history.State = historyState;
                    history.Date = DateTime.UtcNow;
                    await _historyService.UpdateAsync(history);
                }
                else
                {
                    await _historyService.CreateAsync(new HistoryElement()
                    {
                        organization = organization,
                        Name = "invitation response",
                        Date = DateTime.UtcNow,
                        Type = HistoryElementType.VISIT,
                        State = historyState,
                        EventId = eve.Id
                    });
                }
            }
            else if (organization == null)
            {
                throw new KeyNotFoundException("organization not found");
            }
            else
            {
                throw new KeyNotFoundException("event not found");
            }
        }

        public async Task HandleInvitationResponseForContactAsync(long id, long contactId, bool participate, string passCode)
        {
            if (!passCode.Equals(_configuration.Value.InvitationCode))
            {
                _logger.LogWarning("wrong validation code", passCode);
                throw new InvalidOperationException("invalid operation");
            }

            Event eve = await GetByIdAsync(id);
            var eventContact = await contactRepo.GetByIdAsync(contactId);

            if (eve != null && eventContact != null)
            {
                var historyState = participate ? HistoryState.ACCEPT : HistoryState.CANCEL;

                HistoryElement history = await _historyService.GetHistoryByEventAsync(eve.Id);
                if (history != null)
                {
                    history.State = historyState;
                    history.Date = DateTime.UtcNow;
                    await _historyService.UpdateAsync(history);
                }
                else
                {
                    await _historyService.CreateAsync(new HistoryElement()
                    {
                        contact = eventContact,
                        Date = DateTime.UtcNow,
                        Name = "invitation response",
                        Type = HistoryElementType.VISIT,
                        State = historyState,
                        EventId = eve.Id
                    });
                }
            }
            else if (eventContact == null)
            {
                throw new KeyNotFoundException("contact not found");
            }
            else
            {
                throw new KeyNotFoundException("event not found");
            }
        }
    }
}
