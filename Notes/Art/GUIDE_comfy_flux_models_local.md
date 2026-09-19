# Guide — Comfy Desktop : modèles FLUX (art Dump)

**ID tâche :** `[CT-ART-GEN-LOCAL-001]`  
**Date :** 2026-09-18 · **MAJ :** 2026-09-19 (stack 3 modèles — décision auteur)
**Hub workflow :** `Notes/Art/WORKFLOW_creation_assets.md`  
**Usage projet :** brouillons **création + retouche** en local (Dump) → repasse **ChatGPT** (style + droits ship) → `Sprites/`.

---

## Stack Comfy recommandé — **3 modèles** (pipeline art complet)

Objectif : couvrir **tout** le cycle local **avant** la repasse ChatGPT (§ pipeline), pas seulement bandeau/masque.

**Décision auteur (2026-09-19) — 3 poids, 2 principaux :**

| Tier | Rôle | Modèle (nom Comfy) | Workflow |
|------|------|-------------------|----------|
| **Principal A** | **Création from scratch** | **Flux1 Krea Dev FP8 scaled** | Text to Image FLUX |
| **Principal B** | **Modification** (image + consignes) | **Flux1 Dev Kontext FP8 scaled** | Kontext image edit |
| **Plan B retouche** | Si Kontext bloque / rendu raté | **Qwen Image Edit 2511** *(ou **2509 FP8** si VRAM serrée — pas de modèle « 2059 »)* | Template Qwen Image Edit |

Repli création si Krea indispo : **Flux1 Dev FP8**. Optionnel masque serré : **Flux1 Fill Dev** (4ᵉ poids, pas obligatoire).

```
Brief → [1 Krea/Dev] brouillon Dump
     → [2 Kontext] itérations sur PNG (ou ref projet)
     → ([3 Qwen] ou Fill si coin dur)
     → ChatGPT (IMAGE1=Comfy, IMAGE2=ref charte) « unify STYLE only »
     → OK auteur → Sprites/
```

**Ce n’est pas 3× le même modèle** : **Krea** = génération · **Kontext** = édition (duo principal) · **Qwen** = secours edit uniquement.

**ChatGPT direct** (sans Comfy) reste valide pour icônes §1, kit UI, bandeaux simples — voir `WORKFLOW_creation_assets.md` §3.3.

**VRAM / disque** : les 3 poids + CLIP/VAE partagés FLUX ≈ prévoir **~25–45 Go** selon variantes ; installer via **Model Manager** + templates (chemin principal), script HF = surtout Kontext.

---

## Où sont les dossiers sur ta machine (Windows)

**ComfyUI Desktop** ne met **pas** les poids dans le projet Unity.

| Action | Chemin |
|--------|--------|
| **Le plus simple** | Comfy Desktop → menu **Help** → **Open Folder** → **Open Model Folder** |
| Base par défaut | `C:\Users\madbox\Documents\ComfyUI\` |
| Checkpoints classiques | `Documents\ComfyUI\models\checkpoints\` |
| FLUX récent (souvent) | `Documents\ComfyUI\models\diffusion_models\` ou `models\unet\` |
| Text encoders (CLIP/T5) | `models\clip\` · `models\text_encoders\` |
| VAE | `models\vae\` |
| Sorties PNG | `Documents\ComfyUI\output\` |
| Config dossiers extra | `%APPDATA%\ComfyUI\extra_models_config.yaml` |

Après copie de fichiers : **touche `r`** dans Comfy pour refresh, ou redémarrer Desktop.

Réf. officielle : [Comfy — Models](https://docs.comfy.org/basic-concepts/models).

---

## Quel modèle télécharger (détail par rôle)

Liste Comfy : [supported-models](https://comfy.org/p/supported-models). **Partner Node** = API payante.

| Rôle (voir stack § ci-dessus) | Modèle | Où |
|--------|--------|-----|
| **#1 Créer** | **Flux1 Krea Dev FP8 scaled** · repli **Flux1 Dev FP8** | Manager Comfy · templates T2I |
| **#2 Modifier** | **Flux1 Dev Kontext FP8 scaled** · **FLUX.1 Kontext [dev]** | Manager · [HF](https://huggingface.co/black-forest-labs/FLUX.1-Kontext-dev) · `scripts/download-flux-kontext-dev.ps1` |
| **#3 Plan B edit** | **Qwen Image Edit 2511** · alt. **2509 FP8** | Templates Comfy |
| Masque serré (opt.) | **Flux1 Fill Dev** | Templates inpaint Fill |
| Plus tard (GPU costaud) | **Flux2 Dev fp8mixed** | Comfy / BFL |

Style Comfy : viser **`RaymanFarm_UI_CartoonCel`** (proche `FirstTryBandeauAtelier`) → meilleure conversion ChatGPT. Détail : `WORKFLOW_creation_assets.md` §3.

---

## Installation — 3 méthodes

### A) Comfy Desktop (recommandé)

1. **Model Manager** / **Templates** — installer **3 workflows** (téléchargement poids proposé par l’app) :
   - **Flux1 Krea Dev FP8 scaled** → template **Text to Image** (création).
   - **Flux1 Dev Kontext FP8 scaled** → template **Kontext image edit** (modification).
   - **Qwen Image Edit 2511** (ou **2509 FP8**) → template **Qwen Image Edit** (plan B).
2. Menu **Help → Open Model Folder** : vérifier `diffusion_models` / `clip` / `vae` / `text_encoders`.
3. Touche **`r`** dans Comfy pour refresh.

Licence **Agree** HF requise pour les poids BFL (Krea, Kontext, Fill) — compte HF dans l’app ou § B.

### B) Hugging Face (manuel)

1. Compte HF + **Accepter la licence** sur la page du modèle (gate « Agree ») — **obligatoire**, le login seul ne suffit pas.
2. **Login CLI** (huggingface_hub ≥ 1.32 : `huggingface-cli` / `huggingface_hub.cli.huggingface_cli` **ne marchent plus**) :

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\hf-auth-login.ps1
```

