# SPEC UI — Hub « Plus » & composition barre nav mobile

**Création :** 2026-09-09  
**Màj :** 2026-09-15 — contenu hub = **sous-onglets horizontaux** (frames en ligne, patron visuel onglet Vente / `SelectedFrame`), pas liste verticale seule.  
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
| Contenu Plus | **Un écran** `FeaturesHubScreen` : **barre secondaire** de sous-onglets (frames **en ligne**, même langage que `TabVente` / mockup nav) + **zone contenu** qui swap un panneau par sous-onglet |
| Liste verticale | **Réservée à l’intérieur** de chaque panneau (ex. fil Mail, liste quêtes) — pas comme navigation principale du hub |
| Extensibilité | Registre data-driven (`FeaturesHubTabId` + prefab panneau ou `ScreenId` enfant) — V0 placeholders « Bientôt » |
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

**Décision auteur 2026-09-15 :** navigation interne = **onglets en frames horizontaux** (comme l’onglet **Vente** en barre : `SelectedFrame`, glow optionnel, label sous l’icône ou à côté selon place). Scroll horizontal **seulement** si > 5–6 sous-onglets sur petit écran.

```
FeaturesHubScreen (root — RuntimeFeaturesHubScreen, backdrop)
├── Header (optionnel : titre "Plus" + Close — ou fermeture = onglet barre)
├── HubSubTabBar (HorizontalLayoutGroup, hauteur ~ nav cell ou légèrement plus bas)
│   ├── HubSubTab_Notifications   (Button + SelectedFrame + Icon + Label)
│   ├── HubSubTab_Mailbox
│   ├── HubSubTab_RoadmapVote
│   ├── HubSubTab_Craft
│   ├── HubSubTab_Quests
│   ├── HubSubTab_Settings
│   └── … (extensible)
└── HubContentViewport
    └── HubPanelHost (un seul panneau actif à la fois)
        ├── Panel_Notifications   (placeholder V0)
        ├── Panel_Mailbox
        ├── Panel_RoadmapVote
        ├── Panel_Craft           → contenu atelier ou lien `ScreenId.Craft`
        ├── Panel_Quests          (filtres daily / weekly / monthly en sous-barre ou chips)
        └── Panel_Settings        (langue, son, abo — stubs)
```

### Patron `HubSubTab` (copie **read-only** de `TabVente` / `TabInventaire` dans la scène nav)

- Même stack : `SelectedFrame`, `IconLift` / `Glow` / `Icon`, `Label` TMP.
- **Pas** de second `NavigationHUD` : composant dédié `FeaturesHubSubTabView` (Bezy wiring + Cursor logique sélection).
- Badge non-lu sur icône (Mail / Notifications) : petit TMP ou Image en coin.

**Règles :**

- Clic sous-onglet → activer frame + afficher le panneau correspondant (**pas** de `LoadScene`).
- Panneau lourd (Craft complet) : V1 peut ouvrir un `ScreenId` dédié par-dessus le hub ; V0 = placeholder dans `HubPanelHost`.
- Entrée **locked** : sous-onglet grisé + panneau « Bientôt ».

### Patron ligne (à l’intérieur d’un panneau Mail / Notifications)

```
FeatureHubRow (lecture seule, pas nav principale)
├── Icon + Label + date
└── OptionalBadge
```

Utiliser un `ScrollRect` **vertical** dans `Panel_Mailbox` / `Panel_Notifications` pour les listes longues.

---

## 8) Registre sous-onglets hub (proposition auteur 2026-09-15)

| Ordre barre | `FeaturesHubTabId` | Rôle | Statut V0 | Notes métier |
|------------:|--------------------|------|-----------|--------------|
| 1 | `Notifications` | Fil **nouveaux** events / updates (badge global) | stub UI | Peut fusionner avec Mail si trop redondant — **à trancher** |
| 2 | `Mailbox` | Messages **persistants** (patch notes, events, spam dev / outil éditeur) | stub + **dev inject** plus tard | Distinct si Notifications = alertes courtes, Mail = historique |
| 3 | `RoadmapVote` | Vote sur items roadmap communauté / backlog | locked | Backend TBD ; UI liste + boutons vote |
| 4 | `Craft` | Onglet **bricolage** / atelier | stub → `ScreenId.Craft` | `Notes/GDD/SPEC_craft_atelier_aquaponique.md` |
| 5 | `Quests` | Quêtes **daily / weekly / monthly** (+ chips ou 2ᵉ barre) | stub | `[BL-QUEST-DAILY-001]` · icônes H4–H7 |
| 6 | `Settings` | **Options** : langue, son, abonnement | stub | Futur `ScreenId.Settings` si écran plein |
| — | `Shop` | Entrée shop | actif **seulement variante A** (shop hors barre) | Sinon shop reste onglet barre B |

