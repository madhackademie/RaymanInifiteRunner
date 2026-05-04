# Feature Shop — documentation

Document de référence pour le **magasin (Shop)** : état du code, intention produit, et distinction claire avec l’**inventaire joueur**.

**Branche / historique** : travail initial journalisé dans `PROJECT_LOG.md` (2026-04-29) ; évolution catalogue JSON notée en **2026-05-04**. Checklist opérationnelle : `Notes/Todo_project.md` (section Shop) et `Notes/Ui/Todo_ui.md` (flux achat).

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
| `RuntimeShopScreen` | UI **runtime minimale** : modal (`HudModalBackdrop`), grille scrollable, slots issus du **catalogue** (voir §2.2). |
| `MarketCatalogPrototype` | Charge **`Resources/Market/market_catalog.json`** (`JsonUtility`), résout chaque ligne contre `ItemDatabase`, produit des `ListingRow` (`InventorySlot` + `UnitPrice`). |
| `Assets/Resources/Market/market_catalog.json` | Données prototype : `listings[]` avec `itemId`, `price`, `quantity` par offre. |

### 2.2 Affichage catalogue (sans achat)

- Les cellules de la grille utilisent **`InventorySlotUI.Refresh(InventorySlot)`** avec un slot **synthétique** par ligne du JSON (item + quantité affichée comme en inventaire), **pas** une copie des slots du sac du joueur.
- **`PlayerInventory`** n’est utilisé que pour **résoudre** `ItemDatabase` si aucune base n’a été injectée à l’`Initialize` (`TryResolveDatabaseFromPlayer`).
- **Aucun** clic sur une offre, **aucune** déduction de monnaie, **aucun** `TryAdd` depuis le shop : le prototype s’arrête à l’**affichage** + résumé des prix dans le pied de panneau.

### 2.3 Préfab / Inspector

- Un prefab **`ShopScreen`** peut exister dans le projet ; le flux documenté ici reste aligné sur le **fallback runtime** tant que le prefab n’est pas la source unique de vérité en jeu.
- Voir `Notes/Todo_project.md` : linkage Inspector, prefab dédié, ressource **Argent** (première monnaie).

---

## 3. Flux achat dédié (prochaine session — spec produit)

Objectif : **mécanique d’achat** isolée (UI + logique), en s’appuyant sur la même voie d’entrée inventaire que la récolte (`PlayerInventory.TryAdd` ou équivalent unique).

1. **Clic sur une offre** (slot catalogue) → ouverture d’une **fenêtre détail** avec :
   - **Image** de l’item (icône `ItemDefinition` ou équivalent) ;
   - **Prix unitaire** ;
   - **Description** de l’item (optionnelle — champ données ou texte localisé plus tard).
2. **Contrôles quantité** :
   - Boutons **+** / **−** (minimum 1, plafond selon stock vendeur / règle métier) ;
   - Bouton **Payer** (libellé dynamique, voir point 3) ;
   - **Polish ultérieur** : saisie de quantité au **clavier** (champ TMP / input).
3. **Prix total** : recalcul continu **`total = prix_unitaire × quantité`** ; le bouton de paiement (ou un libellé associé) **affiche** ce total (ex. « Payer 15 »).
4. **Confirmation** : au clic sur Payer, ouvrir un **popup de confirmation** (« Confirmer l’achat ? »).
5. **Après confirmation** :
   - Si **fonds suffisants** : débiter la monnaie (cf. paragraphe **Monnaie** ci-dessus), **`TryAdd`** l’item acheté, fermer les modales / rafraîchir le catalogue si besoin ;
   - Si **fonds insuffisants** : message clair **manque de fonds** (sans débit ni ajout inventaire).

**Monnaie** : introduire la **première ressource primaire « Argent »** (item dédié type `money` ou compteur dédié — à trancher en implémentation, mais **une seule** notion de solde pour les prix du JSON). Elle doit être référencée dans `ItemDatabase` si on suit le modèle « item stackable » comme les autres ressources.

---

## 4. Cible technique (écarts restants par rapport au §1 et au §3)

Les points ci-dessous complètent le **§3** (flux achat) et l’intention **§1**.

1. **Données d’écran**  
   - Le prototype JSON couvre déjà une **liste d’offres** ; à faire : **interactions** (clic → détail → achat), éventuellement **service** ou classe dédiée pour ne pas alourdir `RuntimeShopScreen` au-delà du prototype.

2. **Lien inventaire (seul lien obligatoire côté possession)**  
   - Sur **validation d’achat** confirmée, appeler la **même couche** que la récolte pour **ajouter** l’item (`PlayerInventory.TryAdd`, etc.).

3. **UI**  
   - **Modal détail** + **popup confirmation** (nouveaux prefabs ou extension du runtime) ; prefab Shop plein jeu quand le visuel sera figé.

4. **Monnaie**  
   - **Argent** comme première ressource : solde consultable, débit atomique avec l’achat (éviter double clic / race avec `IsTransitioning`-style si pertinent).

5. **Layout vs données**  
   - Le catalogue est déjà **découplé** des slots joueur côté données ; prévoir l’**évolution du layout** (cellules de tailles variables, §1) après le MVP achat.

---

## 5. Fichiers utiles (code)

- `Assets/Scripts/UI/Shop/RuntimeShopScreen.cs` — fallback shop, grille catalogue, `HudModalBackdrop`.
- `Assets/Scripts/Market/MarketCatalogPrototype.cs` — chargement / résolution JSON.
- `Assets/Resources/Market/market_catalog.json` — données offres prototype.
- `Assets/Scripts/Systems/UIManager.cs` — `EnsureShopScreenAvailable`, prefabs runtime shop.
- `Assets/Scripts/UI/NavigationHUD.cs` — onglet Shop.
- `Assets/Scripts/Systems/ScreenId.cs` — constante `Shop`.
- `Assets/Scripts/UI/HudModalBackdrop.cs` — cohérence visuelle avec l’inventaire runtime.

Pour le flux « donner un item au joueur », réutiliser les chemins déjà utilisés ailleurs (récolte, quêtes, etc.) — **à pointer précisément** lors de l’implémentation (ex. service inventaire, `PlayerInventory.TryAdd`, selon ce qui existe dans le projet au moment du dev).

---

## 6. Synthèse

**Aujourd’hui** : le Shop est un **écran modal** qui affiche un **catalogue JSON** (`market_catalog.json`) en **slots** réutilisant `InventorySlotUI`, avec résolution des items via `ItemDatabase` ; **pas d’achat**, **pas de monnaie** en jeu.  
**Prochaine étape** : **Argent** + **flux §3** (détail quantité, total sur le bouton, confirmation, succès / manque de fonds) puis **`TryAdd`** pour les items achetés.  
**Ensuite** : prefab Shop final, polish saisie clavier, layouts hétérogènes si besoin produit.
