using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Shared;

namespace SGVO.Application.Features.Cargos.Commands;

/// <summary>
/// Command to create a new cargo.
/// </summary>
public sealed record CrearCargoCommand(string Nombre, string? Descripcion) : ICommand<CargoDetailDto>;

/// <summary>
/// Command to update an existing cargo.
/// </summary>
public sealed record ActualizarCargoCommand(long Id, string Nombre, string? Descripcion) : ICommand<CargoDetailDto>;

/// <summary>
/// Command to soft-delete a cargo.
/// </summary>
public sealed record EliminarCargoCommand(long Id, long EliminadoPor) : ICommand<Unit>;

/// <summary>
/// Command to reactivate a soft-deleted cargo.
/// </summary>
public sealed record ReactivarCargoCommand(long Id) : ICommand<CargoDetailDto>;
