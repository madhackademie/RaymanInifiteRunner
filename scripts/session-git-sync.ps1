# Ouverture de session : fetch + statut + pull de la branche courante + integration origin/main si besoin.
# A lancer TOI-MEME dans le terminal Cursor (racine du repo). L'assistant ne l'execute pas.
#
# Commande :
#   powershell -ExecutionPolicy Bypass -File .\scripts\session-git-sync.ps1
#
# One-liner equivalent (sans merge main automatique) :
#   git fetch --all --prune; git status -sb; git pull
#
# Quand c'est termine, dis a Cursor : "pull ok"

$ErrorActionPreference = "Continue"

function Get-DefaultRemoteBranchName {
    $sym = git symbolic-ref -q refs/remotes/origin/HEAD 2>$null
    if ($sym -and $sym -match 'refs/remotes/origin/(.+)$') {
        return $Matches[1].Trim()
    }
    return "main"
}

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

$defaultBranch = Get-DefaultRemoteBranchName
$defaultRemoteRef = "origin/$defaultBranch"

Write-Host "Branche : $branch"
Write-Host "Branche principale distante : $defaultRemoteRef"
Write-Host ""

git fetch --all --prune
if ($LASTEXITCODE -ne 0) {
    Write-Host "fetch a echoue (reseau / remote). Stop avant pull." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "--- statut ---" -ForegroundColor Yellow
git status -sb
git --no-pager branch -vv

$porcelain = git status --porcelain 2>$null
if ($porcelain) {
    Write-Host ""
    Write-Host "WORKING TREE NON VIDE — pull risque d'echouer (merge abort)." -ForegroundColor Red
    Write-Host "Avant pull : commit WIP sur ta branche OU stash, puis relance ce script."
    Write-Host ""
    Write-Host "  git stash push -u -m ""WIP avant pull session"""
    Write-Host "  powershell -ExecutionPolicy Bypass -File .\scripts\session-git-sync.ps1"
    Write-Host "  git stash pop"
    Write-Host ""
    Write-Host "Voir GIT_HELPER.md (Si git pull refuse a cause de modifs locales)."
    Write-Host ""
    Write-Host "Stop ici (pas de pull sur tree sale). Corrige puis relance." -ForegroundColor Red
    exit 2
}

Write-Host ""
Write-Host "--- pull ($branch) ---" -ForegroundColor Yellow
git pull
$pullCode = $LASTEXITCODE

if ($pullCode -ne 0) {
    Write-Host ""
    Write-Host "--- statut final ---" -ForegroundColor Yellow
    git status -sb
    Write-Host ""
    Write-Host "Pull bloque. Si pas de tracking :" -ForegroundColor Red
    Write-Host "  git branch --set-upstream-to=origin/$branch"
    Write-Host "  git pull"
    Write-Host "Si modifications locales : commit ou stash, puis relancer ce script. Voir GIT_HELPER.md section --1--."
    exit $pullCode
}

$mergeMainCode = 0
if ($branch -ne $defaultBranch) {
    git rev-parse --verify "$defaultRemoteRef" 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) {
        $mainAheadCountRaw = (git rev-list --count "HEAD..$defaultRemoteRef" 2>$null).Trim()
        $mainAheadCount = 0
        [void][int]::TryParse($mainAheadCountRaw, [ref]$mainAheadCount)

        if ($mainAheadCount -gt 0) {
            Write-Host ""
            Write-Host "--- integration $defaultRemoteRef ($mainAheadCount commit(s) a integrer) ---" -ForegroundColor Yellow
            git --no-pager log --oneline -10 "HEAD..$defaultRemoteRef"
            Write-Host ""
            git merge --no-edit $defaultRemoteRef
            $mergeMainCode = $LASTEXITCODE
        }
        else {
            Write-Host ""
            Write-Host "Branche a jour avec $defaultRemoteRef (rien a integrer)." -ForegroundColor DarkGray
        }
    }
    else {
        Write-Host ""
        Write-Host "Ref $defaultRemoteRef introuvable apres fetch - merge main ignore." -ForegroundColor DarkYellow
    }
}

Write-Host ""
Write-Host "--- statut final ---" -ForegroundColor Yellow
git status -sb

if ($mergeMainCode -ne 0) {
    Write-Host ""
    Write-Host "Merge de $defaultRemoteRef bloque (conflits ou etat sale)." -ForegroundColor Red
    Write-Host "Resous les conflits, puis : git add ... ; git commit"
    Write-Host "Ou annule : git merge --abort"
    Write-Host "Voir GIT_HELPER.md section --1-- et section --3--."
    exit $mergeMainCode
}

Write-Host ""
Write-Host "=== Sync terminee. Dis a Cursor : pull ok ===" -ForegroundColor Green
exit 0
