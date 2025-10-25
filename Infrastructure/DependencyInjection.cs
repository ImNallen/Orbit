using Application.Abstractions.Email;
using Domain.Abstractions;
using Domain.Users.Permission;
using Domain.Users.Role;
using Domain.Users.Session;
using Domain.Users.User;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Database;
using Infrastructure.EmailServices;
using Infrastructure.Http;
using Infrastructure.Repositories;
using Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Infrastructure layer services with the DI container.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // Database
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("DefaultConnection"));
        dataSourceBuilder.EnableDynamicJson();
        NpgsqlDataSource dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                dataSource,
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();

        // Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenGenerator, TokenGenerator>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ITokenExpirationSettings, TokenExpirationSettings>();
        services.AddScoped<IEmailService, ConsoleEmailService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<Domain.Abstractions.IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }
}
