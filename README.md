# Orbit

A modern GraphQL API built with Clean Architecture principles, focusing on authentication, authorization, and user management.

## 🚀 Overview

Orbit is a production-ready GraphQL API built with .NET 9.0 that implements a comprehensive authentication and authorization system. The project follows Clean Architecture principles with a strong emphasis on Domain-Driven Design (DDD), ensuring maintainability, testability, and scalability.

## 🛠️ Technology Stack

- **.NET 9.0** - Latest .NET framework
- **HotChocolate 15.1.11** - GraphQL server framework
- **C# 13** - Latest C# language features
- **Clean Architecture** - Separation of concerns with clear layer boundaries
- **Domain-Driven Design (DDD)** - Rich domain models with entities, value objects, and domain events
- **Result Pattern** - Type-safe error handling without exceptions
- **Entity Framework Core 9.0.10** - ORM for data persistence with PostgreSQL
- **PostgreSQL** - Production-ready relational database
- **BCrypt.Net-Next 4.0.3** - Secure password hashing (work factor 12)
- **JWT** - JSON Web Tokens for stateless authentication
- **MediatR 13.1.0** - CQRS pattern implementation
- **Docker & Docker Compose** - Containerization and orchestration

## 📁 Project Structure

```
Orbit/
├── Domain/                          # Core business logic and domain models
│   ├── Abstractions/
│   │   ├── Entity.cs               # Base entity with domain events
│   │   ├── DomainEvent.cs          # Base domain event
│   │   ├── Result.cs               # Result pattern for error handling
│   │   └── DomainError.cs          # Base domain error
│   └── Users/                      # User bounded context
│       ├── User/                   # User aggregate
│       │   ├── User.cs             # User entity (aggregate root)
│       │   ├── IUserRepository.cs  # User repository interface
│       │   ├── PasswordHistory.cs  # Password history tracking
│       │   ├── Email.cs            # Email value object
│       │   ├── Password.cs         # Password value object
│       │   ├── PasswordHash.cs     # Password hash value object
│       │   ├── FullName.cs         # Full name value object
│       │   ├── UserStatus.cs       # User status enum
│       │   └── *Event.cs           # User domain events (11 events)
│       ├── Role/                   # Role aggregate
│       │   ├── Role.cs             # Role entity (aggregate root)
│       │   ├── IRoleRepository.cs  # Role repository interface
│       │   └── *Event.cs           # Role domain events (2 events)
│       ├── Permission/             # Permission aggregate
│       │   ├── Permission.cs       # Permission entity (aggregate root)
│       │   └── IPermissionRepository.cs # Permission repository interface
│       ├── Session/                # Session aggregate
│       │   ├── Session.cs          # Session entity (aggregate root)
│       │   └── ISessionRepository.cs # Session repository interface
│       └── Errors/
│           └── UserErrors.cs       # User-related domain errors
│
├── Application/                     # Application business logic and use cases
│   ├── Users/
│   │   ├── Commands/               # User commands (CQRS)
│   │   │   ├── RegisterUser/       # User registration
│   │   │   ├── Login/              # User login
│   │   │   ├── RefreshToken/       # Token refresh
│   │   │   ├── VerifyEmail/        # Email verification
│   │   │   ├── RequestPasswordReset/ # Password reset request
│   │   │   ├── ResetPassword/      # Password reset
│   │   │   ├── RevokeSession/      # Session revocation
│   │   │   └── RevokeAllSessions/  # Revoke all sessions
│   │   └── Queries/                # User queries (CQRS)
│   │       ├── GetCurrentUser/     # Get current user
│   │       ├── GetUserById/        # Get user by ID
│   │       ├── GetUsers/           # Get all users
│   │       └── GetUserSessions/    # Get user sessions
│   ├── Roles/
│   │   ├── Commands/
│   │   │   ├── AssignRole/         # Assign role to user
│   │   │   └── RemoveRole/         # Remove role from user
│   │   └── Queries/
│   │       └── GetRoles/           # Get all roles
│   └── Permissions/
│       └── Queries/
│           └── GetPermissions/     # Get all permissions
│
├── Infrastructure/                  # External concerns (database, email, etc.)
│   ├── Authentication/             # Authentication services
│   │   ├── JwtTokenService.cs      # JWT token generation
│   │   ├── PasswordHasher.cs       # BCrypt password hashing
│   │   ├── TokenGenerator.cs       # Secure token generation
│   │   └── JwtOptions.cs           # JWT configuration
│   ├── Authorization/              # Authorization services
│   │   └── AuthorizationService.cs # Permission-based authorization
│   ├── Database/                   # Database context and configuration
│   │   ├── ApplicationDbContext.cs # EF Core DbContext
│   │   ├── DatabaseSeeder.cs       # Seed initial data
│   │   └── Configurations/         # Entity configurations
│   ├── EmailServices/              # Email services
│   │   └── ConsoleEmailService.cs  # Console email implementation
│   ├── Http/                       # HTTP infrastructure
│   │   └── HttpContextAccessor.cs  # HTTP context abstraction
│   ├── Repositories/               # Repository implementations
│   │   ├── UserRepository.cs
│   │   ├── RoleRepository.cs
│   │   ├── PermissionRepository.cs
│   │   └── SessionRepository.cs
│   ├── Time/                       # Time services
│   │   └── DateTimeProvider.cs     # DateTime abstraction
│   └── Migrations/                 # EF Core migrations
│
└── Api/                            # GraphQL API layer
    ├── GraphQL/
    │   ├── Queries/                # GraphQL queries
    │   │   ├── HealthQueries.cs    # Health check
    │   │   ├── UserQueries.cs      # User queries
    │   │   └── RoleQueries.cs      # Role/permission queries
    │   ├── Mutations/              # GraphQL mutations
    │   │   └── UserMutations.cs    # User mutations
    │   ├── Types/                  # GraphQL types
    │   ├── Payloads/               # GraphQL payloads
    │   └── Inputs/                 # GraphQL inputs
    ├── Authorization/              # Authorization infrastructure
    │   ├── PermissionRequirement.cs
    │   └── PermissionAuthorizationHandler.cs
    └── Program.cs                  # Application entry point
```

