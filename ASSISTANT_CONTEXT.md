## Assistant Context — RaymanInifiteRunner

### Etat actuel (compact)
- Projet Unity 6000.3.x : **boot** `Bootstrap.unity` → **`GameBootstrap`** charge additivement le shell **`NavigationHUD`**, **`HomeScene`**, puis **`Inventaire`** (eager, racines masquées jusqu'à navigation) ; visibilité des scènes de contenu via **`SceneNavigator.ShowScene`** ; gameplay ferme dans **`FirstLvl`**.
- Suivi taches : **statut source unique** dans `Notes/Todo_project.md`.

### Priorités en cours (2026-06-16)
1. **[P0-INV-HALO-013]** — **Playtest** arbre Commerce (workflow étape 8) + **fix affichage** en collaboration **Bezy** (prefabs) / **Cursor** (retrait contournements runtime).
2. **[P0-IDEA-001]** — Compléter notes tablette (après playtest Commerce OK).

### Contexte Git session (2026-06-16)
- **`main`** — modifications locales **non commitées** : scripts overlay, `InventoryScreen.prefab` (binding étape 7), `Track_Commerce.prefab`.
- Symptôme : titre overlay OK, **arbre entier masqué** en jeu (ScrollRect/Mask / calques overlay).
- Contournement Cursor temporaire : `TreeMountHost` runtime (`TalentTreeOverlayController`).

### Rappel protocole gestion de projet (session)
- Lire : `WORKFLOW_PROTOCOL.md`, `ASSISTANT_CONTEXT.md`, `PROJECT_LOG.md`, `Notes/Todo_project.md`.
- Priorité immédiate — **2026-06-16** :
  - **[P0-INV-HALO-013]** playtest + fix Bezy (`PROMPTS_Bezi_talent_tree.md` § Phase 4).
  - **[P0-INV-HALO-012]** composition ~ clos (étapes 0–7) ; étape 8 → 013.

### Références clés (talent tree)
- `Notes/Ui/WORKFLOW_creation_arbre_talents.md`
- `Notes/Ui/PROMPTS_Bezi_talent_tree.md`
- `Notes/Ui/SPEC_talent_tree_layout_editeur.md`
- `PROJECT_LOG.md` (entrée 2026-06-16)
- `Notes/Todo_project.md`
