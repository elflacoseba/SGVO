using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Commands;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Domain.Entities;
using SGVO.Domain.Interfaces;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for ReactivarCargoCommand. Reactivates a soft-deleted cargo.
/// Idempotent: if already active, returns the cargo without changes.
/// </summary>
public class ReactivarCargoCommandHandler : ICommandHandler<ReactivarCargoCommand, CargoDetailDto>
{
    private readonly IRepository<Cargo> _repository;
    private readonly SgvoDbContext _dbContext;

    public ReactivarCargoCommandHandler(IRepository<Cargo> repository, SgvoDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<Result<CargoDetailDto>> Handle(
        ReactivarCargoCommand command,
        CancellationToken cancellationToken)
    {
        var cargo = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (cargo is null)
            return Result<CargoDetailDto>.Failure(
                $"Cargo con id {command.Id} no encontrado.",
                "NOT_FOUND");

        // Idempotent: if already active, return as-is
        if (!cargo.EliminadoEn.HasValue)
            return Result<CargoDetailDto>.Success(CargoDetailDto.FromEntity(cargo));

        cargo.Reactivar();

        _repository.Update(cargo);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<CargoDetailDto>.Success(CargoDetailDto.FromEntity(cargo));
    }
}
