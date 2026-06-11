namespace SGVO.Application.Features.Cargos.Dtos;

/// <summary>
/// Request DTO for creating a new cargo.
/// </summary>
public record CreateCargoRequest(string Nombre, string? Descripcion);
