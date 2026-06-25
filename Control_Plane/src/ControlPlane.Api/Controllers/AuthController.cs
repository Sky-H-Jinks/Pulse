using ControlPlane.Api.Database;
using ControlPlane.Api.Database.Tables;
using ControlPlane.Api.Dtos.Login;
using ControlPlane.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlPlane.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, [FromServices] ControlPlaneDbContext dbContext, [FromServices] IPasswordHasher<User> passwordHasher)
    {
        if(string.IsNullOrEmpty(request.Username))
            return BadRequest("Username is required.");

        if(string.IsNullOrEmpty(request.Password))
            return BadRequest("Password is required.");

        if(string.IsNullOrEmpty(request.Email))
            return BadRequest("Email is required.");

        if(await dbContext.Users.AnyAsync(u => u.Username == request.Username))
            return Conflict("Username is already taken.");

        if(await dbContext.Users.AnyAsync(u => u.Email == request.Email))
            return Conflict("Email is already registered.");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            EmailConfirmed = false
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        try 
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync(); 
        }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pg)
        {
            return pg.ConstraintName switch
            {
                // names as they appear in your migration / \d users
                var n when n == "IX_users_Username" => Conflict("Username is already taken."),
                var n when n == "IX_users_Email"    => Conflict("Email is already registered."),
                "ck_users_email_format"             => BadRequest("Email format is invalid."),
                _ => Conflict("Could not register user.")
            };
        }

        return Ok(); 
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, [FromServices] ControlPlaneDbContext dbContext, [FromServices] IPasswordHasher<User> passwordHasher, [FromServices] TokenService tokenService)
    {
        if(string.IsNullOrEmpty(request.Username))
            return BadRequest("Username is required.");

        if(string.IsNullOrEmpty(request.Password))
            return BadRequest("Password is required.");
            
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if(user == null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid username or password.");

        var token = tokenService.CreateToken(user);

        return Ok(new { token });
    }
}