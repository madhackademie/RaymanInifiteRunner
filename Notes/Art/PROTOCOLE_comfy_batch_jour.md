# Protocole — Comfy « masse le jour / tri le soir »

**Objectif :** produire du **matériel brut** (Dump) sans juger pendant les runs ; **valider + prompts du lendemain** en session Cursor le soir.  
**Pas d’agents branchés** — file Comfy manuelle + ce fichier + chat Cursor.

**GPU :** GTX 1060 6 Go — **1 run à la fois**, Unity **fermé** pendant les générations.  
**Stack :** Krea (créer icônes UI) · **ChatGPT** (style lock + grains nobles) · Kontext = **bandeau Plantfischstein seulement** (pas dans la chaîne icônes) · Qwen = **skip** sur cette machine.

**Décision auteur 2026-09-22 — icônes UI :** pas de passe Kontext intermédiaire. **Krea (masse) → tri soir → ChatGPT (ref charte)** pour la sélection / harmonisation. Kontext reste réservé aux **edits** sur PNG existant (ex. bandeau atelier).

---

## Pipeline icônes UI (Krea → ChatGPT)

```
Nuit / jour   Krea T2I (krea_dump_batch)     → output Comfy (brut, plusieurs seeds)
Soir          Tri KEEP / RETRY / DROP         → 0–2 KEEP max par objet
              ChatGPT                         → IMAGE1 = KEEP Comfy
                                                IMAGE2 = ref charte (IconeInventaire, cadreBoisFinal…)
                                                prompt style lock ci-dessous
              OK auteur                       → renommer → Dump/Ui/Dashboard|Quests|Currency/
Promo         plus tard                       → Sprites/ (après OK, pas direct depuis Comfy)
```

**Kontext :** ne pas enfiler entre Krea et ChatGPT pour ce pipeline (gain temps GPU + le visuel de ref est mieux géré en ChatGPT).

**Prompt ChatGPT (copier, adapter [OBJET]) :**

```
Edit IMAGE 1 (the Comfy draft icon). Use IMAGE 2 only as STYLE reference (outlines, rustic farm UI cartoon, cel shading, colors).

Keep IMAGE 1 layout exactly: same single subject, same pose, same silhouette, same white background.
Unify STYLE to match IMAGE 2 — thick pure black outer outline, NO anti-aliasing on silhouette, hard alpha edge, cozy mobile farming game, readable at 64px.
NO text, NO letters, NO numbers, NO watermark. One icon only.

Subject must stay: [OBJET — ex. lightning bolt for electricity / water droplet / daily quest sun badge / bolt token].
```

**Refs IMAGE2 suggérées :** `Dump/Ui/IconeInventaire.png` (icônes bois / quêtes / boulon) · glyphes dashboard : même ref ou une icône HUD déjà validée si tu en as une.

---

## Rythme type

| Moment | Toi | Cursor (soir) |
|--------|-----|----------------|
| **Matin / pause** | Coller **2–4 jobs** dans la file Comfy (ci-dessous), lancer, laisser le PC | — |
| **Journée** | PC peut tourner (pas Unity Play en parallèle) | — |
| **Soir (15–30 min)** | Ouvrir `output` + Dump, tri rapide | On regarde les PNG, on note KEEP / RETRY / DROP, on rédige **liste prompts J+1** |
| **Avant dodo** | Optionnel : file **nuit** (mêmes graphes, seeds +1) | — |

**Débit réaliste :** **Krea** 4–6 icônes / session nuit · **ChatGPT** 1 passe / KEEP (grains nobles) · **Kontext** seulement si chantier **bandeau** actif — pas 5×1 h Kontext full res.

---

## Où vont les fichiers

| Étape | Dossier |
|-------|---------|
| Sortie Comfy | `D:\Comfy-Desktop\ComfyUI-Shared\output\` |
| Après tri soir (KEEP Comfy) | `output/` → tri local, pas encore Dump |
| Après ChatGPT OK | `Dump/Ui/Dashboard/` · `Dump/Ui/Quests/` · `Dump/Ui/Currency/` · bandeau → `Dump/Ui/Tab_Plus/` |
| Nom suggéré | `Icone_Electricite_YYYYMMDD.png` · `Bandeau_Plantfischstein_YYYYMMDD_seedXXXX.png` |

Renommer **après** le tri — pas pendant la file (évite les collisions).

---

## Préparer la file (sans automatisation)

**Même graphe** (ex. Kontext bandeau + 2 resize + Aperçu).

Pour chaque job **avant** de partir :

1. Changer **seed** (ou `randomize` si le nœud le permet).
2. Coller le **prompt** du job (liste « Jour N » ci-dessous).
3. **Queue** → ajouter à la file (pas Run unique si tu enchaînes).
4. Répéter pour job 2, 3…

**Limite 1060 :** **2–3 jobs** par session journée. **File nuit icônes** : jusqu’à **6 Krea**, **1 GPU**, Unity fermé. Kontext bandeau = **fichier séparé**, autre session.

**Sauvegarde :** `Workflow → Save` sous un nom daté, ex. `kontext_bandeau_plantfischstein_light.json`.

---

## Fiche « Jour N » (copier dans ce fichier ou en tête de chat)

```markdown
## Comfy batch — YYYY-MM-DD

