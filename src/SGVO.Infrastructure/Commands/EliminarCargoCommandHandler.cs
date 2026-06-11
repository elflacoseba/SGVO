using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Commands;
using SGVO.Domain.Entities;
using SGVO.Domain.Interfaces;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for EliminarCargoCommand. Soft-deletes a cargo using domain entity method.
/// Blocks deletion if active Puestos or active CargoSkills reference the cargo.
/// </summary>
public class EliminarCargoCommandHandler : ICommandHandler<EliminarCargoCommand, Unit>
{
    private readonly IRepository<Cargo> _repository;
    private readonly SgvoDbContext _dbContext;

    public EliminarCargoCommandHandler(IRepository<Cargo> repository, SgvoDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<Result<Unit>> Handle(
        EliminarCargoCommand command,
        CancellationToken cancellationToken)
    {
        var cargo = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (cargo is null)
            return Result<Unit>.Failure(
                $"Cargo con id {command.Id} no encontrado.",
                "NOT_FOUND");

        // Check if already soft-deleted
        if (cargo.EliminadoEn.HasValue)
            return Result<Unit>.Failure(
                $"Cargo con id {command.Id} ya está eliminado.",
                "NOT_FOUND");

        // Guard: check for active Puestos referencing this cargo
        var hasActivePuestos = await _dbContext.Puestos
            .AnyAsync(p => p.CargoId == command.Id && p.EliminadoEn == null, cancellationToken);

        if (hasActivePuestos)
            return Result<Unit>.Failure(
                "No se puede eliminar: tiene puestos activos.",
                "CONFLICT");

        // Guard: check for active CargoSkills referencing this cargo
        var hasActiveCargoSkills = await _dbContext.CargoSkills
            .AnyAsync(cs => cs.CargoId == command.Id && cs.EliminadoEn == null, cancellationToken);

        if (hasActiveCargoSkills)
            return Result<Unit>.Failure(
                "No se puede eliminar: tiene skills activos.",
                "CONFLICT");

        cargo.Eliminar(command.EliminadoPor);

        _repository.Update(cargo);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
