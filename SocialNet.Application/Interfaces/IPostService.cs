using SocialNet.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Application.Interfaces;

public interface IPostService
{
    
    Task<List<PostDto>> GetAllPostsAsync(string? userId = null);

    //Task<PostDto?> GetPostByIdAsync(Guid id); qveda like ro ar moqonda magito
    Task<PostDto?> GetPostByIdAsync(Guid id, string? userId = null);
    Task<PostDto> CreatePostAsync(CreatePostDto dto, string userId);

    Task<bool> UpdatePostAsync(Guid id, CreatePostDto dto, string userId);
    Task<bool> DeletePostAsync(Guid id, string userId);

    //
    //Task<List<PostDto>> GetPostsByHashtagAsync(string tag);

    Task<List<PostDto>> GetPostsByHashtagAsync(string tag, string? userId = null);


    Task<List<PostDto>> GetFollowingPostsAsync(string userId);
    Task<List<PostDto>> GetFeedPostsAsync(string userId);
}


