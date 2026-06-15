using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;
using SocialNet.Infrastructure.Identity;
using SocialNet.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Infrastructure.Services;

public class FollowService :IFollowService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public FollowService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task FollowAsync(string followerId, string followingId)
    {
        var alreadyFollowing = await _context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);


        if (alreadyFollowing) return;

        var follow = new Follow
        {
            FollowerId = followerId,
            FollowingId = followingId
        };

        _context.Follows.Add(follow);
        await _context.SaveChangesAsync();
    }



    public async Task UnFollowAsync(string followerId, string followingId)
    {
        var follow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

        if (follow == null) return;

        _context.Follows.Remove(follow);
        await _context.SaveChangesAsync();
    }


    public async Task<bool> IsFollowingAsync(string followerId, string followingId)
    {
        return await _context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

    }


    public async Task<int> GetFollowersCountAsync(string userId)
    {
        return await _context.Follows
            .CountAsync(f => f.FollowingId == userId);
    }


    public async Task<int> GetFollowingCountAsync(string userId)
    {
        return await _context.Follows
            .CountAsync(f =>f.FollowerId == userId);
    }



    //
    public async Task<List<string>> GetFollowersAsync(string userId)
    {

        var followersIds = await _context.Follows
            .Where(f => f.FollowingId == userId)
            .Select(f => f.FollowerId)
            .ToListAsync();

        return await _userManager.Users
            .Where(u => followersIds.Contains(u.Id))
            .Select(u => u.UserName!)
            .ToListAsync();


        //return await _context.Follows
        //    .Where(f => f.FollowingId == userId)
        //    .Select(f => f.FollowerId)
        //    .ToListAsync();
    }

    public async Task<List<string>> GetFollowingAsync(string userId)
    {

        var followingIds = await _context.Follows
            .Where(f => f.FollowingId == userId)
            .Select( f => f.FollowerId)
            .ToListAsync();

        return await _userManager.Users
            .Where(u => followingIds.Contains(u.Id))
            .Select(u => u.UserName!)
            .ToListAsync();


        //return await _context.Follows
        //    .Where(f => f.FollowerId == userId)
        //    .Select(f => f.FollowingId)
        //    .ToListAsync();
    }

}
