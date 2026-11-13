# Lunex Development Setup

## Prerequisites

### Required Software
| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 10.0+ | Backend development |
| PostgreSQL | 15+ | Database |
| Node.js | 20+ | Frontend development |
| Git | Latest | Version control |
| IDE | VS Code / Visual Studio 2022 / Rider | Development |

### Recommended Extensions (VS Code)
- C# Dev Kit
- C# Extensions
- ES7+ React/Redux/React-Native snippets
- Angular Language Service
- Tailwind CSS IntelliSense
- PostgreSQL (for database management)

## Initial Setup

### 1. Clone Repository
```bash
git clone https://github.com/your-org/lunex.git
cd lunex
```

### 2. Database Setup

#### Option A: Local PostgreSQL
```bash
# Install PostgreSQL (macOS)
brew install postgresql@15
brew services start postgresql@15

# Install PostgreSQL (Ubuntu)
sudo apt update && sudo apt install postgresql-15

# Install PostgreSQL (Windows)
# Download from https://www.postgresql.org/download/windows/

# Create database and user
psql -U postgres
CREATE DATABASE lunex;
CREATE USER lunex_dev WITH ENCRYPTED PASSWORD 'dev-password';
GRANT ALL PRIVILEGES ON DATABASE lunex TO lunex_dev;
\q
```

#### Option B: Docker PostgreSQL (Recommended)
```bash
docker run -d \
  --name lunex-postgres \
  -e POSTGRES_DB=lunex \
  -e POSTGRES_USER=lunex_dev \
  -e POSTGRES_PASSWORD=dev-password \
  -p 5432:5432 \
  -v lunex-pgdata:/var/lib/postgresql/data \
  postgres:15-alpine
```

### 3. Backend Configuration

#### Create Development Settings
```bash
# Copy template
cp src/Lunex.Api/appsettings.Development.json.template src/Lunex.Api/appsettings.Development.json
```

#### Update `appsettings.Development.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Database=lunex;Username=lunex_dev;Password=dev-password"
  },
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:4200",
      "https://localhost:4200"
    ]
  },
  "JwtSettings": {
    "SecretKey": "DEVELOPMENT-ONLY-SECRET-KEY-MUST-BE-32-CHARS-MINIMUM",
    "Issuer": "lunex.api",
    "Audience": "lunex.client",
    "ExpiryMinutes": 30
  },
  "CloudinarySettings": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

### 4. Frontend Configuration

```bash
cd client

# Install dependencies
npm install

# Create environment file (optional - uses defaults)
cp src/environments/environment.ts.template src/environments/environment.ts
```

#### Default Frontend Environment (`client/src/environments/environment.ts`)
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api'
};
```

## Running the Application

### 1. Start Backend API
```bash
# From solution root
dotnet run --project src/Lunex.Api

# Or with hot reload
dotnet watch --project src/Lunex.Api
```

**API will be available at:**
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Scalar Docs: `https://localhost:5001/scalar/v1`
- OpenAPI JSON: `https://localhost:5001/openapi/v1.json`

### 2. Start Frontend (New Terminal)
```bash
cd client
npm start
```

**Frontend will be available at:**
- `http://localhost:4200`

### 3. Verify Setup
1. Open `http://localhost:4200` - should see Lunex app
2. Register a new user
3. Login and verify member list loads
4. Check Scalar docs at `https://localhost:5001/scalar/v1`

## Development Workflow

### Database Migrations

#### Create New Migration
```bash
# From solution root
dotnet ef migrations add MigrationName \
  --project src/Lunex.Infrastructure \
  --startup-project src/Lunex.Api \
  -o Persistance/Migrations
```

#### Apply Migrations
```bash
dotnet ef database update \
  --project src/Lunex.Infrastructure \
  --startup-project src/Lunex.Api
```

#### Remove Last Migration (if not applied)
```bash
dotnet ef migrations remove \
  --project src/Lunex.Infrastructure \
  --startup-project src/Lunex.Api
```

#### Reset Database (Development Only)
```bash
# Drop and recreate
dotnet ef database drop --force \
  --project src/Lunex.Infrastructure \
  --startup-project src/Lunex.Api

dotnet ef database update \
  --project src/Lunex.Infrastructure \
  --startup-project src/Lunex.Api
```

