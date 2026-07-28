# UI / Popup generique — documentation

Document de référence pour le **magasin (Shop)** : état du code, intention produit, et distinction claire avec l’**inventaire joueur**.

**Branche / historique** : journal `PROJECT_LOG.md` (2026-04-29 → 2026-05-12) ; **2026-05-14** : monnaie + popup shop générique + binding `NavigationHUD` considérés **stables sur `main`**. Checklist polish shop : `Notes/Todo_project.md` / `Notes/Ui/Todo_ui.md`.

---

## 1. Intention produit (cible)

- Le **Shop** est un écran dédié aux **offres marchandes** (catalogue, prix, monnaie, achat éventuellement vente).
- Le **seul lien souhaité avec l’inventaire joueur** (`PlayerInventory`) est, au moment de l’achat (ou équivalent), **ajouter les objets achetés dans l’inventaire** — **même principe** que lorsqu’une **récolte** ajoute un item via la logique inventaire existante (ex. `TryAdd` / pipeline déjà utilisé côté ferme).  
  Autrement dit : l’inventaire reste la source de vérité du **contenu possédé**, le shop celle du **commerce** ; on évite une deuxième logique parallèle pour « donner un item au joueur ».

### Architecture UI (choix projet)

- Conserver le **paradigme du sac** : **grille / slots**, réutilisation des **mêmes briques** que l’inventaire (`InventorySlotUI` ou variantes, prefabs, patterns de refresh) pour que l’écran magasin **reste lisible** et **aligné** avec l’écran inventaire (même « grammaire » visuelle et technique).
- Le **miroir du sac** dans le prototype actuel illustre surtout cette **continuité d’architecture** ; une fois le catalogue en place, l’écran pourra afficher des **offres** (données shop) tout en **gardant** ce socle slot/grille — éventuellement avec une **zone miroir** du sac si le design le demande (aperçu, pas comme seule grille du magasin).

### Cellules de taille variable (mise en avant produit)

- Aujourd’hui, l’écran Shop runtime (comme l’inventaire runtime) utilise un **`GridLayoutGroup`** avec un **`cellSize` unique** : **toutes les cellules ont la même taille** dans cette grille.
- Pour une **carte plus grande** et d’autres **plus petites** (promo, produit mis en avant), il faudra **compléter ou remplacer** ce layout : par exemple conteneur **vertical** + prefabs de hauteurs différentes, **`LayoutElement`** (preferred / min / flex) sur chaque ligne, **grille maison** (positions calculées), **span** sur plusieurs cellules, ou **UI Toolkit** selon la direction Unity du projet.  
  Ce n’est **pas** incompatible avec le choix « même architecture que le sac » : on garde des **widgets slot** cohérents ; c’est le **conteneur de layout** qui doit évoluer pour autoriser des tailles hétérogènes.

---

## 2. État actuel du code (réalité)

### 2.1 Ce qui existe

| Élément | Rôle |
|--------|------|
| `ScreenId.Shop` | Identifiant d’écran pour `UIManager`. |
| `NavigationHUD` | Onglet **Shop** : ouvre l’écran via `UIManager.TryShowScreen(ScreenId.Shop)`, coexistence avec Inventaire. |
| `UIManager` | Enregistre / instancie les prefabs d’écrans configurés dans l’éditeur ; le Shop doit venir du prefab runtime, pas d’une génération complète de fallback en code. |
| `RuntimeShopScreen` | Écran Shop runtime branché sur prefab : modal, grille scrollable, slots issus du **catalogue**, popup détail / achat, transaction monnaie et feedback ressources insuffisantes. |
| `MarketCatalogPrototype` | Charge **`Resources/Market/market_catalog.json`** (`JsonUtility`), résout chaque ligne contre `ItemDatabase`, produit des `ListingRow` (`InventorySlot` + `UnitPrice`). |
| `Assets/Resources/Market/market_catalog.json` | Données prototype : `listings[]` avec `itemId`, `price`, `quantity` par offre. |

### 2.2 Catalogue et achat de base

