# Comparatif playtest mobile — Batch H (2026-09-16)

**Téléphone :** Samsung SM-A137F · APK `armeabi-v7a`  
**Tâches :** `[P0-FARM-MOBILE-BIOFILTER-SCALE-001]` · `[P0-NAV-EXIT-ANCHOR-001]` (croix masquée sur KO)  
**Branche prévue :** `rework/biofilter-mobile-scale`  
**Prompt Bezy :** `Assets/Docs/Bezi/PROMPTS_Bezi_biofilter_mobile_scale.md`

---

## Cible produit (référence auteur)

| Fichier | Rôle |
|---------|------|
| `20260916_REF_p0-farm-mobile-biofilter-scale_firstlvl-target-editor-free-aspect.png` | **CIBLE** — Game view éditeur (Free Aspect) : biofiltre entier visible, marges haut/gauche pour **croix rouge** + widget PA, fond bleu, grille lisible. |

---

## État actuel mobile (KO)

| Fichier | ID todo | Constats |
|---------|---------|----------|
| `20260916_SM-A137F_p0-farm-mobile-biofilter-scale_firstlvl-biofiltre-oversized-no-exit.jpg` | `[P0-FARM-MOBILE-BIOFILTER-SCALE-001]` + exit | Biofiltre **trop grand** (~80 % hauteur écran). **Pas de croix** de sortie FirstLvl visible (HUD recadré / hors safe area). |
| `20260916_SM-A137F_p0-farm-grid-planting-only_firstlvl-grille-visible-mode-plant.jpg` | `[P0-FARM-GRID-PLANTING-ONLY-001]` | Grille visible en mode plantation ; console dev (shader silhouette) — hors scope scale. |

---

## Écart à combler

| Critère | Cible (REF) | Mobile KO |
|---------|-------------|-----------|
| Encombrement biofiltre | ~60–70 % hauteur utile, marges UI | ~80 %+ bords coupés |
| Croix `ExitButtonContainer` | Visible haut-gauche | Absente à l’écran |
| Widget PA (2/160) | Coin haut-droit, ne chevauche pas le deck | Très proche du bord |
| Cadrage global | Centré, « respiration » autour de la cuve | Cuve domine tout le viewport |

**Ordre de fix suggéré :** (1) cadrage monde FirstLvl (~**−10 %** taille apparente du biofiltre, piste `Main Camera` ortho **5 → ~5,5**) · (2) ancrage croix HUD portrait · (3) si APK ≠ Game view à résolution égale → script runtime aspect (Cursor, même branche).

---

## Validation après branche

1. Game view **1080×1920** (portrait) : coller visuellement la REF.  
2. Nouvelle capture : `20260916_SM-A137F_p0-farm-mobile-biofilter-scale_firstlvl-apres-fix.jpg`  
3. SM-A137F : même cadrage + croix visible → `…_apres-fix-device.jpg`

---

## Convention de nommage

`YYYYMMDD_SM-A137F_<id-todo-minuscules>_<slug>.{jpg|png}`

Préfixe `REF_` = référence éditeur / maquette (pas une capture téléphone).
