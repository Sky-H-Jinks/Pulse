using System.ComponentModel.DataAnnotations;

namespace ControlPlane.Api.Dtos.Login;

public record LoginRequest(
    [Required] string Username, 
    [Required] string Password
);