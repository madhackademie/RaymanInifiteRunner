# Étude — Bezy à distance, file d’attente et stack vocal (note de référence)

**Date :** 2026-08-30  
**Statut :** **à voir / décision auteur** — synthèse des recherches (API Bezy, remote, automatisation, vision casque BT).  
**Contexte projet :** Unity 6, skill `/prefab-ui-3phases`, prompts dans `Notes/Ui/PROMPTS_Bezi_*.md`, Cursor prépare / Bezy exécute dans l’Editor.  
**Besoins auteur :**
1. Laisser le PC allumé et gérer des jobs Bezy **à distance** (téléphone, nuit).
2. **Stack cible :** casque Bluetooth → parler à Cursor → agent local type « open CLI » → Bezy lance les phases prefab **sans** tout faire à la main.

**Fichiers opérationnels liés :**
- File d’attente : `Notes/Bezi/BEZY_QUEUE.md`
- Câblage IBC/grille/HUD : `Notes/Farm/CABLAGE_biofiltre_ibc_grille_bezi.md`

**Références internes :**
- `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`
- `Notes/Bezi/README_bezi.md` (§ « Bezi n’expose pas d’API »)
- Tentative abandonnée : branche `cursor/bezi-workspace-rules-skill-76a4` (GitHub MCP comme chaîne principale)

---

## 1. Synthèse exécutive

| Question | Réponse courte |
|----------|----------------|
| Peut-on lancer Bezy **sans** Unity Editor ouvert ? | **Non** — Bezi est un plugin Editor, Agent Mode mute le projet en local. |
| Existe-t-il une **API HTTP** Bezy pour déclencher des prompts ? | **Non** (produit in-editor, pas d’API publique). |
| Peut Cursor **appeler** Bezy directement ? | **Non** — MCP Bezy = contexte **entrant** (GitHub, Notion…), pas exécution sortante. |
| Solution réaliste « à distance » ? | **PC allumé + accès distant à l’écran** (Parsec / Tailscale+RDP) **ou** file d’attente documentée + sessions courtes à distance. |
| Peut-on automatiser **100 %** les prefabs Bezy la nuit ? | **Non fiable** sans risque (dialogs Unity, licence, échec Agent, pas de playtest). **Partiel** possible avec scripts Editor custom (hors Bezy). |
| **Casque BT → voix → Cursor → CLI → Bezy** tout seul ? | **Non aujourd’hui** — pas de CLI Bezy ; un « agent local » = robot UI fragile. **Oui** pour voix → Cursor → prompts + file ; exécution Bezy = geste auteur ou remote court. |

**Recommandation projet :** adopter une **chaîne hybride en 3 couches** (détail § 6) plutôt que chercher une API Bezy inexistante. La **voix** s’intègre en **couche 1** (spec + file), pas en déclencheur Bezy direct.

---

## 2. Contraintes techniques (état des lieux 2026)

### 2.1 Bezi = in-editor uniquement

