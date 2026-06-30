
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SocialNet.Client.Client.Services;

public class PostService
{
    private readonly HttpClient _httpClient;
    private readonly LocalStorageService _localStorage;

    public PostService(HttpClient httpClient, LocalStorageService localStorage)
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


    public async Task<List<PostDto>?> GetAllPostsAsync(int page = 1, int pageSize = 10)
    {
        await SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<PostDto>>($"api/Posts?page={page}&pageSize={pageSize}");
    }
    public async Task<List<PostDto>?> GetPostsByHashtagAsync(string tag, int page = 1, int pageSize = 10)
    {
        await SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<PostDto>>($"api/Posts/hashtag/{tag}?page={page}&pageSize={pageSize}");
    }





    public async Task<bool> CreatePostAsync(string content, List<string> hashtags, string? imageUrl = null, List<string>? imageUrls = null)
    {
        await SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/Posts", new
        {
            content = content,
            imageUrl = imageUrl,
            imageUrls = imageUrls ?? new List<string>(),
            hashtags = hashtags
        });
        return response.IsSuccessStatusCode;
    }

    public async Task<List<CommentDto>?> GetCommentsAsync(Guid postId)
    {
        return await _httpClient
            .GetFromJsonAsync<List<CommentDto>>($"api/Comment/{postId}");
    }

    public async Task<bool> AddCommentAsync(Guid postId, string content, List<string>? imageUrls = null)
    {
        await SetAuthHeader();

        var response = await _httpClient.PostAsJsonAsync("api/Comment", new
        {
            content = content,
            postId = postId,
            imageUrls = imageUrls ?? new List<string>()
        });

        return response.IsSuccessStatusCode;
    }




    public async Task<bool> ToggleLikeAsync(Guid postId)
    {
        await SetAuthHeader();

        var response = await _httpClient
            .PostAsync($"api/Likes/{postId}", null);
        return response.IsSuccessStatusCode;
    }


    public async Task<bool> DeletePostAsync(Guid postId)
    {
        await SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"api/Posts/{postId}");
        return response.IsSuccessStatusCode;
    }



    public async Task<string?> UploadImageAsync(Stream imageStream, string fileName)
    {
        await SetAuthHeader();

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(imageStream);
        content.Add(streamContent, "file", fileName);

        var response = await _httpClient.PostAsync("api/Image/upload", content);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ImageUploadResult>();
        return result?.Url;
    }


    public async Task<List<string>> UploadImagesAsync(IReadOnlyList<IBrowserFile> files)
    {
        var urls = new List<string>();
        foreach (var file in files)
        {
            using var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
            var url = await UploadImageAsync(stream, file.Name);
            if (url != null)
                urls.Add(url);
        }
        return urls;
    }

    public async Task<List<PostDto>?> GetFeedPostsAsync(int page = 1, int pageSize = 10)
    {
        await SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<PostDto>>($"api/Posts/feed?page={page}&pageSize={pageSize}");
    }


    public async Task<List<PostDto>?> GetFollowingPostsAsync(int page = 1, int pageSize = 10)
    {
        await SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<PostDto>>($"api/Posts/following?page={page}&pageSize={pageSize}");
    }


    public async Task<PostDto?> GetPostByIdAsync(Guid id)
    {
        await SetAuthHeader();
        try
        {
            return await _httpClient.GetFromJsonAsync<PostDto>($"api/Posts/{id}");
        }
        catch { return null; }
    }



    public async Task<List<PostDto>?> GetUserPostsAsync(string username, int page = 1, int pageSize = 10)
    {
        await SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<PostDto>>($"api/Users/{username}/posts?page={page}&pageSize={pageSize}");
    }




    public async Task<bool> UpdatePostAsync(Guid postId, string content, List<string> hashtags, List<string>? imageUrls = null)
    {
        await SetAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"api/Posts/{postId}", new
        {
            content = content,
            imageUrls = imageUrls ?? new List<string>(),
            hashtags = hashtags
        });
        return response.IsSuccessStatusCode;
    }
}

public class PostDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    public string AuthorId { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    public List<string> ImageUrls { get; set; } = new(); // ახალი
    public DateTime CreatedAt { get; set; }
    public int LikesCount { get; set; }
    public List<string> Hashtags { get; set; } = new();

    //

    public bool IsLikedByCurrentUser { get; set; }
}

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public DateTime CreatedAt { get; set; }

    //commentshi suratebistvus
    //public string? ImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new(); //bevri suratistvis

}

public class UserSearchDto
{
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    
}


public class ImageUploadResult
{
    public string Url { get; set; } = string.Empty;
}