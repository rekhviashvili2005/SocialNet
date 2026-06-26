using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.DTOs;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;
using SocialNet.Infrastructure.Identity;
using SocialNet.Infrastructure.Persistence;

namespace SocialNet.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    //public async Task CreateLikeNotificationAsync(Guid postId, string actorId)
    //{
    //    var post = await _context.Posts.FindAsync(postId);
    //    if (post == null || post.UserId == actorId) return;

    //    var actor = await _userManager.FindByIdAsync(actorId);

    //    var notification = new Notification
    //    {
    //        UserId = post.UserId,
    //        ActorId = actorId,
    //        Type = "like",
    //        PostId = postId,
    //        Message = $"{actor?.UserName} liked your post",
    //        CreatedAt = DateTime.UtcNow
    //    };

    //    _context.Notifications.Add(notification);
    //    await _context.SaveChangesAsync();
    //}

    public async Task CreateLikeNotificationAsync(Guid postId, string actorId)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post == null || post.UserId == actorId) return;

        var actor = await _userManager.FindByIdAsync(actorId);

        var postPreview = !string.IsNullOrEmpty(post.Content)
            ? (post.Content.Length > 30 ? post.Content.Substring(0, 30) + "..." : post.Content)
            : "your post";

        var notification = new Notification
        {
            UserId = post.UserId,
            ActorId = actorId,
            Type = "like",
            PostId = postId,
            Message = $"{actor?.UserName} liked \"{postPreview}\"",
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    //public async Task CreateCommentNotificationAsync(Guid postId, string actorId, string commentContent)
    //{
    //    var post = await _context.Posts.FindAsync(postId);
    //    if (post == null || post.UserId == actorId) return;

    //    var actor = await _userManager.FindByIdAsync(actorId);

    //    var preview = commentContent.Length > 30
    //        ? commentContent.Substring(0, 30) + "..."
    //        : commentContent;

    //    var notification = new Notification
    //    {
    //        UserId = post.UserId,
    //        ActorId = actorId,
    //        Type = "comment",
    //        PostId = postId,
    //        Message = $"{actor?.UserName} commented: \"{preview}\"",
    //        CreatedAt = DateTime.UtcNow
    //    };

    //    _context.Notifications.Add(notification);
    //    await _context.SaveChangesAsync();
    //}

    public async Task CreateCommentNotificationAsync(Guid postId, string actorId, string commentContent)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post == null || post.UserId == actorId) return;

        var actor = await _userManager.FindByIdAsync(actorId);

        var postPreview = !string.IsNullOrEmpty(post.Content)
            ? (post.Content.Length > 30 ? post.Content.Substring(0, 30) + "..." : post.Content)
            : "your post";

        var commentPreview = commentContent.Length > 30
            ? commentContent.Substring(0, 30) + "..."
            : commentContent;

        var notification = new Notification
        {
            UserId = post.UserId,
            ActorId = actorId,
            Type = "comment",
            PostId = postId,
            Message = $"{actor?.UserName} commented on \"{postPreview}\": \"{commentPreview}\"",
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
    public async Task<List<NotificationDto>> GetUserNotificationsAsync(string userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Type = n.Type,
            ActorUserName = "",
            Message = n.Message,
            PostId = n.PostId,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        }).ToList();
    }

    public async Task MarkAsReadAsync(Guid notificationId, string userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
        if (notification == null) return;

        notification.IsRead = true;
        await _context.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var n in notifications)
            n.IsRead = true;

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }
}