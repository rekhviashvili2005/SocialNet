using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Application.DTOs;

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DisplayName {  get; set; } = string.Empty;
    public string? Bio {  get; set; }
    public string? AvatarUrl { get; set; }
    public int PostsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class  UpdateProfileDto
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; } 
}
