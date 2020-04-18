using ModelLayer;
using RepositoryLayer;
using System;
using System.Threading.Tasks;
using ModelLayer.Models;

namespace ServiceLayer
{
    public interface IOrganizationService : IOrganizationRepository
    {
        /// <summary>
        /// Add new contact to organization
        /// </summary>
        /// <param name="organizationId">The id of the organization</param>
        /// <param name="contactId">The id of the contact which will be added</param>
        /// <exception cref="Exception">Something not correct in workflow</exception>
        Task<Organization> AddContactAsync(long organizationId, long contactId);

        /// <summary>
        /// Remove a contact from a organization
        /// </summary>
        /// <param name="organizationId">The id of the organization</param>
        /// <param name="contactId">The id of the contact which will be removed</param>
        /// <exception cref="Exception">Something not correct in workflow</exception>
        Task RemoveContactAsync(long organizationId, long contactId);
    }

    public class OrganizationService : OrganizationRepository, IOrganizationService
    {
        private readonly IOrganizationContactRepository _organizationContactRepository;
        private readonly IContactRepository _contactRepository;

        public OrganizationService(CrmContext context, IOrganizationContactRepository organizationContactRepository, IContactRepository contactRepository) : base(context)
        {
            _organizationContactRepository = organizationContactRepository;
            _contactRepository = contactRepository;
        }

        public async Task<Organization> AddContactAsync(long organizationId, long contactId)
        {
            var entityExists = await _organizationContactRepository.CheckIfOrganizationContactExistsAsync(contactId, organizationId);
            if (entityExists)
            {
                throw new Exception("try to add a connection, while there is already one");
            }

            var organization = await GetByIdAsync(organizationId);
            var contact = await _contactRepository.GetByIdAsync(contactId);

            if (organization == null || contact == null)
            {
                throw new Exception("can not find entities with given ids");
            }

            var organizationContact = new OrganizationContact() { Organization = organization, Contact = contact };

            await _organizationContactRepository.CreateAsync(organizationContact);

            return organization;
        }

        public async Task RemoveContactAsync(long organizationId, long contactId)
        {
            OrganizationContact organizationContact = await _organizationContactRepository.GetOrganizationContactByIdsAsync(contactId, organizationId);
            if (organizationContact == null)
            {
                throw new Exception("try to remove a connection, while there is none");
            }

            await _organizationContactRepository.DeleteAsync(organizationContact);
        }
    }
}