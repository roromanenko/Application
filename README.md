# Organiza - Event Management System

A full-stack event management application built with .NET 9, Angular 20, and PostgreSQL.

## 🚀 Live Demo

**🌐 Deployed Application:** [Organiza — Event Management System](https://organiza-frontend.onrender.com/)
> This environment contains test/demo data, so you can freely explore the API, event system, and the new AI Assistant without affecting any production information.

---

## AI Assistant (New Feature)
A new **AI-powered** assistant has been integrated into the platform to make interaction with event data more natural and intuitive.

What the AI Assistant Does
- It understands natural-language questions from the user such as:
  - “What events am I attending this week?”
  - "Where is my next event located?"
- Instead of relying on static snapshots, the assistant:
  - Generates SQL queries dynamically based on the user’s request
  - Converts the result into a friendly, conversational response in the user’s language
  
### Security Measures
The assistant can perform only `SELECT` queries. Cannot `INSERT`, `UPDATE`, `DELETE`, or modify schema in any way. This ensures that the AI Assistant is both powerful and completely safe for production use.

## Tags System (New Feature)
A new tag system has been added to improve event categorization and filtering. Events can now be linked to one or more tags.
Permissions
- Only users with the `admin` role are allowed to create new tags
- Regular users can view tags and assign existing ones to their events
- Tags are stored in a dedicated table and linked through the `event_tags` relationship

---

## Architecture

- Backend .NET 9 Web API with JWT authentication
- Frontend Angular 20 with TypeScript
- Database PostgreSQL 16
- Containerization Docker & Docker Compose

## Prerequisites

Before you begin, ensure you have the following installed

- [Docker Desktop](httpswww.docker.comproductsdocker-desktop) (4.0 or higher)
- [Git](httpsgit-scm.comdownloads)

Note You don't need to install .NET SDK, Node.js, or PostgreSQL locally - everything runs in Docker containers.

## Quick Start

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
- Start a PostgreSQL database container
- Build and run the .NET backend API
- Build and run the Angular frontend
- Apply database migrations automatically
- Set up networking between all services

### 3. Access the Application

Once all containers are running, access the application at

| Service | URL | Description |
|---------|-----|-------------|
| **Frontend** | http://localhost:4200 | Angular application |
| **Backend API** | http://localhost:5000 | REST API endpoints |
| **Swagger UI** | http://localhost:5000/swagger | API documentation |
| **Health Check** | http://localhost:5000/health | Service health status |

### 4. Default Credentials

Development Database
- Host `localhost:5432`
- Database `organiza_dev`
- Username `postgres`
- Password `postgres_dev_password`

## Development

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

## API Testing

### Using Swagger UI

1. Navigate to http://localhost:5000/swagger
2. Test endpoints interactively with the Swagger interface

### Using curl
```bash
# Health check
curl http://localhost:5000/health
```

## Troubleshooting

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

## Cleanup

### Remove All Containers and Volumes
```bash
docker-compose down -v
```

### Remove Docker Images
```bash
docker rmi organiza_backend_dev organiza_frontend_dev
```

## Tech Stack Details

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

## Author

- Romanenko Roman

---

Note This application uses Docker Compose for easy setup. No manual installation of .NET, Node.js, or PostgreSQL is required.
