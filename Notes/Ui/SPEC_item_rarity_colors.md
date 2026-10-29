# Spec — rareté item & couleur des noms (polish UI)

**Statut :** backlog polish · **2026-10-29**  
**IDs tâche :** `[CT-UI-RARITY-001]` (Cursor métier + vue) · `[BZ-POLISH-020]` (prefab TMP / lisibilité)

---

## 1. Intention produit

- Le **nom d’item** affiché dans les popups (achat, vente, détail inventaire / drop, recherche vente si applicable) prend une **couleur de texte** selon la **rareté** (qualité) de l’item.
- La rareté est une **donnée jeu** portée par l’**item**, pas par l’écran shop seul : une `Laitue Mature` reste « Commun » que l’on l’achète, la vende ou la jette.
- **Hors scope V1 polish :** couleur des noms dans la **grille inventaire** (slots) — option phase 2 ; monnaie (`euro`) sans couleur rareté (neutre wallet).

---

## 2. État actuel (code)

| Élément | Constat |
|--------|---------|
| `ItemDefinition` | Pas de champ rareté — seulement `displayName`, `icon`, catégorie, etc. |
| `ShopItemDefinition` | `rarityLabel` **string** libre (`Commun` sur graines / vente laitue) — **non partagé** avec inventaire ni `ItemDefinition`. |
| `ShopItemPopupData` | Passe `RarityLabel` (string) + `DisplayName` ; pas de couleur appliquée en C#. |
| `ShopItemPopupView` | `itemNameText`, `HeaderTitle` (prefab), `rarityText` — texte brut, pas de `color` par rareté. |
| Popups concernés | Même prefab **`ShopItemPopup`** : `PopupId.ShopItemPurchase`, `InventoryItemDetail`, flux vente (`RuntimeSaleChannelsScreen` / sell). |

Réf. popups : `Notes/Ui/popup_generique.md` §2.4 / §2.4b.

---

## 3. Game design — échelle de rareté (MVP farm)

Cinq paliers **fixes** (enum), libellés UI **français** :

| Enum (`ItemRarity`) | Libellé UI | Rôle MVP |
|---------------------|------------|----------|
| `Common` | Commun | Défaut — graines, récoltes, consommables du loop farm actuel |
| `Uncommon` | Peu commun | Réservé — variantes / ingrédients un peu plus rares |
| `Rare` | Rare | Réservé — bonus récolte, items quête |
| `Epic` | Épique | Réservé — équipement / boost long terme |
| `Legendary` | Légendaire | Réservé — trophées, plantes spéciales |
| `None` | *(vide)* | **Monnaie** (`ItemInventoryBehavior.Currency`) — pas de ligne rareté |

### 3.1 Affectation — items **déjà dans le jeu** (baseline auteur)

Source : `Assets/Data/Inventaire/*.asset` + offres shop `Assets/Data/Shop/*.asset`.

| `itemId` | Display name | Rareté proposée | Notes |
|----------|--------------|-----------------|--------|
| `laitue_seed` | Laitue Seedling | **Common** | Aligné `ShopItem_LaitueSeedling.rarityLabel` |
| `roquette_seed` | Graines de roquette | **Common** | Aligné shop |
| `laitue_mature` | Laitue Mature | **Common** | Récolte standard ; pas de montée auto sans décision produit |
| `roquette_mature` | Roquette | **Common** | Idem |
| `euro` | Euro | **None** | Wallet uniquement — nom neutre (blanc / gris UI wallet) |

> **Décision MVP :** tout le contenu farm jouable aujourd’hui reste **Commun** ; les paliers supérieurs existent dans l’enum pour les prochains items sans refactor. Si le design veut distinguer **récolte > graine**, trancher explicitement (ex. récoltes → `Uncommon`) avant implémentation.

### 3.2 Couleurs TMP (provisoire — kit UI bois / parchemin)

À centraliser dans un **`ItemRarityPalette`** (ScriptableObject `Assets/Data/Ui/ItemRarityPalette.asset`) ou constantes dans `ItemRarityVisuals` :

| Rareté | Couleur hex (face TMP) | Commentaire |
|--------|------------------------|-------------|
| Common | `#E8E8E8` | Blanc cassé — lisible sur bandeau bois sombre |
| Uncommon | `#7ECF8E` | Vert doux (proche labels graines actuels) |
| Rare | `#6EB5FF` | Bleu |
| Epic | `#C88CFF` | Violet |
| Legendary | `#FFB84D` | Or / ambre |
| None | `#FFFFFF` | Monnaie / pas de rareté |

