using SocialNet.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Application.Interfaces;

public interface IUserService
{
    Task<UserProfileDto?> GetProfileAsync(string username);
    Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto);
    Task<List<PostDto>> GetUserPostsAsync(string username);

}
