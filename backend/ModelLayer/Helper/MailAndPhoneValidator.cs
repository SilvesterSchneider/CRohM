using ModelLayer.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;

namespace ModelLayer.Helper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class MailAndPhoneValidator : ValidationAttribute
    {
        private NumberValidator numberValidator = new NumberValidator();
        private MailValidator mailValidator = new MailValidator();

        public override bool IsValid(object value)
        {
            if (value is string val)
            {
                if (val.Length == 0)
                {
                    return false;
                }

                long valueLong = 0;
                if (long.TryParse(val.Replace("-", "").Replace(" ", ""), out valueLong)) {
                    if (!numberValidator.IsValid(val))
                    {
                        return false;
                    }
                } 
                else
                {
                    if (!mailValidator.IsValid(val))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
