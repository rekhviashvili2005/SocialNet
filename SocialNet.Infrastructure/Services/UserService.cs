//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using SocialNet.Application.DTOs;
//using SocialNet.Application.Interfaces;
//using SocialNet.Infrastructure.Identity;
//using SocialNet.Infrastructure.Persistence;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace SocialNet.Infrastructure.Services;

//public class UserService : IUserService
//{
//    private readonly UserManager<ApplicationUser> _userManager;
//    private readonly ApplicationDbContext _context;


//    public UserService(
//        UserManager<ApplicationUser> userManager,
//        ApplicationDbContext context)
//    {
//        _userManager = userManager;
//        _context = context;
//    }



//    public async Task<UserProfileDto?> GetProfileAsync(string username)
//    {
//        var user = await _userManager.FindByNameAsync(username);
//        if (user == null) return null;

//        var postsCount = await _context.Posts
//            .CountAsync(p => p.UserId == user.Id);

//        return new UserProfileDto
//        {
//            Id = user.Id,
//            UserName = user.UserName,
//            DisplayName = user.DisplayName,
//            Bio = user.Bio,
//            AvatarUrl = user.AvatarUrl,
//            PostsCount = postsCount,
//            CreatedAt = user.CreatedAt,
//        };
//    }


//    public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto)
//    {
//        var user = await _userManager.FindByIdAsync(userId);
//        if(user == null) return false;

//        user.DisplayName = dto.DisplayName;
//        user.Bio = dto.Bio;

//        var result = await _userManager.UpdateAsync(user);
//        return result.Succeeded;
//    }


//    public async Task<List<PostDto>> GetUserPostsAsync(string username, string? currentUserId = null)
//    {
//        var user = await _userManager.FindByNameAsync(username);
//        if (user == null) return new List<PostDto>();

//        var posts = await _context.Posts
//            .Include(p => p.Hashtags)
//            .Where(p => p.UserId == user.Id)
//            .OrderByDescending(p => p.CreatedAt)
//            .ToListAsync();

//        var result = new List<PostDto>();
//        foreach (var p in posts)
//        {
//            var likesCount = await _context.Likes.CountAsync(l => l.PostId == p.Id);
//            var isLiked = currentUserId != null && await _context.Likes
//                .AnyAsync(l => l.PostId == p.Id && l.UserId == currentUserId);

//            result.Add(new PostDto
//            {
//                Id = p.Id,
//                Content = p.Content,
//                ImageUrl = p.ImageUrl,
//                UserName = user.UserName ?? "",
//                CreatedAt = p.CreatedAt,
//                LikesCount = likesCount,
//                Hashtags = p.Hashtags.Select(h => h.Tag).ToList(),
//                IsLikedByCurrentUser = isLiked
//            });
//        }
//        return result;
//    }


//    public async Task<List<UserSearchDto>> SearchUsersAsync(string query)
//    {
//        var users = await _userManager.Users
//            .Where(u => u.UserName!.Contains(query) || u.DisplayName!.Contains(query))
//            .Take(10)
//            .ToListAsync();

//        return users.Select(u => new UserSearchDto
//        {
//            UserName = u.UserName ?? "",
//            DisplayName = u.DisplayName ?? ""
//        }).ToList();
//    }

//}


using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.DTOs;
using SocialNet.Application.Interfaces;
using SocialNet.Infrastructure.Identity;
using SocialNet.Infrastructure.Persistence;

namespace SocialNet.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UserService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
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

    public async Task<List<PostDto>> GetUserPostsAsync(string username, string? currentUserId = null)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return new List<PostDto>();

        var posts = await _context.Posts
            .Include(p => p.Hashtags)
            .Where(p => p.UserId == user.Id)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        // ყველა like ერთ request-ში
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
}