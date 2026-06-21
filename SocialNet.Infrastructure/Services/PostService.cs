//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using SocialNet.Application.DTOs;
//using SocialNet.Application.Interfaces;
//using SocialNet.Domain.Entities;
//using SocialNet.Infrastructure.Identity;
//using SocialNet.Infrastructure.Persistence;

//namespace SocialNet.Infrastructure.Services;

//public class PostService : IPostService
//{
//    private readonly ApplicationDbContext _context;
//    private readonly UserManager<ApplicationUser> _userManager;

//    public PostService(
//        ApplicationDbContext context,
//        UserManager<ApplicationUser> userManager)
//    {
//        _context = context;
//        _userManager = userManager;
//    }

//    public async Task<List<PostDto>> GetAllPostsAsync(string? userId = null)
//    {
//        var posts = await _context.Posts
//            .Include(p => p.Hashtags)
//            .OrderByDescending(p => p.CreatedAt)
//            .ToListAsync();

//        var result = new List<PostDto>();
//        foreach (var post in posts)
//        {
//            var user = await _userManager.FindByIdAsync(post.UserId);
//            var likesCount = await _context.Likes.CountAsync(l => l.PostId == post.Id);
//            var isLiked = userId != null && await _context.Likes
//                .AnyAsync(l => l.PostId == post.Id && l.UserId == userId);

//            result.Add(new PostDto
//            {
//                Id = post.Id,
//                Content = post.Content,
//                ImageUrl = post.ImageUrl,
//                UserName = user?.UserName ?? "",
//                CreatedAt = post.CreatedAt,
//                LikesCount = likesCount,
//                Hashtags = post.Hashtags.Select(h => h.Tag).ToList(),
//                IsLikedByCurrentUser = isLiked
//            });
//        }
//        return result;
//    }



//    public async Task<PostDto?> GetPostByIdAsync(Guid id)
//    {
//        var post = await _context.Posts
//            .Include(p => p.Hashtags)
//            .FirstOrDefaultAsync(p => p.Id == id);

//        if (post == null) return null;

//        var user = await _userManager.FindByIdAsync(post.UserId);
//        return new PostDto
//        {
//            Id = post.Id,
//            Content = post.Content,
//            ImageUrl = post.ImageUrl,
//            UserName = user?.UserName ?? "",
//            CreatedAt = post.CreatedAt,
//            Hashtags = post.Hashtags.Select(h => h.Tag).ToList()
//        };
//    }

//    public async Task<PostDto> CreatePostAsync(CreatePostDto dto, string userId)
//    {
//        var post = new Post
//        {
//            Content = dto.Content,
//            ImageUrl = dto.ImageUrl,
//            UserId = userId,
//            CreatedAt = DateTime.UtcNow,
//            Hashtags = dto.Hashtags
//                .Select(tag => new PostHashtag { Tag = tag })
//                .ToList()
//        };

//        _context.Posts.Add(post);
//        await _context.SaveChangesAsync();

//        var user = await _userManager.FindByIdAsync(userId);
//        return new PostDto
//        {
//            Id = post.Id,
//            Content = post.Content,
//            ImageUrl = post.ImageUrl,
//            UserName = user?.UserName ?? "",
//            CreatedAt = post.CreatedAt,
//            Hashtags = post.Hashtags.Select(h => h.Tag).ToList()
//        };
//    }

//    public async Task<bool> UpdatePostAsync(Guid id, CreatePostDto dto, string userId)
//    {
//        var post = await _context.Posts
//            .Include(p => p.Hashtags)
//            .FirstOrDefaultAsync(p => p.Id == id);

//        if (post == null || post.UserId != userId) return false;

//        post.Content = dto.Content;
//        post.ImageUrl = dto.ImageUrl;
//        post.UpdatedAt = DateTime.UtcNow;

//        post.Hashtags.Clear();
//        foreach (var tag in dto.Hashtags)
//            post.Hashtags.Add(new PostHashtag { Tag = tag });

//        await _context.SaveChangesAsync();
//        return true;
//    }

//    public async Task<bool> DeletePostAsync(Guid id, string userId)
//    {
//        var post = await _context.Posts.FindAsync(id);
//        if (post == null || post.UserId != userId) return false;

//        _context.Posts.Remove(post);
//        await _context.SaveChangesAsync();
//        return true;
//    }

//    //public async Task<List<PostDto>> GetPostsByHashtagAsync(string tag)
//    //{
//    //    var posts = await _context.Posts
//    //        .Include(p => p.Hashtags)
//    //        .Where(p => p.Hashtags.Any(h => h.Tag == tag))
//    //        .OrderByDescending(p => p.CreatedAt)
//    //        .ToListAsync();

//    //    var result = new List<PostDto>();
//    //    foreach (var post in posts)
//    //    {
//    //        var user = await _userManager.FindByIdAsync(post.UserId);
//    //        var likesCount = await _context.Likes.CountAsync(l => l.PostId == post.Id);
//    //        result.Add(new PostDto
//    //        {
//    //            Id = post.Id,
//    //            Content = post.Content,
//    //            ImageUrl = post.ImageUrl,
//    //            UserName = user?.UserName ?? "",
//    //            CreatedAt = post.CreatedAt,
//    //            LikesCount = likesCount,
//    //            Hashtags = post.Hashtags.Select(h => h.Tag).ToList()
//    //        });
//    //    }
//    //    return result;
//    //}


