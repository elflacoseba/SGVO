namespace SGVO.Application.Features.Cargos.Dtos;

/// <summary>
/// Request DTO for updating an existing cargo.
/// </summary>
public record UpdateCargoRequest(string Nombre, string? Descripcion);
