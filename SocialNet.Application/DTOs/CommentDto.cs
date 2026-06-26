using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Application.DTOs;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public DateTime CreatedAt { get; set; }

    //public string? ImageUrl { get; set; } // commentshi suratebistvis
    public List<string> ImageUrls { get; set; } = new(); //ramdenime suratistvis
}

public class CreateCommentDto
{
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }

    //public string? ImageUrl { get; set; }// commentshi suratebistvis
    public List<string> ImageUrls { get; set; } = new(); //ramdenime suratistvis
}
