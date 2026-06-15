using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Application.DTOs;

public class PostDto
{

    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt {get; set; }


    public int LikesCount { get; set; }
}
