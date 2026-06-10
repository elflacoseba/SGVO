using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGVO.Domain.Interfaces;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Repositories;
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
