using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName {  get; set; } = string.Empty;
        public string? bio {  get; set; }
        public string? AvatarUrl { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();


        public  ICollection<Follow> Followers { get; set; }
               = new List<Follow>();

        public ICollection<Follow> Following { get; set; } = 
                new List<Follow>();
    }

}
