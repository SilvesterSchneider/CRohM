using ModelLayer.Helper;
using ModelLayer.Models.Base;

namespace ModelLayer
{
    public class Permission : BaseEntity
    {
        public bool IsEnabled { get; set; }

        public UserRight Grant { get; set; }
    } 
}
