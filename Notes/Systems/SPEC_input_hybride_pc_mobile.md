# Input hybride PC + mobile

**Contexte :** audit Bezy (2026-09-23) · aligné `Notes/Todo_project.md` (farm slice 1 PC / slice 2 tactile).

## État des lieux (confirmé dans le repo)

| Zone | Fichier | Pattern |
|------|---------|---------|
| Ferme — pointeur | `FarmPointerInput.cs` | `Mouse.current` / `Touchscreen.current` direct |
| Ferme — caméra | `FarmCameraInput.cs` | idem (+ pinch 2 doigts) |
| Asset global | `Assets/InputSystem_Actions.inputactions` | Template Unity « Player » + « UI » |
| Runner gameplay | — | **Pas encore de scripts** consommateurs |
| Détection plateforme / schéma | — | **Absent** (`Application.isMobilePlatform`, etc.) |
| Wrapper C# généré | — | **Non** (pas de `InputSystem_Actions.cs`) |

### Bindings groupe `Touch` sur la map **Player** (gameplay)

Seulement **2** entrées liées à Touch :

- `Look` → `<Pointer>/delta` (partagé Keyboard&Mouse + Touch)
- `Attack` → `<Touchscreen>/primaryTouch/tap`

**Manquant pour un runner tactile :** `Move`, `Jump`, `Sprint`, `Crouch`, `Interact`, etc. — aucun binding Touch.

La map **UI** est complète côté Touch (`Point`, `Click`, …).

## Décision d’architecture (recommandée)

### Pattern double — ne pas tout unifier

1. **Ferme (pointer spatial, multi-touch, UI overlap)**  
   Garder `FarmPointerInput` / `FarmCameraInput` en lecture **directe** des devices.  
   Raisons : slop tap vs drag, pinch, `IsOverUi(pointerId)`, priorité clic gauche ≠ pan — difficile à modéliser proprement dans `.inputactions` sans surcouche code identique.

2. **Runner (boutons discrets : saut, esquive, changement de voie)**  
   Passer par **`InputAction` + asset** (`InputSystem_Actions` ou map dédiée `Runner`).  
   Raisons : rebinding, schémas de contrôle, boutons virtuels UI (`InputActionReference`), tests Editor avec clavier.

3. **UI (afficher ou masquer overlays tactiles)**  
   Service léger `GameControlMode` : combine `Application.isMobilePlatform`, présence `Touchscreen.current`, et optionnellement le schéma actif du `PlayerInput`.

### Mobile runner : bindings hardware vs virtuels

- **Saut / esquive** : boutons à l’écran qui **déclenchent** les `InputAction` (pas besoin de path `<Touchscreen>/…` sur l’action si l’UI appelle `WasPerformedThisFrame` via le même action).
- **Voies (gauche/droite)** : souvent **swipe** ou boutons latéraux → soit composite dans l’asset, soit petit `RunnerLaneTouchInput` qui écrit dans une action ou appelle le controller (documenter le choix au moment du premier prototype runner).

## Travail `.inputactions`

### Court terme (sans casser le template Player)

- [ ] Ajouter une action map **`Runner`** : `Lane` (Axis ou boutons Left/Right), `Jump`, `Dodge` (slide / roll).
- [ ] Bindings **Keyboard&Mouse** + **Gamepad** (miroir logique : AD / flèches, Space, Ctrl ou S).
- [ ] Groupe **Touch** : au minimum **ne pas** réutiliser `primaryTouch/tap` pour tout (conflit avec `Attack` legacy) ; préférer actions sans binding device + **UI virtuelle**.
- [ ] Unity : cocher **Generate C# Class** sur l’asset → `RunnerInputReader` ou `InputSystem_Actions` typé.

### Optionnel — nettoyage map Player

Quand le runner consomme la map `Runner`, retirer ou renommer `Attack` si ce n’est plus le « tap mobile » global.

## Détection plateforme / schéma

`GameControlMode` (voir `Assets/Scripts/Systems/GameControlMode.cs`) :

- `PrefersTouchUi` : mobile **ou** touchscreen actif sans souris récente (heuristique éditeur + device).
- `OnModeChanged` : pour activer/désactiver un canvas « contrôles virtuels runner ».
- Ne remplace pas `PlayerInput.SwitchCurrentControlScheme` — les deux sont complémentaires.

### Swap schéma à l’exécution (PC ↔ mobile, un seul build)

Checklist playtest :

1. `PlayerInput` avec `Never Auto Switch` = **off** (ou basculer manuellement au lancement).
2. Au démarrage : `SwitchCurrentControlScheme` selon `GameControlMode` / `Touchscreen.current != null`.
3. Brancher `InputSystem.onDeviceChange` : clavier branché → `Keyboard&Mouse` ; écran tactile seul → `Touch`.
4. Vérifier que la map **UI** reste active (`InputSystemUIInputModule`).
5. Éditeur : **Device Simulator** + bascule souris / touch sans rebuild.

## Phasage produit (aligné farm)

| Slice | Périmètre |
|-------|-----------|
| 1 — PC | Runner clavier + éventuellement manette ; farm inchangée ; `enableTouchCamera = false`. |
| 2 — Mobile | Overlays runner + slice tactile farm (`NOTE_camera_view_zoom.md`) ; activer `PrefersTouchUi`. |

## Tâches Cursor (ordre suggéré)

1. [x] Spec + `GameControlMode` (ce fichier + script).
2. [ ] Map `Runner` dans `.inputactions` + génération C# (Unity Inspector).
3. [ ] `RunnerInputReader` MonoBehaviour (enable/disable map, expose Jump/Dodge/Lane).
4. [ ] Prefab HUD boutons virtuels (Bezy) branchés sur `InputActionReference`.
5. [ ] Playtest swap schéma + Device Simulator.
6. [ ] Quand gameplay existe : tests unitaires légers sur reader (optionnel).

## Hors scope immédiat

- Migrer la ferme vers `.inputactions` (peu de gain / beaucoup de risque).
- Package On-Screen Controls Unity (non requis si UI UGUI + `InputActionReference`).
