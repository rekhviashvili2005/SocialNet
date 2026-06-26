using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Application.DTOs;

public class CreatePostDto
{
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl {  get; set; }

    public List<string> ImageUrls { get; set; } = new(); // ახალი

    public List<string> Hashtags { get; set; } = new(); // ["Football", "Sport"]
}
