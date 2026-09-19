# Workflow — création d’assets (Dump → Sprites)

**Création :** 2026-09-19  
**MAJ :** 2026-09-19 (stack Comfy : Krea + Kontext + Qwen · décision auteur)  
**Règle dump :** `.cursor/rules/art_asset_dump.mdc`  
**Backlog lignes :** `Notes/Art/PROMPT_generation_icones.md` §3  
**Charte :** `Notes/Art/NOTE_graphique.md`

---

## 1) Principe immuable

```
Brief (cette note + prompts spécialisés)
    → génération → Assets/Art/Assets Store Dump/…     (en dump)
    → OK auteur
    → copie → Assets/Art/Sprites/…                    (promu)
    → Bezy : prefabs / 9-slice / wiring (UI)
    → Cursor : services, pas prefabs UI long terme
```

- **Jamais** brancher un brut Dump sur un prefab / `PlantDefinition` / UI runtime sans promo.
- Nommer : `Type_Objet_YYYYMMDD.png` (ex. `UiKit_Panel_9s_20260919.png`).

---

## 2) Choisir l’outil (decision tree)

| Besoin | Outil principal | Prompt / doc |
|--------|-----------------|--------------|
| **Icône UI** 64–256 px, bois rustique | **ChatGPT** (ou générateur §1 backlog) | `PROMPT_generation_icones.md` §1 |
| **Bandeau / illustration UI** (pas iso) | **ChatGPT** (+ ref projet) | `PROMPT_bandeau_atelier_bricolage.md` · `FirstTryBandeauAtelier.png` |
| **Kit UI chrome** (panneaux, boutons, nav bois) | **ChatGPT** → découpe → Bezy 9-slice | `Notes/Ui/PROMPT_chatgpt_ui_kit_planche_bois.md` |
| **Monde iso 2:1** (plantes, IBC) | **Comfy Krea** (brouillon) ou **ChatGPT** ; retouche **Kontext** ; ship ChatGPT si besoin | `PROMPT_assets_monde_iso.md` |
| **Brouillon local puis charte** | **Comfy** (Krea → Kontext → Qwen plan B) | `GUIDE_comfy_flux_models_local.md` |
| **Edit / inpaint** (fusion, masque, garder décor) | **Comfy Kontext** · plan B **Qwen Image Edit** | idem |
| **Composite rapide** (collage script) | **Cursor** (Python) | ex. `scripts/composite_plantfischstein_banner.py` |
| **Prefabs Unity** | **Bezy** | `Notes/Bezi/WORKFLOW_skill_prefab_ui.md` |

---

## 3) Pipeline hybride recommandé (2026-09 — auteur)

Pour **max conversion** vers la charte et **usage commercial indie** (salaire correct, pas publish AAA) :

### 3.1 Style cible unique (UI)

**Nom interne :** `RaymanFarm_UI_CartoonCel`

- Illustration **2D mobile UI** : bois rustique clair, contours bruns épais, cel shading, fun cartoon.
- **Pas** pierre grise barre nav actuelle · **pas** iso sur bandeaux · **pas** photoreal.
- **Ref projet :** `Dump/Ui/Tab_Plus/FirstTryBandeauAtelier.png`, `Dump/Ui/cadreBoisFinal.png`, icônes §1 backlog.

### 3.2 Chaîne Comfy → ChatGPT → Sprites

```
Comfy #1 Krea (créer) ou edit direct
Comfy #2 Kontext (modifier) · #3 Qwen si blocage     → Dump/   [R&D local]
        ↓
ChatGPT : IMAGE1 = sortie Comfy
          IMAGE2 = ref charte projet
          « Unify STYLE only, keep layout exactly »
        ↓
OK auteur → Sprites/                  [ligne APK / UI runtime]
```

- **Comfy seul en Sprites** : possible pour tests ; voir §4 licences.
- **ChatGPT sortie** : CGU OpenAI — Output assigné à toi ; **inputs** = droits OK (refs projet, pas stock volé).

### 3.3 Chaîne ChatGPT directe (icônes, kit UI, bandeaux)

```
ChatGPT (+ 1 ref projet) → Dump/ → OK → Sprites/
```

P0 kit UI : voir prompts dans `Notes/Ui/PROMPT_chatgpt_ui_kit_planche_bois.md`.

---

## 4) Licences (résumé pratique — pas avocat)

