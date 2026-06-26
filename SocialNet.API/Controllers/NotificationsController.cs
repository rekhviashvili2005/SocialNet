using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.Interfaces;
using System.Security.Claims;

namespace SocialNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = User.FindFirstValue("uid");
        if (userId == null) return Unauthorized();
        var notifications = await _notificationService.GetUserNotificationsAsync(userId);
        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = User.FindFirstValue("uid");
        if (userId == null) return Unauthorized();
        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(count);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = User.FindFirstValue("uid");
        if (userId == null) return Unauthorized();
        await _notificationService.MarkAsReadAsync(id, userId);
        return Ok();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.FindFirstValue("uid");
        if (userId == null) return Unauthorized();
        await _notificationService.MarkAllAsReadAsync(userId);
        return Ok();
    }
}