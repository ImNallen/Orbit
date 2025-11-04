using System.Net;
using System.Net.Mail;
using Application.Abstractions.Email;
using Domain.Abstractions;
using Domain.Customers;
using Domain.Permission;
using Domain.Products;
using Domain.Role;
using Domain.Session;
using Domain.Users;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Database;
using Infrastructure.Database.Repositories;
using Infrastructure.EmailConfiguration;
using Infrastructure.EmailServices;
using Infrastructure.Http;
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
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

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
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenGenerator, TokenGenerator>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<ITokenExpirationSettings, TokenExpirationSettings>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<Domain.Abstractions.IHttpContextAccessor, HttpContextAccessor>();

        // Email Services
        AddEmailServices(services, configuration);

        return services;
    }

    /// <summary>
    /// Registers email services with FluentEmail.
    /// </summary>
    private static void AddEmailServices(IServiceCollection services, IConfiguration configuration)
    {
        EmailOptions emailOptions = configuration.GetSection(EmailOptions.SectionName).Get<EmailOptions>() ?? new EmailOptions();

        // Configure FluentEmail
        services
            .AddFluentEmail(emailOptions.FromEmail, emailOptions.FromName)
            .AddSmtpSender(() =>
            {
                var smtpClient = new SmtpClient(emailOptions.Smtp.Host, emailOptions.Smtp.Port)
                {
                    EnableSsl = emailOptions.Smtp.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = string.IsNullOrEmpty(emailOptions.Smtp.Username)
                };

                if (!string.IsNullOrEmpty(emailOptions.Smtp.Username))
                {
                    smtpClient.Credentials = new NetworkCredential(
                        emailOptions.Smtp.Username,
                        emailOptions.Smtp.Password);
                }

                return smtpClient;
            });

        // Register our email service implementation
        services.AddScoped<IEmailService, FluentEmailService>();
    }
}
