# Workflow prod — skill Bezi `/prefab-ui-3phases`

**Pourquoi cette note :** l’auteur a **peu de temps Unity**. Les crédits Bezy se perdent.  
Le skill sert à **consommer les crédits en minutes**, pas en session Unity de 2 h.

Miroir du skill (détail technique) : `Notes/Bezi/SKILL_prefab_ui_3_phases.md`  
Install runtime Bezi : `%AppData%\Roaming\com.bezi.app\skills\prefab-ui-3phases\SKILL.md`  
Slash : `/prefab-ui-3phases`

---

## À l’ouverture du projet (obligatoire)

Cursor doit, dès le bootstrap / « tâche du jour » :

1. Rappeler les **crédits Bezy** (reset en dur le **30** de chaque mois).
2. Pointer **cette note** comme chemin de prod UI.
3. Sortir un **bloc de lancement prêt** pour le prochain job Bezy de `Notes/Todo_project.md` / `Notes/Ui/TODO_Bezy_polish_semaine.md` :
   - Task ID, chemin `Assets/Prefabs/Ui/…`, phase `1|2|3`, fichier `PROMPTS_Bezi_*.md`.
4. Si le prompt n’existe pas encore : **le rédiger tout de suite** (3 phases, &lt; 3500 car. chacune) — ne pas attendre une « vraie » session Unity.
5. **Ne jamais reporter Bezy « faute de temps auteur »** : c’est précisément le cas d’usage du skill.

L’auteur n’ouvre Unity **que** pour coller le slash + `@` le prompt.

---

## Chaîne (qui fait quoi)

| Étape | Qui | Temps auteur |
|-------|-----|----------------|
| Spec + `Notes/Ui/PROMPTS_Bezi_*.md` (1 fichier, 3 phases séparées) | **Cursor** (async, même hors Unity / téléphone) | 0 |
| Prefab Mode : ouvrir le prefab cible | Auteur | ~30 s |
| Thread Bezi : `@` le prompt → `/prefab-ui-3phases` Phase N | Auteur | 2–5 min |
| Review `git diff` du prefab, préparer Phase N+1 | **Cursor** | 0 |
| Playtest Simulate / device | Auteur, **plus tard** | session playtest |

Bezi = hiérarchie / composants / wiring Inspector.  
Cursor = scripts C#, specs, prompts, revue YAML.  
Auteur = lancer le skill + playtest hors prompt.

---

## Recette de lancement (Bezi)

Nouveau thread, **un seul sujet**, prefab déjà ouvert en Prefab Mode (recommandé Ph.1–2, **obligatoire** Ph.3) :

```
/prefab-ui-3phases
Task ID: [BZ-XXX-NNN]
Prefab: Assets/Prefabs/Ui/<Nom>.prefab
Phase: 1
```

Puis `@Notes/Ui/PROMPTS_Bezi_<sujet>.md` (même dossier Unity).

Fin de phase attendue : `Save. List what changed. STOP.`  
Ensuite : Cursor review → auteur relance **Phase 2** (même IDs) → idem Phase 3.

---

## Interdits (sinon ça bloque ou ça brûle du temps)

- Fusionner Ph.1+2+3 dans un seul appel (même si on dit « all » → le skill ne fait que la Ph.1).
- Demander Simulate / Play Mode / « confirm it looks good » à Bezi.
- Rescanner tout le projet ; inventer un chemin prefab.
- Unpack `UiStarRow` / `UiStarSlot`.
- Réécrire du C# métier dans Bezi sans OK auteur.
- Recoller des Workspace Rules / GitHub MCP comme chaîne principale — **abandonné** (branche `cursor/bezi-workspace-rules-skill-76a4` supprimée 2026-08-29). Le `@` local + le skill suffisent.

---

## Si Bezi no-op / timeout

1. Vérifier Prefab Mode (ou scène ouverte pour le wiring scène).
2. Rejouer **la même phase** en sous-étapes — ne pas fusionner.
3. Workaround disque (instancier dans `Bootstrap.unity` → apply → supprimer l’instance) **seulement** si l’auteur l’approuve. Détail : `Notes/Bezi/README_bezi.md`.

---

## Références

- Ownership prefabs : `.cursor/rules/bezi_prefab_ownership.mdc`
- Phases / limite 3500 car. : `.cursor/rules/bezy_execution_phases.mdc`
- Layers UI = 5 : `Notes/Ui/CONVENTION_layers_unity.md`
- File polish : `Notes/Ui/TODO_Bezy_polish_semaine.md`