| Source | Usage Dump / R&D | Promo Sprites / jeu |
|--------|------------------|---------------------|
| **ChatGPT images** | OK | **Oui** en principe (CGU OpenAI + inputs licites) |
| **FLUX [dev] local (BFL)** | Usage **modèle** non commercial / non prod | **Outputs** : licence BFL §2.d souvent **commercial possibles** ; usage **modèle** en prod → [bfl.ai](https://bfl.ai) ou API |
| **Comfy Partner Node (Flux API…)** | Payant | Selon CGU API (souvent commercial, pas gratuit) |
| **FLUX Schnell (Apache 2.0)** | OK | Commercial OK — **pas** Kontext / inpaint bandeau |
| **Main / Photopea / Bezy sur brief** | OK | OK |

**Retouche légère** d’un PNG NC **ne rend pas** automatiquement commercial sans droits sur la base — la chaîne **Comfy → ChatGPT repasse** vise une **nouvelle** sortie ChatGPT pour le ship.

---

## 5) Comfy Desktop (local) — **3 modèles** (2 principaux + plan B)

**Principaux :** **Flux1 Krea Dev FP8** (créer from scratch) · **Flux1 Dev Kontext FP8** (modifier).  
**Plan B :** **Qwen Image Edit 2511** (ou **2509 FP8**). Repasse **ChatGPT** avant `Sprites/` — §3.2 · §4.

| # | Rôle | Modèle Comfy |
|---|------|----------------|
| 1 | **Créer** (texte → PNG) | **Flux1 Krea Dev FP8 scaled** (repli Flux1 Dev FP8) |
| 2 | **Modifier** (image + consignes) | **Flux1 Dev Kontext FP8 scaled** |
| 3 | **Plan B retouche** | **Qwen Image Edit 2511** (alt. **2509 FP8**) |

Optionnel : **Flux1 Fill Dev** — inpaint masque serré (bandeau).

- Templates : **Text to Image** (Krea) · **Kontext edit** · **Qwen Image Edit**
- HF + script projet : surtout **Kontext** — `scripts/hf-auth-login.ps1` · `scripts/download-flux-kontext-dev.ps1`
- **Ship Sprites** : repasse **ChatGPT** style lock (ou voie BFL commerciale) — §4

---

## 6) Implémentation Unity

| Étape | Qui |
|-------|-----|
| Import PNG, 9-slice borders, PPU 100 | Auteur / Bezy (Inspector) |
| Remplacer sprites UI / prefabs | **Bezy** (`PROMPTS_Bezi_nav_wood_frame_slice.md`, `/prefab-ui-3phases`) |
| Logique, services | **Cursor** |

---

## 7) Docs & prompts par type

| Sujet | Fichier |
|-------|---------|
| Backlog + §1 icône | `PROMPT_generation_icones.md` |
| Monde iso | `PROMPT_assets_monde_iso.md` · `NOTE_graphique.md` |
| Bandeau atelier | `PROMPT_bandeau_atelier_bricolage.md` |
| Comfy / FLUX | `GUIDE_comfy_flux_models_local.md` |
| Kit UI bois ChatGPT | `Notes/Ui/PROMPT_chatgpt_ui_kit_planche_bois.md` |
| Hub bandeaux | `PROMPT_features_hub_bandeau_flag.md` |

---

## 8) Routine session art (~15 min / asset)

1. Lire la **première ligne `à générer`** §3 backlog (ou tâche `P0-ART-*` / UI kit).
2. Choisir outil (§2).
3. Générer → **Dump** (sous-dossier famille).
4. Noter statut `en dump` + nom fichier dans backlog.
5. **Après OK auteur** : promo **Sprites** + trace optionnelle : « source ChatGPT / Comfy+ChatGPT / date ».
6. Bezy si prefab impacté.

---

## 9) Dumps utiles (miroir étendu)

| Dump | Promo Sprites |
|------|----------------|
| `…/Dump/Ui/Tab_Plus/` | `Sprites/UI/` ou FeaturesHub |
| `…/Dump/Ui/KitRefonte/` | `Sprites/UI/Kit/` |
| (reste) | voir tableau § miroir `PROMPT_generation_icones.md` |

---

## 10) Exemples chantiers (2026-09)

| Chantier | Outils | Fichiers clés |
|----------|--------|----------------|
| **Bandeau Plantfischstein** | ChatGPT inpaint (limites fusion) → Comfy Kontext → repasse ChatGPT ; collage Cursor | `PROMPT_bandeau_atelier_bricolage.md` · `Dump/Ui/Tab_Plus/BandeauAtelier_Plantfischstein_20260917.png` · `scripts/composite_plantfischstein_banner.py` |
| **Kit UI bois** | ChatGPT planche 2×3 → Dump `Ui/KitRefonte/` → 9-slice → Bezy | `Notes/Ui/PROMPT_chatgpt_ui_kit_planche_bois.md` · `PROMPTS_Bezi_nav_wood_frame_slice.md` |
| **Comfy local** | Stack **Krea + Kontext + Qwen 2511** (2 principaux + plan B) | `GUIDE_comfy_flux_models_local.md` · `[CT-ART-GEN-LOCAL-001]` |

**Décision auteur (2026-09-19) :** Comfy = **Krea** (créer) + **Kontext** (modifier) + **Qwen** (plan B edit) → Dump ; ship UI via **ChatGPT (style lock) → Sprites** ; iso monde peut rester ChatGPT direct avec moins de repasse.
