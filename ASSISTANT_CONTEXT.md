## Assistant Context — RaymanInifiteRunner

### Rappel Bezy — crédits + skill prod (chaque session)
- **P0 ABSOLU (chaque chat) :** premier outil = `Read` `.cursor/session_pull_ok`. Gate **non annulable** par l'auteur. Si gate : crédits Bezy + script pull + **STOP** avant tout metier. Règle : `.cursor/rules/session_git_sync.mdc` (priorité absolue).
- Les **crédits Bezy se réinitialisent en dur le 30 de chaque mois** (pas le 1er, pas le dernier jour du mois si ≠ 30).
- Ex. : reset le **30 juillet**, **30 août**, **30 septembre**… (février : confirmer comportement abonnement si 30 absent).
- Planifier les jobs Bezy lourds **juste après le 30** ; éviter de laisser le stock mourir en fin de cycle.
- **Peu de temps auteur ≠ skip Bezy.** Chemin de prod : skill `/prefab-ui-3phases` — Cursor prépare le prompt, l'auteur lance 2–5 min dans Unity. Note : `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`.
- À l'ouverture : proposer un **bloc de lancement prêt** (Task ID + prefab + phase) pour le prochain job de la file.

### Etat actuel (compact)
- Projet Unity 6000.3.x : boot `Bootstrap` → shell `NavigationHUD` + inventaire via `UIManager`.
- Inventaire drop + insecte Flowering + DirtBurst : **playtests validés** (2026-07-29).
- **Arbre talents Commerce** : layout + filigrane + PA haut-droite — **playtest OK** (2026-08-05).
- **LoadingScreen** `[BZ-POLISH-011]` : Bezy + **playtest OK** (2026-08-05).
- **HomeScene** `[BZ-POLISH-012]` / `[P0-HOME-PLAY-012]` : Bezy + **playtest OK** (2026-08-18).

### Priorités prochaine session
1. **Bezy** `[P0-AP-HUD-SCALE-001]` / `[BZ-AP-HUD-SCALE-001]` : HUD PA ×2 (`ActionPointsHudWidget` 480×120) — `PROMPTS_Bezi_action_points_scale.md` Ph.1.
2. **Auteur Prefab Mode** `Biofiltre` : poser PrimaryRow / StarRow / SecondaryRow (HUD nested, pas de moule unique).
3. Playtest FirstLvl HUD + grille iso `[P0-FARM-ISO-GRID-001]`.
4. **Bezy** `[P0-TAB-SPRITES-001]` / `[BZ-TAB-SPRITES-001]` : TabVente dernier prompt.
5. Reportés : `[P0-FARM-SPRITE-ALPHA-001]`, `[P0-SALE-QTY-RAND-001]`.

### Clos IBC + grille (2026-09-02 / 2026-08-30)
- `[P0-FARM-IBC-GRID-001]` — cuve IBC + grille **carrée** alignées, playtest **OK** 2026-09-02.
- `[P0-FARM-GRID-PLAY-001]` — grille sans colliders, clics, pose/récolte, pause/recall persistance **OK**.
- `[P0-FARM-PLANT-TOUCH-001]` — tactile inclus dans le même playtest 2026-08-30.

### Clos session 2026-08-25 / 2026-08-30 (vente)
- Jauges tooltip ★ `[P0-SALE-STAR-BARS-001]` + compteurs `[P0-SALE-STAR-PROGRESS-001]` + UI étoiles Bezy.
- Playtest 3 bandeaux ★ `[P0-SALE-STAR-PLAY-001]` — **OK 2026-08-30** (hover ★ → jauges + texte + fill live).

### Direction art (2026-09-08)
- Monde : **iso 2:1 cartoon**. Modèles **Township + The Tribez** (sauf champ plat). Volume = biofiltres / hydro. Prompt : `Notes/Art/PROMPT_assets_monde_iso.md`.

### Contexte Git
- Branche : **`main`** — iso 2:1 mergé (ex-`feature/biofiltre-isometric`, branche supprimée locale + remote).

### Rappel « tâche du jour »
- Lire `Notes/Todo_project.md` § *Prochaine session* + `PROJECT_LOG.md`.
- Workflow Bezy prod : `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`
- Layout PA : `Notes/Ui/CONVENTION_hud_pa_safe_zone.md`

### Références talent tree
- Workflow : `Notes/Ui/WORKFLOW_creation_arbre_talents.md` (§ Polish / backlog)
- Backlog IDs : `BL-INV-TALENT-001` … `004` dans `Notes/Todo_project.md`
