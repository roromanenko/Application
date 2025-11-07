# Organiza - Event Management System

A full-stack event management application built with .NET 9, Angular 20, and PostgreSQL.

## 🏗️ Architecture

- Backend .NET 9 Web API with JWT authentication
- Frontend Angular 20 with TypeScript
- Database PostgreSQL 16
- Containerization Docker & Docker Compose

## 📋 Prerequisites

Before you begin, ensure you have the following installed

- [Docker Desktop](httpswww.docker.comproductsdocker-desktop) (4.0 or higher)
- [Git](httpsgit-scm.comdownloads)

Note You don't need to install .NET SDK, Node.js, or PostgreSQL locally - everything runs in Docker containers.

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone your-repository-url
cd Event Management System
```

### 2. Launch the Application

Run the following command in the project root directory
```bash
docker-compose up --build
```

This single command will
- ✅ Start a PostgreSQL database container
- ✅ Build and run the .NET backend API
- ✅ Build and run the Angular frontend
- ✅ Apply database migrations automatically
- ✅ Set up networking between all services

### 3. Access the Application

Once all containers are running, access the application at

 Service  URL  Description 
---------------------------
 Frontend  httplocalhost4200  Angular application 
 Backend API  httplocalhost5000  REST API endpoints 
 Swagger UI  httplocalhost5000swagger  API documentation 
 Health Check  httplocalhost5000health  Service health status 

### 4. Default Credentials

Development Database
- Host `localhost5432`
- Database `organiza_dev`
- Username `postgres`
- Password `postgres_dev_password`

Test User (if seed data is available)
- Email `test@example.com`
- Password `Test123!`

## 📦 Project Structure
```
Event Management System
├── Api                        # .NET Web API project
│   ├── Controllers           # API controllers
│   ├── Extensions            # Service configuration
│   ├── Dockerfile             # Backend container config
│   └── appsettings.json       # API configuration
├── Core                       # Business logic layer
├── Infrastructure             # Data access layer
│   ├── Persistence           # DbContext & Entities
│   └── Repositories          # Data repositories
├── Frontend                   # Angular application
│   ├── src                   # Source code
│   ├── Dockerfile             # Frontend container config
│   └── proxy.conf.json        # Development proxy config
├── docker-compose.yml          # Docker orchestration
├── docker-compose.override.yml # Development overrides
└── README.md                   # This file
```

## 🛠️ Development

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f postgres
```

### Stop the Application
```bash
docker-compose down
```

### Rebuild After Code Changes
```bash
docker-compose up --build
```

### Access Database
```bash
# Connect to PostgreSQL container
docker-compose exec postgres psql -U postgres -d organiza_dev
```

## 🧪 API Testing

### Using Swagger UI

1. Navigate to httplocalhost5000swagger
2. Test endpoints interactively with the Swagger interface

### Using curl
```bash
# Health check
curl httplocalhost5000health

# Register a new user
curl -X POST httplocalhost5000apiUserregister 
  -H Content-Type applicationjson 
  -d '{
    email user@example.com,
    password SecurePass123!,
    firstName John,
    lastName Doe
  }'
```

## 🔧 Troubleshooting

### Port Already in Use

If you see an error about ports being in use
```bash
# Stop all running containers
docker-compose down

# Check what's using the port
# Windows PowerShell
netstat -ano  findstr 4200
netstat -ano  findstr 5000

# LinuxMac
lsof -i 4200
lsof -i 5000
```

### Database Connection Issues
```bash
# Check PostgreSQL is running
docker-compose ps postgres

# View PostgreSQL logs
docker-compose logs postgres

# Restart database
docker-compose restart postgres
```

### Frontend Build Errors
```bash
# Rebuild frontend container
docker-compose up --build frontend
```

### Clear All Docker Data (Nuclear Option)
```bash
# Stop and remove all containers, networks, and volumes
docker-compose down -v

# Rebuild from scratch
docker-compose up --build
```

## 🏭 Production Deployment

### Environment Variables

For production deployment, create a `.env` file
```bash
cp .env.example .env
```

Edit `.env` with your production values
```env
PROD_CONNECTION_STRING=Host=your-prod-db.com;Port=5432;Database=organiza_prod;Username=admin;Password=your-secure-password;
JWT_SECRET_KEY=your-very-long-secret-key-min-32-characters
JWT_ISSUER=httpsyour-domain.com
JWT_AUDIENCE=httpsyour-domain.com
FRONTEND_URL=httpsyour-domain.com
```

### Deploy with Production Configuration
```bash
docker-compose -f docker-compose.prod.yml up -d
```

## 🧹 Cleanup

### Remove All Containers and Volumes
```bash
docker-compose down -v
```

### Remove Docker Images
```bash
docker rmi organiza_backend_dev organiza_frontend_dev
```

## 📊 Tech Stack Details

### Backend
- .NET 9.0
- Entity Framework Core 9.0
- PostgreSQL (Npgsql 9.0)
- JWT Bearer Authentication
- AutoMapper
- SwaggerOpenAPI

### Frontend
- Angular 20
- TypeScript 5.9
- RxJS 7.8
- FullCalendar 6.1

### DevOps
- Docker & Docker Compose
- Multi-stage builds for optimization
- Health checks for reliability

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b featureAmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin featureAmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

## 👥 Authors

- Your Name - Initial work

## 🆘 Support

If you encounter any issues

1. Check the [Troubleshooting](#-troubleshooting) section
2. Review Docker logs `docker-compose logs -f`
3. Ensure Docker Desktop is running
4. Try rebuilding `docker-compose up --build`

---

Note This application uses Docker Compose for easy setup. No manual installation of .NET, Node.js, or PostgreSQL is required.