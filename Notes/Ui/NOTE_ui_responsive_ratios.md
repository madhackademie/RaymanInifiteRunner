# Note théorique — UI responsive (ratios / tailles d’écran)

**Création :** 2026-09-16  
**Statut :** brief théorique — implémentation **après** playtest shell  

**Action prioritaire auteur (2026-09-23) :** lire et exécuter **`Notes/Ui/NOTE_validation_ui_responsive_shell_popups.md`** — `[P0-UI-RESPONSIVE-VALID-001]` (NavigationHUD + popups ; **farm/zoom harvest hors scope**).

**Ensuite :** `[CT-UI-RESPONSIVE-SCAN-001]` — scanner les UI, classer déjà-responsive / à adapter / pixels verrouillés.  
**Déclencheur :** nav 140 + `WalletWidget` (offsets canvas fixes, pas liés à `NavBarContainer`).

Playtest HUD/wallet **avant** ce scan : `[P0-INV-WALLET-PLAY-001]`.

---

## 1) Ce que « responsive » veut dire ici

Portrait mobile, Canvas **Scale With Screen Size**, réf. **1080×1920**, `matchWidthOrHeight = 0,5` (`HUDRoot`).

| Mécanisme | Suit le ratio ? |
|-----------|-----------------|
| Ancres stretch `(0,0)–(1,1)` / bas `(0,0)–(1,0)` | Oui (bords + largeur) |
| `HorizontalLayoutGroup` + largeur parent | Oui en X |
| `sizeDelta` / `anchoredPosition` en px canvas | Non — taille constante après scaler |
| Constantes C# (`NavTabSlotHeight = 128`, `UIManager.NavBarHeight = 260`) | Non — même valeur sur 16:9 et 20:9 |
| `CanvasScaler` match 0,5 | Mise à l’échelle globale, pas un layout % |

Un écran **plus haut** (20:9) : plus d’espace **au-dessus** de la barre ; la barre **140** ne grandit pas.  
Un écran **plus trapu** : la barre prend plus de **%** de hauteur.

**Safe Area** (encoche, barre Android) : pas de convention projet aujourd’hui. Le scan doit le noter, pas l’inventer.

---

## 2) Constat déjà connu (nav + wallet, 2026-09-16)

Trois calages **indépendants** depuis le bas canvas :

| Surface | Valeur | Parent |
|---------|--------|--------|
| `NavBarContainer` | hauteur **140**, ancre bas stretch | `HUDRoot` |
| Slots onglets | **128** px, pivot bas (`LayoutNavTabSlot`) | barre |
| Écrans overlay | `offsetMin.y = 260` | `UIManager` |
| `WalletWidget` | **400×80**, Y **-40**, ancre bas-droite | `InventoryScreen` / `WalletBar` |

Le wallet **n’est pas enfant** de la nav (volontaire : off hors inventaire). Donc il **ne suit pas** un changement de hauteur de barre.

Canvas overlay `sortingOrder 60` sur le widget : au-dessus de la nav (canvas HUD **50**), parce que `NavBarContainer` est dernier sibling.

Mockup onglet actif (`scale 1.87`, lift, expand cadre) : **pixels verrouillés** — un stretch du slot casse le zoom (déjà vu : barre 140 qui gonflait l’icône).

---

## 3) Pistes théoriques (ne pas coder tant que le scan n’a pas classé)

1. **Une source de vérité bas d’écran** — constantes partagées (hauteur fond, hauteur slot, inset overlay, offset wallet) plutôt que 140 / 128 / 260 / -40 séparés.
2. **Ancres, pas d’offsets magiques** — barre = stretch bas + hauteur ; chip = même parent bas-droit **ou** % / LayoutElement, pas un Y calé sur un autre arbre.
3. **Layout groups** — rangée wallet (`CollapsedRow`) déjà en HLG : bon modèle ; éviter `sizeDelta` 400 si la largeur doit suivre l’écran.
4. **Scaler** — garder 1080×1920 ; éventuellement `match` plus proche de 0 (largeur) en portrait. Décision **après** playtest Game view 16:9 / 18:9 / 19.5:9 / 20:9 / 4:3.
5. **Safe Area** — padding bas/haut depuis `Screen.safeArea` **si** le scan le exige (hors V0 tant que SM-A137F playtest OK).
6. **Ne pas responsive-iser le zoom onglet** — garder slot 128 ; adapter seulement le **fond** et les insets.

Lien PA (déjà ancré haut-droite, taille fixe) : `Notes/Ui/CONVENTION_hud_pa_safe_zone.md`.

---

## 4) Scan — nouveau thread uniquement

**Ne pas** commencer le scan dans un chat HUD/wallet. Thread dédié après playtest.

### Périmètre à lister (ordre suggéré)

1. Shell : `NavigationHUD.unity` (nav, PA, `screenRoot`, croix ExitOnly)
2. Overlays `UIManager` : `InventoryScreen`, `ShopScreen`, `SaleChannelsScreen`, `FeaturesHubScreen` (Bottom 260)
3. Popups : `ShopItemPopup`, pipeline `ScreenPopupHost`
4. Farm world-space : `BiofiltreHud`, rows slots (Canvas World — autre règle que le shell)
5. Autres CanvasScaler du repo (FirstLvl, Bootstrap, popups factory)

### Grille de classement (remplir au scan)

| Écran / prefab | Ancres stretch ? | Px fixes ? | Safe area ? | Verdict |
|----------------|------------------|------------|-------------|---------|
| | oui / non / mixte | lister | oui/non | **déjà OK** · **adaptable** · **verrouillé mockup** |

### Hors scope du scan

- Changer des prefabs / YAML
- Jobs Bezy
- Recaler le wallet ou la nav « en passant »

### Livrable du thread scan

- Tableau rempli
- 3–5 reco (scaler, constante unique bas, Safe Area oui/non)
- **Pas** de code tant que l’auteur n’a pas validé

---

## 5) Recette playtest ratios (après scan, ou en même temps que le playtest wallet)

Game view : **16:9**, **18:9**, **19.5:9**, **20:9**, **4:3**.  
Chaque fois : barre collée bas, 5 onglets visibles, wallet inventaire (Y -40, `=` entre timbres et 275), overlay Bottom au-dessus du cadre zoomé, PA haut-droite.

---

## Références

- `Assets/Scenes/NavigationHUD.unity` — scaler + `NavBarContainer` 140
- `Assets/Scripts/UI/NavigationHUD.cs` — `NavTabSlotHeight`
- `Assets/Scripts/Systems/UIManager.cs` — `NavBarHeight = 260`
- `Assets/Prefabs/Ui/InventoryScreen.prefab` — `WalletWidget`
- Credits Bezy : reset le **30** (`Notes/Bezi/README_bezi.md`)
