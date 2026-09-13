# Stops the ConfigLens Docker Compose stack (Windows).
$ErrorActionPreference = "Stop"

Set-Location (Join-Path $PSScriptRoot "..")

docker compose down

Write-Host "ConfigLens stopped."
