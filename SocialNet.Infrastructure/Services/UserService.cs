using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.DTOs;
using SocialNet.Application.Interfaces;
using SocialNet.Infrastructure.Identity;
using SocialNet.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

        var postsCount = await _context.Posts
            .CountAsync(p => p.UserId == user.Id);

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
        if(user == null) return false;

        user.DisplayName = dto.DisplayName;
        user.Bio = dto.Bio;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }



    public async Task<List<PostDto>> GetUserPostsAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return new List<PostDto>();

        var posts = await _context.Posts
            .Where(p => p.UserId == user.Id)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            Content = p.Content,
            ImageUrl = p.ImageUrl,
            UserName = user.UserName,
            CreatedAt = p.CreatedAt
        }).ToList();
    }

}
