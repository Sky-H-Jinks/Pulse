using System.Text;
using ControlPlane.Api.Database;
using ControlPlane.Api.Database.Tables;
using ControlPlane.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ControlPlane.Api;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(ConfigService.GetConfigValue(ConfigKeys.CorsPolicyOrigin))
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddControllers();

        builder.Services.AddDbContext<ControlPlaneDbContext>(options =>
            options.UseNpgsql(ConfigService.GetConfigValue(ConfigKeys.DatabaseUrl)));
    
        var jwtKey = ConfigService.GetConfigValue(ConfigKeys.JwtKey);
        var jwtIssuer = ConfigService.GetConfigValue(ConfigKeys.JwtIssuer);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtIssuer,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.FromSeconds(ConfigService.GetConfigValue<int>(ConfigKeys.JwtClockSkewSeconds))
            };
        });

        builder.Services.AddSingleton(new TokenService(jwtKey, jwtIssuer));
        builder.Services.AddSingleton<IPasswordHasher<User>>(new PasswordHasher<User>());

        var app = builder.Build();

        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        app.Run();
    }
}