using ControlPlane.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace ControlPlane.Api;

public class Program
{
    public static void Main(string[] args)
    {
        DotNetEnv.Env.Load();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddControllers();

        builder.Services.AddDbContext<ControlPlaneDbContext>(options =>
            options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL") ?? throw new InvalidOperationException("Connection string 'ControlPlaneDbContext' not found.")));
    
        var app = builder.Build();

        app.MapControllers();
        app.UseCors();
        
        app.Run();
    }
}