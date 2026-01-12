# Script to run all API tests
# Usage: .\run-all-tests.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Running All API Tests" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$testProjects = @(
    "Catalog.API.Tests",
    "Basket.API.Tests",
    "Ordering.API.Tests",
    "Discount.API.Tests"
)

$totalTests = 0
$passedTests = 0
$failedTests = 0

foreach ($project in $testProjects) {
    $projectPath = "Tests\$project\$project.csproj"
    
    if (Test-Path $projectPath) {
        Write-Host "Running tests for: $project" -ForegroundColor Yellow
        Write-Host "----------------------------------------" -ForegroundColor Gray
        
        $result = dotnet test $projectPath --no-build --verbosity normal 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ $project - All tests passed" -ForegroundColor Green
            $passedTests++
        } else {
            Write-Host "✗ $project - Some tests failed" -ForegroundColor Red
            $failedTests++
        }
        
        Write-Host ""
    } else {
        Write-Host "⚠ Project not found: $projectPath" -ForegroundColor Yellow
    }
    
    $totalTests++
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Test Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Total Projects: $totalTests" -ForegroundColor White
Write-Host "Passed: $passedTests" -ForegroundColor Green
Write-Host "Failed: $failedTests" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Cyan

if ($failedTests -gt 0) {
    exit 1
} else {
    exit 0
}