- Les cellules de la grille utilisent **`InventorySlotUI.Refresh(InventorySlot)`** avec un slot **synthétique** par ligne du JSON (item + quantité affichée comme en inventaire), **pas** une copie des slots du sac du joueur.
- **`PlayerInventory`** n’est utilisé que pour **résoudre** `ItemDatabase` si aucune base n’a été injectée à l’`Initialize` (`TryResolveDatabaseFromPlayer`).
- Le clic sur une offre ouvre une popup détail / achat (`ShopItemPopupController`) avec quantité **+** / **−** et prix total dynamique.
- L’achat passe par `InventoryCurrencyAccount.TryPurchase(...)` : vérification du solde `ItemDatabase.PrimaryCurrency`, débit monnaie, `PlayerInventory.TryAdd(...)`, puis remboursement si l’ajout échoue après débit.
- Le manque de fonds est bloqué avant achat et remonte le popup **`ResourceFeedbackPopupUI`** via **`ScreenPopupHost`** + **`PopupId.ShopResourceFeedback`** (binding **`UIManager.runtimePopupBindings`**, scène **`NavigationHUD`**) ; pas de fallback texte legacy côté shop.

### 2.3 Préfab / Inspector

- Le prefab **`ShopScreen`** est la source à privilégier pour les bindings UI runtime.
- Voir `Notes/Todo_project.md` : linkage Inspector, polish UX, saisie quantité, bouton **Max**.

### 2.4 Popups Shop — mode générique strict

- Le popup d’achat item passe par **`ScreenPopupHost`** + **`ScreenPopupBinding`** + **`PopupId.ShopItemPurchase`** ; configuration dans **`UIManager.runtimePopupBindings`** (scène **`NavigationHUD`**, écran Shop).
- Le popup **feedback ressources** (fonds insuffisants, monnaie non configurée, inventaire plein, etc.) passe par le même host + **`PopupId.ShopResourceFeedback`** + prefab **`ResourceFeedbackPopup`** (même liste de bindings ; pas d’instance embarquée dans **`ShopScreen`**).
- **Mode strict** : pas de fallback legacy ; binding manquant → warning + pas d’ouverture (comportement attendu).

### 2.4b Inventaire — détail item / drop (mode strict)

- **`PopupId.InventoryItemDetail`** (`inventory.item.detail`) : détail item + quantité + drop + confirmation.
- Réutilise le prefab **`ShopItemPopup`** (même `ShopItemPopupController` / `ShopItemPopupView`) en mode **`ShopItemPopupFlowMode.Drop`**.
- Ouverture : clic slot dans **`InventoryUI`** → `ScreenPopupHost` (bindings `UIManager.runtimePopupBindings`, scène **`NavigationHUD`**, `screenId = Inventory`).
- UX Drop : icône, nom, **`ItemDefinition.Description`**, +/− / saisie / **Max**, CTA **Jeter** ; wallet masqué ; surpopup **`ConfirmOverlay`** (« Voulez-vous vraiment jeter ces ressources ? ») puis anim poubelle (**trigger `DropToTrash`**, si `dropTrashRoot` câblé par Bezy) puis `PlayerInventory.TryRemove`.
- Ouverture panel : bool Animator **`IsOpen`** (clips Open/Close existants — polish Bezy Phase 4).
- **Bezy** : binding + anims (`Notes/Ui/PROMPTS_Bezi_inventory_item_drop.md` Phases 1 / 4 / 5). Binding manquant → warning + pas d’ouverture.
- Monnaie (`ItemInventoryBehavior.Currency`) : hors grille inventaire (wallet) — non droppable via ce flux.

### 2.5 Ferme FirstLvl — sélection de graine + popup plante (mode strict lazy)