//    public async Task<List<PostDto>> GetPostsByHashtagAsync(string tag, string? userId = null)
//    {
//        var posts = await _context.Posts
//            .Include(p => p.Hashtags)
//            .Where(p => p.Hashtags.Any(h => h.Tag == tag))
//            .OrderByDescending(p => p.CreatedAt)
//            .ToListAsync();

//        var result = new List<PostDto>();
//        foreach (var post in posts)
//        {
//            var user = await _userManager.FindByIdAsync(post.UserId);
//            var likesCount = await _context.Likes.CountAsync(l => l.PostId == post.Id);
//            var isLiked = userId != null && await _context.Likes
//                .AnyAsync(l => l.PostId == post.Id && l.UserId == userId);

//            result.Add(new PostDto
//            {
//                Id = post.Id,
//                Content = post.Content,
//                ImageUrl = post.ImageUrl,
//                UserName = user?.UserName ?? "",
//                CreatedAt = post.CreatedAt,
//                LikesCount = likesCount,
//                Hashtags = post.Hashtags.Select(h => h.Tag).ToList(),
//                IsLikedByCurrentUser = isLiked
//            });
//        }
//        return result;
//    }
//}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.DTOs;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;
using SocialNet.Infrastructure.Identity;
using SocialNet.Infrastructure.Persistence;

namespace SocialNet.Infrastructure.Services;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PostService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<PostDto>> GetAllPostsAsync(string? userId = null)
    {
        var posts = await _context.Posts
            .Include(p => p.Hashtags)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        // ყველა userId ერთ request-ში
        var userIds = posts.Select(p => p.UserId).Distinct().ToList();
        var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();
        var userDict = users.ToDictionary(u => u.Id);

        // ყველა like ერთ request-ში
        var postIds = posts.Select(p => p.Id).ToList();
        var likes = await _context.Likes
            .Where(l => postIds.Contains(l.PostId))
            .ToListAsync();

        var likesCountDict = likes
            .GroupBy(l => l.PostId)
            .ToDictionary(g => g.Key, g => g.Count());

        var likedPostIds = userId != null
            ? likes.Where(l => l.UserId == userId).Select(l => l.PostId).ToHashSet()
            : new HashSet<Guid>();

        return posts.Select(post => new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            UserName = userDict.TryGetValue(post.UserId, out var u) ? u.UserName ?? "" : "",
            CreatedAt = post.CreatedAt,
            LikesCount = likesCountDict.TryGetValue(post.Id, out var count) ? count : 0,
            Hashtags = post.Hashtags.Select(h => h.Tag).ToList(),
            IsLikedByCurrentUser = likedPostIds.Contains(post.Id)
        }).ToList();
    }

    public async Task<PostDto?> GetPostByIdAsync(Guid id)
    {
        var post = await _context.Posts
            .Include(p => p.Hashtags)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return null;

        var user = await _userManager.FindByIdAsync(post.UserId);
        return new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            UserName = user?.UserName ?? "",
            CreatedAt = post.CreatedAt,
            Hashtags = post.Hashtags.Select(h => h.Tag).ToList()
        };
    }

    public async Task<PostDto> CreatePostAsync(CreatePostDto dto, string userId)
    {
        var post = new Post
        {
            Content = dto.Content,
            ImageUrl = dto.ImageUrl,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Hashtags = dto.Hashtags
                .Select(tag => new PostHashtag { Tag = tag })
                .ToList()
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(userId);
        return new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            UserName = user?.UserName ?? "",
            CreatedAt = post.CreatedAt,
            Hashtags = post.Hashtags.Select(h => h.Tag).ToList()
        };
    }

    public async Task<bool> UpdatePostAsync(Guid id, CreatePostDto dto, string userId)
    {
        var post = await _context.Posts
            .Include(p => p.Hashtags)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null || post.UserId != userId) return false;

        post.Content = dto.Content;
        post.ImageUrl = dto.ImageUrl;
        post.UpdatedAt = DateTime.UtcNow;

        post.Hashtags.Clear();
        foreach (var tag in dto.Hashtags)
            post.Hashtags.Add(new PostHashtag { Tag = tag });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePostAsync(Guid id, string userId)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null || post.UserId != userId) return false;

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<PostDto>> GetPostsByHashtagAsync(string tag, string? userId = null)
    {
        var posts = await _context.Posts
            .Include(p => p.Hashtags)
            .Where(p => p.Hashtags.Any(h => h.Tag == tag))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var userIds = posts.Select(p => p.UserId).Distinct().ToList();
        var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();
        var userDict = users.ToDictionary(u => u.Id);

        var postIds = posts.Select(p => p.Id).ToList();
        var likes = await _context.Likes
            .Where(l => postIds.Contains(l.PostId))
            .ToListAsync();

        var likesCountDict = likes
            .GroupBy(l => l.PostId)
            .ToDictionary(g => g.Key, g => g.Count());

        var likedPostIds = userId != null
            ? likes.Where(l => l.UserId == userId).Select(l => l.PostId).ToHashSet()
            : new HashSet<Guid>();

        return posts.Select(post => new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            UserName = userDict.TryGetValue(post.UserId, out var u) ? u.UserName ?? "" : "",
            CreatedAt = post.CreatedAt,
            LikesCount = likesCountDict.TryGetValue(post.Id, out var count) ? count : 0,
            Hashtags = post.Hashtags.Select(h => h.Tag).ToList(),
            IsLikedByCurrentUser = likedPostIds.Contains(post.Id)
        }).ToList();
    }
}