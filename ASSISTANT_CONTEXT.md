## Assistant Context — RaymanInifiteRunner

### Rappel Bezy — crédits + skill prod (chaque session)
- **Ouverture :** l’auteur pull la branche **avant tout prompt, tout court** (`powershell -ExecutionPolicy Bypass -File .\scripts\session-git-sync.ps1` puis « pull ok »). L’assistant ne lance ni le script ni aucun prompt (Bezy, VM, Cursor, métier) avant confirmation. Règle : `.cursor/rules/session_git_sync.mdc`.
- Les **crédits Bezy se réinitialisent en dur le 30 de chaque mois** (pas le 1er, pas le dernier jour du mois si ≠ 30).
- Ex. : reset le **30 juillet**, **30 août**, **30 septembre**… (février : confirmer comportement abonnement si 30 absent).
- Planifier les jobs Bezy lourds **juste après le 30** ; éviter de laisser le stock mourir en fin de cycle.
- **Peu de temps auteur ≠ skip Bezy.** Chemin de prod : skill `/prefab-ui-3phases` — Cursor prépare le prompt, l’auteur lance 2–5 min dans Unity. Note : `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`.
- À l’ouverture : proposer un **bloc de lancement prêt** (Task ID + prefab + phase) pour le prochain job de la file.

### Etat actuel (compact)
- Projet Unity 6000.3.x : boot `Bootstrap` → shell `NavigationHUD` + inventaire via `UIManager`.
- Inventaire drop + insecte Flowering + DirtBurst : **playtests validés** (2026-07-29).
- **Arbre talents Commerce** : layout + filigrane + PA haut-droite — **playtest OK** (2026-08-05).
- **LoadingScreen** `[BZ-POLISH-011]` : Bezy + **playtest OK** (2026-08-05).
- **HomeScene** `[BZ-POLISH-012]` / `[P0-HOME-PLAY-012]` : Bezy + **playtest OK** (2026-08-18).

### Priorités prochaine session
1. **[P0-FARM-IBC-GRID-001]** Scale le sprite IBC pour qu’il accepte la grille (grille = source de vérité, pas l’inverse). Dump `Cuve_IBC_3quart_carre_parfait.png`.
2. Playtest vente ★ `[P0-SALE-STAR-PLAY-001]`.
3. Reportés : `[P0-FARM-SPRITE-ALPHA-001]`, `[P0-FARM-PLANT-TOUCH-001]`, `[P0-SALE-QTY-RAND-001]`.

### Clos session 2026-08-25 (vente)
- Jauges tooltip ★ `[P0-SALE-STAR-BARS-001]` + compteurs `[P0-SALE-STAR-PROGRESS-001]` + UI étoiles Bezy.

### Contexte Git
- Branche : **`feature/rework-biofiltre-grid`**.

### Rappel « tâche du jour »
- Lire `Notes/Todo_project.md` § *Prochaine session* + `PROJECT_LOG.md`.
- Workflow Bezy prod : `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`
- Layout PA : `Notes/Ui/CONVENTION_hud_pa_safe_zone.md`

### Références talent tree
- Workflow : `Notes/Ui/WORKFLOW_creation_arbre_talents.md` (§ Polish / backlog)
- Backlog IDs : `BL-INV-TALENT-001` … `004` dans `Notes/Todo_project.md`