- **`ScreenId.FirstLvlFarm`** : clé logique pour les bindings **sans** prefab d’écran UIManager (scène gameplay séparée).
- **`PopupId.FarmSeedSelection`** : choix de graine (`SeedSelectionUI`).
- **`PopupId.FarmPlantHarvest`** : info plante / récolte / arrachage (`HarvestPanelUI` sur prefab **`FarmHarvestPanel`**).
- **`PopupId.FarmInventoryFeedback`** : message court inventaire plein (`ResourceFeedbackPopup`, lazy).
- **`PopupId.FarmHarvestReward`** : toast succès récolte (icône item + quantité animée, `HarvestRewardFeedbackPopup`, lazy).
- **`UIManager.runtimePopupBindings`** (`NavigationHUD`) :
  - `farm.seed.selection` → **`SeedSelectionUI.prefab`** ;
  - `farm.plant.harvest` → **`FarmHarvestPanel.prefab`** ;
  - `farm.inventory.feedback` → **`ResourceFeedbackPopup.prefab`** (partagé avec le shop).
  - `farm.harvest.reward` → **`HarvestRewardFeedbackPopup.prefab`**.
- **`ScreenPopupHost`** sur **`LevelController`** (`FirstLvl`) : bindings appliqués au **`Start`** de **`BiofiltreManager`** via **`UIManager.ApplyRuntimePopupBindingsToHost(ScreenId.FirstLvlFarm, host)`** (instanciation lazy uniquement).
- **`BiofiltreManager.farmPopupHost`** : référence **obligatoire** au `ScreenPopupHost` du `LevelController` (override scène sur l’instance `Biofiltre` dans `FirstLvl`) — **pas** de `FindFirstObjectByType` ; host absent → warning + popups non ouverts.
- Ouverture : **`TryShowPopup`** + **`Open`** ; fermeture récolte : **`HarvestPanelUI.Close`** → **`TryHidePopup(FarmPlantHarvest)`** ; graines : **`HideFarmSeedSelectionPopup`**.
- **`PlantHarvestInteractor`** : host injecté à la pose (`InjectFarmPopupHost`) ; clic plante → même pipeline.

*(Pour un nouvel écran : ajouter `PopupId`, une entrée de binding et résoudre via le host — voir `.cursor/rules/ui_popup_generic_runtime.mdc`.)*

---

## 3. Flux achat dédié (état + reste à polir)

Base en place : **mécanique d’achat** isolée (UI + logique), en s’appuyant sur la même voie d’entrée inventaire que la récolte (`PlayerInventory.TryAdd` via `InventoryCurrencyAccount.TryPurchase`). Les points ci-dessous servent maintenant de référence pour le polish.

**Mise à jour 2026-05-19** : flux §3 **livré** sur `ShopItemPopup` (branche `rework/shopitempopup` → `main`) : **+** / **−**, **saisie quantité**, **Max**, **confirmation** (overlay), **solde wallet** (`CurrencyBalanceUI` dans le Header). Polish UX visuel optionnel : **`Notes/Todo_project.md`** [CT-SHOP-002].

1. **Clic sur une offre** (slot catalogue) → ouverture d’une **fenêtre détail** avec :
   - **Image** de l’item (icône `ItemDefinition` ou équivalent) ;
   - **Prix unitaire** ;
   - **Description** de l’item (optionnelle — champ données ou texte localisé plus tard).
2. **Contrôles quantité** :
   - Boutons **+** / **−** (minimum 1, plafond selon stock vendeur / règle métier) ;
   - **Saisie directe** (clavier PC, clavier virtuel mobile) avec clamp sur [min, max] ;
   - Bouton **Max** : applique le maximum **achetable** compte tenu du **solde**, du **prix unitaire**, des plafonds (`MaxPurchaseQuantity`, stock listing, `CanFitQuantity`, etc.) ;
   - Bouton **Payer** (libellé dynamique, voir point 3).
3. **Prix total** : recalcul continu **`total = prix_unitaire × quantité`** ; le bouton de paiement (ou un libellé associé) **affiche** ce total (ex. « Payer 15 »).
4. **Confirmation** : au clic sur **Acheter**, afficher l’overlay **`ConfirmOverlay`** (« Confirmer l’achat ? » + total) ; **Confirmer** déclenche `PurchaseRequested` (sinon achat direct si overlay absent).
5. **Transaction** :
   - Si **fonds suffisants** : débit monnaie + **`TryAdd`** de l’item acheté via `InventoryCurrencyAccount.TryPurchase`, fermeture / refresh ;
   - Si **fonds insuffisants** : popup générique ressources insuffisantes (sans débit ni ajout inventaire).