### Running Tests

```bash
# Run all tests
dotnet test Lunex.sln

# Run with coverage
dotnet test Lunex.sln --collect:"XPlat Code Coverage"

# Run specific project tests
dotnet test src/Lunex.Api.Tests/
dotnet test src/Lunex.Application.Tests/
dotnet test src/Lunex.Infrastructure.Tests/
```

### Code Formatting

```bash
# Format all code
dotnet format Lunex.sln

# Check formatting (CI)
dotnet format Lunex.sln --verify-no-changes
```

### Linting (Frontend)
```bash
cd client
npm run lint
npm run format
```

## Project Structure Deep Dive

### Adding a New Feature

#### 1. Domain Layer (if new entity)
```csharp
// src/Lunex.Domain/Entities/NewEntity/NewEntity.cs
namespace Lunex.Domain.Entities.NewEntity;

public sealed class NewEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    // Properties...
}
```

#### 2. Contracts Layer (API DTOs)
```csharp
// src/Lunex.Contracts/NewFeature/Requests/NewRequest.cs
namespace Lunex.Contracts.NewFeature.Requests;

public sealed record NewRequest(
    [Required] string Property1,
    [Required, MaxLength(100)] string Property2
);

// src/Lunex.Contracts/NewFeature/Responses/NewResponse.cs
namespace Lunex.Contracts.NewFeature.Responses;

public sealed record NewResponse(
    string Id,
    string Property1,
    string Property2
);
```

#### 3. Application Layer
```csharp
// Abstractions
// src/Lunex.Application/NewFeature/Persistence/Abstractions/INewRepository.cs
// src/Lunex.Application/NewFeature/Services/Abstractions/INewService.cs

// Implementations
// src/Lunex.Application/NewFeature/Services/Implementations/NewService.cs
// src/Lunex.Application/NewFeature/Mappers/NewMapper.cs
```

#### 4. Infrastructure Layer
```csharp
// src/Lunex.Infrastructure/Persistence/Repositories/NewRepository.cs
// Add DbSet to ApplicationDbContext
// Create migration
```

#### 5. API Layer
```csharp
// src/Lunex.Api/Controllers/NewController.cs
[Route("api/new")]
public sealed class NewController(INewService service) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await service.GetAsync(ct));
}
```

#### 6. Register in DI
```csharp
// Lunex.Application.DependencyInjection.cs
services.AddScoped<INewService, NewService>();
services.AddScoped<INewRepository, NewRepository>();

// Lunex.Infrastructure.DependencyInjection.cs (if needed)
```

## Debugging

### Backend Debugging (VS Code)
```json
// .vscode/launch.json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Launch Lunex API",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/src/Lunex.Api/bin/Debug/net10.0/Lunex.Api.dll",
      "args": [],
      "cwd": "${workspaceFolder}/src/Lunex.Api",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)",
        "uriFormat": "%s/scalar/v1"
      },
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "sourceFileMap": {
        "/Views": "${workspaceFolder}/Views"
      }
    }
  ]
}
```

### Frontend Debugging (VS Code)
```json
// client/.vscode/launch.json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Launch Angular",
      "type": "chrome",
      "request": "launch",
      "url": "http://localhost:4200",
      "webRoot": "${workspaceFolder}/client/src"
    }
  ]
}
```

### Database Debugging
```bash
# Connect to database
psql -h localhost -U lunex_dev -d lunex

# Useful queries
\dt lunex.*                    -- List tables
\d+ lunex.users                -- Describe table
SELECT * FROM lunex.users;     -- View data
SELECT * FROM lunex.members;   -- View members
```

## Common Development Tasks

### Seed Development Data
```bash
# Automatic on API startup in Development
# Or manually:
dotnet run --project src/Lunex.Api -- --seed
```

### Reset User Password (Database)
```sql
-- Generate new hash (use AccountController logic or)
UPDATE lunex.users 
SET password_hash = '\x...', password_salt = '\x...' 
WHERE email = 'user@example.com';
```

### View JWT Token Claims
```csharp
// In controller or service
var claims = User.Claims.Select(c => new { c.Type, c.Value });
// Or use ClaimsPrincipalExtensions
var userId = User.GetRequiredUserId();
var email = User.GetEmail();
```

