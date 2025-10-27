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
- ✅ **User Queries** - Get current user, user by ID, all users (with pagination)
- ✅ **User Sessions** - View and manage active sessions
- ✅ **Session Revocation** - Revoke individual or all sessions
- ✅ **User Suspension** - Suspend user accounts (admin only)
- ✅ **User Activation** - Activate suspended user accounts (admin only)
- ✅ **User Deletion** - Soft delete user accounts (admin only)
- ✅ **Account Unlock** - Manually unlock locked user accounts (admin only)

### 🎭 Role & Permission Management
- ✅ **Role Queries** - View all roles and their permissions
- ✅ **Permission Queries** - View all available permissions
- ✅ **Role Assignment** - Assign roles to users (requires `roles:assign`)
- ✅ **Role Removal** - Remove roles from users (requires `roles:remove`)

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
- ✅ **Database Seeding** - Initial roles, permissions, and admin user
- ✅ **Docker Support** - Multi-stage Dockerfile and Docker Compose
- ✅ **Clean Architecture** - Well-organized layer structure
- ✅ **Structured Logging** - Serilog with Seq integration
- ✅ **Health Checks** - Comprehensive health monitoring with database connectivity

## 📊 GraphQL API

### Queries
- ✅ `health` - Comprehensive health check with database connectivity, environment, version, and uptime
- ✅ `me` - Get current authenticated user with roles and permissions
- ✅ `user(id: ID!)` - Get user by ID (requires `users:read`)
- ✅ `users(page: Int, pageSize: Int)` - Get all users with pagination (requires `users:read`)
- ✅ `sessions` - Get current user's active sessions (requires authentication)
- ✅ `roles` - Get all roles with permissions (requires `roles:read`)
- ✅ `permissions` - Get all available permissions (requires `permissions:read`)

### Mutations

#### Authentication & Registration
- ✅ `registerUser` - Register new user (requires `users:create`)
- ✅ `login` - User login with credentials (returns JWT tokens)
- ✅ `refreshToken` - Refresh access token using refresh token
- ✅ `verifyEmail` - Verify email address with token
- ✅ `requestPasswordReset` - Request password reset email
- ✅ `resetPassword` - Reset password with token

#### Session Management
- ✅ `revokeSession` - Revoke specific session (requires authentication)
- ✅ `revokeAllSessions` - Revoke all user sessions (requires authentication)

#### User Management (Admin)
- ✅ `suspendUser` - Suspend user account (requires `users:suspend`)
- ✅ `activateUser` - Activate suspended account (requires `users:activate`)
- ✅ `deleteUser` - Soft delete user account (requires `users:delete`)
- ✅ `unlockUserAccount` - Unlock locked account (requires `users:unlock`)

#### Role Management (Admin)
- ✅ `assignRole` - Assign role to user (requires `roles:assign`)
- ✅ `removeRole` - Remove role from user (requires `roles:remove`)

### Permissions
The system includes 17 granular permissions organized by resource:

- **Users** (7): `users:create`, `users:read`, `users:update`, `users:delete`, `users:suspend`, `users:activate`, `users:unlock`
- **Roles** (6): `roles:create`, `roles:read`, `roles:update`, `roles:delete`, `roles:assign`, `roles:remove`
- **Permissions** (1): `permissions:read`
- **Sessions** (3): `sessions:read`, `sessions:revoke`, `sessions:delete`

### Default Roles
- **Admin** - Full system access with all 17 permissions
- **User** - Read-only access to basic resources (users:read, roles:read, permissions:read, sessions:read)

### Default Admin Account
The system automatically seeds a default admin account in development:
- **Email**: `admin@orbit.local`
- **Password**: `Admin123!`
- **Role**: Admin (all permissions)
- **Status**: Active, Email Verified

⚠️ **Important**: Change this password immediately in production environments!

## 📋 Roadmap

