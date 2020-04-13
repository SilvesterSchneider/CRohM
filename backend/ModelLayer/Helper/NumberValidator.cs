using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelLayer.Helper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class NumberValidator : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string text && text.Length == 0)
            {
                return true;
            }
            if (value is string valueString && !valueString.StartsWith("-"))
            {
                long valueLong;
                string valueFormatted = valueString.Replace("-", "");
                return long.TryParse(valueFormatted, out valueLong);
            }
            else
            {
                return false;
            }
        }
    }
}
