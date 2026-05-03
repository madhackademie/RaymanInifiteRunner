# Feature Shop — documentation

Document de référence pour le **magasin (Shop)** : état du code, intention produit, et distinction claire avec l’**inventaire joueur**.

**Branche / historique** : travail initial journalisé dans `PROJECT_LOG.md` (2026-04-29). Checklist opérationnelle : `Notes/Todo_project.md` (section Shop).

---

## 1. Intention produit (cible)

- Le **Shop** est un écran dédié aux **offres marchandes** (catalogue, prix, monnaie, achat éventuellement vente).
- Le **seul lien souhaité avec l’inventaire joueur** (`PlayerInventory`) est, au moment de l’achat (ou équivalent), **ajouter les objets achetés dans l’inventaire** — **même principe** que lorsqu’une **récolte** ajoute un item via la logique inventaire existante (ex. `TryAdd` / pipeline déjà utilisé côté ferme).  
  Autrement dit : l’inventaire reste la source de vérité du **contenu possédé**, le shop celle du **commerce** ; on évite une deuxième logique parallèle pour « donner un item au joueur ».

### Architecture UI (choix projet)

- Conserver le **paradigme du sac** : **grille / slots**, réutilisation des **mêmes briques** que l’inventaire (`InventorySlotUI` ou variantes, prefabs, patterns de refresh) pour que l’écran magasin **reste lisible** et **aligné** avec l’écran inventaire (même « grammaire » visuelle et technique).
- Le **miroir du sac** dans le prototype actuel illustre surtout cette **continuité d’architecture** ; une fois le catalogue en place, l’écran pourra afficher des **offres** (données shop) tout en **gardant** ce socle slot/grille — éventuellement avec une **zone miroir** du sac si le design le demande (aperçu, pas comme seule grille du magasin).

### Cellules de taille variable (mise en avant produit)

- Aujourd’hui, le fallback shop (comme l’inventaire runtime) utilise un **`GridLayoutGroup`** avec un **`cellSize` unique** : **toutes les cellules ont la même taille** dans cette grille.
- Pour une **carte plus grande** et d’autres **plus petites** (promo, produit mis en avant), il faudra **compléter ou remplacer** ce layout : par exemple conteneur **vertical** + prefabs de hauteurs différentes, **`LayoutElement`** (preferred / min / flex) sur chaque ligne, **grille maison** (positions calculées), **span** sur plusieurs cellules, ou **UI Toolkit** selon la direction Unity du projet.  
  Ce n’est **pas** incompatible avec le choix « même architecture que le sac » : on garde des **widgets slot** cohérents ; c’est le **conteneur de layout** qui doit évoluer pour autoriser des tailles hétérogènes.

---

## 2. État actuel du code (réalité)

### 2.1 Ce qui existe

| Élément | Rôle |
|--------|------|
| `ScreenId.Shop` | Identifiant d’écran pour `UIManager`. |
| `NavigationHUD` | Onglet **Shop** : ouvre l’écran via `UIManager.TryShowScreen(ScreenId.Shop)`, coexistence avec Inventaire. |
| `UIManager` | Enregistre l’écran Shop ; peut **auto-créer** un fallback `RuntimeShopScreen` si aucun prefab Shop n’est configuré (`autoCreateShopScreen`). |
| `RuntimeShopScreen` | UI **runtime minimale** : grille de slots, header « Shop », fermeture. |

### 2.2 « Linkage » inventaire **aujourd’hui** — uniquement affichage

Le shop **ne** branche **pas** encore un flux « achat → inventaire ».

`RuntimeShopScreen` fait ceci **uniquement** :

1. **Référence** `PlayerInventory` (`Instance` ou injecté à l’`Initialize`).
2. **Lecture** de `playerInventory.Slots` pour **afficher** les mêmes données que l’inventaire (icônes / quantités via `InventorySlotUI.Refresh`).
3. **Abonnement** à `PlayerInventory.OnInventoryChanged` pour **rafraîchir l’affichage** quand l’inventaire change.

Donc le lien actuel avec l’inventaire est **cosmétique et de prototype** : réutilisation du **prefab de slot inventaire** et des **mêmes slots joueur** pour avoir rapidement une grille à l’écran. **Ce n’est pas** la liaison métier « déposer les achats dans l’inventaire » — cette partie **n’existe pas encore** dans l’écran Shop.

En résumé : **clone / miroir de l’affichage inventaire** + même événement de refresh ; **pas** de catalogue shop, **pas** de monnaie, **pas** d’appel d’API d’ajout d’item depuis le shop.

---

## 3. Cible technique (à revoir / à implémenter)

Les points ci-dessous sont les **écarts** par rapport à l’intention §1.

1. **Données d’écran**  
   - Introduire une source de vérité **Shop** (liste d’offres : `ItemDefinition` + prix, stock vendeur optionnel, etc.), **distincte** de `PlayerInventory.Slots` pour le **catalogue** et les interactions d’achat — tout en pouvant **réutiliser** les mêmes **composants UI** (slots) que le sac pour l’affichage des offres.

2. **Lien inventaire (seul lien obligatoire côté possession)**  
   - Sur **validation d’achat** (ou loot scripté), appeler la **même couche** que la récolte / le gameplay pour **ajouter** l’item au `PlayerInventory` (une seule voie d’entrée pour « le joueur reçoit un item »).

3. **UI**  
   - Remplacer ou compléter le fallback : **prefab Shop** dédié, champs Inspector renseignés (`NavigationHUD`, prefabs slots shop si différents des slots inventaire).

4. **Monnaie**  
   - Item ou compteur **Argent** (voir todo projet) pour prix et solde **sans** confondre avec les slots « sac ».

5. **Layout vs données**  
   - **Découpler** les données affichées (offres shop ≠ slots possédés) tout en gardant si besoin la **même famille** de prefabs / scripts slot. Prévoir l’**évolution du layout** si le design exige des **tailles de cellules variables** (voir §1 « Cellules de taille variable »).

---

## 4. Fichiers utiles (code)

- `Assets/Scripts/UI/Shop/RuntimeShopScreen.cs` — fallback shop + binding lecture `PlayerInventory`.
- `Assets/Scripts/Systems/UIManager.cs` — `EnsureShopScreenAvailable`, prefabs runtime shop.
- `Assets/Scripts/UI/NavigationHUD.cs` — onglet Shop.
- `Assets/Scripts/Systems/ScreenId.cs` — constante `Shop`.

Pour le flux « donner un item au joueur », réutiliser les chemins déjà utilisés ailleurs (récolte, quêtes, etc.) — **à pointer précisément** lors de l’implémentation (ex. service inventaire, `PlayerInventory.TryAdd`, selon ce qui existe dans le projet au moment du dev).

---

## 5. Synthèse

**Aujourd’hui** : le Shop est un **shell UI** qui **montre l’inventaire joueur** comme l’écran inventaire — utile pour valider navigation + **même architecture slots** que le sac.  
**Demain** : **catalogue** (données shop) + **achat** → inventaire uniquement pour **déposer les achats** ; **conserver** l’alignement UI avec le sac (slots / grammaire commune) ; **adapter le layout** (au-delà d’une grille `GridLayoutGroup` uniforme) si une **mise en avant** impose des **cellules de tailles différentes**.
