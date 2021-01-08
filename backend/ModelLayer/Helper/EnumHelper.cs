using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Helper
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class EnumHelper
    {
        public static string GetEnumMemberValue(Enum enumItem)
        {
            var memInfo = enumItem.GetType().GetMember(enumItem.ToString());
            var attr = memInfo[0].GetCustomAttributes(false);
            return attr == null || attr.Length == 0 ? null : ((System.Runtime.Serialization.EnumMemberAttribute)attr[0]).Value;
        }
    }
}
