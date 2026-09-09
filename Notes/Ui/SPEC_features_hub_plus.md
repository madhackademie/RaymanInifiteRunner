# SPEC UI — Hub « Plus » & composition barre nav mobile

**Création :** 2026-09-09  
**Statut :** **brouillon** — variante barre (A vs B) à trancher après **playtest mobile auteur**  
**Backlog :** `[BL-UI-FEATURES-HUB-001]` · `Notes/Todo_project.md`  
**Art onglets :** `Notes/Art/PROMPT_generation_icones.md` § **Vague H-nav**  
**Nav runtime :** `Assets/Scripts/UI/NavigationHUD.cs` · scène `Assets/Scenes/NavigationHUD.unity`  
**Règles projet :** `SceneNavigator` / `UIManager` — pas de `LoadScene` depuis l’UI hub

---

## 1) Problème & objectif

La barre du bas ne doit pas accumuler **6–8 onglets** (Quêtes, Atelier, Mail, Social, shop boulons…) : icônes trop petites, art détaillé illisible, maintenance lourde.

**Objectif :**

1. **Barre fixe** : peu d’entrées **haute fréquence** (boucle gameplay).
2. **Écran Plus** : liste verticale scrollable de **boutons standard** (icône + label) pour tout le reste — extensible sans retoucher la barre.
3. **Playtest mobile** pour choisir entre **3 + Plus** ou **4 + Plus**.

**Hors scope :** nav par rotation / carousel / boutons qui « sortent » de l’écran en **nav primaire** (voir §6).

---

## 2) Variantes barre — A / B (à cocher après test)

### Tableau comparatif

| | **Variante A — 3 + Plus** *(recommandée pour test 1)* | **Variante B — 4 + Plus** |
|---|--------------------------------------------------------|---------------------------|
| **Slots barre** | 4 (3 + Plus) | 5 (4 + Plus) |
| **Largeur cellule** (~390 px) | ~**97 px** | ~**78 px** |
| **Onglets visibles** | Play · Inventaire · Vente · **Plus** | Play · Inventaire · Shop · Vente · **Plus** |
| **Dans le +** | Shop, Quêtes, Atelier, Mail, Social… | Quêtes, Atelier, Mail, Social… |
| **Boucle gameplay** | Play → Inv → Vente en **1 tap** | Idem + Shop en **1 tap** |
| **Art détaillé 128²** | **Meilleur** (cellules larges) | Correct, plus serré |
| **Monétisation shop** | Shop = **+1 tap** | Shop reste primaire |

### Variante C (référence — non retenue pour l’instant)

5 onglets fixes sans Plus (état actuel Bezy : Play, Inv, Shop, Vente + futur 5ᵉ). **Pas** scalable pour Quêtes / Atelier / Mail sans resserrer encore les icônes.

### Décision auteur (remplir après playtest)

```
[ ] Variante A — 3 + Plus
[ ] Variante B — 4 + Plus
[ ] Autre (préciser) : _______________
Date test : ____ / Device : __________ / Résolution : ______
```

---

## 3) Playtest mobile — checklist (15 min)

Cocher sur **téléphone réel** (pas seulement Game view Editor).

### Préparation rapide (sans refonte complète)

- Simuler **A** : masquer `TabShop` dans la scène (ou désactiver le GameObject) ; imaginer Shop dans le +.
- Simuler **B** : barre actuelle 4 onglets + futur `TabPlus`.
- Noter la **largeur RectTransform** d’une cellule onglet (Inspector) en résolution device.

### Critères

| # | Question | A (3+Plus) | B (4+Plus) |
|---|----------|:----------:|:----------:|
| 1 | Icône lisible au premier coup d’œil ? | ☐ | ☐ |
| 2 | Vente accessible sans friction ? | ☐ | ☐ |
| 3 | Shop : 1 tap de plus acceptable ? | ☐ | — |
| 4 | Barre « pas chargée » visuellement ? | ☐ | ☐ |
| 5 | Plus trouvable / compréhensible ? | ☐ | ☐ |
| 6 | Zone pouce confortable (bas écran) ? | ☐ | ☐ |

**Notes libres :**

```
…
```

---

## 4) Décisions architecture (figées sauf variante barre)

