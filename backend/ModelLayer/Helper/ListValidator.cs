using ModelLayer.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;

namespace ModelLayer.Helper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ListValidator : ValidationAttribute
    {
        private NumberValidator numberValidator = new NumberValidator();
        private MailValidator mailValidator = new MailValidator();

        public override bool IsValid(object value)
        {
            if (value is List<ContactPossibilitiesEntryCreateDto> list)
            {
                if (list.Count == 0)
                {
                    return true;
                }

                foreach (ContactPossibilitiesEntryCreateDto dto in list)
                {
                    long valueLong = 0;
                    if (long.TryParse(dto.ContactEntryValue.Replace("-", "").Replace(" ", ""), out valueLong)) {
                        if (!numberValidator.IsValid(dto.ContactEntryValue))
                        {
                            return false;
                        }
                    } 
                    else
                    {
                        if (!mailValidator.IsValid(dto.ContactEntryValue))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }
    }
}
