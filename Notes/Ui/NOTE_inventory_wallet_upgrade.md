# Inventaire + wallet — problématique et mise à niveau (sans script externe)

Document de référence pour **reprendre plus tard** le sujet : wallet dans l’écran inventaire **sans** extraction YAML / script Python hors Unity.

## Ce qui coince aujourd’hui

### 1. Deux « vérités » possibles pour l’UI inventaire

- **Au clic sur l’onglet Inventaire du HUD**, le flux prévu est **`UIManager.TryShowScreen(ScreenId.Inventory)`** : l’écran affiché est une **instance sous `screenRoot`** (prefab enregistré dans `UIManager`, ex. **`InventoryScreen`** + `InventoryUI`). L’ancien fallback procédural **`RuntimeInventoryScreen`** a été **retiré** du dépôt (2026-05).
- La **scène `Inventaire.unity`** peut servir de **atelier / prototype** (layout Bezi, WalletBar, etc.), mais **elle n’est pas automatiquement** l’écran que le joueur voit tant qu’on ne **charge pas** cette scène additive ou qu’on n’en **exporte pas** le panneau vers un prefab branché sur `UIManager`.

Sans alignement explicite (prefab ou navigation vers la scène), le wallet « bien ancré » dans **Inventaire.unity** **ne s’affiche pas** à la place où le joueur ouvre l’inventaire depuis le HUD.

### 2. Ancien fallback `RuntimeInventoryScreen` (historique)

Ce script **construisait** une grille + header en **code** ; il ne reflétait pas la maquette éditoriale (WalletBar, etc.). **Décision** : **prefab d’écran complet** (`InventoryScreen` + `InventoryUI`) comme source de vérité ; le fallback a été **supprimé** du code — le wallet et le reste du layout se font **dans le prefab** sous `UIManager`.

### 3. Pourquoi un script Python (ou toute édition YAML en masse) est une fausse bonne idée

Extraire un prefab en **copiant des blocs YAML** depuis une scène (à la main ou par script) :

- contourne le pipeline Unity (**Prefab**, références, **guid**, ordre d’import) ;
- casse facilement au moindre changement de scène ;
- n’est **pas** tenable par un·e designer ou une revue standard ;
- peut introduire des erreurs subtiles (**Canvas imbriqué**, **sorting order** derrière le HUD, références cassées).

La voie **normale** reste : dans l’éditeur, **sélectionner la racine du panneau** (ex. `InventoryPanel` sous le canvas shell parent prévu) → **Créer un prefab** → assigner ce prefab à **`UIManager`** pour `ScreenId.Inventory`, **`autoCreateInventoryScreen` désactivé**.

### 4. Canvas et ordre d’affichage

Un prefab dont la racine est un **Canvas** avec son **propre** `sortingOrder` peut passer **sous** le canvas du HUD (`NavigationHUD`). Pour éviter la « bidouille », privilégier :

- racine = **Panel** (`RectTransform`) sous le **`screenRoot`** qui a déjà un **Canvas** parent, **ou**
- ajuster **override sorting** / hiérarchie selon la doc Unity pour canvas enfants.

À décider au moment du **design d’intégration** dans l’éditeur, pas via un extracteur de texte.

## Direction recommandée pour la prochaine passe (sans Python)

1. **Trancher une source unique** : soit prefab **unique** sous `UIManager` pour Inventory (maquette complète + wallet), soit scène additive dédiée — mais pas les deux en parallèle sans règle claire.
2. **Construire / dupliquer le prefab dans Unity uniquement** (drag & drop, Apply, variantes).
3. Si on garde provisoirement **`RuntimeInventoryScreen`** : y **instancier** le prefab **WalletBar** / widget déjà validé (références SerializeField ou injection depuis `UIManager`), **sans** régénérer de YAML externe.

---

*Le script `Tools/extract_inventory_prefab.py` a été retiré du dépôt : pas de génération d’UI par script Python dans ce workflow.*