| Sujet | Décision |
|-------|----------|
| Hub features | **Écran UI** `ScreenId.FeaturesHub` — **pas** de scène `.unity` dédiée |
| Ouverture | Onglet **Plus** → `UIManager.TryShowScreen(ScreenId.FeaturesHub)` |
| Fermeture | Bouton retour / backdrop — même pattern `Inventory` / `SaleChannels` |
| Contenu Plus | `ScrollView` vertical, **lignes bouton** (pas mini-onglets) |
| Extensibilité | Registre data-driven (ScriptableObject ou liste sérialisée) — V1 peut être liste en dur |
| Multiverse / runner | Reste sur **Play (Aventures)** — pas dans le Plus |
| Market cloud | Hors barre — spec cloud existante ; ne pas confondre avec Shop HUD |
| Prefabs UI | **Bezy** (`FeaturesHubScreen.prefab`, lignes) ; **Cursor** = scripts, `ScreenId`, navigation |
| Popups | Features qui ouvrent un écran = `ScreenId` ; confirmations = pipeline `PopupId` + `ScreenPopupHost` |

---

## 5) Composition cible selon variante

### Variante A — barre

```
[ Play ]  [ Inventaire ]  [ Vente ]  [ Plus ]
```

| Onglet | `ScreenId` / action | Icône art |
|--------|---------------------|-----------|
| Play | Hub Aventures / `SceneNavigator` | `IconeTab_Aventures_128` (H-nav-1) |
| Inventaire | `ScreenId.Inventory` | H-nav-2 |
| Vente | `ScreenId.SaleChannels` | H-nav-4 |
| Plus | `ScreenId.FeaturesHub` | H-nav-5 |

`TabShop` : **masqué** ou retiré de la barre ; entrée **Shop** dans le Plus.

### Variante B — barre

```
[ Play ]  [ Inventaire ]  [ Shop ]  [ Vente ]  [ Plus ]
```

| Onglet | `ScreenId` | Icône art |
|--------|------------|-----------|
| Play | Hub Aventures | H-nav-1 |
| Inventaire | `ScreenId.Inventory` | H-nav-2 |
| Shop | `ScreenId.Shop` | H-nav-3 |
| Vente | `ScreenId.SaleChannels` | H-nav-4 |
| Plus | `ScreenId.FeaturesHub` | H-nav-5 |

---

## 6) Patterns UX — à éviter / autorisé

| Pattern | Nav primaire (barre) | Écran Plus |
|---------|----------------------|------------|
| Barre fixe égale (`HorizontalLayoutGroup`) | **Oui** | — |
| Icône stretch + `Preserve Aspect` + cadre actif | **Oui** (livré Bezy) | — |
| Barre horizontale **scrollable** | **Non** | — |
| Boutons en **rotation / arc / carousel** | **Non** | **Non** pour la liste principale |
| Slide-up / fade à l’ouverture du panneau Plus | — | **Oui** (léger, optionnel Bezy Animator) |
| Menu radial depuis le bouton Plus seul | — | Optionnel phase 2, pas V0 |

---

## 7) Écran `FeaturesHubScreen` — hiérarchie cible (Bezy)

```
FeaturesHubScreen (root — RuntimeFeaturesHubScreen, Image backdrop semi-opaque)
├── Header
│   ├── TitleLabel ("Plus" / "Menu")
│   └── CloseButton
└── Body
    └── FeaturesScrollView (ScrollRect vertical)
        └── FeaturesContent (VerticalLayoutGroup, spacing ~12–16)
            ├── FeatureHubRowButton (Quêtes)
            ├── FeatureHubRowButton (Atelier craft)
            ├── FeatureHubRowButton (Shop)          ← seulement si variante A
            ├── FeatureHubRowButton (Mail — locked)
            ├── FeatureHubRowButton (Social — locked)
            └── …
```

### Ligne `FeatureHubRowButton` (patron)

```
FeatureHubRowButton (Button, hauteur fixe ~72–88 px, stretch horizontal)
├── Icon (Image 64–72 px, Preserve Aspect)
├── Label (TMP_Text)
├── OptionalBadge (ex. "Bientôt", compteur quêtes)
└── Chevron (optionnel)
```

**Règles :**

- Une ligne = une feature ; clic → `UIManager.TryShowScreen(targetScreenId)` puis fermer ou laisser le hub en arrière-plan selon pattern Inventory (à aligner sur comportement actuel).
- Entrée **locked** : `interactable = false` + overlay gris + label « Bientôt ».
- **Pas** de sous-onglets dans le Plus en V0.

---

## 8) Registre entrées V1 (proposition)

