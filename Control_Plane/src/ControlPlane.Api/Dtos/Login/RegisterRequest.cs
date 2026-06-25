using System.ComponentModel.DataAnnotations;

namespace ControlPlane.Api.Dtos.Login;

public record RegisterRequest(
    [Required] string Username,
    [Required] string Password, 
    [Required, EmailAddress] string Email
);