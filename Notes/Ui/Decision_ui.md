# Spec UI (Proto -> Polish)

## 1) Objectifs UI (début de projet)
- Prototype UI “farm” minimal fonctionnel : écran de départ + UI gameplay de base + menus essentiels.
- Support des animations *fade + slide* (mode polish ensuite).
- Support de la localisation TextMeshPro :
  - choix **langue** par le joueur
  - langue par défaut dérivée de la **détection pays**
- Bonus : rester compatible avec le fait que la caméra utilise **Cinemachine**.

## 2) Architecture UI (stack 2-3 layers)
### Règle de composition
- Stack max : **profondeur 2-3**.
- Un thème d’UI = un panneau base + 0..2 overlays (détails, confirm, tooltip).
- Le panneau “au-dessus” est le seul interactif :
  - overlays non top => `blocksRaycasts = false` (ou équivalent)
  - logique focus/raycast centralisée via `UIManager`

### Types de panneaux
- `BasePanel` : visible et “fond” du thème (menu principal, écran farm, etc.)
- `OverlayPanel` : popup/détail/confirmation

## 3) Animations UI (Animator + fade/slide)
### Convention de clips
Chaque `UIPanel` doit supporter :
- `Open` : fade + slide vers la position cible
- `Close` : fade + slide hors écran / vers position de sortie

### Contrat d’orchestration (event-driven)
- Quand on déclenche `Open()` / `Close()`, le `UIPanel` :
  - démarre l’Animator
  - signale la fin (via Animation Events ou callback équivalent)
- `UIManager` attend la fin de l’animation avant d’empiler/dépiler davantage (si on veut éviter les conflits).

## 4) Orchestration “séquence” (définition)
### Définition
Une **séquence** = suite ordonnée d’**étapes**, où chaque étape attend un “end condition”.
Exemples d’étapes :
- UI step : `OpenPanel / ClosePanel`
- Camera step : Cinemachine blend/switch (et on considère “fin” à la fin du blend)
- World step (plus tard) : activer/désactiver des objets greybox

### API conceptuelle (pour plus tard)
- `UISequence` / `PolishSequence` : liste d’étapes
- chaque étape expose une condition de fin (callback “done”)

## 5) Localization TMP (LanguageManager déclenché par options)
### Concepts séparés
- `country` : détection locale, utilisée pour pub/market/monétisation
- `language` : choisie par le joueur (override)

### Règles de fallback
- Au boot :
  - `language = defaultLanguageFromCountry(country)`
- Si le joueur change la langue :
  - `language override` devient la source de vérité
  - la détection `country` ne doit plus écraser la langue

### Déclenchement
- `OnLanguageChanged(newLanguage)` :
  - notifie tous les `LocalizedTMPText`
  - met à jour le texte (et gère fallback si key manquante)

### Convention de keys
- Keys stables (exemples) :
  - `BTN_PLAY`
  - `BTN_UPGRADE`
  - `TITLE_FARM`
  - `DESC_WELCOME`
- Règle fallback :
  - si key manquante dans la langue courante => fallback `EN` (ou “key” brute si aucune donnée)

## 6) Modèle de données (prototype -> manager final)
### Prototype (simple)
- Mapping rapide : `language -> key -> string` (en dur ou table temporaire)

### Final (recommandé)
- Source data : ScriptableObject ou JSON (format attendu identique)
- Toujours conserver :
  - `key` stable
  - `fallback` défini

## 7) Definition of Done (UI prototype)
- Changer la langue met à jour **tous** les textes localisés (TMP) sans recréer la scène.
- Stack panneaux fonctionne :
  - base visible
  - overlays 2-3 max
  - seuls les éléments au-dessus reçoivent les clics.
- Les transitions fade/slide sont cohérentes et finissent correctement (pas de panneau “bloqué” visible/interactif au mauvais moment).