using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SocialNet.Client.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//API -???? ???????
builder.Services.AddScoped(sp => new HttpClient
{
   BaseAddress = new Uri("https://localhost:7232/")
});

//builder.Services.AddScoped(sp =>
//{
//    var handler = new HttpClientHandler
//    {
//        ServerCertificateCustomValidationCallback =
//            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//    };
//    return new HttpClient(handler)
//    {
//        BaseAddress = new Uri("https://localhost:7232/")
//    };
//});

//services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<FollowService>();

builder.Services.AddScoped<NotificationService>();


builder.Services.AddScoped<AdminService>();




await builder.Build().RunAsync();