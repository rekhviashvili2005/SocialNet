using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.DTOs;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;
using SocialNet.Infrastructure.Identity;
using SocialNet.Infrastructure.Persistence;

namespace SocialNet.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _useManager;
    private readonly INotificationService _notificationService;

    public CommentService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> useManager,
        INotificationService notificationService)
    {
        _context = context;
        _useManager = useManager;
        _notificationService = notificationService;
    }

    public async Task<List<CommentDto>> GetPostCommentsAsync(Guid postId)
    {
        var comments = await _context.Comments
            .Include(c => c.Images)
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var result = new List<CommentDto>();
        foreach (var comment in comments)
        {
            var user = await _useManager.FindByIdAsync(comment.UserId);
            result.Add(new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                UserId = comment.UserId,
                UserName = user?.UserName ?? user?.Email ?? comment.UserId,
                PostId = comment.PostId,
                CreatedAt = comment.CreatedAt,
                ImageUrls = comment.Images.OrderBy(i => i.Order).Select(i => i.ImageUrl).ToList()
            });
        }
        return result;
    }

    public async Task<CommentDto> AddCommentAsync(CreateCommentDto dto, string userId)
    {
        var comment = new Comment
        {
            Content = dto.Content,
            PostId = dto.PostId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Images = dto.ImageUrls
                .Select((url, index) => new CommentImage { ImageUrl = url, Order = index })
                .ToList()
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        await _notificationService.CreateCommentNotificationAsync(dto.PostId, userId, dto.Content);

        var user = await _useManager.FindByIdAsync(userId);
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            UserName = user?.UserName ?? user?.Email ?? userId,
            PostId = comment.PostId,
            CreatedAt = comment.CreatedAt,
            ImageUrls = comment.Images.OrderBy(i => i.Order).Select(i => i.ImageUrl).ToList()
        };
    }

    public async Task<bool> DeleteCommentAsync(Guid id, string userId)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null || comment.UserId != userId) return false;

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return true;
    }
}