# Validation UI responsive — shell NavigationHUD + popups

**Création :** 2026-09-23  
**IDs :** `[P0-UI-RESPONSIVE-VALID-001]` (playtest + décisions) · scan détaillé `[CT-UI-RESPONSIVE-SCAN-001]`  
**Statut tâches :** `Notes/Todo_project.md` § P0 UI responsive

**À lire en priorité** avant tout chantier scaler / Safe Area / Bezy sur le shell.

---

## Décision auteur (2026-09-23)

| Zone | Verdict session |
|------|-----------------|
| **Farm / harvest / zoom caméra** | **Pas prioritaire** pour la validation UI responsive — playtest PC caméra OK (`[P0-FARM-CAMERA-VIEW-001]`). Pan mobile / vertical : voir `[P0-FARM-CAMERA-PAN-LONGPRESS-001]` (hors lot shell UI). |
| **Shell + overlays + popups** | **À valider** : `NavigationHUD`, écrans `UIManager`, popups (`ScreenPopupHost` + prefabs). |

Théorie ratios / constantes 140·128·260 : `Notes/Ui/NOTE_ui_responsive_ratios.md` (ne pas recoder avant validation).

---

## Ce qui est déjà en place (audit Bezy + repo)

- **Référence overlay mobile :** Canvas **Scale With Screen Size**, **1080×1920** portrait, PPU **100**.
- **Déjà alignés :** `Bootstrap`, `HomeScene`, `NavigationHUD`, `FirstLvl`, majorité de `Assets/Prefabs/Ui/`, **`ScreenPopupHost`** (1080×1920, `matchWidthOrHeight = 0.5`).
- **Incohérences connues (shell uniquement) :**
  - `FarmPopupCanvasFactory.cs` → référence **800×600** (popups ferme overlay) — à uniformiser **après** playtest si KO visuel.
  - `BiofiltreHud.prefab` → canvas **World Space**, scaler **désactivé**, panneau **800×600** en unités monde — **hors périmètre** de cette validation (pas le même pipeline que NavigationHUD).
- **Safe Area (encoche / barre système) :** aucun script projet ; option **après** playtest shell si bords coupés sur device.

---

## Périmètre playtest `[P0-UI-RESPONSIVE-VALID-001]`

### 1. Shell — `NavigationHUD`

- Barre nav bas : **5 onglets** visibles, pas coupés (20:9).
- Mockup onglet actif (zoom / lift) : **pas de régression** (pixels verrouillés — voir note ratios §2).
- **PA** haut-droite : lisible, pas sous l’encoche (device réel ou Simulator notch).
- Transitions : Inventaire / Shop / Vente / Hub / Plus — pas de débord horizontal.

### 2. Overlays `UIManager`

- `InventoryScreen` : wallet, halo, overlay talents — insets bas (**260**) cohérents avec la nav.
- `ShopScreen`, `SaleChannelsScreen`, `FeaturesHubScreen` : titres / CTA pas collés aux bords dangereux.
- Croix / Retour accessibles (doigt + notch).

### 3. Popups

- Pipeline **`ScreenPopupHost`** : une action → un popup, lisible plein écran.
- Prefabs courants : shop item, feedback ressources, inventaire (jeter graine, etc.).
- Popups ferme créés via **`FarmPopupCanvasFactory`** : comparer au shell (si trop petit / letterbox → corriger factory en Cursor).

### Hors scope ce lot

- Refonte `BiofiltreHud` world-space.
- Slice tactile caméra `[P0-FARM-CAMERA-TOUCH-001]`.
- Responsive du **zoom onglet** ou des slots **128** (fond nav seulement si besoin).

---

## Recette rapide (Device Simulator ou Game view)

Ratios : **16:9**, **18:9**, **19.5:9** (notch), **20:9**, **4:3** (tablette).

Pour chaque ratio, checklist courte :

1. Ouvrir **Inventaire** → wallet + scroll bas OK.
2. Ouvrir **Vente** → bandeau + onglets.
3. Ouvrir **un popup** (shop ou feedback).
4. Noter : **OK** / **débord X** / **texte illisible** / **bouton sous encoche**.

PA + safe zone chrome : `Notes/Ui/CONVENTION_hud_pa_safe_zone.md`.

---

## Après playtest — ordre de travail

| Résultat playtest | Action |
|-------------------|--------|
| Shell OK sur 3+ ratios | Clore `[P0-UI-RESPONSIVE-VALID-001]` ; reporter Safe Area si plein écran OK sur SM-A137F / iPhone sim |
| KO factory farm popup | Cursor : `FarmPopupCanvasFactory` → 1080×1920 + `matchWidthOrHeight` 0.5 |
| KO bords notch | Cursor : `SafeAreaFitter` + prompt Bezy sur racine `NavigationHUD` / popup root |
| KO layout wallet / 260 | Thread `[CT-UI-RESPONSIVE-SCAN-001]` : remplir tableau dans `NOTE_ui_responsive_ratios.md` §4 |
| Bezy uniformisation prefabs | Uniquement sur écrans **validés KO** — pas de sweep global |

---

## Références

- Audit source : message Bezy scaling UI (2026-09-23).
- `Assets/Scripts/UI/Popups/ScreenPopupHost.cs` — scaler popup runtime.
- `Assets/Scripts/Farm/FarmPopupCanvasFactory.cs` — à aligner si besoin.
- Credits Bezy : reset le **30** (`Notes/Bezi/README_bezi.md`).
