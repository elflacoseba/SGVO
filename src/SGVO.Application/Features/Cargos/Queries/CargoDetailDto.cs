using SGVO.Domain.Entities;

namespace SGVO.Application.Features.Cargos.Queries;

/// <summary>
/// DTO for single cargo detail response.
/// </summary>
public class CargoDetailDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }

    /// <summary>
    /// Creates a CargoDetailDto from a Cargo domain entity.
    /// </summary>
    public static CargoDetailDto FromEntity(Cargo cargo) => new()
    {
        Id = cargo.Id,
        Nombre = cargo.Nombre,
        Descripcion = cargo.Descripcion,
        Activo = cargo.Activo,
        CreadoEn = cargo.CreadoEn,
        ModificadoEn = cargo.ModificadoEn
    };
}
