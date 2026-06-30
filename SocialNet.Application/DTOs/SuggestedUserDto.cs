namespace SocialNet.Application.DTOs;

public class SuggestedUserDto
{
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int PostsLastWeek { get; set; }
}