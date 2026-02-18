@echo off
echo ==========================================
echo  Stopping Backend Application
echo ==========================================
echo.

echo Attempting to stop PersonApi.exe...
taskkill /F /IM PersonApi.exe 2>nul

if %ERRORLEVEL% EQU 0 (
    echo.
    echo [SUCCESS] PersonApi.exe has been stopped!
    echo.
) else (
    echo.
    echo [INFO] PersonApi.exe is not running.
    echo.
)

echo You can now build the solution in Visual Studio.
echo.
pause
