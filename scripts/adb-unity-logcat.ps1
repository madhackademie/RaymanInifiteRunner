# Logs Unity du telephone (USB, debug ON).
# A lancer pendant le playtest. Ctrl+C pour arreter.
#
#   powershell -ExecutionPolicy Bypass -File .\scripts\adb-unity-logcat.ps1

$ErrorActionPreference = "Stop"
$adb = Join-Path $env:LOCALAPPDATA "Android\Sdk\platform-tools\adb.exe"

if (-not (Test-Path $adb)) {
    Write-Host "adb introuvable : $adb" -ForegroundColor Red
    exit 1
}

Write-Host "=== Unity logcat ===" -ForegroundColor Cyan
Write-Host "USB + debug : laisse cette fenetre ouverte pendant le playtest."
Write-Host "Ctrl+C pour arreter."
Write-Host ""

& $adb logcat -s Unity