**Graphe :** kontext_bandeau_plantfischstein_light.json
**Resize :** bandeau 768×320 · chimère 512×288

| # | Seed | Prompt (résumé 1 ligne) | Statut soir |
|---|------|-------------------------|-------------|
| 1 |      | Plantfischstein 1-pass image1+2 | |
| 2 |      | Idem seed+1 | |
| 3 |      | 2e passe : « fix creature only » | |

**Soir — tri :** KEEP: … · RETRY: … · DROP: …
**Prompts J+1 :** (Cursor remplit après session)
```

---

## Comfy batch — 2026-09-22 (nuit)

**Mode :** masse, pas de validation pendant les runs. Tri KEEP / RETRY / DROP **demain soir**.  
**Unity fermé.** Comfy : `--enable-manager --novram`. **1 job GPU.**  
**Pipeline :** **Krea ×6 seulement** (pas Kontext cette nuit). Après runs : **ChatGPT** sur les KEEP (voir § Pipeline icônes UI).  
Chaque Queue fige le graphe : seed + prompt, puis Queue. **6 Krea** = limite haute sur 1060 — si la file rame, finir K1–K3 ce soir, K4–K6 demain.

### Préflight (constat disque)

| Poids | Fichier | État |
|-------|---------|------|
| Kontext FP8 | `flux1-dev-kontext_fp8_scaled.safetensors` | présent |
| T5 / CLIP / VAE Flux | `t5xxl_fp8_e4m3fn_scaled` · `clip_l` · `ae` | présents |
| **Krea FP8** | `flux1-krea-dev_fp8_scaled.safetensors` | **absent** |
| Qwen 2509 | présent | **ne pas lancer** (OOM 1060) |

Sans le fichier Krea dans `D:\Comfy-Desktop\ComfyUI-Shared\models\diffusion_models\`, les jobs K1–K6 échouent. Les installer **avant** de les mettre en file (Model Manager → Flux1 Krea Dev FP8 scaled, ou copie du `.safetensors`). Le graphe est déjà sauvé : `ComfyUI\user\default\workflows\krea_dump_batch.json` (template Krea, **512×512**, T5 **FP8** déjà sur la machine, nœud **Enregistrer image** préfixe `krea_dump`).

**Kontext :** sauver le graphe ouvert sous `kontext_bandeau_plantfischstein_light.json` avant de partir (aujourd’hui seul `qwen_edit_bandeau_atelier.json` est sur disque — archive, ne pas l’ouvrir).

### Krea — UI dashboard / quêtes / monnaie (liste auteur)

**Graphe :** Workflows → `krea_dump_batch.json`.  
**Taille :** **512×512**. Steps **20**, cfg **1**, euler / simple, batch **1**, seed **fixed**.  
**Sortie :** `D:\Comfy-Desktop\ComfyUI-Shared\output\krea_dump_*.png`  
**Dump après tri** (renommer selon ligne) :

| Job | Dossier Dump | Nom suggéré après tri |
|-----|--------------|------------------------|
| K1 éclair | `Dump/Ui/Dashboard/` | `Icone_Electricite_20260922_seed22092221.png` |
| K2 eau | `Dump/Ui/Dashboard/` | `Icone_Eau_20260922_seed22092222.png` |
| K3 daily | `Dump/Ui/Quests/` | `Icone_Quete_Daily_20260922_seed22092223.png` |
| K4 weekly | `Dump/Ui/Quests/` | `Icone_Quete_Weekly_20260922_seed22092224.png` |
| K5 monthly | `Dump/Ui/Quests/` | `Icone_Quete_Monthly_20260922_seed22092225.png` |
| K6 boulon | `Dump/Ui/Currency/` | `Icone_Boulon_Brico_20260922_seed22092226.png` |

Backlog : **H2** · **H3** · **H5** · **H6** · **H7** · **H9** (`PROMPT_generation_icones.md`). Pas H4 (onglet parent quêtes) dans ce batch.

| # | Seed | ID | Résumé | Statut soir |
|---|------|-----|--------|-------------|
| K1 | 22092221 | H2 | éclair consommation électricité | |
| K2 | 22092222 | H3 | goutte consommation eau | |
| K3 | 22092223 | H5 | quête daily (soleil) | |
| K4 | 22092224 | H6 | quête weekly (calendrier 7 j) | |
| K5 | 22092225 | H7 | quête monthly (lune / mois) | |
| K6 | 22092226 | H9 | boulon monnaie brico / déchetterie | |

**Variante masse :** même prompt K3, seeds **22092223 / 22092227 / 22092228** (3 runs daily) — utile si un seul motif quête ne suffit pas.

K1 — électricité :

```
A 2D casual mobile game dashboard icon of a single bright lightning bolt for electricity usage, cartoon style, vibrant yellow and amber energy, thick dark outlines, smooth cel shading, cozy farming game UI glyph. One bolt only, centered, readable at 48 pixels. Isolated on a white background. NOT photoreal, NOT 3D. NO text, NO letters, NO numbers, NO watermark.
```

K2 — eau :

```
A 2D casual mobile game dashboard icon of a single water droplet for water consumption, cartoon style, vibrant blue and cyan, thick dark outlines, smooth cel shading, cozy farming game UI glyph. One teardrop only, centered, readable at 48 pixels. Isolated on a white background. NOT photoreal, NOT 3D. NO text, NO letters, NO numbers, NO watermark.
```

K3 — quête daily :

```
A 2D casual mobile game icon of a daily quest badge, cartoon style, vibrant colors, isolated on a white background. Made with thick rustic light brown wooden textures, thick outlines, smooth shading, cozy farming game aesthetic, high quality UI asset. A small wooden sign or shield with a cute cartoon SUN above it (morning, one day). NO calendar numbers, NO readable text, NO letters, NO watermark.
```

K4 — quête weekly :

```
A 2D casual mobile game icon of a weekly quest badge, cartoon style, vibrant colors, isolated on a white background. Made with thick rustic light brown wooden textures, thick outlines, smooth shading, cozy farming game aesthetic, high quality UI asset. A wooden clipboard or sign with a simple seven-day calendar grid (seven empty squares in a row, no digits). NO readable text, NO letters, NO watermark.
```

K5 — quête monthly :

```
A 2D casual mobile game icon of a monthly quest badge, cartoon style, vibrant colors, isolated on a white background. Made with thick rustic light brown wooden textures, thick outlines, smooth shading, cozy farming game aesthetic, high quality UI asset. A wooden sign with a cute crescent MOON and a simple month calendar page (blank squares, no numbers). NO readable text, NO letters, NO watermark.
```

K6 — boulon monnaie brico :

```
A 2D casual mobile game icon of a large rustic metal bolt token for a craft workshop currency, cartoon style, vibrant colors, isolated on a white background. Thick dark outlines, smooth cel shading, cozy farming game aesthetic, high quality UI asset. One chunky hex-head screw bolt, silver-gray steel with warm orange accent, toy-like silhouette, readable at wallet icon size. NO text, NO letters, NO watermark, NOT a gold coin.
```

**Soir 2026-09-23 — tri Comfy :** KEEP: … · RETRY: … · DROP: …  
**Ensuite :** ChatGPT sur chaque KEEP (§ Pipeline icônes UI) → PNG final dans Dump.  
**Prompts J+1 :** (à remplir après session)

---

## Prompts types (bandeau Plantfischstein — Kontext, hors pipeline icônes)

**Job A — une passe (actuel)**  
Voir graphe Kontext / conversation Cursor (REPLACE truite+poireau → chimère image2).

**Job B — 2e passe (si job A décor OK, perso raté)**  
Même images + resize ; prompt court :

```
Edit image1. Change ONLY the creature area (center-left). Keep workshop 100% identical.
ONE chimera like image2. No separate fish, no separate leek. No fish head.
```

**Job C — bandeau seul (sans image2)**  
Une seule Load Image atelier → `image1` ; retirer câble `image2` ; prompt chimère **décrite en texte** (plus dur, à garder en secours).

---

## Session soir avec Cursor (checklist)

1. Déposer les **KEEP** Comfy (`output/krea_dump_*.png`) — pas besoin des DROP.
2. Dire : **KEEP / RETRY / DROP** (1 phrase par image) · quels objets passent en **ChatGPT** ce soir.
3. Lancer **ChatGPT** (IMAGE1 KEEP + IMAGE2 ref) · ranger les OK dans `Dump/Ui/…`.
4. Demander : **« prépare la liste prompts pour demain »** → fiche J+1 + `PROMPT_generation_icones.md` si besoin.
5. Pas de commit obligatoire — auteur commit quand il veut.

---

## Ce qu’on ne fait pas (volontairement)

- Pas de boucle agent Comfy ↔ Cursor ↔ OpenClaw.
- Pas de validation auto du rendu.
- Pas de Qwen sur 1060 (plan B = ChatGPT ou autre machine).

---

## Références

- Stack : `Notes/Art/GUIDE_comfy_flux_models_local.md`
- Bandeau brief : `Notes/Art/PROMPT_bandeau_atelier_bricolage.md`
- Workflow ship : `Notes/Art/WORKFLOW_creation_assets.md`
