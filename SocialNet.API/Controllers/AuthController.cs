

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.DTOs;
using SocialNet.Infrastructure.Identity;

namespace SocialNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenService _tokenService;



    public AuthController(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager,
        TokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }



    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
        };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if(!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("რეგისტრაცია წარამატებულია");


    }





    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            return Unauthorized("მომხარებელი ვერ მოიძებნა");

        var result = await _signInManager
            .CheckPasswordSignInAsync(user, dto.Password, false);

        if (!result.Succeeded)
            return Unauthorized("პაროლი არასწორია");

        var token = _tokenService.GenerateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            UserName = user.UserName!,
            Email = dto.Email,
            //ესეც იგივე 6/14/2026 20:36 userId ro wigos
            UserId = user.Id
        });
    }
}
