using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Infrastructure.Identity
{
     public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;

        public string? Bio {get; set;}

        public string?  AvatarUrl { get; set;}
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
