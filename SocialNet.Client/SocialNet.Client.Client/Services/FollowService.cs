using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SocialNet.Client.Client.Services;

public class FollowService
{
    private readonly HttpClient _httpClient;
    private readonly LocalStorageService _localStorage;

    public FollowService(HttpClient httpClient, LocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    private async Task SetAuthHeader()
    {
        var token = await _localStorage.GetItemAsync("token");
        if (token != null)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task FollowAsync(string targetUserId)
    {
        await SetAuthHeader();
        await _httpClient.PostAsync($"api/Follow/{targetUserId}", null);
    }

    public async Task UnFollowAsync(string targetUserId)
    {
        await SetAuthHeader();
        await _httpClient.DeleteAsync($"api/Follow/{targetUserId}");
    }

    public async Task<bool> IsFollowingAsync(string targetUserId)
    {
        await SetAuthHeader();
        try
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/Follow/{targetUserId}/isFollowing");
        }
        catch { return false; }
    }

    public async Task<int> GetFollowersCountAsync(string targetUserId)
    {
        await SetAuthHeader();
        try
        {
            return await _httpClient.GetFromJsonAsync<int>($"api/Follow/{targetUserId}/followers-count");
        }
        catch { return 0; }
    }

    public async Task<int> GetFollowingCountAsync(string targetUserId)
    {
        await SetAuthHeader();
        try
        {
            return await _httpClient.GetFromJsonAsync<int>($"api/Follow/{targetUserId}/following-count");
        }
        catch { return 0; }
    }




    ///

    public async Task<List<string>> GetFollowersAsync(string targetUserId)
    {
        await SetAuthHeader();
        try
        {
            return await _httpClient.GetFromJsonAsync<List<string>>($"api/Follow/{targetUserId}/followers") ?? new();

        }
        catch { return new(); }
    }


    public async Task<List<string>> GetFollowingAsync(string targetUserId)
    {
        await SetAuthHeader();
        try
        {
            return await _httpClient.GetFromJsonAsync<List<string>>($"api/Follow/{targetUserId}/following") ?? new();

        }
        catch { return new(); }
    }


}