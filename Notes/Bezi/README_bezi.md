# Playbook collaboration — Bezy.ai + Cursor (Codex)

Objectif: utiliser les deux assistants comme un **binome complementaire** pour avancer vite sans casser l'architecture.

---

## Difference fondamentale

- **Bezy.ai**: excellent pour l'iteration rapide dans Unity (scene, prefab, inspector, UI visuelle, wiring runtime).
- **Cursor/Codex**: excellent pour architecture, refactor, fiabilite, documentation, roadmap, quality gate.

Regle simple:
- **Visuel / scene / prefab / branchements inspector** -> Bezy en premier (**prefabs = Bezy par defaut**, sauf contre-indication explicite de l'auteur).
- **Structure / dette technique / persistance / cloud / clean code** -> Codex en premier.

Regle Cursor : `.cursor/rules/bezi_prefab_ownership.mdc` — l'assistant code ne cree pas les prefabs UI sans instruction contraire.

---

## Repartition conseillee (projet actuel)

### Bezy.ai (execution terrain)
- mise en place d'ecrans, panels, prefabs
- branchements Unity (SerializeField, references, hierarchie)
- iteration gameplay rapide (playtest immediate)
- polish visuel de base

### Codex (consolidation)
- design des services (`IInventoryService`, `IMarketService`)
- separation des responsabilites (UI/vue vs logique vs data)
- refactor propre + suppression code mort
- persistance JSON/cloud-ready + garde-fous
- mise a jour des logs/todos/docs + check coherence globale

---

## Workflow optimal (4 etapes)

1. **Cadrage (Codex)**  
   - definir plan court + criteres de succes + risques.
2. **Implementation rapide (Bezy)**  
   - produire la version fonctionnelle dans Unity.
3. **Passe qualite (Codex)**  
   - clean/refactor, commentaires, doc, TODO, verification de coherence.
4. **Validation auteur**  
   - playtest, feedback, boucle suivante.

---

## Quand passer de l'un a l'autre

- Si tu vois des soucis de lisibilite, couplage fort, duplication, regressions:
  - basculer vers **Codex**.
- Si tu dois surtout manipuler scene/prefab/inspector rapidement:
  - basculer vers **Bezy**.

---

## Objectif de fin de sprint

- Prototype jouable valide rapidement (grace a Bezy).
- Base technique durable et documentee (grace a Codex).

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

### Layers Unity (UI)

**Référence :** `Notes/Ui/CONVENTION_layers_unity.md`

- Layer **UI** = **`m_Layer: 5`** (Canvas + tous les enfants UI).
- Index **4** = **Water** — ne pas l'utiliser pour l'UI.
- Dans les prompts Bezy, inclure le snippet layer ou `@Notes/Ui/CONVENTION_layers_unity.md`.

### Limitation d'execution Bezy (prefabs lourds)

Quand un script de construction est trop volumineux (trop d'objets/composants/wiring en une passe), Bezy peut timeout et s'interrompre.

Regle projet:
- Toujours donner des **fichiers de sortie explicites** (chemins exacts a creer/modifier).
- Toujours demander une execution en **phases sequentielles**, avec confirmation entre phases.
- Toujours preciser "ne pas rescanner tout le projet" et "reutiliser les scripts existants".

Pattern obligatoire pour UI/prefab complexe:
- **Phase 1 - Structure**: prefab vide + hierarchie GameObjects uniquement.
- **Phase 2 - Composants**: Image, Button, TMP_Text, LayoutGroup, etc.
- **Phase 3 - Wiring**: affectation des SerializeField + evenements.

**Critère de succès Bezy (anti-blocage) — 2026-07-25 :**
- Bezy livre le **wiring** uniquement ; fin de phase typique : `Save. List what changed. STOP.`
- **Ne pas** demander à Bezy de confirmer Simulate / Play Mode / rendu / playtest (« confirm it looks good »).
- Bezy ne peut pas (ou refuse de) valider un résultat runtime : il s’arrête plutôt que de deviner (cas VFX `PlantingDirtBurst` Phase 3b).
- Le **playtest auteur** (Simulate, in-game, rendu) est **toujours** l’étape suivante, hors prompt Bezy.
- Règle Cursor : `.cursor/rules/bezy_execution_phases.mdc` § *Success criteria*.

Si echec/timeout:
- Rejouer la phase en sous-etapes plus petites (sans fusionner les 3 phases).

### Workaround Bezy — bug résolution de chemin sur prefabs disque (2026-07-23)

**Symptôme :** la modification directe de GameObjects **déjà présents** dans un prefab sur disque via les actions standard Bezy échoue systématiquement (résolution de chemin). Reproductible aussi sur d’autres prefabs (ex. `ActionPointsHudWidget.prefab`).

**Contournement validé (halo inventaire) :**
1. Instancier temporairement le prefab dans **`Bootstrap.unity`** (où la résolution de chemin fonctionne).
2. Appliquer les mods sur l’instance (Animator, SerializeField, layers).
3. Régénérer / Apply le prefab à son emplacement d’origine.
4. Vérifier que le **GUID** du prefab est **inchangé** (ex. `PlayerHaloSlotUI` = `a1931597dd60ec948aeb14c6a9ccfa34`) pour ne pas casser les instances (`PlayerHaloPanel`).
5. Nettoyer toute instance temporaire dans Bootstrap + supprimer artefacts `TestProbe` / `DebugProbe` / `UiProbe`.

**Quand l’utiliser :** wiring Animator / composants sur prefab existant si Phase « Add Component sur racine » échoue en boucle.

### Workaround Bezy — scène doit être ouverte dans l’Editor (2026-07-29)

**Symptôme :** `updateGameObject` / `addOrUpdateComponent` sur une scène **non chargée** renvoie succès mais **zéro effet** au re-read (ex. `NavigationHUD.unity` alors que seul `Bootstrap.unity` est ouvert).

**Cause :** Bezy Actions ne mute que les scènes actuellement ouvertes ; API sans « open existing scene » (seulement `createScene`).

**Contournement :**
1. Auteur : double-clic la scène cible dans Project (ex. `Assets/Scenes/NavigationHUD.unity`) pour la charger.
2. Relancer la phase Bezy de wiring.
3. Vérifier le diff Git / YAML (`Animator`, `m_Transition: 3`, `m_Layer: 5`) avant de marquer OK.

**Exemple :** `[BZ-POLISH-006]` Ph.2 — `Notes/Ui/PROMPTS_Bezi_nav_tabs_press.md`.

Exemple de sorties explicites:
- `Assets/Prefabs/UI/ShopItemPopup.prefab` (creer/modifier)
- `Assets/Scripts/UI/Shop/ShopItemPopupView.cs` (reutiliser)
- `Assets/Scripts/UI/Shop/ShopItemPopupController.cs` (reutiliser)
- `Assets/Scripts/UI/Shop/ShopItemPopupData.cs` (reutiliser)

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

## Convention data - Item Shop

Pour ce projet, un "item shop" est compose de:
- **Base item**: `ItemDefinition` (identite, nom, icone, stack, etc.)
- **Couche shop**: donnees necessaires a la vente (prix unitaire, min/max quantite, regles d'achat)

Concretement:
- La popup UI reste en `MonoBehaviour` (`ShopItemPopupView` / `ShopItemPopupController`).
- Les donnees affichees/achetees sont projetees dans un modele popup (`ShopItemPopupData`).
- Le refactor cible pourra introduire une definition shop dediee (ex: `ShopOfferDefinition`) qui reference `ItemDefinition` + champs shop.

Objectif:
- Eviter de coupler la logique UI/achat au modele d'inventaire brut.
- Permettre plusieurs offres shop pour un meme `ItemDefinition` sans casser l'existant.

---

## À compléter (projet)

Statut officiel des items à faire : **`Notes/Todo_project.md`** (source unique).

- Version Unity utilisée + packages critiques (Cinemachine, TMP, etc.).
- Scène(s) de travail habituelle(s).
- Conventions dossiers `Assets/_Project/...` (si figées).
- Lien ou résumé **bezi.actions** quand la doc / usage est clarifié.
