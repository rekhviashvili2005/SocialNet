using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Application.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email {  get; set; } = string.Empty;

    //userId ro waigos localSotage shi mgoni magas avketeb 6/14/2026 10:37
    public string UserId { get; set; } = string.Empty;
}
