using System.Text;
using Api.Authorization;
using Api.GraphQL.Mutations;
using Api.GraphQL.Queries;
using Application;
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

// Add JWT Authentication
string jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured");
string jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
string jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");

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

// Add Authorization with policies
builder.Services.AddAuthorization(options =>
{
    // Permission-based policies
    options.AddPolicy("users:create", policy =>
        policy.Requirements.Add(new PermissionRequirement("users:create")));
    options.AddPolicy("users:read", policy =>
        policy.Requirements.Add(new PermissionRequirement("users:read")));
    options.AddPolicy("users:update", policy =>
        policy.Requirements.Add(new PermissionRequirement("users:update")));
    options.AddPolicy("users:delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("users:delete")));

    options.AddPolicy("roles:create", policy =>
        policy.Requirements.Add(new PermissionRequirement("roles:create")));
    options.AddPolicy("roles:read", policy =>
        policy.Requirements.Add(new PermissionRequirement("roles:read")));
    options.AddPolicy("roles:update", policy =>
        policy.Requirements.Add(new PermissionRequirement("roles:update")));
    options.AddPolicy("roles:delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("roles:delete")));
    options.AddPolicy("roles:assign", policy =>
        policy.Requirements.Add(new PermissionRequirement("roles:assign")));
    options.AddPolicy("roles:remove", policy =>
        policy.Requirements.Add(new PermissionRequirement("roles:remove")));

    options.AddPolicy("permissions:read", policy =>
        policy.Requirements.Add(new PermissionRequirement("permissions:read")));

    options.AddPolicy("sessions:read", policy =>
        policy.Requirements.Add(new PermissionRequirement("sessions:read")));
    options.AddPolicy("sessions:delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("sessions:delete")));
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
    .AddMutationType<UserMutations>()
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
    var seeder = new DatabaseSeeder(dbContext, logger);
    await seeder.SeedAsync();

    app.MapOpenApi();
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
