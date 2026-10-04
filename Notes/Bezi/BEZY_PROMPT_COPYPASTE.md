# Bezy — livraison des prompts (workflow auteur)

**MAJ 2026-10-04** — Bezy lit `@` **`Assets/Docs/Bezi/`** (+ `Notes/Bezi/RULES_bezy_code.md` copié/ref). **Pas** GitHub MCP.

## Politique fichiers prompt

- **Par défaut :** Cursor donne le prompt **dans le chat** (copier-coller Bezy). **Pas** de nouveau `PROMPTS_Bezi_*.md`.
- **Fichier `@` Unity :** seulement si l’auteur le **demande** pour une tâche **planifiée et non validée**, ou si le job est listé dans **`Notes/Bezi/PROMPTS_Bezi_A_FAIRE.md`**.
- **Job clos :** supprimer le fichier prompt (historique = `PROJECT_LOG.md`, archive index).

## Chaîne retenue

| Étape | Où | Qui |
|-------|-----|-----|
| Index prompts restants | `Notes/Bezi/PROMPTS_Bezi_A_FAIRE.md` | Cursor |
| Texte job | **Chat Bezy** ou `@Assets/Docs/Bezi/PROMPTS_Bezi_*.md` | Cursor → auteur |
| Règles C# visuel | `Notes/Bezi/RULES_bezy_code.md` | `@` dans Bezy si C# |
| Règles permanentes Bezy | `Notes/Bezi/WORKSPACE_RULES_paste_in_bezi.md` | Auteur une fois |
| Exécution | Scène/prefab + thread Bezy | Auteur |
| Git | Commit quand tu veux | Auteur |

## Ce que Cursor livre à chaque job Bezy

1. Bloc **copier-coller** (Task ID, READ ONLY, STOP, pas Simulate).
2. Liste des `@` scripts/prefabs.
3. **Fichier `.md` uniquement** si demande auteur ou entrée index « En cours / Planifié ».

Prefab UI lourd : `/prefab-ui-3phases` + `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`.

## Ce que l’auteur fait dans Unity

1. Ouvrir scène ou prefab cible.
2. Bezy : coller le prompt + `@` fichiers listés.
3. Keep → commit optionnel → cocher `BEZY_QUEUE.md`.

## Références

- `Notes/Bezi/README_bezi.md`
- `Assets/Docs/Bezi/README.md`
- `Notes/Bezi/BEZY_QUEUE.md`
