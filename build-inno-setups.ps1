# build.ps1
$ErrorActionPreference = "Stop"
$iscc = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"

Write-Host "→ Publish..." -ForegroundColor Cyan
dotnet publish -c Release

$version = (Get-Item ".\publish\NetAi.LetterLoom.exe").VersionInfo.ProductVersion
Write-Host "  Version: $version" -ForegroundColor Yellow

Write-Host "→ Installer..." -ForegroundColor Cyan
& $iscc .\setup.iss

Write-Host "→ Portable..." -ForegroundColor Cyan
& $iscc .\setup-portable.iss

Write-Host "✓ Fertig – Output: .\Output\" -ForegroundColor Green
Get-ChildItem .\Output\*.exe | Select-Object Name, Length, LastWriteTime