### Planned Features
- [ ] Change password for authenticated users
- [ ] Update user profile (name, email)
- [ ] Resend email verification
- [ ] Create/update/delete roles (admin only)
- [ ] Add/remove individual permissions to/from roles (admin only)
- [ ] Two-factor authentication (2FA)
- [ ] OAuth2/OIDC integration (Google, GitHub, etc.)
- [ ] Rate limiting and throttling
- [ ] API versioning
- [ ] Audit logging for sensitive operations
- [ ] User activity tracking
- [ ] Email templates and customization

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
- **users** - User accounts with email, password hash, role, status, and lockout tracking
- **roles** - Role definitions with permission collections (many-to-many with permissions)
- **permissions** - Permission definitions using resource:action pattern
- **sessions** - Active user sessions with refresh tokens, IP address, and user agent
- **password_history** - Historical password hashes for reuse prevention (last 5 passwords)

All tables use UUIDs as primary keys and include proper indexes for performance. The schema supports:
- One-to-many relationship between users and sessions
- Many-to-many relationship between roles and permissions
- One-to-many relationship between users and password history
- Soft deletes for users (status-based)

## ⚙️ Configuration

### Environment Variables

The application can be configured using the following environment variables:

**Database:**
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string

**JWT:**
- `Jwt__Secret` - Secret key for JWT signing (min 32 characters, required)
- `Jwt__Issuer` - JWT issuer (default: "Orbit")
- `Jwt__Audience` - JWT audience (default: "Orbit")
- `Jwt__AccessTokenExpirationMinutes` - Access token expiration (default: 60 minutes)
- `Jwt__RefreshTokenExpirationDays` - Refresh token expiration (default: 7 days)

**Logging:**
- `Seq__ServerUrl` - Seq server URL for structured logging (default: "http://localhost:5341")

**Email:**
- Email service configuration (currently using console output for development)

### Docker Compose Configuration

The `compose.yaml` file includes:
- **PostgreSQL** database (port 5432) with health checks
- **Seq** logging server (port 5341) for structured logs
- **Papercut SMTP** (ports 25, 8080) for email testing
- **Orbit API** (port 3000) with automatic migrations and seeding
- Persistent volumes for data storage
- Dedicated network for service communication

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

The services will be available at:
- **GraphQL API**: http://localhost:3000/graphql
- **Seq Logs**: http://localhost:5341 (username: admin, password: admin)
- **Papercut SMTP**: http://localhost:8080 (email testing)

3. **Explore the GraphQL API**
- Open your browser to `http://localhost:3000/graphql`
- Use the GraphQL playground to explore the schema and run queries
- Login with the default admin account: `admin@orbit.local` / `Admin123!`

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

**Health Check:**
```graphql
query {
  health {
    status
    timestamp
    environment
    database
    version
    uptimeSeconds
    details
  }
}
```

**Register a new user:**
```graphql
mutation {
  registerUser(input: {
    email: "user@example.com"
    password: "SecurePass123!"
    firstName: "John"
    lastName: "Doe"
    roleId: "00000000-0000-0000-0000-000000000002"  # User role ID
  }) {
    user {
      userId
      email
      firstName
      lastName
      isEmailVerified
      status
      createdAt
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
    user {
      userId
      email
      firstName
      lastName
    }
    accessToken
    refreshToken
    expiresAt
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
    isEmailVerified
    status
    roles
    permissions
    createdAt
    lastLoginAt
  }
}
```

**Get all users with pagination (requires users:read):**
```graphql
query {
  users(page: 1, pageSize: 10) {
    users {
      userId
      email
      firstName
      lastName
      status
      createdAt
    }
    totalCount
    page
    pageSize
    totalPages
    errors {
      code
      message
    }
  }
}
```

**Get all roles (requires roles:read):**
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

**Suspend user (requires users:suspend):**
```graphql
mutation {
  suspendUser(input: {
    userId: "00000000-0000-0000-0000-000000000001"
  }) {
    isSuccess
    message
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