**Plus si affinité :** Social, Shop boulons, aide / FAQ — nouveaux `HubSubTab_*` sans toucher `NavigationHUD`.

Réf. craft : `Notes/GDD/SPEC_craft_atelier_aquaponique.md`  
Réf. quêtes : `[BL-QUEST-DAILY-001]` · icônes H4–H7 `PROMPT_generation_icones.md`

### Open design — Mail vs Notifications

| Option | Quand la choisir |
|--------|------------------|
| **A — 2 onglets** | Notifications = toasts agrégés + liste courte ; Mailbox = tout l’historique + contenu riche |
| **B — 1 onglet « Actualités »** | V0 plus simple ; split plus tard si le fil grossit |

**Recommandation V0 :** option **B** (un panneau, deux sections « Nouveau » / « Archives ») sauf besoin explicite de spam mailbox séparé.

---

## 9) Code — chantiers Cursor (après décision A/B)

| Fichier | Changement |
|---------|------------|
| `ScreenId.cs` | `public const string FeaturesHub = "FeaturesHub";` |
| `UIManager` | Binding écran + prefab `FeaturesHubScreen` |
| `NavigationHUD.cs` | `TabMoreOption` + `OnTabMoreOptionClicked` (**livré 2026-09-14**) ; masquer `TabShop` si variante A |
| `RuntimeFeaturesHubScreen.cs` | *(nouveau)* sélection sous-onglet, swap panneaux, badges non-lu |
| `FeaturesHubTabId.cs` | constantes stables (Notifications, Mailbox, …) |
| `FeatureHubTabDefinition.cs` | *(optionnel V1)* SO : tabId, label, icon, panelPrefab, unlocked |

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
| **Nav TabMoreOption** | `TabMoreOption` dans `NavigationHUD.unity` (copie patron `TabInventaire`) | Prompts `[BZ-TAB-MORE-001]` ; sprite H-nav-5 plus tard |
| **FeaturesHub Ph.1** | Shell + `HubSubTabBar` (2–3 tabs placeholder) + `HubPanelHost` vide | `[BZ-TAB-MORE-001]` OK |
| **FeaturesHub Ph.2** | Composants frames (copie patron `TabVente`) + panneaux placeholder | Ph.1 OK |
| **FeaturesHub Ph.3** | Wiring `RuntimeFeaturesHubScreen` + `UIManager` binding | Scripts Cursor |

Prompts détaillés : à rédiger dans `Notes/Ui/PROMPTS_Bezi_features_hub_plus.md` après validation auteur (gate même règle que onglets HUD).

---

## 12) Ordre de livraison recommandé

1. **Playtest mobile** §3 → cocher variante §2  
2. Finir **H-nav** barre (3 ou 4 icônes + Plus) + playtest layout  
3. Cursor : `ScreenId.FeaturesHub` + shell minimal  
4. Bezy : `TabMoreOption` + `FeaturesHubScreen` phases 1–3 (sous-onglets frames)  
5. Cursor : `FeaturesHubTabId`, contrôleur hub, stubs panneaux ; puis Quêtes / Craft / Settings au fil de l’eau  
6. Prompts : `Notes/Ui/PROMPTS_Bezi_features_hub_plus.md` (à rédiger avant Bezy hub)

---

## 13) Liens

- `Notes/Todo_project.md` — `[BL-UI-FEATURES-HUB-001]`
- `Notes/Ui/PROMPTS_Bezi_tab_sprites.md` — patron onglet + régression wallet `[P0-NAV-WALLET-REG-001]`
- `Notes/GDD/SPEC_craft_atelier_aquaponique.md`
- `Notes/GDD/SPEC_inventaire_multiverse_hub.md`
- `.cursor/rules/scene_ui_runtime.mdc` — `SceneNavigator`, `NavigationHUD`