Bezy pourra ajuster contrastes sur **`HeaderTitle`** (fond sombre) vs **`ItemName`** (parchemin) — la **source** reste l’enum + palette ; pas de hex en dur dans le prefab.

---

## 4. Architecture code (Cursor)

### 4.1 Donnée canonique

1. Ajouter **`ItemRarity`** (`Assets/Scripts/Inventory/ItemRarity.cs`) — enum ci-dessus.
2. Ajouter sur **`ItemDefinition`** :
   - `[SerializeField] private ItemRarity rarity = ItemRarity.Common;`
   - `public ItemRarity Rarity => rarity;`
   - Pour `Currency` : forcer `None` en validation éditeur ou getter (`InventoryBehavior == Currency → None`).

3. **Déprécier progressivement** `ShopItemDefinition.rarityLabel` :
   - Lecture : `ItemDefinition.Rarity` → libellé via palette ;
   - Fallback temporaire : si label shop non vide et item sans enum migré, mapper `"Commun"` → `Common` (parser minimal).

### 4.2 API affichage (réutilisable popups)

```csharp
// Fichiers cibles (noms indicatifs)
// Assets/Scripts/Inventory/ItemRarityVisuals.cs
// Assets/Scripts/Inventory/ItemRarityPalette.cs (SO optionnel)

public static ItemRarity ResolveRarity(ItemDefinition item);
public static string GetDisplayLabel(ItemRarity rarity);   // FR
public static Color GetNameColor(ItemRarity rarity);
public static void ApplyNameStyle(TMP_Text text, ItemDefinition item); // text + color
```

**Règle :** les **vues** (`ShopItemPopupView`, futur slot inventaire) appellent **`ItemRarityVisuals`** — pas de switch rareté dans chaque écran.

### 4.3 Flux popup

1. **`ShopItemPopupData`** : ajouter `ItemRarity Rarity` (ou `ItemDefinition` ref readonly pour éviter duplication).
2. **Controllers** (`ShopItemPopupController`, `RuntimeShopScreen`, `InventoryUI`, vente) : remplir la rareté via `ItemRarityVisuals.ResolveRarity(itemDefinition)`.
3. **`ShopItemPopupView.SetItemVisuals`** :
   - `displayName` inchangé ;
   - appliquer **`ApplyNameStyle`** sur **`itemNameText`** et **`headerTitleText`** (nouveau `[SerializeField]` si `HeaderTitle` distinct) ;
   - **`rarityText`** : libellé FR + **même couleur** (ou alpha 0.85) ; masquer si `None`.

### 4.4 Surfaces UI (checklist polish)

| Surface | Popup / écran | Champs colorés |
|---------|---------------|----------------|
| Achat shop | `ShopItemPurchase` | Header titre plante / nom item, ligne rareté |
| Détail inventaire / drop | `InventoryItemDetail` | Idem |
| Vente canal | sell popup (même prefab) | Idem |
| Grille shop / inventaire | `InventorySlotUI` | **Phase 2** (optionnel) |
| Toast récolte | `HarvestRewardFeedbackPopup` | **Hors scope** V1 |

---

## 5. Bezy (prefab only)

- **`Assets/Prefabs/Ui/ShopItemPopup.prefab`** : s’assurer que **`HeaderTitle`** et **`ItemName`** sont référencés dans `ShopItemPopupView` ; **`rarityText`** câblé (aujourd’hui `{fileID: 0}` sur le runtime prefab — à brancher si ligne rareté visible).
- **Ne pas** coder la logique rareté en dur dans le prefab — uniquement champs TMP exposés.
- Playtest auteur : lisibilité **blanc Commun** sur bandeau bois + parchemin.

Prompt Bezy : après merge Cursor `[CT-UI-RARITY-001]`, job **`[BZ-POLISH-020]`** (wiring TMP + contraste).

---

## 6. Migration données (auteur / Cursor)

1. Renseigner **`rarity`** sur chaque `Assets/Data/Inventaire/*.asset` (table §3.1).
2. Vérifier cohérence shop : les `rarityLabel` string peuvent rester le temps du fallback puis être vidés.
3. Playtest : ouvrir popup achat laitue, détail inventaire laitue mature, vente — même couleur **Commun**.

---

## 7. Liens

- Popups : `Notes/Ui/popup_generique.md`
- File Bezy : `Notes/Ui/TODO_Bezy_polish_semaine.md` #29
- Backlog projet : `Notes/Todo_project.md` § Shop — polish restant
