# Guide — Comfy Desktop : modèles FLUX (art Dump)

**ID tâche :** `[CT-ART-GEN-LOCAL-001]`  
**Date :** 2026-09-18  
**Usage projet :** inpaint / image→image (bandeau atelier, assets UI) → `Assets/Art/Assets Store Dump/` puis promo auteur → `Sprites/`.

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

## Quel modèle télécharger (production d’assets **édition**)

| Besoin | Modèle | Où le prendre |
|--------|--------|----------------|
| **Modifier** un PNG existant (masque, bandeau) | **FLUX.1 Kontext [dev]** | [huggingface.co/black-forest-labs/FLUX.1-Kontext-dev](https://huggingface.co/black-forest-labs/FLUX.1-Kontext-dev) |
| Création from scratch + edit multi-ref (GPU costaud) | **FLUX.2 [dev]** | [huggingface.co/black-forest-labs/FLUX.2-dev](https://huggingface.co/black-forest-labs/FLUX.2-dev) (vérifier page BFL à jour) |
| Brouillons rapides | FLUX.2 Klein | Plus léger — voir doc BFL |

**Pour ton bandeau Plantfischstein :** commencer par **Kontext [dev]** (workflows Comfy « Kontext / image edit »).

---

## Installation — 3 méthodes

### A) Comfy Desktop (recommandé si proposé)

1. Onglet **Templates** / **Get started** / **Model Manager** (libellé selon version).
2. Chercher **FLUX Kontext** ou importer un **workflow template** Kontext → l’app propose souvent le téléchargement des poids manquants.
3. Vérifier dans **Open Model Folder** que les fichiers sont apparus.

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

CLI (après `py -3 -m pip install -U huggingface_hub`) — **sans** dépendre du PATH :

Ensuite, déplacer ou pointer les fichiers selon le template Comfy (parfois `diffusion_models` + `clip` + `vae` séparés).

### C) Script projet (optionnel)

`scripts/download-flux-kontext-dev.ps1` — lance le téléchargement HF **vers** `Documents\ComfyUI\models\` **après** login HF. Ne remplace pas l’acceptation licence sur le site.

---

## Licence & coût (jeu commercial)

| Sujet | Détail |
|-------|--------|
| **Licence [dev]** | **FLUX.1-dev Non-Commercial License** — recherche / perso / prototypage Dump OK ; **promo Sprites / jeu publié** = vérifier [BFL licensing](https://bfl.ai) ou API Pro/Max commerciale. |
| **Coût local** | Gratuit en redevance modèle ; coût = disque (~20–40 Go selon variante) + électricité GPU. |
| **Coût API** | Kontext Pro ~0,04 $/image, Max ~0,08 $ (si pas de GPU). |

**Décision à tracer ici** quand tranché : cocher dans `Notes/Todo_project.md` `[CT-ART-GEN-LOCAL-001]`.

---

## Ce que Cursor peut / ne peut pas faire

| Cursor | |
|--------|--|
| **Peut** | Doc (ce fichier), prompts, workflows texte, script `huggingface-cli`, composite Python (cf. `scripts/composite_plantfischstein_banner.py`). |
| **Ne peut pas** | Accepter la licence HF à ta place, télécharger des dizaines de Go sans ton compte HF, installer les poids dans Comfy sans accès à ton `Documents\ComfyUI`. |

**Toi :** login HF + Agree licence + (A) Manager Comfy ou (B) CLI / script.

---

## Premier test projet

1. Workflow Comfy **FLUX.1 Kontext image edit** + masque.
2. Input : `Assets/Art/Assets Store Dump/Ui/Tab_Plus/FirstTryBandeauAtelier.png`.
3. Masque : zone truite/poireau/pince (comme ChatGPT).
4. Output → `Dump/Ui/Tab_Plus/` — comparer avec `BandeauAtelier_Plantfischstein_20260917.png`.

Réf. prompts bandeau : `Notes/Art/PROMPT_bandeau_atelier_bricolage.md` (Passe 2 / 3).