| Ordre | Label FR | `ScreenId` cible | Statut V0 | Art icône |
|------:|----------|------------------|-----------|-----------|
| 1 | Quêtes | *(futur `Quests`)* | locked ou stub | H4 backlog |
| 2 | Atelier | *(futur `Craft`)* | locked ou stub | G5 / craft spec |
| 3 | Shop | `Shop` | actif **si variante A** | H-nav-3 ou icône liste |
| 4 | Mail | — | locked | backlog |
| 5 | Social | — | locked | backlog |
| 6 | Shop boulons | *(futur)* | locked | H8 backlog |

Réf. craft : `Notes/GDD/SPEC_craft_atelier_aquaponique.md`  
Réf. quêtes : `[BL-QUEST-DAILY-001]` · icônes H4–H7 `PROMPT_generation_icones.md`

---

## 9) Code — chantiers Cursor (après décision A/B)

| Fichier | Changement |
|---------|------------|
| `ScreenId.cs` | `public const string FeaturesHub = "FeaturesHub";` |
| `UIManager` | Binding écran + prefab `FeaturesHubScreen` |
| `NavigationHUD.cs` | `TabPlus` + `OnTabPlusClicked` ; masquer `TabShop` si variante A ; enum `Tab` étendu |
| `RuntimeFeaturesHubScreen.cs` | *(nouveau)* liste entrées, navigation, états locked |
| `FeatureHubEntryDefinition.cs` | *(optionnel V1)* SO : id, label, icon, screenId, unlocked |

**Comportement Plus :**

- Clic Plus → ouvrir `FeaturesHub` ; onglet Plus = actif (cadre or) tant que l’écran est visible.
- Clic autre onglet barre → fermer `FeaturesHub` + écrans modaux existants (`HideOtherModalScreens` étendu).

---

## 10) Art — onglets barre (`Vague H-nav`)

| ID | Fichier promo | Variante A | Variante B |
|----|---------------|:----------:|:----------:|
| H-nav-1 Play | `Sprites/UI/Nav/Tabs/IconeTab_Aventures_128.png` | barre | barre |
| H-nav-2 Inventaire | `IconeTab_Inventaire_128.png` | barre | barre |
| H-nav-3 Shop | `IconeTab_Shop_128.png` | **ligne Plus** | barre |
| H-nav-4 Vente | `IconeTab_Vente_128.png` | barre | barre |
| H-nav-5 Plus | `IconeTab_Plus_128.png` | barre | barre |

Brief : sujet **85–90 %** du cadre 128², même poids visuel, Trim à l’import.

---

## 11) Bezy — file d’attente (après playtest + promo sprites)

| Phase | Livrable | Prérequis |
|-------|----------|-----------|
| **Nav TabPlus** | `TabPlus` dans `NavigationHUD.unity` (copie patron `TabAventures`) | Décision A ou B ; H-nav-5 |
| **FeaturesHub Ph.1** | Hiérarchie `FeaturesHubScreen` shell + scroll + 3 lignes placeholder | Spec validée |
| **FeaturesHub Ph.2** | Image, Button, TMP, Layout sur lignes | Ph.1 OK |
| **FeaturesHub Ph.3** | Wiring `RuntimeFeaturesHubScreen` + `UIManager` | Scripts Cursor |

Prompts détaillés : à rédiger dans `Notes/Ui/PROMPTS_Bezi_features_hub_plus.md` après validation auteur (gate même règle que onglets HUD).

---

## 12) Ordre de livraison recommandé

1. **Playtest mobile** §3 → cocher variante §2  
2. Finir **H-nav** barre (3 ou 4 icônes + Plus) + playtest layout  
3. Cursor : `ScreenId.FeaturesHub` + shell minimal  
4. Bezy : `TabPlus` + `FeaturesHubScreen` phases 1–3  
5. Remplir lignes V1 au fur et à mesure (Shop si A, Quêtes, Craft)

---

## 13) Liens

- `Notes/Todo_project.md` — `[BL-UI-FEATURES-HUB-001]`
- `Notes/Ui/PROMPTS_Bezi_tab_sprites.md` — patron onglet + régression wallet `[P0-NAV-WALLET-REG-001]`
- `Notes/GDD/SPEC_craft_atelier_aquaponique.md`
- `Notes/GDD/SPEC_inventaire_multiverse_hub.md`
- `.cursor/rules/scene_ui_runtime.mdc` — `SceneNavigator`, `NavigationHUD`
