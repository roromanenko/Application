@echo off
echo ========================================
echo   Starting Organiza Production Stack
echo ========================================
echo.

echo Checking .env file...
if not exist .env (
    echo ERROR: .env file not found!
    echo Please copy .env.example to .env and fill in your production values
    pause
    exit /b 1
)

echo.
echo Stopping existing containers...
docker-compose -f docker-compose.prod.yml down

echo.
echo Building and starting production containers...
docker-compose -f docker-compose.prod.yml up --build -d

echo.
echo Waiting for services to start...
timeout /t 15 /nobreak > nul

echo.
echo ========================================
echo   Services Status
echo ========================================
docker-compose -f docker-compose.prod.yml ps

echo.
echo ========================================
echo   Production Stack Started
echo ========================================
echo.
echo To view logs: docker-compose -f docker-compose.prod.yml logs -f
echo To stop:      docker-compose -f docker-compose.prod.yml down
echo.