namespace SGVO.Application.Features.Auth.Dtos;

/// <summary>
/// DTO de solicitud para iniciar sesión.
/// </summary>
public class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