## ✨ Implemented Features

### 🔐 Authentication & Authorization
- ✅ **User Registration** - Email-based registration with validation
- ✅ **Email Verification** - Token-based email verification workflow
- ✅ **User Login** - Credential-based authentication with JWT tokens
- ✅ **Token Refresh** - Refresh token rotation for enhanced security
- ✅ **Password Reset** - Secure password reset workflow with tokens
- ✅ **Account Lockout** - Automatic lockout after 5 failed login attempts (15-minute duration)
- ✅ **Session Management** - Multiple concurrent sessions with revocation support
- ✅ **Permission-Based Authorization** - Fine-grained access control with 13 permissions

### 👥 User Management
- ✅ **User Queries** - Get current user, user by ID, all users
- ✅ **User Sessions** - View and manage active sessions
- ✅ **Session Revocation** - Revoke individual or all sessions

### 🎭 Role & Permission Management
- ✅ **Role Queries** - View all roles and their permissions
- ✅ **Permission Queries** - View all available permissions
- ✅ **Role Assignment** - Assign roles to users (admin only)
- ✅ **Role Removal** - Remove roles from users (admin only)

### 🏗️ Domain Model
- ✅ **User Aggregate** - Rich domain model with email, password, profile, roles, and sessions
- ✅ **Role Aggregate** - Roles with permission collections
- ✅ **Permission Aggregate** - Resource:action permission pattern
- ✅ **Session Aggregate** - Session tracking with IP and user agent
- ✅ **Value Objects** - Email, Password, PasswordHash, FullName
- ✅ **Domain Events** - 11 events for audit and integration

