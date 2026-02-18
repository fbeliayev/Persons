# Stop Backend Application
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host " Stopping Backend Application" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Attempting to stop PersonApi.exe..." -ForegroundColor Yellow

try {
    $processes = Get-Process -Name PersonApi -ErrorAction SilentlyContinue
    
    if ($processes) {
        $processes | Stop-Process -Force
        Write-Host ""
        Write-Host "[SUCCESS] PersonApi.exe has been stopped!" -ForegroundColor Green
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "[INFO] PersonApi.exe is not running." -ForegroundColor Gray
        Write-Host ""
    }
} catch {
    Write-Host ""
    Write-Host "[INFO] PersonApi.exe is not running." -ForegroundColor Gray
    Write-Host ""
}

Write-Host "You can now build the solution in Visual Studio." -ForegroundColor White
Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
