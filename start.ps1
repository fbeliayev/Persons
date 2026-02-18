# Person Management Application Startup Script

Write-Host "Starting Person Management Application..." -ForegroundColor Green
Write-Host ""

# Start Backend
Write-Host "Starting .NET Backend API on port 5000..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd Backend; Write-Host 'Backend API Starting...' -ForegroundColor Cyan; dotnet run"

# Wait for backend to initialize
Write-Host "Waiting 5 seconds for backend to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Start Frontend
Write-Host "Starting React Frontend on port 5173..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd Frontend; Write-Host 'React App Starting...' -ForegroundColor Cyan; npm run dev"

Write-Host ""
Write-Host "Application is starting!" -ForegroundColor Green
Write-Host "Backend API: http://localhost:5000" -ForegroundColor Cyan
Write-Host "Frontend App: http://localhost:5173" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
