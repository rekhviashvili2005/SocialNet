using System.Globalization;
using System.Net.Http.Json;

namespace SocialNet.Client.Client.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<UserProfileDto?> GetProfileAsync(string username)
    {
        return await _httpClient
            .GetFromJsonAsync<UserProfileDto>($"api/Users/{username}");
    }

    public async Task<List<PostDto>> GetUserPostsAsync(string username)
    {
        return await _httpClient
            .GetFromJsonAsync<List<PostDto>>($"api/Users/{username}/posts")
            ?? new();
    }
}



public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DisplayName {  get; set; } = string.Empty;    

    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public int PostsCount { get; set; }

    public DateTime CreatedAt { get; set; }
}