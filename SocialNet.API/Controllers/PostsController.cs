using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.DTOs;
using SocialNet.Application.Interfaces;
using System.Security.Claims;

namespace SocialNet.API.Controllers;



[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public object ClaimsTypes { get; private set; }

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        //var posts = await _postService.GetAllPostsAsync();
        //return Ok(posts);
        var userId = User.FindFirstValue("uid");
        var posts = await _postService.GetAllPostsAsync(userId);
        return Ok(posts);
    }


   
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = User.FindFirstValue("uid");
        var post = await _postService.GetPostByIdAsync(id, userId);
        if (post == null) return NotFound("პოსტი ვერ მოიძებნა");
        return Ok(post);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePostDto dto)
    {
        // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = User.FindFirstValue("uid");

        if (userId == null) return Unauthorized();

        var post = await _postService.CreatePostAsync(dto, userId);
        return Ok(post);
    }


    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CreatePostDto dto)
    {
        //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = User.FindFirstValue("uid");

        if (userId == null) return Unauthorized();

        var result = await _postService.UpdatePostAsync(id, dto, userId);
        if (!result) return BadRequest("განახლება ვერ მოხერხდა");

        return Ok("პოსტი წარმატებით განახლდა");
    }


    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = User.FindFirstValue("uid");

        if (userId == null) return Unauthorized();

        var result = await _postService.DeletePostAsync(id, userId);
        return Ok("პოსტი წაიშალა");
    }


    [HttpGet("hashtag/{tag}")]
    public async Task<IActionResult> GetByHashtag(string tag)
    {
        //var posts = await _postService.GetPostsByHashtagAsync(tag);
        //return Ok(posts);
        var userId = User.FindFirstValue("uid");
        var posts = await _postService.GetPostsByHashtagAsync(tag, userId);
        return Ok(posts);
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed()
    {
        var userId = User.FindFirstValue("uid");
        var posts = await _postService.GetFeedPostsAsync(userId);
        return Ok(posts);
    }


    [Authorize]
    [HttpGet("following")]
    public async Task<IActionResult> GetFollowingPosts()
    {
        var userId = User.FindFirstValue("uid");
        if (userId == null) return Unauthorized();
        var posts = await _postService.GetFollowingPostsAsync(userId);
        return Ok(posts);
    }
}