### 🔒 Security Features
- ✅ **BCrypt Password Hashing** - Work factor 12 for secure password storage
- ✅ **Password History** - Prevents reuse of last 5 passwords
- ✅ **Strong Password Validation** - 8+ chars, mixed case, digits, special characters
- ✅ **JWT Authentication** - Stateless authentication with access tokens
- ✅ **Refresh Token Rotation** - Each refresh generates new tokens
- ✅ **Email Verification Required** - Users must verify email before login
- ✅ **Structured Logging** - Comprehensive logging for security events

### 📊 Database & Infrastructure
- ✅ **PostgreSQL Database** - Production-ready relational database
- ✅ **Entity Framework Core** - Code-first migrations and configurations
- ✅ **Repository Pattern** - Clean separation of data access
- ✅ **Database Seeding** - Initial roles and permissions
- ✅ **Docker Support** - Multi-stage Dockerfile and Docker Compose
- ✅ **Clean Architecture** - Well-organized layer structure

## 📊 GraphQL API

### Queries
- ✅ `health` - Health check endpoint
- ✅ `me` - Get current authenticated user
- ✅ `user(id: ID!)` - Get user by ID (requires `users:read`)
- ✅ `users` - Get all users (requires `users:read`)
- ✅ `sessions` - Get current user's sessions (requires authentication)
- ✅ `roles` - Get all roles (requires `roles:read`)
- ✅ `permissions` - Get all permissions (requires `permissions:read`)

### Mutations
- ✅ `registerUser` - User registration
- ✅ `login` - User login with credentials
- ✅ `refreshToken` - Refresh access token
- ✅ `verifyEmail` - Verify email with token
- ✅ `requestPasswordReset` - Request password reset
- ✅ `resetPassword` - Reset password with token
- ✅ `revokeSession` - Revoke specific session (requires authentication)
- ✅ `revokeAllSessions` - Revoke all sessions (requires authentication)
- ✅ `assignRole` - Assign role to user (requires `roles:assign`)
- ✅ `removeRole` - Remove role from user (requires `roles:remove`)

### Permissions
The system includes 13 granular permissions:
- **Users**: `users:create`, `users:read`, `users:update`, `users:delete`
- **Roles**: `roles:create`, `roles:read`, `roles:update`, `roles:delete`, `roles:assign`, `roles:remove`
- **Permissions**: `permissions:read`
- **Sessions**: `sessions:read`, `sessions:revoke`

### Default Roles
- **Admin** - Full system access with all permissions
- **User** - Read-only access to basic resources

## 📋 Roadmap

### Planned Features
- [ ] Change password for authenticated users
- [ ] Update user profile (name, etc.)
- [ ] Resend email verification
- [ ] Create/update/delete roles (admin only)
- [ ] Add/remove permissions to/from roles (admin only)
- [ ] User account suspension/activation
- [ ] Soft delete user accounts
- [ ] Two-factor authentication (2FA)
- [ ] OAuth2/OIDC integration
- [ ] Rate limiting
- [ ] API versioning

### Testing & Quality
- [ ] Unit tests for domain entities and value objects
- [ ] Integration tests for use cases
- [ ] API tests for GraphQL endpoints
- [ ] Performance testing
- [ ] Security audit

### DevOps & Monitoring
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Kubernetes deployment manifests
- [ ] Application monitoring (Prometheus/Grafana)
- [ ] Distributed tracing (OpenTelemetry)
- [ ] Centralized logging (ELK stack)

## 🏗️ Architecture Principles

### Clean Architecture
- **Domain Layer**: Core business logic, no dependencies on other layers
- **Application Layer**: Use cases and application logic, depends only on Domain
- **Infrastructure Layer**: External concerns, depends on Application and Domain
- **API Layer**: GraphQL endpoints, depends on Application

