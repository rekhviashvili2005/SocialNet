using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SocialNet.Application.DTOs;

namespace SocialNet.Application.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetPostCommentsAsync(Guid postId);
    Task<CommentDto> AddCommentAsync(CreateCommentDto dto, string userId);
    Task<bool> DeleteCommentAsync(Guid id, string userId);

}
