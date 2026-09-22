# Bezy — livraison des prompts (workflow auteur)

**Décision : 2026-09-15** — Bezy lit les prompts via **`Assets/Docs/Bezi/`** (`@` dans Unity). **Pas** GitHub MCP. **Pas** de commit obligatoire à chaque prompt.

## Chaîne retenue

| Étape | Où | Qui |
|-------|-----|-----|
| Spec source (Cursor) | `Assets/Docs/Bezi/PROMPTS_Bezi_*.md` · `Notes/Bezi/RULES_bezy_code.md` | **Cursor** rédige / met à jour |
| Ref Bezy (`@` Unity) | **`Assets/Docs/Bezi/PROMPTS_Bezi_*.md`** (+ `RULES_bezy_code.md`) | **Cursor** recopie le miroir à chaque job |
| Règles permanentes | `Notes/Bezi/WORKSPACE_RULES_paste_in_bezi.md` | **Auteur** une fois dans Bezy |
| Exécution | Scène/prefab ouvert + thread Bezy | **Auteur** `@` le fichier Assets + phase (ou slash prefab) |
| Git | Commit **quand tu veux** (fin session, lot Bezy) — pas lié à chaque `@` | **Auteur** |

## Ce qu’on ne fait plus

- GitHub MCP pour pousser/lire les prompts (commit parasite à chaque phase).
- `@Notes/...` dans Bezy (`Notes/` hors `Assets/`).

## Ce que Cursor doit livrer à chaque job Bezy

1. Mettre à jour **`Assets/Docs/Bezi/PROMPTS_Bezi_<sujet>.md`** (vérité doc).
2. **Recopier** le même contenu dans **`Assets/Docs/Bezi/PROMPTS_Bezi_<sujet>.md`**.
3. Te donner le **chemin Assets exact** + une ligne de lancement, ex. :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_tab_more_option.md
[BZ-TAB-MORE-001] Execute PHASE 1 only. Scene Assets/Scenes/NavigationHUD.unity must be open. No .cs. Save. List. STOP.
```

4. Prefab UI lourd : en plus, `/prefab-ui-3phases` + chemin `Assets/Prefabs/Ui/…` si applicable.

## Ce que l’auteur fait dans Unity

1. Ouvrir scène ou prefab cible.
2. Bezy : `@Assets/Docs/Bezi/…` + ligne phase (ou copier le bloc Phase du fichier si Bezy ne parse pas).
3. Keep → **commit optionnel** (pas à chaque prompt) → cocher `BEZY_QUEUE.md` quand tu commits.

## Sync Notes → Assets

Quand Cursor modifie un prompt dans `Notes/`, **toujours** mettre à jour le miroir `Assets/Docs/Bezi/` dans la même session (même contenu, pas de résumé tronqué).

## Références

- `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`
- `Notes/Bezi/README_bezi.md`
- `Notes/Bezi/BEZY_QUEUE.md`
- `Assets/Docs/Bezi/README.md`
