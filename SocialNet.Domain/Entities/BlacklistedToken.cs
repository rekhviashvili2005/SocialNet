namespace SocialNet.Domain.Entities;

public class BlacklistedToken
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public DateTime BlacklistedAt { get; set; }
}