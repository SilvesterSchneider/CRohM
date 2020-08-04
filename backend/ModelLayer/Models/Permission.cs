using Microsoft.AspNetCore.Identity;
using ModelLayer.Helper;
using ModelLayer.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    public class Permission : IdentityRole<long>
    {
        public UserRight UserRight { get; set; }
    }
}
