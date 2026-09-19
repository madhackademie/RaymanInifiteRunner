# Telecharge FLUX.1 Kontext [dev] vers le dossier modeles Comfy Desktop (Windows).
# Preconditions :
#   1) Licence acceptee sur https://huggingface.co/black-forest-labs/FLUX.1-Kontext-dev (compte HF)
#   2) Login : powershell -File .\scripts\hf-auth-login.ps1

$ErrorActionPreference = "Stop"

$dest = Join-Path $env:USERPROFILE "Documents\ComfyUI\models\flux_kontext_dev"
New-Item -ItemType Directory -Force -Path $dest | Out-Null

Write-Host "Destination: $dest"
Write-Host "Installation / mise a jour huggingface_hub..."
py -3 -m pip install -U huggingface_hub | Out-Null

Write-Host "Verification token HF..."
$checkAuth = @"
from huggingface_hub import get_token, whoami
if not get_token():
    raise SystemExit(
        'ERREUR: pas de token HF. Lance: powershell -File scripts/hf-auth-login.ps1'
    )
print('Compte HF:', whoami().get('name'))
"@

py -3 -c $checkAuth
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Telechargement (plusieurs Go, patienter)..."
Write-Host ""

$pyCmd = @"
from huggingface_hub import snapshot_download
try:
    snapshot_download(
        repo_id='black-forest-labs/FLUX.1-Kontext-dev',
        local_dir=r'$dest',
        local_dir_use_symlinks=False,
    )
except Exception as e:
    msg = str(e)
    if '401' in msg or 'GatedRepo' in msg or 'restricted' in msg.lower():
        raise SystemExit(
            'ERREUR 401 / repo gated: (1) hf-auth-login.ps1 (2) page HF FLUX.1-Kontext-dev -> Agree licence (3) relancer ce script.'
        ) from e
    raise
print('Download OK:', r'$dest')
"@

py -3 -c $pyCmd
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "OK. Comfy Desktop -> Help -> Open Model Folder."
Write-Host "Deplace les .safetensors selon le workflow Kontext (diffusion_models / clip / vae)."
Write-Host "Doc: Notes/Art/GUIDE_comfy_flux_models_local.md"
