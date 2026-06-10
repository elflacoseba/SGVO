using SGVO.Application.Common;
using SGVO.Application.Features.Auth.Dtos;
using SGVO.Domain.Interfaces;
using SGVO.Shared;

namespace SGVO.Application.Features.Auth.Queries;

/// <summary>
/// Handler para obtener la información del usuario autenticado actual.
/// </summary>
public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, UserDto?>
{
    private readonly IAuthService _authService;

    public GetCurrentUserQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<UserDto?>> Handle(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _authService.GetUserByIdAsync(query.UserId, cancellationToken);

        if (result.IsFailure)
            return Result<UserDto?>.Failure(result.Error!, result.ErrorCode);

        var (id, username, email, roles) = result.Value;

        var userDto = new UserDto
        {
            Id = id,
            Username = username,
            Email = email,
            Roles = roles.ToList()
        };

        return Result<UserDto?>.Success(userDto);
    }
}
