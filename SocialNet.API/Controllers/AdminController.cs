using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;
using SocialNet.Infrastructure.Identity;
using SocialNet.Infrastructure.Persistence;

namespace SocialNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPostService _postService;
    private readonly ApplicationDbContext _context;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        IPostService postService,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _postService = postService;
        _context = context;
    }

    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        var users = _userManager.Users.Select(u => new
        {
            u.Id,
            u.UserName,
            u.Email,
            u.DisplayName,
            u.Bio,
            IsBanned = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.Now
        }).ToList();
        return Ok(users);
    }

    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // Blacklist-ში დამატება
        var alreadyBlacklisted = await _context.BlacklistedTokens
            .AnyAsync(t => t.UserId == userId);
        if (!alreadyBlacklisted)
        {
            _context.BlacklistedTokens.Add(new BlacklistedToken
            {
                UserId = userId,
                BlacklistedAt = DateTime.UtcNow
            });
        }

        var follows = _context.Follows
            .Where(f => f.FollowerId == userId || f.FollowingId == userId);
        _context.Follows.RemoveRange(follows);

        var posts = _context.Posts.Where(p => p.UserId == userId);
        _context.Posts.RemoveRange(posts);

        await _context.SaveChangesAsync();

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }

    [HttpDelete("posts/{postId}")]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        var result = await _postService.AdminDeletePostAsync(postId);
        return result ? Ok() : NotFound();
    }

    [HttpPost("users/{userId}/ban")]
    public async Task<IActionResult> BanUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // Blacklist-ში დამატება
        var alreadyBlacklisted = await _context.BlacklistedTokens
            .AnyAsync(t => t.UserId == userId);
        if (!alreadyBlacklisted)
        {
            _context.BlacklistedTokens.Add(new BlacklistedToken
            {
                UserId = userId,
                BlacklistedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        await _userManager.SetLockoutEnabledAsync(user, true);
        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        return Ok();
    }

    [HttpPost("users/{userId}/unban")]
    public async Task<IActionResult> UnbanUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // Blacklist-იდან ამოღება
        var blacklisted = await _context.BlacklistedTokens
            .FirstOrDefaultAsync(t => t.UserId == userId);
        if (blacklisted != null)
            _context.BlacklistedTokens.Remove(blacklisted);

        await _context.SaveChangesAsync();
        await _userManager.SetLockoutEndDateAsync(user, null);
        return Ok();
    }



    [HttpGet("users/{userId}/posts")]
    public async Task<IActionResult> GetUserPosts(string userId)
    {
        var posts = await _context.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Content,
                p.CreatedAt
            })
            .ToListAsync();

        return Ok(posts);
    }
}