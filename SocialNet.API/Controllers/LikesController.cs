using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using SocialNet.Application.Interfaces;
using System.Security.Claims;

namespace SocialNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikesController(ILikeService likeService)
        {
            _likeService = likeService;
        }


        [HttpGet("{postId}/count")]
        public async Task<IActionResult> GetLikesCount(Guid postId)
        {
            var count = await _likeService.GetLikesCountAsync(postId);
            return Ok(count);
        }

        [Authorize]
        [HttpPost("{postId}")]
        public async Task<IActionResult> ToggleLike(Guid postId)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = User.FindFirstValue("uid");

            if (userId == null) return Unauthorized();

            var isLiked = await _likeService.ToggleLikeAsync(postId, userId);

            return Ok(new 
            {
                isLiked = isLiked,
                message = isLiked ? "ლაიქი დაემტა" : "ლაიქი წაიშალა"
            });

            

        }

        [Authorize]
        [HttpGet("{postId}/isLiked")]
        public async Task<IActionResult> Isliked(Guid postId)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = User.FindFirstValue("uid");

            if (userId == null) return Unauthorized();

            var isLiked = await _likeService.IsLikedByUserAsync(postId, userId);
            return Ok(isLiked);
        }


    }
}
