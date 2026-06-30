using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.DTOs;
using SocialNet.Application.Interfaces;
using System.Security.Claims;

namespace SocialNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            var profile = await _userService.GetProfileAsync(username);
            if(profile == null) return NotFound("მომხარებელი ვერ მოიძებნა");
            return Ok(profile);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized();

            var result = await _userService.UpdateProfileAsync(userId, dto);

            if (!result) return BadRequest("განახლება ვერ მოხერხდა");
            return Ok(result);


        }

        [HttpGet("{username}/posts")]
        public async Task<IActionResult> GetUserPosts(string username, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var currentUserId = User.FindFirstValue("uid");
            var posts = await _userService.GetUserPostsAsync(username, currentUserId, page, pageSize);
            return Ok(posts);
        }


        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Ok(new List<UserSearchDto>());
            var users = await _userService.SearchUsersAsync(query);
            return Ok(users);
        }

        [Authorize]
        [HttpGet("suggested")]
        public async Task<IActionResult> GetSuggested([FromQuery] int count = 5)
        {
            var currentUserId = User.FindFirstValue("uid");
            var users = await _userService.GetSuggestedUsersAsync(currentUserId, count);
            return Ok(users);
        }


    }


}