### Test API Endpoints
```bash
# Use HTTP files in https/ folder
# Or curl:
curl -X POST https://localhost:5001/api/account/register \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","email":"test@test.com","password":"Test@123","gender":"Male","city":"NYC","country":"USA","dateOfBirth":"1990-01-01"}'

curl -X POST https://localhost:5001/api/account/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test@123"}'
```

## Troubleshooting

### Port Already in Use
```bash
# Find process on port 5001
lsof -i :5001  # macOS/Linux
netstat -ano | findstr :5001  # Windows

# Kill process
kill -9 <PID>  # macOS/Linux
taskkill /PID <PID> /F  # Windows
```

### Database Connection Failed
```bash
# Check PostgreSQL running
brew services list | grep postgresql  # macOS
sudo systemctl status postgresql  # Linux

# Check connection string in appsettings.Development.json
# Verify user/password/database exist
```

### Migration Errors
```bash
# Check pending migrations
dotnet ef migrations list --project src/Lunex.Infrastructure --startup-project src/Lunex.Api

# If schema mismatch, reset database (dev only)
dotnet ef database drop --force --project src/Lunex.Infrastructure --startup-project src/Lunex.Api
dotnet ef database update --project src/Lunex.Infrastructure --startup-project src/Lunex.Api
```

### Frontend Build Errors
```bash
cd client
rm -rf node_modules package-lock.json
npm install
npm run build
```

### CORS Errors
- Verify `CorsSettings.AllowedOrigins` includes `http://localhost:4200`
- Check browser console for exact error
- Ensure API and frontend ports match configuration

## Useful Commands Reference

```bash
# Solution-wide
dotnet restore Lunex.sln
dotnet build Lunex.sln
dotnet test Lunex.sln
dotnet format Lunex.sln

# API specific
dotnet run --project src/Lunex.Api
dotnet watch --project src/Lunex.Api
dotnet ef migrations add Name --project src/Lunex.Infrastructure --startup-project src/Lunex.Api
dotnet ef database update --project src/Lunex.Infrastructure --startup-project src/Lunex.Api

# Frontend specific
cd client
npm start
npm run build
npm run test
npm run lint

# Docker
docker-compose up -d
docker-compose logs -f api
docker-compose down -v  # Remove volumes too
```

## IDE Tips

### Visual Studio 2022
- Right-click solution → "Configure Startup Projects" → Multiple startup projects
- Set both API and Frontend to "Start"
- Use "Hot Reload" for both .NET and Angular

### VS Code
- Install "C# Dev Kit" for full IntelliSense
- Use "Tasks: Run Build Task" (Ctrl+Shift+B)
- Debug both frontend and backend simultaneously with compound launch config

### Rider
- Automatic run configuration detection
- Built-in database tools (connect to PostgreSQL)
- Excellent refactoring support

## Performance Profiling

### Backend
```bash
# Add to launch settings for profiling
"environmentVariables": {
  "DOTNET_EnableDiagnostics": "1"
}

# Use dotnet-trace
dotnet trace collect --process-id <PID> --providers Microsoft-AspNetCore-Hosting
```

### Database
```sql
-- Enable query logging in PostgreSQL
ALTER SYSTEM SET log_statement = 'all';
ALTER SYSTEM SET log_duration = on;
SELECT pg_reload_conf();
```

## Contributing Guidelines

### Branch Naming
- `feature/description` - New features
- `fix/description` - Bug fixes
- `refactor/description` - Code improvements
- `docs/description` - Documentation updates

### Commit Messages
```
type(scope): description

[optional body]

[optional footer]
```

Types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

### Pull Request Checklist
- [ ] Code builds without warnings
- [ ] All tests pass
- [ ] Code formatted (`dotnet format`)
- [ ] No linting errors (frontend)
- [ ] Documentation updated
- [ ] Migration included (if schema changed)
- [ ] Breaking changes noted

## Resources

- [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)
- [.NET 10 Documentation](https://learn.microsoft.com/dotnet/)
- [EF Core Documentation](https://learn.microsoft.com/ef/core/)
- [Angular 20 Documentation](https://angular.io/docs)
- [TailwindCSS Documentation](https://tailwindcss.com/docs)
- [Cloudinary Documentation](https://cloudinary.com/documentation)
- [Scalar OpenAPI](https://github.com/scalar/scalar)