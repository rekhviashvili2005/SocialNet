using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Domain.Entities
{
    public class Post : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ICollection<PostHashtag> Hashtags { get; set; } = new List<PostHashtag>();
        public ICollection<PostImage> Images { get; set; } = new List<PostImage>(); // ახალი


    }
}