### Domain-Driven Design
- **Aggregates**: User, Role, Permission, Session
- **Value Objects**: Email, Password, PasswordHash, FullName
- **Domain Events**: Raised by entities for important domain actions
- **Result Pattern**: Type-safe error handling without exceptions

### Security Best Practices
- ✅ Strong password requirements (8+ chars, mixed case, digits, special chars)
- ✅ Password history tracking (prevent reuse of last 5 passwords)
- ✅ Account lockout after 5 failed login attempts (15-minute duration)
- ✅ Email verification required before login
- ✅ Secure password hashing (BCrypt with work factor 12)
- ✅ JWT access tokens (15-minute expiration) + refresh tokens (7-day expiration)
- ✅ Refresh token rotation (new tokens on each refresh)
- ✅ Session management with revocation support
- ✅ Permission-based authorization (13 granular permissions)
- ✅ IP address and user agent tracking for sessions
- ✅ Structured logging for security events

## 🗄️ Database Schema

The application uses PostgreSQL with the following main tables:
- **users** - User accounts with email, password hash, roles, and status
- **roles** - Role definitions with permission collections
- **permissions** - Permission definitions (resource:action pattern)
- **sessions** - Active user sessions with refresh tokens
- **password_history** - Historical password hashes for reuse prevention

All tables use UUIDs as primary keys and include proper indexes for performance.

## ⚙️ Configuration

### Environment Variables

The application can be configured using the following environment variables:

**Database:**
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string

**JWT:**
- `Jwt__Secret` - Secret key for JWT signing (min 32 characters)
- `Jwt__Issuer` - JWT issuer (default: "Orbit")
- `Jwt__Audience` - JWT audience (default: "Orbit")
- `Jwt__AccessTokenExpirationMinutes` - Access token expiration (default: 15)
- `Jwt__RefreshTokenExpirationDays` - Refresh token expiration (default: 7)

**Email:**
- Email service configuration (currently using console output for development)

### Docker Compose Configuration

The `docker-compose.yml` file includes:
- **PostgreSQL** database (port 5432)
- **Orbit API** (port 3000)
- Automatic database initialization and seeding
- Health checks for both services

## 🚦 Getting Started

### Prerequisites
- .NET 9.0 SDK
- Docker and Docker Compose
- Your favorite IDE (Visual Studio, Rider, VS Code)

### Quick Start with Docker

1. **Clone the repository**
```bash
git clone <repository-url>
cd Orbit
```

2. **Start the application**
```bash
docker compose up --build
```

The API will be available at `http://localhost:3000/graphql`

3. **Explore the GraphQL API**
- Open your browser to `http://localhost:3000/graphql`
- Use the GraphQL playground to explore the schema and run queries

### Development Setup

1. **Build the solution**
```bash
dotnet build
```

2. **Run migrations** (if needed)
```bash
dotnet ef database update --project Infrastructure --startup-project Api
```

3. **Run the API**
```bash
dotnet run --project Api
```

### Example Queries

**Register a new user:**
```graphql
mutation {
  registerUser(input: {
    email: "user@example.com"
    password: "SecurePass123!"
    firstName: "John"
    lastName: "Doe"
  }) {
    user {
      userId
      email
    }
    errors {
      code
      message
    }
  }
}
```

**Login:**
```graphql
mutation {
  login(input: {
    email: "user@example.com"
    password: "SecurePass123!"
  }) {
    accessToken
    refreshToken
    expiresIn
    errors {
      code
      message
    }
  }
}
```

**Get current user:**
```graphql
query {
  me {
    userId
    email
    firstName
    lastName
    roles
  }
}
```

**Get all roles (requires admin):**
```graphql
query {
  roles {
    roles {
      roleId
      name
      description
      permissionIds
    }
    errors {
      code
      message
    }
  }
}
```

## 📝 License

[Add your license here]

## 🤝 Contributing

[Add contributing guidelines here]

