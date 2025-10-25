# Docker Setup Guide

This guide explains how to run the Orbit API using Docker Compose.

## Prerequisites

- Docker Desktop (or Docker Engine + Docker Compose)
- At least 2GB of free disk space

## Quick Start

### 1. Start all services

```bash
docker compose up -d
```

This will:
- Start PostgreSQL database on port 5432
- Build and start the API on port 3000
- Create a Docker network for service communication
- Create a persistent volume for PostgreSQL data

### 2. View logs

```bash
# View all logs
docker compose logs -f

# View API logs only
docker compose logs -f api

# View PostgreSQL logs only
docker compose logs -f postgres
```

### 3. Stop all services

```bash
docker compose down
```

### 4. Stop and remove volumes (clean slate)

```bash
docker compose down -v
```

## Available Commands

### Build and start services
```bash
docker compose up --build
```

### Rebuild API only
```bash
docker compose build api
docker compose up -d api
```

### Access PostgreSQL database
```bash
docker compose exec postgres psql -U postgres -d orbit
```

### Run migrations manually
```bash
docker compose exec api dotnet ef database update --project /app/Infrastructure.dll
```

### View running containers
```bash
docker compose ps
```

### Restart a service
```bash
docker compose restart api
```

## Service Details

### PostgreSQL
- **Image**: postgres:16-alpine
- **Port**: 5432
- **Database**: orbit
- **Username**: postgres
- **Password**: postgres
- **Volume**: postgres_data (persistent)

### API
- **Port**: 3000
- **GraphQL Endpoint**: http://localhost:3000/graphql
- **Health Check**: http://localhost:3000/health
- **Environment**: Development

## Accessing the API

Once the services are running:

1. **GraphQL Playground**: http://localhost:3000/graphql
2. **Test Registration**:
   ```graphql
   mutation {
     registerUser(
       input: {
         email: "test@example.com"
         password: "SecurePassword123!"
         firstName: "Test"
         lastName: "User"
       }
     ) {
       user {
         id
         email
         firstName
         lastName
         isEmailVerified
         createdAt
       }
       errors {
         code
         message
       }
     }
   }
   ```

## Troubleshooting

### Port already in use
If port 3000 or 5432 is already in use, you can change the ports in `compose.yaml`:

```yaml
services:
  postgres:
    ports:
      - "5433:5432"  # Change host port
  
  api:
    ports:
      - "3001:8080"  # Change host port
```

### Database connection issues
Check if PostgreSQL is healthy:
```bash
docker compose ps
```

The postgres service should show "healthy" status.

### API not starting
Check the logs:
```bash
docker compose logs api
```

Common issues:
- Database not ready: Wait for PostgreSQL health check to pass
- Build errors: Run `docker compose build --no-cache api`
- Migration errors: Check database connection string

### Clean rebuild
If you encounter persistent issues:
```bash
# Stop and remove everything
docker compose down -v

# Remove all images
docker compose down --rmi all

# Rebuild from scratch
docker compose up --build
```

## Development Workflow

### Making code changes

1. Make your code changes
2. Rebuild and restart:
   ```bash
   docker compose up --build -d
   ```

### Database migrations

After creating a new migration:
```bash
# Rebuild the API image
docker compose build api

# Restart the API (migrations run automatically on startup)
docker compose up -d api
```

### Viewing database data

```bash
# Connect to PostgreSQL
docker compose exec postgres psql -U postgres -d orbit

# List tables
\dt

# Query users
SELECT * FROM users;

# Exit
\q
```

## Production Considerations

For production deployment:

1. **Use secrets** instead of hardcoded passwords
2. **Use environment-specific** compose files
3. **Enable SSL/TLS** for PostgreSQL
4. **Use a reverse proxy** (nginx, Traefik) for the API
5. **Set up proper logging** and monitoring
6. **Use health checks** for all services
7. **Implement backup strategy** for PostgreSQL data

## Network Architecture

```
┌─────────────────────────────────────┐
│         orbit-network (bridge)      │
│                                     │
│  ┌──────────────┐  ┌─────────────┐ │
│  │  PostgreSQL  │  │     API     │ │
│  │   :5432      │◄─┤   :8080     │ │
│  └──────────────┘  └─────────────┘ │
│         │                  │        │
└─────────┼──────────────────┼────────┘
          │                  │
     Host :5432         Host :3000
```

## Volume Management

### Backup database
```bash
docker compose exec postgres pg_dump -U postgres orbit > backup.sql
```

### Restore database
```bash
cat backup.sql | docker compose exec -T postgres psql -U postgres orbit
```

### Inspect volume
```bash
docker volume inspect orbit_postgres_data
```

