using Microsoft.AspNetCore.Identity;
using ModelLayer.Helper;

namespace ModelLayer.Models
{
    public class Role : IdentityRole<long>
    {
        UserRight userRight;
    }
}