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
}