Équivalent manuel (si `hf.exe` dans `%LOCALAPPDATA%\Python\pythoncore-3.14-64\Scripts\`) :

```powershell
& "$env:LOCALAPPDATA\Python\pythoncore-3.14-64\Scripts\hf.exe" auth login
py -3 -c "from huggingface_hub import whoami; print(whoami())"
```

3. Télécharger :

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\download-flux-kontext-dev.ps1
```

**Erreur `GatedRepoError 401` :** token absent **ou** licence FLUX pas acceptée sur le site (même compte que le login).

4. Après téléchargement : placer ou pointer les fichiers selon le template Comfy (`diffusion_models` + `clip` + `vae` séparés). Refresh Comfy : touche **`r`**.

### C) Script projet

`scripts/download-flux-kontext-dev.ps1` — télécharge HF **vers** `Documents\ComfyUI\models\` **après** login. Ne remplace pas **Agree** licence sur le site HF.

---

## Licence & coût (jeu commercial)

| Sujet | Détail |
|-------|--------|
| **Usage du modèle [dev]** | **Non-commercial / non-prod** (R&D Dump) — licence [FLUX.1 dev NC](https://huggingface.co/black-forest-labs/FLUX.1-Kontext-dev/blob/main/LICENSE.md) |
| **Outputs FLUX [dev]** | BFL §2.d : usage Output **y compris commercial** possible, sauf interdits (ex. entraîner un modèle concurrent) |
| **Pipeline projet ship** | **ChatGPT repasse** sur ref charte → Sprites ; ou licence commerciale BFL / API |
| **Qwen Image Edit** | Lire licence fiche modèle (HF / Comfy) — Dump OK ; ship via repasse ChatGPT comme FLUX |
| **ChatGPT images** | CGU OpenAI — Output assigné à toi (indie OK en principe) |
| **Coût local** | GPU + disque ; scripts `scripts/hf-auth-login.ps1` |

Tableau complet : `WORKFLOW_creation_assets.md` §4.

---

## Ce que Cursor peut / ne peut pas faire

| Cursor | |
|--------|--|
| **Peut** | Doc (ce fichier), prompts, workflows texte, script `huggingface-cli`, composite Python (cf. `scripts/composite_plantfischstein_banner.py`). |
| **Ne peut pas** | Accepter la licence HF à ta place, télécharger des dizaines de Go sans ton compte HF, installer les poids dans Comfy sans accès à ton `Documents\ComfyUI`. |

**Toi :** login HF + Agree licence + (A) Manager Comfy ou (B) CLI / script.

---

## Premier test projet (checklist 3 modèles)

| # | Modèle | Test minimal | Output |
|---|--------|--------------|--------|
| A | **Krea Dev FP8** | Prompt court iso plante ou panneau UI bois, T2I | `Dump/` brouillon |
| B | **Kontext FP8** | Edit sur `FirstTryBandeauAtelier.png` (consigne ou masque chimère) | `Dump/Ui/Tab_Plus/` |
| C | **Qwen Image Edit** | Même PNG + consigne simple si B raté | comparer avec B |

Bandeau : `PROMPT_bandeau_atelier_bricolage.md` · composite ref `BandeauAtelier_Plantfischstein_20260917.png`.

---

## Pipeline Comfy → ChatGPT (rappel)

1. Comfy → `Dump/` (edit, même style UI cartoon si possible).  
2. ChatGPT : image1 = Comfy, image2 = ref charte → *« unify style only, keep layout »*.  
3. OK auteur → `Sprites/`.

Schéma complet : **`Notes/Art/WORKFLOW_creation_assets.md`**.

