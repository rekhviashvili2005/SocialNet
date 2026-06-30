using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.Interfaces;
using System.Security.Claims;

namespace SocialNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;
        

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }



        [HttpPost("{targetUserId}")]
        public async Task<IActionResult> Follow(string targetUserId)
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized();

            await _followService.FollowAsync(userId, targetUserId);
            return Ok(new {message = "follow ადამიანს"});
        }


        [HttpDelete("{targetUserId}")]
        public async Task<IActionResult> Unfollow(string targetUserId)
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized();

            await _followService.UnFollowAsync(userId, targetUserId);
            return Ok(new { message = "Unfollow " });
        }


        [AllowAnonymous]
        [HttpGet("{targetUserId}/following-count")]
        public async Task<IActionResult> GetFollowingCount(string targetUserId)
        {
            var count = await _followService.GetFollowingCountAsync(targetUserId);
            return Ok(count);
        }





        [HttpGet("{targetUserId}/isFollowing")]
        public async Task<IActionResult> IsFollowing(string targetUserId)
        {
            var userId = User.FindFirstValue("uid");
            if(userId == null) return Unauthorized();

            var result = await _followService.IsFollowingAsync(userId, targetUserId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{targetUserId}/followers-count")]
        public async Task<IActionResult> GetFollowersCount(string targetUserId)
        {
            var count = await _followService.GetFollowersCountAsync(targetUserId);
            return Ok(count);
        }




        //
        [AllowAnonymous]
        [HttpGet("{targetUserId}/followers")]
        public async Task<IActionResult> GetFollowers(string targetUserId)
        {
            var followers = await _followService.GetFollowersAsync(targetUserId);
            return Ok(followers);
        }

        [AllowAnonymous]
        [HttpGet("{targetUserId}/following")]
        public async Task<IActionResult> GetFollowing(string targetUserId)
        {
            var following = await _followService.GetFollowingAsync(targetUserId);
            return Ok(following);
        }


    }
}
