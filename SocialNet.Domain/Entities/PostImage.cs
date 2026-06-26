namespace SocialNet.Domain.Entities;

public class PostImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
    public string ImageUrl { get; set; } = string.Empty;
    public int Order { get; set; }
}