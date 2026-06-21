using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.Interfaces;

namespace SocialNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("სურათი არ არის არჩეული");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("სურათი მაქსიმუმ 5MB უნდა იყოს");

        try
        {
            using var stream = file.OpenReadStream();
            var url = await _imageService.UploadImageAsync(stream, file.FileName);

            if (url == null)
                return StatusCode(500, "URL null დაბრუნდა");

            return Ok(new { url });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}