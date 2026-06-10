namespace SGVO.Application.Features.Auth.Dtos;

/// <summary>
/// DTO de información del usuario autenticado.
/// </summary>
public class UserDto
{
    public ulong Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
