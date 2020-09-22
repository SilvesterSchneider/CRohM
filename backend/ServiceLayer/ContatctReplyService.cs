using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IContactReplyService : IContactRepository
    {
        Task<bool> ContactReply(long id);
    }

    public class ContactReplyService : ContactRepository, IContactReplyService
    {

        public ContactReplyService(CrmContext context) : base(context){}

        public async Task<bool> ContactReply(long id)
        {
            Contact contact = await GetByIdAsync(id);
            if (contact == null)
            {
                return false;
            }
            contact.approval = true;
            return await UpdateAsync(contact, id);
        }
    }
}
