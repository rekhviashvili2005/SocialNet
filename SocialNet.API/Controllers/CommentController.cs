using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.DTOs;
using SocialNet.Application.Interfaces;
using System.Security.Claims;

namespace SocialNet.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }


    [HttpGet("{postId}")]
    public async Task<IActionResult> GetPostComments(Guid postId)
    {
        var comments = await _commentService.GetPostCommentsAsync(postId);
        return Ok(comments);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddComment(CreateCommentDto dto)
    {
        //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = User.FindFirstValue("uid");

        if (userId == null) return Unauthorized();

        var comment = await _commentService.AddCommentAsync(dto, userId);
        return Ok(comment);
    }


    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = User.FindFirstValue("uid");

        if (userId == null) return Unauthorized();


        var result = await _commentService.DeleteCommentAsync(id, userId);
        if (!result) return BadRequest("წაშლა ვერ მოხერხდა");
        return Ok("კომენტარი წარმატებით წაიშალა");
    }





}
