using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Commands;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Domain.Entities;
using SGVO.Domain.Interfaces;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for ActualizarCargoCommand. Updates an existing cargo using domain entity methods.
/// </summary>
public class ActualizarCargoCommandHandler : ICommandHandler<ActualizarCargoCommand, CargoDetailDto>
{
    private readonly IRepository<Cargo> _repository;
    private readonly SgvoDbContext _dbContext;

    public ActualizarCargoCommandHandler(IRepository<Cargo> repository, SgvoDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<Result<CargoDetailDto>> Handle(
        ActualizarCargoCommand command,
        CancellationToken cancellationToken)
    {
        var cargo = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (cargo is null)
            return Result<CargoDetailDto>.Failure(
                $"Cargo con id {command.Id} no encontrado.",
                "NOT_FOUND");

        // Check if already soft-deleted
        if (cargo.EliminadoEn.HasValue)
            return Result<CargoDetailDto>.Failure(
                $"Cargo con id {command.Id} está eliminado.",
                "NOT_FOUND");

        cargo.Actualizar(command.Nombre, command.Descripcion);

        _repository.Update(cargo);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<CargoDetailDto>.Success(MapToDto(cargo));
    }

    private static CargoDetailDto MapToDto(Cargo cargo) => new()
    {
        Id = cargo.Id,
        Nombre = cargo.Nombre,
        Descripcion = cargo.Descripcion,
        Activo = cargo.Activo,
        CreadoEn = cargo.CreadoEn,
        ModificadoEn = cargo.ModificadoEn
    };
}
