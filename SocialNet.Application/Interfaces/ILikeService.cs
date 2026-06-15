using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Application.Interfaces;

public interface ILikeService
{
    Task<bool> ToggleLikeAsync(Guid postId, string userId);
    Task<int> GetLikesCountAsync(Guid postId);
    Task<bool> IsLikedByUserAsync(Guid postId, string userId);
}
