using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SocialNet.Client.Client.Services;

public class NotificationService
{
    private readonly HttpClient _httpClient;
    private readonly LocalStorageService _localStorage;

    public NotificationService(HttpClient httpClient, LocalStorageService localStorage)
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

    public async Task<List<NotificationDto>> GetNotificationsAsync()
    {
        await SetAuthHeader();
        try
        {
            return await _httpClient.GetFromJsonAsync<List<NotificationDto>>("api/Notifications") ?? new();
        }
        catch { return new(); }
    }

    public async Task<int> GetUnreadCountAsync()
    {
        await SetAuthHeader();
        try
        {
            return await _httpClient.GetFromJsonAsync<int>("api/Notifications/unread-count");
        }
        catch { return 0; }
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        await SetAuthHeader();
        await _httpClient.PutAsync($"api/Notifications/{id}/read", null);
    }

    public async Task MarkAllAsReadAsync()
    {
        await SetAuthHeader();
        await _httpClient.PutAsync("api/Notifications/read-all", null);
    }
}

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string ActorUserName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}