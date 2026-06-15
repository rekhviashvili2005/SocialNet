using SocialNet.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Application.Interfaces;

public interface IPostService
{
    Task<List<PostDto>> GetAllPostsAsync();
    Task<PostDto?> GetPostByIdAsync(Guid id);
    Task<PostDto> CreatePostAsync(CreatePostDto dto, string userId);

    Task<bool> UpdatePostAsync(Guid id, CreatePostDto dto, string userId);
    Task<bool> DeletePostAsync(Guid id, string userId);
}

