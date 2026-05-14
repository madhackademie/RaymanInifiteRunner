# Guide rapide — rester dans les clous (suivi projet)

Ce guide explique **comment utiliser le suivi** sans recreer de doublons.

---

## 1) Fichier de reference (obligatoire)

- **Statut officiel des taches** : `Notes/Todo_project.md`
- C'est le **seul** fichier avec les cases :
  - `[ ]` a faire
  - `[~]` en cours
  - `[x]` termine

Dans les autres notes (`Notes/Ui/*`, `Notes/Farm/*`, etc.), on garde le **detail technique** (comment faire), pas le statut.

---

## 2) Comment lire les IDs

Chaque tache a un ID stable, exemple : `P0-POP-001`.

Convention :
- `P0-*` : priorite immediate (prochaine session)
- `CT-*` : court terme actif
- `BL-*` : backlog

Principe :
- on garde le meme ID dans le temps ;
- on change le statut, pas l'ID.

---

## 3) Routine simple de session

Debut de session :
1. Lire `WORKFLOW_PROTOCOL.md`
2. Lire `ASSISTANT_CONTEXT.md`
3. Lire la derniere entree de `PROJECT_LOG.md`
4. Ouvrir `Notes/Todo_project.md` -> section **Prochaine session (priorite immediate)**

Pendant la session :
- passer une tache de `[ ]` vers `[~]` ou `[x]` dans `Notes/Todo_project.md`.
- si besoin, enrichir les notes de detail (UI/Farm/etc.) sans ajouter de cases.

Fin de session :
1. Mettre a jour `Notes/Todo_project.md` (statuts)
2. Ajouter une entree datee dans `PROJECT_LOG.md` (ce qui a ete fait, decisions, prochaine etape)

---

## 4) Ce qu'il faut eviter

- Ne pas mettre des checkboxes de suivi dans les notes satellites.
- Ne pas dupliquer une meme tache avec 2 IDs differents.
- Ne pas inventer une "tache du jour" hors des 4 docs de reference.

---

## 5) Exemple concret

Si tu veux prioriser la migration popup FirstLvl :
- dans `Notes/Todo_project.md`, tu mets `P0-POP-001` en `[~]` ;
- tu notes le detail d'implementation dans `Notes/Ui/Todo_ui.md` (sans case) ;
- en fin de session, tu traces l'avancement dans `PROJECT_LOG.md`.
