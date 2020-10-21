using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

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
            Regex regex = new Regex("^0[0-9- ]*$");
            if (value is string valueString)
            {
                return regex.IsMatch(valueString);
            }
            else
            {
                return false;
            }
        }
    }
}
