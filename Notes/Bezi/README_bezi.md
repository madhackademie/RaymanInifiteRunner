# Bezi — note de référence (projet)

Source principale : [Welcome — docs.bezi.com](https://docs.bezi.com/get-started/welcome)

Index complet pour exploration (LLM / découverte des pages) : [llms.txt](https://docs.bezi.com/llms.txt)

---

## Qu’est-ce que Bezi ?

Outil de dev jeu intégré à **Unity** : indexation **en temps réel** du projet (assets, scènes, packages, codebase, etc.). Utilise des LLMs pour trouver et appliquer le **contexte pertinent** à la tâche.

### Sécurité / IP (rappel doc officielle)

- **Aucune donnée de projet utilisée pour entraîner les modèles** (selon la doc).
- Programme sécurité & FAQ : [Security](https://docs.bezi.com/bezi/security)
- Contact sécurité : `security@bezi.com`

---

## Comment l’utiliser efficacement

### Structure de prompt recommandée

1. **État actuel** — ce que tu as maintenant  
2. **État attendu** — ce que tu veux obtenir  
3. **Format de réponse** — comment tu veux la sortie (étapes, code seul, liste, etc.)

**Bon exemple :**  
*« J’ai un système pour X. Je veux l’ajuster pour ajouter Y. Prototype ça et décris les étapes de setup nécessaires. »*

**Mauvais exemple :**  
*« Fix errors »* (trop vague)

### Qualité du prompt

- Qualité prompt = qualité réponse : être **explicite** et inclure **tout le contexte utile**.
- **Épingler** les assets / scripts / GameObjects pertinents **en ligne** avec `@` (recherche dans Unity). Voir : [Prompt tagging](https://docs.bezi.com/bezi/product/prompt-tagging)
- Joindre des **images** si utile (captures, maquettes Figma, etc.) : [Attach images](https://docs.bezi.com/bezi/product/attach-images)

### Threads (conversations)

Doc : [Using Threads](https://docs.bezi.com/fundamentals/threads)

- Un thread = série de prompts / réponses.
- Garder les threads **courts** : viser **&lt; 10 prompts** par thread.
- **Un seul sujet / une seule tâche** par thread ; ouvrir un **nouveau thread** si le sujet change.
- Les threads longs ou multi-sujets ajoutent du **bruit** et dégradent les réponses.

### Ressources communauté

- Discord Bezi (lien depuis la page Welcome) + canal tips (voir doc).

---

## Lien avec ce repo (Cursor + notes)

- **Bezi** : contexte Unity en direct, bon pour générer / ajuster dans l’éditeur avec `@` sur les objets.
- **Cursor** : bon pour architecture, gros refactors, fichiers Markdown du repo (`Notes/`, `PROJECT_LOG.md`, etc.).
- Cette note sert de **référence rapide** sans remplacer la doc officielle.

---

## À compléter (projet)

- [ ] Version Unity utilisée + packages critiques (Cinemachine, TMP, etc.)
- [ ] Scène(s) de travail habituelle(s)
- [ ] Conventions dossiers `Assets/_Project/...` (si figées)
- [ ] Lien ou résumé **bezi.actions** quand la doc / usage est clarifié
