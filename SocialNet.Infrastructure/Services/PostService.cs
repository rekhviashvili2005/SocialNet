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

    public async Task<List<PostDto>> GetAllPostsAsync()
    {
        var posts = await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var result = new List<PostDto>();

        foreach (var post in posts)
        {
            var user = await _userManager.FindByIdAsync(post.UserId);
            var likesCount = await _context.Likes
                .CountAsync(l => l.PostId == post.Id);
            result.Add(new PostDto
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                UserName = user?.UserName ?? "",
                CreatedAt = post.CreatedAt,
                LikesCount = likesCount
            });
        }

        return result;
    }

    public async Task<PostDto?> GetPostByIdAsync(Guid id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null) return null;

        var user = await _userManager.FindByIdAsync(post.UserId);

        return new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            UserName = user?.UserName ?? "",
            CreatedAt = post.CreatedAt
        };
    }

    public async Task<PostDto> CreatePostAsync(CreatePostDto dto, string userId)
    {
        var post = new Post
        {
            Content = dto.Content,
            ImageUrl = dto.ImageUrl,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
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
            CreatedAt = post.CreatedAt
        };
    }

    public async Task<bool> UpdatePostAsync(Guid id, CreatePostDto dto, string userId)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null || post.UserId != userId) return false;

        post.Content = dto.Content;
        post.ImageUrl = dto.ImageUrl;
        post.UpdatedAt = DateTime.UtcNow;

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
}