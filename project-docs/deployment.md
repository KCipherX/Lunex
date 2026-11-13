# Lunex Deployment Guide

## Prerequisites

### Required Software
- **.NET 10.0 SDK** (or later)
- **PostgreSQL 15+** (or compatible)
- **Node.js 20+** (for Angular frontend)
- **Docker** (optional, for containerized deployment)

### Required Accounts
- **Cloudinary** account for photo storage
- **PostgreSQL** database (local or cloud)

## Environment Configuration

### API Configuration (`src/Lunex.Api/appsettings.json`)

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Database=lunex;Username=postgres;Password=your-password"
  },
  "CorsSettings": {
    "AllowedOrigins": [
      "https://your-frontend-domain.com"
    ]
  },
  "JwtSettings": {
    "SecretKey": "YOUR-VERY-LONG-SECURE-KEY-AT-LEAST-32-CHARACTERS",
    "Issuer": "lunex.api",
    "Audience": "lunex.client",
    "ExpiryMinutes": 30
  },
  "CloudinarySettings": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Frontend Configuration (`client/src/environments/environment.ts`)

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.yourdomain.com/api'
};
```

## Database Setup

### 1. Create Database
```sql
CREATE DATABASE lunex;
CREATE USER lunex_user WITH ENCRYPTED PASSWORD 'secure-password';
GRANT ALL PRIVILEGES ON DATABASE lunex TO lunex_user;
```

### 2. Run Migrations
```bash
# From solution root
dotnet ef database update --project src/Lunex.Infrastructure --startup-project src/Lunex.Api
```

### 3. Verify Schema
```sql
\c lunex
\dn+ lunex
\dt lunex.*
```

## Build & Publish

### API (Backend)

```bash
# Restore dependencies
dotnet restore Lunex.sln

# Build in Release mode
dotnet build Lunex.sln -c Release

# Publish API
dotnet publish src/Lunex.Api/Lunex.Api.csproj -c Release -o ./publish/api

# Run published API
cd ./publish/api
dotnet Lunex.Api.dll
```

### Frontend (Angular)

```bash
cd client

# Install dependencies
npm ci

# Build for production
npm run build

# Output: client/dist/lunex-client/browser/
```

## Docker Deployment

### Dockerfile (API)
```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Lunex.sln", "."]
COPY ["src/Lunex.Api/Lunex.Api.csproj", "src/Lunex.Api/"]
COPY ["src/Lunex.Application/Lunex.Application.csproj", "src/Lunex.Application/"]
COPY ["src/Lunex.Contracts/Lunex.Contracts.csproj", "src/Lunex.Contracts/"]
COPY ["src/Lunex.Domain/Lunex.Domain.csproj", "src/Lunex.Domain/"]
COPY ["src/Lunex.Infrastructure/Lunex.Infrastructure.csproj", "src/Lunex.Infrastructure/"]
RUN dotnet restore "Lunex.sln"

COPY . .
WORKDIR "/src/src/Lunex.Api"
RUN dotnet publish "Lunex.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Lunex.Api.dll"]
```

### Dockerfile (Frontend)
```dockerfile
# Build stage
FROM node:20-alpine AS build
WORKDIR /app
COPY client/package*.json ./
RUN npm ci
COPY client/ .
RUN npm run build

# Runtime stage (nginx)
FROM nginx:alpine
COPY --from=build /app/dist/lunex-client/browser /usr/share/nginx/html
COPY client/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### docker-compose.yml
```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15-alpine
    environment:
      POSTGRES_DB: lunex
      POSTGRES_USER: lunex_user
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U lunex_user -d lunex"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    build:
      context: .
      dockerfile: Dockerfile.api
    environment:
      ConnectionStrings__Database: "Host=postgres;Port=5432;Database=lunex;Username=lunex_user;Password=${DB_PASSWORD}"
      CorsSettings__AllowedOrigins__0: "https://${FRONTEND_DOMAIN}"
      JwtSettings__SecretKey: ${JWT_SECRET}
      JwtSettings__Issuer: "lunex.api"
      JwtSettings__Audience: "lunex.client"
      JwtSettings__ExpiryMinutes: "30"
      CloudinarySettings__CloudName: ${CLOUDINARY_CLOUD_NAME}
      CloudinarySettings__ApiKey: ${CLOUDINARY_API_KEY}
      CloudinarySettings__ApiSecret: ${CLOUDINARY_API_SECRET}
      ASPNETCORE_ENVIRONMENT: Production
      ASPNETCORE_URLS: "http://+:8080"
    ports:
      - "5001:8080"
    depends_on:
      postgres:
        condition: service_healthy
    restart: unless-stopped

  frontend:
    build:
      context: .
      dockerfile: Dockerfile.frontend
    ports:
      - "80:80"
      - "443:443"
    depends_on:
      - api
    restart: unless-stopped

volumes:
  postgres_data:
```

## Environment Variables

Create `.env` file for docker-compose:

```env
# Database
DB_PASSWORD=your-secure-db-password

# JWT
JWT_SECRET=your-very-long-secure-key-at-least-32-characters

# Cloudinary
CLOUDINARY_CLOUD_NAME=your-cloud-name
CLOUDINARY_API_KEY=your-api-key
CLOUDINARY_API_SECRET=your-api-secret

# Domains
FRONTEND_DOMAIN=yourdomain.com
```

