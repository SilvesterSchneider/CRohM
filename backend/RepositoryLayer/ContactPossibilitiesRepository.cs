using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RepositoryLayer
{
    public interface IContactPossibilitiesRepository : IBaseRepository<ContactPossibilities>
    {
        bool IsMailFormatValid(long id);

        bool IsTelephoneformatValid(long id);

        bool IsFaxFormatValid(long id);

        bool AreAllFormatsValid(long id);
    }

    public class ContactPossibilitiesRepository : BaseRepository<ContactPossibilities>, IContactPossibilitiesRepository
    {
        public ContactPossibilitiesRepository(CrmContext context) : base(context)
        {
        }

        public bool AreAllFormatsValid(long id)
        {
            ContactPossibilities contact = Entities.Where(contact => contact.Id == id).ToList<ContactPossibilities>()[0];
            if (contact != null)
            {
                return IsMailFormatValid(id) && IsNumberFormatCorrect(contact.Fax) && IsNumberFormatCorrect(contact.PhoneNumber);
            }
            else
            {
                return false;
            }
        }

        public bool IsFaxFormatValid(long id)
        {
            ContactPossibilities contact = Entities.Where(contact => contact.Id == id).ToList<ContactPossibilities>()[0];
            if (contact != null)
            {
                return IsNumberFormatCorrect(contact.Fax);
            }
            else
            {
                return false;
            }
        }

        public bool IsMailFormatValid(long id)
        {
            ContactPossibilities contact = Entities.Where(contact => contact.Id == id).ToList<ContactPossibilities>()[0];
            if (contact != null)
            {
                return contact.Mail.Contains("@") && contact.Mail.LastIndexOf(".") > contact.Mail.LastIndexOf("@");
            }
            else
            {
                return false;
            }
        }

        public bool IsTelephoneformatValid(long id)
        {
            ContactPossibilities contact = Entities.Where(contact => contact.Id == id).ToList<ContactPossibilities>()[0];
            if (contact != null)
            {
                return IsNumberFormatCorrect(contact.PhoneNumber);
            }
            else
            {
                return false;
            }
        }

        private bool IsNumberFormatCorrect(string valueTocheck)
        {
            if (valueTocheck.StartsWith("-"))
            {
                return false;
            }
            long value;
            string valueFormatted = valueTocheck.Replace("-", "");
            return long.TryParse(valueFormatted, out value);   
        }
    }
}
