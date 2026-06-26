namespace SocialNet.Domain.Entities;

public class Notification : BaseEntity
{
    public string UserId { get; set; } = string.Empty; // ვისთვის
    public string ActorId { get; set; } = string.Empty; // ვინ გააკეთა
    public string Type { get; set; } = string.Empty; // "like" ან "comment"
    public Guid PostId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
}