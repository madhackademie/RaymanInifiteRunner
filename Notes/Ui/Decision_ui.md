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



## 5) Localization TMP (LanguageManager)

**Décision :** séparer `country` (market/pub, détection locale) et `language` (choix joueur, override). Au boot : langue par défaut depuis le pays ; ensuite seule la langue choisie par le joueur fait foi. Mise à jour des textes via `OnLanguageChanged` + composants type `LocalizedTMPText` (keys stables + fallback).



**Tâches d’implémentation (checklist) :** voir `Todo_ui.md` → section **LanguageManager / TextMeshPro**.



## 6) Definition of Done (UI prototype) — hors LanguageManager

- Stack panneaux fonctionne :

  - base visible

  - overlays 2-3 max

  - seuls les éléments au-dessus reçoivent les clics.

- Les transitions fade/slide sont cohérentes et finissent correctement (pas de panneau “bloqué” visible/interactif au mauvais moment).



*(Critère « changer la langue met à jour tous les TMP » est dans `Todo_ui.md` avec le LanguageManager.)*

