
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SocialNet.Application.DTOs;
using SocialNet.Application.Interfaces;
using SocialNet.Infrastructure.Identity;
using SocialNet.Infrastructure.Persistence;

namespace SocialNet.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public UserService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IMemoryCache cache)
    {
        _userManager = userManager;
        _context = context;
        _cache = cache;
    }

    public async Task<UserProfileDto?> GetProfileAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return null;

        var postsCount = await _context.Posts.CountAsync(p => p.UserId == user.Id);

        return new UserProfileDto
        {
            Id = user.Id,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            PostsCount = postsCount,
            CreatedAt = user.CreatedAt,
        };
    }

    public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.DisplayName = dto.DisplayName;
        user.Bio = dto.Bio;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<List<PostDto>> GetUserPostsAsync(string username, string? currentUserId = null, int page = 1, int pageSize = 10)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return new List<PostDto>();

        var posts = await _context.Posts
            .Include(p => p.Hashtags)
            .Include(p => p.Images)
            .Where(p => p.UserId == user.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var postIds = posts.Select(p => p.Id).ToList();
        var likes = await _context.Likes
            .Where(l => postIds.Contains(l.PostId))
            .ToListAsync();

        var likesCountDict = likes
            .GroupBy(l => l.PostId)
            .ToDictionary(g => g.Key, g => g.Count());

        var likedPostIds = currentUserId != null
            ? likes.Where(l => l.UserId == currentUserId).Select(l => l.PostId).ToHashSet()
            : new HashSet<Guid>();

        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            Content = p.Content,
            ImageUrl = p.ImageUrl,
            ImageUrls = p.Images.OrderBy(i => i.Order).Select(i => i.ImageUrl).ToList(),
            UserName = user.UserName ?? "",
            CreatedAt = p.CreatedAt,
            LikesCount = likesCountDict.TryGetValue(p.Id, out var count) ? count : 0,
            Hashtags = p.Hashtags.Select(h => h.Tag).ToList(),
            IsLikedByCurrentUser = likedPostIds.Contains(p.Id)
        }).ToList();
    }

    public async Task<List<UserSearchDto>> SearchUsersAsync(string query)
    {
        var users = await _userManager.Users
            .Where(u => u.UserName!.Contains(query) || u.DisplayName!.Contains(query))
            .Take(10)
            .ToListAsync();

        return users.Select(u => new UserSearchDto
        {
            UserName = u.UserName ?? "",
            DisplayName = u.DisplayName ?? ""
        }).ToList();
    }

    public async Task<List<SuggestedUserDto>> GetSuggestedUsersAsync(string? currentUserId, int count = 5)
    {
        var cacheKey = $"suggested_{currentUserId ?? "anon"}";

        if (_cache.TryGetValue(cacheKey, out List<SuggestedUserDto>? cached) && cached != null)
            return cached;

        var cutoff = DateTime.UtcNow.AddDays(-7);

        var alreadyFollowing = currentUserId != null
            ? await _context.Follows
                .Where(f => f.FollowerId == currentUserId)
                .Select(f => f.FollowingId)
                .ToListAsync()
            : new List<string>();

        var topUserIds = await _context.Posts
            .Where(p => p.CreatedAt >= cutoff
                     && p.UserId != currentUserId
                     && !alreadyFollowing.Contains(p.UserId))
            .GroupBy(p => p.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(count)
            .ToListAsync();

        var userIds = topUserIds.Select(x => x.UserId).ToList();

        var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.UserName, u.DisplayName, u.AvatarUrl })
            .ToListAsync();

        var result = users.Select(u => new SuggestedUserDto
        {
            UserName = u.UserName ?? "",
            DisplayName = u.DisplayName,
            AvatarUrl = u.AvatarUrl,
            PostsLastWeek = topUserIds.First(x => x.UserId == u.Id).Count
        })
        .OrderByDescending(u => u.PostsLastWeek)
        .ToList();

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }
}
