using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SocialNet.Infrastructure.Identity;
using SocialNet.Infrastructure.Persistence;
using System.Text;
//using Scalar.AspNetCore;
using SocialNet.Application.Interfaces;
using SocialNet.Infrastructure.Services;




var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnectionString));



//Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


//JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// TokenService
builder.Services.AddScoped<TokenService>();

//PostService
builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<IUserService, UserService>();

//
builder.Services.AddScoped<IFollowService, FollowService>();

//
builder.Services.AddScoped<IImageService, ImageService>();

//notifications
builder.Services.AddScoped<INotificationService, NotificationService>();


//Cors blazor ?????????? ? ????? ?????
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7136",
            "http://localhost:5246")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});


builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
     app.MapOpenApi();
     app.MapScalarApiReference();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

//cors ????
app.UseCors("BlazorPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
