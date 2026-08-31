# Ouverture de session : fetch + statut + pull de la branche courante.
# A lancer TOI-MEME dans le terminal Cursor (racine du repo). L'assistant ne l'execute pas.
#
# Commande :
#   powershell -ExecutionPolicy Bypass -File .\scripts\session-git-sync.ps1
#
# One-liner equivalent :
#   git fetch --all --prune; git status -sb; git pull
#
# Quand c'est termine, dis a Cursor : "pull ok"

$ErrorActionPreference = "Continue"

$repoRoot = Split-Path -Parent $PSScriptRoot
if (-not (Test-Path (Join-Path $repoRoot ".git"))) {
    $repoRoot = (Get-Location).Path
}

Set-Location $repoRoot

Write-Host ""
Write-Host "=== Session git sync ===" -ForegroundColor Cyan
Write-Host "Dossier : $repoRoot"

$branch = (git branch --show-current).Trim()
if ([string]::IsNullOrWhiteSpace($branch)) {
    Write-Host "Aucune branche courante (HEAD detache ?). Stop." -ForegroundColor Red
    exit 1
}

Write-Host "Branche : $branch"
Write-Host ""

git fetch --all --prune
if ($LASTEXITCODE -ne 0) {
    Write-Host "fetch a echoue (reseau / remote). Stop avant pull." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "--- statut ---" -ForegroundColor Yellow
git status -sb
git branch -vv

Write-Host ""
Write-Host "--- pull ---" -ForegroundColor Yellow
git pull
$pullCode = $LASTEXITCODE

Write-Host ""
Write-Host "--- statut final ---" -ForegroundColor Yellow
git status -sb

if ($pullCode -ne 0) {
    Write-Host ""
    Write-Host "Pull bloque. Si pas de tracking :" -ForegroundColor Red
    Write-Host "  git branch --set-upstream-to=origin/$branch"
    Write-Host "  git pull"
    Write-Host "Si modifications locales : commit ou stash, puis relancer ce script. Voir GIT_HELPER.md § --1--."
    exit $pullCode
}

Write-Host ""
Write-Host "=== Sync terminee. Dis a Cursor : pull ok ===" -ForegroundColor Green
exit 0
