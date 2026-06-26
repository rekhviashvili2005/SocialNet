namespace SocialNet.Application.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string ActorUserName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
