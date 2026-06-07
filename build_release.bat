@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo =======================================
echo Compilar RELEASE - Club Deportivo
echo (carpeta para entrega del TP)
echo =======================================
echo.
echo Cierre la aplicacion si esta abierta.
echo.

dotnet clean -c Release
if errorlevel 1 goto error

dotnet build -c Release
if errorlevel 1 goto error

echo OK: bin\Release\net8.0-windows\
echo.
echo ===============================
echo Listo. Ejecutable en:
echo   bin\Release\net8.0-windows\ClubDeportivo.exe
echo Entregar esa carpeta completa (exe + DLLs + Database\Scripts)
echo Documentacion en doc\
echo ===============================
pause
exit /b 0

:error
echo.
echo [ERROR] Fallo la compilacion.
pause
exit /b 1
