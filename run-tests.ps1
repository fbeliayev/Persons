# Run All Tests Script

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Running Person Management App Tests" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Backend Tests
Write-Host "Running Backend Tests (.NET xUnit)..." -ForegroundColor Yellow
Write-Host "--------------------------------------" -ForegroundColor Yellow

Set-Location Backend.Tests

$backendResult = dotnet test --verbosity minimal

Set-Location ..

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Backend tests PASSED!" -ForegroundColor Green
} else {
    Write-Host "❌ Backend tests FAILED!" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Frontend Tests
Write-Host "Running Frontend Tests (Vitest)..." -ForegroundColor Yellow
Write-Host "--------------------------------------" -ForegroundColor Yellow

Set-Location Frontend

$frontendResult = npm test -- --run --reporter=verbose 2>&1

Set-Location ..

if ($frontendResult -match "FAIL" -or $frontendResult -match "failed") {
    Write-Host "❌ Frontend tests FAILED!" -ForegroundColor Red
} else {
    Write-Host "✅ Frontend tests PASSED!" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($LASTEXITCODE -eq 0 -and -not ($frontendResult -match "FAIL")) {
    Write-Host "✅ All tests PASSED!" -ForegroundColor Green
} else {
    Write-Host "❌ Some tests FAILED! Check output above." -ForegroundColor Red
}

Write-Host ""
