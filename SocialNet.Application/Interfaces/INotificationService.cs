using SocialNet.Application.DTOs;

namespace SocialNet.Application.Interfaces;

public interface INotificationService
{
    Task CreateLikeNotificationAsync(Guid postId, string actorId);
    //Task CreateCommentNotificationAsync(Guid postId, string actorId);
    Task CreateCommentNotificationAsync(Guid postId, string actorId, string commentContent);
    Task<List<NotificationDto>> GetUserNotificationsAsync(string userId);
    Task MarkAsReadAsync(Guid notificationId, string userId);
    Task MarkAllAsReadAsync(string userId);
    Task<int> GetUnreadCountAsync(string userId);
}