using Microsoft.EntityFrameworkCore;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;
using SocialNet.Infrastructure.Persistence;

namespace SocialNet.Infrastructure.Services;

public class LikeService : ILikeService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public LikeService(
        ApplicationDbContext context,
        INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<bool> ToggleLikeAsync(Guid postId, string userId)
    {
        var existingLike = await _context.Likes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

        if (existingLike != null)
        {
            _context.Likes.Remove(existingLike);
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { }
            return true;
        }

        var like = new Like
        {
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
        };
        _context.Likes.Add(like);
        try
        {
            await _context.SaveChangesAsync();
            await _notificationService.CreateLikeNotificationAsync(postId, userId); // ახალი
        }
        catch (DbUpdateException) { }

        return true;
    }

    public async Task<int> GetLikesCountAsync(Guid postId)
    {
        return await _context.Likes.CountAsync(l => l.PostId == postId);
    }

    public async Task<bool> IsLikedByUserAsync(Guid postId, string userId)
    {
        return await _context.Likes.AnyAsync(l => l.PostId == postId && l.UserId == userId);
    }
}