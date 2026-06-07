@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo =======================================
echo Compilar DEBUG - Club Deportivo
echo =======================================
echo.
echo Cierre la aplicacion si esta abierta.
echo.

dotnet clean -c Debug
if errorlevel 1 goto error

dotnet build -c Debug
if errorlevel 1 goto error

echo.
echo ===============================
echo OK: bin\Debug\net8.0-windows\ClubDeportivo.exe
echo ===============================
pause
exit /b 0

:error
echo.
echo [ERROR] Fallo la compilacion.
pause
exit /b 1