**Monnaie** : modèle implémenté avec une ressource de type `ItemInventoryBehavior.Currency`, exposée par `ItemDatabase.PrimaryCurrency` et manipulée via `InventoryCurrencyAccount`.

---

## 4. Cible technique (écarts restants par rapport au §1 et au §3)

Les points ci-dessous complètent le **§3** (flux achat) et l’intention **§1**.

1. **Données d’écran**  
   - Le prototype JSON couvre déjà une **liste d’offres** ; à faire plus tard si le Shop grossit : service dédié pour ne pas alourdir `RuntimeShopScreen`.

2. **Lien inventaire (seul lien obligatoire côté possession)**  
   - Sur **validation d’achat** confirmée, appeler la **même couche** que la récolte pour **ajouter** l’item (`PlayerInventory.TryAdd`, etc.).

3. **UI**  
   - Modal détail, popup item générique shop, feedback ressources insuffisantes, **saisie quantité**, **Max**, **confirmation**, **wallet dans le popup** : **OK** (2026-05-19). Reste polish visuel optionnel [CT-SHOP-002].

4. **Monnaie**  
   - **OK** sur `main` (solde, débit, `TryPurchase`). Optionnel plus tard : durcir doubles clics / transactions concurrentes.

5. **Layout vs données**  
   - Le catalogue est déjà **découplé** des slots joueur côté données ; prévoir l’**évolution du layout** (cellules de tailles variables, §1) après le MVP achat.

---

## 5. Fichiers utiles (code)

- `Assets/Scripts/UI/Shop/RuntimeShopScreen.cs` — écran Shop runtime, grille catalogue, `HudModalBackdrop`.
- `Assets/Scripts/UI/Shop/ShopItemPopupController.cs` — popup détail, quantité, achat / vente / **drop**.
- `Assets/Scripts/UI/Inventory/InventoryUI.cs` — grille inventaire + ouverture popup drop (`PopupId.InventoryItemDetail`).
- `Assets/Scripts/Inventory/InventoryCurrencyAccount.cs` — solde, crédit, débit et achat atomique.
- `Assets/Scripts/Inventory/ItemDatabase.cs` — `PrimaryCurrency`.
- `Assets/Scripts/UI/ResourceFeedbackPopupUI.cs` — popup générique de feedback ressources insuffisantes, réutilisable hors shop.
- Ferme inventaire plein : **`PopupId.FarmInventoryFeedback`** + **`ResourceFeedbackPopupUI`** (plus de `InventoryFeedbackUI`).
- `Assets/Scripts/Market/MarketCatalogPrototype.cs` — chargement / résolution JSON.
- `Assets/Resources/Market/market_catalog.json` — données offres prototype.
- `Assets/Scripts/Systems/UIManager.cs` — prefabs runtime shop et bindings.
- `Assets/Scripts/UI/NavigationHUD.cs` — onglet Shop.
- `Assets/Scripts/Systems/ScreenId.cs` — constante `Shop`.
- `Assets/Scripts/UI/HudModalBackdrop.cs` — cohérence visuelle avec l’inventaire runtime.

Pour le flux « donner un item au joueur », réutiliser les chemins déjà utilisés ailleurs (récolte, quêtes, etc.) — **à pointer précisément** lors de l’implémentation (ex. service inventaire, `PlayerInventory.TryAdd`, selon ce qui existe dans le projet au moment du dev).

---

## 6. Synthèse

**Aujourd’hui (shop sur `main`)** : catalogue JSON en slots, modal détail, **+**/**−**, total, **popup item** via host générique, **TryPurchase** (débit + `TryAdd`), feedback ressources insuffisantes.  
**Prochaine étape (shop)** : polish §3 — UI/UX, confirmation, saisie quantité, **Max**.  
**Priorité produit globale** : étendre le **pipeline popups** à **`FirstLvl`** (graines + panneau plante) — `PROJECT_LOG.md` **2026-05-14**, `Notes/Todo_project.md`.
