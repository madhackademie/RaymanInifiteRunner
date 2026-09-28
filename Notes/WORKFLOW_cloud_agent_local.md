# Alignement travail local (IDE) et agent cloud (Project)

## Problème

- L’agent **cloud** modifie un clone sur une VM (branche nommée explicitement).
- Ton **PC Unity** est un autre clone : tu ne vois les changements **qu’après** `git push` (agent ou toi) + `git pull` sur **la même branche**.
- Les fichiers **Context / Project** (`/cursor/stores/…`) ne sont **pas** dans Git : jamais visibles dans `git status` local.

## Règle d’or

| Où tu bosses | Source de vérité Git | Ce que fait l’agent |
|--------------|----------------------|---------------------|
| **PC (IDE)** | Ton dossier `M:\…\RaymanInifiteRunner` | Lit le repo **après** ton push ; ou prompts seuls sans toucher au repo distant |
| **Cloud (Project)** | Branche poussée sur `origin` | Code + `Notes/Todo_project.md` etc. → **commit + push sur la branche de session** |

Sans push sur la branche, ton `git status` local reste vide : **normal**, pas un bug IDE.

## Début de session (auteur, PC)

```powershell
git checkout feature/ui-kit-refonte   # ou la branche du jour
powershell -ExecutionPolicy Bypass -File .\scripts\session-git-sync.ps1
git log --oneline HEAD..origin/main   # optionnel : voir si main a avancé
# si besoin : git merge origin/main
```

Puis dans le chat : **branche** + **pull ok**.

## Fin de session agent cloud

1. Agent liste les fichiers modifiés.
2. **Toi ou l’agent** (si tu dis « commit » / « push ») : commit sur la **branche de travail**, puis `git push`.
3. Sur le PC : `git pull` → l’IDE et Unity voient les mêmes fichiers.

## Mode « 100 % local » (sans décalage cloud)

- Tu codes et commits **uniquement** sur le PC.
- Dans Project : *« Ne modifie pas le repo ; prépare specs / prompts Bezy / réponses à partir de mes diffs. »*
- Option avancée : worker **self-hosted** (`cursor worker start`) quand disponible — même machine que Unity.

## Mode « cloud produit, local consomme »

- Tu annonces : `Branche : feature/… — push attendu en fin de tâche.`
- L’agent travaille sur cette branche et **pousse** avant de considérer la tâche livrée (docs incluses).
- Tu `git pull` avant d’ouvrir Unity.

## Fichiers de suivi projet

- **`Notes/Todo_project.md`** : statuts tâches — doit vivre dans Git (pas seulement sur la VM).
- **`PROJECT_LOG.md`** : journal — idem.
- **Project Notes / Context** : mémoire agent, rappels ; recopier vers `Notes/` si tu veux les versionner.

Références : `WORKFLOW_PROTOCOL.md`, `GIT_HELPER.md`, `.cursor/rules/git_commits_user_only.mdc`.
