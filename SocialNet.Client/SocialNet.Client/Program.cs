using SocialNet.Client.Client.Pages;
using SocialNet.Client.Components;
using SocialNet.Client.Client.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// ← შეცვალე მარტივ HttpClient-ზე
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7232/")
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<PostService>();

builder.Services.AddScoped<FollowService>(); // ← 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SocialNet.Client.Client._Imports).Assembly);

app.Run();