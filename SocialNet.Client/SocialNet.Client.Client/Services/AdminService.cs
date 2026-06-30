using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace SocialNet.Client.Client.Services;

public class AdminUserDto
{
    public string Id { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Bio { get; set; }
    public bool IsBanned { get; set; }
}

public class AdminPostDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class AdminService
{
    private readonly HttpClient _http;
    private readonly LocalStorageService _localStorage;

    public AdminService(HttpClient http, LocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    private async Task SetAuthHeader()
    {
        var token = await _localStorage.GetItemAsync("token");
        if (token != null)
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<AdminUserDto>> GetAllUsersAsync()
    {
        await SetAuthHeader();
        return await _http.GetFromJsonAsync<List<AdminUserDto>>("api/Admin/users") ?? new();
    }

    public async Task<List<AdminPostDto>> GetUserPostsAsync(string userId)
    {
        await SetAuthHeader();
        return await _http.GetFromJsonAsync<List<AdminPostDto>>($"api/Admin/users/{userId}/posts") ?? new();
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        await SetAuthHeader();
        var response = await _http.DeleteAsync($"api/Admin/users/{userId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeletePostAsync(Guid postId)
    {
        await SetAuthHeader();
        var response = await _http.DeleteAsync($"api/Admin/posts/{postId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> BanUserAsync(string userId)
    {
        await SetAuthHeader();
        var response = await _http.PostAsync($"api/Admin/users/{userId}/ban", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UnbanUserAsync(string userId)
    {
        await SetAuthHeader();
        var response = await _http.PostAsync($"api/Admin/users/{userId}/unban", null);
        return response.IsSuccessStatusCode;
    }
}