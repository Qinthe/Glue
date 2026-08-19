using Glue.API.Database;
using Glue.API.Middleware;
using Glue.API.Repositories.Implements;
using Glue.API.Repositories.Interfaces;
using Glue.API.Services;
using Glue.API.Services.Implements;
using Glue.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using Dapper;

// ⭐ 配置 Dapper 自动映射 snake_case 到 PascalCase
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

// 添加控制器和 API 探索
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 添加 OpenAPI
builder.Services.AddOpenApi();

#region -- 配置JWT认证
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();
#endregion

#region -- 注册服务
// 注册数据库连接工厂（单例模式）
builder.Services.AddSingleton<IDbConnectionFactory, GlueDBConnectionFactory>();

// 注册仓储（Scoped模式）
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMemoNoteRepository, MemoNoteRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ITabRepository, TabRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();

// 注册服务（Scoped模式）
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMemoNoteService, MemoNoteService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ITabService, TabService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();

// 注册JWT服务（单例模式）
builder.Services.AddSingleton<JwtService>();
#endregion

#region -- 添加CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
#endregion

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// 配置 HTTP 请求管道
app.MapOpenApi();

// 配置 Scalar UI - 不限制环境，方便测试
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Glue API Documentation")
        .WithTheme(ScalarTheme.Purple);
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();
app.MapFallbackToFile("index.html");

// 添加启动信息
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
        .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()
        ?.Addresses;

    if (addresses != null)
    {
        foreach (var address in addresses)
        {
            Console.WriteLine($"API 文档地址: {address}/scalar/v1");
            Console.WriteLine($"OpenAPI JSON: {address}/openapi/v1.json");
        }
    }
});

app.Run();
