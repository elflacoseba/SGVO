using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGVO.Application.Common;
using SGVO.Application.Features.Auth.Commands;
using SGVO.Application.Features.Auth.Dtos;
using SGVO.Application.Features.Auth.Queries;
using SGVO.Shared;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Application.Features.Postulantes.Queries;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;
using SGVO.Application.Features.Vacantes.Queries;
using SGVO.Domain.Interfaces;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Repositories;
using SGVO.Infrastructure.Queries;
using SGVO.Infrastructure.Services;

namespace SGVO.Infrastructure.DependencyInjection;

/// <summary>
/// Métodos de extensión para registrar los servicios de la capa de infraestructura en el contenedor de DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios de infraestructura (repositorios, servicios, etc.).
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        // Servicios de autenticación
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Query handlers
        services.AddScoped<IQueryHandler<GetAllVacantesQuery, PagedResult<VacanteDto>>, GetAllVacantesQueryHandler>();
        services.AddScoped<IQueryHandler<GetVacanteByIdQuery, VacanteDto?>, GetVacanteByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllCargosQuery, PagedResult<CargoDto>>, GetAllCargosQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllPostulantesQuery, PagedResult<PostulanteDto>>, GetAllPostulantesQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllSkillsQuery, PagedResult<SkillDto>>, GetAllSkillsQueryHandler>();
        services.AddScoped<IQueryHandler<GetCurrentUserQuery, UserDto?>, GetCurrentUserQueryHandler>();

        // UnidadesOrganizativas query handlers
        services.AddScoped<IQueryHandler<GetAllUnidadesOrganizativasQuery, PagedResult<UnidadOrganizativaDto>>, GetAllUnidadesOrganizativasQueryHandler>();
        services.AddScoped<IQueryHandler<GetUnidadOrganizativaByIdQuery, UnidadOrganizativaDetailDto?>, GetUnidadOrganizativaByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetUnidadesOrganizativasTreeQuery, IReadOnlyList<UnidadOrganizativaTreeDto>>, GetUnidadesOrganizativasTreeQueryHandler>();

        // Command handlers
        services.AddScoped<ICommandHandler<LoginCommand, LoginResponseDto>, LoginCommandHandler>();
        services.AddScoped<ICommandHandler<RefreshTokenCommand, LoginResponseDto>, RefreshTokenCommandHandler>();
        services.AddScoped<ICommandHandler<LogoutCommand, Unit>, LogoutCommandHandler>();

        return services;
    }

    /// <summary>
    /// Registra el DbContext con la cadena de conexión de MySQL.
    /// </summary>
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");

        services.AddDbContext<SgvoDbContext>(options =>
        {
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString));
        });

        // También registramos el DbContext base para compatibilidad con IRepository<T>
        services.AddScoped<Microsoft.EntityFrameworkCore.DbContext>(sp => sp.GetRequiredService<SgvoDbContext>());

        return services;
    }

    /// <summary>
    /// Registra los servicios de la capa de aplicación (validadores, etc.).
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Application.IAssemblyMarker>();

        return services;
    }
}
