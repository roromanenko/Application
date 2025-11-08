@echo off
echo ========================================
echo   Starting Organiza Development Stack
echo ========================================
echo.

echo Stopping existing containers...
docker-compose down

echo.
echo Building and starting containers...
docker-compose up --build -d

echo.
echo Waiting for services to start...
timeout /t 10 /nobreak > nul

echo.
echo ========================================
echo   Services Status
echo ========================================
docker-compose ps

echo.
echo ========================================
echo   Available URLs:
echo ========================================
echo   Frontend:  http://localhost:4200
echo   Backend:   http://localhost:5000
echo   Swagger:   http://localhost:5000/swagger
echo   Health:    http://localhost:5000/health
echo ========================================
echo.
echo To view logs: docker-compose logs -f
echo To stop:      docker-compose down
echo.