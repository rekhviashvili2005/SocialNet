namespace SocialNet.Domain.Entities;

public class CommentImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ImageUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid CommentId { get; set; }
    public Comment Comment { get; set; } = null!;
}