D’après la doc officielle ([Welcome](https://docs.bezi.com/get-started/welcome), [Agent Mode](https://docs.bezi.com/fundamentals/agent-mode)) :

- Bezi s’exécute **dans Unity Editor** via une couche TypeScript interne (Actions).
- **Pas de MCP** pour *implémenter* dans Unity — les MCP servent à **lire** Jira, GitHub, Figma, Notion, etc.
- Un seul thread peut être en **Agent Mode** à la fois (stabilité projet).
- Les threads restent **&lt; 10 prompts** par sujet.

Le repo le documente déjà :

> « Bezi n’expose pas d’API pour que Cursor déclenche Bezy en autonomie. » — `Notes/Bezi/README_bezi.md`

### 2.2 Ce que Cursor / Cloud Agent peut faire à distance (déjà en place)

| Action | Distance | Outil |
|--------|----------|-------|
| Rédiger / mettre à jour `PROMPTS_Bezi_*.md` | ✅ | Cursor (cloud ou local) |
| Préparer blocs `/prefab-ui-3phases` | ✅ | Cursor |
| Modifier scripts C#, docs, `PROJECT_LOG` | ✅ | Cursor |
| Review `git diff` prefab après Bezy | ✅ | Cursor |
| **Exécuter** Bezy Agent Mode | ❌ | Nécessite Unity + plugin |

### 2.3 Unity headless / batchmode

`Unity -batchmode -executeMethod` peut modifier des prefabs via `PrefabUtility.EditPrefabContentsScope` — **sans Bezy**.

- ✅ Utile pour tâches **mécaniques** (layers, batch rename, imports).
- ❌ Ne reproduit pas l’intelligence contextuelle Bezy (hiérarchie UI, wiring SerializeField, respect du skill 3 phases).
- ⚠️ Contredit la règle projet **prefabs UI = Bezy** (`.cursor/rules/bezi_prefab_ownership.mdc`) sauf exception auteur.

**Conclusion :** batchmode = plan B technique, pas remplacement Bezy pour la UI gameplay.

### 2.4 Ce qui a déjà été testé dans le projet

| Approche | Résultat |
|----------|----------|
| GitHub MCP + Workspace Rules pour chaîne Cursor→Bezy | **Abandonné** 2026-08-29 — `@` local + skill suffisent |
| Agent VM overnight (`PROMPT_agent_vm_*.md`) | ✅ pour **C#** ; **interdit** prefab YAML |
| Skill `/prefab-ui-3phases` | ✅ réduit chaque phase à 2–5 min **si auteur présent** devant Unity |

---

## 3. Cartographie des solutions

### Option A — Accès distant au bureau (recommandée pour « vrai » Bezy à distance)

**Principe :** PC fixe allumé, Unity ouvert (ou ouvert à la connexion), accès écran/clavier à distance.

| Outil | Type | Points forts | Points faibles |
|-------|------|--------------|----------------|
| **Parsec** | Stream bas latence | Excellent pour Unity, souris précise | PC hôte doit rester éveillé ; compte Parsec |
| **Tailscale + RDP** (Windows) / **Screen Sharing** (macOS) | VPN mesh + bureau à distance | Privé, pas de port ouvert | Latence variable ; config RDP |
| **RustDesk** | Open source, self-host possible | Gratuit, contrôle total | Qualité &lt; Parsec sur UI fine |
| **Chrome Remote Desktop** | Simple | Setup rapide | Moins adapté prefab mode fin |
| **Moonlight + Sunshine** | Game stream | Très fluide si GPU NVIDIA | Setup technique |

**Workflow type :**

1. **Jour (Cursor, téléphone OK) :** Cursor pousse sur Git une phase prête dans `Notes/Ui/PROMPTS_Bezi_*.md` + entrée file `BEZY_QUEUE.md` (§ 5).
2. **Soir / nuit :** PC ne dort pas ; auteur se connecte en remote 10–15 min.
3. Unity : pull Git → ouvrir prefab → Bezy : `/prefab-ui-3phases` + `@PROMPTS…` → **Keep** → commit local.
4. **Lendemain :** Cursor review diff sur Git.

**Coût :** électricité PC + écran éteint ; prévoir ~30–80 W idle selon machine.

---

### Option B — File d’attente documentée (sans remote, mais async)

**Principe :** pas de déclenchement automatique ; **standardiser** ce que Cursor prépare et ce que l’auteur exécute en bloc.

| Composant | Rôle |
|-----------|------|
| `Notes/Ui/PROMPTS_Bezi_*.md` | Prompts par phase (&lt; 3500 car.) |
| `Notes/Bezi/BEZY_QUEUE.md` | File lisible : Task ID, prefab, phase, statut, date |
| Cursor (cloud) | Remplit la file + prompts **sans** Unity |
| Auteur (session 15 min) | Exécute toute la file en rafale devant Unity |

**Avantage :** zéro infra ; fonctionne déjà avec le skill.  
**Limite :** l’auteur doit **physiquement** (ou via Option A) ouvrir Unity.

---

### Option C — MCP GitHub dans Bezy (lecture prompts distants)

**Principe :** connecter le MCP GitHub **dans Bezy** (pas dans Cursor) pour que Bezy lise `PROMPTS_Bezi_*.md` depuis le repo sans copier-coller.

Doc Bezi MCP : [docs.bezi.com/context/mcp](https://docs.bezi.com/context/mcp)

| Étape | Détail |
|-------|--------|
| Setup | Workspace Settings → MCP → GitHub (read-only suffit pour lire les prompts) |
| Usage | `@` fichier via contexte GitHub **ou** coller le chemin ; skill `/prefab-ui-3phases` |
| Gain | Moins de friction copier-coller ; prompts toujours à jour après `git pull` |
| Limite | **Ne déclenche pas** Bezy à distance ; ne remplace pas l’ouverture Unity |

**Complément utile** à Option A ou B — pas une solution autonome.

---

### Option D — Machine dédiée « Unity + Bezy » toujours allumée

**Principe :** mini-PC / vieux laptop branché 24/7, Unity projet cloné, accès Parsec/Tailscale uniquement.

| Avantage | Inconvénient |
|----------|--------------|
| PC principal libre | 2ᵉ machine à maintenir |
| Session Bezy isolée | Licence Unity / Bezy sur 2 postes ? (vérifier contrat) |
| Git comme bus de sync | Merge conflicts si 2 machines modifient |

**Variante cloud :** VM Windows GPU (Shadow, Paperspace) — coût mensuel, latence, **IP / licence Unity** à clarifier.

---

### Option E — Automatisation Unity sans Bezy (non recommandé par défaut)

Scripts Editor `MenuItem` ou CI qui appliquent des templates prefab.

- Exemple : menu « Apply Biofiltre slot shell Phase 1 » qui crée la hiérarchie vide.
- **Risque :** divergence avec ce que Bezy ferait ; maintenance double.
- **Quand l’envisager :** tâches 100 % répétitives (ex. dupliquer 5 slots identiques) **après** validation auteur d’exception à `bezi_prefab_ownership`.

---

### Option F — Automatisation UI (AutoHotkey / scripts OS) — déconseillé

Simuler clics dans Bezi / Unity pendant la nuit.

| Problème |
|----------|
| Fragile (dialogs, updates, focus fenêtre) |
| Pas de reprise sur erreur Agent |
| Risque corruption prefab sans review |
| Violation esprit « Save. STOP. » du skill |

**Verdict :** à éviter sauf expérimentation jetable.

---

## 4. Matrice de décision

| Critère | A Remote desktop | B File doc | C MCP GitHub Bezy | E Batch Editor |
|---------|------------------|------------|-------------------|----------------|
| Vrai Bezy Agent | ✅ | ✅ (local) | ✅ (local) | ❌ |
| Depuis téléphone | ✅ (via remote) | ❌ (prep seulement) | ❌ | ❌ |
| Setup initial | Moyen | Faible | Faible | Élevé |
| Fiabilité nuit | Moyenne* | N/A | N/A | Haute** |
| Respect règles projet | ✅ | ✅ | ✅ | ⚠️ |
| Coût | Élec + évent. Parsec | 0 | 0 | Temps dev |

\* Nécessite quand même une **connexion humaine** pour valider Keep/Undo et gérer les échecs.  
\*\* Haute pour tâches mécaniques seulement, pas équivalent Bezy.

---

## 5. Proposition opérationnelle — chaîne hybride (recommandée)

### Couche 1 — Préparation async (Cursor, 0 min Unity)

**Qui :** Cursor Cloud Agent ou session Cursor hors Unity (téléphone OK pour demander la prep).

**Livrables :**
- Phase N rédigée dans `Notes/Ui/PROMPTS_Bezi_<sujet>.md`
- Ligne ajoutée dans `Notes/Bezi/BEZY_QUEUE.md` (voir template § 5.3)
- Bloc copiable :

```
/prefab-ui-3phases
Task ID: [BZ-FARM-BIOHUD-PRIM-001]
Prefab: Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlotRow.prefab
Phase: 4
```

- `git push` sur la branche de travail

### Couche 2 — Exécution Bezy (PC allumé, 2–5 min / phase)

**Prérequis machine hôte :**

| Paramètre | Réglage suggéré |
|-----------|-----------------|
| Veille | **Désactivée** (alimentation) |
| Mise en veille écran | OK après 10–15 min |
| Unity | Projet ouvert ; prefab cible en **Prefab Mode** (Ph. 3 obligatoire) |
| Git | `git pull` avant chaque job |
| Bezy | Agent Mode ; **un thread = une phase** |

**À distance :** Option A (Parsec / Tailscale) pour envoyer le prompt depuis téléphone ou laptop léger.

### Couche 3 — Qualité (Cursor, lendemain)

- `git diff` prefab
- Checklist skill (layers 5, pas d’unpack, wiring SerializeField)
- Playtest auteur **hors** prompt Bezy
- Mise à jour `BEZY_QUEUE.md` → `[x]`

### 5.1 Checklist session remote (15 min)

1. [ ] `git pull`
2. [ ] Ouvrir prefab indiqué dans la file
3. [ ] Nouveau thread Bezy (sujet unique)
4. [ ] Coller bloc `/prefab-ui-3phases` + `@Notes/Ui/PROMPTS_Bezi_….md`
5. [ ] Attendre fin → **Keep** ou Undo si doute
6. [ ] Vérifier diff Git (GUID, layers, pas de Dump path)
7. [ ] `git commit` + `push`
8. [ ] Cocher la file + prévenir Cursor pour phase suivante

### 5.2 Configuration PC « toujours prêt »

**Windows (exemple PowerShell admin, à adapter) :**

```powershell
# Empêcher veille sur secteur (session courante)
powercfg /change standby-timeout-ac 0
powercfg /change monitor-timeout-ac 15
```

**Bonnes pratiques :**
- Fermer les apps lourdes inutiles ; laisser **Unity + Bezi** sur le projet
- Désactiver Windows Update redémarrage automatique pendant les fenêtres Bezy (ou planifier)
- **Ne pas** laisser Play Mode actif toute la nuit (inutile pour prefab work)
- Écran physique éteint OK ; session utilisateur doit rester **déverrouillée** (ou utiliser outil remote qui gère le lock)

**Sécurité :**
- Accès distant **uniquement** via VPN (Tailscale) ou Parsec avec 2FA
- Ne pas exposer RDP sur Internet public
- Verrouiller la session si absence prolongée sans remote prévu

### 5.3 Template file `BEZY_QUEUE.md`

Voir fichier créé : `Notes/Bezi/BEZY_QUEUE.md`

---

## 6. Scénarios d’usage

### Scénario 1 — « Je prépare dans le train, j’exécute le soir »

1. Cursor mobile : « Prépare Ph. 4 PRIM row biofiltre »
2. Cloud Agent push prompts + file
3. Le soir : 10 min Parsec → 1 phase Bezy → commit
4. Cursor review le lendemain

### Scénario 2 — « PC allumé la nuit, je me connecte depuis le lit »

1. PC configuré ne pas dormir (§ 5.2)
2. Unity reste sur le bon prefab / projet
3. Remote : exécuter 2–3 phases max (crédits Bezy + fatigue erreur)
4. **Stop** si dialog Unity ou Bezy bloqué — ne pas enchaîner à l’aveugle

### Scénario 3 — « Zéro remote, batch le week-end »

1. Cursor remplit toute la file en semaine (5–10 phases)
2. Samedi : session 45 min Unity, enchaînement phases avec pauses `git commit` entre chaque

---

## 7. Ce qui ne marchera probablement pas

| Idée | Pourquoi |
|------|----------|
| Webhook → lance Bezy automatiquement | Pas d’API |
| Cursor appelle Bezy via MCP | MCP Bezy = entrant seulement |
| GitHub Action modifie prefabs UI à la place de Bezy | Contredit ownership ; review YAML fragile |
| Laisser Bezy Agent tourner seul des heures | Un thread Agent à la fois ; dialogs ; Undo/Keep requis |
| iPad seul sans remote | Pas d’Unity iOS Editor pour ce projet |

---

## 8. Piste d’évolution (si besoin grandit)

| Horizon | Action |
|---------|--------|
| Court terme | Adopter **A + B + C** (remote + file + MCP GitHub read) |
| Moyen terme | Mini-PC dédié Unity (Option D) si le principal est un laptop mobile |
| Long terme | Demander à Bezi une **API / CLI** ou « scheduled agent » — feedback produit |
| Fallback technique | Menu Editor « scaffold Phase 1 shell » pour jobs 100 % mécaniques (exception documentée) |

---

## 9. Décision projet suggérée

1. **Valider** Option **A** (Parsec ou Tailscale+RDP) comme canal « à distance ».
2. **Instaurer** `Notes/Bezi/BEZY_QUEUE.md` comme file unique (statuts `[ ]/[x]`).
3. **Tester** MCP GitHub **read-only** dans Bezy pour `@` prompts sans copier-coller.
4. **Ne pas** réinvestir dans GitHub MCP côté Cursor comme déclencheur Bezy (déjà abandonné).
5. **Garder** Cloud Cursor pour **préparation** prompts — c’est déjà le gain maximal sans API Bezy.

---

## 10. Références externes

| Ressource | URL |
|-----------|-----|
| Bezi Welcome | https://docs.bezi.com/get-started/welcome |
| Bezi Agent Mode | https://docs.bezi.com/fundamentals/agent-mode |
| Bezi MCP (contexte entrant) | https://docs.bezi.com/context/mcp |
| Bezi Threads | https://docs.bezi.com/fundamentals/threads |
| Bezi Workspaces | https://docs.bezi.com/fundamentals/workspaces |
| Unity EditPrefabContentsScope | https://docs.unity3d.com/ScriptReference/PrefabUtility.EditPrefabContentsScope.html |

---

## 11. Vision « stack parfait » — casque Bluetooth + open CLI + Bezy

### 11.1 Chaîne idéale (auteur)

```
[Casque BT] → dictée vocale
     ↓
[Cursor / Cloud Agent]  « Lance BZ-XXX phase N sur prefab Y »
     ↓
[Agent local « open CLI »]  envoie le prompt à Bezy
     ↓
[Bezy Agent Mode]  modifie prefab UI
     ↓
[Git]  commit automatique (?)
```

### 11.2 Ce qui fonctionne **déjà** dans cette chaîne

| Maillon | Faisable ? | Détail |
|---------|------------|--------|
| Casque BT → micro | ✅ | Dictée OS, Whisper, ou saisie vocale dans l’app Cursor |
| Voix → Cursor | ✅ | Tu décris le job ; Cursor rédige prompt + `BEZY_QUEUE.md` + push Git |
| Cursor → repo | ✅ | Prep 24/7 sans Unity (Cloud Agent) |
| Cursor → Bezy direct | ❌ | Pas d’API, pas de MCP sortant |
| « Open CLI » → Bezy | ❌ officiel | N’existe pas ; équivalent = **automation UI** (Open Interpreter, computer use, AutoHotkey) |

### 11.3 Le trou : pas de CLI Bezy

Un « open CLI » vers Bezy serait en pratique un **daemon sur le PC Unity** qui :

- lit une file (`BEZY_QUEUE.md` ou JSON) ;
- ouvre Unity / focus Bezy ;
- colle le prompt et attend la fin Agent ;
- clique **Keep** ou **Undo**.

**Problèmes :** dialogs Unity, un seul thread Agent à la fois, mises à jour Bezy/Unity, erreurs silencieuses, **pas de playtest** — **non fiable en prod** sans surveillance humaine.

| Approche locale | Verdict |
|-----------------|---------|
| Open Interpreter / agent desktop sur le PC | POC fragile |
| Script + AutoHotkey | Déconseillé (étude § Option F) |
| Unity `-batchmode` sans Bezy | Hors règle prefab UI = Bezy (sauf exception auteur) |
| **Parsec + 5 min** depuis téléphone | **Recommandé** — fiable |

### 11.4 Stack **réaliste** proche du rêve

```
[Casque BT] → tu parles à Cursor (train, canapé, nuit)
     ↓
[Cursor]  prompt Ph.N + BEZY_QUEUE + git push
     ↓
[PC allumé]  Unity + projet ouverts
     ↓
[Toi — remote 2–5 min OU session fixe]  coller dans Bezy → Keep → commit
     ↓
[Cursor lendemain]  review git diff → phase suivante
```

**Presque mains libres :** PC fixe + Parsec sur téléphone : tu dictes le job le matin, tu colles le bloc préparé le soir au lit.

### 11.5 Ce que l’auteur devra **toujours** faire (même stack idéal)

1. Machine avec **Unity + Bezy** disponible (ou connexion remote).
2. **Keep / Undo** après chaque phase Agent.
3. **git pull / commit** (ou validation avant commit auto).
4. **Playtest** — hors prompt Bezy (règle projet).
5. **Crédits Bezy** — reset le **30** ; pas d’enchaînement infini sans contrôle.

### 11.6 Format vocal standard (proposition)

Pour que Cursor mappe ta voix vers la file sans ambiguïté :

> **« Bezy — task `[BZ-XXX-NNN]` — prefab `Assets/Prefabs/Ui/…` — phase `N` — branche `…` »**

Exemple :

> « Bezy, task BZ-FARM-BIOHUD-PRIM-001, prefab PrimarySlotRow, phase 4, branche feature rework biofiltre grid. »

Cursor doit alors : mettre à jour `PROMPTS_Bezi_*.md` si besoin, ajouter une ligne dans `BEZY_QUEUE.md`, fournir le bloc `/prefab-ui-3phases` copiable.

### 11.7 Pistes d’évolution (R&D)

| Horizon | Action |
|---------|--------|
| **Court** | Voix → Cursor → `BEZY_QUEUE.md` + remote Parsec |
| **Moyen** | MCP GitHub **read-only dans Bezy** (moins de copier-coller) |
| **Long** | Feedback produit Bezy : **API / CLI / file watcher** officiel |
| **Labo** | POC « Bezy Runner » (computer use local) — **jamais** sans review humaine |

---

## 12. Prochaine action concrète (auteur)

1. Choisir outil remote (Parsec recommandé si GPU / UI fine).
2. Configurer PC « ne pas dormir sur secteur » (§ 5.2).
3. **Test à blanc** : une phrase vocale → Cursor prépare 1 phase → remote 5 min → 1 commit.
4. Si OK : exécuter la file `BEZY_QUEUE.md` pour `[BZ-FARM-BIOHUD-*]`.
5. **Ne pas** investir tout de suite dans un open-CLI maison — ROI faible vs remote court.
