namespace SGVO.Application.Features.Auth.Dtos;

/// <summary>
/// DTO de solicitud para refrescar el token de acceso.
/// </summary>
public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