## Deployment Commands

### Using Docker Compose
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down

# Rebuild and restart
docker-compose up -d --build
```

### Manual Deployment (Linux/Windows Server)

#### API as Systemd Service (Linux)
```ini
# /etc/systemd/system/lunex-api.service
[Unit]
Description=Lunex API
After=network.target postgresql.service

[Service]
Type=notify
WorkingDirectory=/var/www/lunex/api
ExecStart=/usr/bin/dotnet /var/www/lunex/api/Lunex.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=lunex-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5001
Environment=ConnectionStrings__Database=Host=localhost;Port=5432;Database=lunex;Username=lunex_user;Password=your-password
Environment=JwtSettings__SecretKey=your-secret-key
Environment=CloudinarySettings__CloudName=your-cloud-name
Environment=CloudinarySettings__ApiKey=your-api-key
Environment=CloudinarySettings__ApiSecret=your-api-secret

[Install]
WantedBy=multi-user.target
```

```bash
# Enable and start
sudo systemctl daemon-reload
sudo systemctl enable lunex-api
sudo systemctl start lunex-api
sudo systemctl status lunex-api
```

#### Nginx Reverse Proxy
```nginx
# /etc/nginx/sites-available/lunex
server {
    listen 80;
    server_name api.yourdomain.com;

    location / {
        proxy_pass http://localhost:5001;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

server {
    listen 80;
    server_name yourdomain.com;

    root /var/www/lunex/frontend;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://localhost:5001;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

## SSL/TLS Configuration

### Let's Encrypt (Certbot)
```bash
# Install certbot
sudo apt install certbot python3-certbot-nginx

# Obtain certificates
sudo certbot --nginx -d api.yourdomain.com -d yourdomain.com

# Auto-renewal
sudo certbot renew --dry-run
```

## Health Checks

### API Health Endpoint
Add to `Program.cs`:
```csharp
app.MapHealthChecks("/health");
```

### Database Health Check
```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);
```

## Monitoring & Logging

### Structured Logging (Serilog)
```json
// appsettings.Production.json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "/var/log/lunex/api-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

## Backup Strategy

### Database Backup
```bash
# Daily backup script
#!/bin/bash
BACKUP_DIR="/var/backups/lunex"
DATE=$(date +%Y%m%d_%H%M%S)
pg_dump -h localhost -U lunex_user -d lunex > "$BACKUP_DIR/lunex_$DATE.sql"
gzip "$BACKUP_DIR/lunex_$DATE.sql"
find "$BACKUP_DIR" -name "lunex_*.sql.gz" -mtime +30 -delete
```

### Cloudinary Backup
- Enable Cloudinary backup in dashboard
- Or use Cloudinary API to sync assets

## Rollback Procedure

### API Rollback
```bash
# Keep previous version
mv /var/www/lunex/api /var/www/lunex/api.backup.$(date +%Y%m%d)
# Deploy new version
# If issues: mv /var/www/lunex/api.backup.* /var/www/lunex/api
```

### Database Rollback
```bash
# Restore from backup
gunzip -c /var/backups/lunex/lunex_20260115_020000.sql.gz | psql -h localhost -U lunex_user -d lunex
```

## CI/CD Pipeline (GitHub Actions Example)

```yaml
# .github/workflows/deploy.yml
name: Deploy to Production

on:
  push:
    branches: [main]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore
        run: dotnet restore Lunex.sln

      - name: Build
        run: dotnet build Lunex.sln -c Release --no-restore

      - name: Test
        run: dotnet test Lunex.sln -c Release --no-build

      - name: Publish API
        run: dotnet publish src/Lunex.Api -c Release -o ./publish/api

      - name: Build Frontend
        run: |
          cd client
          npm ci
          npm run build

      - name: Deploy to Server
        uses: appleboy/ssh-action@v1
        with:
          host: ${{ secrets.SERVER_HOST }}
          username: ${{ secrets.SERVER_USER }}
          key: ${{ secrets.SERVER_SSH_KEY }}
          script: |
            cd /var/www/lunex
            # Backup current
            mv api api.backup.$(date +%Y%m%d)
            # Deploy new
            # ... copy files via scp or rsync
            systemctl restart lunex-api
```

## Troubleshooting

### Common Issues

| Issue | Solution |
|-------|----------|
| Database connection failed | Check connection string, firewall, PostgreSQL running |
| JWT validation failed | Verify SecretKey matches, check clock sync |
| CORS errors | Verify AllowedOrigins includes frontend domain |
| Cloudinary upload fails | Check credentials, folder permissions |
| Migration fails | Ensure database exists, user has CREATE privileges |

### Logs Location
- **Docker**: `docker-compose logs -f api`
- **Systemd**: `journalctl -u lunex-api -f`
- **File**: `/var/log/lunex/api-*.log`

## Security Checklist

- [ ] Strong JWT SecretKey (32+ chars, random)
- [ ] HTTPS enforced (HSTS, redirect)
- [ ] CORS restricted to known domains
- [ ] Database credentials rotated regularly
- [ ] Cloudinary credentials secured
- [ ] Rate limiting implemented
- [ ] Security headers configured (CSP, HSTS, etc.)
- [ ] Regular dependency updates (`dotnet list package --vulnerable`)
- [ ] Secrets in environment variables, not config files
- [ ] Database backups encrypted