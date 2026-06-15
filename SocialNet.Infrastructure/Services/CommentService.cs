using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.DTOs;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;
using SocialNet.Infrastructure.Identity;
using SocialNet.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _useManager;


    public CommentService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> useManager)
    {
        _context = context;
        _useManager = useManager;
    }



    public async Task<List<CommentDto>> GetPostCommentsAsync(Guid postId)
    {
        var comments = await _context.Comments
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var result = new List<CommentDto>();

        foreach(var comment in comments)
        {
            var user = await _useManager.FindByIdAsync(comment.UserId);
            result.Add(new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                UserId = comment.UserId,
                // UserName = user?.UserName ?? "უცნობი",
                UserName = user?.UserName ?? user?.Email ?? comment.UserId,
                PostId = comment.PostId,
                CreatedAt = comment.CreatedAt,
            });
        }

        return result;

    }



    public async Task<CommentDto> AddCommentAsync(CreateCommentDto dto, string userId)
    {
        var comment = new Comment
        {
            Content = dto.Content,
            PostId = dto.PostId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow

        };


        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        var user = await _useManager.FindByIdAsync(userId); //Id შევცვაკე email ით იყო


        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            UserName = user?.UserName ?? user?.Email ?? userId,
           // UserName = user?.UserName ?? "უცნობი",
            PostId = comment.PostId,
            CreatedAt = comment.CreatedAt
        };
    }


    public async Task<bool> DeleteCommentAsync(Guid id, string userId)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null || comment.UserId != userId) return false;

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return true;
    }


}


