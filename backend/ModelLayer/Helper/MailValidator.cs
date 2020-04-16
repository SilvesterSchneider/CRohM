using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelLayer.Helper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class MailValidator : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string text && text.Length == 0)
            {
                return true;
            }
            if (value is string valueString)
            {
                return valueString.Contains("@") && valueString.LastIndexOf(".") > valueString.LastIndexOf("@");
            }
            else
            {
                return false;
            }
        }
    }
}
