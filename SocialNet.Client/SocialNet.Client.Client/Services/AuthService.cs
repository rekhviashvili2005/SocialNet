using System.Net.Http.Json;
using System.Reflection.Metadata;

namespace SocialNet.Client.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly LocalStorageService _localStorage;

    public AuthService(HttpClient httpClient, LocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", new
        {
            email = email,
            password = password
        });

        if (!response.IsSuccessStatusCode) return null;


        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        //
        if(result == null) return null;

        await _localStorage.SetItemAsync("token", result.Token);
        await _localStorage.SetItemAsync("userId", result.UserId);
        //6/15/2026 -- 5:12
        await _localStorage.SetItemAsync("username", result.UserName);

        return result?.Token;
    }

    public async Task<bool> RegisterAsync(string userName, string email,
        string password, string displayName)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", new
        {
            userName = userName,
            email = email,
            password = password,
            displayName = displayName
        });

        return response.IsSuccessStatusCode;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("token");
        await _localStorage.RemoveItemAsync("userId");

        //6/15/2026 -- 5:12
        await _localStorage.RemoveItemAsync("username");


    }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;


    //mere davamate  UserId c ro waigos
    public string UserId { get; set; } = string.Empty;
    
}