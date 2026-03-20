# UI Journal (Stack + Localization)

But: centraliser toutes les décisions, idées et futures tâches liées à l’UI (panneaux, animations, localisation TextMeshPro).

Règle de fonctionnement :
- Tu peux ajouter des idées en vrac dans `## Inbox (idées brutes)`
- Quand on revient dessus, je les réorganise en `## Tâches (planifiées)` et je mets à jour `## Décisions actées` / `## Questions ouvertes`.

---

## Décisions actées

### 1) Animation UI
- On démarre avec une UI simple (prototype) puis on enrichit en “polish”.
- On privilégie `Animator` (cinématiques via fade + slide) pour l’UI et les transitions de panneaux.
- `Cinemachine` est utilisé pour la caméra (les séquences UI/monde/caméra seront orchestrées plus tard).

### 2) Structure des panneaux (profondeur 2-3 layers)
- Modèle recommandé : un panneau de base + 1..2 overlays par thème (stack).
- Règle d’interaction : seul le panneau “au-dessus” reçoit les clics/raycast (les overlays sont non-interactifs quand ils ne sont pas top).

### 3) Séparation Country vs Language
- `country` sert pour les mécanismes liés à la région (publicité, market, etc.) via détection locale.
- `language` sert pour les textes UI (TextMeshPro) et est choisi par le joueur.
- Langue par défaut : au départ, `language = languageFromCountry(country)` puis l’override joueur prend le dessus.

### 4) Localization UI avec TextMeshPro
- Le mécanisme sera déclenché quand le joueur change l’option de langue.
- Le TextMeshPro affiché doit être piloté par une logique centrale (language manager + mapping key -> string).
- Idéalement : utiliser des “keys” stables côté UI pour faciliter le passage du prototype au manager final.

---

## Architecture UI cible (haut niveau)

### Stack manager (2-3 layers)
- `UIManager` (ou `UIRootController`)
  - maintient la stack de panneaux ouverts
  - gère le “push/pop” des overlays
  - évite d’ouvrir/fermer plusieurs panneaux en conflit pendant une animation (optionnel mais utile)

- `UIPanel` (composant sur prefab)
  - référence `Animator` (clip Open/Close) et éventuellement `CanvasGroup`
  - expose des méthodes : `Open()`, `Close()`
  - expose des callbacks/événements “animation finished” (via Animation Events)

### Orchestration “séquence” (pour plus tard)
- Notion de “séquence” = suite d’étapes (UI / World / Camera)
- Pour ce journal : pour l’instant on se limite à UI, mais on prépare le modèle d’étapes pour synchroniser plus tard.

---

## LanguageManager (spécification fonctionnelle)

### Rôles
- Entrée : `SetLanguage(selectedLanguage)` déclenché par le menu d’options.
- Sortie : mise à jour de tous les `TextMeshProUGUI` localisés.

### Comportement
- Au boot :
  - détecter `country`
  - calculer `defaultLanguage = languageFromCountry(country)`
  - si une override joueur existe : utiliser `override`
  - sinon : utiliser `defaultLanguage`
- Quand le joueur change la langue :
  - enregistrer l’override (plus tard : PlayerPrefs/Save)
  - notifier les textes (event)

### Données (phase prototype -> phase manager)
- Phase prototype possible :
  - mapping rapide `language -> string` par key (ou switch)
- Phase manager final :
  - table de localisation par langue (ScriptableObject/JSON)
  - fallback (ex. EN si missing)

---

## Inbox (idées brutes)

- [ ] (Vrac) …
- [ ] (Vrac) …

---

## Tâches (planifiées)

### A) UI prototype (baseline)
1. [ ] Créer 1 panneau “base” (ex. Menu/Farm) en prefab.
2. [ ] Ajouter `Animator` + clips `Open`/`Close` (fade + slide).
3. [ ] Ajouter un `UIPanel` composant qui expose `Open/Close` et gère l’activation/raycast.

### B) Stack 2-3 layers
1. [ ] Implémenter `UIManager` qui supporte `push overlay` et `pop overlay`.
2. [ ] Mettre en place la règle “seul top reçoit les clics”.

### C) Localization TextMeshPro
1. [ ] Définir une convention de keys stables pour les textes UI (ex. `BTN_PLAY`, `TITLE_FARM`).
2. [ ] Implémenter un mécanisme temporaire (prototype) qui met à jour les TMP quand `language` change.
3. [ ] Préparer le passage au `LanguageManager` final (source de données + fallback).

### D) Préparation “séquences” (plus tard)
1. [ ] Définir une API d’étapes (UI step, Camera step) pour synchroniser fade/slide + blend Cinemachine.

---

## Questions ouvertes

- [ ] Quels types de panneaux seront “modaux” (bloquent tout) vs “non-modaux” ?
- [ ] Combien de textes à localiser au démarrage (10 ? 50 ? 200) ?
- [ ] Faut-il un fallback EN ou fallback = langue du pays ?
- [ ] Où stocker `language override` (PlayerPrefs, save file, autre) ?

---

## Journal de changements (pour le futur)

Date | Changement
:---|:---
TODO | (à remplir)

