# Input hybride PC + mobile

**Contexte :** audit Bezy (2026-09-23) · aligné `Notes/Todo_project.md` (farm slice 1 PC / slice 2 tactile).

**Nom du projet Unity / repo (`RaymanInifiteRunner`, typo à la création) :** on **ne renomme pas** (dossiers, solution, `productName`, remote Git, etc.) tant que ce n’est pas voulu explicitement — risque de casser chemins, CI, historique. La spec parle de « gameplay course » / multiverse **sans** imposer un renommage du projet ni une nouvelle map obligatoire nommée `Runner`.

## État des lieux (confirmé dans le repo)

| Zone | Fichier | Pattern |
|------|---------|---------|
| Ferme — pointeur | `FarmPointerInput.cs` | `Mouse.current` / `Touchscreen.current` direct |
| Ferme — caméra | `FarmCameraInput.cs` | idem (+ pinch 2 doigts) |
| Asset global | `Assets/InputSystem_Actions.inputactions` | Template Unity « Player » + « UI » |
| Gameplay course (futur, hors ferme) | — | **Pas encore de scripts** consommateurs |
| Détection plateforme / schéma | — | **Absent** (`Application.isMobilePlatform`, etc.) |
| Wrapper C# généré | — | **Non** (pas de `InputSystem_Actions.cs`) |

### Bindings groupe `Touch` sur la map **Player** (gameplay)

Seulement **2** entrées liées à Touch :

- `Look` → `<Pointer>/delta` (partagé Keyboard&Mouse + Touch)
- `Attack` → `<Touchscreen>/primaryTouch/tap`

**Manquant pour un futur mode course tactile :** `Move`, `Jump`, `Sprint`, `Crouch`, `Interact`, etc. — aucun binding Touch.

La map **UI** est complète côté Touch (`Point`, `Click`, …).

## Décision d’architecture (recommandée)

### Pattern double — ne pas tout unifier

1. **Ferme (pointer spatial, multi-touch, UI overlap)**  
   Garder `FarmPointerInput` / `FarmCameraInput` en lecture **directe** des devices.  
   Raisons : slop tap vs drag, pinch, `IsOverUi(pointerId)`, priorité clic gauche ≠ pan — difficile à modéliser proprement dans `.inputactions` sans surcouche code identique.

2. **Gameplay course futur (boutons discrets : saut, esquive, voies)**  
   Passer par **`InputAction` + asset** existant `InputSystem_Actions` — **compléter la map `Player`** (ou ajouter des actions dedans), **sans** renommer le projet ni l’asset.  
   Raisons : rebinding, schémas de contrôle, boutons virtuels UI (`InputActionReference`), tests Editor avec clavier.

3. **UI (afficher ou masquer overlays tactiles)**  
   Service léger `GameControlMode` : combine `Application.isMobilePlatform`, présence `Touchscreen.current`, et optionnellement le schéma actif du `PlayerInput`.

### Mobile (gameplay course) : bindings hardware vs virtuels

- **Saut / esquive** : boutons à l’écran qui **déclenchent** les `InputAction` (pas besoin de path `<Touchscreen>/…` sur l’action si l’UI appelle `WasPerformedThisFrame` via le même action).
- **Voies (gauche/droite)** : souvent **swipe** ou boutons latéraux → soit composite dans l’asset, soit petit `RunnerLaneTouchInput` qui écrit dans une action ou appelle le controller (documenter le choix au moment du premier prototype runner).

## Travail `.inputactions`

### Court terme (sans casser le template Player)

- [ ] Compléter la map **`Player`** (pas de nouvelle map obligatoire) : ex. `Jump` / `Crouch` (esquive) / `Move` pour les voies — bindings Touch + virtuels.
- [ ] Bindings **Keyboard&Mouse** + **Gamepad** (miroir logique : AD / flèches, Space, Ctrl ou S).
- [ ] Groupe **Touch** : au minimum **ne pas** réutiliser `primaryTouch/tap` pour tout (conflit avec `Attack` legacy) ; préférer actions sans binding device + **UI virtuelle**.
- [ ] Unity : cocher **Generate C# Class** sur l’asset → classe typée depuis `InputSystem_Actions` (nom généré par Unity, pas de rename projet).

### Optionnel — nettoyage map Player

Quand le mode course consommera ces actions, ajuster `Attack` / tap Touch si le sens produit a changé — **sans** renommer l’asset ni le repo.

## Détection plateforme / schéma

`GameControlMode` (voir `Assets/Scripts/Systems/GameControlMode.cs`) :

- `PrefersTouchUi` : mobile **ou** touchscreen actif sans souris récente (heuristique éditeur + device).
- `OnModeChanged` : pour activer/désactiver un canvas de contrôles virtuels (ferme slice 2, futur gameplay, etc.).
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
| 1 — PC | Farm + UI ; `enableTouchCamera = false` ; bindings clavier sur map `Player` quand le mode course existera. |
| 2 — Mobile | Overlays tactiles + slice farm (`NOTE_camera_view_zoom.md`) ; `PrefersTouchUi`. |

## Tâches Cursor (ordre suggéré)

1. [x] Spec + `GameControlMode` (ce fichier + script).
2. [ ] Bindings Touch manquants sur map **`Player`** dans `.inputactions` + génération C# (Unity Inspector).
3. [ ] Lecteur gameplay (`InputAction` enable/disable) quand le mode course sera codé.
4. [ ] Prefab HUD boutons virtuels (Bezy) branchés sur `InputActionReference`.
5. [ ] Playtest swap schéma + Device Simulator.
6. [ ] Quand gameplay existe : tests unitaires légers sur reader (optionnel).

## Hors scope immédiat

- Migrer la ferme vers `.inputactions` (peu de gain / beaucoup de risque).
- Package On-Screen Controls Unity (non requis si UI UGUI + `InputActionReference`).
