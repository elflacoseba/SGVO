using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Commands;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Domain.Entities;
using SGVO.Domain.Interfaces;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for CrearCargoCommand. Creates a new cargo using domain entity and repository.
/// </summary>
public class CrearCargoCommandHandler : ICommandHandler<CrearCargoCommand, CargoDetailDto>
{
    private readonly IRepository<Cargo> _repository;
    private readonly SgvoDbContext _dbContext;

    public CrearCargoCommandHandler(IRepository<Cargo> repository, SgvoDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<Result<CargoDetailDto>> Handle(
        CrearCargoCommand command,
        CancellationToken cancellationToken)
    {
        var cargo = new Cargo(command.Nombre, command.Descripcion);

        _repository.Add(cargo);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<CargoDetailDto>.Success(CargoDetailDto.FromEntity(cargo));
    }
}
