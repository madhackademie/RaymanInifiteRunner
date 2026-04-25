# Unity — scènes, navigation, Inventaire / Market (réactivité UI)

**Git** : pour un chantier de cette taille, travailler sur une **branche dédiée** et fusionner dans `main` une fois validé — **`GIT_HELPER.md` section --3--**. *(Branche navigation / UI multi-stage créée côté auteur le 2026-04-17.)*

---

Objectif : documenter les modèles de **navigation entre scènes** (et/ou couches UI) et conserver un guide de décision.  
**État runtime actuel (2026-04-24)** : la base active utilise `SceneNavigator.ShowScene` + shell `NavigationHUD` + hub `HomeScene`, avec cohabitation possible inventaire scène/prefab en transition.

---

## 1. Vocabulaire Unity

- **Scène** : fichier `.unity` ; contient GameObjects, caméras, UI, lumières, etc.
- **`SceneManager`** : API pour charger / décharger (`LoadScene`, `LoadSceneAsync`, `LoadSceneAsync` en **additive**, `UnloadSceneAsync`).
- **Mode `Single`** : une seule scène “active” de jeu à la fois (souvent) ; la nouvelle scène **remplace** l’ancienne (sauf objets `DontDestroyOnLoad`).
- **Mode `Additive`** : on **ajoute** une scène par-dessus (ou à côté) sans décharger la scène de gameplay — utile pour **superposer** UI plein écran ou sous-scènes.

---

## 2. Superposition ou pas ?

| Approche | Idée | Réactivité perçue | Complexité |
|----------|------|-------------------|------------|
| **A — UI dans la même scène** | Inventaire / Market = **Canvas + panneaux** (masqués par défaut) dans chaque scène de jeu | **Très rapide** (pas de chargement disque) | Duplication si chaque scène duplique le même prefab — mitigé par un **prefab HUD unique** |
| **B — Scène “coque” persistante** | Une scène **Bootstrap / Shell** (chargée une fois) contient **HUD global** + `EventSystem` ; les **niveaux** se chargent en **Additive** ou la “scène monde” est une seule couche | **Très rapide** pour ouvrir panneaux ; chargement niveau peut être async | Bon compromis prod / prototype |
| **C — Scènes dédiées Inventaire / Market en Additive** | Gameplay reste chargé ; `LoadSceneAsync(..., Additive)` pour `Inventory.unity` / `Market.unity` | **Bon** si scènes **légères** et **préchargées** ; sinon petit freeze au premier chargement | Gestion unload + tri Canvas / `Sort Order` |
| **D — `LoadScene(Single)` vers Inventaire** | Quitter le niveau pour aller sur une scène inventaire plein écran | **Sensation de coupure** ; coût unload/reload du niveau — rarement “instantané” sans astuces | Simple conceptuellement, lourd pour l’UX runner / ferme |

**Principe UX** : pour un bouton qu’on spamme (inventaire), éviter **D** comme action par défaut si le joueur doit **revenir au même état** de niveau sans coût.

**À trancher pour le projet** : est-ce qu’**Inventaire** et **Market** sont de **vraies scènes** (assets séparés) ou des **écrans UI** dans une scène persistante ? Les deux sont valides ; la note ci-dessous aide à décider.

---

## 3. Synchrone vs asynchrone

### Chargement **synchrone** (`LoadScene`, mode par défaut souvent `Single`)

- Le moteur **bloque** jusqu’à ce que la scène soit chargée et activée (frame(s) sans interaction fluide possible selon taille).
- OK pour **tout petit** prototype ou transitions avec **écran noir** assumé.
- **Mauvais** pour “ouvrir inventaire” si la scène est grosse.

### Chargement **asynchrone** (`LoadSceneAsync`)

- Retourne un **`AsyncOperation`** ; progression `progress` (jusqu’à **0.9** tant que `allowSceneActivation` est false — comportement Unity documenté).
- **`allowSceneActivation = false`** jusqu’à un frame où tu es prêt → évite d’**activer** la scène trop tôt (utile avec barre de chargement).
- Pour **réactivité** : lancer le chargement **en avance** (menu, chargement de niveau) ou garder la scène **déjà chargée** en additive et seulement **afficher** le Canvas.

**Règle pratique** : tout ce qui doit réagir en **moins d’une frame perçue** au clic → **pas** de `LoadScene(Single)` d’une grosse scène ; préférer **panneau UI** ou scène additive **déjà** en mémoire.

---

## 4. Modèles recommandés pour “boutons partout” + Inventaire / Market

### Option recommandée (prototype → prod légère)

1. **Scène persistante minimale** (ou objet `DontDestroyOnLoad`) : **HUD** avec boutons **Inventaire** et **Market**, `EventSystem` unique si besoin.
2. **Contenu gameplay** : scène(s) niveau en **Additive** **ou** une seule scène “monde” selon ton découpage actuel.
3. **Inventaire / Market** : au début, **prefab UI** (panneau plein écran) instancié depuis le HUD ou **scènes additives très légères** (quasi que du Canvas) chargées **une fois** au boot et **activées/désactivées** (`SetActive`) plutôt que reload à chaque clic.

`SetActive` sur un root UI + refresh liste depuis `PlayerInventory` est **synchrone** et **immédiat** côté perception.

### Si vous tenez aux **fichiers scène** séparés `Inventory.unity` / `Market.unity`

- Les mettre en **Additive**.
- Les charger **au démarrage** (écran de chargement) ou au premier accès avec **`LoadSceneAsync` + `allowSceneActivation`**, puis **ne plus décharger** (ou les garder en pool).
- **Canvas** : `Sort Order` plus élevé que le HUD gameplay pour passer devant.
- **Unload** seulement si mémoire critique (mobile).

---

## 5. Pièges fréquents

- **Plusieurs `EventSystem`** : une scène additive avec son propre `EventSystem` peut casser les clics — en garder **un seul** (souvent dans la coque persistante).
- **`Time.timeScale = 0`** : le gameplay pause mais pas toujours l’UI ; utiliser **`unscaledDeltaTime`** / `UpdateMode` sur les animators UI si besoin.
- **Références perdues** : en `Single`, tout est détruit sauf `DontDestroyOnLoad` — les singletons (`PlayerInventory`, etc.) doivent être **créés une fois** dans la bonne scène d’amorçage.
- **Ordre d’exécution** : même rappel que pour l’inventaire — `Awake` / `Start` entre objets de scènes différentes ; documenter dans **`Project Settings > Script Execution Order`** si nécessaire.

---

## 6. Décisions encore ouvertes (reste à trancher)

- [ ] **Inventaire / Market** : finaliser la cible unique runtime (**prefabs UIManager** recommandés) ou maintenir un double chemin scène/prefab documenté.
- [ ] **Superposition** : gameplay visible en fond (blur / assombrissement) ou écran opaque ?
- [ ] **Chargement** : préchargement au boot vs **à la demande** async la première fois seulement ?

Une fois tranché, mettre à jour **`Notes/Farm/SYSTEMES_carte_mentale.md`** ou un schéma UI dans **`Notes/Ui/Spec_ui.md`** avec le flux : *clic bouton → quel panneau global / quelle scène de contenu → quelles données (`PlayerInventory`, futur Economy)*.

---

## 7. Liens officiels / API

- [SceneManager](https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html) — `LoadScene`, `LoadSceneAsync`, modes `LoadSceneMode.Single` / `Additive`.
- [AsyncOperation](https://docs.unity3d.com/ScriptReference/AsyncOperation.html) — `allowSceneActivation`, `progress`.

*(Unity 6 — mêmes concepts que les versions récentes.)*
