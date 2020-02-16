using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace ModelLayer.Models
{
    public class User : IdentityUser<long>
    {
    }
}