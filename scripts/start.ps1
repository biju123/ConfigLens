# Starts ConfigLens locally via Docker Compose (Windows).
$ErrorActionPreference = "Stop"

Set-Location (Join-Path $PSScriptRoot "..")

if (-not (Test-Path ".env")) {
    Write-Host "No .env file found - copying .env.example to .env."
    Write-Host "Edit .env (especially CONFIGLENS_JWT_SIGNING_KEY) before using this outside local development."
    Copy-Item ".env.example" ".env"
}

docker compose up --build -d

Write-Host ""
Write-Host "ConfigLens is starting."
Write-Host "  Frontend: http://localhost:5173"
Write-Host "  Backend:  http://localhost:5080 (Swagger at /swagger)"
Write-Host ""
Write-Host "Run 'docker compose logs -f' to follow logs, or scripts/stop.ps1 to stop."
