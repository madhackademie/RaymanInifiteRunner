# Workflow prod — skill Bezi `/prefab-ui-3phases`

**Pourquoi cette note :** l’auteur a **peu de temps Unity**. Les crédits Bezy se perdent, les crédits Cursor se consument trop.  
Prefab = skill 3 phases. **C# simple (transforms, vues) = Bezy aussi** (`RULES_bezy_code.md`). Cursor prépare, n’exécute pas le visuel.

## Décision workflow (2026-09-15)

Voir **`Notes/Bezi/BEZY_PROMPT_COPYPASTE.md`** — `@Assets/Docs/Bezi/` pour Bezy ; pas GitHub MCP ; pas commit par prompt.

Miroir skill : `Notes/Bezi/SKILL_prefab_ui_3_phases.md` · Slash `/prefab-ui-3phases` (prefabs `Assets/Prefabs/Ui/` uniquement).

---


## À l’ouverture du projet (obligatoire)

Cursor doit, dès le bootstrap / « tâche du jour » :

1. Rappeler les **crédits Bezy** (reset en dur le **30** de chaque mois).
2. Pointer **cette note** comme chemin de prod UI.
3. Sortir un **bloc de lancement prêt** pour le prochain job Bezy de `Notes/Todo_project.md` / `Notes/Ui/TODO_Bezy_polish_semaine.md` :
   - Prefab : Task ID, chemin `Assets/Prefabs/Ui/…`, phase `1|2|3`, fichier `PROMPTS_Bezi_*.md`.
   - **C# simple** (transforms, vues) : Task ID, scripts exacts, bloc collable — **pas** `/prefab-ui-3phases`.
4. Si le prompt n’existe pas encore : **le rédiger tout de suite** (3 phases prefab **ou** 1 prompt C#, &lt; 3500 car. chacune) — ne pas attendre une « vraie » session Unity.
5. **Ne jamais reporter Bezy « faute de temps auteur »** : c’est précisément le cas d’usage du skill.

L’auteur n’ouvre Unity **que** pour Bezy : **`@Assets/Docs/Bezi/…`** + ligne de phase. Voir `Notes/Bezi/BEZY_PROMPT_COPYPASTE.md`.

---

## Chaîne (qui fait quoi)

| Étape | Qui | Temps auteur |
|-------|-----|----------------|
| Spec + `Assets/Docs/Bezi/PROMPTS_Bezi_*.md` (1 fichier, 3 phases séparées) | **Cursor** (async, même hors Unity / téléphone) | 0 |
| Prefab Mode : ouvrir le prefab cible | Auteur | ~30 s |
| Thread Bezy : coller le bloc Phase N (+ `/prefab-ui-3phases` si prefab `Assets/Prefabs/Ui/`) | Auteur | 2–5 min |
| Review `git diff` du prefab, préparer Phase N+1 | **Cursor** | 0 |
| Playtest Simulate / device | Auteur, **plus tard** | session playtest |

Bezi = hiérarchie / composants / wiring Inspector **+ C# simple** (transforms, vues).  
Cursor = architecture, services, specs, prompts, revue. **Pas** le C# transform/vue.  
Auteur = lancer Bezy + playtest hors prompt.

C# simple ≠ ce skill. Thread Bezy séparé : coller le prompt depuis `PROMPTS_Bezi_*.md`.  
Règle Cursor : `.cursor/rules/bezy_delegate_simple_code.mdc`.

---

## Recette de lancement (Bezi)

Nouveau thread, **un seul sujet**, prefab déjà ouvert en Prefab Mode (recommandé Ph.1–2, **obligatoire** Ph.3) :

```
/prefab-ui-3phases
Task ID: [BZ-XXX-NNN]
Prefab: Assets/Prefabs/Ui/<Nom>.prefab
Phase: 1
```

Puis coller le bloc **Phase N** depuis `Assets/Docs/Bezi/PROMPTS_Bezi_<sujet>.md` (copier-coller, pas `@`).

Fin de phase attendue : `Save. List what changed. STOP.`  
Ensuite : Cursor review → auteur relance **Phase 2** (même IDs) → idem Phase 3.

---

## Recette scène / C# simple (hors skill 3 phases)

Nouveau thread Bezy, **coller** le bloc Phase du prompt + règles courtes (layer 5, pas métier, STOP).  
Référence règles : `Notes/Bezi/RULES_bezy_code.md` — **copier** le passage utile si besoin, pas de `@` obligatoire.  
Pas de `/prefab-ui-3phases` sur un job scène seule.

- Fusionner Ph.1+2+3 dans un seul appel (même si on dit « all » → le skill ne fait que la Ph.1).
- Demander Simulate / Play Mode / « confirm it looks good » à Bezi.
- Rescanner tout le projet ; inventer un chemin prefab.
- Unpack `UiStarRow` / `UiStarSlot`.
- C# **métier / services / navigation / popup** dans Bezi (hors `RULES_bezy_code.md`).
- C# simple (transform, vue) **dans Cursor** — ça va à Bezy.
- Recoller des Workspace Rules / GitHub MCP comme chaîne principale — **abandonné** (branche `cursor/bezi-workspace-rules-skill-76a4` supprimée 2026-08-29). Le `@` local + le skill suffisent.

---

## Recette C# simple (hors skill 3 phases)

Nouveau thread Bezy, **un seul script / sujet**, scène ou script `@` :

```
@Notes/Bezi/RULES_bezy_code.md
@Assets/Docs/Bezi/PROMPTS_Bezi_<sujet>.md
```

Le prompt liste les `.cs` exacts, ce qu’il faut changer, et finit par `Save. List what changed. STOP.`  
Pas de `/prefab-ui-3phases` sur un job C#.

---

## Si Bezi no-op / timeout

1. Vérifier Prefab Mode (ou scène ouverte pour le wiring scène).
2. Rejouer **la même phase** en sous-étapes — ne pas fusionner.
3. Workaround disque (instancier dans `Bootstrap.unity` → apply → supprimer l’instance) **seulement** si l’auteur l’approuve. Détail : `Notes/Bezi/README_bezi.md`.

---

## Références

- Ownership prefabs + C# simple : `.cursor/rules/bezi_prefab_ownership.mdc` · `.cursor/rules/bezy_delegate_simple_code.mdc`
- Règles C# (texte à copier si besoin) : `Notes/Bezi/RULES_bezy_code.md`
- Phases / limite 3500 car. : `.cursor/rules/bezy_execution_phases.mdc`
- Layers UI = 5 : `Notes/Ui/CONVENTION_layers_unity.md`
- Livraison prompts : `Notes/Bezi/BEZY_PROMPT_COPYPASTE.md` (copier-coller Unity, pas GitHub MCP)
- File polish : `Notes/Ui/TODO_Bezy_polish_semaine.md`
