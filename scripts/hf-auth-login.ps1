# Login Hugging Face (token stocke pour py -3 / snapshot_download).
# Prealable : accepter la licence sur https://huggingface.co/black-forest-labs/FLUX.1-Kontext-dev

$ErrorActionPreference = "Stop"

py -3 -m pip install -U huggingface_hub | Out-Null

$scriptsDir = py -3 -c "import pathlib, sys; print(pathlib.Path(sys.executable).parent / 'Scripts')"
$hf = Join-Path $scriptsDir "hf.exe"

if (-not (Test-Path $hf)) {
    Write-Host "hf.exe introuvable dans $scriptsDir"
    Write-Host "Alternative : py -3 -c `"from huggingface_hub import login; login()`""
    exit 1
}

Write-Host "Login HF (navigateur ou token)..."
& $hf auth login

Write-Host ""
Write-Host "Verification..."
py -3 -c "from huggingface_hub import whoami; u=whoami(); print('Connecte:', u.get('name', u))"

Write-Host ""
Write-Host "Si le download FLUX echoue encore en 401 : ouvrir la page du modele, cliquer Agree (meme compte HF)."
