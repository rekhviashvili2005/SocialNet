//using System.Net.Http.Headers;
//using System.Net.Http.Json;

//namespace SocialNet.Client.Client.Services;

//public class PostService
//{
//    private readonly HttpClient _httpClient;
//    private readonly LocalStorageService _localStorage;

//    public PostService(HttpClient httpClient, LocalStorageService localStorage)
//    {
//        _httpClient = httpClient;
//        _localStorage = localStorage;
//    }

//    private async Task SetAuthHeader()
//    {
//        var token = await _localStorage.GetItemAsync("token");
//        if (token != null)
//        {
//            _httpClient.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", token);
//        }
//    }

//    public async Task<List<PostDto>?> GetAllPostsAsync()
//    {
//        return await _httpClient.GetFromJsonAsync<List<PostDto>>("api/Posts");
//    }

//    public async Task<bool> CreatePostAsync(string content)
//    {
//        await SetAuthHeader();

//        var response = await _httpClient.PostAsJsonAsync("api/Posts", new
//        {
//            content = content,
//            imageUrl = (string?)null
//        });

//        return response.IsSuccessStatusCode;
//    }

//    public async Task<List<CommentDto>?> GetCommentsAsync(Guid postId)
//    {
//        return await _httpClient
//            .GetFromJsonAsync<List<CommentDto>>($"api/Comment/{postId}");
//    }

//    public async Task<bool> AddCommentAsync(Guid postId, string content)
//    {
//        await SetAuthHeader();

//        var response = await _httpClient.PostAsJsonAsync("api/Comment", new
//        {
//            content = content,
//            postId = postId
//        });

//        return response.IsSuccessStatusCode;
//    }

//    //public async Task<bool> ToggleLikeAsync(Guid postId)
//    //{
//    //    await SetAuthHeader();

//    //    var response = await _httpClient
//    //        .PostAsJsonAsync($"api/Likes/{postId}", new { });

//    //    return response.IsSuccessStatusCode;
//    //}


//    public async Task<bool> ToggleLikeAsync(Guid postId)
//    {
//        await SetAuthHeader();

//        var response = await _httpClient
//            .PostAsync($"api/Likes/{postId}", null);

//        return response.IsSuccessStatusCode;
//    }
//}

//public class PostDto
//{
//    public Guid Id { get; set; }
//    public string Content { get; set; } = string.Empty;
//    public string UserName { get; set; } = string.Empty;
//    public string? ImageUrl { get; set; }
//    public DateTime CreatedAt { get; set; }



//    public int LikesCount { get; set; } //6/10/2026 //string iyo da int shevvcale


//}

//public class CommentDto
//{
//    public Guid Id { get; set; }
//    public string Content { get; set; } = string.Empty;
//    public string UserName { get; set; } = string.Empty;
//    public Guid PostId { get; set; }
//    public DateTime CreatedAt { get; set; }
//}

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

    //public async Task<List<PostDto>?> GetAllPostsAsync()
    //{
    //    return await _httpClient.GetFromJsonAsync<List<PostDto>>("api/Posts");
    //}
    public async Task<List<PostDto>?> GetAllPostsAsync()
    {
        await SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<PostDto>>("api/Posts");
    }

    //public async Task<List<PostDto>?> GetPostsByHashtagAsync(string tag)
    //{
    //    return await _httpClient
    //        .GetFromJsonAsync<List<PostDto>>($"api/Posts/hashtag/{tag}");
    //}
    public async Task<List<PostDto>?> GetPostsByHashtagAsync(string tag)
    {
        await SetAuthHeader();
        return await _httpClient
            .GetFromJsonAsync<List<PostDto>>($"api/Posts/hashtag/{tag}");
    }


    //public async Task<bool> CreatePostAsync(string content, List<string> hashtags)
    //{
    //    await SetAuthHeader();

    //    var response = await _httpClient.PostAsJsonAsync("api/Posts", new
    //    {
    //        content = content,
    //        imageUrl = (string?)null,
    //        hashtags = hashtags
    //    });

    //    return response.IsSuccessStatusCode;
    //}
    public async Task<bool> CreatePostAsync(string content, List<string> hashtags, string? imageUrl = null)
    {
        await SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/Posts", new
        {
            content = content,
            imageUrl = imageUrl,
            hashtags = hashtags
        });
        return response.IsSuccessStatusCode;
    }

    public async Task<List<CommentDto>?> GetCommentsAsync(Guid postId)
    {
        return await _httpClient
            .GetFromJsonAsync<List<CommentDto>>($"api/Comment/{postId}");
    }

    public async Task<bool> AddCommentAsync(Guid postId, string content)
    {
        await SetAuthHeader();

        var response = await _httpClient.PostAsJsonAsync("api/Comment", new
        {
            content = content,
            postId = postId
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
}

public class PostDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
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