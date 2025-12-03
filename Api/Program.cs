using System.Text;
using Api.Authorization;
using Api.Extensions;
using Api.GraphQL.ErrorFilters;
using Api.GraphQL.Mutations;
using Api.GraphQL.Queries;
using Application;
using Domain.Abstractions;
using Infrastructure;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Orbit API");


// Add services to the container
    builder.Services.AddHttpContextAccessor(); // Required for accessing HTTP context in services
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS for development
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentCorsPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

// Add JWT Authentication
    string jwtSecret = builder.Configuration["Jwt:Secret"] ??
                       throw new InvalidOperationException("JWT Secret is not configured");
    string jwtIssuer = builder.Configuration["Jwt:Issuer"] ??
                       throw new InvalidOperationException("JWT Issuer is not configured");
    string jwtAudience = builder.Configuration["Jwt:Audience"] ??
                         throw new InvalidOperationException("JWT Audience is not configured");

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.Zero // Remove default 5 minute clock skew
            };
        });

// Add Authorization with permission-based policies
    builder.Services.AddAuthorization(options =>
    {
        // Automatically register all permission policies using the extension method
        options.AddPermissionPoliciesWithBuilder();
    });

// Register the permission authorization handler
    builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Add GraphQL
    builder.Services
        .AddGraphQLServer()
        .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<HealthQueries>()
        .AddTypeExtension<UserQueries>()
        .AddTypeExtension<RoleQueries>()
        .AddTypeExtension<CustomerQueries>()
        .AddTypeExtension<ProductQueries>()
        .AddTypeExtension<LocationQueries>()
        .AddTypeExtension<InventoryQueries>()
        .AddMutationType(d => d.Name("Mutation"))
        .AddTypeExtension<UserMutations>()
        .AddTypeExtension<CustomerMutations>()
        .AddTypeExtension<ProductMutations>()
        .AddTypeExtension<LocationMutations>()
        .AddTypeExtension<InventoryMutations>()
        .AddErrorFilter<DomainErrorFilter>()
        .AddAuthorization()
        .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = builder.Environment.IsDevelopment());

    builder.Services.AddOpenApi();

    WebApplication app = builder.Build();

// Auto-apply migrations and seed data in development
    if (app.Environment.IsDevelopment())
    {
        using IServiceScope scope = app.Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        // Seed initial data
        ILogger<DatabaseSeeder> logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();
        IPasswordHasher passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var seeder = new DatabaseSeeder(dbContext, logger, passwordHasher);
        await seeder.SeedAsync();

        app.MapOpenApi();
    }

// Use CORS in development
    if (app.Environment.IsDevelopment())
    {
        app.UseCors("DevelopmentCorsPolicy");
    }

    app.UseHttpsRedirection();

// Add authentication and authorization middleware
    app.UseAuthentication();
    app.UseAuthorization();

// Map GraphQL endpoint
    app.MapGraphQL();

    await app.RunAsync().ConfigureAwait(false);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
